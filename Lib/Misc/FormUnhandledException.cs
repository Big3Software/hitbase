using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.Miscellaneous
{
    public partial class FormUnhandledException : Form
    {
        public FormUnhandledException()
        {
            InitializeComponent();
        }

        public static void InitWPF()
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.UnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(CurrentDispatcher_UnhandledException);
            System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            FormUnhandledException formUnhandledException = new FormUnhandledException(e.Exception);

            formUnhandledException.ShowDialog();
        }

        static void CurrentDispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            FormUnhandledException formUnhandledException = new FormUnhandledException(e.Exception);

            formUnhandledException.ShowDialog();
            e.Handled = true;
        }

        static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            MessageBox.Show(e.ToString());
        }

        public FormUnhandledException(Exception e) : this()
        {
            Exception = e;
        }

        public Exception Exception
        {
            set
            {
                string exceptionDetails = value.ToString();

                textBoxDetails.Text = StringTable.ErrorMessage + ":\r\n" + value.Message + "\r\n\r\nWeitere Informationen:\r\n" + exceptionDetails;
            }
        }

        private void buttonSendEMail_Click(object sender, EventArgs e)
        {
            try
            {
                string subject = string.Format("{0} Bug-Report", Application.ProductName);

                subject = Uri.EscapeUriString(subject);

                string body = StringTable.EnterBugDescriptionHere + "\r\n\r\n" + textBoxDetails.Text.ToString();

                body = body.Replace("%", "%25");
                body = body.Replace("\r", "%0D");
                body = body.Replace("\n", "%0A");
                body = body.Replace(" ", "%20");
                body = body.Replace("=", "%3D");
                body = body.Replace("&", "%26");
                body = body.Replace("+", "%2B");
                body = body.Replace("#", "%23");

                string sendMail = string.Format("mailto:bug@hitbase.de?subject={0}&body={1}", subject, body);

                System.Diagnostics.Process.Start(sendMail);
            }
            catch (Exception ex)
            {
                MessageBox.Show(StringTable.ErrorSendingMail, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxDetails.Text);
        }
    }
}
