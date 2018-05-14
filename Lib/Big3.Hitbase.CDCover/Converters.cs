using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Big3.Hitbase.CDCover
{
    public class FontDecorationWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FontDecoration fontDecoration = (FontDecoration)value;

            if ((fontDecoration & FontDecoration.Bold) == FontDecoration.Bold)
            {
                return FontWeights.Bold;
            }
            else
            {
                return FontWeights.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FontDecorationItalicConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FontDecoration fontDecoration = (FontDecoration)value;

            if ((fontDecoration & FontDecoration.Italic) == FontDecoration.Italic)
            {
                return FontStyles.Italic;
            }
            else
            {
                return FontStyles.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FontDecorationUnderlineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FontDecoration fontDecoration = (FontDecoration)value;

            if ((fontDecoration & FontDecoration.Underline) == FontDecoration.Underline)
            {
                return TextDecorations.Underline;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
