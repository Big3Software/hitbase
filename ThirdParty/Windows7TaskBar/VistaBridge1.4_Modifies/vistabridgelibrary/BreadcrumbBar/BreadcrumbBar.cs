// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// ========================================
    /// .NET Framework 3.0 Custom Control
    /// ========================================
    ///
    /// Follow these steps to use this custom control in a XAML file.
    ///
    /// Step 1a) Add a reference to the Vista Bridge Library project from the
    ///          project where you want to use this control:
    ///          
    ///          Right click on the target project in the Solution Explorer and
    ///          "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 1b) Add this XmlNamespace attribute to the root element of the
    ///          markup file where it is to be used:
    ///
    ///          xmlns:vb="clr-namespace:Microsoft.SDK.Samples.VistaBridge.Library;assembly=VistaBridgeLibrary"
    ///
    ///
    /// Step 2)  Go ahead and use your control in the XAML file. Note that
    ///          Intellisense in the XML editor does not currently work on
    ///          custom controls and its child elements.
    /// 
    /// Example WPF usage:
    ///     <vb:BreadcrumbBar />
    /// </summary>
    public class BreadcrumbBar : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Creates an in
        /// </summary>
        static BreadcrumbBar()
        {
            // This OverrideMetadata call tells the system that this element wants to provide a style that
            // is different than its base class. This style is defined in themes\generic.xaml.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbBar),
                                                     new FrameworkPropertyMetadata(typeof(BreadcrumbBar)));

            // Register WPF command bindings
            CommandManager.RegisterClassCommandBinding(typeof(BreadcrumbBar),
                new CommandBinding(SelectItemCommand,
                    new ExecutedRoutedEventHandler(OnSelectItem),
                    new CanExecuteRoutedEventHandler(OnCanSelectItem)));
        }

        #endregion

        #region Properties

        private ObservableCollection<object> _selectedHierarchy;
        /// <summary>
        /// The hierarchy of objects that are selected. The list is ordered with
        /// parents first, followed by children.
        /// </summary>
        public ObservableCollection<object> SelectedHierarchy
        {
            get
            {
                if (_selectedHierarchy == null)
                {
                    _selectedHierarchy = new ObservableCollection<object>();
                    _selectedHierarchy.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSelectedHierarchyChanged);
                }

                return _selectedHierarchy;
            }
        }

        /// Dependency property key
        private static readonly DependencyPropertyKey SelectedItemPropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedItem",
                typeof(object),
                typeof(BreadcrumbBar),
                new UIPropertyMetadata(new CoerceValueCallback(OnCoerceSelectedItem)));

        /// <summary>
        /// DependencyProperty to use as the backing store for SelectedItem.
        /// This enables animation, styling, binding, and so forth.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;

        /// <summary>
        /// The currently selected item. Also the last item in the SelectedHierarchy list.
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
        }

        /// <summary>
        /// Template to be used to generate icons.
        /// The data object will be the same source used for the header.
        /// </summary>
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }

        /// <summary>
        /// DependencyProperty to use as the backing store for IconTemplate.
        /// This enables animation, styling, binding, and so forth.
        /// </summary>
        public static readonly DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate",
                typeof(DataTemplate),
                typeof(BreadcrumbBar),
                new UIPropertyMetadata(null));

        /// <summary>
        /// DependencyProperty to use as the backing store for IconTemplateSelector.
        /// This enables animation, styling, binding, and so forth.
        /// </summary>
        public static readonly DependencyProperty IconTemplateSelectorProperty =
            DependencyProperty.Register("IconTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(BreadcrumbBar),
                new UIPropertyMetadata(null));

        /// <summary>
        /// Template selector to determine the template to use to generate icons.
        /// </summary>
        public DataTemplateSelector IconTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(IconTemplateSelectorProperty); }
            set { SetValue(IconTemplateSelectorProperty, value); }
        }

        #endregion

        #region Events/Routed Commands
        /// <summary>
        /// Occurs when an item is selected and before that item is added to the
        /// currently selected hierarchy. This gives the user a chance to update
        /// or populate the children of the currently selected item.
        /// </summary>
        public event EventHandler PopulateItem = delegate { };

        /// <summary>
        /// Occurs when an item has been selected
        /// </summary>
        public event EventHandler ItemSelected = delegate { };

        /// <summary>
        /// The command that is invoked to indicate that an item should be
        /// selected. Set the CommandParameter to the container of the item to
        /// be selected. The control will travel up the element tree to the
        /// BreadcrumbBar where it will then select the object hierarchy
        /// leading to the desired item.
        /// </summary>
        public static readonly RoutedCommand SelectItemCommand = new RoutedCommand("SelectItem", typeof(BreadcrumbBar));

        private static object OnCoerceSelectedItem(DependencyObject d, object baseValue)
        {
            BreadcrumbBar bar = d as BreadcrumbBar;

            if ((bar != null) && (bar._selectedHierarchy != null))
            {
                int count = bar._selectedHierarchy.Count;

                if (count > 0)
                {
                    return bar._selectedHierarchy[count - 1];
                }
            }

            return null;
        }

        private void OnSelectedHierarchyChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CoerceValue(SelectedItemProperty);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Return true if the item is (or should be) its own item container
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is HeaderedItemsControl);
        }

        /// <summary>
        /// Create or identify the element used to display the given item.
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HeaderedItemsControl();
        }

        /// <summary>
        /// Occurs when a dependency property has changed
        /// </summary>
        /// <param name="e">Data for property change event</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if ((e.Property == ItemsControl.HasItemsProperty) && ((bool)e.NewValue))
            {
                if ((_selectedHierarchy == null) || (_selectedHierarchy.Count == 0))
                {
                    //  Selects the root item initially
                    SelectItem(Items[0]);
                }
            }
        }

        // This method is called when an item is selected
        private static void OnSelectItem(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            BreadcrumbBar bar = (BreadcrumbBar)sender;
            DependencyObject container = e.Parameter as DependencyObject;

            if (container != null)
            {
                ItemsControl parentContainer = ItemsControl.ItemsControlFromItemContainer(container);
                if (parentContainer != null)
                {
                    object item = parentContainer.ItemContainerGenerator.ItemFromContainer(container);
                    if (item != null)
                    {
                        bar.SelectItem(item, container, parentContainer);
                    }
                }
            }
        }

        // This method is called to determine if an item can be selected
        private static void OnCanSelectItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.ContinueRouting = false;
            e.Handled = true;
        }

        // Selects the specified item as current. This method removes items that are no longer
        // selected and then adds the current item if needed
        private void SelectItem(object newItem, DependencyObject itemContainer, ItemsControl parentContainer)
        {
            List<object> newItems = new List<object>();
            while ((parentContainer != null) && (parentContainer != this) && (parentContainer.TemplatedParent != this))
            {
                newItems.Add(parentContainer.ItemContainerGenerator.ItemFromContainer(itemContainer));

                itemContainer = parentContainer;
                parentContainer = ItemsControl.ItemsControlFromItemContainer(itemContainer);
            }

            if ((parentContainer != null) && (itemContainer != null))
            {
                object item = parentContainer.ItemContainerGenerator.ItemFromContainer(itemContainer);
                if (item != null)
                {
                    Debug.Assert(_selectedHierarchy != null, "_selectedHierarchy should be non-null");

                    int index = _selectedHierarchy.IndexOf(item);
                    if (index >= 0)
                    {
                        // Remove items that are no longer selected
                        int count = _selectedHierarchy.Count;
                        for (int i = count - 1; i > index; i--)
                        {
                            _selectedHierarchy.RemoveAt(i);
                        }

                        if (item != newItem)
                        {
                            // If the item is a child, then select it.
                            // Otherwise, it was a parent, and doesn't need to be re-selected.
                            SelectItems(newItems);
                        }
                    }
                    else
                    {
                        // This is a new selection
                        _selectedHierarchy.Clear();
                        SelectItems(newItems);
                    }

                    // This is to ensure that menus are closed
                    if (IsKeyboardFocusWithin)
                    {
                        Keyboard.Focus(this);
                    }
                }
            }
        }

        // Adds the current items to the current hierarchy
        private void SelectItems(List<object> newItems)
        {
            int count = newItems.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                SelectItem(newItems[i]);
            }
        }

        // Add the selected item to the current heirarchy after populating it's children
        private void SelectItem(object item)
        {
            PopulateItem(item, EventArgs.Empty);

            SelectedHierarchy.Add(item);

            ItemSelected(item, EventArgs.Empty);
        }
        #endregion
    }
}