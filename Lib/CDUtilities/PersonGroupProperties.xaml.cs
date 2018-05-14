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
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.DataBaseEngine;
using System.Diagnostics;
using Big3.Hitbase.SharedResources;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using Big3.Hitbase.Controls;
using System.Globalization;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for PersonGroupProperties.xaml
    /// </summary>
    public partial class PersonGroupProperties : UserControl, IModalUserControl, INotifyPropertyChanged
    {
        private string originalPersonGroupName;
        SortedList slCountry = new SortedList();

        ObservableCollection<Role> roleList = null;

        public PersonGroupProperties()
        {
            InitializeComponent();

            fillCountryList();

            Loaded += new RoutedEventHandler(PersonGroupProperties_Loaded);
        }

        //Pseudo-Variablen: Brauch ich fürs Validating
        public string DateOfBirth { get; set; }
        public string DateOfDeath { get; set; }

        private void CreateDataGridColumns()
        {
            DataGridComboBoxColumn roleColumn = new DataGridComboBoxColumn();
            roleColumn.Header = StringTable.Role;
            roleColumn.Width = 150;
            roleColumn.ItemsSource = roleList;
            roleColumn.DisplayMemberPath = "Name";
            roleColumn.SelectedValuePath = "Name";
            roleColumn.SelectedValueBinding = new Binding("Role");
            this.dataGridParticipants.Columns.Add(roleColumn);

            DataGridPersonGroupTextColumn nameColumn = new DataGridPersonGroupTextColumn();
            nameColumn.DataBase = dataBase;
            nameColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            nameColumn.Header = StringTable.Name;
            nameColumn.Binding = new Binding("Name");
            this.dataGridParticipants.Columns.Add(nameColumn);

            DataGridTextColumn startColumn = new DataGridTextColumn();
            startColumn.Width = 100;
            startColumn.Header = StringTable.Begin;
            startColumn.Binding = new Binding("Begin") { Converter = new DateConverter() };
            this.dataGridParticipants.Columns.Add(startColumn);

            DataGridTextColumn endColumn = new DataGridTextColumn();
            endColumn.Width = 100;
            endColumn.Header = StringTable.End;
            endColumn.Binding = new Binding("End") { Converter = new DateConverter() };
            this.dataGridParticipants.Columns.Add(endColumn);
        }

        void PersonGroupProperties_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataBase == null)
                return;

            roleList = dataBase.GetAllRoles();

            CreateDataGridColumns();

            // get list of currently used Items
            string[] usedcontries = DataBase.GetAvailableCountries();

            // Fill ComboBox with used countries
            foreach (string usedcountry in usedcontries)
            {
//                usedcountryItems.Add(new CountryItem() { CountryCode = (string)slCountry[usedcountry], CountryName = usedcountry });
                comboBoxCountry.Items.Add(new CountryItem() { CountryCode = (string)slCountry[usedcountry], CountryName = usedcountry });
            }

            comboBoxCountry.Items.Add(new CountryItem() { CountryName = "" });
            
            // Fill combobox with all available countries
            foreach (DictionaryEntry country in slCountry)
            {
                string buf = (string)country.Key;
                if (buf.Length > 2)
                {
                    comboBoxCountry.Items.Add(new CountryItem() { CountryCode = (string)country.Value, CountryName = (string)country.Key, PersonGroupProperties = this });
                }
            }

            //comboBoxCountry.ItemsSource = cc;

            originalPersonGroupName = PersonGroup.Name;

            Fill();

            SetFlag();

            UpdateWindowState();
        }

        private void Fill()
        {
            comboBoxType.ItemsSource = DataBase.GetAvailablePersonGroupTypes();
            comboBoxSex.ItemsSource = DataBase.GetAvailablePersonGroupSex();

            choosePictureUserControl.CoverType = CoverType.PersonGroup;
            choosePictureUserControl.PersonGroup = PersonGroup;
        }

        public PersonType PersonType { get; set; }

        private PersonGroup personGroup;
        public PersonGroup PersonGroup 
        { 
            get
            {
                return personGroup;
            }
            set
            {
                personGroup = value;
                this.DataContext = personGroup;
                this.dataGridParticipants.ItemsSource = personGroup.Participants;
                this.dataGridLinks.ItemsSource = personGroup.Urls;
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
                choosePictureUserControl.DataBase = value;
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(this, new EventArgs());
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (!Save())
                return;

            if (OKClicked != null)
                OKClicked(this, new EventArgs());
        }

        private bool Save()
        {
            if (String.Compare(originalPersonGroupName, PersonGroup.Name, true) != 0)
            {
                // JUS 16.10.2004: Prüfen, ob Interpret schon vorhanden 
                PersonGroup personGroup = DataBase.GetPersonGroupByName(PersonGroup.Name, false);

                if (personGroup != null)
                {
                    String str;

                    str = String.Format(StringTable.ArtistAlreadyExists, PersonGroup.Name, originalPersonGroupName);

                    MessageBox.Show(str, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }

            PersonGroup.Save(DataBase);

            return true;
        }

        #region IModalUserControl Members

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;

        #endregion

        private void textBoxName_LostFocus(object sender, RoutedEventArgs e)
        {
            // Wenn man nur zwei Wörter eingegeben hat, setzen wir bei "Speichern unter" jetzt erstmal 
            // standardmäßig die getauschten Wörter (z.B. "Bryan Adams" -> "Adams, Bryan"

            string[] words = this.textBoxName.Text.Split(' ');
            if (textBoxSaveAs.Text.Length < 1)
            {
                if (words.Length == 2)
                {
                    PersonGroup.SaveAs = words[1] + ", " + words[0];
                }
                else
                {
                    PersonGroup.SaveAs = this.textBoxName.Text;
                }
            }

            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonOK.IsEnabled = !String.IsNullOrEmpty(PersonGroup.Name);

            PersonGroupType groupType = (PersonGroupType)comboBoxType.SelectedIndex;
            if (groupType == PersonGroupType.Single)
            {
                textBlockBirthDay.Text = StringTable.Born + ":";
                textBlockDeath.Text = StringTable.Died + ":";
            }
            else
            {
                textBlockBirthDay.Text = StringTable.Founded + ":";
                textBlockDeath.Text = StringTable.BreakAway + ":";
            }

            this.ButtonDeleteLink.IsEnabled = (dataGridLinks.SelectedIndex >= 0);
            this.ButtonDeleteParticipant.IsEnabled = (dataGridParticipants.SelectedIndex >= 0);

            this.ButtonOpenPersonGroup.IsEnabled = false;
            GroupParticipant groupParticipant = dataGridParticipants.SelectedItem as GroupParticipant;
            if (groupParticipant != null)
            {
                PersonGroup personGroup = dataBase.GetPersonGroupByName(groupParticipant.Name, false);
                if (personGroup != null)
                {
                    this.ButtonOpenPersonGroup.IsEnabled = true;
                }
            }

        }

        private void buttonUrl_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PersonGroup.Homepage))
            {
                try
                {
                    Process.Start(PersonGroup.Homepage);
                }
                catch
                {       // Exception wird ignoriert
                }
            }
        }

        private void comboBoxCountry_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetFlag();
        }

        private void SetFlag()
        {
            // Hier Image setzen
            try
            {
                CountryItem selectedcountryItem = null;
                
                foreach (CountryItem ci in comboBoxCountry.Items)
                {
                    if (ci.CountryName == PersonGroup.Country)
                    {
                        selectedcountryItem = ci;
                        break;
                    }
                }

                if (selectedcountryItem == null)
                    return;

                string bitmapResource = "pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/Flags/150/" + selectedcountryItem.CountryCode + ".png";
                Uri uri = new Uri(bitmapResource, UriKind.RelativeOrAbsolute);
                try
                {
                    ImageSource imgSource = new BitmapImage(uri);
                    imageFlag.Source = imgSource;
                }
                catch
                {
                    ImageSource imgSource = null;
                    imageFlag.Source = imgSource;
                }
            }
            catch
            {
                imageFlag.Source = null;
            }
        }

        private void ButtonAddParticipant_Click(object sender, RoutedEventArgs e)
        {
            PersonGroup.Participants.Add(new GroupParticipant());
        }

        private void ButtonDeleteParticipant_Click(object sender, RoutedEventArgs e)
        {
            DeleteParticipant();
        }

        private void DeleteParticipant()
        {
            int selectedIndex = dataGridParticipants.SelectedIndex;

            GroupParticipant selectedParticipant = dataGridParticipants.SelectedItem as GroupParticipant;
            if (selectedParticipant != null)
            {
                PersonGroup.Participants.Remove(selectedParticipant);
                if (selectedIndex >= PersonGroup.Participants.Count)
                    dataGridParticipants.SelectedIndex = PersonGroup.Participants.Count - 1;
                else
                    dataGridParticipants.SelectedIndex = selectedIndex;
            }
        }

        private void ButtonAddLink_Click(object sender, RoutedEventArgs e)
        {
            PersonGroup.Urls.Add(new Url());
        }

        private void ButtonDeleteLink_Click(object sender, RoutedEventArgs e)
        {
            DeleteLink();
        }

        private void DeleteLink()
        {
            int selectedIndex = dataGridLinks.SelectedIndex;

            Url selectedUrl = dataGridLinks.SelectedItem as Url;
            if (selectedUrl != null)
            {
                PersonGroup.Urls.Remove(selectedUrl);
                if (selectedIndex >= PersonGroup.Urls.Count)
                    dataGridLinks.SelectedIndex = PersonGroup.Urls.Count - 1;
                else
                    dataGridLinks.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// Fill all known countries to sorted List - Index for both types - 2 byte Code and Country-Code
        /// </summary>
        void fillCountryList()
        {
            slCountry.Add("af", "Afghanistan");
            slCountry.Add("Afghanistan", "af");
            slCountry.Add("eg", "Ägypten");
            slCountry.Add("Ägypten", "eg");
            slCountry.Add("ax", "Åland");
            slCountry.Add("Åland", "ax");
            slCountry.Add("al", "Albanien");
            slCountry.Add("Albanien", "al");
            slCountry.Add("dz", "Algerien");
            slCountry.Add("Algerien", "dz");
            slCountry.Add("as", "Amerikanisch-Samoa");
            slCountry.Add("Amerikanisch-Samoa", "as");
            slCountry.Add("vi", "Amerikanische Jungferninseln");
            slCountry.Add("Amerikanische Jungferninseln", "vi");
            slCountry.Add("ad", "Andorra");
            slCountry.Add("Andorra", "ad");
            slCountry.Add("ao", "Angola");
            slCountry.Add("Angola", "ao");
            slCountry.Add("ai", "Anguilla");
            slCountry.Add("Anguilla", "ai");
            //countryItems.Add(new CountryItem() { CountryCode = "aq", CountryName = "Antarktis" });
            slCountry.Add("ag", "Antigua und Barbuda");
            slCountry.Add("Antigua und Barbuda", "ag");
            slCountry.Add("gq", "Äquatorialguinea");
            slCountry.Add("Äquatorialguinea", "gq");
            slCountry.Add("ar", "Argentinien");
            slCountry.Add("Argentinien", "ar");
            slCountry.Add("am", "Armenien");
            slCountry.Add("Armenien", "am");
            slCountry.Add("aw", "Aruba");
            slCountry.Add("Aruba", "aw");
            //countryItems.Add(new CountryItem() { CountryCode = "ac", CountryName = "Ascension" });
            slCountry.Add("az", "Aserbaidschan");
            slCountry.Add("Aserbaidschan", "az");
            slCountry.Add("et", "Äthiopien");
            slCountry.Add("Äthiopien", "et");
            slCountry.Add("au", "Australien");
            slCountry.Add("Australien", "au");
            slCountry.Add("bs", "Bahamas");
            slCountry.Add("Bahamas", "bs");
            slCountry.Add("bh", "Bahrain");
            slCountry.Add("Bahrain", "bh");
            slCountry.Add("bd", "Bangladesch");
            slCountry.Add("Bangladesch", "bd");
            slCountry.Add("bb", "Barbados");
            slCountry.Add("Barbados", "bb");
            //slCountry.Add("by", "Belarus");
            slCountry.Add("Belarus", "by");
            slCountry.Add("be", "Belgien");
            slCountry.Add("Belgien", "be");
            slCountry.Add("bz", "Belize");
            slCountry.Add("Belize", "bz");
            slCountry.Add("bj", "Benin");
            slCountry.Add("Benin", "bj");
            slCountry.Add("bm", "Bermuda");
            slCountry.Add("Bermuda", "bm");
            slCountry.Add("bt", "Bhutan");
            slCountry.Add("Bhutan", "bt");
            slCountry.Add("bo", "Bolivien");
            slCountry.Add("Bolivien", "bo");
            slCountry.Add("ba", "Bosnien und Herzegowina");
            slCountry.Add("Bosnien und Herzegowina", "ba");
            slCountry.Add("bw", "Botswana");
            slCountry.Add("Botswana", "bw");
            slCountry.Add("br", "Brasilien");
            slCountry.Add("Brasilien", "br");
            slCountry.Add("vg", "Britische Jungferninseln");
            slCountry.Add("Britische Jungferninseln", "vg");
            slCountry.Add("bn", "Brunei Darussalam");
            slCountry.Add("Brunei Darussalam", "bn");
            slCountry.Add("bg", "Bulgarien");
            slCountry.Add("Bulgarien", "bg");
            slCountry.Add("bf", "Burkina Faso");
            slCountry.Add("Burkina Faso", "bf");
            //countryItems.Add(new CountryItem() { CountryCode = "bu", CountryName = "Burma" });
            slCountry.Add("bi", "Burundi");
            slCountry.Add("Burundi", "bi");
            //countryItems.Add(new CountryItem() { CountryCode = "ea", CountryName = "Ceuta, Melilla" });
            slCountry.Add("cl", "Chile");
            slCountry.Add("Chile", "cl");
            slCountry.Add("cn", "China");
            slCountry.Add("China", "cn");
            //countryItems.Add(new CountryItem() { CountryCode = "cp", CountryName = "Clipperton" });
            slCountry.Add("ck", "Cookinseln");
            slCountry.Add("Cookinseln", "ck");
            slCountry.Add("cr", "Costa Rica");
            slCountry.Add("Costa Rica", "cr");
            //slCountry.Add("ci", "Côte d'Ivoire");
            slCountry.Add("Côte d'Ivoire", "ci");
            slCountry.Add("dk", "Dänemark");
            slCountry.Add("Dänemark", "dk");
            slCountry.Add("de", "Deutschland");
            slCountry.Add("Deutschland", "de");
            //countryItems.Add(new CountryItem() { CountryCode = "dg", CountryName = "Diego Garcia" });
            slCountry.Add("dm", "Dominica");
            slCountry.Add("Dominica", "dm");
            slCountry.Add("do", "Dominikanische Republik");
            slCountry.Add("Dominikanische Republik", "do");
            slCountry.Add("dj", "Dschibuti");
            slCountry.Add("Dschibuti", "dj");
            slCountry.Add("ec", "Ecuador");
            slCountry.Add("Ecuador", "ec");
            slCountry.Add("sv", "El Salvador");
            slCountry.Add("El Salvador", "sv");
            slCountry.Add("ci", "Elfenbeinküste");
            slCountry.Add("Elfenbeinküste", "ci");
            //slCountry.Add("gb", "England");
            slCountry.Add("England", "gb");
            slCountry.Add("er", "Eritrea");
            slCountry.Add("Eritrea", "er");
            slCountry.Add("ee", "Estland");
            slCountry.Add("Estland", "ee");
            //countryItems.Add(new CountryItem() { CountryCode = "ce", CountryName = "Europäische Gemeinschaft" });
            //countryItems.Add(new CountryItem() { CountryCode = "eu", CountryName = "Europäische Union" });
            slCountry.Add("fk", "Falklandinseln");
            slCountry.Add("Falklandinseln", "fk");
            slCountry.Add("fo", "Färöer");
            slCountry.Add("Färöer", "fo");
            slCountry.Add("fj", "Fidschi");
            slCountry.Add("Fidschi", "fj");
            slCountry.Add("fi", "Finnland");
            slCountry.Add("Finnland", "fi");
            slCountry.Add("fr", "Frankreich");
            slCountry.Add("Frankreich", "fr");
            slCountry.Add("gf", "Französisch-Guayana");
            slCountry.Add("Französisch-Guayana", "gf");
            slCountry.Add("pf", "Französisch-Polynesien");
            slCountry.Add("Französisch-Polynesien", "pf");
            //countryItems.Add(new CountryItem() { CountryCode = "tf", CountryName = "Französische Süd- und Antarktisgebiete" });
            slCountry.Add("ga", "Gabun");
            slCountry.Add("Gabun", "ga");
            slCountry.Add("gm", "Gambia");
            slCountry.Add("Gambia", "gm");
            slCountry.Add("ge", "Georgien");
            slCountry.Add("Georgien", "ge");
            slCountry.Add("gh", "Ghana");
            slCountry.Add("Ghana", "gh");
            slCountry.Add("gi", "Gibraltar");
            slCountry.Add("Gibraltar", "gi");
            slCountry.Add("gd", "Grenada");
            slCountry.Add("Grenada", "gd");
            slCountry.Add("gr", "Griechenland");
            slCountry.Add("Griechenland", "gr");
            slCountry.Add("gl", "Grönland");
            slCountry.Add("Grönland", "gl");
            slCountry.Add("gb", "Großbritannien");
            slCountry.Add("Großbritannien", "gb");
            slCountry.Add("gp", "Guadeloupe");
            slCountry.Add("Guadeloupe", "gp");
            slCountry.Add("gu", "Guam");
            slCountry.Add("Guam", "gu");
            slCountry.Add("gt", "Guatemala");
            slCountry.Add("Guatemala", "gt");
            //countryItems.Add(new CountryItem() { CountryCode = "gg", CountryName = "Guernsey (Kanalinsel)" });
            slCountry.Add("gn", "Guinea");
            slCountry.Add("Guinea", "gn");
            slCountry.Add("gw", "Guinea-Bissau");
            slCountry.Add("Guinea-Bissau", "gw");
            slCountry.Add("gy", "Guyana");
            slCountry.Add("Guyana", "gy");
            slCountry.Add("ht", "Haiti");
            slCountry.Add("Haiti", "ht");
            //countryItems.Add(new CountryItem() { CountryCode = "hm", CountryName = "Heard und McDonaldinseln" });
            slCountry.Add("hn", "Honduras");
            slCountry.Add("Honduras", "hn");
            slCountry.Add("hk", "Hongkong");
            slCountry.Add("Hongkong", "hk");
            slCountry.Add("in", "Indien");
            slCountry.Add("Indien", "in");
            slCountry.Add("id", "Indonesien");
            slCountry.Add("Indonesien", "id");
            //countryItems.Add(new CountryItem() { CountryCode = "im", CountryName = "Insel Man" });
            slCountry.Add("iq", "Irak");
            slCountry.Add("Irak", "iq");
            slCountry.Add("ir", "Iran");
            slCountry.Add("Iran", "ir");
            slCountry.Add("ie", "Irland");
            slCountry.Add("Irland", "ie");
            slCountry.Add("is", "Island");
            slCountry.Add("Island", "is");
            slCountry.Add("il", "Israel");
            slCountry.Add("Israel", "il");
            slCountry.Add("it", "Italien");
            slCountry.Add("Italien", "it");
            slCountry.Add("jm", "Jamaika");
            slCountry.Add("Jamaika", "jm");
            slCountry.Add("jp", "Japan");
            slCountry.Add("Japan", "jp");
            slCountry.Add("ye", "Jemen");
            slCountry.Add("Jemen", "ye");
            //countryItems.Add(new CountryItem() { CountryCode = "je", CountryName = "Jersey (Kanalinsel)" });
            slCountry.Add("jo", "Jordanien");
            slCountry.Add("Jordanien", "jo");
            //countryItems.Add(new CountryItem() { CountryCode = "yu", CountryName = "Jugoslawien" });
            slCountry.Add("ky", "Kaimaninseln");
            slCountry.Add("Kaimaninseln", "ky");
            slCountry.Add("kh", "Kambodscha");
            slCountry.Add("Kambodscha", "kh");
            slCountry.Add("cm", "Kamerun");
            slCountry.Add("Kamerun", "cm");
            slCountry.Add("ca", "Kanada");
            slCountry.Add("Kanada", "ca");
            //countryItems.Add(new CountryItem() { CountryCode = "ic", CountryName = "Kanarische Inseln" });
            slCountry.Add("cv", "Kap Verde");
            slCountry.Add("Kap Verde", "cv");
            slCountry.Add("kz", "Kasachstan");
            slCountry.Add("Kasachstan", "kz");
            slCountry.Add("qa", "Katar");
            slCountry.Add("Katar", "qa");
            slCountry.Add("ke", "Kenia");
            slCountry.Add("Kenia", "ke");
            slCountry.Add("kg", "Kirgisistan");
            slCountry.Add("Kirgisistan", "kg");
            slCountry.Add("ki", "Kiribati");
            slCountry.Add("Kiribati", "ki");
            slCountry.Add("cc", "Kokosinseln");
            slCountry.Add("Kokosinseln", "cc");
            slCountry.Add("co", "Kolumbien");
            slCountry.Add("Kolumbien", "co");
            slCountry.Add("km", "Komoren");
            slCountry.Add("Komoren", "km");
            slCountry.Add("cd", "Kongo");
            slCountry.Add("Kongo", "cd");
            slCountry.Add("cg", "Republik Kongo");
            slCountry.Add("Republik Kongo", "cg");
            //slCountry.Add("kp", "Korea, Demokratische Volksrepublik");
            slCountry.Add("Korea, Demokratische Volksrepublik", "kp");
            //slCountry.Add("kr", "Korea, Republik");
            slCountry.Add("Korea, Republik", "kr");
            slCountry.Add("hr", "Kroatien");
            slCountry.Add("Kroatien", "hr");
            slCountry.Add("cu", "Kuba");
            slCountry.Add("Kuba", "cu");
            slCountry.Add("kw", "Kuwait");
            slCountry.Add("Kuwait", "kw");
            slCountry.Add("la", "Laos");
            slCountry.Add("Laos", "la");
            slCountry.Add("ls", "Lesotho");
            slCountry.Add("Lesotho", "ls");
            slCountry.Add("lv", "Lettland");
            slCountry.Add("Lettland", "lv");
            slCountry.Add("lb", "Libanon");
            slCountry.Add("Libanon", "lb");
            slCountry.Add("lr", "Liberia");
            slCountry.Add("Liberia", "lr");
            slCountry.Add("ly", "Libyen");
            slCountry.Add("Libyen", "ly");
            slCountry.Add("li", "Liechtenstein");
            slCountry.Add("Liechtenstein", "li");
            slCountry.Add("lt", "Litauen");
            slCountry.Add("Litauen", "lt");
            slCountry.Add("lu", "Luxemburg");
            slCountry.Add("Luxemburg", "lu");
            slCountry.Add("mo", "Macao");
            slCountry.Add("Macao", "mo");
            slCountry.Add("mg", "Madagaskar");
            slCountry.Add("Madagaskar", "mg");
            slCountry.Add("mw", "Malawi");
            slCountry.Add("Malawi", "mw");
            slCountry.Add("my", "Malaysia");
            slCountry.Add("Malaysia", "my");
            slCountry.Add("mv", "Malediven");
            slCountry.Add("Malediven", "mv");
            slCountry.Add("ml", "Mali");
            slCountry.Add("Mali", "ml");
            slCountry.Add("mt", "Malta");
            slCountry.Add("Malta", "mt");
            slCountry.Add("ma", "Marokko");
            slCountry.Add("Marokko", "ma");
            slCountry.Add("mh", "Marshallinseln");
            slCountry.Add("Marshallinseln", "mh");
            slCountry.Add("mq", "Martinique");
            slCountry.Add("Martinique", "mq");
            slCountry.Add("mr", "Mauretanien");
            slCountry.Add("Mauretanien", "mr");
            slCountry.Add("mu", "Mauritius");
            slCountry.Add("Mauritius", "mu");
            slCountry.Add("yt", "Mayotte");
            slCountry.Add("Mayotte", "yt");
            slCountry.Add("mk", "Mazedonien");
            slCountry.Add("Mazedonien", "mk");
            slCountry.Add("mx", "Mexiko");
            slCountry.Add("Mexiko", "mx");
            slCountry.Add("fm", "Mikronesien");
            slCountry.Add("Mikronesien", "fm");
            slCountry.Add("md", "Moldawien");
            slCountry.Add("Moldawien", "md");
            slCountry.Add("mc", "Monaco");
            slCountry.Add("Monaco", "mc");
            slCountry.Add("mn", "Mongolei");
            slCountry.Add("Mongolei", "mn");
            slCountry.Add("me", "Montenegro");
            slCountry.Add("Montenegro", "me");
            slCountry.Add("ms", "Montserrat");
            slCountry.Add("Montserrat", "ms");
            slCountry.Add("mz", "Mosambik");
            slCountry.Add("Mosambik", "mz");
            slCountry.Add("mm", "Myanmar");
            slCountry.Add("Myanmar", "mm");
            slCountry.Add("na", "Namibia");
            slCountry.Add("Namibia", "na");
            slCountry.Add("nr", "Nauru");
            slCountry.Add("Nauru", "nr");
            slCountry.Add("np", "Nepal");
            slCountry.Add("Nepal", "np");
            slCountry.Add("nc", "Neukaledonien");
            slCountry.Add("Neukaledonien", "nc");
            slCountry.Add("nz", "Neuseeland");
            slCountry.Add("Neuseeland", "nz");
            slCountry.Add("ni", "Nicaragua");
            slCountry.Add("Nicaragua", "ni");
            slCountry.Add("nl", "Niederlande");
            slCountry.Add("Niederlande", "nl");
            slCountry.Add("an", "Niederländische Antillen");
            slCountry.Add("Niederländische Antillen", "an");
            slCountry.Add("ne", "Niger");
            slCountry.Add("Niger", "ne");
            slCountry.Add("ng", "Nigeria");
            slCountry.Add("Nigeria", "ng");
            slCountry.Add("nu", "Niue");
            slCountry.Add("Niue", "nu");
            //slCountry.Add("gb", "Nordirland");
            slCountry.Add("Nordirland", "gb");
            slCountry.Add("kp", "Nordkorea");
            slCountry.Add("Nordkorea", "kp");
            slCountry.Add("mp", "Nördliche Marianen");
            slCountry.Add("Nördliche Marianen", "mp");
            slCountry.Add("nf", "Norfolkinsel");
            slCountry.Add("Norfolkinsel", "nf");
            slCountry.Add("no", "Norwegen");
            slCountry.Add("Norwegen", "no");
            slCountry.Add("om", "Oman");
            slCountry.Add("Oman", "om");
            slCountry.Add("at", "Österreich");
            slCountry.Add("Österreich", "at");
            slCountry.Add("tl", "Osttimor");
            slCountry.Add("Osttimor", "tl");
            slCountry.Add("pk", "Pakistan");
            slCountry.Add("Pakistan", "pk");
            slCountry.Add("ps", "Palästinensische Autonomiegebiete");
            slCountry.Add("Palästinensische Autonomiegebiete", "ps");
            slCountry.Add("pw", "Palau");
            slCountry.Add("Palau", "pw");
            slCountry.Add("pa", "Panama");
            slCountry.Add("Panama", "pa");
            slCountry.Add("pg", "Papua-Neuguinea");
            slCountry.Add("Papua-Neuguinea", "pg");
            slCountry.Add("py", "Paraguay");
            slCountry.Add("Paraguay", "py");
            slCountry.Add("pe", "Peru");
            slCountry.Add("Peru", "pe");
            slCountry.Add("ph", "Philippinen");
            slCountry.Add("Philippinen", "ph");
            slCountry.Add("pn", "Pitcairninseln");
            slCountry.Add("Pitcairninseln", "pn");
            slCountry.Add("pl", "Polen");
            slCountry.Add("Polen", "pl");
            slCountry.Add("pt", "Portugal");
            slCountry.Add("Portugal", "pt");
            slCountry.Add("pr", "Puerto Rico");
            slCountry.Add("Puerto Rico", "pr");
            //slCountry.Add("tw", "Republik China");
            slCountry.Add("Republik China", "tw");
            slCountry.Add("re", "Réunion");
            slCountry.Add("Réunion", "re");
            slCountry.Add("rw", "Ruanda");
            slCountry.Add("Ruanda", "rw");
            slCountry.Add("ro", "Rumänien");
            slCountry.Add("Rumänien", "ro");
            //slCountry.Add("ru", "Russische Föderation");
            slCountry.Add("Russische Föderation", "ru");
            slCountry.Add("ru", "Russland");
            slCountry.Add("Russland", "ru");
            slCountry.Add("sb", "Salomonen");
            slCountry.Add("Salomonen", "sb");
            //countryItems.Add(new CountryItem() { CountryCode = "bl", CountryName = "Saint-Barthélemy" });
            //countryItems.Add(new CountryItem() { CountryCode = "mf", CountryName = "Saint-Martin (franz. Teil)" });
            slCountry.Add("zm", "Sambia");
            slCountry.Add("Sambia", "zm");
            slCountry.Add("ws", "Samoa");
            slCountry.Add("Samoa", "ws");
            slCountry.Add("sm", "San Marino");
            slCountry.Add("San Marino", "sm");
            slCountry.Add("st", "São Tomé und Príncipe");
            slCountry.Add("São Tomé und Príncipe", "st");
            slCountry.Add("sa", "Saudi-Arabien");
            slCountry.Add("Saudi-Arabien", "sa");
            slCountry.Add("se", "Schweden");
            slCountry.Add("Schweden", "se");
            slCountry.Add("ch", "Schweiz");
            slCountry.Add("Schweiz", "ch");
            slCountry.Add("sn", "Senegal");
            slCountry.Add("Senegal", "sn");
            slCountry.Add("rs", "Serbien");
            slCountry.Add("Serbien", "rs");
            //countryItems.Add(new CountryItem() { CountryCode = "cs", CountryName = "Serbien und Montenegro (ehemalig)" });
            slCountry.Add("sc", "Seychellen");
            slCountry.Add("Seychellen", "sc");
            slCountry.Add("sl", "Sierra Leone");
            slCountry.Add("Sierra Leone", "sl");
            slCountry.Add("zw", "Simbabwe");
            slCountry.Add("Simbabwe", "zw");
            slCountry.Add("sg", "Singapur");
            slCountry.Add("Singapur", "sg");
            slCountry.Add("sk", "Slowakei");
            slCountry.Add("Slowakei", "sk");
            slCountry.Add("si", "Slowenien");
            slCountry.Add("Slowenien", "si");
            slCountry.Add("so", "Somalia");
            slCountry.Add("Somalia", "so");
            slCountry.Add("es", "Spanien");
            slCountry.Add("Spanien", "es");
            slCountry.Add("lk", "Sri Lanka");
            slCountry.Add("Sri Lanka", "lk");
            //countryItems.Add(new CountryItem() { CountryCode = "sh", CountryName = "St. Helena" });
            slCountry.Add("kn", "St. Kitts und Nevis");
            slCountry.Add("St. Kitts und Nevis", "kn");
            slCountry.Add("lc", "St. Lucia");
            slCountry.Add("St. Lucia", "lc");
            slCountry.Add("pm", "Saint-Pierre und Miquelon");
            slCountry.Add("Saint-Pierre und Miquelon", "pm");
            slCountry.Add("vc", "St. Vincent und die Grenadinen");
            slCountry.Add("St. Vincent und die Grenadinen", "vc");
            slCountry.Add("za", "Südafrika");
            slCountry.Add("Südafrika", "za");
            slCountry.Add("kr", "Südkorea");
            slCountry.Add("Südkorea", "kr");
            slCountry.Add("sd", "Sudan");
            slCountry.Add("Sudan", "sd");
            slCountry.Add("gs", "Südgeorgien und die Südlichen Sandwichinseln");
            slCountry.Add("Südgeorgien und die Südlichen Sandwichinseln", "gs");
            slCountry.Add("sr", "Suriname");
            slCountry.Add("Suriname", "sr");
            //countryItems.Add(new CountryItem() { CountryCode = "sj", CountryName = "Svalbard und Jan Mayen" });
            slCountry.Add("sz", "Swasiland");
            slCountry.Add("Swasiland", "sz");
            slCountry.Add("sy", "Syrien");
            slCountry.Add("Syrien", "sy");
            slCountry.Add("tj", "Tadschikistan");
            slCountry.Add("Tadschikistan", "tj");
            slCountry.Add("tw", "Taiwan");
            slCountry.Add("Taiwan", "tw");
            slCountry.Add("tz", "Tansania");
            slCountry.Add("Tansania", "tz");
            slCountry.Add("th", "Thailand");
            slCountry.Add("Thailand", "th");
            slCountry.Add("tg", "Togo");
            slCountry.Add("Togo", "tg");
            slCountry.Add("tk", "Tokelau");
            slCountry.Add("Tokelau", "tk");
            slCountry.Add("to", "Tonga");
            slCountry.Add("Tonga", "to");
            slCountry.Add("tt", "Trinidad und Tobago");
            slCountry.Add("Trinidad und Tobago", "tt");
            //countryItems.Add(new CountryItem() { CountryCode = "ta", CountryName = "Tristan da Cunha" });
            slCountry.Add("td", "Tschad");
            slCountry.Add("Tschad", "td");
            slCountry.Add("cz", "Tschechische Republik");
            slCountry.Add("Tschechische Republik", "cz");
            //countryItems.Add(new CountryItem() { CountryCode = "cs", CountryName = "Tschechoslowakei" });
            slCountry.Add("tn", "Tunesien");
            slCountry.Add("Tunesien", "tn");
            slCountry.Add("tr", "Türkei");
            slCountry.Add("Türkei", "tr");
            slCountry.Add("tm", "Turkmenistan");
            slCountry.Add("Turkmenistan", "tm");
            slCountry.Add("tc", "Turks- und Caicosinseln");
            slCountry.Add("Turks- und Caicosinseln", "tc");
            slCountry.Add("tv", "Tuvalu");
            slCountry.Add("Tuvalu", "tv");
            //countryItems.Add(new CountryItem() { CountryCode = "su", CountryName = "UdSSR" });
            slCountry.Add("ug", "Uganda");
            slCountry.Add("Uganda", "ug");
            slCountry.Add("ua", "Ukraine");
            slCountry.Add("Ukraine", "ua");
            slCountry.Add("hu", "Ungarn");
            slCountry.Add("Ungarn", "hu");
            slCountry.Add("um", "United States Minor Outlying Islands");
            slCountry.Add("United States Minor Outlying Islands", "um");
            slCountry.Add("uy", "Uruguay");
            slCountry.Add("Uruguay", "uy");
            slCountry.Add("us", "USA");
            slCountry.Add("USA", "us");
            slCountry.Add("uz", "Usbekistan");
            slCountry.Add("Usbekistan", "uz");
            slCountry.Add("vu", "Vanuatu");
            slCountry.Add("Vanuatu", "vu");
            slCountry.Add("va", "Vatikanstadt");
            slCountry.Add("Vatikanstadt", "va");
            slCountry.Add("ve", "Venezuela");
            slCountry.Add("Venezuela", "ve");
            slCountry.Add("ae", "Vereinigte Arabische Emirate");
            slCountry.Add("Vereinigte Arabische Emirate", "ae");
            //slCountry.Add("us", "Vereinigte Staaten von Amerika");
            slCountry.Add("Vereinigte Staaten von Amerika", "us");
            //slCountry.Add("gb", "Vereinigtes Königreich Großbritannien und Nordirland");
            slCountry.Add("Vereinigtes Königreich Großbritannien und Nordirland", "gb");
            slCountry.Add("vn", "Vietnam");
            slCountry.Add("Vietnam", "vn");
            slCountry.Add("wf", "Wallis und Futuna");
            slCountry.Add("Wallis und Futuna", "wf");
            slCountry.Add("cx", "Weihnachtsinsel");
            slCountry.Add("Weihnachtsinsel", "cx");
            slCountry.Add("by", "Weißrussland");
            slCountry.Add("Weißrussland", "by");
            slCountry.Add("eh", "Westsahara");
            slCountry.Add("Westsahara", "eh");
            //countryItems.Add(new CountryItem() { CountryCode = "zr", CountryName = "Zaire (jetzt Demokratische Republik Kongo)" });
            slCountry.Add("cf", "Zentralafrikanische Republik");
            slCountry.Add("Zentralafrikanische Republik", "cf");
            slCountry.Add("cy", "Zypern");
            slCountry.Add("Zypern", "cy");
        }

        private void comboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateWindowState();
        }
        private string convertDate(string orgDate)
        {
            if (orgDate == null)
                return "";

            orgDate = orgDate.Replace("-", "");
            orgDate = orgDate.PadRight(8, '0');
            if (orgDate == "00000000")
                return "";
            return orgDate;
        }

        private void buttonSearchData_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            waitProgress1.Visibility = System.Windows.Visibility.Visible;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                SearchOnline();
            };
            bw.RunWorkerCompleted += delegate
            {
                this.IsEnabled = true;
                waitProgress1.Visibility = System.Windows.Visibility.Collapsed;
            };
            bw.RunWorkerAsync();
        }

        private void SearchOnline()
        {
            MusicBrainzData mbdata = new MusicBrainzData();
            ResultMusicBrainzArtist mbart = new ResultMusicBrainzArtist();
            /*if (personGroup.Urls.Count > 0 || personGroup.Participants.Count > 0)
            {
                if (MessageBox.Show("Es sind bereits Daten erfasst, sollen diese überschrieben werden?", "Daten vorhanden", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }*/
            mbart = mbdata.SearchMusicBrainzArtistData(PersonGroup.Name);

            if (mbart != null && mbart.mbArtists.Count() > 0)
            {
                if (mbart.mbArtists[0].Score == "100")
                {
                }
                MusicBrainzArtist mba = new MusicBrainzArtist();
                mba = mbdata.GetMusicBrainzArtistByID(mbart.mbArtists[0].MBID);
                //

                PersonGroup.Birthday = convertDate(mba.BeginDate);
                PersonGroup.DayOfDeath = convertDate(mba.EndDate);
                PersonGroup.Homepage = mba.Homepage;
                PersonGroup.SaveAs = mba.SortName;
                if (!string.IsNullOrEmpty(mba.Country))
                    PersonGroup.Country = mba.Country.ToLower();
                if (!string.IsNullOrEmpty(mba.Gender) && mba.Gender.Length > 0)
                {
                    if (mba.Gender.ToLower() == "female")
                        PersonGroup.Sex = SexType.Feminin;

                    if (mba.Gender.ToLower() == "male")
                        PersonGroup.Sex = SexType.Masculin;
                }

                if (mba.Type.Length > 0)
                {
                    if (mba.Type.ToLower() == "group")
                        PersonGroup.Type = PersonGroupType.Group;
                    else
                        PersonGroup.Type = PersonGroupType.Single;
                }
                
                foreach (MusicBrainzArtistBaseData mbb in mba.BandMember)
                {
                    GroupParticipant gp = new GroupParticipant();
                    if (string.IsNullOrEmpty(mbb.Type))
                        gp.Role = "Bandmitglied";
                    else
                        gp.Role = mbb.Type;                    
                    
                    gp.Begin = convertDate(mbb.BeginGroupMember);
                    if (string.IsNullOrEmpty(gp.Begin) && mba.BeginDate.Length > 0)
                    {
                        if (mba.Type.ToLower() == "group" && mbb.Type.ToLower() == "bandmitglied")
                            gp.Begin = convertDate(mba.BeginDate);
                        if (mba.Type.ToLower() == "person" && mbb.Type.ToLower() == "person")
                            gp.Begin = convertDate(mba.BeginDate);
                    }

                    gp.End = convertDate(mbb.EndGroupMember);
                    if (string.IsNullOrEmpty(gp.End) && mba.EndDate.Length > 0)
                    {
                        if (mba.Type.ToLower() == "group" && mbb.Type.ToLower() == "bandmitglied")
                            gp.End = convertDate(mba.EndDate);
                        if (mba.Type.ToLower() == "person" && mbb.Type.ToLower() == "person")
                            gp.End = convertDate(mba.EndDate);
                    }

                    gp.Name = mbb.ArtistName;


                    if (roleList.FirstOrDefault(x => x.Name == gp.Role) == null)
                    {
                        roleList.Add(new Role() { Name = gp.Role });
                    }

                    PersonGroup pg = dataBase.GetPersonGroupByName(mbb.ArtistName, false);
                    if (pg == null)
                    {
                        PersonGroup newPerson = new PersonGroup();

                        newPerson.Birthday = convertDate(mbb.BeginDate);
                        newPerson.DayOfDeath = convertDate(mbb.EndDate);
                        newPerson.SaveAs = mbb.SortName;
                        newPerson.Name = mbb.ArtistName;
                        newPerson.Type = PersonGroupType.Single;

                        newPerson.Save(dataBase);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(pg.SaveAs))
                            pg.SaveAs = mbb.SortName;
                        if (mbb.SortName != pg.SaveAs && mbb.ArtistName == pg.Name)
                            pg.SaveAs = mbb.SortName;
                        if (string.IsNullOrEmpty(pg.Birthday))
                            pg.Birthday = convertDate(mbb.BeginDate);

                        if (string.IsNullOrEmpty(pg.DayOfDeath))
                            pg.DayOfDeath = convertDate(mbb.EndDate);

                        pg.Save(dataBase);
                    }


                    //gp.Role
                    bool changedpg = false;

                    foreach (GroupParticipant gpold in personGroup.Participants)
                    {
                        if (gpold.Name == gp.Name && gpold.Role == gp.Role)
                        {
                            changedpg = true;
                            if (gpold.Begin != gp.Begin)
                                gpold.Begin = gp.Begin;
                            if (gpold.End != gp.End)
                                gpold.End = gp.End;
                        }
                    }
                    if (changedpg == false)
                        personGroup.Participants.AddItemFromThread(gp);
                }

                foreach (MusicbrainzURLs mbu in mba.miscURLs)
                {
                    bool donotadd = false;
                    foreach (Url pgurl in personGroup.Urls)
                    {
                        if (pgurl.Link == mbu.Target && pgurl.UrlType == mbu.Type)
                            donotadd = true;
                    }
                    if (donotadd == false)
                    {
                        Url ul = new Url();
                        ul.Link = mbu.Target;
                        ul.UrlType = mbu.Type;
                        personGroup.Urls.AddItemFromThread(ul);
                    }
                }

                // Now check for top level domains and guess country
                // Old
                // if (!string.IsNullOrEmpty(PersonGroup.Homepage) && PersonGroup.Homepage.LastIndexOf(".") > 0 && string.IsNullOrEmpty(PersonGroup.Country))

                string tldName="";

                if (!string.IsNullOrEmpty(mba.Country))
                    tldName = mba.Country.ToLower();
                if (tldName.Length > 1)
                {
                    if (tldName == "uk")
                    {
                        tldName = "gb";
                    }
                    foreach (CountryItem ci in comboBoxCountry.Items)
                    {
                        if (ci.CountryCode == tldName)
                        {
                            PersonGroup.Country = ci.CountryName;
                            break;
                        }
                    }
                }
            }
        }

        private void ButtonLinksUrl_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow row = VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject);

            if (row != null)
            {
                Url url = row.DataContext as Url;

                try
                {
                    if (url != null)
                        Process.Start(url.Link);
                }
                catch
                {       // Exception wird ignoriert
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void dataGridParticipants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void dataGridLinks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void dataGridParticipants_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && e.OriginalSource is DataGridCell)
            {
                DeleteParticipant();
                dataGridParticipants.Focus();
                e.Handled = true;
            }
        }

        private void dataGridLinks_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteLink();
                dataGridLinks.Focus();
            }
        }

        private void ButtonOpenPersonGroup_Click(object sender, RoutedEventArgs e)
        {
            OpenPersonGroup();
        }

        private void OpenPersonGroup()
        {
            GroupParticipant groupParticipant = dataGridParticipants.SelectedItem as GroupParticipant;
            if (groupParticipant != null)
            {
                PersonGroup personGroup = dataBase.GetPersonGroupByName(groupParticipant.Name, false);
                if (personGroup != null)
                {
                    PersonGroupWindow personGroupWindow = new PersonGroupWindow(this.dataBase, PersonType.Unknown, personGroup);
                    personGroupWindow.ShowDialog();
                }
            }
        }

        private void dataGridParticipants_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                OpenPersonGroup();
                e.Handled = true;
            }
        }
    }

    public class CountryItem : INotifyPropertyChanged
    {
        internal PersonGroupProperties PersonGroupProperties { get; set; }

        private string countryCode;

        public string CountryCode
        {
            get 
            { 
                return countryCode; 
            }
            set 
            {
                try
                {
                    countryCode = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("CountryCode"));
                        PropertyChanged(this, new PropertyChangedEventArgs("FlagImageSource"));
                    }
                }
                catch
                {
                }
            }
        }

        public ImageSource FlagImageSource
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(CountryCode))
                        return null;

                    return ImageLoader.FromResource(string.Format("Flags/{0}.png", CountryCode));
                }
                catch
                {
                    return null;
                }
            }
        }

        private string countryName;

        public string CountryName 
        {
            get
            {
                return countryName;
            }
            set
            {
                countryName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CountryName"));
            }
        }

        public override string ToString()
        {
            return CountryName;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler  PropertyChanged;

        #endregion
    }

    public class CountryItemSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            CountryItem ci = item as CountryItem;
            PersonGroupProperties pgp = ci.PersonGroupProperties;

            if (string.IsNullOrEmpty(ci.CountryName))
                return pgp.FindResource("CountryItemSeparatorTemplate") as DataTemplate;
            else
                return pgp.FindResource("CountryItemTemplate") as DataTemplate;
        }
    }  

    public class DataGridPersonGroupTextColumn : DataGridTextColumn
    {
        public DataBase DataBase { get; set; }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            TextBoxAutoComplete textBoxAutoComplete = new TextBoxAutoComplete();
            textBoxAutoComplete.DataBase = this.DataBase;
            textBoxAutoComplete.Margin = new Thickness(0);
            textBoxAutoComplete.Padding = new Thickness(0);
            textBoxAutoComplete.BorderThickness = new Thickness(0);
            textBoxAutoComplete.SetBinding(TextBoxAutoComplete.TextProperty, this.Binding);
            textBoxAutoComplete.AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;
            return textBoxAutoComplete;
            //return base.GenerateEditingElement(cell, dataItem);
        }
    }

    public class SexTypeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SexType)value;
        }
    }

    public class PersonGroupTypeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (PersonGroupType)value;
        }
    }

    public class MyComboBox : ComboBox
    {
        public event TextChangedEventHandler TextChanged;

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            TextBox txt = this.GetTemplateChild("PART_EditableTextBox") as TextBox;
            txt.TextChanged += new TextChangedEventHandler(txt_TextChanged);
        }

        void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextChanged != null)
                TextChanged(sender, e);
        }
    }
}


