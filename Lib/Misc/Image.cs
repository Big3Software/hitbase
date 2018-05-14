using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Big3.Hitbase.Miscellaneous
{
    public class Picture
    {
        public void Open(string filename)
        {
            Image img = Bitmap.FromFile(filename);

        }
    }
}
