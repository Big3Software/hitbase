// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Windows7.DesktopIntegration.DocumentReader
{
    public partial class DocumentReader : Form
    {
        public DocumentReader()
        {
            InitializeComponent();
        }

        private void DocumentReader_Load(object sender, EventArgs e)
        {
            rtbText.Text = "Mary had a little lamb,\r\nHer fur as white as snow,\r\nAnd everywhere that Mary went,\r\nThe lamb was sure to go.\r\n\r\n";
            for (int i = 0; i < 5; ++i)
                rtbText.Text = rtbText.Text + rtbText.Text;
        }

        CustomWindowsManager _windowsManager;

        protected override void WndProc(ref Message m)
        {
            if (_clipToggled)
            {
                SetClip();
            }
            else if (_windowsManager != null)
            {
                _windowsManager.DispatchMessage(ref m);
                _windowsManager.InvalidatePreviews();
            }

            base.WndProc(ref m);
        }

        private void btnTogglePreview_Click(object sender, EventArgs args)
        {
            _clipToggled = false;

            _windowsManager = CustomWindowsManager.CreateWindowsManager(Handle, IntPtr.Zero);
            _windowsManager.PeekRequested += (o, e) =>
            {
                e.UseWindowScreenshot = true;
            };
            _windowsManager.ThumbnailRequested += (o, e) =>
            {
                Bitmap bmp = new Bitmap(e.Width, e.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    int index = rtbText.GetFirstCharIndexOfCurrentLine();

                    g.DrawString(rtbText.Text.Substring(index, Math.Min(150, rtbText.Text.Length - index)),
                        new Font("Tahoma", 9),
                        new SolidBrush(Color.Black),
                        new RectangleF(0, 0, e.Width, e.Height));
                }
                e.Bitmap = bmp;
            };
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            rtbText.LoadFile(ofd.FileName);
        }

        bool _clipToggled;

        private void btnToggleClip_Click(object sender, EventArgs e)
        {
            if (_windowsManager != null)
            {
                _windowsManager.DisablePreview();
                
            }

            _clipToggled = true;

            SetClip();
        }

        private void SetClip()
        {
            int index = rtbText.GetFirstCharIndexOfCurrentLine();
            Point point = rtbText.GetPositionFromCharIndex(index);

            Windows7Taskbar.SetThumbnailClip(Handle, new Rectangle(point, new Size(200, 119)));
        }
    }
}