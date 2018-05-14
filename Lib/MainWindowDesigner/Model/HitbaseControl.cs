using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using Big3.Hitbase.DataBaseEngine;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;

namespace Big3.Hitbase.MainWindowDesigner.Model
{
    public class IDGenerator
    {
        static int nextControlID = 1;
        static public int GetNextID()
        {
            return nextControlID++;
        }
    }


    public interface IHitbaseControl
    {
        HitbaseControl HitbaseControl
        {
            get;
        }
    }

    [Serializable]
    public abstract class HitbaseControl : ICustomTypeDescriptor
    {
        //TODO_WPF!!!!!!!!!!!!!public static SelectionBandControl firstActivatedSelectionControl = null;

        public HitbaseControl()
        {
        }

        //TODO_WPF!!!!!!!!!!!!![NonSerialized]
        //TODO_WPF!!!!!!!!!!!!!protected SelectionBandControl bandControl;

        [NonSerialized]
        public MainCDUserControl hitbaseMainWindowControl;

        private int controlID;

        public int ControlID
        {
            get { return controlID; }
        }

        public void SetControlID(int id)
        {
            controlID = id;
        }

        public HitbaseControl(MainCDUserControl dlg)
        {
            hitbaseMainWindowControl = dlg;

            //TODO_WPF!!!!!!!!!!!!!bandControl = new SelectionBandControl(this);
            //TODO_WPF!!!!!!!!!!!!!bandControl.Visible = false;
            //TODO_WPF!!!!!!!!!!!!!dlg.Controls.Add(bandControl);
            //TODO_WPF!!!!!!!!!!!!!dlg.Controls.SetChildIndex(bandControl, 0);

            controlID = IDGenerator.GetNextID();
        }

        /// <summary>
        /// Wird aufgerufen, nachdem das Control angelegt wurde. Wird benutzt, um Events vom Control
        /// abzufangen.
        /// </summary>
        public virtual void ControlCreated()
        {
/*TODO_WPF!!!!!!!!!!!!!!!            Control.Resize += new EventHandler(Control_Resize);

            // Rekursiv für alle Child-Controls Maus-Eventhandlers setzen
            SetMouseEventHandlers(Control);

            if (IsInDesignMode)
                Control.ContextMenuStrip = hitbaseMainWindowControl.contextMenuStrip;*/
        }

        protected virtual void SetMouseEventHandlers(FrameworkElement ctl)
        {
            /*TODO_WPF!!!!!!!!!!!ctl.MouseDown += HitbaseControl_MouseDown;

            foreach (Control ctlChild in ctl.Controls)
                SetMouseEventHandlers(ctlChild);*/
        }

        private bool selected;

        public bool Selected
        {
            get { return selected; }
            set 
            { 
                selected = value;

                if (hitbaseMainWindowControl != null && Control != null)
                {
                    if (selected)
                    {
                        /*TODO_WPF!!!!!!!!!!!!!!!!!Rectangle rectControl = SelectionBandControl.GetSelectionBandRectangleFromControl(hitbaseMainWindowControl, Control);
                        bandControl.Location = rectControl.Location;
                        bandControl.Size = rectControl.Size;
                        bandControl.Visible = true;
                        if (firstActivatedSelectionControl != null)
                            firstActivatedSelectionControl.Invalidate();
                        else
                            firstActivatedSelectionControl = bandControl;*/
                    }
                    else
                    {
                        //TODO_WPF!!!!!!!!!!!!!bandControl.Visible = false;
                    }
                }
            }
        }

        protected abstract FrameworkElement Control { get; }

        public abstract string ControlName { get; }

        /// <summary>
        /// Liefert true zurück, wenn sich der Dialog im Designmodus befindet.
        /// </summary>
        public virtual bool IsInDesignMode
        {
            get
            {
                return hitbaseMainWindowControl.IsInDesignMode;
            }
        }

        /// <summary>
        /// Füllt die Daten in das Control.
        /// </summary>
        public abstract void UpdateControlData();

        /// <summary>
        /// Das angegebene Control und alle Subcontrols aktualisieren
        /// </summary>
        /// <param name="parent"></param>
        public void UpdateControlDataRecursive()
        {
            UpdateControlData();
            UpdateControlState();

            foreach (HitbaseControl ctl in Children)
            {
                ctl.UpdateControlDataRecursive();
            }
        }

        /// <summary>
        /// Läd die aktuellen Daten aus dem Control und speichert sie.
        /// </summary>
        public abstract void SaveControlData();

