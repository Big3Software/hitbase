using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.SoundEngineGUI
{
    public partial class FormPreListenVirgin : Form
    {
        public FormPreListenVirgin()
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);
        }

        private void FormPreListenVirgin_Load(object sender, EventArgs e)
        {
            string[] outputDevices = Big3.Hitbase.SoundEngine.SoundEngine.GetOutputDevices();

            for (int i = 0; i < outputDevices.Length; i++)
            {
                comboBoxOutputDevices.Items.Add(outputDevices[i]);
                if (Settings.Current.OutputDevice != i)
                {
                    if (comboBoxOutputDevices.SelectedIndex < 0)
                        comboBoxOutputDevices.SelectedIndex = i;
                }
                else
                {
                    labelOutputDevicePlaylist.Text = outputDevices[i];
                }
            }

            UpdateWindowState();
        }

        private void comboBoxOutputDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonOK.Enabled = comboBoxOutputDevices.SelectedIndex >= 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (comboBoxOutputDevices.SelectedIndex == Settings.Current.OutputDevice)
            {
                if (MessageBox.Show(StringTable.OutputDevicesIdentical, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                {
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            Settings.Current.OutputDevicePreListen = comboBoxOutputDevices.SelectedIndex;
        }
    }
}
