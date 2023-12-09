namespace AdventOfCode._2020;

class Day04 : Solution
{
    private const string FLD_BIRTHYEAR = "byr";
    private const string FLD_ISSUEYEAR = "iyr";
    private const string FLD_EXPIRATIONYEAR = "eyr";
    private const string FLD_HEIGHT = "hgt";
    private const string FLD_HAIRCOLOR = "hcl";
    private const string FLD_EYECOLOR = "ecl";
    private const string FLD_PASSPORTID = "pid";

    private static readonly Regex ParseKeyValuePair = new(@"(^|\W)(?<key>\w{3})\:(?<value>\S*)", RegexOptions.Compiled);
    private static readonly Regex ParseHeight = new(@"(?<value>\d+)(?<unit>in|cm)", RegexOptions.Compiled);

    private static readonly Dictionary<string, Func<string, bool>> ValidationRules = new() {
        { FLD_BIRTHYEAR, CheckYear(1920, 2002) },
        { FLD_ISSUEYEAR, CheckYear(2010, 2020) },
        { FLD_EXPIRATIONYEAR, CheckYear(2020, 2030) },
        { FLD_HEIGHT, CheckHeight },
        { FLD_HAIRCOLOR, CheckRegex(@"^#[0-9a-f]{6}$") },
        { FLD_EYECOLOR, CheckRegex(@"^(amb|blu|brn|gry|grn|hzl|oth)$") },
        { FLD_PASSPORTID, CheckRegex(@"^\d{9}$") },
    };

    private static Func<string, bool> CheckYear(int min, int max)
        => (strVal) =>
        {
            if (strVal.Length == 4 && int.TryParse(strVal, out int val))
                return (val >= min) && (val <= max);
            return false;
        };

    private static Func<string, bool> CheckRegex(string pattern)
    {
        var re = new Regex(pattern, RegexOptions.Compiled);
        return (val) => re.IsMatch(val);
    }

    private static bool CheckHeight(string val)
    {
        var match = ParseHeight.Match(val);
        if (!match.Success)
            return false;
        if (!int.TryParse(match.Groups["value"].Value, out int value))
            return false;

        return match.Groups["unit"].Value switch
        {
            // If cm, the number must be at least 150 and at most 193.
            "cm" => value is >= 150 and <= 193,
            // If in, the number must be at least 59 and at most 76.
            "in" => value is >= 59 and <= 76,
            _ => false,
        };
    }

    private static IEnumerable<Dictionary<string, string>> ReadPassports(string passports)
    {
        var result = new Dictionary<string, string>();
        foreach (var line in passports.Lines())
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (result.Count > 0)
                {
                    yield return result;
                    result = [];
                }

                continue;
            }

            foreach (var match in ParseKeyValuePair.Matches(line).Cast<Match>())
            {
                result[match.Groups["key"].Value] = match.Groups["value"].Value;
            }
        }
        if (result.Count > 0)
            yield return result;
    }

    private static int CountPassports(string batchFile, Func<Dictionary<string, string>, bool> validation)
        => ReadPassports(batchFile)
            .Where(validation)
            .Count();

    private bool HasRequiredFields(Dictionary<string, string> passport)
        => !ValidationRules.Keys.Any(fld => !passport.ContainsKey(fld));

    private bool IsPassportValid(Dictionary<string, string> passport)
    {
        foreach (var validationRule in ValidationRules)
        {
            if (!passport.TryGetValue(validationRule.Key, out string value))
                return false;
            if (!validationRule.Value(value))
                return false;
        }
        return true;
    }
    protected override long? Part1()
    {
        Assert(CountPassports(Sample(), HasRequiredFields), 2);
        return CountPassports(Input, HasRequiredFields);
    }

    protected override long? Part2()
    {
        Assert(CountPassports(Sample("Invalid"), IsPassportValid), 0, "Invalid");
        Assert(CountPassports(Sample("Valid"), IsPassportValid), 4, "Valid");
        Assert(CountPassports(Sample(), IsPassportValid), 2);
        return CountPassports(Input, IsPassportValid);
    }
}
