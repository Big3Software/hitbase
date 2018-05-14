using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using System.IO;
using System.Drawing.Imaging;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.Controls
{
    public partial class ChoosePictureButton : UserControl
    {
        string artist;
        string title;

        public delegate void LoadFromWebDelegate();
        public event LoadFromWebDelegate LoadFromWeb;

        public delegate void ScanDelegate();
        public event ScanDelegate Scan;

        public ChoosePictureButton()
        {
            InitializeComponent();

            enableLoadFromWeb = false;
        }

        public CD CD { get; set; }

        public PersonGroupDataSet.PersonGroupRow PersonGroup { get; set; }

        public CoverType CoverType { get; set; }

        public string ButtonText
        {
            get
            {
                return buttonChoosePicture.Text;
            }
            set
            {
                buttonChoosePicture.Text = value;
            }
        }

        private Image imageWatermark;
        public Image ImageWatermark
        {
            get
            {
                return imageWatermark;
            }
            set
            {
                imageWatermark = value;
                if (value == null)
                {
                    buttonChoosePicture.BackgroundImage = Images.CDCover;
                }
                else
                {
                    buttonChoosePicture.BackgroundImage = value;
                }
            }
        }

        private bool enableLoadFromWeb = false;

        public bool EnableLoadFromWeb
        {
            get { return enableLoadFromWeb; }
            set 
            {
                enableLoadFromWeb = value;

                loadFromWebToolStripMenuItem.Visible = enableLoadFromWeb;
            }
        }

        private string imageFilename;

        public virtual string ImageFilename
        {
            get { return imageFilename; }
            set 
            {
                try
                {
                    imageFilename = value;
                    if (value == null)
                        pictureBox.Image = null;
                    else
                    {
                        byte[] imageBytes = File.ReadAllBytes(Misc.FindCover(value));

                        MemoryStream m = new MemoryStream(imageBytes);

                        pictureBox.Image = Image.FromStream(m);
                        m.Close();
                    }

                    //pictureBox.ImageLocation = value;
                    if (string.IsNullOrEmpty(imageFilename))
                    {
                        buttonChoosePicture.Visible = true;
                        pictureBox.Visible = false;
                    }
                    else
                    {
                        buttonChoosePicture.Visible = false;
                        pictureBox.Visible = true;
                    }
                }
                catch   // Ignorieren
                {
                    pictureBox.Image = Images.InvalidCDCover;
                    buttonChoosePicture.Visible = false;
                    pictureBox.Visible = true;
                }
            }
        }

        private void changePictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChooseImage();
        }

        private void deletePictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageFilename = null;
        }

        private void copyPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
                Clipboard.SetImage(pictureBox.Image);
        }

        private void contextMenuStripImage_Opening(object sender, CancelEventArgs e)
        {
            copyPictureToolStripMenuItem.Enabled = (pictureBox.Image != null);
            deletePictureToolStripMenuItem.Enabled = (pictureBox.Image != null);
            showPictureToolStripMenuItem.Enabled = (pictureBox.Image != null);
            pastePictureToolStripMenuItem.Enabled = (Clipboard.ContainsImage());
        }

        private void buttonChoosePicture_Click(object sender, EventArgs e)
        {
            ChooseImage();
        }

        private void ChooseImage()
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = StringTable.FilterImages;
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                ImageFilename = openDlg.FileName;
            }
        }

        private void loadFromWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadFromWeb != null)
                LoadFromWeb();
        }

        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Scan != null)
                Scan();
        }

        private void showPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPicture();
        }

        private void ShowPicture()
        {
            ShowPictureForm showPictureForm = new ShowPictureForm();
            showPictureForm.Image = this.pictureBox.Image;
            showPictureForm.Show(this);
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            ShowPicture();
        }

        private void pastePictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename;
            if (CoverType == CoverType.PersonGroup)
                filename = Misc.GetCDCoverFilename(Misc.FilterFilenameChars(PersonGroup.Name) + ".jpg");
            else
                filename = CD.GetCDCoverFilename(CoverType);

            using (Image img = Clipboard.GetImage())
                img.Save(filename, ImageFormat.Jpeg);
            
            ImageFilename = filename;
        }

        private void SearchInWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSearchImageInWeb formSearchImageInWeb = new FormSearchImageInWeb();
            formSearchImageInWeb.CD = CD;
            formSearchImageInWeb.SearchText = PersonGroup.Name;
            formSearchImageInWeb.CoverType = CoverType;

            if (formSearchImageInWeb.ShowDialog(this) == DialogResult.OK)
            {
                string filename;
                if (CoverType == CoverType.PersonGroup)
                    filename = Misc.GetCDCoverFilename(Misc.FilterFilenameChars(PersonGroup.Name) + ".jpg");
                else
                    filename = CD.GetCDCoverFilename(CoverType);

                /*Klasse wird nicht mehr benutzt... später ganz löschen
                 * das hier muss aber schon mal raus!
                 * using (Image img = new Bitmap(formSearchImageInWeb.SelectedImage))
                {
                    img.Save(filename, ImageFormat.Jpeg);
                }*/

                ImageFilename = filename;
            }
        }

        private void ChoosePictureButton_Load(object sender, EventArgs e)
        {
        }
    }
}
