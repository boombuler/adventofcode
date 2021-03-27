using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode._2018
{
    class Day19 : Solution
    {
        /*        delegate void OpCodeImpl(int a, int b, int c, int[] registers);

                private static void Set(int[] registers, int c, int val) => registers[c] = val;
                private static OpCodeImpl BinOpR(Func<int, int, int> calc) => (int a, int b, int c, int[] registers) => Set(registers, c, calc(registers[a], registers[b]));
                private static OpCodeImpl BinOpI(Func<int, int, int> calc) => (int a, int b, int c, int[] registers) => Set(registers, c, calc(registers[a], b));

                private static readonly ReadOnlyDictionary<string, OpCodeImpl> OpCodes = new ReadOnlyDictionary<string, OpCodeImpl>(new Dictionary<string, OpCodeImpl>(StringComparer.OrdinalIgnoreCase)
                {
                    {"addr", BinOpR((a, b) => a + b) },
                    {"addi", BinOpI((a, b) => a + b) },
                    {"mulr", BinOpR((a, b) => a * b) },
                    {"muli", BinOpI((a, b) => a * b) },
                    {"banr", BinOpR((a, b) => a & b) },
                    {"bani", BinOpI((a, b) => a & b) },
                    {"borr", BinOpR((a, b) => a | b) },
                    {"bori", BinOpI((a, b) => a | b) },
                    {"setr", (a, b, c, registers) => Set(registers, c, registers[a]) },
                    {"seti", (a, b, c, registers) => Set(registers, c, a) },
                    {"gtir", (a, b, c, registers) => Set(registers, c, a > registers[b] ? 1 : 0)},
                    {"gtri", (a, b, c, registers) => Set(registers, c, registers[a] > b ? 1 : 0)},
                    {"gtrr", (a, b, c, registers) => Set(registers, c, registers[a] > registers[b] ? 1 : 0)},
                    {"eqir", (a, b, c, registers) => Set(registers, c, a == registers[b] ? 1 : 0)},
                    {"eqri", (a, b, c, registers) => Set(registers, c, registers[a] == b ? 1 : 0)},
                    {"eqrr", (a, b, c, registers) => Set(registers, c, registers[a] == registers[b] ? 1 : 0)},
                });*/

        record OpCodeLine(string OpCode, int a, int b, int c)
        {
            public static Func<string, OpCodeLine> Factory = new Regex(@"(?<OpCode>\w{4}) (?<a>\d+) (?<b>\d+) (?<c>\d+)").ToFactory<OpCodeLine>();
        }

        private string OptimizeCode(string code)
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

        private int RunCompiled(string code, int initialR0 = 0)
        {
            code = OptimizeCode(code);
            var (ipDef, other) = code.Lines();
            var rIP = ipDef.Last() - '0';
            var opCodesDefs = other.Select(OpCodeLine.Factory).ToList();
            var dm = new DynamicMethod("RunCode", typeof(int), null);
            var il = dm.GetILGenerator();
            var registers = Enumerable.Range(0, 6).Select(i => il.DeclareLocal(typeof(int)).LocalIndex).ToArray();
            var labels = opCodesDefs.Select(o => il.DefineLabel()).ToArray();
            var exit = il.DefineLabel();

            il.Emit(OpCodes.Ldc_I4, initialR0);
            il.Emit(OpCodes.Stloc_S, registers[0]);
            il.Emit(OpCodes.Ldc_I4, -1);
            il.Emit(OpCodes.Stloc_S, registers[rIP]);

            void ExecuteJump()
            {
                il.Emit(OpCodes.Ldloc_S, registers[rIP]);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Switch, labels);
                il.Emit(OpCodes.Br, exit);
            }

            for (int i = 0; i < opCodesDefs.Count; i++)
            {
                // Line[n]: reg[ip] = IP;
                il.MarkLabel(labels[i]);
                il.Emit(OpCodes.Ldc_I4_S, (byte)i);
                il.Emit(OpCodes.Stloc_S, registers[rIP]);
                
                var op = opCodesDefs[i];
                void LdRR()
                {
                    il.Emit(OpCodes.Ldloc_S, registers[op.a]);
                    il.Emit(OpCodes.Ldloc_S, registers[op.b]);
                }
                void LdRI()
                {
                    il.Emit(OpCodes.Ldloc_S, registers[op.a]);
                    il.Emit(OpCodes.Ldc_I4, op.b);
                }
                void LdIR()
                {
                    il.Emit(OpCodes.Ldc_I4, op.a);
                    il.Emit(OpCodes.Ldloc_S, registers[op.b]);
                }

                void Apply(OpCode opcode, Action EmitLD)
                {
                    EmitLD();
                    il.Emit(opcode);
                    il.Emit(OpCodes.Stloc_S, registers[op.c]);
                }

                switch (op.OpCode.ToLower())
                {
                    case "seti":
                        // rc = a
                        il.Emit(OpCodes.Ldc_I4, op.a);
                        il.Emit(OpCodes.Stloc_S, registers[op.c]);
                        break;
                    case "setr":
                        // rc = ra
                        il.Emit(OpCodes.Ldloc_S, registers[op.a]);
                        il.Emit(OpCodes.Stloc_S, registers[op.c]);
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
                    case "remr":
                        // (ra % rb) > 0 ? 1 : 0
                        LdRR();
                        il.Emit(OpCodes.Rem);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Cgt);
                        il.Emit(OpCodes.Stloc_S, registers[op.c]);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (op.c == rIP)
                    ExecuteJump();
            }

            il.MarkLabel(exit);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            var result = (int)dm.Invoke(null, null);
            return result;
        }

        protected override long? Part1()
        {
            Assert(RunCompiled(Sample()), 6);
            return RunCompiled(Input);
        }

        protected override long? Part2() => RunCompiled(Input, 1);
    }
}