        /// <summary>
        /// Läd die aktuellen Daten aus dem Control und allen untergeordneten Controls und speichert sie.
        /// </summary>
        /// <param name="parent"></param>
        public void SaveControlDataRecursive()
        {
            SaveControlData();

            foreach (HitbaseControl ctl in Children)
            {
                ctl.SaveControlData();
                ctl.SaveControlDataRecursive();
            }
        }

        /// <summary>
        /// Aktualisiert den Control-Status (z.B. Disabled/enabled)
        /// </summary>
        public virtual void UpdateControlState()
        {
            if (!IsInDesignMode)
            {
                if (ReadOnly)
                {
                    SetControlDisabled(true);
                }
                else
                {
                    if (IsControlDisabled())
                        SetControlDisabled(true);
                    else
                        SetControlDisabled(!Enabled);
                }
            }
        }

        public virtual void SetControlDisabled(bool disableControl)
        {
            if (IsInDesignMode)
                Control.IsEnabled = true;
            else
                Control.IsEnabled = !disableControl;
        }

        /// <summary>
        /// Prüfen, ob das Control disabled ist.
        /// </summary>
        /// <param name="ctl"></param>
        /// <returns></returns>
        public bool IsControlDisabled()
        {
            return hitbaseMainWindowControl.ReadOnly || !Enabled;
        }

        /// <summary>
        /// Designmodus wird aktiviert bzw. deaktiviert.
        /// </summary>
        /// <param name="activateDesignMode"></param>
        public virtual void SetDesignMode(bool activateDesignMode)
        {
            if (activateDesignMode)
            {
                //TODO_WPF!!!!!!!!!!!!!!Control.Enabled = true;
                //TODO_WPF!!!!!!!!!!!!!!Control.ContextMenuStrip = hitbaseMainWindowControl.contextMenuStrip;
            }
            else
            {
                //TODO_WPF!!!!!!!!!!!!!!Control.ContextMenuStrip = null;

                UpdateControlData();
                UpdateControlState();
            }
        }

        /// <summary>
        /// Liefert die Children-Controls zurück.
        /// </summary>
        [Category("helpLine"), ReadOnly(true)]
        public virtual ArrayList Children
        {
            get
            {
                return new ArrayList();
            }
        }

        public DataBase DataBase
        {
            get
            {
                return hitbaseMainWindowControl.DataBase;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return hitbaseMainWindowControl.ReadOnly;
            }
        }

        /// <summary>
        /// Linke Position des Controls.
        /// </summary>
        private int left;
        [Category("Hitbase")]
        public virtual int Left
        {
            get
            {
                return left;
            }
            set
            {
                Control.SetValue(Canvas.LeftProperty, (double)value);
                left = value;
            }
        }

        /// <summary>
        /// Obere Position des Controls.
        /// </summary>
        private int top;
        [Category("Hitbase")]
        public virtual int Top
        {
            get
            {
                return top;
            }
            set
            {
                Control.SetValue(Canvas.TopProperty, (double)value);
                top = value;
            }
        }

        /// <summary>
        /// Breite des Controls.
        /// </summary>
        private int width;
        [Category("Hitbase")]
        public virtual int Width
        {
            get
            {
                return width;
            }
            set
            {
                Control.Width = value;
                width = value;
            }
        }

        /// <summary>
        /// Höhe des Controls.
        /// </summary>
        private int height;
        [Category("Hitbase")]
        public virtual int Height
        {
            get
            {
                return height;
            }
            set
            {
                Control.Height = value;
                height = value;
            }
        }

        /// <summary>
        /// Liefert das Rectangle des Controls zurück (relativ zum Parent).
        /// </summary>
        public Rect ClientRectangle
        {
            get
            {
                return new Rect(Left, Top, Width, Height);
            }
        }

        private System.Windows.Forms.DockStyle dock;
        [Category("Hitbase")]
        public virtual System.Windows.Forms.DockStyle Dock
        {
            get
            {
                return dock;
            }
            set
            {
                //TODO_WPF!!!!!!!!!!!!!!Control.Dock = value;
                dock = value;
            }
        }

        private System.Windows.Forms.AnchorStyles anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
        [Category("Hitbase")]
        public virtual System.Windows.Forms.AnchorStyles Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                //TODO_WPF!!!!!!!!!!!!!!!!Control.Anchor = value;
                anchor = value;
            }
        }

        /// <summary>
        /// Text
        /// </summary>
