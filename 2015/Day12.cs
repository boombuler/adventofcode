using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AdventOfCode._2015
{
    class Day12 : Solution
    {
        private static Regex FindNumbers = new Regex(@"\-?\d+");

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

        private long GetSum(Func<JsonElement, bool> filter)
            => Walk(JsonDocument.Parse(Input).RootElement, filter).Sum();

        protected override long? Part1() 
            => GetSum(_ => true);
        protected override long? Part2() 
            => GetSum(js => js.EnumerateObject().Any(jp => jp.Value.ValueKind == JsonValueKind.String && jp.Value.GetString() == "red"));
    }
}
