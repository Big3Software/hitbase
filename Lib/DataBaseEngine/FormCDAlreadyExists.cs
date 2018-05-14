using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Big3.Hitbase.DataBaseEngine
{
    public enum MultiAnswer
    {
        None,
        Yes,
        YesAll,
        No,
        NoAll
    }

    public partial class FormCDAlreadyExists : Form
    {
        public MultiAnswer Answer;

        public FormCDAlreadyExists()
        {
            InitializeComponent();
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            Answer = MultiAnswer.Yes;
            DialogResult = DialogResult.OK;
        }

        private void buttonYesAll_Click(object sender, EventArgs e)
        {
            Answer = MultiAnswer.YesAll;
            DialogResult = DialogResult.OK;
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            Answer = MultiAnswer.No;
            DialogResult = DialogResult.OK;
        }

        private void buttonNoAll_Click(object sender, EventArgs e)
        {
            Answer = MultiAnswer.NoAll;
            DialogResult = DialogResult.OK;
        }
    }
}
