// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Windows7.DesktopIntegration.WebBrowser
{
    public partial class SimpleWebBrowser : Form
    {
        public SimpleWebBrowser()
        {
            InitializeComponent();
        }

        int _lastTab = 0;
        private List<CustomWindowsManager> _windowManagers = new List<CustomWindowsManager>();

        private void button1_Click(object sender, EventArgs args)
        {
            _tabBitmaps.Add(null);

            TabPage newTab = new TabPage("Tab " + _lastTab);
            tabControl1.TabPages.Add(newTab);
            tabControl1.SelectedTab = newTab;

            int copy = _lastTab;
            System.Windows.Forms.WebBrowser wb = new System.Windows.Forms.WebBrowser();
            wb.Dock = DockStyle.Fill;
            wb.DocumentCompleted += delegate
            {
                Bitmap bmp = ScreenCapture.GrabWindowBitmap(newTab.Handle, newTab.Size);
                _tabBitmaps[copy] = bmp;
            };
            wb.Navigate(textBox1.Text);
            textBox1.Text = "www.yahoo.com";
            newTab.Controls.Add(wb);

            //TODO: UseWindowScreenshot=true doesn't work
            //because the tabs are obstructing one another.

            CustomWindowsManager cwm = CustomWindowsManager.CreateWindowsManager(newTab.Handle, Handle);
            cwm.PeekRequested += (o, e) =>
            {
                e.Bitmap = new Bitmap(_tabBitmaps[copy], e.Width, e.Height);
                e.DoNotMirrorBitmap = true;
            };
            cwm.ThumbnailRequested += (o, e) =>
            {
                e.Bitmap = new Bitmap(_tabBitmaps[copy], e.Width, e.Height);
                e.DoNotMirrorBitmap = true;
            };
            _windowManagers.Add(cwm);

            _windowManagers.ForEach(wm => wm.InvalidatePreviews());

            ++_lastTab;
        }

        private List<Bitmap> _tabBitmaps = new List<Bitmap>();

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //Bitmap oldTabBitmap = WindowUtilities.GrabWindowBitmap(tabControl1.SelectedTab.Handle, tabControl1.SelectedTab.Size);
            //_tabBitmaps[tabControl1.SelectedIndex] = oldTabBitmap;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            //Bitmap newTabBitmap = WindowUtilities.GrabWindowBitmap(tabControl1.SelectedTab.Handle, tabControl1.SelectedTab.Size);
            //_tabBitmaps[tabControl1.SelectedIndex] = newTabBitmap;
        }
    }
}