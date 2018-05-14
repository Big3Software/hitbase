using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using Big3.Hitbase.SharedResources;
using System.IO;

namespace Big3.Hitbase.Miscellaneous
{
    public class LengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int number = (int)value;

                return Misc.GetShortTimeString(number);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return Misc.ParseTimeString(value.ToString());

            return 0;
        }
    }

    public class LongLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                long number = (long)value;

                return Misc.GetShortTimeString(number);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !(value is DBNull))
            {
                int number = 0;

                Int32.TryParse(value.ToString(), out number);

                // 0,00€ nicht anzeigen
                if (number != 0)
                {
                    return Misc.FormatCurrencyValue(number);
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string price = value.ToString();

                return Misc.ParseCurrencyValue(price);
            }

            return 0;
        }
    }

    public class UserFieldPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !(value is DBNull))
            {
                int number = 0;

                Int32.TryParse(value.ToString(), out number);

                // 0,00€ nicht anzeigen
                if (number != 0)
                {
                    return Misc.FormatCurrencyValue(number);
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string price = value.ToString();

                return Misc.ParseCurrencyValue(price);
            }

            return 0;
        }
    }

    public class UserFieldBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string sValue = value.ToString();
                if (sValue == "1" || sValue.ToLower() == "ja")
                    return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((bool)value == true)
                    return "1";
            }

            return "0";
        }
    }

    public class AlbumTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int albumType = (int)value;
            switch (albumType)
            {
                case 0: return StringTable.AudioCD;
                case 1: return StringTable.MusicDataCD;
                case 2: return StringTable.Soundfiles;
                case 3: return StringTable.ManagedSoundFiles;
                default:
                    return "<" + StringTable.Unknown + ">";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MyInt32Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is DBNull)
                    return 0;

                int number = (int)value;

                return number;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string stringValue = value.ToString();

                return Misc.Atoi(stringValue);
            }

            return 0;
        }
    }

    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DBNull || value == null)
            {
                return "";
            }

            bool b = (bool)value;

            return b ? StringTable.Yes : StringTable.No;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString().ToLower() == StringTable.Yes.ToLower())
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Ein normaler Int, wobei jedoch die Zahl 0 nicht angezeigt wird.
    /// </summary>
    public class IntHideZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !(value is DBNull))
            {
                int number = (int)value;

                if (number == 0)
                    return string.Empty;

                return number.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return Misc.Atoi(value.ToString());
            else
                return 0;
        }
    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Misc.FormatDate((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Misc.ParseDate((string)value);
        }
    }

    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Misc.Atoi((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (string)value != "")
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class IntegerToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value != 0)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }


    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }


    /// <summary>
    /// Löscht CR/LF, um eine Single-Line anzuzeigen.
    /// </summary>
    public class SingleLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string text = (string)value;

                int pos = text.IndexOf('\n');
                if (pos > 0)
                    return text.Left(pos).Replace("\n", "").Replace("\r", "");
                return text;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;

            if (value is bool)
            {
                flag = (bool)value;
            }
            else if (value is bool?)
            {
                bool ?nullable = (bool?)value;
                flag = nullable.GetValueOrDefault();
            }
            else
            {
                // type unbekannt
                throw new Exception("BooleanToVisibilityConverter: Der Parameter 'value' kann nicht in bool konvertiert werden.");
            }

            if (parameter != null)
            {
                if (bool.Parse((string)parameter))
                {
                    flag = !flag;
                }
            }

            if (flag)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool back = ((value is Visibility) && (((Visibility)value) == Visibility.Visible));

            if (parameter != null)
            {
                if ((bool)parameter)
                {
                    back = !back;
                }
            }

            return back;
        }
    }

    /// <summary>
    /// Konvertiert eine Dateigröße in eine String-Darstellung (z.B. 2048 => "2 KB")
    /// </summary>
    public class FileSizeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long filesize = (long)value;

            if (filesize == 0)
                return "0 KB";

            return String.Format("{0:#,###} KB", (filesize + 1023) / 1024);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FileExistsToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string filename = (string)value;

            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
