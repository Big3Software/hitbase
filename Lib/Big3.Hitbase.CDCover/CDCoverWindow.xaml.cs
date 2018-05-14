using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.CDUtilities;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Controls.Ribbon;

namespace Big3.Hitbase.CDCover
{
    /// <summary>
    /// Interaction logic for CDCoverWindow.xaml
    /// </summary>
    public partial class CDCoverWindow : RibbonWindow, IModalService
    {
        DataBase dataBase;

        public CDCoverWindow(DataBase db, CD cd)
        {
            InitializeComponent();

            this.dataBase = db;
            this.cdCoverPageUserControl.CD = cd;

            cdCoverPageUserControl.ZoomWholePage();
            cdCoverPageUserControl.SelectionChanged += cdCoverPageUserControl_SelectionChanged;

            FillFontFamiliesInRibbon();

            UpdateWindowState();
        }

        void cdCoverPageUserControl_SelectionChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            ribbonComboBoxFontFamilies.Text = cdCoverPageUserControl.GetFontFamily();

            double fontSize = cdCoverPageUserControl.GetFontSize();
            if (fontSize != 0)
                ribbonComboBoxFontSizes.Text = (fontSize * 2).ToString();
            else
                ribbonComboBoxFontSizes.Text = "";

            FontDecoration fontDecoration = cdCoverPageUserControl.GetFontDecoration();

            ToggleButtonBold.IsChecked = (fontDecoration & FontDecoration.Bold) == FontDecoration.Bold;
            ToggleButtonItalic.IsChecked = (fontDecoration & FontDecoration.Italic) == FontDecoration.Italic;
            ToggleButtonUnderline.IsChecked = (fontDecoration & FontDecoration.Underline) == FontDecoration.Underline;
            buttonShowBorder.IsChecked = this.cdCoverPageUserControl.ShowBorders;
        }

        private void FillFontFamiliesInRibbon()
        {
            RibbonGalleryAllFonts.ItemsSource = FontModel.OrderBy(x => x.ToString());
            RibbonGalleryAllFontSizes.ItemsSource = FontSizeModel;
        }

        public static ObservableCollection<FontFamily> FontModel = new ObservableCollection<FontFamily>();
        public static ObservableCollection<int> FontSizeModel = new ObservableCollection<int>();

        static CDCoverWindow()
        {
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                FontModel.Add(fontFamily);
            }

