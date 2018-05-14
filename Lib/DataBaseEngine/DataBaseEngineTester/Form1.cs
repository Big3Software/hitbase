using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.CatalogView;
using System.IO;
using System.Reflection;
using Community;

namespace DataBaseEngineTester
{
    public partial class Form1 : Form
    {
        DataBase db = new DataBase();
           
        public Form1()
        {
            InitializeComponent();

            textBoxLogin.Text = "jus";
            textBoxPassword.Text = "vulkan";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            db.Open("hitbase.sdf");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MdbToSdfConverter converter = new MdbToSdfConverter();

            converter.Convert("hitbase2009.hdb", "leer.sdf");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            db.Open("c:\\hitbase2009\\cd-katalog.sdf");
            FormCatalog formCatalog = new FormCatalog(db, null);

            formCatalog.Show(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Hitbase Kataloge (*.hdb)|*.hdb||";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                HdbToSdfConverter converter = new HdbToSdfConverter();
                string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string targetFilename = Path.ChangeExtension(fd.FileName, ".sdf");
                converter.Convert(fd.FileName, targetFilename, false);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Community.CommunityRoom cr = new CommunityRoom();
            cr.Login(textBoxLogin.Text, textBoxPassword.Text);
            cr.ShowDialog();
//            FormCommunityRoom formCommunity = new FormCommunityRoom();
//            formCommunity.ShowDialog();
        }
    }
}