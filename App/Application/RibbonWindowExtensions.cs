using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Data;
using System.Xml;
using System.IO;
using System.Windows.Markup;
using Big3.Hitbase.Configuration;
using System.Xml.Serialization;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Ribbon;

namespace Big3.Hitbase.Miscellaneous
{
    public class RibbonWindowExtensions
    {
        public static void LoadState(RibbonWindow ribbonWindow, Ribbon ribbon)
        {
            ribbon.IsMinimized = Settings.Current.RibbonIsMinimized;
            ribbon.ShowQuickAccessToolBarOnTop = Settings.Current.ShowQuickAccessToolBarOnTop;
            #region Load the QuickAccessToolbarButtonCollection
            try
            {
                if (!string.IsNullOrEmpty(Settings.Current.RibbonQuickAccessToolBar))
                {
                    QuickAccessToolbarButtonCollection buttons = null;

                    XmlSerializer bf = new XmlSerializer(typeof(QuickAccessToolbarButtonCollection));
                    using (StringReader sr = new StringReader(Settings.Current.RibbonQuickAccessToolBar))
                    {
                        buttons = (QuickAccessToolbarButtonCollection)bf.Deserialize(sr);
                    }

                    if (buttons != null)
                    {
                        foreach (QuickAccessToolbarButton qaButton in buttons)
                        {
                            RibbonButton ribbonElem = ribbonWindow.FindName(qaButton.Name) as RibbonButton;

                            RibbonButton rButton = new RibbonButton()
                            {
                                Label = qaButton.Label,
                                KeyTip = qaButton.KeyTip,
                                //ToolTip = qaButton.ToolTip,
                                ToolTipDescription = qaButton.ToolTipDescription,
                                QuickAccessToolBarId = qaButton.QuickAccessToolBarId,
                                Name = qaButton.Name
                            };

                            if (!string.IsNullOrEmpty(qaButton.LargeImageSource))
                                rButton.LargeImageSource = new BitmapImage(new Uri(qaButton.LargeImageSource));
                            if (!string.IsNullOrEmpty(qaButton.SmallImageSource))
                                rButton.SmallImageSource = new BitmapImage(new Uri(qaButton.SmallImageSource));

                            if (ribbonElem != null && ribbonElem.Command != null)
                                rButton.Command = ribbonElem.Command;
                            else
                            {
                                // Noch im Applicationmenü suchen
                                RibbonApplicationMenuItem ribbonMenuItem = ribbon.ApplicationMenu.FindName(qaButton.Name) as RibbonApplicationMenuItem;

                                if (ribbonMenuItem != null)
                                    rButton.Command = ribbonMenuItem.Command;
                            }
                            ribbon.QuickAccessToolBar.Items.Add(rButton);
                        }
                    }
                }
            }
            catch
            {
                // Ignorieren, falls die Toolbar nicht geladen werden kann.
            }
            #endregion
        }

        public static void SaveState(RibbonWindow ribbonWindow, Ribbon ribbon)
        {
            #region Create the QuickAccessToolbarButtonCollection
            QuickAccessToolbarButtonCollection buttons = new QuickAccessToolbarButtonCollection();
            foreach (RibbonButton rButton in ribbon.QuickAccessToolBar.Items)
            {
                QuickAccessToolbarButton qaButton = new QuickAccessToolbarButton()
                {
                    Label = rButton.Label,
                    KeyTip = rButton.KeyTip,
                    //ToolTip = rButton.ToolTip,
                    ToolTipDescription = rButton.ToolTipDescription,
                    //QuickAccessToolBarId = rButton.QuickAccessToolBarId,
                    Name = rButton.Name,
                    //CommandBinding = BindingOperations.GetBinding(rButton, RibbonButton.CommandProperty)
                };

                if (string.IsNullOrEmpty(qaButton.Name) && rButton.QuickAccessToolBarId is string)
                {
                    qaButton.Name = rButton.QuickAccessToolBarId as string;
                }

                if (rButton.LargeImageSource != null)
                    qaButton.LargeImageSource = rButton.LargeImageSource.ToString();
                if (rButton.SmallImageSource != null)
                    qaButton.SmallImageSource = rButton.SmallImageSource.ToString();

                buttons.Add(qaButton);
            }
            #endregion

            Settings.Current.RibbonIsMinimized = ribbon.IsMinimized;
            Settings.Current.ShowQuickAccessToolBarOnTop = ribbon.ShowQuickAccessToolBarOnTop;

            XmlSerializer bf = new XmlSerializer(typeof(QuickAccessToolbarButtonCollection));
            using (StringWriter sw = new StringWriter())
            {
                bf.Serialize(sw, buttons);

                Settings.Current.RibbonQuickAccessToolBar = sw.ToString();
            }
        }
    }

    [Serializable]
    public class QuickAccessToolbarButtonCollection : List<QuickAccessToolbarButton>
    {
    }

    [Serializable]
    public class QuickAccessToolbarButton
    {
        public string LargeImageSource { get; set; }

        public string SmallImageSource { get; set; }

        public string Label { get; set; }

        public object QuickAccessToolBarId { get; set; }

        //public object ToolTip { get; set; }

        public string ToolTipDescription { get; set; }

        public string KeyTip { get; set; }

        public string Name { get; set; }
        
        //public Binding CommandBinding { get; set; }
    }



}
