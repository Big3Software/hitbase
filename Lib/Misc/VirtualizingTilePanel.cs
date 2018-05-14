using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Big3.Hitbase.Miscellaneous
{
    public class VirtualizingTilePanel : VirtualizingPanel, IScrollInfo
    {
        DispatcherTimer dtIdleCleaner = new DispatcherTimer();
        public VirtualizingTilePanel()
        {
            // For use in the IScrollInfo implementation
            this.RenderTransform = _trans;

            dtIdleCleaner.Interval = TimeSpan.FromMilliseconds(1000);
            dtIdleCleaner.Tick += new EventHandler(dtIdleCleaner_Tick);
        }

        // Dependency property that controls the size of the child elements
        public static readonly DependencyProperty ChildSizeProperty
           = DependencyProperty.RegisterAttached("ChildSize", typeof(Size), typeof(VirtualizingTilePanel),
              new FrameworkPropertyMetadata(new Size(200.0d, 200.0d), FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

        // Accessor for the child size dependency property
        public Size ChildSize
        {
            get { return (Size)GetValue(ChildSizeProperty); }
            set { SetValue(ChildSizeProperty, value); }
        }

        // Figure out range that's visible based on layout algorithm
        int firstVisibleItemIndex, lastVisibleItemIndex;
        
        /// <summary>
        /// Measure the children
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns>Size desired</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateScrollInfo(availableSize);

            GetVisibleRange(out firstVisibleItemIndex, out lastVisibleItemIndex);

            // We need to access InternalChildren before the generator to work around a bug
            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            // Get the generator position of the first visible data item
            GeneratorPosition startPos = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);

            // Get index where we'd insert the child for this position. If the item is realized
            // (position.Offset == 0), it's just position.Index, otherwise we have to add one to
            // insert after the corresponding child
            int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

            using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = firstVisibleItemIndex; itemIndex <= lastVisibleItemIndex; ++itemIndex, ++childIndex)
                {
                    bool newlyRealized;

                    // Get or create the child
                    UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                    if (newlyRealized)
                    {
                        // Figure out if we need to insert the child at the end or somewhere in the middle
                        if (childIndex >= children.Count)
                        {
                            base.AddInternalChild(child);
                        }
                        else
                        {
                            base.InsertInternalChild(childIndex, child);
                        }
                        generator.PrepareItemContainer(child);
                    }
                    else
                    {
                        // The child has already been created, let's be sure it's in the right spot
                        Debug.Assert(child == children[childIndex], "Wrong child was generated");
                    }

                    // Measurements will depend on layout algorithm
                    child.Measure(GetChildSize());
                }
            }

            //dtIdleCleaner.Stop();
            //dtIdleCleaner.Start();

            CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);

            return availableSize;
        }

        int lastFirstVisibleItemIndex = -1;
        int lastLastVisibleItemIndex = -1;
        void dtIdleCleaner_Tick(object sender, EventArgs e)
        {
            if (lastFirstVisibleItemIndex == firstVisibleItemIndex && lastLastVisibleItemIndex == lastVisibleItemIndex)
            {
                CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);
                dtIdleCleaner.Stop();
            }

            lastFirstVisibleItemIndex = firstVisibleItemIndex;
            lastLastVisibleItemIndex = lastVisibleItemIndex;
        }


        /// <summary>
        /// Arrange the children
        /// </summary>
        /// <param name="finalSize">Size available</param>
        /// <returns>Size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            UpdateScrollInfo(finalSize);

            for (int i = 0; i < this.Children.Count; i++)
            {
                UIElement child = this.Children[i];

                // Map the child offset to an item offset
                int itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

                ArrangeChild(itemIndex, child, finalSize);
            }

            return finalSize;
        }

        /// <summary>
        /// Revirtualize items that are no longer visible
        /// </summary>
        /// <param name="minDesiredGenerated">first item index that should be visible</param>
        /// <param name="maxDesiredGenerated">last item index that should be visible</param>
        private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated)
        {
            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
                if (itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated)
                {
                    generator.Remove(childGeneratorPos, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        /// <summary>
        /// When items are removed, remove the corresponding UI if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
            }
        }

        #region Layout specific code
        // I've isolated the layout specific code to this region. If you want to do something other than tiling, this is
        // where you'll make your changes

        /// <summary>
        /// Calculate the extent of the view based on the available size
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <param name="itemCount">number of data items</param>
        /// <returns></returns>
        private Size CalculateExtent(Size availableSize, int itemCount)
        {
            int childrenPerRow = CalculateChildrenPerRow(availableSize);

            // See how big we are
            return new Size(childrenPerRow * this.ChildSize.Width,
                this.ChildSize.Height * Math.Ceiling((double)itemCount / childrenPerRow));
        }

        /// <summary>
        /// Get the range of children that are visible
        /// </summary>
        /// <param name="firstVisibleItemIndex">The item index of the first visible item</param>
        /// <param name="lastVisibleItemIndex">The item index of the last visible item</param>
        private void GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            int childrenPerRow = CalculateChildrenPerRow(_extent);

            firstVisibleItemIndex = (int) Math.Floor(_offset.Y / this.ChildSize.Height) * childrenPerRow;
            lastVisibleItemIndex = (int) Math.Ceiling((_offset.Y + _viewport.Height) / this.ChildSize.Height) * childrenPerRow - 1;

            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;
            if (lastVisibleItemIndex >= itemCount)
                lastVisibleItemIndex = itemCount-1;
            
        }

        /// <summary>
        /// Get the size of the children. We assume they are all the same
        /// </summary>
        /// <returns>The size</returns>
        private Size GetChildSize()
        {
            return this.ChildSize;
        }

        private bool scrollUp = false;

        /// <summary>
        /// Position a child
        /// </summary>
        /// <param name="itemIndex">The data item index of the child</param>
        /// <param name="child">The element to position</param>
        /// <param name="finalSize">The size of the panel</param>
        private void ArrangeChild(int itemIndex, UIElement child, Size finalSize)
        {
            int childrenPerRow = CalculateChildrenPerRow(finalSize);

            int row = itemIndex / childrenPerRow;
            int column = itemIndex % childrenPerRow;

            child.Arrange(new Rect(column * this.ChildSize.Width, row * this.ChildSize.Height, this.ChildSize.Width, this.ChildSize.Height));

/*            double startPositionX = 0;
            double startPositionY = 0;
            bool animate = false;
            TranslateTransform tt;
            if (!(child.RenderTransform is TranslateTransform))
            {
                tt = new TranslateTransform(0, 0);
                child.RenderTransform = tt;
                animate = true;
                child.Arrange(new Rect(0, 0, this.ChildSize.Width, this.ChildSize.Height));
                startPositionX = column * this.ChildSize.Width;

                int firstVisibleItemIndex = 0;
                int lastVisibleItemIndex = 0;
                GetVisibleRange(out firstVisibleItemIndex, out lastVisibleItemIndex);

                if (scrollUp)
                    startPositionY = (row - 2) * this.ChildSize.Height;
                else
                    startPositionY = (row + 2) * this.ChildSize.Height;
                //                child.Arrange(new Rect(column * this.ChildSize.Width, row * this.ChildSize.Height, this.ChildSize.Width, this.ChildSize.Height));
            }
            else
            {
                tt = child.RenderTransform as TranslateTransform;
                animate = true;
                child.Arrange(new Rect(0, 0, this.ChildSize.Width, this.ChildSize.Height));
            }

            if (animate)
            {
                DoubleAnimation daX = new DoubleAnimation(column * this.ChildSize.Width, TimeSpan.FromMilliseconds(500).Duration());
                if (startPositionX > 0)
                    daX.From = startPositionX;
                DoubleAnimation daY = new DoubleAnimation(row * this.ChildSize.Height, TimeSpan.FromMilliseconds(500).Duration());
                if (startPositionY > 0)
                    daY.From = startPositionY;
                daX.EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut };
                daY.EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut };
                tt.BeginAnimation(TranslateTransform.XProperty, daX);
                tt.BeginAnimation(TranslateTransform.YProperty, daY);
            }*/
        }

        /// <summary>
        /// Helper function for tiling layout
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns></returns>
        private int CalculateChildrenPerRow(Size availableSize)
        {
            // Figure out how many children fit on each row
            int childrenPerRow;
            if (availableSize.Width == Double.PositiveInfinity)
                childrenPerRow = this.Children.Count;
            else
                childrenPerRow = Math.Max(1, (int)Math.Floor(availableSize.Width / this.ChildSize.Width));
            return childrenPerRow;
        }

        #endregion

        #region IScrollInfo implementation
        // See Ben Constable's series of posts at http://blogs.msdn.com/bencon/


        private void UpdateScrollInfo(Size availableSize)
        {
            // See how many items there are
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;

            Size extent = CalculateExtent(availableSize, itemCount);
            // Update extent
            if (extent != _extent)
            {
                _extent = extent;
                if (_owner != null)
                    _owner.InvalidateScrollInfo();
            }

            // Update viewport
            if (availableSize != _viewport)
            {
                _viewport = availableSize;
                if (_owner != null)
                    _owner.InvalidateScrollInfo();
            }
        }

        public ScrollViewer ScrollOwner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public bool CanHorizontallyScroll
        {
            get { return _canHScroll; }
            set { _canHScroll = value; }
        }

        public bool CanVerticallyScroll
        {
            get { return _canVScroll; }
            set { _canVScroll = value; }
        }

        public double HorizontalOffset
        {
            get { return _offset.X; }
        }

        public double VerticalOffset
        {
            get { return _offset.Y; }
        }

        public double ExtentHeight
        {
            get { return _extent.Height; }
        }

        public double ExtentWidth
        {
            get { return _extent.Width; }
        }

        public double ViewportHeight
        {
            get { return _viewport.Height; }
        }

        public double ViewportWidth
        {
            get { return _viewport.Width; }
        }

        public void LineUp()
        {
            SetVerticalOffset(this.VerticalOffset - 20);
        }

        public void LineDown()
        {
            SetVerticalOffset(this.VerticalOffset + 20);
        }

        public void PageUp()
        {
            SetVerticalOffset(this.VerticalOffset - _viewport.Height);
        }

        public void PageDown()
        {
            SetVerticalOffset(this.VerticalOffset + _viewport.Height);
        }

        public void MouseWheelUp()
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Tag = 20;
            dt.Tick += new EventHandler(dtUp_Tick);
            dt.Interval = TimeSpan.FromMilliseconds(10);
            dt.Start();
