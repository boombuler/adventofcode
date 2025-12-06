namespace AdventOfCode.Utils;

using System.Numerics;

static class Operators
{
    public static Func<T, T, T> ToBinaryOperator<T>(this char c) where T : INumber<T>
        => c switch 
        { 
            '+' => (a, b) => a + b, 
            '-' => (a, b) => a - b, 
            '*' => (a, b) => a * b, 
            '/' => (a, b) => a / b, 
            _ => throw new NotImplementedException($"Operator '{c}' is not implemented for type {typeof(T).Name}.") 
        };
}
