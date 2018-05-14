using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;

namespace Big3.Hitbase.RecordMedium
{
    public partial class RecordSelectTracks : Form
    {
        private CD CD;
        public RecordSelectTracks(CD cd)
        {
            int nPos;
            InitializeComponent();
            CD = cd;
            for (nPos = 0; nPos < CD.NumberOfTracks; nPos++)
            {
                ListViewItem newItem = new ListViewItem(CD.Tracks[nPos].TrackNumber.ToString());
                newItem.SubItems.Add(CD.Tracks[nPos].Title);
                listTracks.Items.Add(newItem);
            }
            updateDialog();
        }
        
        public string SelectedTracks { get; set; }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateDialog();
        }
        
        private void buttonMove_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem moveItem in listTracks.SelectedItems)
            {
                ListViewItem newItem2 = (ListViewItem)moveItem.Clone();
                listSelectedTracks.Items.Add(newItem2);
                listTracks.Items.Remove(moveItem);
            }
            updateDialog();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem moveItem in listSelectedTracks.SelectedItems)
            {
                ListViewItem newItem2 = (ListViewItem)moveItem.Clone();
                listTracks.Items.Add(newItem2);
                listSelectedTracks.Items.Remove(moveItem);
            }
            updateDialog();
        }

        private void buttonMoveAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem moveItem in listTracks.Items)
            {
                ListViewItem newItem2 = (ListViewItem)moveItem.Clone();
                listSelectedTracks.Items.Add(newItem2);
                listTracks.Items.Remove(moveItem);
            }
            updateDialog();
        }

        private void buttonRemoveAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem moveItem in listSelectedTracks.Items)
            {
                ListViewItem newItem2 = (ListViewItem)moveItem.Clone();
                listTracks.Items.Add(newItem2);
                listSelectedTracks.Items.Remove(moveItem);
            }
            updateDialog();
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (listSelectedTracks.SelectedItems.Count == 0 || listSelectedTracks.Items.Count < 2)
                return;

            int nPos;
            ListViewItem newitem;

            if (listSelectedTracks.Items[0].Selected == true)
            {
                return;
            }

            for (nPos = 1; nPos <= listSelectedTracks.Items.Count - 1; nPos++)
            {
                if (listSelectedTracks.Items[nPos].Selected == true)
                {
                    newitem = (ListViewItem)listSelectedTracks.Items[nPos].Clone();
                    listSelectedTracks.Items[nPos].Remove();
                    newitem = listSelectedTracks.Items.Insert(nPos - 1, newitem);
                    newitem.Selected = true;
                }
                if (listSelectedTracks.Items[0].Selected == true)
                {
                    updateDialog();
                    listSelectedTracks.Focus();
                    return;
                }
            }
            updateDialog();
            listSelectedTracks.Focus();
        }

        // Move down selected Items
        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (listSelectedTracks.SelectedItems.Count == 0 || listSelectedTracks.Items.Count < 2)
                return;

            int nPos;
            ListViewItem newitem;
            if (listSelectedTracks.Items[listSelectedTracks.Items.Count - 1].Selected == true)
            {
                listSelectedTracks.Focus();
                return;
            }

            for (nPos = listSelectedTracks.Items.Count - 1; nPos >= 0; nPos--)
            {
                if (listSelectedTracks.Items[nPos].Selected == true)
                {
                    newitem = (ListViewItem)listSelectedTracks.Items[nPos].Clone();
                    listSelectedTracks.Items[nPos].Remove();
                    newitem = listSelectedTracks.Items.Insert(nPos + 1, newitem);
                    newitem.Selected = true;
                }
                if (listSelectedTracks.Items[listSelectedTracks.Items.Count - 1].Selected == true)
                {
                    updateDialog();
                    listSelectedTracks.Focus();
                    return;
                }

            }
            updateDialog();
            listSelectedTracks.Focus();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem curItem in listSelectedTracks.Items)
            {
                SelectedTracks = SelectedTracks + curItem.Text + ",";
            }
            if (SelectedTracks.Length > 0)
                SelectedTracks = SelectedTracks.Remove(SelectedTracks.Length - 1);
        }

        private void updateDialog()
        {
            if (listSelectedTracks.Items.Count == 0)
            {
                buttonRemove.Enabled = false;
                buttonRemoveAll.Enabled = false;
            }
            else
            {
                buttonRemove.Enabled = true;
                buttonRemoveAll.Enabled = true;
            }

            if (listTracks.Items.Count == 0)
            {
                buttonMove.Enabled = false;
                buttonMoveAll.Enabled = false;
            }
            else
            {
                buttonMove.Enabled = true;
                buttonMoveAll.Enabled = true;
            }
            if (listSelectedTracks.Items.Count > 0)
            {
                if (listSelectedTracks.Items[listSelectedTracks.Items.Count - 1].Selected == true)
                    buttonDown.Enabled = false;
                else
                    buttonDown.Enabled = true;
            }

            if (listSelectedTracks.Items.Count > 0)
            {
                if (listSelectedTracks.Items[0].Selected == true)
                    buttonUp.Enabled = false;
                else
                    buttonUp.Enabled = true;
            }
        }

    }
}
