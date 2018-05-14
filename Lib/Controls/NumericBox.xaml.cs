using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Big3.Hitbase.Controls;


namespace Big3.Hitbase.Controls
{
    [TemplatePart(Name = "PART_Popup", Type = typeof(System.Windows.Controls.Primitives.Popup))]
    [TemplatePart(Name = "PART_IncrementTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_NumericTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_MenuItem", Type = typeof(MenuItem))]
    [TemplatePart(Name = "PART_IncreaseButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_DecreaseButton", Type = typeof(Button))]
    /// <summary>
    /// WPF User control - NumericBox
    /// </summary>
    public partial class NumericBox : UserControl
    {
        #region Variables

        private double value;           // value
        private double increment;       // increment
        private double minimum;         // minimum value
        private double maximum;         // maximum value

        private string valueFormat;     // string format of the value

        private DispatcherTimer timer;  // timer for Increaseing/Decreasing value with certain time interval

        #endregion

        public NumericBox()
        {
            InitializeComponent();

            // Set timer properties
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(500.0);
        }

        #region Properties

        #region Overrided properties
        /*
         * Override the properties for PART_NumericTextBox
         */
        new public Brush Foreground
        {
            get { return PART_NumericTextBox.Foreground; }
            set { PART_NumericTextBox.Foreground = value; }
        }

        #endregion      

        #region Value format
        /*
         * --- Property name - ValueFormat ---
         * This property is necessary to show the value in a specific format
         */
        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register("ValueFormat", typeof(string), typeof(NumericBox), new PropertyMetadata("0", OnValueFormatChanged));

        private static void OnValueFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            NumericBox numericBoxControl = new NumericBox();
            numericBoxControl.valueFormat = (string)args.NewValue;
        }
        //===========================================================
        /// <summary>
        /// Set or get value format
        /// </summary>
        public string ValueFormat
        {
            get { return (string)GetValue(ValueFormatProperty); }
            set { SetValue(ValueFormatProperty, value); }
        }

        #endregion

