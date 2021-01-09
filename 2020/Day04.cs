using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2020
{
    class Day04 : Solution
    {
        private const string FLD_BirthYear = "byr";
        private const string FLD_IssueYear = "iyr";
        private const string FLD_ExpirationYear = "eyr";
        private const string FLD_Height = "hgt";
        private const string FLD_HairColor = "hcl";
        private const string FLD_EyeColor = "ecl";
        private const string FLD_PassportID = "pid";
        private const string FLD_CountryID = "cid";
        
        private static readonly Regex ParseKeyValuePair = new Regex(@"(^|\W)(?<key>\w{3})\:(?<value>\S*)", RegexOptions.Compiled);
        private static readonly Regex ParseHeight = new Regex(@"(?<value>\d+)(?<unit>in|cm)", RegexOptions.Compiled);

        private static readonly Dictionary<string, Func<string, bool>> ValidationRules = new Dictionary<string, Func<string, bool>> {
            { FLD_BirthYear, CheckYear(1920, 2002) },
            { FLD_IssueYear, CheckYear(2010, 2020) },
            { FLD_ExpirationYear, CheckYear(2020, 2030) },
            { FLD_Height, CheckHeight },
            { FLD_HairColor, CheckRegex(@"^#[0-9a-f]{6}$") },
            { FLD_EyeColor, CheckRegex(@"^(amb|blu|brn|gry|grn|hzl|oth)$") },
            { FLD_PassportID, CheckRegex(@"^\d{9}$") },
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

            switch(match.Groups["unit"].Value)
            {
                // If cm, the number must be at least 150 and at most 193.
                case "cm": return (value >= 150) && (value <= 193);
                // If in, the number must be at least 59 and at most 76.
                case "in": return (value >= 59) && (value <= 76); 
            }
            return false;
        }
        
        private IEnumerable<Dictionary<string, string>> ReadPassports(string passports)
        {
            var result = new Dictionary<string, string>();
            foreach(var line in passports.Lines())
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (result.Count > 0)
                    {
                        yield return result;
                        result = new Dictionary<string, string>();
                    }

                    continue;
                }

                foreach(Match match in ParseKeyValuePair.Matches(line))
                {
                    result[match.Groups["key"].Value] = match.Groups["value"].Value;
                }
            }
            if (result.Count > 0)
                yield return result;
        }

        private int CountPassports(string batchFile, Func<Dictionary<string, string>, bool> validation)
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
}
