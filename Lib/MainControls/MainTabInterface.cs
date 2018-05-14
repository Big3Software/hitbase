using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Big3.Hitbase.MainControls
{
    public interface IMainTabInterface
    {
        bool Closing();
        void Restore(RegistryKey regKey);
        void Save(RegistryKey regKey);
    }
}
