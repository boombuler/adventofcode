using AdventOfCode.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using Con = System.Console;

namespace AdventOfCode
{
    abstract class TitleScreen : ScreenBase
    {
        class Cell
        {
            public char Char { get; }
            public char Attributes { get; }
            public Cell(char c, char a)
                => (Char, Attributes) = (c, a);
        }

        private int? fSelectedDay;
        private List<int> fSolvedDays;
        private Cell[][] Image { get; }

        protected abstract int[] LineDayLookup { get; }
        protected abstract int Year { get; }
        

        protected TitleScreen(char[][] chars, char[][] attributes)
        {
            Image = chars.Zip(attributes, (chrs, attrs) => chrs.Zip(attrs, (c, a) => new Cell(c, a)).ToArray()).ToArray();
            var solutions = new SolutionRepository();
            fSelectedDay = solutions.GetMostRecentDayInYear(Year);
            fSolvedDays = solutions.GetSolvedDays(Year).ToList();
        }

        protected abstract AttributeMode GetAttributeMode(char attribute);

        public void Run()
        {
            Con.CursorVisible = false;
            try
            {
                Con.SetWindowSize(120, Image.Length);
                Con.SetBufferSize(120, Image.Length);
            }
            catch { }

            Render();
            while (true)
            {
                Render();

                switch (Con.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        fSelectedDay = fSolvedDays.Where(d => d > fSelectedDay).OrderBy(d => d).Cast<int?>().FirstOrDefault() ?? fSelectedDay; break;
                    case ConsoleKey.DownArrow:
                        fSelectedDay = fSolvedDays.Where(d => d < fSelectedDay).OrderByDescending(d => d).Cast<int?>().FirstOrDefault() ?? fSelectedDay; break;
                    case ConsoleKey.Escape:
                        return;
                    case ConsoleKey.Enter:
                        Con.Clear();
                        new SolutionRepository().GetSolution(Year, fSelectedDay)?.Run();
                        Console<DefaultOut>().WriteLine(" -- press enter to continue --");
                        Con.ReadLine();
                        break;
                }
            }
        }

        private AttributeMode fStars = new AttributeMode(0xffff66);
        private AttributeMode fTextOut = new AttributeMode(0xffffff);
        protected virtual AttributeMode Unsolved { get; } = new AttributeMode(0x666666);

        private void SetAttribute(int curLine, AttributeMode mode)
        {
            var day = curLine < LineDayLookup.Length ? LineDayLookup[curLine] : -2;
            if (day >= 0 && !fSolvedDays.Contains(day))
                mode = Unsolved;

            base.Console(mode);
            mode.Selected = day == (fSelectedDay ?? -1);
        }
        private void SetAttribute(int curLine, char attribute)
            => SetAttribute(curLine, GetAttributeMode(attribute));

        private void Render()
        {
            for (int l = 0; l < Image.Length; l++)
            {
                var line = Image[l];
                Con.SetCursorPosition(0, l);
                

                foreach (var cell in line)
                {
                    SetAttribute(l, cell.Attributes);
                    Con.Write(cell.Char);
                }

                if (l < LineDayLookup.Length && (l + 1 == LineDayLookup.Length || LineDayLookup[l] != LineDayLookup[l + 1]))
                {
                    SetAttribute(l, fStars);

                    int day = LineDayLookup[l];
                    Con.Write("   ");
                    if (fSolvedDays.Contains(day))
                        Con.Write("**");
                    else
                        Con.Write("  ");

                    SetAttribute(l, fTextOut);
                    Con.Write(" {0, 2}", day);
                }
                Con.Write(new string(' ', Con.BufferWidth - Con.CursorLeft));
            }
        }

    }
}
