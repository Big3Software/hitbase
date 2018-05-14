// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Text;
//using System.Windows.Forms;
using Microsoft.SDK.Samples.VistaBridge.Library;


namespace Microsoft.SDK.Samples.VistaBridge
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
        private TaskDialog taskDialog;
        private CommonOpenFileDialog openFileDialog;

        bool initializedComplexDialog;
        private Guid dialog1Guid = new Guid("00000000000000000000000000000001");
        private Guid dialog2Guid = new Guid("00000000000000000000000000000002");
        private bool useFirstGuid = true;

        // used for consistancy in messages
        private const string MenuItemSelectedTemplate = 
            "Menu Option selected by:{0}sender: {1}";

        public Window1()
        {
            InitializeComponent();

            // want a debug assert to pop a dialog.
            // Remove the original default trace listener.
            Trace.Listeners.RemoveAt(0);

            // Create and add a new default trace listener.
            DefaultTraceListener defaultListener;
            defaultListener = new DefaultTraceListener();
            Trace.Listeners.Add(defaultListener);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

        }


        #region MessageBox/TaskDialog Handlers and Helpers

        private void WFMessageBoxClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Hello from Windows Forms!", "Hello World!");
        }

        private void WPFMessageBoxClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Hello from WPF!", "Hello world!");
        }

        private void HelloWorldTDClicked(object sender, RoutedEventArgs e)
        {
            TaskDialog.Show("Hello from Vista!", "Hello World!", "Hello World!");
        }

        private void SimpleWaitTDClicked(object sender, RoutedEventArgs e)
        {
            taskDialog = FindTaskDialog("simpleWaitTD");
            taskDialog.Show();
        }

        /// <summary>
        /// "Open File Dialog Customization 2" button click event handler.
        /// </summary>
        /// 
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        /// 
        private void OpenFileDialogCustomizationXamlClicked(object sender, RoutedEventArgs e)
        {
            openFileDialog = FindOpenFileDialog("simpleOFD");

            // Name access to Xaml resource control work-around.
            
            /*
             * Resources can also be referenced by code from within the 
             * collection, but be aware that resources created in XAML will 
             * definitely not be accessible until after Loaded is raised by 
             * the element that declares the dictionary. In fact, resources 
             * are parsed asynchronously and not even the Loaded event is an 
             * assurance that you can reference a XAML defined resource. For 
             * this reason you should generally only access XAML defined 
             * resources as part of run-time code, or through other XAML 
             * techniques such as styles or resource extension references for 
             * attribute values.
             */

            int forIdx = 0;
            int controlIndex = -1;
            int numberOfControls = openFileDialog.Controls.Count;
            CommonFileDialogControl cfdc = null;

            // find the control you want to initialize
            for (forIdx = 0; forIdx < numberOfControls; forIdx++)
            {
                cfdc = openFileDialog.Controls[forIdx];

                // un-comment the .Equals test if looking for a specific
                // control.
                if (cfdc is CommonFileDialogTextBox /*&& 
                    cfdc.Name.Equals("textName", StringComparison.InvariantCulture)*/)
                {
                    // save the index of the CommonFileDialogTextBox (or 
                    // otherwise perform what ever other operation is deemed
                    // necessary / useful here).
                    controlIndex = forIdx;
                    break;
                }
            }

            // init the control's text
            cfdc.Text = "enter text here.";

            CommonFileDialogResult result = openFileDialog.ShowDialog();

            if (!result.Canceled)
            {
                MessageBox.Show("File Selected: " + openFileDialog.FileName,
                    "Open File Dialog Customization 2 Result" );
            }

        }

        private void ConfirmationTDClicked(object sender, RoutedEventArgs e)
        {
            taskDialog = FindTaskDialog("confirmationTD");
            taskDialog.Show();
        }

        private void ComplexTDClicked(object sender, RoutedEventArgs e)
        {
            taskDialog = FindTaskDialog("complexTD");

            if (initializedComplexDialog == false)
            {
                taskDialog.ExpandedText += " Link: <A HREF=\"Http://www.microsoft.com\">Microsoft</A>";
                initializedComplexDialog = true;
            }

            taskDialog.Show();
        }

        private void ComplexTD2Clicked(object sender, RoutedEventArgs e)
        {
            taskDialog = FindTaskDialog("complexTD2");
            taskDialog.Show();
        }

        private TaskDialog FindTaskDialog(string name)
        {
            return (TaskDialog)FindResource(name);
        }

        private CommonOpenFileDialog FindOpenFileDialog(string name)
        {
            return (CommonOpenFileDialog)FindResource(name);
        }

        private CommonFileDialogControl FindCommonFileDialogControl(string name)
        {
            return (CommonFileDialogControl)FindResource(name);
        }

        private void OnTick(object sender, TaskDialogTickEventArgs e)
        {
            taskDialog.Content = "Seconds elapsed:   " + e.Ticks / 1000;
            // Update the progress bar if there is one.
            if (taskDialog.ProgressBar != null)
            {
                if (taskDialog.ProgressBar.Value == taskDialog.ProgressBar.Maximum)
                {
                    taskDialog.ProgressBar.Value = taskDialog.ProgressBar.Minimum;
                }
                taskDialog.ProgressBar.Value = taskDialog.ProgressBar.Value + 10;
            }
        }

        private void OnDialogClosing(object sender, TaskDialogClosingEventArgs e)
        {
            // Can check e.CustomButton and e.StandardButton properties
            // to determine whether or not to cancel this Close event.
        }

        private void OnHyperlinkClick(object sender, TaskDialogHyperlinkClickedEventArgs e)
        {
            MessageBox.Show("Link clicked: " + e.LinkText);
        }


        // Executes if the user presses F1 when the dialog is showing.
        private void OnHelpInvoked(object sender, EventArgs e)
        {
            // Launch Windows Help and Support.
            string path = Environment.GetEnvironmentVariable("windir")
                + @"\helppane.exe";
            ProcessStartInfo info = new ProcessStartInfo(path, "-embedding");
            Process.Start(info);

        }

        #endregion

        #region File Dialog Handlers and Helpers

        private void WPFDialogClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialogOpenFile = new Microsoft.Win32.OpenFileDialog();
            dialogOpenFile.ShowDialog();
        }

        private void SaveVistaFileDialogClicked(object sender, RoutedEventArgs e)
        {
            CommonSaveFileDialog saveDialog =
                new CommonSaveFileDialog("My Save File Dialog");

            saveDialog.Filters.Add(new CommonFileDialogFilter("My Test Docs", "tdc,tdcx"));
            saveDialog.Filters.Add(new CommonFileDialogFilter("Word Docs", "doc,docx"));

            saveDialog.DefaultExtension = "doc";
            saveDialog.AddExtension = true;

            CommonFileDialogResult result = saveDialog.ShowDialog();
            if (!result.Canceled)
                MessageBox.Show("File chosen: " + saveDialog.FileName);
        }

        private void OpenVistaFileDialogClicked(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openDialog = new CommonOpenFileDialog();
            ApplyCommonSettings(openDialog);
            CommonFileDialogResult result = openDialog.ShowDialog();
            if (!result.Canceled)
            {
                StringBuilder output = new StringBuilder("Files selected: ");
                foreach (string file in openDialog.FileNames)
                {
                    output.Append(file);
                    output.Append(Environment.NewLine);
                }
                TaskDialog.Show(output.ToString(), "Files Chosen", "Files Chosen");
            }
        }

        private void OpenFileDialogCustomizationClicked(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openDialog = new CommonOpenFileDialog();
            ApplyCommonSettings(openDialog);
            
            // set the 'allow multi-select' flag
            openDialog.MultiSelect = true;

            AddCustomControls(openDialog);
            CommonFileDialogResult result = openDialog.ShowDialog();
            if (!result.Canceled)
            {
                StringBuilder output = new StringBuilder("Files selected: ");
                foreach (string file in openDialog.FileNames)
                {
                    output.Append(file);
                    output.Append(Environment.NewLine);
                }
                TaskDialog.Show(output.ToString(), "Files Chosen", "Files Chosen");
            }
        }

        private void AddCustomControls(CommonFileDialog openDialog)
        {
            // Add a RadioButtonList
            CommonFileDialogRadioButtonList list = new CommonFileDialogRadioButtonList("Options");
            list.Items.Add(new CommonFileDialogRadioButtonListItem("Option A"));
            list.Items.Add(new CommonFileDialogRadioButtonListItem("Option B"));
            list.SelectedIndexChanged += RBLOptions_SelectedIndexChanged;
            list.SelectedIndex = 1;
            openDialog.Controls.Add(list);

            // Create a groupbox
            CommonFileDialogGroupBox groupBox = new CommonFileDialogGroupBox("Options");

            // Create and add two check boxes to this group
            CommonFileDialogCheckBox checkA = new CommonFileDialogCheckBox("Option A", true);
            CommonFileDialogCheckBox checkB = new CommonFileDialogCheckBox("Option B", true);
            checkA.CheckedChanged += ChkOptionA_CheckedChanged;
            checkB.CheckedChanged += ChkOptionB_CheckedChanged;
            groupBox.Items.Add(checkA);
            groupBox.Items.Add(checkB);

            // Create and add a separator to this group
            groupBox.Items.Add(new CommonFileDialogSeparator());

            // Create and add a button to this group
            CommonFileDialogButton btnCFDPushButton = new CommonFileDialogButton("Push Button");
            btnCFDPushButton.Click += PushButton_Click;
            groupBox.Items.Add(btnCFDPushButton);

            // Add groupbox to dialog
            openDialog.Controls.Add(groupBox);

            // Add a Menu
            CommonFileDialogMenu menu = new CommonFileDialogMenu("Sample Menu");
            CommonFileDialogMenuItem itemA = new CommonFileDialogMenuItem("Menu Item 1");
            CommonFileDialogMenuItem itemB = new CommonFileDialogMenuItem("Menu Item 2");
            itemA.Click += MenuOptionA_Click;
            itemB.Click += MenuOptionA_Click;
            menu.Items.Add(itemA);
            menu.Items.Add(itemB);
            openDialog.Controls.Add(menu);

            // Add a TextBox
            openDialog.Controls.Add(new CommonFileDialogLabel("Enter name"));
            openDialog.Controls.Add(new CommonFileDialogTextBox("textBox", Environment.UserName));

            // Add a ComboBox
            CommonFileDialogComboBox comboBox = new CommonFileDialogComboBox();
            comboBox.SelectedIndexChanged += ComboEncoding_SelectedIndexChanged;
            comboBox.Items.Add(new CommonFileDialogComboBoxItem("Combobox Item 1"));
            comboBox.Items.Add(new CommonFileDialogComboBoxItem("Combobox Item 2"));
            comboBox.SelectedIndex = 1;
            openDialog.Controls.Add(comboBox);
        }

        private void ApplyCommonSettings(CommonFileDialog openDialog)
        {
            openDialog.Title = "My Open File Dialog";
            if (useFirstGuid)
                openDialog.UsageIdentifier = dialog1Guid;
            else
                openDialog.UsageIdentifier = dialog2Guid;
            useFirstGuid = !useFirstGuid;

            // Add custom file filter.
            openDialog.Filters.Add(new CommonFileDialogFilter("My File Types", "john,doe"));

            // Add some standard filters.
            openDialog.Filters.Add(CommonFileDialogStandardFilters.TextFiles);
            openDialog.Filters.Add(CommonFileDialogStandardFilters.OfficeFiles);
        }

        #endregion

        // TODO: fixup names of event handler methods
        // TODO: fixup; improve the information displayed by the messageboxes.

        private void TaskDialogRadioButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(sender.ToString());
        }

        private void RBLOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(sender.ToString());
        }

        private void PushButton_Click(object sender, EventArgs e)
        {
            // This is how we get data from a CommonFileDialogTextBox 
            // (CFDTextBox) while the dialog is still showing. This is the 
            // only time you can retrieve data from a CFDTextBox.

            int forIdx = 0;
            int numberOfControls = openFileDialog.Controls.Count;
            CommonFileDialogControl cfdc = null;
            StringBuilder textBoxes = new StringBuilder( 
                "TextBox control(s) and values:" + Environment.NewLine );
            
            const string itemTemplate = "Name: {0}; Text: \"{1}\".";

            // find the control(s) you want to get data from
            for (forIdx = 0; forIdx < numberOfControls; forIdx++)
            {
                cfdc = openFileDialog.Controls[forIdx];

                if (cfdc is CommonFileDialogTextBox)
                {
                    textBoxes.Append(string.Format(itemTemplate,
                        cfdc.Name, cfdc.Text));
                }
            }

            MessageBox.Show( textBoxes.ToString(), "Data from the CommonFileDialogTextBoxs", 
                MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void ComboEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(sender.ToString());
        }

        private void ChkOptionA_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show(sender.ToString());
        }

        private void ChkOptionB_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show(sender.ToString());
        }

        private void MenuOptionA_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format(MenuItemSelectedTemplate,
                Environment.NewLine, sender.ToString()));
        }

        private void MenuOptionB_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format(MenuItemSelectedTemplate,
                Environment.NewLine, sender.ToString()));
        }

        private void MenuOptionC_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format(MenuItemSelectedTemplate,
                Environment.NewLine, sender.ToString()));
        }

    }
}