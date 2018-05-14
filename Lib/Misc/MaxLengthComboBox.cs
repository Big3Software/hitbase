using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Windows;

namespace Big3.Hitbase.Miscellaneous
{
    public class MaxLengthComboBox : ComboBox
    {
        public MaxLengthComboBox()
            : base()
        {
        }

        protected override void OnGotFocus(System.Windows.RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            ComboBoxAutomationPeer peer = new ComboBoxAutomationPeer(this);
            List<AutomationPeer> children = peer.GetChildren();
            if (children != null && children.Count == 1 && children[0] is TextBoxAutomationPeer)
            {
                ((TextBox)((TextBoxAutomationPeer)children[0]).Owner).MaxLength = MaxLength;
            }
        }

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(MaxLengthComboBox), new UIPropertyMetadata(0));
    }
}
