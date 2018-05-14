// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Windows7.DesktopIntegration;
using Windows7.DesktopIntegration.WindowsForms;

namespace Windows7.DesktopIntegration.MainDemo
{
    public partial class Windows7Form : Form
    {
        public Windows7Form()
        {
            InitializeComponent();

            Windows7Taskbar.AllowTaskbarWindowMessagesThroughUIPI();

            Windows7Taskbar.SetCurrentProcessAppId("TaskbarManaged");
            //string appId = Windows7Taskbar.GetCurrentProcessAppId();
        }

        Icon _taskbarOverlayIcon;
        protected Icon TaskbarOverlayIcon
        {
            get
            {
                return _taskbarOverlayIcon;
            }
            set
            {
                _taskbarOverlayIcon = value;
                this.SetTaskbarOverlayIcon(value, "Overlay");
            }
        }

        protected string WindowAppId
        {
            get
            {
                return this.GetAppId();
            }
            set
            {
                this.SetAppId(value);
            }
        }

        private ThumbButtonManager _thumbButtonManager;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Windows7Taskbar.TaskbarButtonCreatedMessage)
            {
                this.SetThumbnailTooltip("Cool window!");

                _jumpListManager = this.CreateJumpListManager();
                _jumpListManager.UserRemovedItems += (o, e) =>
                {
                    statusLabel.Text = "User removed " + e.RemovedItems.Length + " items (cancelling refresh)";
                    e.CancelCurrentOperation = true;
                };
                _jumpListManager.ClearAllDestinations();
                _jumpListManager.EnabledAutoDestinationType = ApplicationDestinationType.Recent;

                TaskbarOverlayIcon = SystemIcons.Hand;

                _thumbButtonManager = this.CreateThumbButtonManager();
                ThumbButton button2 = _thumbButtonManager.CreateThumbButton(102, SystemIcons.Exclamation, "Beware of me!");
                button2.Clicked += delegate
                {
                    statusLabel.Text = "Second button clicked";
                    button2.Enabled = false;
                };
                ThumbButton button = _thumbButtonManager.CreateThumbButton(101, SystemIcons.Information, "Click me");
                button.Clicked += delegate
                {
                    statusLabel.Text = "First button clicked";
                    button2.Enabled = true;
                };
                _thumbButtonManager.AddThumbButtons(button, button2);
            }

            if (_windowsManager != null)
                _windowsManager.DispatchMessage(ref m);

            if (_thumbButtonManager != null)
                _thumbButtonManager.DispatchMessage(ref m);

