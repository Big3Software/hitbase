/*
 
 2008 José Manuel Menéndez Poo
 * 
 * Please give me credit if you use this code. It's all I ask.
 * 
 * Contact me for more info: menendezpoo@gmail.com
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a list of buttons that can be navigated
    /// </summary>
    [Designer(typeof(RibbonButtonListDesigner))]
    public sealed class RibbonButtonList : RibbonItem, 
        IContainsSelectableRibbonItems, IScrollableRibbonItem, IContainsRibbonComponents

    {
        #region Fields

        private RibbonButtonCollection _buttons;
        private int _itemsInLargeMode;
        private int _itemsInMediumMode;
        private Size _ItemsInDropwDownMode;
        private Rectangle _buttonUpBounds;
        private Rectangle _buttonDownBounds;
        private Rectangle _buttonDropDownBounds;
        private Rectangle _contentBounds;
        private int _controlButtonsWidth;
        private RibbonElementSizeMode _buttonsSizeMode;
        private int _jumpDownSize;
        private int _jumpUpSize;
        private int _offset;
        private bool _buttonDownSelected;
        private bool _buttonDownPressed;
        private bool _buttonUpSelected;
        private bool _buttonUpPressed;
        private bool _buttonDropDownSelected;
        private bool _buttonDropDownPressed;
        private bool _buttonUpEnabled;
        private bool _buttonDownEnabled;
        private RibbonDropDown _dropDown;
        private bool _dropDownVisible;
        private RibbonItemCollection _dropDownItems;
        #endregion

        #region Ctor

        public RibbonButtonList()
        {
            _buttons = new RibbonButtonCollection(this);
            _dropDownItems = new RibbonItemCollection();

            _controlButtonsWidth = 16;
            _itemsInLargeMode = 7;
            _itemsInMediumMode = 3;
            _ItemsInDropwDownMode = new Size(7, 5);
            _buttonsSizeMode = RibbonElementSizeMode.Large;
            
            
        }

        public RibbonButtonList(IEnumerable<RibbonButton> buttons)
            : this(buttons, null)
        {
        }

        public RibbonButtonList(IEnumerable<RibbonButton> buttons, IEnumerable<RibbonItem> dropDownItems)
            : this()
        {
            if (buttons != null)
            {
                List<RibbonButton> items = new List<RibbonButton>(buttons);

                _buttons.AddRange(items.ToArray()); 
            }

            if (dropDownItems != null)
            {
                _dropDownItems.AddRange(dropDownItems);
            }
        }

        #endregion

        #region Props

        /// <summary>
        /// Gets if the DropDown button is present on thelist
        /// </summary>
        public bool ButtonDropDownPresent
        {
            get 
            {
                return ButtonDropDownBounds.Height > 0;
            }
        }

        /// <summary>
        /// Gets the collection of items shown on the dropdown pop-up when Style allows it
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonItemCollection DropDownItems
        {
            get
            {
                return _dropDownItems;
            }
        }

        /// <summary>
        /// Gets or sets the size that the buttons on the list should be
        /// </summary>
        public RibbonElementSizeMode ButtonsSizeMode
        {
            get { return _buttonsSizeMode; }
            set { _buttonsSizeMode = value; if (Owner != null) Owner.OnRegionsChanged(); }
        }

        /// <summary>
        /// Gets a value indicating if the button that scrolls up the content is currently enabled
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonUpEnabled
        {
            get { return _buttonUpEnabled; }
        }

        /// <summary>
        /// Gets a value indicating if the button that scrolls down the content is currently enabled
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonDownEnabled
        {
            get { return _buttonDownEnabled; }
        }

        /// <summary>
        /// Gets a value indicating if the DropDown button is currently selected
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonDropDownSelected
        {
            get { return _buttonDropDownSelected; }
        }

        /// <summary>
        /// Gets a value indicating if the DropDown button is currently pressed
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonDropDownPressed
        {
            get { return _buttonDropDownPressed; }
        }

        /// <summary>
        /// Gets a vaule indicating if the button that scrolls down the content is currently selected
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonDownSelected
        {
            get { return _buttonDownSelected; }
        }

        /// <summary>
        /// Gets a vaule indicating if the button that scrolls down the content is currently pressed
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonDownPressed
        {
            get { return _buttonDownPressed; }
        }

        /// <summary>
        /// Gets a vaule indicating if the button that scrolls up the content is currently selected
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonUpSelected
        {
            get { return _buttonUpSelected; }
        }

        /// <summary>
        /// Gets a vaule indicating if the button that scrolls up the content is currently pressed
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonUpPressed
        {
            get { return _buttonUpPressed; }
        }

        /// <summary>
        /// Gets the bounds of the content where items are shown
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ContentBounds
        {
            get { return _contentBounds; }
        }

        /// <summary>
        /// Gets the bounds of the button that scrolls the items up
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ButtonUpBounds
        {
            get { return _buttonUpBounds; }
        }

        /// <summary>
        /// Gets the bounds of the button that scrolls the items down
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ButtonDownBounds
        {
            get { return _buttonDownBounds; }
        }

        /// <summary>
        /// Gets the bounds of the button that scrolls
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ButtonDropDownBounds
        {
            get { return _buttonDropDownBounds; }
        }

        /// <summary>
        /// Gets or sets the with of the buttons that allow to navigate thru the list
        /// </summary>
        [DefaultValue(16)]
        public int ControlButtonsWidth
        {
            get { return _controlButtonsWidth; }
            set { _controlButtonsWidth = value; if (Owner != null) Owner.OnRegionsChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating the amount of items to show
        /// (wide) when SizeMode is Large 
        /// </summary>
        [DefaultValue(7)]
        public int ItemsWideInLargeMode
        {
            get { return _itemsInLargeMode; }
            set { _itemsInLargeMode = value; if (Owner != null) Owner.OnRegionsChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating the amount of items to show
        /// (wide) when SizeMode is Medium
        /// </summary>
        [DefaultValue(3)]
        public int ItemsWideInMediumMode
        {
            get { return _itemsInMediumMode; }
            set { _itemsInMediumMode = value; if (Owner != null) Owner.OnRegionsChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating the amount of items to show
        /// (wide) when SizeMode is Medium
        /// </summary>
        public Size ItemsSizeInDropwDownMode
        {
            get { return _ItemsInDropwDownMode; }
            set { _ItemsInDropwDownMode = value; if (Owner != null) Owner.OnRegionsChanged(); }
        }

        /// <summary>
        /// Gets the collection of buttons of the list
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonButtonCollection Buttons
        {
            get
            {
                return _buttons;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Ignores deactivation of canvas if it is a volatile window
        /// </summary>
        private void IgnoreDeactivation()
        {
            if (Canvas is RibbonPanelPopup)
            {
                (Canvas as RibbonPanelPopup).IgnoreNextClickDeactivation();
            }

            if (Canvas is RibbonDropDown)
            {
                (Canvas as RibbonDropDown).IgnoreNextClickDeactivation();
            }
        }

        /// <summary>
        /// Redraws the control buttons: up, down and dropdown
        /// </summary>
        private void RedrawControlButtons()
        {
            Canvas.Invalidate(Rectangle.FromLTRB(
                Bounds.Left - ControlButtonsWidth - 1, 
                Bounds.Top, Bounds.Right + 1, Bounds.Bottom + 1));
        }

        /// <summary>
        /// Pushes the amount of _offset of the top of items
        /// </summary>
        /// <param name="amount"></param>
        private void ScrollOffset(int amount)
        {
            _offset += amount;
            SetBounds(Bounds);
            RedrawItem();
        }

        /// <summary>
        /// Scrolls the list down
        /// </summary>
        public void ScrollDown()
        {
            ScrollOffset(-(_jumpDownSize + 1));
        }

        /// <summary>
        /// Scrolls the list up
        /// </summary>
        public void ScrollUp()
        {
            ScrollOffset(_jumpDownSize + 1);
        }

        /// <summary>
        /// Shows the drop down items of the button, as if the dropdown part has been clicked
        /// </summary>
        public void ShowDropDown()
        {
            if (DropDownItems.Count == 0)
            {
                SetPressed(false);
                return;
            }

            IgnoreDeactivation();

            _dropDown = new RibbonDropDown(this, DropDownItems, Owner);
            //_dropDown.FormClosed += new FormClosedEventHandler(dropDown_FormClosed);
            //_dropDown.StartPosition = FormStartPosition.Manual;
            _dropDown.ShowSizingGrip = true;
            Point location = Canvas.PointToScreen(new Point(Bounds.Left, Bounds.Top));
            

            SetDropDownVisible(true);
            _dropDown.Show(location);

        }

        private void dropDown_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetDropDownVisible(false);
        }


        /// <summary>
        /// Closes the DropDown if opened
        /// </summary>
        public void CloseDropDown()
        {
            if (_dropDown != null)
            {
                //RibbonDropDown.DismissTo(_dropDown);
            }

            SetDropDownVisible(false);
        }

        /// <summary>
        /// Sets the value of DropDownVisible
        /// </summary>
        /// <param name="visible"></param>
        internal void SetDropDownVisible(bool visible)
        {
            _dropDownVisible = visible;
        }

        #endregion

        #region Overrides

        internal override void SetOwner(Ribbon owner)
        {
            base.SetOwner(owner);

            _buttons.SetOwner(owner);
            _dropDownItems.SetOwner(owner);
        }

        internal override void SetOwnerPanel(RibbonPanel ownerPanel)
        {
            base.SetOwnerPanel(ownerPanel);

            _buttons.SetOwnerPanel(ownerPanel);
            _dropDownItems.SetOwnerPanel(ownerPanel);
        }

        internal override void SetOwnerTab(RibbonTab ownerTab)
        {
            base.SetOwnerTab(ownerTab);

            _buttons.SetOwnerTab(ownerTab);
            _dropDownItems.SetOwnerTab(OwnerTab);
        }

        public override void OnPaint(object sender, RibbonElementPaintEventArgs e)
        {
            Owner.Renderer.OnRenderRibbonItem(new RibbonItemRenderEventArgs(Owner, e.Graphics, e.Clip, this));
            
            if (e.Mode != RibbonElementSizeMode.Compact)
            {
                Region lastClip = e.Graphics.Clip;
                Region newClip = new Region(lastClip.GetBounds(e.Graphics));
                newClip.Intersect(ContentBounds);
                e.Graphics.SetClip(newClip.GetBounds(e.Graphics));

                foreach (RibbonButton button in Buttons)
                {
                    if(!button.Bounds.IsEmpty)
                    button.OnPaint(this, new RibbonElementPaintEventArgs(button.Bounds, e.Graphics,
                        ButtonsSizeMode));
                }
                e.Graphics.SetClip(lastClip.GetBounds(e.Graphics));
            }
        }

        public override void SetBounds(System.Drawing.Rectangle bounds)
        {
            base.SetBounds(bounds);

            #region Assign control buttons bounds

            int cbtns = Canvas is RibbonDropDown ? 2 : 3;
            int buttonHeight = bounds.Height / cbtns;
            int buttonWidth = _controlButtonsWidth;


            _buttonUpBounds = Rectangle.FromLTRB(bounds.Right - buttonWidth,
                bounds.Top, bounds.Right, bounds.Top + buttonHeight);

            _buttonDownBounds = Rectangle.FromLTRB(_buttonUpBounds.Left, _buttonUpBounds.Bottom,
                bounds.Right, _buttonUpBounds.Bottom + buttonHeight);
                        

            if (cbtns == 2)
            {
                _buttonDropDownBounds = Rectangle.Empty;
            }
            else
            {
                _buttonDropDownBounds = Rectangle.FromLTRB(_buttonDownBounds.Left, _buttonDownBounds.Bottom,
                bounds.Right, bounds.Bottom + 1);
            }

            _contentBounds = Rectangle.FromLTRB(bounds.Left, bounds.Top, _buttonUpBounds.Left, bounds.Bottom);

            #endregion

            #region Assign buttons regions

            _buttonUpEnabled = _offset < 0;
            if (!_buttonUpEnabled) _offset = 0;
            _buttonDownEnabled = false;

            int curLeft = ContentBounds.Left + 1;
            int curTop = ContentBounds.Top + 1 + _offset;
            int maxBottom = int.MinValue;

            foreach (RibbonItem item in Buttons)
            {
                item.SetBounds(Rectangle.Empty);
            }

            for (int i = 0; i < Buttons.Count; i++)
            {
                RibbonButton button = Buttons[i] as RibbonButton; if (button == null) break;

                if (curLeft + button.LastMeasuredSize.Width > ContentBounds.Right)
                {
                    curLeft = ContentBounds.Left + 1;
                    curTop = maxBottom + 1;

                    if (curTop > ContentBounds.Bottom)
                    {
                        break;
                    }
                }

                button.SetBounds(new Rectangle(curLeft, curTop, button.LastMeasuredSize.Width, button.LastMeasuredSize.Height));
                
                curLeft = button.Bounds.Right + 1;
                maxBottom = Math.Max(maxBottom, button.Bounds.Bottom);

                if (button.Bounds.Bottom > ContentBounds.Bottom) _buttonDownEnabled = true;

                _jumpDownSize = button.Bounds.Height;
                _jumpUpSize = button.Bounds.Height;
            }

            

            #endregion
        }

        public override Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            #region Determine items

            int itemsWide = 0;

            switch (e.SizeMode)
            {
                case RibbonElementSizeMode.DropDown:
                    itemsWide = ItemsSizeInDropwDownMode.Width;
                    break;
                case RibbonElementSizeMode.Large:
                    itemsWide = ItemsWideInLargeMode;
                    break;
                case RibbonElementSizeMode.Medium:
                    itemsWide = ItemsWideInMediumMode;
                    break;
                case RibbonElementSizeMode.Compact:
                    itemsWide = 0;
                    break;
            }

            #endregion

            int height = OwnerPanel.ContentBounds.Height - Owner.ItemPadding.Vertical - 4;
            int scannedItems = 0;
            int widthSum = 1;
            int buttonHeight = 0;
            bool sumWidth = true;

            foreach (RibbonButton button in Buttons)
            {
                Size s = button.MeasureSize(this, 
                    new RibbonElementMeasureSizeEventArgs(e.Graphics, this.ButtonsSizeMode));

                if (sumWidth)
                    widthSum += s.Width + 1;

                buttonHeight = button.LastMeasuredSize.Height;

                if (++scannedItems == itemsWide) sumWidth = false;
            }

            if (e.SizeMode == RibbonElementSizeMode.DropDown)
            {
                height = buttonHeight * ItemsSizeInDropwDownMode.Height;
            }

            SetLastMeasuredSize(new Size(widthSum + ControlButtonsWidth, height));

            return LastMeasuredSize;
        }

        internal override void SetSizeMode(RibbonElementSizeMode sizeMode)
        {
            base.SetSizeMode(sizeMode);

            foreach (RibbonItem item in Buttons)
            {
                item.SetSizeMode(ButtonsSizeMode);
            }
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (ButtonDownPressed && ButtonDownSelected && ButtonDownEnabled)
            {
                ScrollOffset(-1);
            }

            if (ButtonUpPressed && ButtonUpSelected && ButtonUpEnabled)
            {
                ScrollOffset(1);
            }

            bool upCache = _buttonUpSelected;
            bool downCache = _buttonDownSelected;
            bool dropCache = _buttonDropDownSelected;

            _buttonUpSelected = _buttonUpBounds.Contains(e.X, e.Y);
            _buttonDownSelected = _buttonDownBounds.Contains(e.X, e.Y);
            _buttonDropDownSelected = _buttonDropDownBounds.Contains(e.X, e.Y);

            if (  (upCache != _buttonUpSelected)
                || (downCache != _buttonDownSelected)
                || (dropCache != _buttonDropDownSelected))
                RedrawControlButtons();
        }

        public override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            bool mustRedraw = _buttonUpSelected || _buttonDownSelected || _buttonDropDownSelected;

            _buttonUpSelected = false;
            _buttonDownSelected = false;
            _buttonDropDownSelected = false;

            if (mustRedraw)
                RedrawControlButtons();
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (ButtonDownSelected || ButtonUpSelected || ButtonDropDownSelected)
            {
                IgnoreDeactivation();
            }

            if (ButtonDownSelected && ButtonDownEnabled)
            {
                _buttonDownPressed = true;
                ScrollDown();
            }

            if (ButtonUpSelected && ButtonUpEnabled)
            {
                _buttonUpPressed = true;
                ScrollUp();
            }

            if (ButtonDropDownSelected)
            {
                _buttonDropDownPressed = true;
                ShowDropDown();
            }
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _buttonDownPressed = false;
            _buttonUpPressed = false;
            _buttonDropDownPressed = false;
        }

        #endregion

        #region IContainsRibbonItems Members

        public IEnumerable<RibbonItem> GetItems()
        {
            return Buttons;
        }

        public Rectangle GetContentBounds()
        {
            return ContentBounds;
        }

        #endregion

        #region IContainsRibbonComponents Members

        public IEnumerable<Component> GetAllChildComponents()
        {
            List<Component> result = new List<Component>(Buttons.ToArray());

            result.AddRange(DropDownItems.ToArray());

            return result;
        }

        #endregion
    }
}
