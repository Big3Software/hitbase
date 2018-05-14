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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a tab that can contain RibbonPanel objects
    /// </summary>
    [DesignTimeVisible(false)]
    [Designer(typeof(RibbonTabDesigner))]
    public class RibbonTabTextBox : RibbonItem, IRibbonElement
    {
        private Ribbon _owner;
        private Rectangle _tabBounds;
        private TextBox _actualTextBox;
        private bool _removingTxt;
        private string _textBoxText;
        private bool _selected;
        private bool _alignRight;

        /// <summary>
        /// Raised when the <see cref="TextBoxText"/> property value has changed
        /// </summary>
        public event EventHandler TextBoxTextChanged;

        #region Ctor

        public RibbonTabTextBox()
        {
        }

        /// <summary>
        /// Creates a new RibbonTab
        /// </summary>
        public RibbonTabTextBox(Ribbon owner, string text)
        {
        } 
        #endregion

        /// <summary>
        /// Gets the <see cref="TabBounds"/> property value
        /// </summary>
        public Rectangle Bounds
        {
            get { return TabBounds; }
        }

        public bool AlignRight
        {
            get
            {
                return _alignRight;
            }
            set
            {
                _alignRight = value;
            }
        }

        public override Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            Size size = Size.Empty;

            int w = 0;
            int twidth = Width;

            w += Width;

            SetLastMeasuredSize(new Size(w, 11));

            return LastMeasuredSize;
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

        #region IRibbonElement Members

        public override void OnPaint(object sender, RibbonElementPaintEventArgs e)
        {
            if (Owner == null) return;

            Owner.Renderer.OnRenderRibbonTabTextBox(new RibbonTabTextBoxRenderEventArgs(Owner, e.Graphics, e.Clip, this));
        }

        /// <summary>
        /// Overriden. Raises the Bound
        /// </summary>
        public void SetBounds(System.Drawing.Rectangle bounds)
        {
            throw new Exception("Not used");
        }

        /// <summary>
        /// Sets the value of the TabBounds property
        /// </summary>
        /// <param name="tabBounds">Rectangle representing the bounds of the tab</param>
        internal void SetTabBounds(Rectangle tabBounds)
        {
            _tabBounds = tabBounds;
        }

        private int width = 50;
        public int Width 
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        /// <summary>
        /// Gets a value indicating if user is currently editing the text of the textbox
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Editing
        {
            get { return _actualTextBox != null; }
        }

        /// <summary>
        /// Starts editing the text and focuses the TextBox
        /// </summary>
        public void StartEdit()
        {
            //if (!Enabled) return;

            PlaceActualTextBox();

            _actualTextBox.SelectAll();
            _actualTextBox.Focus();
        }

        /// <summary>
        /// Ends the editing of the textbox
        /// </summary>
        public void EndEdit()
        {
            RemoveActualTextBox();
        }

        /// <summary>
        /// Places the Actual TextBox on the owner so user can edit the text
        /// </summary>
        protected void PlaceActualTextBox()
        {
            _actualTextBox = new TextBox();

            InitTextBox(_actualTextBox);

            _actualTextBox.TextChanged += new EventHandler(_actualTextbox_TextChanged);
            _actualTextBox.KeyDown += new KeyEventHandler(_actualTextbox_KeyDown);
            _actualTextBox.LostFocus += new EventHandler(_actualTextbox_LostFocus);
            _actualTextBox.VisibleChanged += new EventHandler(_actualTextBox_VisibleChanged);

            _actualTextBox.Visible = true;
            Owner.Controls.Add(_actualTextBox);
        }

        private void _actualTextBox_VisibleChanged(object sender, EventArgs e)
        {
            if (!(sender as TextBox).Visible && !_removingTxt)
            {
                RemoveActualTextBox();
            }
        }

        /// <summary>
        /// Removes the actual TextBox that edits the text
        /// </summary>
        protected void RemoveActualTextBox()
        {
            if (_actualTextBox == null || _removingTxt)
            {
                return;
            }
            _removingTxt = true;

            TextBoxText = _actualTextBox.Text;
            _actualTextBox.Visible = false;
            _actualTextBox.Parent.Controls.Remove(_actualTextBox);
            _actualTextBox.Dispose();
            _actualTextBox = null;

            //RedrawItem();
            _removingTxt = false;
        }

        /// <summary>
        /// Gets or sets the text on the textbox
        /// </summary>
        [Description("Text on the textbox")]
        public string TextBoxText
        {
            get { return _textBoxText; }
            set
            {
                _textBoxText = value;

                if (TextBoxTextChanged != null)
                {
                    TextBoxTextChanged(this, new EventArgs());
                }
            }
        }


        /// <summary>
        /// Initializes the texbox that edits the text
        /// </summary>
        /// <param name="t"></param>
        protected virtual void InitTextBox(TextBox t)
        {
            t.Text = this.TextBoxText;
            t.BorderStyle = BorderStyle.None;
            t.Width = TabBounds.Width - 2;

            t.Location = new Point(
                TabBounds.Left + 2,
                Bounds.Top + (Bounds.Height - t.Height) / 2);
        }

        /// <summary>
        /// Handles the LostFocus event of the actual TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _actualTextbox_LostFocus(object sender, EventArgs e)
        {
            RemoveActualTextBox();
        }

        /// <summary>
        /// Handles the KeyDown event of the actual TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _actualTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return ||
                e.KeyCode == Keys.Enter ||
                e.KeyCode == Keys.Escape)
            {
                RemoveActualTextBox();
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the actual TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _actualTextbox_TextChanged(object sender, EventArgs e)
        {
            TextBoxText = (sender as TextBox).Text;
        }

        /// <summary>
        /// Sets the value of the Owner Property
        /// </summary>
        internal void SetOwner(Ribbon owner)
        {
            _owner = owner;
        }

        public void SetTabSelected(bool selected)
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

        /// <summary>
        /// Sets the value of the Pressed property
        /// </summary>
        /// <param name="pressed">Value that indicates if the element is pressed</param>
        internal void SetPressed(bool pressed)
        {
            //_pressed = pressed;
        }

        #endregion

        public override void OnMouseEnter(MouseEventArgs e)
        {
            if (!Enabled) return;

            base.OnMouseEnter(e);

            Owner.Cursor = Cursors.IBeam;
        }

        public override void OnMouseLeave(MouseEventArgs e)
        {
            if (!Enabled) return;

            base.OnMouseLeave(e);

            Owner.Cursor = Cursors.Default;
        }

        public void SetActive()
        {
            StartEdit();
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (!Enabled) return;

            base.OnMouseDown(e);

            if (Bounds.Contains(e.X, e.Y))
            {
                StartEdit();
            }

        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (!Enabled) return;

            base.OnMouseMove(e);

            if (Bounds.Contains(e.X, e.Y))
            {
                Owner.Cursor = Cursors.IBeam;
            }
            else
            {
                Owner.Cursor = Cursors.Default;
            }
        }
    }
}
