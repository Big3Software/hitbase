using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Big3.Hitbase.CatalogView
{
    public class VirtualizingTilePanel : VirtualizingPanel, IScrollInfo
    {
        List<double> itemsTopPosition = new List<double>();
        List<double> itemsHeight = new List<double>();
        double totalHeight = 0;

        public VirtualizingTilePanel()
        {
            // For use in the IScrollInfo implementation
            this.RenderTransform = _trans;
        }

        /// <summary>
        /// Measure the children
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns>Size desired</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateScrollInfo(availableSize);

            // Figure out range that's visible based on layout algorithm
            int firstVisibleItemIndex, lastVisibleItemIndex;
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
                        //Debug.Assert(child == children[childIndex], "Wrong child was generated");
                    }

                    // Measurements will depend on layout algorithm
                    child.Measure(GetChildSize());
                }
            }

            // Note: this could be deferred to idle time for efficiency
            CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);

            return availableSize;
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
                case NotifyCollectionChangedAction.Reset:
                    SetVerticalOffset(0); //TODO: Is this enough?
                    itemsTopPosition = new List<double>();
                    itemsHeight = new List<double>();
                    totalHeight = 0;
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
        private Size CalculateExtent(Size availableSize)
        {
            if (totalHeight > 0)
            {
                return new Size(availableSize.Width, totalHeight);
            }
            // Die Gesamtgr��e der Liste berechnen.
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);

            // We need to access InternalChildren before the generator to work around a bug
            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            // Get the generator position of the first visible data item
            GeneratorPosition startPos = generator.GeneratorPositionFromIndex(0);

            using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = 0; itemIndex < itemsControl.Items.Count; ++itemIndex)
                {
                    itemsTopPosition.Add(totalHeight);

                    // Get or create the child
                    bool newlyRealized;
                    UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;

                    generator.PrepareItemContainer(child);

                    // Measurements will depend on layout algorithm
                    //child.Measure(GetChildSize());
                    AlbumViewItem avi = itemsControl.Items[itemIndex] as AlbumViewItem;
                    double height = Math.Max((avi.Tracks.Count * 18)+10, 150);
                    totalHeight += height;
                    itemsHeight.Add(height);
                }
            }

            generator.RemoveAll();

            // See how big we are
            return new Size(this.ActualWidth, totalHeight);
        }

        /// <summary>
        /// Get the range of children that are visible
        /// </summary>
        /// <param name="firstVisibleItemIndex">The item index of the first visible item</param>
        /// <param name="lastVisibleItemIndex">The item index of the last visible item</param>
        private void GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            firstVisibleItemIndex = 0;
            lastVisibleItemIndex = 0;

            for (int i = 0; i < itemsTopPosition.Count; i++)
            {
                if (itemsTopPosition[i] + itemsHeight[i] >= _offset.Y)
                {
                    firstVisibleItemIndex = i;
                    break;
                }
            }

            for (int i = firstVisibleItemIndex+1; i < itemsTopPosition.Count; i++)
            {
                if (itemsTopPosition[i] >= _offset.Y + _viewport.Height)
                {
                    lastVisibleItemIndex = i;
                    break;
                }
            }

            if (lastVisibleItemIndex == 0)
                lastVisibleItemIndex = itemsTopPosition.Count - 1;
        }

        /// <summary>
        /// Get the size of the children. We assume they are all the same
        /// </summary>
        /// <returns>The size</returns>
        private Size GetChildSize()
        {
            double maxHeight = 1000000;     // H�he sollte eigentlich unbegrenzt sein
            return new Size(this.ActualWidth, maxHeight);
        }

        /// <summary>
        /// Position a child
        /// </summary>
        /// <param name="itemIndex">The data item index of the child</param>
        /// <param name="child">The element to position</param>
        /// <param name="finalSize">The size of the panel</param>
        private void ArrangeChild(int row, UIElement child, Size finalSize)
        {
            child.Arrange(new Rect(0, itemsTopPosition[row], finalSize.Width, itemsHeight[row]));
        }

        #endregion

        #region IScrollInfo implementation
        // See Ben Constable's series of posts at http://blogs.msdn.com/bencon/


        private void UpdateScrollInfo(Size availableSize)
        {
            // See how many items there are
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);

            Size extent = CalculateExtent(availableSize);
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
            SetVerticalOffset(this.VerticalOffset - 50);
        }

        public void LineDown()
        {
            SetVerticalOffset(this.VerticalOffset + 50);
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
            SetVerticalOffset(this.VerticalOffset - 50);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(this.VerticalOffset + 50);
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
