using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.Miscellaneous
{
    public partial class FormAnnouncements : Form
    {
        public FormAnnouncements()
        {
            InitializeComponent();

            Load += new EventHandler(FormAnnouncements_Load);
        }

        void FormAnnouncements_Load(object sender, EventArgs e)
        {
            labelDate.Text = Date.ToLongDateString();
        }

        public DateTime Date
        {
            get;
            set;
        }

        public static void CheckAnnouncement()
        {
            try
            {
                DateTime lastModified = GetLastModifiedTime("http://www.hitbase.de/announcement.htm");

                if (lastModified.ToString() != Settings.Current.LastAnnouncementDate)
                {
                    FormAnnouncements formAnnouncements = new FormAnnouncements();
                    formAnnouncements.Date = lastModified;
                    formAnnouncements.ShowDialog();
                    Settings.Current.LastAnnouncementDate = lastModified.ToString();
                }
            }
            catch
            {
                // Fehler hierbei ignorieren
            }
        }
    
        public static DateTime GetLastModifiedTime(string url) 
        { 
            WebRequest request = WebRequest.Create(url); 
            request.Credentials = CredentialCache.DefaultNetworkCredentials; 
            request.Method = "HEAD"; 
            
            using (WebResponse response = request.GetResponse()) 
            { 
                string lastModifyString = response.Headers.Get("Last-Modified");
                DateTime remoteTime; 
                if (DateTime.TryParse(lastModifyString, out remoteTime))
                { 
                    return remoteTime; 
                } 
                
                return DateTime.MinValue; 
            } 
        }
    }
}