            FontSizeModel.Add(8);
            FontSizeModel.Add(9);
            FontSizeModel.Add(10);
            FontSizeModel.Add(11);
            FontSizeModel.Add(12);
            FontSizeModel.Add(14);
            FontSizeModel.Add(16);
            FontSizeModel.Add(18);
            FontSizeModel.Add(20);
            FontSizeModel.Add(22);
            FontSizeModel.Add(24);
            FontSizeModel.Add(26);
            FontSizeModel.Add(28);
            FontSizeModel.Add(36);
            FontSizeModel.Add(48);
            FontSizeModel.Add(72);

        }

        private void CommandBindingPrint_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            
            if (dialog.ShowDialog() == true)
            { 
                dialog.PrintVisual(this.cdCoverPageUserControl, "Hitbase 2012 CD-Cover"); 
            }
 
        }

        private void CommandBindingShowBorders_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (cdCoverPageUserControl.ShowBorders)
                cdCoverPageUserControl.ShowBorders = false;
            else
                cdCoverPageUserControl.ShowBorders = true;
        }

        private void CommandBindingZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            double percentage = Convert.ToDouble(e.Parameter);

            cdCoverPageUserControl.Zoom(percentage / 100);
        }

        private void CommandBindingZoomWholePage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cdCoverPageUserControl.ZoomWholePage();
        }

        private void CommandBindingZoomWholeWidth_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cdCoverPageUserControl.ZoomWholeWidth();
        }

        private void CommandBindingChooseColumns_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectCDCoverTrackFields selectCDCoverTrackFields = new SelectCDCoverTrackFields();
            selectCDCoverTrackFields.Init(dataBase, FieldType.Track, cdCoverPageUserControl.Model.BackCoverTrackColumns, cdCoverPageUserControl.defaultTrackColumns);
            ((IModalService)this).NavigateTo(selectCDCoverTrackFields, StringTable.ChooseColumns, delegate(bool returnValue)
            {
                if (returnValue == true)
                {
                    cdCoverPageUserControl.SetBackCoverTrackColumns(selectCDCoverTrackFields.SelectedFields);
                }
            });
        }

        private void CommandBindingChooseFont_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cdCoverPageUserControl.ChooseFont();
        }

        #region IModalService Members

        private Stack<BackNavigationEventHandler> _backFunctions
            = new Stack<BackNavigationEventHandler>();

        void IModalService.NavigateTo(UserControl uc, string title, BackNavigationEventHandler backFromDialog)
        {
            NavigateModal(uc, title, backFromDialog, true);
        }

        void IModalService.NavigateTo(UserControl uc, string title, BackNavigationEventHandler backFromDialog, bool allowClose)
        {
            NavigateModal(uc, title, backFromDialog, allowClose);
        }

        private void NavigateModal(UserControl uc, string title, BackNavigationEventHandler backFromDialog, bool allowClose)
        {
            DoubleAnimation da = new DoubleAnimation(0.5, TimeSpan.FromMilliseconds(400).Duration());
            da.BeginTime = TimeSpan.FromMilliseconds(400);
            LayoutRoot.BeginAnimation(Grid.OpacityProperty, da);
            LayoutRoot.IsHitTestVisible = false;

            SimulatedWindow sw = new SimulatedWindow();
            sw.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            sw.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            sw.Title = title;
            sw.AllowClose = allowClose;

            sw.WindowContent = uc;
            modalGrid.Children.Clear();
            modalGrid.Children.Add(sw);

            _backFunctions.Push(backFromDialog);
        }

        void IModalService.CloseModal()
        {
            if (modalGrid.Children.Count > 0)
            {
                SimulatedWindow sw = modalGrid.Children[0] as SimulatedWindow;
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        void IModalService.GoBackward(bool dialogReturnValue)
        {
            DoubleAnimation da = new DoubleAnimation(1, TimeSpan.FromMilliseconds(400).Duration());
            LayoutRoot.BeginAnimation(Grid.OpacityProperty, da);
            LayoutRoot.IsHitTestVisible = true;
            //            modalGrid.Children.RemoveAt(modalGrid.Children.Count - 1);

            //UIElement element = modalGrid.Children[modalGrid.Children.Count - 1];
            //element.IsEnabled = true;

            BackNavigationEventHandler handler = _backFunctions.Pop();
            if (handler != null)
                handler(dialogReturnValue);
        }

        #endregion

        private void CommandBindingSizeAndPosition_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SizeAndPositionUserControl sizeAndPositionUserControl = new SizeAndPositionUserControl(this.cdCoverPageUserControl.Model);
            ((IModalService)this).NavigateTo(sizeAndPositionUserControl, StringTable.SizeAndPosition, delegate(bool returnValue)
            {
                if (returnValue == true)
                {
                }
            });

        }

        private void CommandBindingBackCoverChooseBackgroundColor_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                cdCoverPageUserControl.SetBackCoverBackgroundColor(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }

        private void CommandBindingFrontCoverChooseBackgroundColor_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                cdCoverPageUserControl.SetFrontCoverBackgroundColor(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }

        private void CommandBindingBackCoverChooseBackgroundImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = StringTable.FilterImages;
            openDlg.FileName = cdCoverPageUserControl.Model.BackCoverBackgroundPictureFilename;
            if (!string.IsNullOrEmpty(cdCoverPageUserControl.Model.BackCoverBackgroundPictureFilename) &&
                Directory.Exists(System.IO.Path.GetDirectoryName(cdCoverPageUserControl.Model.BackCoverBackgroundPictureFilename)))
                openDlg.InitialDirectory = System.IO.Path.GetDirectoryName(cdCoverPageUserControl.Model.BackCoverBackgroundPictureFilename);
            else
                openDlg.InitialDirectory = Misc.GetCDCoverDirectory();


            openDlg.Title = StringTable.ChooseImage;
            if (openDlg.ShowDialog() == true)
            {
                cdCoverPageUserControl.SetBackCoverBackgroundPictureFilename(openDlg.FileName);
            }
        }

        private void CommandBindingFrontCoverChooseBackgroundImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = StringTable.FilterImages;
            openDlg.FileName = cdCoverPageUserControl.Model.FrontCoverBackgroundPictureFilename;
            if (!string.IsNullOrEmpty(cdCoverPageUserControl.Model.FrontCoverBackgroundPictureFilename) &&
                Directory.Exists(System.IO.Path.GetDirectoryName(cdCoverPageUserControl.Model.FrontCoverBackgroundPictureFilename)))
                openDlg.InitialDirectory = System.IO.Path.GetDirectoryName(cdCoverPageUserControl.Model.FrontCoverBackgroundPictureFilename);
            else
                openDlg.InitialDirectory = Misc.GetCDCoverDirectory();


            openDlg.Title = StringTable.ChooseImage;
            if (openDlg.ShowDialog() == true)
            {
                cdCoverPageUserControl.SetFrontCoverBackgroundPictureFilename(openDlg.FileName);
            }
        }

        private void CommandBindingBackCoverChooseBackgroundFromCD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cdCoverPageUserControl.SetBackCoverBackgroundFromCD();

        }

        private void CommandBindingFrontCoverChooseBackgroundFromCD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cdCoverPageUserControl.SetFrontCoverBackgroundFromCD();

        }

        private void CommandBindingBackCoverChooseBackgroundNone_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cdCoverPageUserControl.SetBackCoverBackgroundNone();
        }

        private void CommandBindingFrontCoverChooseBackgroundNone_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cdCoverPageUserControl.SetFrontCoverBackgroundNone();
        }

        private void RibbonGalleryFontSize_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            cdCoverPageUserControl.SetFontSize((double)(int)this.ribbonFontSizeGallery.SelectedItem / 2.0);
        }

        private void RibbonGalleryFontFamily_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            cdCoverPageUserControl.SetFontFamily((FontFamily)this.ribbonFontGallery.SelectedItem);
        }

        private void CommandBindingBold_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FontDecoration fontDecoration = cdCoverPageUserControl.GetFontDecoration();

            if ((fontDecoration & FontDecoration.Bold) == FontDecoration.Bold)
                fontDecoration &= ~FontDecoration.Bold;
            else
                fontDecoration |= FontDecoration.Bold;
            cdCoverPageUserControl.SetFontDecoration(fontDecoration);

            UpdateWindowState();
        }

        private void CommandBindingItalic_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FontDecoration fontDecoration = cdCoverPageUserControl.GetFontDecoration();
            if ((fontDecoration & FontDecoration.Italic) == FontDecoration.Italic)
                fontDecoration &= ~FontDecoration.Italic;
            else
                fontDecoration |= FontDecoration.Italic;
            cdCoverPageUserControl.SetFontDecoration(fontDecoration);

            UpdateWindowState();
        }

        private void CommandBindingUnderline_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FontDecoration fontDecoration = cdCoverPageUserControl.GetFontDecoration();
            if ((fontDecoration & FontDecoration.Underline) == FontDecoration.Underline)
                fontDecoration &= ~FontDecoration.Underline;
            else
                fontDecoration |= FontDecoration.Underline;
            cdCoverPageUserControl.SetFontDecoration(fontDecoration);

            UpdateWindowState();
        }
    }
}