//            SetVerticalOffset(this.VerticalOffset - 20);
        }

        public void MouseWheelDown()
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Tag = 20;
            dt.Tick += new EventHandler(dtDown_Tick);
            dt.Interval = TimeSpan.FromMilliseconds(10);
            dt.Start();
            //SetVerticalOffset(this.VerticalOffset + 20);
        }

        void dtUp_Tick(object sender, EventArgs e)
        {
            int count = (int)((DispatcherTimer)sender).Tag;
            ((DispatcherTimer)sender).Tag = count - 1;
            SetVerticalOffset(this.VerticalOffset - count / 4);
            if (count == 1)
                ((DispatcherTimer)sender).Stop();

        }

        void dtDown_Tick(object sender, EventArgs e)
        {
            int count = (int)((DispatcherTimer)sender).Tag;
            ((DispatcherTimer)sender).Tag = count - 1;
            SetVerticalOffset(this.VerticalOffset + count/4);
            if (count == 1)
                ((DispatcherTimer)sender).Stop();
            
        }

        public void LineLeft()
        {
            throw new InvalidOperationException();
        }

        public void LineRight()
        {
            throw new InvalidOperationException();
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect();
        }

        public void MouseWheelLeft()
        {
            throw new InvalidOperationException();
        }

        public void MouseWheelRight()
        {
            throw new InvalidOperationException();
        }

        public void PageLeft()
        {
            throw new InvalidOperationException();
        }

        public void PageRight()
        {
            throw new InvalidOperationException();
        }

        public void SetHorizontalOffset(double offset)
        {
            throw new InvalidOperationException();
        }

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || _viewport.Height >= _extent.Height)
            {
                offset = 0;
            }
            else
            {
                if (offset + _viewport.Height >= _extent.Height)
                {
                    offset = _extent.Height - _viewport.Height;
                }
            }

            if (offset < _offset.Y)
                scrollUp = true;
            else
                scrollUp = false;

            _offset.Y = offset;

            if (_owner != null)
                _owner.InvalidateScrollInfo();

            _trans.Y = -offset;

            // Force us to realize the correct children
            InvalidateMeasure();
        }

        private TranslateTransform _trans = new TranslateTransform();
        private ScrollViewer _owner;
        private bool _canHScroll = false;
        private bool _canVScroll = false;
        private Size _extent = new Size(0, 0);
        private Size _viewport = new Size(0, 0);
        private Point _offset;

        #endregion

    }
}
