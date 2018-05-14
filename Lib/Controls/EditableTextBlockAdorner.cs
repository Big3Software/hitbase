using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Big3.Hitbase.Controls
{
    /// <summary>
    /// Adorner class which shows textbox over the text block when the Edit mode is on.
    /// </summary>
    public class EditableTextBlockAdorner : Adorner
    {
        private readonly VisualCollection _collection;

        private readonly TextBox _textBox;

        private readonly TextBlock _textBlock;

        internal string InitialText { get; set; }

        public EditableTextBlockAdorner(EditableTextBlock adornedElement)
            : base(adornedElement)
        {
            _collection = new VisualCollection(this);
            _textBox = new TextBox();
            _textBlock = adornedElement;
            Binding binding = new Binding("Text") {Source = adornedElement};
            _textBox.SetBinding(TextBox.TextProperty, binding);
            _textBox.AcceptsReturn = true;
            _textBox.MaxLength = adornedElement.MaxLength;
            _textBox.PreviewKeyDown += _textBox_PreviewKeyDown;
            _collection.Add(_textBox);
        }

        void _textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression expression = _textBox.GetBindingExpression(TextBox.TextProperty);
                if (null != expression)
                {
                    expression.UpdateSource();
                }
                e.Handled = true;
            }

            if (e.Key == Key.Escape)
            {
                _textBox.Text = InitialText;
                BindingExpression expression = _textBox.GetBindingExpression(TextBox.TextProperty);
                if (null != expression)
                {
                    expression.UpdateSource();
                }
                e.Handled = true;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _collection[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _collection.Count;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = _textBlock.ActualWidth + 10;
            if (width < 100)
                width = 100;
            _textBox.Arrange(new Rect(-5, -3, width, 20));
            _textBox.Width = width;
            _textBox.Focus();
            return finalSize;
        }

        public event RoutedEventHandler TextBoxLostFocus
        {
            add
            {
                _textBox.LostFocus += value;
            }
            remove
            {
                _textBox.LostFocus -= value;
            }
        }

        public event KeyEventHandler TextBoxKeyUp
        {
            add
            {
                _textBox.KeyUp += value;
            }
            remove
            {
                _textBox.KeyUp -= value;
            }
        }
    }
}
