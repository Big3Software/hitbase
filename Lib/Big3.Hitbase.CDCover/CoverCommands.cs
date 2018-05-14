using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Big3.Hitbase.CDCover
{
    public static class CoverCommands
    {
        public static RoutedCommand FrontCoverChooseBackgroundNone = new RoutedCommand("FrontCoverChooseBackgroundNone", typeof(CoverCommands));
        public static RoutedCommand FrontCoverChooseBackgroundColor = new RoutedCommand("FrontCoverChooseBackgroundColor", typeof(CoverCommands));
        public static RoutedCommand FrontCoverChooseBackgroundImage = new RoutedCommand("FrontCoverChooseBackgroundImage", typeof(CoverCommands));
        public static RoutedCommand FrontCoverChooseBackgroundFromCD = new RoutedCommand("FrontCoverChooseBackgroundFromCD", typeof(CoverCommands));

        public static RoutedCommand BackCoverChooseBackgroundNone = new RoutedCommand("BackCoverChooseBackgroundNone", typeof(CoverCommands));
        public static RoutedCommand BackCoverChooseBackgroundColor = new RoutedCommand("BackCoverChooseBackgroundColor", typeof(CoverCommands));
        public static RoutedCommand BackCoverChooseBackgroundImage = new RoutedCommand("BackCoverChooseBackgroundImage", typeof(CoverCommands));
        public static RoutedCommand BackCoverChooseBackgroundFromCD = new RoutedCommand("BackCoverChooseBackgroundFromCD", typeof(CoverCommands));

        public static RoutedCommand Bold = new RoutedCommand("Bold", typeof(CoverCommands));
        public static RoutedCommand Italic = new RoutedCommand("Italic", typeof(CoverCommands));
        public static RoutedCommand Underline = new RoutedCommand("Underline", typeof(CoverCommands));

        public static RoutedCommand Close = new RoutedCommand("Close", typeof(CoverCommands));
        
    }
}
