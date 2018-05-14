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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Miscellaneous;
using System.ComponentModel;
using Big3.Hitbase.DataBaseEngine;
using System.IO;

namespace Big3.Hitbase.CDCover
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CDCoverPageUserControl : UserControl, INotifyPropertyChanged
    {
        private List<UIElement> selectedElements = new List<UIElement>();
        
        internal ColumnFieldCollection defaultTrackColumns = new ColumnFieldCollection();

        private bool zoomWholePageActive = true;
        private bool zoomWholeWidthActive = false;

        private double currentZoom = 1.0;

        public event EventHandler SelectionChanged;

        private CD cd;
        public CD CD 
        {
            get
            {
                return cd;
            }
            set
            {
                cd = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CD"));
                    PropertyChanged(this, new PropertyChangedEventArgs("TextBackCoverLeft"));
                    PropertyChanged(this, new PropertyChangedEventArgs("TextBackCoverRight"));
                }
            }
        }

        public CoverModel Model
        {
            get;
            set;
        }

        public CDCoverPageUserControl()
        {
            DataContext = this;

            defaultTrackColumns.Add(new ColumnField(Field.TrackNumber, 10));
            defaultTrackColumns.Add(new ColumnField(Field.TrackTitle, 100));
            defaultTrackColumns.Add(new ColumnField(Field.TrackLength, 15, TextAlignment.Right));

            Model = CreateNewModel();

            InitializeComponent();

            // DIN A4 = 21x cm in 96 dpi
            GridPage.Width = (int)(21.0 / 2.54 * 96);
            GridPage.Height = (int)(29.7 / 2.54 * 96);

            SizeChanged += new SizeChangedEventHandler(CDCoverPageUserControl_SizeChanged);
        }

        private CoverModel CreateNewModel()
        {
            CoverModel model = new CoverModel();
            
            model.BackCoverTrackColumns = new ColumnFieldCollection();

            foreach (ColumnField cf in defaultTrackColumns)
                model.BackCoverTrackColumns.Add(cf);

            model.FrontCoverBackgroundType = BackgroundType.FromCD;
            model.BackCoverBackgroundType = BackgroundType.FromCD;

            model.FrontCoverFontTitle1 = new CoverFontModel() { Size = 8, FontFamily = "Arial" };
            model.FrontCoverFontTitle2 = new CoverFontModel() { Size = 8, FontFamily = "Arial" };
            model.BackCoverFontModel = new CoverFontModel() { Size = 4, FontFamily = "Arial" };
            model.BackCoverLeftSideFontModel = new CoverFontModel() { Size = 4, FontFamily = "Arial" };
            model.BackCoverRightSideFontModel = new CoverFontModel() { Size = 4, FontFamily = "Arial" };

            return model;
        }

        void CDCoverPageUserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateZoom();
        }

        private void UpdateZoom()
        {
            if (zoomWholePageActive)
            {
                ZoomWholePage();
            }

            if (zoomWholeWidthActive)
            {
                ZoomWholeWidth();
            }
        }

        private void UpdateCoverLayout()
        {
            DataTemplate dt = new DataTemplate();
            FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));
            
            foreach (ColumnField col in Model.BackCoverTrackColumns)
            {
                FrameworkElementFactory columnFactory = new FrameworkElementFactory(typeof(ColumnDefinition));
                
                columnFactory.SetValue(ColumnDefinition.WidthProperty, new GridLength(col.Width, GridUnitType.Star));
                
                gridFactory.AppendChild(columnFactory);
            }
                       
            int columnIndex = 0;
            foreach (ColumnField col in Model.BackCoverTrackColumns)
            {
                FrameworkElementFactory textblockFactory = new FrameworkElementFactory(typeof(TextBlock));

                string colName = Track.GetPropertyNameByField(col.Field);

                Binding colBinding = new Binding(colName);
                if (col.Field == Field.TrackLength)
                {
                    colBinding.Converter = new Big3.Hitbase.Miscellaneous.LengthConverter();
                }
                textblockFactory.SetBinding(TextBlock.TextProperty, colBinding);
                textblockFactory.SetValue(Grid.ColumnProperty, columnIndex);
                textblockFactory.SetValue(TextBlock.TextAlignmentProperty, col.TextAlignment);

                Binding fontFamilyBinding = new Binding("Model.BackCoverFontModel.FontFamily");
                fontFamilyBinding.Source = this;
                textblockFactory.SetBinding(TextBlock.FontFamilyProperty, fontFamilyBinding);

                Binding fontSizeBinding = new Binding("Model.BackCoverFontModel.Size");
                fontSizeBinding.Source = this;
                textblockFactory.SetBinding(TextBlock.FontSizeProperty, fontSizeBinding);

                Binding fontWeightBinding = new Binding("Model.BackCoverFontModel.FontDecoration");
                fontWeightBinding.Source = this;
                fontWeightBinding.Converter = new FontDecorationWeightConverter();
                textblockFactory.SetBinding(TextBlock.FontWeightProperty, fontWeightBinding);

                Binding fontStyleBinding = new Binding("Model.BackCoverFontModel.FontDecoration");
                fontStyleBinding.Source = this;
                fontStyleBinding.Converter = new FontDecorationItalicConverter();
                textblockFactory.SetBinding(TextBlock.FontStyleProperty, fontStyleBinding);

                Binding textDecorationsBinding = new Binding("Model.BackCoverFontModel.FontDecoration");
                textDecorationsBinding.Source = this;
                textDecorationsBinding.Converter = new FontDecorationUnderlineConverter();
                textblockFactory.SetBinding(TextBlock.TextDecorationsProperty, textDecorationsBinding);

                gridFactory.AppendChild(textblockFactory);

                columnIndex++;
            }

            dt.VisualTree = gridFactory;
            
            ItemsControlTracks.ItemTemplate = dt;
            FrontCover.Background = Brushes.Transparent;

            switch (Model.FrontCoverBackgroundType)
            {
                case BackgroundType.Color:
                    FrontCover.Background = new SolidColorBrush(Model.FrontCoverBackgroundColor);
                    break;
                case BackgroundType.Picture:
                    FrontCover.Background = new ImageBrush(new BitmapImage(new Uri(Model.FrontCoverBackgroundPictureFilename)));
                    break;
                case BackgroundType.FromCD:
                    {
                        string coverFilename = Misc.FindCover(this.CD.CDCoverFrontFilename);
                        if (File.Exists(coverFilename))
                        {
                            FrontCover.Background = new ImageBrush(new BitmapImage(new Uri(coverFilename)));
                        }
                        break;
                    }
            }

            BackCover.Background = Brushes.Transparent;
            switch (Model.BackCoverBackgroundType)
            {
                case BackgroundType.Color:
                    BackCover.Background = new SolidColorBrush(Model.BackCoverBackgroundColor);
                    break;
                case BackgroundType.Picture:
                    BackCover.Background = new ImageBrush(new BitmapImage(new Uri(Model.BackCoverBackgroundPictureFilename)));
                    break;
                case BackgroundType.FromCD:
                    {
                        string coverFilename = Misc.FindCover(this.CD.CDCoverBackFilename);
                        if (File.Exists(coverFilename))
                        {
                            BackCover.Background = new ImageBrush(new BitmapImage(new Uri(coverFilename)));
                        }
                        break;
                    }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCoverLayout();

            UpdateZoom();
        }

        private void Cover_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectObject(e.OriginalSource as UIElement);
        }

        private void SelectObject(UIElement element)
        {
            UIElement selectableObject = GetSelectableObject(element);

            if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
            {
                ClearAllAdorners();
                selectedElements.Clear();
            }

            if (selectableObject != null)
            {
                selectedElements.Add(selectableObject);
            }

            UpdateAdorners();

            if (SelectionChanged != null)
                SelectionChanged(this, new EventArgs());
        }

        private UIElement GetSelectableObject(UIElement element)
        {
            while (element != null)
            {
                bool isSelectable = (bool)element.GetValue(CoverElementExtensions.IsSelectableProperty);

                if (isSelectable)
                    return element;

                element = VisualTreeExtensions.FindParent<UIElement>(element);
            }

            return null;
        }

        private void ClearAllAdorners()
        {
            foreach (UIElement element in selectedElements)
            {
                AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(element);

                Adorner[] toRemoveArray = myAdornerLayer.GetAdorners(element);
                if (toRemoveArray != null)
                {
                    for (int x = 0; x < toRemoveArray.Length; x++)
                    {
                        myAdornerLayer.Remove(toRemoveArray[x]);
                    }
                }
            }
        }

        private void UpdateAdorners()
        {
            foreach (UIElement element in selectedElements)
            {
                AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(element);

                Adorner[] toRemoveArray = myAdornerLayer.GetAdorners(element);
                if (toRemoveArray != null)
                {
                    for (int x = 0; x < toRemoveArray.Length; x++)
                    {
                        myAdornerLayer.Remove(toRemoveArray[x]);
                    }
                }

                myAdornerLayer.Add(new SimpleCircleAdorner(element));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool showBorders = true;

        public bool ShowBorders
        {
            get { return showBorders; }
            set 
            { 
                showBorders = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ShowBorders"));
                }
            }
        }

        public Thickness ShowBordersThickness
        {
            get
            {
                return new Thickness(0.2);
            }
        }

        public string TextBackCoverLeft
        {
            get
            {
                if (CD != null)
                    return CD.Artist + " - " + CD.Title;
                else
                    return "";
            }
        }

        public string TextBackCoverRight
        {
            get
            {
                if (CD != null)
                    return CD.Artist + " - " + CD.Title;
                else
                    return "";
            }
        }

        internal void Zoom(double factor)
        {
            SetZoomFactor(factor);

            zoomWholePageActive = false;
            zoomWholeWidthActive = false;

        }

        private void SetZoomFactor(double factor)
        {
            scaleTransform.ScaleX = factor;
            scaleTransform.ScaleY = factor;

            currentZoom = factor;
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Delta < 0)
                {
                    SetZoomFactor(currentZoom / 1.1);
                }
                else
                {
                    SetZoomFactor(currentZoom * 1.1);
                }
            }
        }

        internal void ZoomWholePage()
        {
            if (this.IsLoaded)
            {
                double factor = this.ActualHeight / GridPage.ActualHeight;

                SetZoomFactor(factor);
            }

            zoomWholePageActive = true;
            zoomWholeWidthActive = false;

        }

        internal void ZoomWholeWidth()
        {
            if (this.IsLoaded)
            {
                double factor = (this.ActualWidth - 20) / GridPage.ActualWidth;

                SetZoomFactor(factor);
            }

            zoomWholeWidthActive = true;
            zoomWholePageActive = false;
        }

        internal void ChooseFont()
        {
            System.Windows.Forms.FontDialog fontDialog = new System.Windows.Forms.FontDialog();
            fontDialog.Font = new System.Drawing.Font(Model.BackCoverFontModel.FontFamily, (float)Model.BackCoverFontModel.Size* 2);
            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Model.BackCoverFontModel.Size = fontDialog.Font.Size / 2;
            }
        }

        internal void SetBackCoverTrackColumns(ColumnFieldCollection columnFieldCollection)
        {
            Model.BackCoverTrackColumns = columnFieldCollection;

            UpdateCoverLayout();
        }

        internal void SetFrontCoverBackgroundNone()
        {
            Model.FrontCoverBackgroundType = BackgroundType.None;

            UpdateCoverLayout();
        }

        internal void SetBackCoverBackgroundNone()
        {
            Model.BackCoverBackgroundType = BackgroundType.None;

            UpdateCoverLayout();
        }

        internal void SetBackCoverBackgroundColor(Color color)
        {
            Model.BackCoverBackgroundType = BackgroundType.Color;
            Model.BackCoverBackgroundColor = color;

            UpdateCoverLayout();
        }

        internal void SetFrontCoverBackgroundColor(Color color)
        {
            Model.FrontCoverBackgroundType = BackgroundType.Color;
            Model.FrontCoverBackgroundColor = color;

            UpdateCoverLayout();
        }

        internal void SetFrontCoverBackgroundFromCD()
        {
            Model.FrontCoverBackgroundType = BackgroundType.FromCD;

            UpdateCoverLayout();
        }

        internal void SetBackCoverBackgroundFromCD()
        {
            Model.BackCoverBackgroundType = BackgroundType.FromCD;

            UpdateCoverLayout();
        }

        internal void SetFrontCoverBackgroundPictureFilename(string filename)
        {
            Model.FrontCoverBackgroundType = BackgroundType.Picture;
            Model.FrontCoverBackgroundPictureFilename = filename;

            UpdateCoverLayout();
        }

        internal void SetBackCoverBackgroundPictureFilename(string filename)
        {
            Model.BackCoverBackgroundType = BackgroundType.Picture;
            Model.BackCoverBackgroundPictureFilename = filename;

            UpdateCoverLayout();
        }

        internal void SetFontSize(double fontSize)
        {
            foreach (var obj in selectedElements)
            {
                CoverFontModel fontModel = GetCoverElementFontModel(obj);
                if (fontModel != null)
                    fontModel.Size = fontSize;
            }
        }

        internal void SetFontFamily(FontFamily fontFamily)
        {
            foreach (var obj in selectedElements)
            {
                CoverFontModel fontModel = GetCoverElementFontModel(obj);
                if (fontModel != null)
                    fontModel.FontFamily = fontFamily.ToString();
            }
        }

        internal string GetFontFamily()
        {
            string fontFamily = null;

            foreach (var obj in selectedElements)
            {
                string elementFontFamily = null;

                CoverFontModel fontModel = GetCoverElementFontModel(obj);
                if (fontModel != null)
                {
                    elementFontFamily = fontModel.FontFamily;

                    if (elementFontFamily != null)
                    {
                        if (fontFamily == null)
                        {
                            fontFamily = elementFontFamily;
                        }
                        else
                        {
                            if (fontFamily != elementFontFamily)
                                return "";
                        }
                    }
                }
            }

            return fontFamily;
        }

        internal double GetFontSize()
        {
            double fontSize = 0;

            foreach (var obj in selectedElements)
            {
                double elementFontSize = 0;

                CoverFontModel fontModel = GetCoverElementFontModel(obj);
                if (fontModel != null)
                {
                    elementFontSize = fontModel.Size;

                    if (elementFontSize != 0)
                    {
                        if (fontSize == 0)
                        {
                            fontSize = elementFontSize;
                        }
                        else
                        {
                            if (fontSize != elementFontSize)
                                return 0;
                        }
                    }
                }
            }

            return fontSize;
        }

        internal FontDecoration GetFontDecoration()
        {
            FontDecoration fontDecoration = FontDecoration.None;

            foreach (var obj in selectedElements)
            {
                FontDecoration elementFontDecoration;

                CoverFontModel fontModel = GetCoverElementFontModel(obj);
                if (fontModel != null)
                {
                    elementFontDecoration = GetCoverElementFontModel(obj).FontDecoration;

                    if ((elementFontDecoration & FontDecoration.Bold) == FontDecoration.Bold)
                    {
                        fontDecoration |= FontDecoration.Bold;
                    }

                    if ((elementFontDecoration & FontDecoration.Italic) == FontDecoration.Italic)
                    {
                        fontDecoration |= FontDecoration.Italic;
                    }

                    if ((elementFontDecoration & FontDecoration.Underline) == FontDecoration.Underline)
                    {
                        fontDecoration |= FontDecoration.Underline;
                    }
                }
            }

            return fontDecoration;
        }

        internal void SetFontDecoration(FontDecoration fontDecoration)
        {
            foreach (var obj in selectedElements)
            {
                CoverFontModel fontModel = GetCoverElementFontModel(obj);
                if (fontModel != null)
                    fontModel.FontDecoration = fontDecoration;
            }
        }

        private CoverFontModel GetCoverElementFontModel(UIElement obj)
        {
            CoverElement coverElement = (CoverElement)obj.GetValue(CoverElementExtensions.CoverElementProperty);
            switch (coverElement)
            {
                case CoverElement.FrontCoverTitle1:
                    return Model.FrontCoverFontTitle1;
                case CoverElement.FrontCoverTitle2:
                    return Model.FrontCoverFontTitle2;
                case CoverElement.BackCoverLeftSide:
                    return Model.BackCoverLeftSideFontModel;
                case CoverElement.BackCoverRightSide:
                    return Model.BackCoverRightSideFontModel;
                case CoverElement.BackCoverTracklist:
                    return Model.BackCoverFontModel;
            }
            return null;
        }
    }

    public class ShowBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
                return Brushes.Black;
            else
                return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
