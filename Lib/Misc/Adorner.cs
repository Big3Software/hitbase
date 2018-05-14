using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Big3.Hitbase.Miscellaneous
{
    // Adorners must subclass the abstract base class Adorner.
    public class SimpleCircleAdorner : Adorner
    {
        // Be sure to call the base class constructor.
        public SimpleCircleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        // A common way to implement an adorner's rendering behavior is to override the OnRender
        // method, which is called by the layout system as part of a rendering pass.
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.White);
            //renderBrush.Opacity = 0.2;
            double renderRadius = 5.0;

            // Draw a circle at each corner.
            Viewbox parentViewbox = VisualTreeExtensions.FindParent<Viewbox>(this.AdornedElement);

            double factor = parentViewbox.Child.RenderSize.Width / parentViewbox.RenderSize.Width;
            renderRadius *= factor;
            Pen renderPen = new Pen(Brushes.Black, 1.5 * factor);

            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }
    }
}