/*TODO_WPF!!!!!!!!!!!!!        private String text;
        [Category("Hitbase")]
        public virtual String Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                Control.Text = value;
            }
        }*/

        /// <summary>
        /// Hintergrundfarbe
        /// </summary>
        private Color backColor;
        [Category("Hitbase")]
        public Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                Control ctl = Control as Control;
                if (ctl != null)
                {
                    Brush brush = new SolidColorBrush(value);
                    if (brush != null)
                        ctl.Background = brush;
                }
                backColor = value;
            }
        }

        /// <summary>
        /// Schriftfarbe
        /// </summary>
        private Color textColor;
        [Category("Hitbase")]
        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
                //TODO_WPF!!!!!!!!!!!!!!!!Control.SetValue(TextElement.ForegroundProperty, new SolidColorBrush(Tools.HelpLineColor.GetColorFromColorName(value)));
            }
        }

        /// <summary>
        /// Die Font des Controls.
        /// </summary>
        private System.Drawing.Font font;
        [Category("Hitbase")]
        public virtual System.Drawing.Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
                //TODO_WPF!!!!!!!!!!!!!!!!Control.Font = font;
            }
        }

        private Field field;
        [Category("Hitbase")]
        public virtual Field Field
        {
            get { return field; }
            set 
            { 
                field = value; 
            }
        }

        /// <summary>
        /// Tab-Index
        /// </summary>
        private int tabIndex;
        [Category("Hitbase")]
        public int TabIndex
        {
            get
            {
                return tabIndex;
            }
            set
            {
                tabIndex = value;
//TODO_WPF!!!!!!!!!!!!!!!!!                Control.TabIndex = value;
            }
        }

        /// <summary>
        /// Liefert die Position der Baseline zurück (für Ausrichten)
        /// </summary>
        public virtual int Baseline
        {
            get
            {
                return 0;
            }
        }

        private bool enabled = true;
        /// <summary>
        /// Gibt an, ob das Control enabled ist
        /// </summary>
        [Browsable(false)]
        public virtual bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                SetControlDisabled(!value);      // Control disabled/enablen
            }
        }

        /// <summary>
        /// Das Parent-Control.
        /// </summary>
        public HitbaseControl Parent
        {
            get
            {
                if (Control == null || Control.Parent == null || !(Control.Parent is IHitbaseControl))
                    return null;

                return ((IHitbaseControl)Control.Parent).HitbaseControl;
            }
        }

        /// <summary>
        /// Eine Collection aller enthaltenen Controls.
        /// </summary>
        public virtual List<FrameworkElement> Controls
        {
            get
            {
                //TODO_WPF!!!!!!!!!!!!!!!!!return Control.Controls;
                return new List<FrameworkElement>();
            }
        }


        public FrameworkElement GetControl()
        {
            return Control;
        }

        /// <summary>
        /// Fügt ein neues Child-Control dem Control hinzu.
        /// </summary>
        /// <param name="newControl"></param>
        public virtual void Add(HitbaseControl newControl)
        {
            throw new NotImplementedException();
        }

        public virtual void SetChildIndex(HitbaseControl control, int controlIndex)
        {
            control.Control.SetValue(Panel.ZIndexProperty, controlIndex);
        }

        public virtual int GetChildIndex(HitbaseControl control)
        {
            return (int)control.Control.GetValue(Panel.ZIndexProperty);
        }

        public virtual int GetTabIndex()
        {
            return 0;
            //TODO_WPF!!!!!!!!!!!!!!!return Control.TabIndex;
        }

        public virtual void SetTabIndex(int tabIndex)
        {
            //TODO_WPF!!!!!!!!!!!!!!!!!Control.TabIndex = tabIndex;
        }

        public virtual void AddToControl(FrameworkElement ctl)
        {
            ((Panel)ctl).Children.Add(this.Control);
        }

        public virtual void SetChildIndexToControl(Control ctl, int controlIndex)
        {
            //TODO_WPF!!!!!!!!!!!!!!!!!!!!ctl.Controls.SetChildIndex(this.Control, controlIndex);
        }

        public void RemoveChild(HitbaseControl ctl)
        {
            //TODO_WPF!!!!!!!!!!!!!!!!!!!!Control.Controls.Remove(ctl.Control);
        }

        public void RemoveFromControl(Control ctl)
        {
            //TODO_WPF!!!!!!!!!!!!!!!!!!!!ctl.Controls.Remove(Control);
        }

        /// <summary>
        /// Aktualisiert die Farben (z.b. wenn sich der Skin geändert hat)
        /// </summary>
        public void UpdateColors()
        {
            BackColor = BackColor;
            TextColor = TextColor;
        }


        public virtual void SetActive()
        {
            if (!hitbaseMainWindowControl.IsInDesignMode)
                return;

            /*TODO_WPF!!!!!!!!!!!!!if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (hitbaseMainWindowControl.PropertyGrid != null)
                {
                    List<object> sels = new List<object>();
                    foreach (object obj in hitbaseMainWindowControl.PropertyGrid.SelectedObjects)
                        sels.Add(obj);
                    sels.Add(this);
                    if (hitbaseMainWindowControl.PropertyGrid != null)
                        hitbaseMainWindowControl.PropertyGrid.SelectedObjects = sels.ToArray();
                }
            }
            else
            {
                hitbaseMainWindowControl.ClearSelection();
                if (hitbaseMainWindowControl.PropertyGrid != null)
                    hitbaseMainWindowControl.PropertyGrid.SelectedObject = this;
            }*/

            Selected = true;
        }

        /// <summary>
        /// Schiebt das Control eins nach vorne.
        /// </summary>
        public virtual void BringForward()
        {
            /*TODO_WPF!!!!!!!!!!!!!!!Control parent = Control.Parent;

            if (parent == null)
                return;

            int currentChildIndex = parent.Controls.GetChildIndex(Control);

            if (currentChildIndex > 0)
                parent.Controls.SetChildIndex(Control, currentChildIndex - 1);

            if (Control.TabIndex > 0)
                Control.TabIndex--;
            bandControl.BringToFront();*/
        }

        /// <summary>
        /// Schiebt das Control eins nach hinten.
        /// </summary>
        public virtual void SendBackward()
        {
            /*TODO_WPF!!!!!!!!!!!!!!!!Control parent = Control.Parent;

            if (parent == null)
                return;

            int currentChildIndex = parent.Controls.GetChildIndex(Control);

            if (currentChildIndex < parent.Controls.Count - 1)
                parent.Controls.SetChildIndex(Control, currentChildIndex + 1);

            Control.TabIndex++;*/
        }

        /// <summary>
        /// Schiebt das Control ganz nach vorne.
        /// </summary>
        public void BringToFront()
        {
            /*TODO_WPF!!!!!!!!!!!!!!!!Control.BringToFront();
            bandControl.BringToFront();*/
        }

        /// <summary>
        /// Schiebt das Control ganz nach hinten.
        /// </summary>
        public void SendToBack()
        {
            /*TODO_WPF!!!!!!!!!!!!!!!!Control.SendToBack();*/
        }

        /// <summary>
        /// Hilfsklasse zum Sortieren der Controls nach Y- und X-Koordinate
        /// </summary>
        public class HitbaseControlItem : IComparable
        {
            public HitbaseControl hitbaseControl;

            public HitbaseControlItem(HitbaseControl ctl)
            {
                hitbaseControl = ctl;
            }

            public int CompareTo(object obj)
            {
                const int gridSize = 8;

                if (hitbaseControl.Top / gridSize > ((HitbaseControlItem)obj).hitbaseControl.Top / gridSize)
                    return 1;

                if (hitbaseControl.Top / gridSize < ((HitbaseControlItem)obj).hitbaseControl.Top / gridSize)
                    return -1;

                if (hitbaseControl.Left / gridSize > ((HitbaseControlItem)obj).hitbaseControl.Left / gridSize)
                    return 1;

                if (hitbaseControl.Left / gridSize < ((HitbaseControlItem)obj).hitbaseControl.Left / gridSize)
                    return -1;

                return 0;
            }
        }

        protected void HitbaseControl_MouseDown(object sender, MouseEventArgs e)
        {
            /*TODO_WPF!!!!!!!!!!!!!!!!!!!!!if (IsInDesignMode && e.Button == MouseButtons.Left)
            {
                if (hitbaseMainWindowControl.SelectedControlTool == ControlToolEnum.Pointer)
                {
                    SetActive();
                    StartMoving((Control)sender, e.Location);
                }
                else
                {
                    hitbaseMainWindowControl.AddHitbaseControlFromMouseClick(this, e.Location);
                }
            }

            if (IsInDesignMode && e.Button == MouseButtons.Right)       // Bevor das Kontextmenü aufgeht, das Control noch selektieren
            {
                SetActive();
            }*/
        }

        public void StartMoving(Control ctl, Point pt)
        {
            /*TODO_WPF!!!!!!!!!!!!!!!!!!pt = ctl.PointToScreen(pt);
            pt = bandControl.PointToClient(pt);
            bandControl.StartMoving(pt);*/
        }

        void Control_Resize(object sender, EventArgs e)
        {
            //TODO_WPF!!!!!!!!!!!!!if (Selected && bandControl != null && bandControl.Visible == true)
            //TODO_WPF!!!!!!!!!!!!!UpdateSelection();
        }


        #region PropertyGridStuff
        private string[] NamesToRemove={ 
            "Controls",
            "Selected"
            };

        //Does the property filtering... 
        private PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection pdc)
        {
            ArrayList toRemove = new ArrayList();
            foreach (string s in NamesToRemove)
                toRemove.Add(s);

            PropertyDescriptorCollection adjustedProps = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
            foreach (PropertyDescriptor pd in pdc)
                if (!toRemove.Contains(pd.Name))
                    adjustedProps.Add(pd);

            return adjustedProps;
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, attributes, true);
            return FilterProperties(pdc);
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, true);
            return FilterProperties(pdc);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }
        #endregion

        public void UpdateSelection()
        {
            if (Selected)
                Selected = true;
        }

        public void ShowSelection(bool show)
        {
            //TODO_WPF!!!!!!!!!!!!!bandControl.Visible = show;
        }

        /// <summary>
        /// Prüft, ob das angegebene Control direktes oder indirektes Child des aktuellen Controls ist.
        /// </summary>
        /// <param name="ctl"></param>
        /// <returns></returns>
        public bool IsChild(HitbaseControl ctlChild)
        {
            if (this == ctlChild)
                return true;

            foreach (HitbaseControl childControl in Children)
            {
                if (childControl.IsChild(ctlChild))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Liefert true zurück, wenn das Control ein Container-Control (GroupBox, etc.) ist.
        /// </summary>
        /// <param name="ctl"></param>
        /// <returns></returns>
        public bool IsContainerControl()
        {
/*            if (this is HitbaseGroupBox || this is HitbaseCompound || this is HitbaseTabPage)
                return true;
            else*/
                return false;
        }

        /// <summary>
        /// Liefert das Container-Control (GroupBox, etc.) zurück, in dem das angegebene Control enthalten ist.
        /// </summary>
        /// <param name="ctl"></param>
        /// <returns></returns>
        public HitbaseControl GetContainerControl()
        {
            HitbaseControl ctlParent = Parent;
            while (ctlParent != null && !ctlParent.IsContainerControl())
            {
                ctlParent = ctlParent.Parent;
            }

            return ctlParent;
        }

        public Point PointToScreen(Point pt)
        {
            return Control.PointToScreen(pt);
        }

        public Point PointToClient(Point pt)
        {
            return Control.PointFromScreen(pt);
        }

        public void ShowContextMenu(Point ptScreen)
        {
            //TODO_WPF!!!!!!!!!!!!!!!!!!!!Control.ContextMenuStrip.Show(ptScreen);
        }

        public HitbaseControl GetNextControl(bool forward)
        {
           /*TODO_WPF!!!!!!!!!!!!!!!!!! Control ctlNext = Control;

            while (true)
            {
                ctlNext = hitbaseMainWindowControl.GetNextControl(ctlNext, forward);
                if (ctlNext is IHitbaseControl || ctlNext == null)
                    break;
            }

            if (ctlNext != null)
                return ((IHitbaseControl)ctlNext).HitbaseControl;
            else*/
                return null;
        }

        public Object GetDataFromCD()
        {
            try
            {
                return hitbaseMainWindowControl.theCd.GetValueByField(Field);
            }
            catch
            {
                return null;
            }
        }

        public void SaveDataToCD(Object value)
        {
            try
            {
                hitbaseMainWindowControl.theCd.SetValueToField(Field, value);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Liefert zurück, in welche Richtungen das Control vergrößert/verkleiner werden darf.
        /// </summary>
        /// <returns></returns>
        /*TODO_WPF!!!!!!!!!!!!public virtual GripperDirection AllowedResizeDirection()
        {
            switch (Dock)
            {
                case System.Windows.Forms.DockStyle.Bottom:
                    return GripperDirection.N;
                case System.Windows.Forms.DockStyle.Fill:
                    return GripperDirection.Unknown;
                case System.Windows.Forms.DockStyle.Left:
                    return GripperDirection.E;
                case System.Windows.Forms.DockStyle.None:
                    return GripperDirection.All | GripperDirection.Move;
                case System.Windows.Forms.DockStyle.Right:
                    return GripperDirection.W;
                case System.Windows.Forms.DockStyle.Top:
                    return GripperDirection.S;
                default:
                    return GripperDirection.All | GripperDirection.Move;
            }
        }*/
    }
}
