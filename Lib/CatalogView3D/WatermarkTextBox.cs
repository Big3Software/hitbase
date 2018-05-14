using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Big3.Hitbase.CatalogView3D
{
    public class WatermarkTextBox : TextBox, INotifyPropertyChanged
    {
        public WatermarkTextBox()
        {
            ApplyWatermark();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            Changed("Text");
            base.OnTextChanged(e);
        }

        protected virtual void Changed(string propertyName)
        {
            ApplyWatermark();
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Reset()
        {
            Text = "";
            ApplyWatermark();
            // MUSS doppelt aufgerufen werden, da sonst bei jedem zweiten Áufruf das Textfeld leer bleibt
            Text = "";
            ApplyWatermark();
        }

        private string _watermarkText = "Watermark";
        public string WatermarkText
        {
            get
            {
                return _watermarkText;
            }
            set
            {
                if (_watermarkText != value)
                {
                    if (string.IsNullOrEmpty(Text) || Text == _watermarkText)
                    {
                        Text = value;
                    }
                    _watermarkText = value;
                    Changed("WatermarkText");
                }
            }
        }

        private System.Windows.Media.Brush _watermarkForeground = new SolidColorBrush(Colors.LightGray);
        public System.Windows.Media.Brush WatermarkForeground
        {
            get
            {
                return _watermarkForeground;
            }
            set
            {
                if (_watermarkForeground != value)
                {
                    _watermarkForeground = value;
                    Changed("WatermarkForeground");
                }
            }
        }

        private System.Windows.Media.Brush _watermarkBackground = new SolidColorBrush(Colors.White);
        public System.Windows.Media.Brush WatermarkBackground
        {
            get
            {
                return _watermarkBackground;
            }
            set
            {
                if (_watermarkBackground != value)
                {
                    _watermarkBackground = value;
                    Changed("WatermarkBackground");
                }
            }
        }

        private System.Windows.Media.Brush _standardForeground = new SolidColorBrush(Colors.Black);
        public System.Windows.Media.Brush StandardForeground
        {
            get
            {
                return _standardForeground;
            }
            set
            {
                if (_standardForeground != value)
                {
                    _standardForeground = value;
                    Changed("StandardForeground");
                }
            }
        }

        private System.Windows.Media.Brush _standardBackground = new SolidColorBrush(Colors.White);
        public System.Windows.Media.Brush StandardBackground
        {
            get
            {
                return _standardBackground;
            }
            set
            {
                if (_standardBackground != value)
                {
                    _standardBackground = value;
                    Changed("StandardBackground");
                }
            }
        }

        private void SetStandard()
        {
            Foreground = StandardForeground;
            Background = StandardBackground;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Text == WatermarkText)
            {
                Text = string.Empty;
                SetStandard();
                IsWatermarkShown = false;
            }
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            Text = Text.Trim();
            if (Text.Length == 0)
            {
                ApplyWatermark();
            }
            base.OnLostFocus(e);
        }

        private void ApplyWatermark()
        {
            if ((string.IsNullOrEmpty(Text) || Text == WatermarkText) && !IsFocused)
            {
                IsWatermarkShown = true;
                Text = WatermarkText;
                Foreground = WatermarkForeground;
                Background = WatermarkBackground;
            }
        }


        public bool IsWatermarkShown
        {
            get;
            set;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
 


