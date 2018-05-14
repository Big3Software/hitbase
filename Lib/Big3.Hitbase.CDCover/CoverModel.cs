using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Big3.Hitbase.DataBaseEngine;
using System.Windows.Media;

namespace Big3.Hitbase.CDCover
{
    public enum BackgroundType 
    {
        None,
        Color,
        Picture,
        FromCD
    }

    [Flags]
    public enum FontDecoration
    {
        None,
        Bold = 1,
        Italic = 2,
        Underline = 4
    }

    public class CoverModel : INotifyPropertyChanged
    {
        public CoverModel()
        {
            BackCoverFontModel = new CoverFontModel();
            BackCoverFontModel.Size = 4;
            BackCoverFontModel.FontFamily = "Arial";

            backCoverLeftSideFontModel = new CoverFontModel();
            backCoverLeftSideFontModel.Size = 4;
            backCoverLeftSideFontModel.FontFamily = "Arial";

            backCoverRightSideFontModel = new CoverFontModel();
            backCoverRightSideFontModel.Size = 4;
            backCoverRightSideFontModel.FontFamily = "Arial";
        }

        private int backCoverWidth = 138;

        public int BackCoverWidth
        {
            get { return backCoverWidth; }
            set
            {
                backCoverWidth = value;
                FirePropertyChanged("BackCoverWidth");
            }
        }

        private int backCoverHeight = 118;

        public int BackCoverHeight
        {
            get { return backCoverHeight; }
            set
            {
                backCoverHeight = value;
                FirePropertyChanged("BackCoverHeight");
            }
        }


        private CoverFontModel backCoverFontModel;

        public CoverFontModel BackCoverFontModel
        {
            get { return backCoverFontModel; }
            set 
            { 
                backCoverFontModel = value;
                FirePropertyChanged("BackCoverFontModel");
            }
        }

        private CoverFontModel backCoverLeftSideFontModel;

        public CoverFontModel BackCoverLeftSideFontModel
        {
            get { return backCoverLeftSideFontModel; }
            set 
            {
                backCoverLeftSideFontModel = value;
                FirePropertyChanged("BackCoverLeftSideFontModel");
            }
        }

        private CoverFontModel backCoverRightSideFontModel;

        public CoverFontModel BackCoverRightSideFontModel
        {
            get { return backCoverRightSideFontModel; }
            set 
            { 
                backCoverRightSideFontModel = value;
                FirePropertyChanged("BackCoverRightSideFontModel");
            }
        }

        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private ColumnFieldCollection backCoverTrackColumns;

        public ColumnFieldCollection BackCoverTrackColumns
        {
            get { return backCoverTrackColumns; }
            set 
            { 
                backCoverTrackColumns = value;
                FirePropertyChanged("BackCoverTrackColumns");
            }
        }

        private BackgroundType backCoverBackgroundType;

        public BackgroundType BackCoverBackgroundType
        {
            get { return backCoverBackgroundType; }
            set 
            {
                backCoverBackgroundType = value;
                FirePropertyChanged("BackCoverBackgroundType");
            }
        }

        private Color backCoverBackgroundColor;

        public Color BackCoverBackgroundColor
        {
            get { return backCoverBackgroundColor; }
            set 
            {
                backCoverBackgroundColor = value;
                FirePropertyChanged("BackCoverBackgroundColor");
            }
        }

        private string backCoverBackgroundPictureFilename;

        public string BackCoverBackgroundPictureFilename
        {
            get { return backCoverBackgroundPictureFilename; }
            set 
            { 
                backCoverBackgroundPictureFilename = value;
                FirePropertyChanged("BackCoverBackgroundColor");
            }
        }

        private int frontCoverWidth = 120;

        public int FrontCoverWidth
        {
            get { return frontCoverWidth; }
            set 
            {
                frontCoverWidth = value;
                FirePropertyChanged("FrontCoverWidth");
            }
        }

        private int frontCoverHeight = 120;

        public int FrontCoverHeight
        {
            get { return frontCoverHeight; }
            set
            {
                frontCoverHeight = value;
                FirePropertyChanged("FrontCoverHeight");
            }
        }

        private BackgroundType frontCoverBackgroundType;

        public BackgroundType FrontCoverBackgroundType
        {
            get { return frontCoverBackgroundType; }
            set
            {
                frontCoverBackgroundType = value;
                FirePropertyChanged("FrontCoverBackgroundType");
            }
        }

        private Color frontCoverBackgroundColor;

        public Color FrontCoverBackgroundColor
        {
            get { return frontCoverBackgroundColor; }
            set
            {
                frontCoverBackgroundColor = value;
                FirePropertyChanged("FrontCoverBackgroundColor");
            }
        }

        private string frontCoverBackgroundPictureFilename;

        public string FrontCoverBackgroundPictureFilename
        {
            get { return frontCoverBackgroundPictureFilename; }
            set
            {
                frontCoverBackgroundPictureFilename = value;
                FirePropertyChanged("FrontCoverBackgroundPictureFilename");
            }
        }

        private CoverFontModel frontCoverFontTitle1;

        public CoverFontModel FrontCoverFontTitle1
        {
            get { return frontCoverFontTitle1; }
            set
            {
                frontCoverFontTitle1 = value;
                FirePropertyChanged("FrontCoverFontTitle1");
            }
        }

        private CoverFontModel frontCoverFontTitle2;

        public CoverFontModel FrontCoverFontTitle2
        {
            get { return frontCoverFontTitle2; }
            set
            {
                frontCoverFontTitle2 = value;
                FirePropertyChanged("FrontCoverFontTitle2");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class CoverFontModel : INotifyPropertyChanged
    {
        private double size;
        public double Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                FirePropertyChanged("Size");
            }
        }

        private string fontFamily;

        public string FontFamily
        {
            get { return fontFamily; }
            set
            {
                fontFamily = value;
                FirePropertyChanged("FontFamily");
            }
        }

        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private FontDecoration fontDecoration;

        public FontDecoration FontDecoration
        {
            get { return fontDecoration; }
            set 
            {
                fontDecoration = value;
                FirePropertyChanged("FontDecoration");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
