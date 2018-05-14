using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.Controls
{
    public class FormThemeManager
    {
        public static void SetTheme(Form form)
        {
            switch (Settings.Current.CurrentColorStyle)
            {
                case ColorStyle.Default:
                    form.BackgroundImage = Big3.Hitbase.SharedResources.Properties.Resources.background;
                    break;
                case ColorStyle.Black:
                    form.BackgroundImage = Big3.Hitbase.SharedResources.Properties.Resources.BlackBackground;
                    break;
                case ColorStyle.Silver:
                    form.BackgroundImage = Big3.Hitbase.SharedResources.Properties.Resources.SilverBackground;
                    break;
            }

            form.BackgroundImageLayout = ImageLayout.Stretch;
        }
    }
}
