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
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.DialogDataSetTableAdapters;
using Big3.Hitbase.MainWindowDesigner.Controls;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Big3.Hitbase.MainWindowDesigner.Model;
using System.Xml;

namespace Big3.Hitbase.MainWindowDesigner
{
    /// <summary>
    /// Interaction logic for MainCDUserControl.xaml
    /// </summary>
    public partial class MainCDUserControl : UserControl
    {
        public DataBase DataBase { get; set; }

        public HitbaseMainCDControl Model { get; set; }

        public CD theCd = null;

        public UndoEngine undoEngine;

        public MainCDUserControl()
        {
            InitializeComponent();

            Model = new HitbaseMainCDControl(this);
        }

        private bool isInDesignMode = false;
        public bool IsInDesignMode
        {
            get { return isInDesignMode; }
        }

        private bool readOnly = false;
        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; UpdateAllControls(); }
        }

        public void SetDataSource(DataBase db, CD cd)
        {
            DataBase = db;
            theCd = cd;

            UpdateAllControls();
        }


        public bool OpenDialogDesign(int categoryId)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Debug.WriteLine("1.: " + sw.ElapsedMilliseconds.ToString());
            DialogDataSet ds = new DialogDataSet();
            DialogTableAdapter dialogTableAdapter = new DialogTableAdapter(DataBase);

            Debug.WriteLine("2.: " + sw.ElapsedMilliseconds.ToString());
            dialogTableAdapter.FillByCategoryID(ds.Dialog, categoryId);

            Debug.WriteLine("3.: " + sw.ElapsedMilliseconds.ToString());
            if (ds.Dialog.Rows.Count < 1)         // nichts gefunden
                return false;

            Type[] extraTypes = new Type[] { typeof(Field) }; 
            //TODO_WPF!!!!!!!!!!!!!!!extraTypes[0] = typeof(TrackListColumn);
            XmlSerializer bf = new XmlSerializer(typeof(HitbaseDialogData), extraTypes);
            StringReader xmlString = new StringReader(ds.Dialog[0].DialogXML);
            HitbaseDialogData hitbaseDialogData = (HitbaseDialogData)bf.Deserialize(xmlString);

            xmlString.Close();

            Debug.WriteLine("4.: " + sw.ElapsedMilliseconds.ToString());
            ReadDialogFromDialogData(null, hitbaseDialogData);
            Debug.WriteLine("5.: " + sw.ElapsedMilliseconds.ToString());

            UpdateAllControls();
            Debug.WriteLine("6.: " + sw.ElapsedMilliseconds.ToString());

