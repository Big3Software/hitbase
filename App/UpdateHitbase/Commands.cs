using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace UpdateHitbase
{
    public class Commands
    {
        public static RoutedCommand NextPage = new RoutedCommand("NextPage", typeof(Commands));

        public static RoutedCommand HideNavigationButtons = new RoutedCommand("HideNavigationButtons", typeof(Commands));

        public static RoutedCommand ShowCloseButton = new RoutedCommand("ShowCloseButton", typeof(Commands));

    }
}
