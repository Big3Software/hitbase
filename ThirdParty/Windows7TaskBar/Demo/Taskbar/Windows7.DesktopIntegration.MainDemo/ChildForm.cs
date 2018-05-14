// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Drawing;
using System.Windows.Forms;
using Windows7.DesktopIntegration;

namespace Windows7.DesktopIntegration.MainDemo
{
    public class ChildForm : Form
    {
        Point _location;

        public ChildForm(Form parent, Point location)
        {
            Text = "This is a child!";
            MdiParent = parent;
            _location = location;
        }

        CustomWindowsManager _windowsManager;

        protected override void OnShown(EventArgs args)
        {
            Location = _location;

            _windowsManager = CustomWindowsManager.CreateWindowsManager(
                Handle, MdiParent.Handle);
            _windowsManager.PeekRequested += (o, e) =>
                {
                    e.UseWindowScreenshot = true;
                };
            _windowsManager.ThumbnailRequested += (o, e) =>
                {
                    Bitmap bmp = new Bitmap(e.Width, e.Height);
                    for (int i = 0; i < e.Width; ++i)
                        for (int j = 0; j < e.Height; ++j)
                            bmp.SetPixel(i, j, Color.Red);
                    e.Bitmap = bmp;
                };

            base.OnShown(args);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_windowsManager != null)
            {
                _windowsManager.WindowClosed();
            }

            base.OnClosed(e);
        }
    }
}