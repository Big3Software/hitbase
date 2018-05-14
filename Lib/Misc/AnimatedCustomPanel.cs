using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Big3.Hitbase.Miscellaneous
{


    #region #using Directives

    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;

    #endregion

    public class AnimatedWrapPanel : WrapPanel
    {
        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(Duration), typeof(AnimatedWrapPanel),
                                                                                                 new UIPropertyMetadata(
                                                                                                    new Duration(new TimeSpan(0, 0, 0, 0, 500))));

        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        /// <summary>
        /// Arranges the content of a <see cref="T:System.Windows.Controls.WrapPanel"/> element.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the arranged size of this <see cref="T:System.Windows.Controls.WrapPanel"/> element and its children.
        /// </returns>
        /// <param name="finalSize">The <see cref="T:System.Windows.Size"/> that this element should use to arrange its child elements.
        ///                 </param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int start = 0;
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            double v = 0.0;
            double itemU = (Orientation == Orientation.Horizontal) ? itemWidth : itemHeight;
            var size = new UVSize(Orientation);
            var size2 = new UVSize(Orientation, finalSize.Width, finalSize.Height);
            bool flag = !DoubleUtil.IsNaN(itemWidth);
            bool flag2 = !DoubleUtil.IsNaN(itemHeight);
            bool useItemU = (Orientation == Orientation.Horizontal) ? flag : flag2;
            var internalChildren = base.InternalChildren;
            int end = 0;
            int count = internalChildren.Count;
            while (end < count)
            {
                var element = internalChildren[end];
                if (element != null)
                {
                    var size3 = new UVSize(Orientation, flag ? itemWidth : element.DesiredSize.Width, flag2 ? itemHeight : element.DesiredSize.Height);
                    if (DoubleUtil.GreaterThan(size.U + size3.U, size2.U))
                    {
                        arrangeLine(v, size.V, start, end, useItemU, itemU);
                        v += size.V;
                        size = size3;
                        if (DoubleUtil.GreaterThan(size3.U, size2.U))
                        {
                            arrangeLine(v, size3.V, end, ++end, useItemU, itemU);
                            v += size3.V;
                            size = new UVSize(Orientation);
                        }
                        start = end;
                    }
                    else
                    {
                        size.U += size3.U;
                        size.V = Math.Max(size3.V, size.V);
                    }
                }
                end++;
            }
            if (start < internalChildren.Count)
            {
                arrangeLine(v, size.V, start, internalChildren.Count, useItemU, itemU);
            }
            return finalSize;
        }

        private void arrangeLine(double v, double lineV, int start, int end, bool useItemU, double itemU)
        {
            double num = 0.0;
            bool horizontal = Orientation == Orientation.Horizontal;
            var duration = Duration;
            var internalChildren = base.InternalChildren;
            for (int i = start; i < end; i++)
            {
                var element = internalChildren[i];
                if (element != null)
                {
                    var size = new UVSize(Orientation, element.DesiredSize.Width, element.DesiredSize.Height);
                    double u = useItemU ? itemU : size.U;
                    AnimatedPanelHelper.ArrangeChild(this, element,
                                                     new Rect(horizontal ? num : v, horizontal ? v : num, horizontal ? u : lineV, horizontal ? lineV : u), duration);
                    num += u;
                }
            }
        }

        #region Nested type: DoubleUtil

        private static class DoubleUtil
        {
            // Fields
            private const double DBL_EPSILON = 2.2204460492503131E-16;

            // Methods
            private static bool AreClose(double value1, double value2)
            {
                if (value1 == value2)
                {
                    return true;
                }
                double num = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * DBL_EPSILON;
                double num2 = value1 - value2;
                return ((-num < num2) && (num > num2));
            }

            internal static bool GreaterThan(double value1, double value2)
            {
                return ((value1 > value2) && !AreClose(value1, value2));
            }

            internal static bool IsNaN(double value)
            {
                var union = new NanUnion
                {
                    DoubleValue = value
                };
                ulong num = union.UintValue & 18442240474082181120L;
                ulong num2 = union.UintValue & (0xfffffffffffffL);
                if ((num != 0x7ff0000000000000L) && (num != 18442240474082181120L))
                {
                    return false;
                }
                return (num2 != 0L);
            }

            // Nested Types

            #region Nested type: NanUnion

            [StructLayout(LayoutKind.Explicit)]
            private struct NanUnion
            {
                // Fields
                [FieldOffset(0)]
                internal double DoubleValue;
                [FieldOffset(0)]
                internal ulong UintValue;
            }

            #endregion
        }

        #endregion

        #region Nested type: UVSize

        [StructLayout(LayoutKind.Sequential)]
        private struct UVSize
        {
            internal double U;
            internal double V;
            private readonly Orientation _orientation;

            internal UVSize(Orientation orientation, double width, double height)
            {
                this.U = this.V = 0.0;
                this._orientation = orientation;
                Width = width;
                Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                this.U = this.V = 0.0;
                this._orientation = orientation;
            }

            private double Width
            {
                get
                {
                    if (this._orientation != Orientation.Horizontal)
                    {
                        return this.V;
                    }
                    return this.U;
                }
                set
                {
                    if (this._orientation == Orientation.Horizontal)
                    {
                        this.U = value;
                    }
                    else
                    {
                        this.V = value;
                    }
                }
            }

            private double Height
            {
                get
                {
                    if (this._orientation != Orientation.Horizontal)
                    {
                        return this.U;
                    }
                    return this.V;
                }
                set
                {
                    if (this._orientation == Orientation.Horizontal)
                    {
                        this.V = value;
                    }
                    else
                    {
                        this.U = value;
                    }
                }
            }
        }

        #endregion
    }

    internal class AnimatedPanelHelper
    {
        public static void ArrangeChild(UIElement panel, UIElement element, Rect finalRect, Duration duration)
        {
            var trans = element.RenderTransform as TranslateTransform;
            // create new Translate Transform
            if (trans == null)
            {
                trans = new TranslateTransform();
                element.RenderTransform = trans;
            }

            // tell the child element to arrange itself
            element.Arrange(new Rect(finalRect.Size));

            // keep translation point and store it in its Tag property for rendering later.
            var translatePosition = finalRect.TopLeft;

            panel.Dispatcher.BeginInvoke(new Action<FrameworkElement, KeyValuePair<Point, Duration>>(BeginAnimation), DispatcherPriority.Render, element,
                                         new KeyValuePair<Point, Duration>(translatePosition, duration));
        }

        private static void BeginAnimation(FrameworkElement element, KeyValuePair<Point, Duration> translatePositionAndDuration)
        {
            var xKeyFrame = new SplineDoubleKeyFrame
            {
                KeyTime = KeyTime.FromPercent(1.0),
                Value = translatePositionAndDuration.Key.X
            };
            var yKeyFrame = new SplineDoubleKeyFrame
            {
                KeyTime = KeyTime.FromPercent(1.0),
                Value = translatePositionAndDuration.Key.Y
            };
            var animationX = new DoubleAnimationUsingKeyFrames
            {
                Duration = translatePositionAndDuration.Value,
                AccelerationRatio = 0.5,
                DecelerationRatio = 0.5,
                KeyFrames = new DoubleKeyFrameCollection
			                 		            	{
			                 		            		xKeyFrame,
			                 		            		xKeyFrame
			                 		            	}
            };
            var animationY = new DoubleAnimationUsingKeyFrames
            {
                Duration = translatePositionAndDuration.Value,
                AccelerationRatio = 0.5,
                DecelerationRatio = 0.5,
                KeyFrames = new DoubleKeyFrameCollection
			                 		            	{
			                 		            		yKeyFrame,
			                 		            		yKeyFrame
			                 		            	}
            };

            // hook up X and Y translate transformations
            element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animationX, HandoffBehavior.Compose);
            element.RenderTransform.BeginAnimation(TranslateTransform.YProperty, animationY, HandoffBehavior.Compose);
        }
    }

