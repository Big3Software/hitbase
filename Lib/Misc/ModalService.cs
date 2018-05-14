using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace Big3.Hitbase.Miscellaneous
{
    public delegate void BackNavigationEventHandler(bool dialogReturn);

    public interface IModalService
    {
        void NavigateTo(UserControl uc, string title, BackNavigationEventHandler backFromDialog);
        void NavigateTo(UserControl uc, string title, BackNavigationEventHandler backFromDialog, bool allowClose);
        void GoBackward(bool dialogReturnValue);
        void CloseModal();
    }

    public class GlobalServices
    {
        public static IModalService ModalService
        {
            get { return (IModalService)System.Windows.Application.Current.MainWindow; }
        }
    }
}

