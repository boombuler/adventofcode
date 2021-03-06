﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Console
{
    class AttributeMode : OutputMode
    {
        const int COLOR_SELECTED = 0x25243E;
        private bool fSelected;
        private int fColor;
        public bool Selected
        {
            get => fSelected;
            set
            {
                if (fSelected != value)
                {
                    SetBG(value ? COLOR_SELECTED : DEFAULT_BACKGROUND);
                    fSelected = value;
                }
            }
        }

        public AttributeMode(int color)
        {
            fColor = color;
        }

        public override void Enter()
        {
            SetBG(fSelected ? COLOR_SELECTED : DEFAULT_BACKGROUND);
            SetFG(fColor);
        }

        public override void Exit()
        {
        }
    }
}
