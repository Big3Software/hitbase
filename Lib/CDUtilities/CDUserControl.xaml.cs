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
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SoundEngine;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Big3.Hitbase.Miscellaneous;
using System.Diagnostics;
using Microsoft.Win32;
using Big3.Hitbase.SharedResources;
using System.IO;
using System.Windows.Threading;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for CDUserControl.xaml
    /// </summary>
    public partial class CDUserControl : UserControl, INotifyPropertyChanged
    {
        private ColumnFieldCollection trackListColumns = new ColumnFieldCollection();
        private ColumnFieldCollection defaultTrackListColumns = new ColumnFieldCollection();

        public CDUserControl()
        {
            InitializeComponent();

            defaultTrackListColumns.Add(new ColumnField(Field.TrackNumber));
            defaultTrackListColumns.Add(new ColumnField(Field.ArtistTrackName));
            defaultTrackListColumns.Add(new ColumnField(Field.TrackTitle));
            defaultTrackListColumns.Add(new ColumnField(Field.TrackLength));
            defaultTrackListColumns.Add(new ColumnField(Field.TrackRating));

            LoadConfiguration();

            Loaded += new RoutedEventHandler(CDUserControl_Loaded);

            this.DataContext = new CD();
        }

        void CDUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CreateOverviewTrackColumns();
        }

        public int[] GetSelectedTrackIndices()
        {
            List<int> selectedTrackIndices = new List<int>();

            foreach (Track track in dataTracks.SelectedItems)
            {
                selectedTrackIndices.Add(track.TrackNumber - 1);
            }

            return selectedTrackIndices.ToArray();
        }

        private void CreateOverviewTrackColumns()
        {
            if (CD == null)
                return;

            dataTracks.Columns.Clear();

            foreach (ColumnField col in trackListColumns)
            {
                DataGridColumn newCol = CreateDataGridColumn(col);

                if (newCol != null)
                {
                    newCol.SetValue(Big3.Hitbase.Controls.DataGridExtensions.FieldProperty, col.Field);
                    dataTracks.Columns.Add(newCol);
                }
            }
        }

        private DataGridColumn CreateDataGridColumn(ColumnField col)
        {
            DataGridColumn newColumn = null;

            switch (col.Field)
            {
                case Field.TrackNumber:
                    {
                        DataGridTemplateColumn newTrackNumberColumn = new DataGridTemplateColumn();
                        newTrackNumberColumn.Width = col.Width;
                        newTrackNumberColumn.Header = DataBase.GetNameOfField(col.Field);
                        if (CD.Type == AlbumType.AudioCD && !string.IsNullOrEmpty(CD.Identity))
                        {
                            newTrackNumberColumn.IsReadOnly = true;
                        }
                        else
                        {
                            newTrackNumberColumn.CellEditingTemplate = this.FindResource("TrackNumberEditTemplate") as DataTemplate;
                        }

                        DataTemplate multilineCelltemplate = this.FindResource("TrackNumberTemplate") as DataTemplate;
                        newTrackNumberColumn.CellTemplate = multilineCelltemplate;

                        newColumn = newTrackNumberColumn;
                        break;
                    }
                case Field.ArtistTrackName:
                    {
                        DataGridTextColumn newTextColumn = CreateDataGridTextColumn(col);

                        if (CD != null && !CD.Sampler)
                        {
                            Binding binding = new Binding("Artist");
                            binding.Source = this.CD;
                            newTextColumn.Binding = binding;
                            newTextColumn.IsReadOnly = true;
                        }

                        newColumn = newTextColumn;
                        break;
                    }
                case Field.TrackBpm:
                case Field.TrackYearRecorded:
                    {
                        DataGridTextColumn newTextColumn = CreateDataGridTextColumn(col);
                        ((Binding)newTextColumn.Binding).Converter = new IntHideZeroConverter();
                        newColumn = newTextColumn;
                        break;
                    }
                case Field.TrackComment:
                    {
                        DataGridTemplateColumn newMultilineColumn = new DataGridTemplateColumn();
                        newMultilineColumn.Width = col.Width;
                        newMultilineColumn.Header = DataBase.GetNameOfField(col.Field);

                        DataTemplate multilineCelltemplate = this.FindResource("CommentTemplate") as DataTemplate;
                        newMultilineColumn.CellTemplate = multilineCelltemplate;
                        newMultilineColumn.CellEditingTemplate = multilineCelltemplate;

                        newColumn = newMultilineColumn;
                        break;
                    }
                case Field.TrackLyrics:
                    {
                        DataGridTemplateColumn newMultilineColumn = new DataGridTemplateColumn();
                        newMultilineColumn.Width = col.Width;
                        newMultilineColumn.Header = DataBase.GetNameOfField(col.Field);

                        DataTemplate multilineCelltemplate = this.FindResource("LyricsTemplate") as DataTemplate;
                        newMultilineColumn.CellTemplate = multilineCelltemplate;
                        newMultilineColumn.CellEditingTemplate = multilineCelltemplate;

                        newColumn = newMultilineColumn;
                        break;
                    }
                case Field.TrackCodes:
                    {
                        DataGridCodesColumn textCol = new DataGridCodesColumn();
                        textCol.Width = new DataGridLength(col.Width);
                        textCol.Header = DataBase.GetNameOfField(col.Field);
                        string propertyName = Track.GetPropertyNameByField(col.Field);
                        textCol.Binding = new Binding(propertyName);

                        newColumn = textCol;
                        break;
                    }
                case Field.TrackCategory:
                    {
                        DataGridComboBoxColumn categoryColumn = new DataGridComboBoxColumn();
                        categoryColumn.Width = new DataGridLength(col.Width);
                        categoryColumn.Header = DataBase.GetNameOfField(col.Field);
                        categoryColumn.CanUserSort = false;
                        categoryColumn.SelectedValueBinding = new Binding(Track.GetPropertyNameByField(col.Field));
                        if (Settings.Current.AutoSortGenres)
                            categoryColumn.ItemsSource = from x in dataBase.AllCategories orderby x.Name select x;
                        else
                            categoryColumn.ItemsSource = from x in dataBase.AllCategories orderby x.Order select x;

                        categoryColumn.DisplayMemberPath = "Name";
                        categoryColumn.SelectedValuePath = "Name";
                        newColumn = categoryColumn;
                        break;
                    }
                case Field.TrackLength:
                    {
                        DataGridTextColumn newTextColumn = CreateDataGridTextColumn(col);
                        ((Binding)newTextColumn.Binding).Converter = FindResource("lengthConverter") as IValueConverter;
                        if (CD.Type == AlbumType.AudioCD && !string.IsNullOrEmpty(CD.Identity))
                            newTextColumn.IsReadOnly = true;
                        newColumn = newTextColumn;
                        break;
                    }
                case Field.TrackRating:
                    {
                        DataGridTemplateColumn newRatingColumn = new DataGridTemplateColumn();
                        newRatingColumn.Header = DataBase.GetNameOfField(col.Field);

                        FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Big3.Hitbase.Controls.RatingUserControl));
                        Binding b = new Binding(Track.GetPropertyNameByField(col.Field));
                        b.Mode = BindingMode.TwoWay;
                        factory.SetBinding(Big3.Hitbase.Controls.RatingUserControl.RatingProperty, b);
                        factory.SetValue(Big3.Hitbase.Controls.RatingUserControl.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Left);
                        factory.AddHandler(RatingUserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RatingCell_MouseLeftButtonDown));
                        DataTemplate cellTemplate = new DataTemplate();
                        cellTemplate.VisualTree = factory;
                        newRatingColumn.CellTemplate = cellTemplate;
                        //newRatingColumn.CellEditingTemplate = cellTemplate;
                        newRatingColumn.IsReadOnly = true;
                        newColumn = newRatingColumn;
                        break;
                    }
                default:
                    {
                        newColumn = CreateDataGridTextColumn(col);

                        break;
                    }
            }

            return newColumn;
        }

        private void RatingCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = VisualTreeExtensions.FindParent<DataGridRow>((DependencyObject)e.OriginalSource);
            Track track = row.DataContext as Track;

            RatingUserControl ratingUserControl = sender as RatingUserControl;
            int rating = ratingUserControl.Rating;

            track.Rating = rating;
        }

        /// <summary>
        /// Die Track-Spalte an den Track-Interpreten oder an den CD-Interpreten binden (abhängig von Sampler)
        /// </summary>
        private void UpdateTrackArtistBinding()
        {
            CreateOverviewTrackColumns();

            if (CD.Sampler)
            {
                Binding binding = new Binding("CurrentTrack.Artist");
                binding.Source = this;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                textTrackInterpret.SetBinding(TextBox.TextProperty, binding);
            }
            else
            {
                Binding binding = new Binding("CD.Artist");
                binding.Source = this;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                textTrackInterpret.SetBinding(TextBox.TextProperty, binding);
            }
        }

        private DataGridTextColumn CreateDataGridTextColumn(ColumnField col)
        {
            DataGridMaxLengthTextColumn textCol;

            switch (col.Field)
            {
                case Field.ArtistTrackName:
                case Field.ComposerTrackName:
                    {
                        textCol = new DataGridAutoCompleteTextColumn() { AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup, DataBase = this.DataBase };
                        break;
                    }
                case Field.TrackTitle:
                    {
                        textCol = new DataGridAutoCompleteTextColumn() { AutoCompleteTextBoxType = AutoCompleteTextBoxType.TrackTitle, DataBase = this.DataBase };
                        break;
                    }
                default:
                    {
                        textCol = new DataGridMaxLengthTextColumn();
                        break;
                    }
            }

            textCol.Width = new DataGridLength(col.Width);
            textCol.Header = DataBase.GetNameOfField(col.Field);
            if (DataBase.GetTypeByField(col.Field) == typeof(string))
                textCol.MaxLength = DataBase.GetMaxStringLength(col.Field);
            string propertyName = Track.GetPropertyNameByField(col.Field);
            textCol.Binding = new Binding(propertyName);

            return textCol;
        }

        private void CommandBindingChooseColumns_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveConfiguration();

            FormChooseColumnFields formChooseFields = new FormChooseColumnFields(this.DataBase, FieldType.TrackMain, this.trackListColumns, this.defaultTrackListColumns);

            if (formChooseFields.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.trackListColumns = formChooseFields.SelectedFields;
                CreateOverviewTrackColumns();
                SaveConfiguration();
            }
        }


        private DataBase dataBase;
        public DataBase DataBase
        {
            get
            {
                return dataBase;
            }
            set
            {
                dataBase = value;
                textInterpret.DataBase = value;
                textTitel.DataBase = value;
                textKomponist.DataBase = value;
                textCDInterpret.DataBase = value;
                textCDTitel.DataBase = value;
                textCDKomponist.DataBase = value;
                textTrackInterpret.DataBase = value;
                textTrackTitel.DataBase = value;
                textTrackKomponist.DataBase = value;
                artistParticipants.DataBase = value;
                FillCategories();
                FillMedium();
                comboBoxCDSet.ItemsSource = dataBase.GetAvailableSets();
                comboBoxCDLabel.ItemsSource = dataBase.GetAvailableLabels();
                comboBoxCDLocation.ItemsSource = dataBase.GetAvailableLocations();
                List<string> availableLanguages = dataBase.GetAvailableLanguages();
                comboBoxCDSprache.ItemsSource = availableLanguages;
                comboBoxTrackSprache.ItemsSource = availableLanguages;

                // Muss ich hier im Code machen, da wir den ConverterParameter nicht binden können.
                Binding bd = new Binding("Date");
                bd.ConverterParameter = value;
                bd.Converter = new DataBaseDateConverter();
                textCDKaufdatum.SetBinding(TextBox.TextProperty, bd);

                TextBlockCDKaufdatum.Text = DataBase.Master.DateName + ":";
            }
        }

        private void FillMedium()
        {
            string saveMedium = "";
            
            // alten Wert merken, da durch das Setzen der ComboBox ItemSource der Wert gelöscht wird.
            if (CD != null)
                saveMedium = CD.Medium;

            if (Settings.Current.AutoSortMediums)
            {
                comboBoxCDMedium.ItemsSource = from x in dataBase.AllMediums orderby x.Name select x;
            }
            else
            {
                comboBoxCDMedium.ItemsSource = from x in dataBase.AllMediums orderby x.Order select x;
            }

            if (CD != null)
                CD.Medium = saveMedium;
        }

        private void FillCategories()
        {
            string saveCategory = "";

            // alten Wert merken, da durch das Setzen der ComboBox ItemSource der Wert gelöscht wird.
            if (CD != null)
                saveCategory = CD.Category;

            if (Settings.Current.AutoSortGenres)
            {
                comboBoxCategory.ItemsSource = from x in dataBase.AllCategories orderby x.Name select x;
                comboBoxCDCategory.ItemsSource = from x in dataBase.AllCategories orderby x.Name select x;
                comboBoxTrackCategory.ItemsSource = from x in dataBase.AllCategories orderby x.Name select x;
            }
            else
            {
                comboBoxCategory.ItemsSource = from x in dataBase.AllCategories orderby x.Order select x;
                comboBoxCDCategory.ItemsSource = from x in dataBase.AllCategories orderby x.Order select x;
                comboBoxTrackCategory.ItemsSource = from x in dataBase.AllCategories orderby x.Order select x;
            }

            if (CD != null)
                CD.Category = saveCategory;
        }

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
                dataTracks.ItemsSource = cd.Tracks;
                imageFrontCover.CD = value;
                imageCDFrontCover.CD = value;
                imageCDBackCover.CD = value;
                imageCDLabelCover.CD = value;
                userCDFieldsUserControl.CD = value;
                this.DataContext = CD;

                CurrentTrackIndex = 0;

                UpdateTrackArtistBinding();

                FillData();

                userCDFieldsUserControl.SetFields(dataBase, dataBase.Master.UserCDFields);
            }
        }

        private int currentTrackIndex;

        public int CurrentTrackIndex
        {
            get
            {
                return currentTrackIndex;
            }
            set
            {
                currentTrackIndex = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentTrackIndex"));
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentTrack"));

                userTrackFieldsUserControl.Track = CurrentTrack;

                userTrackFieldsUserControl.SetFields(dataBase, dataBase.Master.UserTrackFields);

                UpdateWindowState();
            }
        }

        public Track CurrentTrack
        {
            get
            {
                if (this.CD == null || this.CD.Tracks == null || this.CD.Tracks.Count <= currentTrackIndex)
                    return null;

                return this.CD.Tracks[currentTrackIndex];
            }
        }

        private void FillData()
        {
/*            imageFrontCover.ImageFilename = CD.CDCoverFrontFilename;
            imageCDFrontCover.ImageFilename = CD.CDCoverFrontFilename;
            imageCDBackCover.ImageFilename = CD.CDCoverBackFilename;
            imageCDLabelCover.ImageFilename = CD.CDCoverLabelFilename;*/

            buttonCDTogglebuttonCDSET.IsChecked = CD.CDSetNumber > 0;

            // Bei Audio-CDs kann die Tracknummer nicht geändert werden (außer manuell erfasste).
            textTrackNumber.IsReadOnly = (CD.Type == 0 && !string.IsNullOrEmpty(CD.Identity));

            SetParticipantsView();

            if (string.IsNullOrEmpty(CD.Identity))
            {
                this.DockPanelTrackToolbar.Visibility = System.Windows.Visibility.Visible;
            }

            this.FillCategories();

            UpdateWindowState();
        }

        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            textCDKaufdatum.Text = datePicker1.Text;
            textCDKaufdatum.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void buttonCDTogglebuttonSampler_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTrackArtistBinding();

            if (CD.Sampler)
            {
                if (Settings.Current.SamplerUseFixedArtist && CD.Type != AlbumType.ManagedSoundFiles)
                {
                    CD.Artist = Settings.Current.SamplerFixedArtistText;
                }
            }

            UpdateWindowState();
        }

        private void buttonCDTogglebuttonSampler_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateTrackArtistBinding();
            UpdateWindowState();
        }

        private void buttonTogglebutton_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateWindowState();
        }

        private void buttonTogglebutton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateWindowState();
        }

        private void buttonArchivNr_Click(object sender, RoutedEventArgs e)
        {
            CD.ArchiveNumber = DataBase.GetNextFreeArchiveNumber();
        }

        private void CommandBindingEditCategories_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Big3.Hitbase.SharedResources.HitbaseCommands.EditCategories.Execute(null, Application.Current.MainWindow);
            FillCategories();
        }

        private void CommandBindingEditMediums_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Big3.Hitbase.SharedResources.HitbaseCommands.EditMediums.Execute(null, Application.Current.MainWindow);
            FillMedium();
        }

        private void CommandBindingSelectCodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Button btn = e.Parameter as Button;

            if (btn == buttonCDKennzeichen)
                Big3.Hitbase.SharedResources.HitbaseCommands.SelectCodes.Execute(this.textCDKennzeichen, Application.Current.MainWindow);
            if (btn == buttonTrackKennzeichen)
                Big3.Hitbase.SharedResources.HitbaseCommands.SelectCodes.Execute(this.textTrackKennzeichen, Application.Current.MainWindow);
        }

        public static readonly RoutedEvent CDTitleChangedEvent = EventManager.RegisterRoutedEvent(
    "CDTitleChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CDUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler CDTitleChanged
        {
            add { AddHandler(CDTitleChangedEvent, value); }
            remove { RemoveHandler(CDTitleChangedEvent, value); }
        }


        private void UpdateTabCaption()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(CDUserControl.CDTitleChangedEvent);
            RaiseEvent(newEventArgs);
        }

        private void buttonCDInterpret_Click(object sender, RoutedEventArgs e)
        {
            EditArtist();
        }

        private void buttonInterpret_Click(object sender, RoutedEventArgs e)
        {
            EditArtist();
        }

        private void EditArtist()
        {
            PersonGroup personGroup = DataBase.GetPersonGroupByName(textCDInterpret.Text, true);
            PersonGroupWindow personGroupWindow = new PersonGroupWindow(DataBase, PersonType.Artist, personGroup);
            personGroupWindow.Owner = Window.GetWindow(this);
            if (personGroupWindow.ShowDialog() == true)
            {
                textCDInterpret.Text = personGroup.Name;
            }
        }

        private void buttonCDHomepage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textCDHomepage.Text))
            {
                try
                {
                    Process.Start(textCDHomepage.Text);
                }
                catch
                {       // Exception wird ignoriert
                }
            }

        }

        private void buttonCDKomponist_Click(object sender, RoutedEventArgs e)
        {
            EditComposer();
        }

        private void EditComposer()
        {
            PersonGroup personGroup = DataBase.GetPersonGroupByName(textCDKomponist.Text, true);
            PersonGroupWindow personGroupWindow = new PersonGroupWindow(DataBase, PersonType.Composer, personGroup);
            personGroupWindow.Owner = Window.GetWindow(this);
            if (personGroupWindow.ShowDialog() == true)
            {
                textCDKomponist.Text = personGroup.Name;
            }
        }

        private void textCDKomponist_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonCDKomponist.IsEnabled = textCDKomponist.Text.Length > 0;
            buttonComposer.IsEnabled = textCDKomponist.Text.Length > 0;
            buttonTrackKomponist.IsEnabled = textTrackKomponist.Text.Length > 0;

            comboBoxCDSet.IsEnabled = buttonCDTogglebuttonCDSET.IsChecked.Value;
            comboBoxCDSetNumber.IsEnabled = buttonCDTogglebuttonCDSET.IsChecked.Value;

            buttonMitwirkendeDel.IsEnabled = GetSelectedParticipant() != null;
            buttonMitwirkendeEdit.IsEnabled = GetSelectedParticipant() != null;

            buttonTrackPrevTrack.IsEnabled = this.CurrentTrackIndex > 0;
            buttonTrackNextTrack.IsEnabled = this.CurrentTrackIndex < this.CD.Tracks.Count - 1;

            if (CD.Sampler)
            {
                this.TextBlockInterpret.Text = "Titel 1:";
                this.TextBlockTitel.Text = "Titel 2:";

                TextBlockCDInterpret.Text = "Titel 1:";
                TextBlockCDTitle.Text = "Titel 2:";

                textTrackInterpret.IsReadOnly = false;
                textTrackInterpret.ToolTip = null;
            }
            else
            {
                TextBlockInterpret.Text = "Interpret:";
                TextBlockTitel.Text = "Titel:";

                TextBlockCDInterpret.Text = "Interpret:";
                TextBlockCDTitle.Text = "Titel:";

                textTrackInterpret.IsReadOnly = true;
                textTrackInterpret.ToolTip = StringTable.ReadOnlyBecauseOfNoSampler;
            }

            this.textCDInterpret.IsReadOnly = Settings.Current.SamplerUseFixedArtist && CD.Sampler;
            this.textInterpret.IsReadOnly = Settings.Current.SamplerUseFixedArtist && CD.Sampler;

            if (!string.IsNullOrEmpty(CD.Artist) && !CD.Sampler)
            {
                tabItemMember.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                tabItemMember.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(CD.Identity))
            {
                comboBoxCDTracks.IsEnabled = false;
            }

            this.ButtonDeleteTrack.IsEnabled = (dataTracks.SelectedIndex >= 0);

            StackPanelAddTracks.Visibility = (CD.Tracks.Count == 0) ? Visibility.Visible : Visibility.Collapsed;
            tabItemTrack.Visibility = (CD.Tracks.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void buttonTrackPrevTrack_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTrackIndex > 0)
                CurrentTrackIndex--;
        }

        private void buttonTrackNextTrack_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTrackIndex < this.CD.Tracks.Count - 1)
                CurrentTrackIndex++;
        }


        private void ButtonMultiLineEdit_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow row = VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
            DataGridCell cell = VisualTreeExtensions.FindParent<DataGridCell>(e.OriginalSource as DependencyObject);

            Field field = (Field)cell.Column.GetValue(Big3.Hitbase.Controls.DataGridExtensions.FieldProperty);
            Track track = row.DataContext as Track;

            WindowMultiline wm = new WindowMultiline();
            wm.Owner = Window.GetWindow(this);
            object textValue = track.GetValueByField(field);
            if (textValue != null)
                wm.textBox.Text = textValue.ToString();
            wm.Title = dataBase.GetNameOfField(field);
            if (wm.ShowDialog() == true)
            {
                cell.IsEditing = true;
                track.SetValueToField(field, wm.textBox.Text);
                cell.IsEditing = false;
            }
        }

        private void DockPanelMultiLineEdit_MouseEnter(object sender, RoutedEventArgs e)
        {
            Button row = VisualTreeExtensions.FindVisualChildByName<Button>(e.OriginalSource as DependencyObject, "MultiLineEditButton");
            row.Visibility = System.Windows.Visibility.Visible;
        }

        private void DockPanelMultiLineEdit_MouseLeave(object sender, RoutedEventArgs e)
        {
            Button row = VisualTreeExtensions.FindVisualChildByName<Button>(e.OriginalSource as DependencyObject, "MultiLineEditButton");
            row.Visibility = System.Windows.Visibility.Collapsed;

        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void buttonTrackMusikdatei_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = StringTable.ChooseSoundfile;
            ofd.Filter = StringTable.FilterSoundfile;
            ofd.FileName = textTrackMusikdatei.Text;
            if (ofd.ShowDialog(Window.GetWindow(this)) == true)
            {
                textTrackMusikdatei.Text = ofd.FileName;
            }
        }

        private void buttonCDTogglebuttonCDSET_Checked(object sender, RoutedEventArgs e)
        {
            if (CD.CDSetNumber == 0)
                CD.CDSetNumber = 1;

            UpdateWindowState();
        }

        private void buttonCDTogglebuttonCDSET_Unchecked(object sender, RoutedEventArgs e)
        {
            CD.CDSetNumber = 0;
            CD.CDSetName = "";

            UpdateWindowState();
        }

        private void buttonMitwirkendeAdd_Click(object sender, RoutedEventArgs e)
        {
            Participant newParticipant = new Participant();

            WindowParticipant windowParticipant = new WindowParticipant(dataBase, newParticipant, false);
            windowParticipant.Owner = Window.GetWindow(this);
            if (windowParticipant.ShowDialog() == true)
            {
                cd.Participants.Add(newParticipant);
            }
        }

        private void buttonMitwirkendeEdit_Click(object sender, RoutedEventArgs e)
        {
            EditParticipant();
        }

        private void EditParticipant()
        {
            Participant p = GetSelectedParticipant();

            WindowParticipant windowParticipant = new WindowParticipant(dataBase, p, true);
            windowParticipant.Owner = Window.GetWindow(this);
            windowParticipant.ShowDialog();
        }

        private void buttonMitwirkendeDel_Click(object sender, RoutedEventArgs e)
        {
            Participant p = GetSelectedParticipant();

            CD.Participants.Remove(p);
        }

        private Participant GetSelectedParticipant()
        {
            Participant p;

            if (Settings.Current.ShowParticipantPictures)
            {
                p = listboxParticipantPicture.SelectedItem as Participant;
            }
            else
            {
                p = dataGridParticipant.SelectedItem as Participant;
            }

            return p;
        }

        private void buttonCDArchivNR_Click(object sender, RoutedEventArgs e)
        {
            CD.ArchiveNumber = DataBase.GetNextFreeArchiveNumber();
        }

        private void dataGridParticipant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void dataGridParticipant_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject) != null)
            {
                EditParticipant();
            }
        }

        private void ButtonShowParticipantsTable_Click(object sender, RoutedEventArgs e)
        {
            Settings.Current.ShowParticipantPictures = false;

            SetParticipantsView();
        }

        private void LoadConfiguration()
        {
            this.trackListColumns = ColumnFieldCollection.LoadFromRegistry("OverviewTrackListColumns", defaultTrackListColumns);
        }

        /// <summary>
        /// Speichert die Spalten-Konfiguration in der Registry unter dem
        /// angegebenen Namen
        /// </summary>
        /// <param name="name"></param>
        public void SaveConfiguration()
        {
            ColumnFieldCollection newCols = new ColumnFieldCollection();

            foreach (DataGridColumn col in dataTracks.Columns.OrderBy(x => x.DisplayIndex))
            {
                Field field = (Field)col.GetValue(Big3.Hitbase.Controls.DataGridExtensions.FieldProperty);
                newCols.Add(new ColumnField(field, (int)col.Width.DisplayValue));
            }

            this.trackListColumns = newCols;

            this.trackListColumns.SaveToRegistry("OverviewTrackListColumns");
        }


        private void SetParticipantsView()
        {
            if (Settings.Current.ShowParticipantPictures)
            {
                listboxParticipantPicture.ItemsSource = cd.Participants;
                listboxParticipantPicture.Visibility = System.Windows.Visibility.Visible;
                ButtonShowParticipantsTable.IsChecked = false;
                ButtonShowParticipantsPictures.IsChecked = true;
                dataGridParticipant.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                dataGridParticipant.ItemsSource = cd.Participants;
                listboxParticipantPicture.Visibility = System.Windows.Visibility.Collapsed;
                ButtonShowParticipantsTable.IsChecked = true;
                ButtonShowParticipantsPictures.IsChecked = false;
                dataGridParticipant.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void ButtonShowParticipantsPictures_Click(object sender, RoutedEventArgs e)
        {
            Settings.Current.ShowParticipantPictures = true;

            SetParticipantsView();
        }

        private void listboxParticipantPicture_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisualTreeExtensions.FindParent<ListBoxItem>(e.OriginalSource as DependencyObject) != null)
            {
                EditParticipant();
            }
        }

        private void listboxParticipantPicture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private int lastTrackIndex = -1;
        public void SetRecordProgress(int track, double percentage)
        {
            if (lastTrackIndex != track && lastTrackIndex >= 0)
            {
                DataGridRow rowLast = this.dataTracks.ItemContainerGenerator.ContainerFromIndex(lastTrackIndex) as DataGridRow;
                if (rowLast != null)
                {
                    rowLast.Background = null;
                    rowLast.ClearValue(DataGridRow.BackgroundProperty);
                }
            }

            lastTrackIndex = track;

            if (track >= 0)
            {
                DataGridRow row = this.dataTracks.ItemContainerGenerator.ContainerFromIndex(track) as DataGridRow;
                if (row != null)
                {
                    double percentage1 = Math.Max(0, percentage - 0.1);

                    LinearGradientBrush lgb = new LinearGradientBrush();
                    lgb.StartPoint = new Point(0, 0);
                    lgb.EndPoint = new Point(1, 0);
                    lgb.GradientStops.Add(new GradientStop(Colors.Transparent, 0));
                    lgb.GradientStops.Add(new GradientStop(Colors.Transparent, percentage1));
                    lgb.GradientStops.Add(new GradientStop(Color.FromArgb(0, 50, 255, 50), percentage1));
                    lgb.GradientStops.Add(new GradientStop(Color.FromArgb(255, 50, 255, 50), percentage));
                    lgb.GradientStops.Add(new GradientStop(Colors.Transparent, percentage));
                    lgb.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
                    row.Background = lgb;
                }
            }
        }

        private void tabItemTrack_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.PageDown)
            {
                if (CurrentTrackIndex < cd.Tracks.Count-1)
                    CurrentTrackIndex++;
                e.Handled = true;
            }

            if (e.Key == Key.PageUp)
            {
                if (CurrentTrackIndex > 0)
                    CurrentTrackIndex--;
                e.Handled = true;
            }
                
        }

        private void buttonTrackSucheLyrics_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker bw = new BackgroundWorker();
            this.IsEnabled = false;
            this.waitProgress1.Visibility = System.Windows.Visibility.Visible;
            bw.DoWork += delegate
            {
                string lyrics = LyricsSearch.Search(this.CurrentTrack.Artist, this.CurrentTrack.Title);
                if (!string.IsNullOrEmpty(lyrics))
                    CurrentTrack.Lyrics = lyrics;
            };
            bw.RunWorkerCompleted += delegate
            {
                this.IsEnabled = true;
                this.waitProgress1.Visibility = System.Windows.Visibility.Collapsed;
            };
            bw.RunWorkerAsync();

        }

        internal void ShowTrack(int showTrack)
        {
            CurrentTrackIndex = showTrack;
            tabControl1.SelectedIndex = 2;
        }

        private void buttonComposer_Click(object sender, RoutedEventArgs e)
        {
            PersonGroup personGroup = DataBase.GetPersonGroupByName(textKomponist.Text, true);
            PersonGroupWindow personGroupWindow = new PersonGroupWindow(DataBase, PersonType.Composer, personGroup);
            personGroupWindow.Owner = Window.GetWindow(this);
            if (personGroupWindow.ShowDialog() == true)
            {
                textKomponist.Text = personGroup.Name;
            }
        }

        private void CommandBindingOpenTrackLocation_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            HitbaseCommands.OpenTrackLocation.Execute(e.Parameter, System.Windows.Application.Current.MainWindow);
        }

        private void buttonTrackBeats_Click(object sender, RoutedEventArgs e)
        {
            FormCalcBPM formCalcBpm = new FormCalcBPM();

            if (formCalcBpm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentTrack.Bpm = formCalcBpm.BPM;
            }
        }

        private void textInterpret_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTabCaption();

            if (!string.IsNullOrEmpty(CD.Artist) && !CD.Sampler)
            {
                tabItemMember.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                tabItemMember.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void textTitel_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTabCaption();
        }

        private void buttonTrackInterpret_Click(object sender, RoutedEventArgs e)
        {
            EditTrackArtist();
        }

        private void EditTrackArtist()
        {
            PersonGroup personGroup = DataBase.GetPersonGroupByName(textTrackInterpret.Text, true);
            PersonGroupWindow personGroupWindow = new PersonGroupWindow(DataBase, PersonType.Artist, personGroup);
            personGroupWindow.Owner = Window.GetWindow(this);

            if (personGroupWindow.ShowDialog() == true)
            {
                textTrackInterpret.Text = personGroup.Name;
            }
        }

        private void buttonTrackKomponist_Click(object sender, RoutedEventArgs e)
        {
            EditTrackComposer();
        }

        private void EditTrackComposer()
        {
            PersonGroup personGroup = DataBase.GetPersonGroupByName(textTrackKomponist.Text, true);
            PersonGroupWindow personGroupWindow = new PersonGroupWindow(DataBase, PersonType.Composer, personGroup);
            personGroupWindow.Owner = Window.GetWindow(this);
            if (personGroupWindow.ShowDialog() == true)
            {
                textTrackKomponist.Text = personGroup.Name;
            }
        }

        private void textTrackKomponist_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Mitwirkende
            if (e.OriginalSource is TabControl && tabControl1.SelectedItem == tabItemMember && e.AddedItems.Count > 0)
            {
                PersonGroup pg = DataBase.GetPersonGroupByName(CD.Artist, false);
                artistParticipants.PersonGroup = pg;
            }

            if (e.OriginalSource is TabControl && tabControl1.SelectedItem == tabItemCD && e.AddedItems.Count > 0)
            {
                this.GridCD.InvalidateArrange();// Arrange();
                this.GridCD.InvalidateMeasure();// Arrange();
                this.GridCD.UpdateLayout();
            }
        }

        private void CommandBindingOpenTrackLocation_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = HitbaseCommands.OpenTrackLocation.CanExecute(e.Parameter, Application.Current.MainWindow);
        }

        private void ButtonAddTrack_Click(object sender, RoutedEventArgs e)
        {
            CD.Tracks.Add(new Track());

            UpdateWindowState();
        }

        private void ButtonDeleteTrack_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = this.dataTracks.SelectedIndex;

            if (selectedIndex >= 0)
            {
                CD.Tracks.RemoveAt(selectedIndex);
                UpdateWindowState();
            }
        }

        private void dataTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void dataTracks_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject) != null)
            {
                if (dataTracks.SelectedItem != null)
                {
                    HitbaseCommands.AddTracksToPlaylistNow.Execute(((Track)dataTracks.SelectedItem).ID, this);

                    e.Handled = true;
                }
            }
        }

        private void CommandBindingAddTracksToPlaylistNow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Window.GetWindow(this).GetType() == typeof(WindowAlbum))
            {
                HitbaseCommands.AddTracksToPlaylistNow.Execute(e.Parameter, Application.Current.MainWindow);
                e.Handled = true;
            }
            else
            {
                HitbaseCommands.AddTracksToPlaylistNow.Execute(e.Parameter, (IInputElement)this.Parent);
            }
        }

        private void CommandBindingAddTracksToPlaylistNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CD.Type != AlbumType.AudioCD)
            {
                HitbaseCommands.AddTracksToPlaylistNext.Execute(e.Parameter, Application.Current.MainWindow);
                e.Handled = true;
            }
            else
            {
                HitbaseCommands.AddTracksToPlaylistNext.Execute(e.Parameter, (IInputElement)this.Parent);
            }
        }

        private void CommandBindingAddTracksToPlaylistLast_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CD.Type != AlbumType.AudioCD)
            {
                HitbaseCommands.AddTracksToPlaylistLast.Execute(e.Parameter, Application.Current.MainWindow);
                e.Handled = true;
            }
            else
            {
                HitbaseCommands.AddTracksToPlaylistLast.Execute(e.Parameter, (IInputElement)this.Parent);
            }
        }

        private void CommandBindingTrackInformation_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int trackIndex = CD.FindTrackIndexByTrackID((int)e.Parameter);

            ShowTrack(trackIndex);
        }

        private bool CheckArchiveNumber()
        {
            if (Configuration.Settings.Current.NoDuplicateArchiveNumbers)
            {
                int recordFound = this.DataBase.CheckArchiveNumber(this.CD.ArchiveNumber);

                if (recordFound > 0 && recordFound != this.CD.ID)           // Doppelte Archiv-Nummer gefunden!!
                {
                    MessageBox.Show(StringTable.ArchiveNumberAlreadyExists, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }

            return true;
        }

        private void textCDArchivNr_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!CheckArchiveNumber())
            {
                e.Handled = true;
                textCDArchivNr.Focus();
            }
        }

        private void textArchivNr_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!CheckArchiveNumber())
            {
                e.Handled = true;
                textArchivNr.Focus();
            }
        }

        private void comboBoxCDCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string oldValue = "";

            if (!IsLoaded)
                return;

            if (e.RemovedItems.Count > 0)
            {
                Category oldCategory = e.RemovedItems[0] as Category;
                oldValue = oldCategory.Name;
            }

            // Genre der CD auf die Tracks übertragen
            foreach (Track track in this.CD.Tracks)
            {
                if (string.IsNullOrEmpty(track.Category) || (oldValue != "" && track.Category == oldValue))
                {
                    track.Category = CD.Category;
                }
            }
        }

        private void ScrollViewerCD_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ScrollViewerCD.ActualHeight < 500)
            {
                ScrollViewerCD.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else
            {
                ScrollViewerCD.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
        }

        private void ScrollViewerTrack_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ScrollViewerTrack.ActualHeight < 500)
            {
                if (ScrollViewerTrack.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled)
                {
                    ScrollViewerTrack.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    textTrackComment.MaxHeight = textTrackComment.ActualHeight;
                    textBoxTrackLyrics.MaxHeight = textBoxTrackLyrics.ActualHeight;
                }
            }
            else
            {
                if (ScrollViewerTrack.VerticalScrollBarVisibility == ScrollBarVisibility.Auto)
                {
                    ScrollViewerTrack.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    textTrackComment.ClearValue(TextBox.MaxHeightProperty);
                    textBoxTrackLyrics.ClearValue(TextBox.MaxHeightProperty);
                }
            }
        }

        private int oldYearRecorded = -1;
        private void textCDAufnahmeJahr_GotFocus(object sender, RoutedEventArgs e)
        {
            oldYearRecorded = CD.YearRecorded;
        }

        private void textCDAufnahmeJahr_LostFocus(object sender, RoutedEventArgs e)
        {
            // Aufnahmejahr der CD auf die Tracks übertragen, falls im Track nichts eingetragen ist, oder der alte Wert der CD eingetragen war.
            foreach (Track track in this.CD.Tracks)
            {
                if (CD.YearRecorded == 0 || (oldYearRecorded != -1 && track.YearRecorded == oldYearRecorded))
                {
                    track.YearRecorded = CD.YearRecorded;
                }
            }
        }

        private string oldLanguage = "";
        private void comboBoxCDSprache_GotFocus(object sender, RoutedEventArgs e)
        {
            oldLanguage = CD.Language;
        }

        private void comboBoxCDSprache_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = e.OriginalSource as TextBox;

            if (comboBoxCDSprache.Template.FindName("PART_EditableTextBox", comboBoxCDSprache) == tb)
            {
                // Sprache der CD auf die Tracks übertragen, falls im Track nichts eingetragen ist, oder der alte Wert der CD eingetragen war.
                foreach (Track track in this.CD.Tracks)
                {
                    if (string.IsNullOrEmpty(track.Language) || (oldLanguage != "" && track.Language == oldLanguage))
                    {
                        track.Language = CD.Language;
                    }
                }
            }
        }

        private int lastActiveTrackIndex = -1;
        public void SetActiveTrack(PlaylistItem playlistItem)
        {
            // Suchen, ob ein CD-Track gerade gespielt wird und dann fest hinterlegen
            int track = -1;

            if (playlistItem != null)
            {
                string filename = playlistItem.Info.Filename;
                string[] parts = filename.Split(':');
                if (parts.Length == 3)
                {
                    track = Convert.ToInt32(parts[2]);
                }
            }

            if (lastActiveTrackIndex != track && lastActiveTrackIndex >= 0)
            {
                DataGridRow rowLast = this.dataTracks.ItemContainerGenerator.ContainerFromIndex(lastActiveTrackIndex) as DataGridRow;
                if (rowLast != null)
                {
                    rowLast.ClearValue(DataGridRow.FontWeightProperty);
                }
            }

            lastActiveTrackIndex = track;

            if (track >= 0)
            {
                DataGridRow row = this.dataTracks.ItemContainerGenerator.ContainerFromIndex(track) as DataGridRow;
                if (row != null)
                {
                    row.FontWeight = FontWeights.Bold;
                }
            }
            
        }
    }

    public class ParticipantImageConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = (string)value[0];
            DataBase db = (DataBase)value[1];

            PersonGroupDataSet.PersonGroupRow row = db.GetPersonGroupRowByName(name, false);

            if (row != null && row.ImageFilename != null && File.Exists(Misc.FindCover(row.ImageFilename)))
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bi.UriSource = new Uri(Misc.FindCover(row.ImageFilename));
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.DecodePixelWidth = 100;
                bi.EndInit();
                bi.Freeze();
                return bi;
            }
            else
            {
                return ImageLoader.FromResource("MissingPersonGroupImage.png");
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class DataGridCodesColumn : DataGridTextColumn
    {
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            Big3.Hitbase.Controls.TextBoxCodesWPF textBox = new Controls.TextBoxCodesWPF();
            textBox.BorderThickness = new Thickness(0);
            textBox.Padding = new Thickness(0);
            textBox.SetBinding(Big3.Hitbase.Controls.TextBoxCodesWPF.TextProperty, this.Binding);
            return textBox;
            //return base.GenerateEditingElement(cell, dataItem);
        }
    }

    /*
    public class MyTextBox : TextBox
    {
        protected override Size MeasureOverride(Size constraint)
        {
            Size origSize = base.MeasureOverride(constraint);
            origSize.Height = MinHeight;
            return origSize;
        }
    }*/
}
