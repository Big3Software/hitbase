using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateHitbase
{
    public interface IWizardPage
    {
        bool NextButtonDisabled { get; }
    }
}
