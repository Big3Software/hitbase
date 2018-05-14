using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a quick access toolbar hosted on the Ribbon
    /// </summary>
    public class RibbonQuickAccessToolbar : RibbonItem,
        IContainsSelectableRibbonItems, IContainsRibbonComponents
    {
        #region Fields
        private RibbonQuickAccessToolbarItemCollection _items;
        private bool _menuButtonVisible;
        private Padding _margin;
        private Padding _padding;
        private RibbonSensor _sensor;
        private RibbonButton _dropDownButton;
        private bool _DropDownButtonVisible;
        #endregion

        #region Ctor
        internal RibbonQuickAccessToolbar(Ribbon ownerRibbon)
        {
            if (ownerRibbon == null) throw new ArgumentNullException("ownerRibbon");

            SetOwner(ownerRibbon);

            _dropDownButton = new RibbonButton();
            _dropDownButton.SetOwner(ownerRibbon);
            _dropDownButton.SmallImage = CreateDropDownButtonImage();

            _margin = new Padding(9);
            _padding = new Padding(3, 0, 0, 0);
            _items = new RibbonQuickAccessToolbarItemCollection(this);
            _sensor = new RibbonSensor(ownerRibbon, ownerRibbon, Items);
            _DropDownButtonVisible = true;
            
        }

        private Image CreateDropDownButtonImage()
        {
            Bitmap bmp = new Bitmap(7, 6);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawLine(Pens.Navy, 0, 0, 4, 0);
                g.FillPolygon(Brushes.Navy, new Point[] { 
                    new Point(0,3),
                    new Point(5, 3),
                    new Point(2, 6)
                });
                
            }

            return bmp;
        }
        #endregion

        #region Properties


        [Description("Shows or hides the dropdown button of the toolbar")]
        [DefaultValue(true)]
        public bool DropDownButtonVisible
        {
            get { return _DropDownButtonVisible; }
            set { _DropDownButtonVisible = value; Owner.OnRegionsChanged(); }
        }


        /// <summary>
        /// Gets the bounds of the toolbar including the graphic adornments
        /// </summary>
        [Browsable(false)]
        internal Rectangle SuperBounds
        {
            get { return Rectangle.FromLTRB(Bounds.Left - Padding.Horizontal, Bounds.Top, DropDownButton.Bounds.Right, Bounds.Bottom); }
        }

        /// <summary>
        /// Gets the dropdown button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden)]
        public RibbonButton DropDownButton
        {
            get { return _dropDownButton; }
        }

        /// <summary>
        /// Gets or sets the padding (internal) of the toolbar
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Padding Padding
        {
            get { return _padding; }
        }

        /// <summary>
        /// Gets or sets the margin (external) of the toolbar
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Padding Margin
        {
            get { return _margin; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the button that shows the menu of the 
        /// QuickAccess toolbar should be visible
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MenuButtonVisible
        {
            get { return _menuButtonVisible; }
            set { _menuButtonVisible = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonSensor Sensor
        {
            get { return _sensor; }
        }

        /// <summary>
        /// Gets the 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonQuickAccessToolbarItemCollection Items
        {
            get 
            {
                if (DropDownButtonVisible)
                {
                    if (!_items.Contains(DropDownButton))
                    {
                        _items.Add(DropDownButton);
                    }
                }
                else
                {
                    if (_items.Contains(DropDownButton))
                    {
                        _items.Remove(DropDownButton);
                    }
                }
                return _items; 
            }
        }

        #endregion

        #region Methods

        public override void OnPaint(object sender, RibbonElementPaintEventArgs e)
        {
            Owner.Renderer.OnRenderRibbonQuickAccessToolbarBackground(new RibbonRenderEventArgs(Owner, e.Graphics, e.Clip));

            foreach (RibbonItem item in Items)
            {
                item.OnPaint(this, new RibbonElementPaintEventArgs(item.Bounds, e.Graphics, RibbonElementSizeMode.Compact));
            }

        }

        public override Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            ///For RibbonItemGroup, size is always compact, and it's designed to be on an horizontal flow
            ///tab panel.
            ///
            int widthSum = Padding.Horizontal;
            int maxHeight = 16;

            foreach (RibbonItem item in Items)
            {
                if (item.Equals(DropDownButton)) continue;
                Size s = item.MeasureSize(this, new RibbonElementMeasureSizeEventArgs(e.Graphics, RibbonElementSizeMode.Compact));
                widthSum += s.Width + 1;
                maxHeight = Math.Max(maxHeight, s.Height);
            }

            widthSum -= 1;

            if (Site != null && Site.DesignMode) widthSum += 16;

            Size result = new Size(widthSum, maxHeight);
            SetLastMeasuredSize(result);
            return result;
        }

        public override void SetBounds(Rectangle bounds)
        {
            base.SetBounds(bounds);

            int curLeft = bounds.Left + Padding.Left;

            foreach (RibbonItem item in Items)
            {
                item.SetBounds(new Rectangle(new Point(curLeft, bounds.Top), item.LastMeasuredSize));

                curLeft = item.Bounds.Right + 1;
            }

            DropDownButton.SetBounds(new Rectangle(bounds.Right + bounds.Height / 2 + 2, bounds.Top, 12, bounds.Height));
        }

        #endregion

        #region IContainsRibbonComponents Members

        public IEnumerable<System.ComponentModel.Component> GetAllChildComponents()
        {
            return Items.ToArray();
        }

        #endregion

        #region IContainsSelectableRibbonItems Members

        public IEnumerable<RibbonItem> GetItems()
        {
            return Items;
        }

        public System.Drawing.Rectangle GetContentBounds()
        {
            return Bounds;
        }

        #endregion
    }
}
