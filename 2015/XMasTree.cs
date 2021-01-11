using AdventOfCode.Console;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Con = System.Console;

namespace AdventOfCode._2015
{
    class XMasTree : TitleScreen
    {

        private AttributeMode fYellow = new AttributeMode(0xffff66);
        private AttributeMode fOrange = new AttributeMode(0xff9900);
        private AttributeMode fBlue = new AttributeMode(0x0066ff);
        private AttributeMode fRed = new AttributeMode(0xff0000);
        private AttributeMode fGreen = new AttributeMode(0x009900);
        private AttributeMode fWhite = new AttributeMode(0xffffff);

        protected override int[] LineDayLookup { get; } = new[] { 25, 25, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        protected override int Year => 2015;

        public XMasTree()
            : base(ReadMap("Tree"), ReadMap("TreeAttr"))
        {
        }

        protected override AttributeMode GetAttributeMode(char attribute)
            => attribute switch
            {
                '*' => fYellow,
                'o' => fOrange,
                'O' => fBlue,
                '@' => fRed,
                '_' => fGreen,
                _ => fWhite 
            };

        static char[][] ReadMap(string name)
        {
            using (var strm = typeof(XMasTree).Assembly.GetManifestResourceStream(typeof(XMasTree).Namespace + "." + name + ".txt"))
            {
                using (var tr = new StreamReader(strm))
                    return tr.ReadToEnd().Lines().Select(l => l.ToCharArray()).ToArray();
            }
        }
    }
}
