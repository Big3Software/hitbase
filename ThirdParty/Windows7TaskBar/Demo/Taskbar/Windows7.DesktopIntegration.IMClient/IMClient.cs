// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Windows7.DesktopIntegration.IMClient
{
    public partial class IMClient : Form
    {
        public IMClient()
        {
            InitializeComponent();
        }

        private void IMClient_Load(object sender, EventArgs e)
        {
            Icon = new Icon("IMClient.ico");

            cmbStatus.SelectedIndex = 0;

            lstBuddies.Font = new Font(new FontFamily("Calibri"), 14, FontStyle.Bold);
            lstBuddies.Items.Add("Joe - Available");
            lstBuddies.Items.Add("Kate - Away (7 minutes)");
            lstBuddies.Items.Add("-------------------");
            lstBuddies.Items.Add("Mike - Offline");
            lstBuddies.Items.Add("Jim - Offline");
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            Icon icon = GetIconByStatus((string)cmbStatus.SelectedItem);
            Windows7Taskbar.SetTaskbarOverlayIcon(Handle, icon, (string)cmbStatus.SelectedItem);
        }

        private static Icon GetIconByStatus(string status)
        {
            switch (status)
            {
                case "Available":
                    return new Icon("Available.ico");
                case "Appear Offline":
                    return new Icon("Offline.ico");
                case "Away":
                    return new Icon("Away.ico");
            }
            return null;
        }

        int _percentFileCompleted;

        private void sendFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sendFileTimer.Interval = 1000;
            sendFileTimer.Tick += delegate
            {
                _percentFileCompleted += 10;
                if (_percentFileCompleted == 100)
                {
                    sendFileTimer.Stop();
                    MessageBox.Show("File operation failed!");
                    Windows7Taskbar.SetProgressState(Handle, Windows7Taskbar.ThumbnailProgressState.Error);
                    _percentFileCompleted = 0;
                }
                else
                {
                    Windows7Taskbar.SetProgressValue(Handle, (ulong)_percentFileCompleted, (ulong)100);
                }
            };
            sendFileTimer.Start();
        }
    }
}