            base.WndProc(ref m);
        }

        private JumpListManager _jumpListManager;

        private void btnChangeAppId_Click(object sender, EventArgs e)
        {
            this.WindowAppId = "SomethingElse";
        }

        const string progId = "Microsoft.SDK.Samples.SevenBridge.TaskbarDemo";

        private void registerFileExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrationHelper.RegisterFileAssociations(
                progId,
                false,  //TODO: Access denied to HKCU\Software\Classes, check why
                Windows7Taskbar.GetCurrentProcessAppId(),
                Assembly.GetEntryAssembly().Location + " /doc:%1",
                ".txt");

        }

        private void unregisterFileExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrationHelper.UnregisterFileAssociations(
                progId,
                false,  //TODO: Access denied to HKCU\Software\Classes, check why
                Windows7Taskbar.GetCurrentProcessAppId(),
                Assembly.GetEntryAssembly().Location + " /doc:%1",
                ".txt");
        }

        string _thisAppDataPath;

        private void Windows7Form_Load(object sender, EventArgs e)
        {
            //Create test files for the app to run:
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _thisAppDataPath = Path.Combine(localAppDataPath, progId);
            if (!Directory.Exists(_thisAppDataPath))
                Directory.CreateDirectory(_thisAppDataPath);
            File.WriteAllText(Path.Combine(_thisAppDataPath, "mary.txt"),
                "Mary had a little lamb...");
            File.WriteAllText(Path.Combine(_thisAppDataPath, "raven.txt"),
                "Once upon a midnight dreary, while I pondered weak and weary...");
            File.WriteAllText(Path.Combine(_thisAppDataPath, "sonnet.txt"),
                "Shall I compare thee to a summer's day?");
            File.WriteAllText(Path.Combine(_thisAppDataPath, "hello.html"),
                "<h1>Hello World!</h1>");

            Bitmap shieldBitmap = SystemIcons.Shield.ToBitmap();
            registerFileExtensionsToolStripMenuItem.Image = shieldBitmap;
            unregisterFileExtensionsToolStripMenuItem.Image = shieldBitmap;

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a text document to load";
            ofd.Filter = "Text Files (*.txt)|*.txt";
            ofd.ShowDialog();
            //If the user OK'd the dialog, the shell will add the item for us.
        }

        private void chooseOverlayIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Choose an icon file";
            ofd.Filter = "Icon Files (*.ico)|*.ico";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TaskbarOverlayIcon = new Icon(ofd.FileName);
            }
        }

        private Icon _oldOverlayIcon;

        private void btnToggleOverlay_Click(object sender, EventArgs e)
        {
            if (TaskbarOverlayIcon != null)
            {
                _oldOverlayIcon = TaskbarOverlayIcon;
                TaskbarOverlayIcon = null;
            }
            else
            {
                TaskbarOverlayIcon = _oldOverlayIcon;
            }
        }

        private void btnAddToRecent_Click(object sender, EventArgs e)
        {
            _jumpListManager.AddToRecent(Path.Combine(_thisAppDataPath, "mary.txt"));
            _jumpListManager.AddToRecent(
                new ShellItem
                {
                    Path = Path.Combine(_thisAppDataPath, "sonnet.txt")
                });
            //TODO: This still doesn't work (Shell Link)
            _jumpListManager.AddToRecent(
                new ShellLink
                {
                    Path = Path.Combine(_thisAppDataPath, "raven.txt"),
                    Title = "The Raven"
                });
            string allDestinations = string.Join(", ",
                _jumpListManager.GetApplicationDestinations(
                ApplicationDestinationType.Recent).Select(d => Path.GetFileName(d.Path)).ToArray());
            statusLabel.Text = allDestinations;
        }

        private void btnBuildJumpList_Click(object sender, EventArgs e)
        {
            string shell32DllPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");

            _jumpListManager.AddCustomDestination(new ShellLink
            {
                Path = Path.Combine(_thisAppDataPath, "sonnet.txt"),
                Title = "Shall I compare thee...",
                Category = "My Category",
                IconLocation = shell32DllPath,
                IconIndex = 1
            });
            _jumpListManager.AddCustomDestination(new ShellLink
            {
                Path = Path.Combine(_thisAppDataPath, "mary.txt"),
                Title = "Mary had a little...",
                Category = "My Category",
                IconLocation = shell32DllPath,
                IconIndex = 1
            });
            _jumpListManager.AddCustomDestination(new ShellLink
            {
                Path = Path.Combine(_thisAppDataPath, "hello.html"),
                Title = "Hello World",
                Category = "My Other Category",
                IconLocation = shell32DllPath,
                IconIndex = 5
            });
            _jumpListManager.AddCustomDestination(new ShellItem
            {
                Path = Path.Combine(_thisAppDataPath, "raven.txt"),
                Category = "My Other Category"
            });
            _jumpListManager.AddUserTask(new ShellLink
            {
                Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "notepad.exe"),
                Title = "Launch Notepad",
                IconLocation = shell32DllPath,
                IconIndex = 14
            });
            _jumpListManager.AddUserTask(new Separator());
            _jumpListManager.AddUserTask(new ShellLink
            {
                Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "calc.exe"),
                Title = "Launch Calculator",
                IconLocation = shell32DllPath,
                IconIndex = 15
            });
            if (_jumpListManager.Refresh())
            {
                statusLabel.Text = "Maximum slots in list: " +
                    _jumpListManager.MaximumSlotsInList;
            }
        }

        private void btnProgressDemo_Click(object sender, EventArgs e)
        {
            progressBar.Increment(10);
            progressBar.SetTaskbarProgress();

            //TODO: Encapsulate into a new progress bar control
        }

        private void btnBGWorker_Click(object sender, EventArgs e)
        {
            BackgroundWorker worker = this.CreateProgressEnabledBackgroundWorker();
            worker.DoWork += delegate
            {
                for (int i = 0; i < 10; ++i)
                {
                    Thread.Sleep(1000);
                    worker.ReportProgress(i * 10 + 1);
                }
            };
            worker.RunWorkerAsync();
        }

        bool _customPreviewEnabled;
        CustomWindowsManager _windowsManager;

        private void btnTogglePreview_Click(object sender, EventArgs args)
        {
            if (!_customPreviewEnabled)
            {
                _windowsManager = CustomWindowsManager.CreateWindowsManager(Handle, IntPtr.Zero);
                _windowsManager.PeekRequested += (o, e) =>
                {
                    //TODO: This doesn't actually work when
                    //the window is minimized...
                    e.UseWindowScreenshot = true;
                };
                _windowsManager.ThumbnailRequested += (o, e) =>
                {
                    e.UseWindowScreenshot = true;
                };
            }
            else
                this.DisableCustomWindowPreview();

            _customPreviewEnabled = !_customPreviewEnabled;
        }

        private Size _originalSize = new Size(0, 0);
        private ChildForm _child;

        private void btnCreateChild_Click(object sender, EventArgs e)
        {
            if (_originalSize.Height == 0 && _originalSize.Width == 0)
            {
                _originalSize = Size;
                Size = new Size(Size.Width, Size.Height * 2);

                this.IsMdiContainer = true;
            }

            Point location = new Point(10, _originalSize.Height - menuStrip1.Height - statusLabel.Height);
            _child = new ChildForm(this, location);
            _child.Show();
        }

        private void btnThumbClip_Click(object sender, EventArgs e)
        {
            this.SetThumbnailClip(
                new Rectangle(btnThumbClip.Location, btnThumbClip.Size));
        }
    }
}