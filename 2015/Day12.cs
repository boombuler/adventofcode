namespace AdventOfCode._2015;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

class Day12 : Solution
{
    public IEnumerable<long> Walk(JsonElement json, Func<JsonElement, bool> filterObject)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.Object:
                if (filterObject(json))
                {
                    foreach (var itm in json.EnumerateObject().SelectMany(p => Walk(p.Value, filterObject)))
                        yield return itm;
                }
                break;
            case JsonValueKind.Array:
                foreach (var itm in json.EnumerateArray().SelectMany(j => Walk(j, filterObject)))
                    yield return itm;

                break;
            case JsonValueKind.Number:
                yield return json.GetInt64();
                break;
        }
    }

    private long GetSum(string input, Func<JsonElement, bool> filter)
        => Walk(JsonDocument.Parse(input).RootElement, filter).Sum();

    protected override long? Part1()
    {
        long Sum(string s)
            => GetSum(s, _ => true);
        Assert(Sum("[1,2,3]"), 6);
        Assert(Sum("{\"a\":{\"b\":4},\"c\":-1}"), 3);
        return Sum(Input);
    }
    protected override long? Part2()
    {
        long Sum(string s) =>
            GetSum(s, js => !js.EnumerateObject().Any(jp => jp.Value.ValueKind == JsonValueKind.String && jp.Value.GetString() == "red"));
        Assert(Sum("[1,{\"c\":\"red\",\"b\":2},3]"), 4);
        return Sum(Input);
    }
}
