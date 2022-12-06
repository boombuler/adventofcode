namespace AdventOfCode._2020;

class Day21 : Solution<string>
{
    private static long CountAllergenFreeIngredients(string input)
    {
        var (ingredients, allergens) = LoadAllergenList(input);
        return ingredients.Keys.Except(allergens.Select(a => a.Ingredient)).Sum(i => ingredients[i]);
    }

    private static string GetDangerousList(string input)
    {
        (_, var allergens) = LoadAllergenList(input);
        return string.Join(",", allergens.OrderBy(a => a.Allergen).Select(a => a.Ingredient));
    }

    private static (Dictionary<string, int> Ingredients, List<(string Allergen, string Ingredient)> Allergens) LoadAllergenList(string input)
    {
        var recieps = input.Lines().Select(line => line.Split(" (contains ")).Select(itms => new
        {
            Ingredients = itms[0].Split(' '),
            Allergens = itms[1].Trim(')').Split(", ").ToArray()
        }).ToList();

        var unassignedAllergens = recieps.SelectMany(r =>
            r.Allergens.Select(a => new { Allergen = a, Ingrs = r.Ingredients })
        ).GroupBy(e => e.Allergen).ToDictionary(grp => grp.Key, grp =>
            new HashSet<string>(grp.Select(g => (IEnumerable<string>)g.Ingrs).Aggregate((a, b) => a.Intersect(b)))
        );

        var assigned = new List<(string Allergen, string Ingredient)>();
        var unassignedIngredients = new HashSet<string>(unassignedAllergens.SelectMany(a => a.Value));
        while (unassignedAllergens.Count > 0)
        {
            var assign = unassignedAllergens
                .Select(kvp => new { Allergen = kvp.Key, Candidates = kvp.Value.Intersect(unassignedIngredients).ToList() })
                .First(a => a.Candidates.Count == 1);
            var ingr = assign.Candidates.Single();
            assigned.Add((assign.Allergen, ingr));
            unassignedIngredients.Remove(ingr);
            unassignedAllergens.Remove(assign.Allergen);
        }

        return (
            recieps.SelectMany(r => r.Ingredients).GroupBy(i => i).ToDictionary(g => g.Key, g => g.Count()),
            assigned
        );
    }

    protected override string Part1()
    {
        Assert(CountAllergenFreeIngredients(Sample()), 5);
        return CountAllergenFreeIngredients(Input).ToString();
    }

    protected override string Part2()
    {
        Assert(GetDangerousList(Sample()), "mxmxvkd,sqjhc,fvjkl");
        return GetDangerousList(Input);
    }
}
