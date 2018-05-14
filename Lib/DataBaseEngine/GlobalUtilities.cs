using System;
using System.Collections.Generic;
using System.Text;

namespace Big3.Hitbase.DataBaseEngine
{
    public abstract class GlobalUtilities
    {
        public abstract bool IsCDInDrive(int id);

        public static GlobalUtilities Current;
    }
}
