using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;

namespace Big3.Hitbase.Controls
{
    public class CueBannerTextBox : TextBox
    {
        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern bool SendMessage(IntPtr hWnd, int message, IntPtr wParam, string lParam);

        // Windows message constants

        const int EM_SETCUEBANNER = 0x1501;

        // private internal variables

        private bool focusSelect = true;
        private string bannerText = String.Empty;

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("Appearance")]
        [Description("The prompt text to display when there is nothing in the Text property.")]
        public string CueBanner
        {
            get { return bannerText; }
            set { bannerText = value.Trim(); SetCueBanner(); }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("Behavior")]
        [Description("Automatically select the text when control receives the focus.")]
        public bool FocusSelect
        {
            get { return focusSelect; }
            set { focusSelect = value; }
        }

        protected override void OnEnter(EventArgs e)
        {
            if (Text.Length > 0 && focusSelect)
            {
                SelectAll();
            }

            base.OnEnter(e);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            SetCueBanner();
        }

        private void SetCueBanner()
        {
            SendMessage(Handle, EM_SETCUEBANNER, IntPtr.Zero, bannerText);
        }

    }
}
