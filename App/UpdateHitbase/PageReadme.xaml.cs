using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.IO;

namespace UpdateHitbase
{
    /// <summary>
    /// Interaction logic for PageWelcome.xaml
    /// </summary>
    public partial class PageReadme : UserControl
    {
        public PageReadme()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream reader = assembly.GetManifestResourceStream("UpdateHitbase.readme.rtf");

            richTextBoxReadme.Selection.Load(reader, DataFormats.Rtf);
        }
    }
}
