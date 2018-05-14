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

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides sensitive functionality to detect buttons and panels
    /// </summary>
    public class RibbonSensor
        : IDisposable
    {
        #region Subclasses

        private enum MessageType
        {
            MouseMove,

            MouseUp,

            MouseDown,

            Click
        }

        #endregion

        #region Fields

        private Ribbon _ribbon;
        private Control _control;
        private RibbonTab _tab;
        private bool _dispose;
        private bool _suspended;
        private RibbonPanel _lastSelectedPanel;
        private RibbonPanel _mouseDownPanel;
        private RibbonItem _lastSelectedItem;
        private RibbonItem _lastSelectedSubItem;
        private RibbonItem _lastSelectedSubItemParent;
        private RibbonItem _mouseDownItem;
        private RibbonPanel _limitPanel;
        private bool _senseOnlyItems;
        private IEnumerable<RibbonItem> _itemsToSense;
        #endregion

        #region Ctor

        public RibbonSensor(Control control, Ribbon ribbon, RibbonTab tabToSense)
        {
            _ribbon = ribbon;
            _control = control;
            _tab = tabToSense;

            _control.MouseMove += new MouseEventHandler(_control_MouseMove);
            _control.MouseDown += new MouseEventHandler(_control_MouseDown);
            _control.MouseUp += new MouseEventHandler(_control_MouseUp);
            _control.MouseLeave += new EventHandler(_control_MouseLeave);
        }

        public RibbonSensor(Control control, Ribbon ribbon, IEnumerable<RibbonItem> itemsToSense)
        {
            _ribbon = ribbon;
            _control = control;
            _itemsToSense = itemsToSense;
            SenseOnlyItems = true;

            _control.MouseMove += new MouseEventHandler(_control_MouseMove);
            _control.MouseDown += new MouseEventHandler(_control_MouseDown);
            _control.MouseUp += new MouseEventHandler(_control_MouseUp);
            _control.MouseLeave += new EventHandler(_control_MouseLeave);
        }

        #endregion

        #region Props

        /// <summary>
        /// Gets or sets if only items should be sensed
        /// </summary>
        public bool SenseOnlyItems
        {
            get { return _senseOnlyItems; }
            set { _senseOnlyItems = value; }
        }

        /// <summary>
        /// Gets the items to sense if <see cref="Tab"/> is null
        /// </summary>
        public IEnumerable<RibbonItem> ItemsToSense
        {
            get { return _itemsToSense; }
        }

        /// <summary>
        /// Gets if the sensor has been disposed
        /// </summary>
        public bool Disposed
        {
            get { return _dispose; }
        }

        /// <summary>
        /// Gets or sets the panel that the sensor will be limited to (if any)
        /// </summary>
        public RibbonPanel PanelLimit
        {
            get { return _limitPanel; }
            set { _limitPanel = value; }
        }

        /// <summary>
        /// Gets or sets the tab that this sensor responds to
        /// </summary>
        public RibbonTab Tab
        {
            get { return _tab; }
            set { _tab = value; }
        }

        /// <summary>
        /// Gets the control that this sensor responds to
        /// </summary>
        public Control Control
        {
            get { return _control; }
            set { _control = value; }
        }

        /// <summary>
        /// Gets the Ribbon that this sensor uses.
        /// </summary>
        public Ribbon Ribbon
        {
            get { return _ribbon; }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Suspends the listening for events
        /// </summary>
        public void Suspend()
        {
            _suspended = true;
        }

        /// <summary>
        /// Resumes the listening for events after <see cref="Suspend"/> has been called
        /// </summary>
        public void Resume()
        {
            _suspended = false;
        }

        /// <summary>
        /// Gets the panels to sense by this object
        /// </summary>
        /// <returns></returns>
        private IEnumerable<RibbonPanel> GetPanelsToSense()
        {
            if (SenseOnlyItems)
            {
                return new RibbonPanel[] { };
            }

            if (PanelLimit != null)
            {
                return new RibbonPanel[] { PanelLimit };
            }
            else
            {
                return Tab.Panels;
            }
        }

        /// <summary>
        /// Gets the items that should be sensed according to sense settings
        /// </summary>
        /// <returns></returns>
        private IEnumerable<RibbonItem> GetItemsToSense()
        {
            return ItemsToSense;
        }

        /// <summary>
        /// Redraws the specified panel
        /// </summary>
        /// <param name="panel"></param>
        private void RedrawPanel(RibbonPanel panel)
        {
            //RibbonElementSizeMode size = panel.SizeMode;

            //if (panel.PopUp != null)
            //{
            //    size = RibbonElementSizeMode.Large;
            //}

            //using (Graphics g = Control.CreateGraphics())
            //{
            //    g.SetClip(panel.Bounds);
            //    panel.OnPaint(this, new RibbonElementPaintEventArgs(panel.Bounds, g, size, Control));
            //}

            Control.Invalidate(panel.Bounds);
        }

        /// <summary>
        /// Sets the currently selected panel
        /// </summary>
        /// <param name="panel"></param>
        private void SetSelectedPanel(RibbonPanel panel, int mouseX, int mouseY)
        {
            if (panel == _lastSelectedPanel) return;

            if (_lastSelectedPanel != null)
            {
                _lastSelectedPanel.SetSelected(false, mouseX, mouseY );
                RedrawPanel(_lastSelectedPanel);
            }

            if (panel != null)
            {
                panel.SetSelected(true, mouseX, mouseY);
                RedrawPanel(panel);
            }

            _lastSelectedPanel = panel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="clipIntersect"></param>
        private void RedrawItem(RibbonItem item)
        {
            RedrawItem(item, Rectangle.Empty);
        }

        /// <summary>
        /// Redraws the specified item, intersected by the specified rectangle
        /// </summary>
        /// <param name="item"></param>
        /// <param name="clipIntersect"></param>
        private void RedrawItem(RibbonItem item, Rectangle clipIntersect)
        {
            
            Rectangle clip = Rectangle.FromLTRB(
                item.Bounds.Left,
                item.Bounds.Top, 
                item.Bounds.Right + 1,
                item.Bounds.Bottom + 1);

            if (clipIntersect.IsEmpty)
            {
                clip = (item.Bounds);
            }
            else
            {
                clip = (Rectangle.Intersect(item.Bounds, clipIntersect));
            }

            Control.Invalidate(clip);
            

        }

        /// <summary>
        /// Selects the specified item as the currently selected item
        /// </summary>
        /// <param name="item"></param>
        private void SetSelectedItem(RibbonItem item)
        {
            if (item == _lastSelectedItem) return;

            if (_lastSelectedItem != null)
            {
                _lastSelectedItem.SetSelected(false);
                RedrawItem(_lastSelectedItem);
            }

            if (item != null)
            {
                item.SetSelected(true);
                RedrawItem(item);
            }

            _lastSelectedItem = item;
        }

        /// <summary>
        /// Selects the specified item as the currently selected sub-item
        /// </summary>
        /// <param name="item"></param>
        private void SetSelectedSubItem(RibbonItem item)
        {
            if (item == _lastSelectedSubItem) return;

            if (_lastSelectedSubItem != null)
            {
                _lastSelectedSubItem.SetSelected(false);
                RedrawItem(_lastSelectedSubItem, GetContentBounds(_lastSelectedSubItemParent));
            }

            if (item != null)
            {
                item.SetSelected(true);
                RedrawItem(item, GetContentBounds(_lastSelectedSubItemParent));
            }

            _lastSelectedSubItem = item;
        }

        /// <summary>
        /// Tries to extract from IContainsRibbonItems.GetContentBounds()
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Rectangle GetContentBounds(RibbonItem item)
        {
            IContainsSelectableRibbonItems cont = item as IContainsSelectableRibbonItems;

            if (cont == null)
            {
                return Rectangle.Empty;
            }
            else
            {
                return cont.GetContentBounds();
            }
        }

        /// <summary>
        /// Handles the MouseUp event on the _control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _control_MouseUp(object sender, MouseEventArgs e)
        {
            if(!Disposed && !_suspended)
                SenseMouseUp(e);
        }

        /// <summary>
        /// Handles the MouseDown event on the _control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _control_MouseDown(object sender, MouseEventArgs e)
        {
            if(!Disposed && !_suspended)
                SenseMouseDown(e);
        }

        /// <summary>
        /// Handles the MouseMove of the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _control_MouseMove(object sender, MouseEventArgs e)
        {
            if(!Disposed && !_suspended)
                SenseMouseMove(e);
        }

        /// <summary>
        /// Handles the MouseLeave of control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _control_MouseLeave(object sender, EventArgs e)
        {
            if (!Disposed && !_suspended)
                SenseMouseMove(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
        }

        /// <summary>
        /// Checks hit on the tab's left-scroll button
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private bool TabScrollLeftHit(int x, int y)
        {
            if (Tab.ScrollLeftVisible)
            {
                return Tab.ScrollLeftBounds.Contains(x, y);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks hit on the tab's right-scroll button
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private bool TabScrollRightHit(int x, int y)
        {
            if (Tab.ScrollRightVisible)
            {
                return Tab.ScrollRightBounds.Contains(x, y);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the mouse hitted a scroll button of the tab (if present)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool MouseMoveTabScroll(int x, int y)
        {
            bool redLeft = false;
            bool redRight = false;
            bool res = false;

            if (!(Control is Ribbon))
            {
                return false;
            }

            if (TabScrollLeftHit(x, y))
            {
                Tab.SetScrollLeftSelected(true);
                redLeft = true;
                res = true;
            }
            else
            {
                if (Tab.ScrollLeftSelected)
                {
                    Tab.SetScrollLeftSelected(false);
                    redLeft = true;
                }
            }

            if (TabScrollRightHit(x, y))
            {
                Tab.SetScrollRightSelected(true);
                redRight = true;
                res = true;
            }
            else
            {
                if (Tab.ScrollRightSelected)
                {
                    Tab.SetScrollRightSelected(false);
                    redRight = true;
                }
            }

            if (redLeft)
            {
                Tab.Owner.Invalidate(Tab.ScrollLeftBounds);
            }

            if (redRight)
            {
                Tab.Owner.Invalidate(Tab.ScrollRightBounds);
            }

            return res;
        }

        /// <summary>
        /// Returns true if the mouse hitted a scroll button of the tab (if present)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool MouseDownTabScroll(int x, int y)
        {
            if (TabScrollLeftHit(x, y))
            {
                Tab.SetScrollLeftPressed(true);
                Tab.ScrollLeft();
                return true;
            }

            if (TabScrollRightHit(x, y))
            {
                Tab.SetScrollRightPressed(true);
                Tab.ScrollRight();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the mouse hitted a scroll button of the tab (if present)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool MouseUpTabScroll(int x, int y)
        {
            if (TabScrollLeftHit(x, y))
            {
                Tab.SetScrollLeftPressed(false);
                return true;
            }

            if (TabScrollRightHit(x, y))
            {
                Tab.SetScrollRightPressed(false);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Senses the movement of the mouse
        /// </summary>
        /// <param name="e"></param>
        public void SenseMouseMove(MouseEventArgs e)
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().Name);

            RibbonPanel hittedPanel = null;
            RibbonItem hittedItem = null;
            RibbonItem hittedSubItem = null;
            bool subItemChanged = false;

            if (!SenseOnlyItems)
            {
                if (MouseMoveTabScroll(e.X, e.Y)) return;

                ///Check for the selected panel
                if (Tab.TabContentBounds.Contains(e.X, e.Y) || PanelLimit != null)
                {
                    foreach (RibbonPanel panel in GetPanelsToSense())
                    {
                        if (panel.Bounds.Contains(e.X, e.Y))
                        {
                            SetSelectedPanel(panel, e.X, e.Y);
                            if (panel.PopUp != null)
                            {
                                hittedPanel = panel;
                            }
                            hittedPanel = panel;
                            break;
                        }
                    }
                }

                if (hittedPanel != null)
                    hittedPanel.OnMouseMove(e);

                ///If no panel, unselect last panel
                if (hittedPanel == null && _lastSelectedPanel != null)
                {
                    SetSelectedPanel(null, e.X, e.Y);
                    SetSelectedItem(null);
                    SetSelectedSubItem(null);
                } 
            }

            ///Check for the selected item
            if ((hittedPanel != null && (hittedPanel.SizeMode != RibbonElementSizeMode.Overflow || hittedPanel.PopUp != null))
                 || SenseOnlyItems)
            {
                IEnumerable<RibbonItem> items = SenseOnlyItems ? ItemsToSense : hittedPanel.Items;

                foreach (RibbonItem item in items)
                {
                    if (item.Bounds.Contains(e.X, e.Y))
                    {
                        if (item != _lastSelectedSubItemParent) SetSelectedSubItem(null);
                        SetSelectedItem(item);
                        hittedItem = item;
                        break;
                    }
                }
            }

            ///Check for sub-items on the item
            ///(This should be recursive, but it's not necessary so far)
            if (hittedItem != null && hittedItem is IContainsSelectableRibbonItems)
            {
                foreach (RibbonItem item in (hittedItem as IContainsSelectableRibbonItems).GetItems())
                {
                    if (item.Bounds.Contains(e.X, e.Y))
                    {
                        _lastSelectedSubItemParent = hittedItem;
                        subItemChanged = item != _lastSelectedSubItem;
                        SetSelectedSubItem(item);
                        hittedSubItem = item;
                        break;
                    }
                }
            }

            ///if no hitted item, unselect last item
            if (hittedItem == null && _lastSelectedItem != null)
            {
                SetSelectedItem(null);
                SetSelectedSubItem(null);
            }

            if (hittedPanel != null)
            {
                hittedPanel.OnMouseMove(e);
            }

            if (hittedItem != null) 
               hittedItem.OnMouseMove(e);
            

            ///If no hitted sub-item, unselect last item
            if ((hittedSubItem == null && _lastSelectedSubItem != null))
            {
                SetSelectedSubItem(null);
            }
            
            if(hittedSubItem != null)
                hittedSubItem.OnMouseMove(e);
            
        }
        
        /// <summary>
        /// Senses the mouse button donwn
        /// </summary>
        /// <param name="e"></param>
        public void SenseMouseDown(MouseEventArgs e)
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().Name);

            RibbonPanel hittedPanel = null;
            RibbonItem hittedItem = null;

            if (!SenseOnlyItems)
            {
                if (MouseDownTabScroll(e.X, e.Y)) return;

                ///Check for the selected panel
                if (Tab.TabContentBounds.Contains(e.X, e.Y) || PanelLimit != null)
                {
                    foreach (RibbonPanel panel in GetPanelsToSense())
                    {
                        if (panel.Bounds.Contains(e.X, e.Y))
                        {
                            hittedPanel = panel;
                            if (PanelLimit == null || PanelLimit == hittedPanel)
                            {
                                _mouseDownPanel = panel;
                                panel.OnMouseDown(e);
                                RedrawPanel(panel);
                            }
                            else
                                _mouseDownPanel = null;
                            break;
                        }
                    }
                } 
            }

            if (hittedPanel != null && hittedPanel.Collapsed && Control is Ribbon)
            {
                return;
            }

            ///Check for the selected item
            if ((hittedPanel != null) || SenseOnlyItems)
            {
                IEnumerable<RibbonItem> items = SenseOnlyItems ? ItemsToSense : hittedPanel.Items;

                foreach (RibbonItem item in items)
                {
                    if (item.Bounds.Contains(e.X, e.Y))
                    {
                        item.OnMouseDown(e);
                        hittedItem = item;
                        _mouseDownItem = item;
                        RedrawItem(hittedItem);
                        break;
                    }
                }
            }

            ///Check for sub-items on the item
            ///(This should be recursive, but it's not necessary so far)
            if (hittedItem != null && hittedItem is IContainsSelectableRibbonItems)
            {

                foreach (RibbonItem item in (hittedItem as IContainsSelectableRibbonItems).GetItems())
                {
                    if (item.Bounds.Contains(e.X, e.Y))
                    {
                        item.OnMouseDown(e);
                        _mouseDownItem = item;
                        RedrawItem(item, GetContentBounds(hittedItem));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Senses the mouse button up
        /// </summary>
        /// <param name="e"></param>
        public void SenseMouseUp(MouseEventArgs e)
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().Name);
            
            RibbonPanel mouseUpPanel = null;
            RibbonItem mouseUpItem = null;
            RibbonPanel hittedPanel = null;
            RibbonItem hittedItem = null;

            if (!SenseOnlyItems)
            {
                if (MouseUpTabScroll(e.X, e.Y)) return;

                ///Check for the selected panel
                if (Tab.TabContentBounds.Contains(e.X, e.Y) || PanelLimit != null)
                {
                    foreach (RibbonPanel panel in GetPanelsToSense())
                    {
                        if (panel.Bounds.Contains(e.X, e.Y))
                        {
                            hittedPanel = panel;
                            if (PanelLimit == null || PanelLimit == hittedPanel)
                            {
                                mouseUpPanel = panel;
                                panel.OnMouseUp(e);
                            }
                            break;
                        }
                    }
                }

                if (mouseUpPanel != null && mouseUpPanel == _mouseDownPanel)
                {
                    _mouseDownPanel = null;
                    mouseUpPanel.OnClick(EventArgs.Empty);
                }

                if (hittedPanel != null)
                {
                    hittedPanel.OnMouseUp(e);
                }
            }

            ///Check for the selected item
            if (hittedPanel != null || SenseOnlyItems)
            {
                IEnumerable<RibbonItem> items = SenseOnlyItems ? ItemsToSense : hittedPanel.Items;

                foreach (RibbonItem item in items)
                {
                    if (item.Bounds.Contains(e.X, e.Y))
                    {
                        item.OnMouseUp(e);
                        hittedItem = item;
                        mouseUpItem = item;
                        RedrawItem(item);
                        break;
                    }
                }
            }

            ///Check for sub-items on the item
            ///(This should be recursive, but it's not necessary so far)
            if (hittedItem != null && hittedItem is IContainsSelectableRibbonItems)
            {
                foreach (RibbonItem item in (hittedItem as IContainsSelectableRibbonItems).GetItems())
                {
                    if (item.Bounds.Contains(e.X, e.Y))
                    {
                        item.OnMouseUp(e);
                        mouseUpItem = item;
                        RedrawItem(item, GetContentBounds(hittedItem));
                        break;
                    }
                }
            }

            if (mouseUpItem == _mouseDownItem && mouseUpItem != null)
            {
                _mouseDownItem = null;
                mouseUpItem.OnClick(EventArgs.Empty);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _dispose = true;

            _control.MouseMove -= _control_MouseMove;
            _control.MouseUp -= _control_MouseUp;
            _control.MouseDown -= _control_MouseDown;
            _control.MouseLeave -= _control_MouseLeave;
        }

        #endregion
    }
}
