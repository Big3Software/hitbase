using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Big3.Hitbase.Controls
{
    public class TextBoxCodesWPF : TextBox
    {
        public TextBoxCodesWPF()
        {
            CharacterCasing = System.Windows.Controls.CharacterCasing.Upper;

            PreviewTextInput += new TextCompositionEventHandler(TextBoxCodesWPF_PreviewTextInput);
            MaxLength = 5;
        }

        void TextBoxCodesWPF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char upperKey = Char.ToUpper(e.Text[0]);
            if (upperKey > 31 && (upperKey < 'A' || upperKey > 'Z' || Text.IndexOf(upperKey) >= 0 || Text.Length >= 5))
            {
                e.Handled = true;
                return;
            }
        }
    }
}
