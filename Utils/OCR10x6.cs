using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    class OCR10x6 : OCR
    {
        public override int CharHeight => 10;
        public override int CharWidth => 6;
        public override int Spacing => 2;

        protected override (string letters, string drawing) GetAlphabet() => ("ABCEFGHJKLNPRXZ",@"
  ##    #####    ####   ######  ######   ####   #    #     ###  #    #  #       #    #  #####   #####   #    #  ######
 #  #   #    #  #    #  #       #       #    #  #    #      #   #   #   #       ##   #  #    #  #    #  #    #       #
#    #  #    #  #       #       #       #       #    #      #   #  #    #       ##   #  #    #  #    #   #  #        #
#    #  #    #  #       #       #       #       #    #      #   # #     #       # #  #  #    #  #    #   #  #       # 
#    #  #####   #       #####   #####   #       ######      #   ##      #       # #  #  #####   #####     ##       #  
######  #    #  #       #       #       #  ###  #    #      #   ##      #       #  # #  #       #  #      ##      #   
#    #  #    #  #       #       #       #    #  #    #      #   # #     #       #  # #  #       #   #    #  #    #    
#    #  #    #  #       #       #       #    #  #    #  #   #   #  #    #       #   ##  #       #   #    #  #   #     
#    #  #    #  #    #  #       #       #   ##  #    #  #   #   #   #   #       #   ##  #       #    #  #    #  #     
#    #  #####    ####   ######  #        ### #  #    #   ###    #    #  ######  #    #  #       #    #  #    #  ######");
    }
}
