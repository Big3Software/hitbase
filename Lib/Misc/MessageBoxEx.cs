using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.Miscellaneous
{
    public partial class MessageBoxEx : Form
    {
        public MessageBoxEx(string text, MessageBoxButtons buttons, MessageBoxIcon icon, string saveAnswerInRegistryKey)
        {
            InitializeComponent();

            labelText.Text = text;

            SaveAnswerInRegistryKey = saveAnswerInRegistryKey;

            switch (icon)
            {
                case MessageBoxIcon.Exclamation:
                    pictureBoxIcon.Image = Misc.IconToAlphaBitmap(Icons.exclamation);
                    break;
                case MessageBoxIcon.Information:
                    pictureBoxIcon.Image = Misc.IconToAlphaBitmap(Icons.information);
                    break;
                case MessageBoxIcon.Question:
                    pictureBoxIcon.Image = Misc.IconToAlphaBitmap(Icons.question);
                    break;
                case MessageBoxIcon.Stop:
                    pictureBoxIcon.Image = Misc.IconToAlphaBitmap(Icons.error);
                    break;
                case MessageBoxIcon.None:
                default:
                    break;
            }

            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    tableLayoutPanel.ColumnCount = 3;
                    button1.Text = StringTable.Abort;
                    button1.DialogResult = DialogResult.Abort;
                    button2.Text = StringTable.Retry;
                    button2.DialogResult = DialogResult.Retry;
                    button3.Text = StringTable.Ignore;
                    button3.DialogResult = DialogResult.Ignore;
                    AcceptButton = button1;
                    CancelButton = button1;
                    break;
                case MessageBoxButtons.OK:
                    tableLayoutPanel.ColumnCount = 1;
                    button1.Anchor = AnchorStyles.None;
                    button1.Text = StringTable.OK;
                    button1.DialogResult = DialogResult.OK;
                    AcceptButton = button1;
                    CancelButton = button1;
                    checkBoxDontAskAgain.Text = StringTable.DontDisplayAgain;
                    break;
                case MessageBoxButtons.OKCancel:
                    tableLayoutPanel.ColumnCount = 2;
                    button1.Text = StringTable.OK;
                    button1.DialogResult = DialogResult.OK;
                    button2.Text = StringTable.Cancel;
                    button2.DialogResult = DialogResult.Cancel;
                    AcceptButton = button1;
                    CancelButton = button2;
                    break;
                case MessageBoxButtons.RetryCancel:
                    tableLayoutPanel.ColumnCount = 2;
                    button1.Text = StringTable.Retry;
                    button1.DialogResult = DialogResult.Retry;
                    button2.Text = StringTable.Cancel;
                    button2.DialogResult = DialogResult.Cancel;
                    AcceptButton = button1;
                    CancelButton = button2;
                    break;
                case MessageBoxButtons.YesNo:
                    tableLayoutPanel.ColumnCount = 2;
                    button1.Text = StringTable.Yes;
                    button1.DialogResult = DialogResult.Yes;
                    button2.Text = StringTable.No;
                    button2.DialogResult = DialogResult.No;
                    AcceptButton = button1;
                    CancelButton = button2;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    tableLayoutPanel.ColumnCount = 3;
                    button1.Text = StringTable.Yes;
                    button1.DialogResult = DialogResult.Yes;
                    button2.Text = StringTable.No;
                    button2.DialogResult = DialogResult.No;
                    button3.Text = StringTable.Cancel;
                    button3.DialogResult = DialogResult.Cancel;
                    AcceptButton = button1;
                    CancelButton = button3;
                    break;
                default:
                    break;
            }
        }

        public bool SaveAnswer
        {
            get { return checkBoxDontAskAgain.Checked; }
        }

        private string saveAnswerInRegistryKey;

        public string SaveAnswerInRegistryKey
        {
            get { return saveAnswerInRegistryKey; }
            set { saveAnswerInRegistryKey = value; }
        }

        public string DontAskAgainText
        {
            get { return checkBoxDontAskAgain.Text; }
            set { checkBoxDontAskAgain.Text = value; }
        }

        public new DialogResult ShowDialog(IWin32Window parent)
        {
            if (!string.IsNullOrEmpty(SaveAnswerInRegistryKey))
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Global.HitbaseRegistryKey))
                {
                    int show = (int)regKey.GetValue(saveAnswerInRegistryKey + "Show", 1);

                    DialogResult value = (DialogResult)regKey.GetValue(saveAnswerInRegistryKey + "Value", (int)DialogResult.None);

                    if (show == 0)
                        return value;
                }
            }

            return base.ShowDialog(parent);
        }

        public static DialogResult Show(IWin32Window parent, string text, MessageBoxButtons buttons, MessageBoxIcon icon, ref bool saveAnswer)
        {
            return MessageBoxEx.Show(parent, text, buttons, icon, ref saveAnswer, null);
        }

        public static DialogResult Show(IWin32Window parent, string text, MessageBoxButtons buttons, MessageBoxIcon icon, ref bool saveAnswer, string saveAnswerInRegistryKey)
        {
            MessageBoxEx msgBox = new MessageBoxEx(text, buttons, icon, saveAnswerInRegistryKey);

            DialogResult res = msgBox.ShowDialog(parent);
            
            saveAnswer = msgBox.SaveAnswer;

            return res;
        }

        private void MessageBoxEx_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SaveAnswerInRegistryKey))
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Global.HitbaseRegistryKey, true))
                {
                    regKey.SetValue(saveAnswerInRegistryKey + "Show", SaveAnswer ? 0 : 1);

                    regKey.SetValue(saveAnswerInRegistryKey + "Value", (int)DialogResult);
                }
            }
        }
    }
}