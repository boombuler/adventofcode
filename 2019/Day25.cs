namespace AdventOfCode._2019;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

class Day25 : Solution
{
    private readonly ImmutableHashSet<string> ITEM_BLACKLIST = new string[]
    {
            "molten lava", "photons", "giant electromagnet", "infinite loop", "escape pod"
    }.ToImmutableHashSet();
    private const string CHECKPOINT = "Security Checkpoint";

    [Flags]
    enum Door { North = 1, East = 2, South = 4, West = 8 };
    record Room(string Name, string Lore, Door Doors, ImmutableList<string> Items);

    private static IEnumerable<Room> ParseRooms(IEnumerable<string> Input)
    {
        var inp = Input.GetEnumerator();
        string buffer = null;
        string NextLine()
        {
            if (buffer != null)
            {
                var b = buffer;
                buffer = null;
                return b;
            }
            while (inp.MoveNext() && string.IsNullOrEmpty(inp.Current))
                /* do nothing */;
            return inp.Current;
        }
        bool ListItem(out string s)
        {
            if (inp.MoveNext() && !string.IsNullOrEmpty(inp.Current))
            {
                s = inp.Current.Trim().TrimStart('-', ' ');
                return true;
            }
            s = null;
            return false;
        }

        while (inp.MoveNext())
        {
            string name = NextLine();
            if (name == null)
                continue;
            while (!name.StartsWith('='))
                name = NextLine();

            name = name.Trim('=', ' ');
            string lore = NextLine();
            var doors = (Door)0;
            var items = ImmutableList<string>.Empty;
            var next = NextLine();
            while (next is not null and not "Command?")
            {
                if (next == "Doors here lead:")
                {
                    while (ListItem(out string s))
                        doors |= (Door)Enum.Parse(typeof(Door), s, true);
                }
                else if (next == "Items here:")
                {
                    while (ListItem(out string s))
                        items = items.Add(s);
                }
                else
                {
                    if (next.StartsWith("="))
                    {
                        buffer = next;
                        break;
                    }
                    lore = lore + "\n" + next;
                }
                next = NextLine();
            }
            yield return new Room(name, lore, doors, items);
        }
    }

    private (HashSet<string> Items, Func<IEnumerable<string>, long?> CheckInput) ExploreMap()
    {
        var vm = new IntCodeVM(Input);

        Door Other(Door d) => d switch
        {
            Door.South => Door.North,
            Door.North => Door.South,
            Door.East => Door.West,
            _ => Door.East
        };

        var allItems = new HashSet<string>();

        (ImmutableList<string> gatherItems, ImmutableList<string> goToCheckPoint) Explore(Door src, ImmutableList<string> gatherItems, ImmutableList<string> wp)
        {
            var room = ParseRooms(vm.RunASCIICommands(gatherItems).Select(n => n.Result)).Last();
            ImmutableList<string> itemInstructions = null;
            ImmutableList<string> wpInstructions = null;
            if (room.Name == CHECKPOINT)
            {
                var measure = Enum.GetValues<Door>().Single(d => d != src && room.Doors.HasFlag(d));
                wpInstructions = wp.Add(measure.ToString().ToLowerInvariant());
            }
            else
            {
                foreach (var itm in room.Items.Except(ITEM_BLACKLIST))
                {
                    allItems.Add(itm);
                    itemInstructions = (itemInstructions ?? gatherItems).Add("take " + itm);
                }

                foreach (var dir in Enum.GetValues<Door>())
                {
                    if (room.Doors.HasFlag(dir) && src != dir)
                    {
                        var dirStr = dir.ToString().ToLowerInvariant();
                        var gather = (itemInstructions ?? gatherItems).Add(dirStr);

                        var (items, checkpt) = Explore(Other(dir), gather, wp.Add(dirStr));
                        if (items != null)
                            itemInstructions = items;
                        wpInstructions ??= checkpt;
                    }
                }
            }

            if (src != 0 && itemInstructions != null)
                itemInstructions = itemInstructions.Add(src.ToString().ToLowerInvariant());
            return (itemInstructions, wpInstructions);
        }
        var (gather, checkpt) = Explore(0, ImmutableList<string>.Empty, ImmutableList<string>.Empty);
        var savePt = vm.RunASCIICommands(gather.Concat(checkpt.Take(checkpt.Count - 1))).Select(n => n.State).Last();

        var testCommand = checkpt.Last();

        var checkInput = new Func<IEnumerable<string>, long?>(items =>
        {
            var cmds = allItems.Except(items).Select(i => "drop " + i).Append(testCommand);
            var state = ParseRooms(savePt.RunASCIICommands(cmds).Select(n => n.Result)).First();
            var keyCode = Regex.Match(state.Lore, "\\d+", RegexOptions.Multiline);
            if (keyCode.Success)
                return long.Parse(keyCode.Value);
            return null;
        });
        return (allItems, checkInput);
    }

    protected override long? Part1()
    {
        var (items, check) = ExploreMap();
        int max = (int)Math.Pow(2, items.Count);
        for (int i = 0; i < max; i++)
        {
            var room = check(items.Where((_, b) => (i & (1 << b)) != 0));
            if (room.HasValue)
                return room;
        }
        return null;
    }
}