        #region Min\Max value property
        /*
         * --- Property name - Minimum ---
         * This property sets a minimum bound of the value
         */
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumericBox), new PropertyMetadata(Double.MinValue, OnMinimumChanged));

        private static void OnMinimumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            NumericBox numericBoxControl = new NumericBox();
            numericBoxControl.minimum = (double)args.NewValue;
        }
        //===========================================================
        /// <summary>
        /// Set or get minimum
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------
        /*
         * --- Property name - Maximum ---
         * This property sets a maximum bound of the value
         */
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumericBox), new PropertyMetadata(Double.MaxValue, OnMaximumChanged));

        private static void OnMaximumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            NumericBox numericBoxControl = new NumericBox();
            numericBoxControl.maximum = (double)args.NewValue;
        }
        //===========================================================
        /// <summary>
        /// Set or get maximum
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        #endregion

        #region Increment property
        /*
         * --- Property name - Increment ---
         * This property defines an increment value
         */
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double), typeof(NumericBox), new PropertyMetadata((double)1, OnIncrementChanged));

        private static void OnIncrementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            NumericBox numericBoxControl = new NumericBox();
            numericBoxControl.increment = (double)args.NewValue;
        }
        //===========================================================
        /// <summary>
        /// Set or get increment
        /// </summary>
        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        #endregion

        #region Value property
        /*
         * --- Property name - Value ---
         * This property defines a current value
         */
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register("Value", typeof(double), typeof(NumericBox), new PropertyMetadata(new Double(), OnValueChanged));

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            NumericBox numericBoxControl = (NumericBox)sender;
            numericBoxControl.value = (double)args.NewValue;
            numericBoxControl.PART_NumericTextBox.Text = numericBoxControl.value.ToString(numericBoxControl.ValueFormat);
            numericBoxControl.OnValueChanged((double)args.OldValue, (double)args.NewValue);
        }
        //===========================================================
        /// <summary>
        /// Set or get value
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        #endregion

        #endregion

        #region Custom events

        #region ValueEvent
        /*
         * Registering new event - ValueChanged. It is invoked when the ValueProperty changes 
         */
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<double>), typeof(NumericBox));

        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private void OnValueChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventArgs<double> args = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);
            args.RoutedEvent = NumericBox.ValueChangedEvent;
            RaiseEvent(args);
        }
        #endregion

        #endregion

        #region Events

        #region Buttons Event

        private void increaseBtn_Click(object sender, RoutedEventArgs e)
        {
            IncreaseValue();
        }

        private void decreaseBtn_Click(object sender, RoutedEventArgs e)
        {
            DecreaseValue();
        }

        #endregion

        #region Text input event
        private void numericBox_TextInput(object sender, TextCompositionEventArgs e)
        {
           try
            {
                double tempValue = Double.Parse(PART_NumericTextBox.Text);
                if (!(tempValue < Minimum || tempValue > Maximum)) Value = tempValue;
            }
            catch (FormatException)
            {
            }
            
        }
        #endregion

        #region Mouse wheel event

        private void numericBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) IncreaseValue();
            else if (e.Delta < 0) DecreaseValue();
        }

        #endregion

        #region Mouse left button event
        private void optionsPopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetIncrement();
            PART_Popup.IsOpen = false;
        }
        #endregion

        #region Menu event
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PART_IncrementTextBox.Text = Increment.ToString();
            PART_Popup.IsOpen = true;
        }
        #endregion

        #region KeyDown event
        private void incrementTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetIncrement();
                PART_Popup.IsOpen = false;
            }
        }
        #endregion

        #region Change value when Increase\Decrease buttons are pressed

        private void Increase_Timer_Tick(object sender, EventArgs e)
        {
            IncreaseValue();
            this.timer.Interval = TimeSpan.FromMilliseconds(100.0);
        }

        private void Deccrease_Timer_Tick(object sender, EventArgs e)
        {
            DecreaseValue();
            this.timer.Interval = TimeSpan.FromMilliseconds(100.0);
        }

        private void increaseBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.timer.Tick += Increase_Timer_Tick;
            timer.Start();
        }

        private void increaseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.timer.Tick -= Increase_Timer_Tick;
            timer.Stop();
            this.timer.Interval = TimeSpan.FromMilliseconds(500.0);
        }

        private void decreaseBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.timer.Tick += Deccrease_Timer_Tick;
            timer.Start();
        }

        private void decreaseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.timer.Tick -= Deccrease_Timer_Tick;
            timer.Stop();
            this.timer.Interval = TimeSpan.FromMilliseconds(500.0);
        }

        #endregion

        #endregion

        #region Private Methods
        //=============================================================
        /// <summary>
        /// Set increment
        /// </summary>
        private void SetIncrement()
        {
            try
            {
                Increment = Double.Parse(PART_IncrementTextBox.Text); 
            }
            catch (FormatException)
            {
                Increment = 1;
            }
        }
        //=============================================================
        /// <summary>
        /// Increase value
        /// </summary>
        private void IncreaseValue()
        {
            Value += Increment;
            if (Value < Minimum || Value > Maximum) Value -= Increment;
        }
        //=============================================================
        /// <summary>
        /// Decrease value
        /// </summary>
        private void DecreaseValue()
        {
            Value -= Increment;
            if (Value < Minimum || Value > Maximum) Value += Increment;
        }
        #endregion

        #region Overrided Methods
        //=============================================================
        /// <summary>
        /// Apply new templates after setting new style
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button btn = GetTemplateChild("PART_IncreaseButton") as Button;
            if (btn != null)
            {
                btn.Click += increaseBtn_Click;
                btn.PreviewMouseLeftButtonDown += increaseBtn_PreviewMouseLeftButtonDown;
                btn.PreviewMouseLeftButtonUp += increaseBtn_PreviewMouseLeftButtonUp;
            }

            btn = GetTemplateChild("PART_DecreaseButton") as Button;
            if (btn != null)
            {
                btn.Click += decreaseBtn_Click;
                btn.PreviewMouseLeftButtonDown += decreaseBtn_PreviewMouseLeftButtonDown;
                btn.PreviewMouseLeftButtonUp += decreaseBtn_PreviewMouseLeftButtonUp;
            }

            TextBox tb = GetTemplateChild("PART_NumericTextBox") as TextBox;
            if (tb != null)
            {
                PART_NumericTextBox = tb;
                PART_NumericTextBox.Text = Value.ToString(ValueFormat);
                PART_NumericTextBox.PreviewTextInput += numericBox_TextInput;
                PART_NumericTextBox.MouseWheel += numericBox_MouseWheel;
            }

            System.Windows.Controls.Primitives.Popup popup = GetTemplateChild("PART_Popup") as System.Windows.Controls.Primitives.Popup;
            if (popup != null)
            {
                PART_Popup = popup;
                PART_Popup.MouseLeftButtonDown += optionsPopup_MouseLeftButtonDown;
            }

            tb = GetTemplateChild("PART_IncrementTextBox") as TextBox;
            if (tb != null)
            {
                PART_IncrementTextBox = tb;
                PART_IncrementTextBox.KeyDown += incrementTB_KeyDown;
            }

            MenuItem mi = GetTemplateChild("PART_MenuItem") as MenuItem;
            if (mi != null)
            {
                PART_MenuItem = mi;
                PART_MenuItem.Click += MenuItem_Click;
            }
            btn = null;
            mi = null;
            tb = null;
            popup = null;

        }
        #endregion
    }
}
