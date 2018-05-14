/*
 
 2008 Jos� Manuel Men�ndez Poo
 * 
 * Please give me credit if you use this code. It's all I ask.
 * 
 * Contact me for more info: menendezpoo@gmail.com
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a tab that can contain RibbonPanel objects
    /// </summary>
    [DesignTimeVisible(false)]
    [Designer(typeof(RibbonTabDesigner))]
    public class RibbonTab : Component, IRibbonElement, IContainsRibbonComponents
    {
        #region Fields
        private RibbonPanelCollection _panels;
        private Rectangle _tabBounds;
        private Rectangle _tabContentBounds;
        private Ribbon _owner;
        private bool _pressed;
        private bool _selected;
        private bool _active;
        private Rectangle _bounds;
        private object _tag;
        private string _text;
        private RibbonContext _context;
        private bool _scrollLeftVisible;
        private Rectangle _scrollLeftBounds;
        private bool _scrollLeftSelected;
        private bool _scrollLeftPressed;
        private Rectangle _scrollRightBounds;
        private bool _scrollRightSelected;
        private bool _scrollRightVisible;
        private bool _scrollRightPressed;
        private int _offset;


        /// <summary>
        /// Occurs when the mouse pointer enters the panel
        /// </summary>
        public event MouseEventHandler MouseEnter;

        /// <summary>
        /// Occurs when the mouse pointer leaves the panel
        /// </summary>
        public event MouseEventHandler MouseLeave;

        /// <summary>
        /// Occurs when the mouse pointer is moved inside the panel
        /// </summary>
        public event MouseEventHandler MouseMove; 

        #endregion

        #region Ctor

        public RibbonTab()
        {
            _panels = new RibbonPanelCollection(this);
        }

        /// <summary>
        /// Creates a new RibbonTab
        /// </summary>
        public RibbonTab(Ribbon owner, string text)
        {
            _panels = new RibbonPanelCollection(owner, this);
            _text = text;
        } 
        #endregion

        #region Events

        
        public event EventHandler ScrollRightVisibleChanged;
        public event EventHandler ScrollRightPressedChanged;
        public event EventHandler ScrollRightBoundsChanged;
        public event EventHandler ScrollLeftVisibleChanged;
        public event EventHandler ScrollLeftPressedChanged;
        public event EventHandler BoundsChanged;
        public event EventHandler TabBoundsChanged;
        public event EventHandler TabContentBoundsChanged;
        public event EventHandler OwnerChanged;
        public event EventHandler PressedChanged;
        public event EventHandler ActiveChanged;
        public event EventHandler TextChanged;
        public event EventHandler ContextChanged;

        #endregion

        #region Props

        /// <summary>
        /// Gets if the right-side scroll button is currently visible
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ScrollRightVisible
        {
            get { return _scrollRightVisible; }
        }

        /// <summary>
        /// Gets if the right-side scroll button is currently selected
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ScrollRightSelected
        {
            get { return _scrollRightSelected; }
        }

        /// <summary>
        /// Gets if the right-side scroll button is currently pressed
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ScrollRightPressed
        {
            get { return _scrollRightPressed; }
        }

        /// <summary>
        /// Gets if the right-side scroll button bounds
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Rectangle ScrollRightBounds
        {
            get { return _scrollRightBounds; }
        }

        /// <summary>
        /// Gets if the left scroll button is currently visible
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ScrollLeftVisible
        {
            get { return _scrollLeftVisible; }
        }

        /// <summary>
        /// Gets if the left scroll button bounds
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Rectangle ScrollLeftBounds
        {
            get { return _scrollLeftBounds; }
        }

        /// <summary>
        /// Gets if the left scroll button is currently selected
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ScrollLeftSelected
        {
            get { return _scrollLeftSelected; }
        }

        /// <summary>
        /// Gets if the left scroll button is currently pressed
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ScrollLeftPressed
        {
            get { return _scrollLeftPressed; }
        }

        /// <summary>
        /// Gets the <see cref="TabBounds"/> property value
        /// </summary>
        public Rectangle Bounds
        {
            get { return TabBounds; }
        }

        /// <summary>
        /// Gets the collection of panels that belong to this tab
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonPanelCollection Panels
        {
            get
            {
                return _panels;
            }
        }

        /// <summary>
        /// Gets the bounds of the little tab showing the text
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Rectangle TabBounds
        {
            get
            {
                return _tabBounds;
            }
        }

        /// <summary>
        /// Gets the bounds of the tab content on the Ribbon
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Rectangle TabContentBounds
        {
            get
            {
                return _tabContentBounds;
            }
        }

        /// <summary>
        /// Gets the Ribbon that contains this tab
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Ribbon Owner
        {
            get
            {
                return _owner;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the state of the tab is being pressed by the mouse or a key
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Pressed
        {
            get
            {
                return _pressed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tab is selected
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Selected
        {
            get
            {
                return _selected;
            }
        }

        /// <summary>
        /// Gets a value indicating if the tab is currently the active tab
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Gets or sets the object that contains data about the control
        /// </summary>
        public object Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        /// <summary>
        /// Gets or sets the text that is to be displayed on the tab
        /// </summary>
        [Localizable(true)]
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                if (Owner != null) Owner.OnRegionsChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tab is attached to a  Context
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Contextual
        {
            get
            {
                return _context != null;
            }
        }

        /// <summary>
        /// Gets or sets the context this tab belongs to
        /// </summary>
        /// <remarks>Tabs on a context are highlighted with a special glow color</remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonContext Context
        {
            get
            {
                return _context;
            }
        }

        #endregion

        #region IRibbonElement Members

        public void OnPaint(object sender, RibbonElementPaintEventArgs e)
        {
            if (Owner == null) return;

            Owner.Renderer.OnRenderRibbonTab(new RibbonTabRenderEventArgs(Owner, e.Graphics, e.Clip, this));
            Owner.Renderer.OnRenderRibbonTabText(new RibbonTabRenderEventArgs(Owner, e.Graphics, e.Clip, this));

            if(Active)
                foreach (RibbonPanel panel in Panels)
                {
                    panel.OnPaint(this, new RibbonElementPaintEventArgs(e.Clip, e.Graphics, panel.SizeMode));
                }

            Owner.Renderer.OnRenderTabScrollButtons(new RibbonTabRenderEventArgs(Owner, e.Graphics, e.Clip, this));
        }

        /// <summary>
        /// Overriden. Raises the Bound
        /// </summary>
        public void SetBounds(System.Drawing.Rectangle bounds)
        {
            throw new Exception("Not used");
        }

        /// <summary>
        /// Measures the size of the tab. The tab content bounds is measured by the Ribbon control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            Size textSize = TextRenderer.MeasureText(Text, Owner.Font);

            return textSize;
        }

        /// <summary>
        /// Sets the value of the Owner Property
        /// </summary>
        internal void SetOwner(Ribbon owner)
        {
            _owner = owner;

            Panels.SetOwner(owner);
        }

        /// <summary>
        /// Sets the value of the Pressed property
        /// </summary>
        /// <param name="pressed">Value that indicates if the element is pressed</param>
        internal void SetPressed(bool pressed)
        {
            _pressed = pressed;
        }

        /// <summary>
        /// Sets the value of the Selected property
        /// </summary>
    /// <param name="selected">Value that indicates if the element is selected</param>
        internal void SetSelected(bool selected)
        {
            _selected = selected;

            if (selected)
            {
                OnMouseEnter(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
            }
            else
            {
                OnMouseLeave(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Sets the tab as active without sending the message to the Ribbon
        /// </summary>
        internal void SetActive(bool active)
        {
            _active = active;
        }

        /// <summary>
        /// Sets the value of the TabBounds property
        /// </summary>
        /// <param name="tabBounds">Rectangle representing the bounds of the tab</param>
        internal void SetTabBounds(Rectangle tabBounds)
        {
            _tabBounds = tabBounds;
        }

        /// <summary>
        /// Sets the value of the TabContentBounds
        /// </summary>
        /// <param name="tabContentBounds">Rectangle representing the bounds of the tab's content</param>
        internal void SetTabContentBounds(Rectangle tabContentBounds)
        {
            _tabContentBounds = tabContentBounds;
        }

        /// <summary>
        /// Gets the panel with the larger with and the specified size mode
        /// </summary>
        /// <param name="size">Size mode of panel to search</param>
        /// <returns>Larger panel. Null if none of the specified size mode</returns>
        private RibbonPanel GetLargerPanel(RibbonElementSizeMode size)
        {
            RibbonPanel result = null;

            foreach (RibbonPanel panel in Panels)
            {
                if (panel.SizeMode != size) continue;

                if (result == null) result = panel;

                if (panel.Bounds.Width > result.Bounds.Width)
                {
                    result = panel;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the panel with a larger size
        /// </summary>
        /// <returns></returns>
        private RibbonPanel GetLargerPanel()
        {
            RibbonPanel largeLarger = GetLargerPanel(RibbonElementSizeMode.Large);

            if (largeLarger != null) return largeLarger;

            RibbonPanel mediumLarger = GetLargerPanel(RibbonElementSizeMode.Medium);

            if (mediumLarger != null) return mediumLarger;

            RibbonPanel compactLarger = GetLargerPanel(RibbonElementSizeMode.Compact);

            if (compactLarger != null) return compactLarger;

            RibbonPanel overflowLarger = GetLargerPanel(RibbonElementSizeMode.Overflow);

            if (overflowLarger != null) return overflowLarger;

            return null;
        }

        private bool AllPanelsOverflow()
        {

            foreach (RibbonPanel  panel in Panels)
            {
                if (panel.SizeMode != RibbonElementSizeMode.Overflow)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Updates the regions of the panels and its contents
        /// </summary>
        internal void UpdatePanelsRegions()
        {
            if (Panels.Count == 0) return;

            bool dMode = Site != null && Site.DesignMode;

            if(!dMode)
                _offset = 0;

            int curRight = TabContentBounds.Left + Owner.PanelPadding.Left + _offset;
            int panelsTop = TabContentBounds.Top + Owner.PanelPadding.Top;

            using (Graphics g = Owner.CreateGraphics())
            {
                //Check all at full size
                foreach (RibbonPanel panel in Panels)
                {
                    RibbonElementSizeMode sMode = panel.FlowsTo == RibbonPanelFlowDirection.Right ? RibbonElementSizeMode.Medium : RibbonElementSizeMode.Large;
                    //Set the bounds of the panel to let it know it's height
                    panel.SetBounds(new Rectangle(0, 0, 1, TabContentBounds.Height - Owner.PanelPadding.Vertical));

                    ///Size of the panel
                    Size size = panel.MeasureSize(this, new RibbonElementMeasureSizeEventArgs(g, sMode));

                    ///Creates the bounds of the panel
                    Rectangle bounds = new Rectangle(
                        curRight, panelsTop,
                        size.Width, size.Height);

                    ///Set the bouns of the panel
                    panel.SetBounds(bounds);

                    ///Let the panel know what size we have decided for it
                    panel.SetSizeMode(sMode);

                    ///Update curLeft
                    curRight = bounds.Right + 1 + Owner.PanelSpacing;
                }


                if (!dMode)
                {
                    while (curRight > TabContentBounds.Right && !AllPanelsOverflow())
                    {
                        #region Down grade the larger panel one position

                        RibbonPanel larger = GetLargerPanel();

                        if (larger.SizeMode == RibbonElementSizeMode.Large)
                            larger.SetSizeMode(RibbonElementSizeMode.Medium);
                        else if (larger.SizeMode == RibbonElementSizeMode.Medium)
                            larger.SetSizeMode(RibbonElementSizeMode.Compact);
                        else if (larger.SizeMode == RibbonElementSizeMode.Compact)
                            larger.SetSizeMode(RibbonElementSizeMode.Overflow);

                        Size size = larger.MeasureSize(this, new RibbonElementMeasureSizeEventArgs(g, larger.SizeMode));

                        larger.SetBounds(new Rectangle(larger.Bounds.Location, new Size(size.Width + Owner.PanelMargin.Horizontal, size.Height)));

                        #endregion

                        ///Reset x-axis reminder
                        curRight = TabContentBounds.Left + Owner.PanelPadding.Left;

                        ///Re-arrange location because of the new bounds
                        foreach (RibbonPanel panel in Panels)
                        {
                            Size s = panel.Bounds.Size;
                            panel.SetBounds(new Rectangle(new Point(curRight, panelsTop), s));
                            curRight += panel.Bounds.Width + 1 + Owner.PanelSpacing;
                        }

                    } 
                }

                ///Update regions of all panels
                foreach (RibbonPanel panel in Panels)
                {
                    panel.UpdateItemsRegions(g, panel.SizeMode);
                }
            }

            UpdateScrollBounds();
        }

        /// <summary>
        /// Updates the bounds of the scroll bounds
        /// </summary>
        private void UpdateScrollBounds()
        {
            int w = 13;

            if (Panels.Count == 0) return;

            if (Panels[Panels.Count - 1].Bounds.Right > TabContentBounds.Right)
            {
                _scrollRightVisible = true;
            }
            else
            {
                _scrollRightVisible = false;
            }

            if (_offset < 0)
            {
                _scrollLeftVisible = true;
            }
            else
            {
                _scrollLeftVisible = false;
            }

            if (_scrollLeftVisible || _scrollRightVisible)
            {
                _scrollRightBounds = Rectangle.FromLTRB(
                    Owner.ClientRectangle.Right - w,
                    TabContentBounds.Top,
                    Owner.ClientRectangle.Right,
                    TabContentBounds.Bottom);

                _scrollLeftBounds = Rectangle.FromLTRB(
                    0,
                    TabContentBounds.Top,
                    w,
                    TabContentBounds.Bottom);
            }
        }

        /// <summary>
        /// Overriden. Returns a string representation of the tab
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Tab: {0}", Text);
        }

        /// <summary>
        /// Raises the MouseEnter event
        /// </summary>
        /// <param name="e">Event data</param>
        public virtual void OnMouseEnter(MouseEventArgs e)
        {
            if (MouseEnter != null)
            {
                MouseEnter(this, e);
            }
        }

        /// <summary>
        /// Raises the MouseLeave event
        /// </summary>
        /// <param name="e">Event data</param>
        public virtual void OnMouseLeave(MouseEventArgs e)
        {
            if (MouseLeave != null)
            {
                MouseLeave(this, e);
            }
        }

        /// <summary>
        /// Raises the MouseMove event
        /// </summary>
        /// <param name="e">Event data</param>
        public virtual void OnMouseMove(MouseEventArgs e)
        {
            if (MouseMove != null)
            {
                MouseMove(this, e);
            }
        }

        /// <summary>
        /// Sets the value of the <see cref="ScrollLeftPressed"/>
        /// </summary>
        /// <param name="pressed"></param>
        internal void SetScrollLeftPressed(bool pressed)
        {
            _scrollLeftPressed = pressed;
        }

        /// <summary>
        /// Sets the value of the <see cref="ScrollLeftSelected"/>
        /// </summary>
        /// <param name="selected"></param>
        internal void SetScrollLeftSelected(bool selected)
        {
            _scrollLeftSelected = selected;
        }

        /// <summary>
        /// Sets the value of the <see cref="ScrollRightPressed"/>
        /// </summary>
        /// <param name="pressed"></param>
        internal void SetScrollRightPressed(bool pressed)
        {
            _scrollRightPressed = pressed;
        }

        /// <summary>
        /// Sets the value of the <see cref="ScrollRightSelected"/>
        /// </summary>
        /// <param name="selected"></param>
        internal void SetScrollRightSelected(bool selected)
        {
            _scrollRightSelected = selected;
        }

        /// <summary>
        /// Presses the lef-scroll button
        /// </summary>
        public void ScrollLeft()
        {
            ScrollOffset(50);
        }

        /// <summary>
        /// Presses the left-scroll button
        /// </summary>
        public void ScrollRight()
        {
            ScrollOffset(-50);
        }

        public void ScrollOffset(int amount)
        {
            _offset += amount;

            foreach (RibbonPanel p in Panels)
            {
                p.SetBounds(new Rectangle(p.Bounds.Left + amount,
                    p.Bounds.Top, p.Bounds.Width, p.Bounds.Height));
            }

            if((Site != null && Site.DesignMode))
            UpdatePanelsRegions();

            UpdateScrollBounds();



            Owner.Invalidate();
        }

        #endregion

        #region IContainsRibbonComponents Members

        public IEnumerable<Component> GetAllChildComponents()
        {
            return Panels.ToArray();
        }

        #endregion
    }
}
