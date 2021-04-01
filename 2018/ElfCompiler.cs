using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode._2018
{
    static class ElfCompiler
    {
        public class Registers : IRegisters
        {
            const int REGISTER_COUNT = 6;

            private readonly int[] Values = new int[REGISTER_COUNT];
            public virtual int this[int r]
            {
                get => Values[r];
                set => Values[r] = value;
            }
        }

        public interface IRegisters
        {
            int this[int r] { get; set; }
        }

        public record OpCodeLine(string OpCode, int a, int b, int c)
        {
            
        }
        private static Func<string, OpCodeLine> OpCodeLineFactory = new Regex(@"(?<OpCode>\w{4}) (?<a>\d+) (?<b>\d+) (?<c>\d+)").ToFactory<OpCodeLine>();

        private static string OptimizeCode(string code)
        {
            var opti = new Regex(string.Join("\\n", @"seti 1 \d (?<i>\d)
mulr (?<factor>\d) \k<i> (?<tmp>\d)
eqrr \k<tmp> (?<num>\d) \k<tmp>
addr \k<tmp> (?<ip>\d) \k<ip>
addi \k<ip> 1 \k<ip>
addr \k<factor> (?<result>\d) \k<result>
addi \k<i> 1 \k<i>
gtrr \k<i> \k<num> \k<tmp>
addr \k<ip> \k<tmp> \k<ip>
seti 2 \d+ \k<ip>".Lines()), RegexOptions.Multiline | RegexOptions.IgnoreCase);

            return opti.Replace(code, @"eqri ${factor} 0 ${tmp}
muli ${tmp} 6 ${tmp}
addr ${ip} ${tmp} ${ip} 
remr ${num} ${factor} ${tmp}
addr ${ip} ${tmp} ${ip}
addr ${result} ${factor} ${result}
seti 0 0 ${tmp}
setr ${num} 0 ${i}
addi 0 0 0
addi 0 0 0");
        }

        public static (int rIP, OpCodeLine[]) Parse(string code)
        {
            var (ipDef, other) = code.Lines();
            var rIP = ipDef.Last() - '0';
            return (rIP, other.Select(OpCodeLineFactory).ToArray());
        }

        public static Action<IRegisters> CompileCode(string code, bool Optimize = true)
        {
            code = OptimizeCode(code);
            var (ipDef, other) = code.Lines();
            var (rIP, opCodesDefs) = Parse(code);
            var dm = new DynamicMethod("RunCode", null, new Type[] { typeof(IRegisters) });
            var il = dm.GetILGenerator();

            il.DeclareLocal(typeof(int));
            
            var labels = opCodesDefs.Select(o => il.DefineLabel()).ToArray();
            var exit = il.DefineLabel();

            var getRegMember = typeof(IRegisters).GetMethod("get_Item");
            void LdReg(int rNo)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4_S, rNo);
                il.Emit(OpCodes.Callvirt, getRegMember);
            }

            var setRegMember = typeof(IRegisters).GetMethod("set_Item");
            void StoReg(int rNo)
            {
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4_S, rNo);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Callvirt, setRegMember);
            }

            void ExecuteJump()
            {
                LdReg(rIP);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Switch, labels);
                il.Emit(OpCodes.Br, exit);
            }


            il.Emit(OpCodes.Ldc_I4, -1);
            StoReg(rIP);

            for (int i = 0; i < opCodesDefs.Length; i++)
            {
                // Line[n]: reg[ip] = IP;
                il.MarkLabel(labels[i]);
                il.Emit(OpCodes.Ldc_I4_S, (byte)i);
                StoReg(rIP);

                var op = opCodesDefs[i];
                void LdRR()
                {
                    LdReg(op.a); LdReg(op.b);
                }
                void LdRI()
                {
                    LdReg(op.a);
                    il.Emit(OpCodes.Ldc_I4, op.b);
                }
                void LdIR()
                {
                    il.Emit(OpCodes.Ldc_I4, op.a);
                    LdReg(op.b);
                }

                void Apply(OpCode opcode, Action EmitLD)
                {
                    EmitLD();
                    il.Emit(opcode);
                    StoReg(op.c);
                }

                switch (op.OpCode.ToLower())
                {
                    case "seti":
                        // rc = a
                        il.Emit(OpCodes.Ldc_I4, op.a);
                        StoReg(op.c);
                        break;
                    case "setr":
                        // rc = ra
                        LdReg(op.a);
                        StoReg(op.c);
                        break;
                    case "addi": Apply(OpCodes.Add, LdRI); break;
                    case "addr": Apply(OpCodes.Add, LdRR); break;
                    case "mulr": Apply(OpCodes.Mul, LdRR); break;
                    case "muli": Apply(OpCodes.Mul, LdRI); break;
                    case "eqrr": Apply(OpCodes.Ceq, LdRR); break;
                    case "eqri": Apply(OpCodes.Ceq, LdRI); break;
                    case "eqir": Apply(OpCodes.Ceq, LdIR); break;
                    case "banr": Apply(OpCodes.And, LdRR); break;
                    case "bani": Apply(OpCodes.And, LdRI); break;
                    case "borr": Apply(OpCodes.Or, LdRR); break;
                    case "bori": Apply(OpCodes.Or, LdRI); break;
                    case "gtrr": Apply(OpCodes.Cgt, LdRR); break;
                    case "gtri": Apply(OpCodes.Cgt, LdRI); break;
                    case "gtir": Apply(OpCodes.Cgt, LdIR); break;
                    case "remr":
                        // (ra % rb) > 0 ? 1 : 0
                        LdRR();
                        il.Emit(OpCodes.Rem);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Cgt);
                        StoReg(op.c);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (op.c == rIP)
                    ExecuteJump();
            }


            il.MarkLabel(exit);
            il.Emit(OpCodes.Ret);
            return (Action<IRegisters>)dm.CreateDelegate(typeof(Action<IRegisters>));
        }
    }
}
