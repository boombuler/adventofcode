namespace AdventOfCode.Utils;

class InvalidInputException(string? message = null) : Exception(message ?? "The given puzzle input does not match the required format!")
{
}
