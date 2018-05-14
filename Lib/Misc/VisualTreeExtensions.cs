using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Big3.Hitbase.Miscellaneous
{
    public class VisualTreeExtensions
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            if (!(child is Visual))
                return null;

            DependencyObject parent = VisualTreeHelper.GetParent(child) as DependencyObject;
            if (parent == null) return null;
            if (parent is T) return (parent as T);

            return VisualTreeExtensions.FindParent<T>(parent);
        }

        public static T FindParentByName<T>(DependencyObject child, string name) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child) as DependencyObject;
            if (parent == null) return null;

            FrameworkElement fe = parent as FrameworkElement;
            if (fe != null)
            {
                if (fe is T && fe.Name.CompareTo(name) == 0) 
                    return (parent as T);
            }

            return VisualTreeExtensions.FindParent<T>(parent);
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static T FindVisualChildByName<T>(DependencyObject obj, string name)
            where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                object childObj = VisualTreeHelper.GetChild(obj, i);
                if (childObj is FrameworkElement)
                {
                    FrameworkElement child = (FrameworkElement)childObj;
                    if ((child != null) && (child is T) && (child.Name.CompareTo(name) == 0))
                        return (T)(DependencyObject)child;
                }
                T childOfChild = FindVisualChildByName<T>((DependencyObject)childObj, name);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

    }

    public class LogicalTreeExtensions
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            if (!(child is Visual))
                return null;

            DependencyObject parent = LogicalTreeHelper.GetParent(child) as DependencyObject;
            if (parent == null) return null;
            if (parent is T) return (parent as T);

            return LogicalTreeExtensions.FindParent<T>(parent);
        }
    }
}