//TODO_WPF!!!!!!!!!!!!!            undoEngine.Initialize();

            return true;
        }

        /// <summary>
        /// Läd den Dialog aus dem angegebenen Dateinamen.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="objdefid"></param>
        /// <returns></returns>
        public bool OpenFromFilename(String filename)
        {
            try
            {
                String xmlDialog = File.ReadAllText(filename, Encoding.Default);

                Type[] extraTypes = new Type[0];
                //TODO_WPF!!!!!!!!!!!!!extraTypes[0] = typeof(TrackListColumn);
                XmlSerializer bf = new XmlSerializer(typeof(HitbaseDialogData), extraTypes);
                StringReader xmlString = new StringReader(xmlDialog);
                HitbaseDialogData hitbaseDialogData = (HitbaseDialogData)bf.Deserialize(xmlString);

                xmlString.Close();

                ReadDialogFromDialogData(null, hitbaseDialogData);

                UpdateAllControls();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return true;
            }
        }

        public bool ReadDialogFromDialogData(HitbaseControl parent, HitbaseControlData hitbaseControlData)
        {
            //int controlIndex = 0;
            foreach (HitbaseControlData ctl in hitbaseControlData.Controls)
            {
                Object newControl = null;

                if (ctl.ControlName != "MainWindowCDUserControl")
                {
                    switch (ctl.ControlName)
                    {
                        case "HitbaseTextBox": newControl = new HitbaseTextBox(this); break;
                        case "HitbaseLabel": newControl = new HitbaseLabel(this); break;
                        case "HitbaseButton": newControl = new HitbaseButton(this); break;
                        case "HitbaseComboBox": newControl = new HitbaseComboBox(this); break;
                        case "HitbaseCheckBox": newControl = new HitbaseCheckBox(this); break;
                        case "HitbaseRating": newControl = new HitbaseRating(this); break;
                        //TODO_WPF!!!!!!!!!!!!!!!!case "HitbaseTrackList": newControl = new HitbaseTrackList(this); break;
                        //TODO_WPF!!!!!!!!!!!!!!!!case "HitbaseParticipants": newControl = new HitbaseParticipants(this); break;
                        case "HitbaseSeperator": newControl = new HitbaseSeperator(this); break;
                        //TODO_WPF!!!!!!!!!!!!!!!!case "HitbaseCover": newControl = new HitbaseCover(this); break;
                        default:
                            //MessageBox.Show("unknown hitbase Control " + ctl.ControlName, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }
                }
                else
                {
                    newControl = this;
                }

                if (newControl != null)
                {
                    foreach (HitbaseControlData.Property prop in ctl.Properties)
                    {
                        PropertyInfo property = newControl.GetType().GetProperty(prop.Name);
                        if (property != null)
                        {
                            if (property.PropertyType == typeof(System.Drawing.Color))
                            {
                                string colorString = prop.Value.ToString();
                                if (colorString.StartsWith("#"))
                                {
                                    int R, G, B;
                                    R = Convert.ToInt32(colorString.Substring(1, 2), 16);
                                    G = Convert.ToInt32(colorString.Substring(3, 2), 16);
                                    B = Convert.ToInt32(colorString.Substring(5, 2), 16);

                                    //TODO_WPF!!!!!!!!!!!!!!!!!property.SetValue(newControl, Color.FromArgb(R, G, B), null);
                                }
                                else
                                {
                                    //TODO_WPF!!!!!!!!!!!!!!!!!property.SetValue(newControl, Color.FromName(colorString), null);
                                }
                            }
                            else
                            {
                                if (property.PropertyType == typeof(System.Drawing.Font) && prop.Value != null)
                                {
                                    property.SetValue(newControl, GetFontFromString(prop.Value.ToString()), null);
                                }
                                else
                                {
                                    property.SetValue(newControl, prop.Value, null);
                                }
                            }
                        }
                    }

                    if (newControl is HitbaseControl)
                    {
                        AddHitbaseControl(parent, 0, (HitbaseControl)newControl);

                        ReadDialogFromDialogData((HitbaseControl)newControl, ctl);
                    }
                }
            }

            return true;
        }

        public static System.Drawing.Font GetFontFromString(string fontString)
        {
            string[] token = fontString.Split(';');

            if (token.Length < 2)
                return null;

            string fontName = token[0];
            string fontSizeString = token[1].Trim();
            if (fontSizeString.IndexOf(' ') > 0)
                fontSizeString = fontSizeString.Substring(0, fontSizeString.IndexOf(' '));
            float fontSize = Convert.ToSingle(fontSizeString);

            System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;

            if (token.Length > 2)
            {
                string fontStyleString = token[2].Trim();

                if (fontStyleString.StartsWith("style=", true, null))
                {
                    fontStyleString = fontStyleString.Substring(6);
                    string[] styleToken = fontStyleString.Split(',');
                    foreach (string style in styleToken)
                    {
                        string styleTrim = style.Trim();
                        if (string.Compare(styleTrim, "bold", true) == 0)
                            fontStyle |= System.Drawing.FontStyle.Bold;
                        if (string.Compare(styleTrim, "italic", true) == 0)
                            fontStyle |= System.Drawing.FontStyle.Italic;
                        if (string.Compare(styleTrim, "underline", true) == 0)
                            fontStyle |= System.Drawing.FontStyle.Underline;
                    }
                }
            }

            System.Drawing.Font font = new System.Drawing.Font(token[0], fontSize, fontStyle);

            return font;
        }

        static public bool IsHitbaseDialog(string xmlData, ref string errorMessage)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlData);
                if (xmlDoc.SelectSingleNode("/HitbaseDialogData") == null)
                {
                    errorMessage = StringTable.HitbaseDialogDataNotFound;
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                // Exception, also wohl kein Hitbase-Dialog
                errorMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// Speichert das Dialogdesign für die ausgewählte Kategorie.
        /// </summary>
        /// <returns></returns>
        public bool SaveDialogDesign()
        {
            FormSaveDialog formSaveDialog = new FormSaveDialog(DataBase);

            if (formSaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return false;


            return SaveDialogDesign(formSaveDialog.SelectedCategoryId);
        }

        /// <summary>
        /// Speichert das Dialogdesign für die angegebene Kategorie.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public bool SaveDialogDesign(int categoryId)
        {
            string dialog = GetXMLStringFromDialog();

            DialogDataSet ds = new DialogDataSet();
            DialogTableAdapter dialogTableAdapter = new DialogTableAdapter(DataBase);

            dialogTableAdapter.FillByCategoryID(ds.Dialog, categoryId);

            if (ds.Dialog.Rows.Count == 0)      // Noch nicht vorhanden, also anlegen
                ds.Dialog.AddDialogRow(categoryId, dialog);
            else
                ds.Dialog[0].DialogXML = dialog;

            dialogTableAdapter.Update(ds.Dialog);
            return true;
        }


        /// <summary>
        /// Speichern den Dialog im Design-Modus in die angegebene Datei.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool SaveDialogToFilename(String filename)
        {
            HitbaseDialogData hitbaseDialog = new HitbaseDialogData();
            hitbaseDialog.Properties.Add(new HitbaseControlData.Property("Version", 1));
            SaveDialogToXMLRecursive(hitbaseDialog, this);

            Type[] extraTypes = new Type[0];
            //TODO_WPF!!!!!!!!!!!!!!extraTypes[0] = typeof(TrackListColumn);
            XmlSerializer bf = new XmlSerializer(typeof(HitbaseDialogData), extraTypes);
            StreamWriter stream = new StreamWriter(filename, false, Encoding.Default);
            bf.Serialize(stream, hitbaseDialog);

            stream.Close();

            return true;
        }

        public string GetXMLStringFromDialog()
        {
            HitbaseDialogData hitbaseDialog = new HitbaseDialogData();
            hitbaseDialog.Properties.Add(new HitbaseControlData.Property("Version", 1));

            hitbaseDialog.Add(this);

            SaveDialogToXMLRecursive(hitbaseDialog, this);

            Type[] extraTypes = new Type[0];
            //TODO_WPF!!!!!!!!!!!!!!!!!!!extraTypes[0] = typeof(TrackListColumn);
            XmlSerializer bf = new XmlSerializer(typeof(HitbaseDialogData), extraTypes);
            StringWriter stream = new StringWriter();
            bf.Serialize(stream, hitbaseDialog);
            stream.Close();

            return stream.ToString();
        }

        public void SaveDialogToXMLRecursive(HitbaseControlData ctlData, FrameworkElement parent)
        {
            // Von hinten nach vorne laufen, weil sonst die Reihenfolge umgedreht wird.
            /*TODO_WPF!!!!!!!!!!!!!!!!!for (int i = parent.Controls.Count - 1; i >= 0; i--)
            {
                if (parent.Controls[i] is IHitbaseControl)
                {
                    int index = ctlData.Add(((IHitbaseControl)parent.Controls[i]).HitbaseControl);

                    SaveDialogToXMLRecursive((HitbaseControlData)ctlData.Controls[index], parent.Controls[i]);
                }
            }*/
        }

        /// <summary>
        /// Das Hitbase-Objekt speichern.
        /// </summary>
        public bool Save()
        {
            try
            {
                // Vor dem Speichern alle Controls auffordern, Ihre Daten in das Hitbase-Objekt zu übertragen.
                SaveAllControlData();

                UpdateAllControls();

                //TODO_WPF!!!!!!!!!!!!!!!!!Modified = false;

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
        }

        public bool CurrentUpdatingAllControls = false;
        /// <summary>
        /// Alle Controls updaten (falls z.B. neue Daten vorliegen, oder die Sprache geändert wurde).
        /// </summary>
        public void UpdateAllControls()
        {
            CurrentUpdatingAllControls = true;
            try
            {
                List<HitbaseControl> allControls = GetAllControls();

                allControls.ForEach(x => x.UpdateControlData());

                UpdateAllControlStates();
            }
            finally
            {
                CurrentUpdatingAllControls = false;
            }
        }

        public void UpdateAllControlStates()
        {
            List<HitbaseControl> allControls = GetAllControls();

            allControls.ForEach(x => x.UpdateControlState());
        }

        /// <summary>
        /// Alle Controls auffordern, Ihre Daten persistent im Hitbase-Objekt zu speichern.
        /// </summary>
        public void SaveAllControlData()
        {
            List<HitbaseControl> allControls = GetAllControls();
            allControls.ForEach(x => x.SaveControlData());
        }

        /// <summary>
        /// Liefert alle Controls zurück.
        /// </summary>
        /// <returns></returns>
        public List<HitbaseControl> GetAllControls()
        {
            List<HitbaseControl> allControls = null;

            allControls = GetAllControlsOfType(typeof(HitbaseControl));

            return allControls;
        }

        /// <summary>
        /// Liefert alle Child-Controls des angegebenen Controls zurück.
        /// </summary>
        /// <returns></returns>
        public List<HitbaseControl> GetAllControls(HitbaseControl parent)
        {
            return GetAllControlsOfType(parent, typeof(HitbaseControl));
        }

        /// <summary>
        /// Liefert alle Controls der Seite zurück, die vom angegebenen Type sind.
        /// </summary>
        /// <param name="hitbaseControlType"></param>
        /// <returns></returns>
        public List<HitbaseControl> GetAllControlsOfType(Type hitbaseControlType)
        {
            List<HitbaseControl> controls = new List<HitbaseControl>();

            GetAllControlsOfTypeRecursive(controls, Model, hitbaseControlType);

            return controls;
        }

        /// <summary>
        /// Liefert alle Child-Controls dess angegebenen Controls zurück, die vom angegebenen Type sind.
        /// </summary>
        /// <param name="hitbaseControlType"></param>
        /// <returns></returns>
        public List<HitbaseControl> GetAllControlsOfType(HitbaseControl parent, Type hitbaseControlType)
        {
            List<HitbaseControl> controls = new List<HitbaseControl>();

            GetAllControlsOfTypeRecursive(controls, parent, hitbaseControlType);

            return controls;
        }

        public void GetAllControlsOfTypeRecursive(List<HitbaseControl> controls, HitbaseControl parent, Type hitbaseControlType)
        {
            foreach (HitbaseControl ctl in parent.Children)
            {
                if (ctl == null)
                    continue;
                if (ctl.GetType() == hitbaseControlType || ctl.GetType().IsSubclassOf(hitbaseControlType))
                    controls.Add(ctl);
                GetAllControlsOfTypeRecursive(controls, ctl, hitbaseControlType);
            }
        }

        /// <summary>
        /// Sucht das Control mit der angegebenen ID
        /// </summary>
        /// <param name="controlID"></param>
        /// <returns></returns>
        public HitbaseControl FindHitbaseControlFromID(int controlID)
        {
            if (controlID == 0)
                return null;

            List<HitbaseControl> allControls = GetAllControls();

            foreach (HitbaseControl ctl in allControls)
            {
                if (ctl.ControlID == controlID)
                    return ctl;
            }

            return null;
        }

        /// <summary>
        /// Lösche das angegebene Control mit allen Child Controls.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="ctl"></param>
        private void DeleteHitbaseControl(HitbaseControl parent, HitbaseControl ctl)
        {
            /*TODO_WPF!!!!!!!!!!!!!!!!!!!!!!!!!!if (thePropertyGrid != null && thePropertyGrid.SelectedObject == ctl)
            {
                thePropertyGrid.SelectedObject = null;
            }

            undoEngine.BeginUndoStep(StringTable.Delete);
            undoEngine.AddDelete(ctl);*/

            if (parent == null)
                ctl.RemoveFromControl(this);
            else
                parent.RemoveChild(ctl);
            
            //TODO_WPF!!!!!!!!!!!!!!!!!!undoEngine.EndUndoStep();
        }

        /// <summary>
        /// Lösche das angegebene Control mit allen Child Controls.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="ctl"></param>
        public void DeleteHitbaseControlWithChilds(HitbaseControl parent, HitbaseControl ctl)
        {
            //TODO_WPF!!!!!!!!!!!!!!undoEngine.BeginUndoStep(StringTable.Delete);

            DeleteAllChildHitbaseControls(ctl);

            DeleteHitbaseControl(parent, ctl);
            
            //TODO_WPF!!!!!!!!!!!!!!!undoEngine.EndUndoStep();
        }

        private void DeleteAllChildHitbaseControls(HitbaseControl parent)
        {
            List<Control> delControls = new List<Control>();
            foreach (Control ctl in parent.Controls)
            {
                if (ctl is IHitbaseControl)
                {
                    delControls.Add(ctl);
                    DeleteAllChildHitbaseControls(((IHitbaseControl)ctl).HitbaseControl);
                }
            }

            // Jetzt die Controls löschen
            foreach (Control ctl in delControls)
            {
                if (!(parent is IHitbaseControl))
                    DeleteHitbaseControl(null, ((IHitbaseControl)ctl).HitbaseControl);
                else
                    DeleteHitbaseControl(((IHitbaseControl)parent).HitbaseControl, ((IHitbaseControl)ctl).HitbaseControl);
            }
        }


        public void ChangeProperty(HitbaseControl ctl, string property, object value)
        {
            //TODO_WPF!!!!!!!!!!!!!undoEngine.BeginUndoStep(StringTable.PropertyChanged);
            PropertyInfo propInfo = ctl.GetType().GetProperty(property);

            //TODO_WPF!!!!!!!!!!!!!undoEngine.AddPropertyChange(ctl.ControlID, property, propInfo.GetValue(ctl, null));

            propInfo.SetValue(ctl, value, null);

            //TODO_WPF!!!!!!!!!!!!!undoEngine.EndUndoStep();
        }


        public HitbaseControl AddControlFromStream(int parentControlID, int controlIndex, byte[] objectStream, bool keepControlID)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream(objectStream);

            HitbaseControl parentCtl = FindHitbaseControlFromID(parentControlID);
            HitbaseControl ctl = (HitbaseControl)bf.Deserialize(mem);

            PropertyInfo[] propInfo = ctl.GetType().GetProperties();

            HitbaseControl newControl = Activator.CreateInstance(ctl.GetType(), this) as HitbaseControl;

            foreach (PropertyInfo pi in propInfo)
            {
                if (pi.CanWrite)
                    pi.SetValue(newControl, pi.GetValue(ctl, null), null);
            }

            if (keepControlID)
            {
                PropertyInfo piControlID = ctl.GetType().GetProperty("ControlID");
                newControl.SetControlID((int)piControlID.GetValue(ctl, null));
            }

            AddHitbaseControl(parentCtl, controlIndex, newControl);

            mem.Close();

            return newControl;
        }



        /// <summary>
        /// Die Farbeinstellungen für alle Controls updaten (falls sich ein Skin geändert hat)
        /// </summary>
        public void UpdateColorsAllControls()
        {
            List<HitbaseControl> allControls = GetAllControls();

            allControls.ForEach(x => x.UpdateColors());
        }

        /// <summary>
        /// Fügt das angegebene neue Hitbase-Control hinzu.
        /// </summary>
        /// <param name="parent">Das Parent, oder null, wenn das neue Control im Hauptframe hinzugefügt werden soll.</param>
        /// <param name="newControl">Das hinzuzufügende Control.</param>
        public void AddHitbaseControl(HitbaseControl parent, int controlIndex, HitbaseControl newControl)
        {
            if (parent == null)
            {
                this.Canvas.Children.Add(newControl.GetControl());
                if (controlIndex >= 0)
                    newControl.SetChildIndexToControl(this, controlIndex);
            }
            else
            {
                parent.Add(newControl);
                if (controlIndex >= 0)
                    parent.SetChildIndex(newControl, controlIndex);
            }

            newControl.ControlCreated();

            /*TODO_WPF!!!!!!!!!!!!!!!undoEngine.BeginUndoStep(StringTable.New);
            undoEngine.AddNew(newControl);
            undoEngine.EndUndoStep();*/
        }


    }
}
