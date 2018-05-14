using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Big3.Hitbase.CDCover
{
    public static class CoverElementExtensions
    {
        public static bool GetIsSelectable(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectableProperty);
        }

        public static void SetIsSelectable(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectableProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSelectable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectableProperty =
            DependencyProperty.RegisterAttached("IsSelectable", typeof(bool), typeof(CoverElementExtensions), new UIPropertyMetadata(false));



        public static CoverElement GetCoverElement(DependencyObject obj)
        {
            return (CoverElement)obj.GetValue(CoverElementProperty);
        }

        public static void SetCoverElement(DependencyObject obj, CoverElement value)
        {
            obj.SetValue(CoverElementProperty, value);
        }

        // Using a DependencyProperty as the backing store for CoverElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoverElementProperty =
            DependencyProperty.RegisterAttached("CoverElement", typeof(CoverElement), typeof(CoverElementExtensions), new UIPropertyMetadata(CoverElement.None));


    }

    public enum CoverElement
    {
        None,
        FrontCover,
        FrontCoverTitle1,
        FrontCoverTitle2,
        BackCover,
        BackCoverLeftSide,
        BackCoverRightSide,
        BackCoverTracklist, 
    }
}
