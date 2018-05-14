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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    [ToolboxItem(false)]
    public partial class RibbonPanelPopup : RibbonPopup
    {
        #region Fields
        private RibbonSensor _sensor;
        private RibbonPanel _panel;
        private bool _avoidDeactivate;
        private bool _ignoreNext;

        #endregion

        #region Ctor
        internal RibbonPanelPopup(RibbonPanel panel)
        {
            DoubleBuffered = true;

            _sensor = new RibbonSensor(this, panel.Owner, panel.OwnerTab);
            _sensor.PanelLimit = panel;
            _panel = panel;
            _panel.PopUp = this;

            using (Graphics g = CreateGraphics())
            {
                Size s = panel.MeasureSize(this, new RibbonElementMeasureSizeEventArgs(g,
                    GetSizeMode(panel)));
                //s.Width+=3; s.Height+=3;
                Size = s;
                panel.SetBounds(new Rectangle(0, 0, Size.Width, Size.Height));
                panel.UpdateItemsRegions(g, GetSizeMode(panel));
            }

            foreach (RibbonItem item in panel.Items)
            {
                item.SetCanvas(this);
            }
        } 
        #endregion
        
        #region Props

        public RibbonSensor Sensor
        {
            get
            {
                return _sensor;
            }
        }

        /// <summary>
        /// Gets the panel related to the form
        /// </summary>
        public RibbonPanel Panel
        {
            get
            {
                return _panel;
            }
        } 

        #endregion

        #region Methods

        public RibbonElementSizeMode GetSizeMode(RibbonPanel pnl)
        {
            if (pnl.FlowsTo == RibbonPanelFlowDirection.Right)
            {
                return RibbonElementSizeMode.Medium;
            }
            else
            {
                return RibbonElementSizeMode.Large;
            }
        }

        /// <summary>
        /// Prevents the form from being hidden the next time the mouse clicks on the form.
        /// It is useful for reacting to clicks of items inside items.
        /// </summary>
        public void IgnoreNextClickDeactivation()
        {
            _ignoreNext = true;
        }

        #endregion

        #region Overrides
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            _avoidDeactivate = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _avoidDeactivate = false;

            if (_ignoreNext)
            {
                _ignoreNext = false;
                return;
            }

            Close();
        }

        

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Panel.avoidPaintBg = true;
            Panel.Owner.Renderer.OnRenderPanelPopupBackground(
                new RibbonCanvasEventArgs(Panel.Owner, e.Graphics, new Rectangle(Point.Empty, ClientSize), this, Panel));

            //Panel.OnPaint(this, new RibbonElementPaintEventArgs(
            //    new Rectangle(Point.Empty, Panel.Bounds.Size), e.Graphics, GetSizeMode(Panel)));
            foreach (RibbonItem item in Panel.Items)
            {
                item.OnPaint(this, new RibbonElementPaintEventArgs(e.ClipRectangle, e.Graphics, RibbonElementSizeMode.Large));
            }

            Panel.Owner.Renderer.OnRenderRibbonPanelText(new RibbonPanelRenderEventArgs(Panel.Owner, e.Graphics, e.ClipRectangle, Panel, this));

            Panel.avoidPaintBg = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            foreach (RibbonItem item in _panel.Items)
            {
                item.SetCanvas(null);
            }
            
            _panel.Owner.UpdateRegions();
            _panel.Owner.Refresh();
            _panel.PopUp = null;
            _panel.Owner.ResumeSensor();
        } 

        #endregion

        #region Shadow

        // Define the CS_DROPSHADOW constant
        private const int CS_DROPSHADOW = 0x00020000;

        // Override the CreateParams property
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }


        #endregion
    }
}