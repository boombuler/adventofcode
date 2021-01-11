using AdventOfCode.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    class ScreenBase
    {
        private OutputMode fTextMode = new DefaultOut();

        #region Console

        protected T Console<T>()
            where T : OutputMode, new()
        {
            if (!(fTextMode is T))
            {
                fTextMode.Exit();
                fTextMode = new T();
                fTextMode.Enter();
            }
            return (T)fTextMode;
        }
        protected OutputMode Console(OutputMode mode)
        {
            if (fTextMode != mode)
            {
                fTextMode.Exit();
                fTextMode = mode;
                fTextMode.Enter();
            }
            return mode;
        }


        #endregion
    }
}