#if false

    public class AnimatedWrapPanel : AnimatedPanel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in this.Children)
            {
                child.Measure(availableSize);
            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0;
            double y = 0;
            double maxHeight = 0;
            foreach (UIElement child in this.Children)
            {
                if (x + child.DesiredSize.Width >= finalSize.Width)
                {
                    x = 0;
                    y += maxHeight;
                    maxHeight = 0;
                }

                base.AnimatedArrange(child, new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
                maxHeight = Math.Max(child.DesiredSize.Height, maxHeight);
                x += child.DesiredSize.Width;

            }

            return base.ArrangeOverride(finalSize);
        }
    }

    public class AnimatedPanel : Panel
    {
        Dictionary<UIElement, Rect> targetPositions = new Dictionary<UIElement, Rect>();
        Dictionary<UIElement, Rect> currentPositions = new Dictionary<UIElement, Rect>();
        Dictionary<UIElement, Rect> startingPositions = new Dictionary<UIElement, Rect>();
        DateTime lastUpdateTime = DateTime.Now;
        DateTime endTime = DateTime.Now;

        public bool IsAnimating { get; set; }

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(AnimatedPanel), new UIPropertyMetadata(TimeSpan.FromMilliseconds(500)));

        protected override Size ArrangeOverride(Size finalSize)
        {
            DateTime now = DateTime.Now;

            foreach (UIElement child in this.Children)
            {
                if (!this.targetPositions.ContainsKey(child))
                    throw new InvalidOperationException("Must call AnimatedPanel.AnimatedArrange for all children");

                if (!this.currentPositions.ContainsKey(child))
                    this.currentPositions[child] = this.targetPositions[child];

                if (!this.startingPositions.ContainsKey(child))
                    this.startingPositions[child] = this.targetPositions[child];
            }

            bool somethingMoved = false;
            foreach (UIElement child in this.Children)
            {
                if (this.startingPositions.ContainsKey(child))
                {
                    Rect s = this.startingPositions[child];
                    Rect t = this.targetPositions[child];
                    if (s.Left != t.Left || s.Top != t.Top || s.Width != t.Width || s.Height != t.Height)
                    {
                        somethingMoved = true;
                        break;
                    }
                }
            }

            if (somethingMoved)
            {
                // Start animating (make endTime later than now)
                this.IsAnimating = true;
                this.lastUpdateTime = now;
                this.endTime = this.lastUpdateTime.AddMilliseconds(this.Duration.TotalMilliseconds);
                foreach (UIElement child in this.Children)
                {
                    this.startingPositions[child] = this.targetPositions[child];
                }
            }

            double timeRemaining = (this.endTime - now).TotalMilliseconds;

            double deltaTimeSinceLastUpdate = (now - lastUpdateTime).TotalMilliseconds;
            if (deltaTimeSinceLastUpdate > timeRemaining)
                deltaTimeSinceLastUpdate = timeRemaining;

            DateTime startTime = this.endTime.AddMilliseconds(-this.Duration.TotalMilliseconds);
            double timeIntoAnimation = (now - startTime).TotalMilliseconds;

            double fractionComplete;
            if (timeRemaining > 0)
                fractionComplete = GetCurrentValue(timeIntoAnimation, 0, 1, this.Duration.TotalMilliseconds);
            else
                fractionComplete = 1;

            // Debug.WriteLine("Arrange " + fractionComplete.ToString());

            foreach (UIElement child in this.Children)
            {
                Rect t = this.targetPositions[child];
                Rect c = this.currentPositions[child];
                double left = ((t.Left - c.Left) * fractionComplete) + c.Left;
                Rect pos = new Rect(left, ((t.Top - c.Top) * fractionComplete) + c.Top,
                ((t.Width - c.Width) * fractionComplete) + c.Width, ((t.Height - c.Height) * fractionComplete) + c.Height);

                child.Arrange(pos);
                this.currentPositions[child] = pos;
            }

            this.lastUpdateTime = now;

            CompositionTarget.Rendering -= CompositionTarget_Rendering;

            if (timeRemaining > 0)
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                this.IsAnimating = false;
            }

            Clean(this.startingPositions);
            Clean(this.currentPositions);
            Clean(this.targetPositions);

            return base.ArrangeOverride(finalSize);
        }

        // Dictionary may reference children that have been removed
        private void Clean(Dictionary<UIElement, Rect> dictionary)
        {
            if (dictionary.Count != this.Children.Count)
            {
                Dictionary<UIElement, Rect> newDictionary = new Dictionary<UIElement, Rect>();
                foreach (UIElement child in this.Children)
                {
                    newDictionary[child] = dictionary[child];
                }

                dictionary = newDictionary;
            }
        }

        public void AnimatedArrange(UIElement child, Rect finalSize)
        {
            this.targetPositions[child] = finalSize;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            this.InvalidateArrange();
        }

        /**
        * Default easing equation: EaseInOutQuad 
        * Easing equation function for a quadratic (t^2) easing in/out: acceleration until halfway, then deceleration.
        *
        * time Current time (in milliseconds).
        * begin Starting value.
        * change Change needed in value.
        * duration Expected easing duration (in milliseconds).
        * @return The correct value.
        */
        public virtual double GetCurrentValue(double time, double begin, double change, double duration)
        {
            if ((time /= duration / 2) < 1)
                return change / 2 * time * time + begin;
            return -change / 2 * ((--time) * (time - 2) - 1) + begin;
        }
    }

#endif
}
