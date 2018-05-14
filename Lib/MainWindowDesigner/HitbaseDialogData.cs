using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using Big3.Hitbase.MainWindowDesigner.Controls;
using System.Drawing;

namespace Big3.Hitbase.MainWindowDesigner
{
    [Serializable]
    public class HitbaseDialogData : HitbaseControlData
    {
        
    }

    public class HitbaseControlData
    {
        public string ControlName;

        public class Property
        {
            public Property()
            {
            }

            public Property(string name, object value)
            {
                Name = name;
                Value = value;
            }

            public string Name;
            public object Value;
        }

        public List<Property> Properties = new List<Property>();
        public ArrayList Controls = new ArrayList();

        public void AddProperties(List<Property> props, Object hlControl)
        {
            HitbaseControlData newControl = new HitbaseControlData();
            newControl.ControlName = hlControl.GetType().Name;

            // Jetzt per Reflection alle Hitbase-Properties auslesen

            PropertyInfo[] propInfo = hlControl.GetType().GetProperties();
            foreach (PropertyInfo prop in propInfo)
            {
                // Handelt es sich um ein Hitbase-Attribut?
                CategoryAttribute[] attribs = (CategoryAttribute[])prop.GetCustomAttributes(typeof(CategoryAttribute), true);

                if (attribs != null && attribs.Length > 0 && attribs[0].Category == "Hitbase")
                {
                    object objValue = prop.GetValue(hlControl, null);

                    if (objValue != null)
                    {
                        if (objValue is System.Drawing.Color)
                        {
                            if (!((System.Drawing.Color)objValue).IsEmpty)
                            {
                                System.Drawing.Color col = ((System.Drawing.Color)objValue);
                                String colValue;

                                if (col.IsNamedColor)
                                    colValue = col.Name;
                                else
                                    colValue = String.Format("#{0:x2}{1:x2}{2:x2}", col.R, col.G, col.B);

                                props.Add(new Property(prop.Name, colValue));
                            }
                        }
                        else
                        {
                            if (objValue is System.Drawing.Font)
                            {
                                props.Add(new Property(prop.Name, GetStringFromFont((System.Drawing.Font)objValue)));
                            }
                            else
                            {
                                props.Add(new Property(prop.Name, objValue));
                            }
                        }
                    }
                }
            }
        }

        public static string GetStringFromFont(Font font)
        {
            string fontString;

            fontString = string.Format("{0}; {1} pt", font.FontFamily.Name, font.Size);

            if (font.Style != FontStyle.Regular)
                fontString += "; Style=" + GetFontStyleString(font.Style);

            return fontString;
        }

        public static string GetFontStyleString(FontStyle fontStyle)
        {
            string fontStyleString = "";

            if ((fontStyle & FontStyle.Bold) != 0)
                fontStyleString += "Bold, ";
            if ((fontStyle & FontStyle.Italic) != 0)
                fontStyleString += "Italic, ";
            if ((fontStyle & FontStyle.Underline) != 0)
                fontStyleString += "Underline, ";

            if (fontStyleString.Length > 0)
                fontStyleString = fontStyleString.Substring(0, fontStyleString.Length - 2);

            return fontStyleString;
        }


        public int Add(Object hlControl)
        {
            HitbaseControlData newControl = new HitbaseControlData();
            newControl.ControlName = hlControl.GetType().Name;

            AddProperties(newControl.Properties, hlControl);

            return Controls.Add(newControl);
        }
    }

}
