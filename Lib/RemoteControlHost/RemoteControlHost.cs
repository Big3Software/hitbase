using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows.Forms;
using System.Threading;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.RemoteControlService;

namespace Big3.Hitbase.RemoteControlHost
{
    public class RemoteControlHost
    {
        static private DataBase dataBase;

        static public event EventHandler OnPlay;

        static public event AddToPlaylistHandler OnAddToPlaylist;
        static public event AddToWishlistHandler OnAddToWishlist;

        static public AutoResetEvent RemoteHostEvent = new AutoResetEvent(false);

        public RemoteControlHost(DataBase db)
        {
            dataBase = db;
        }

        public void StartListening()
        {
            Thread hostThread = new Thread(StartListeningHost);
            hostThread.Start();
        }

        static private void StartListeningHost()
        {
            try
            {
                // Create a ServiceHost for the CalculatorService type and use
                // the base address from config.
                ServiceHost hostDefault = new ServiceHost(typeof(RemoteControlService.RemoteControlService));

                TimeSpan closeTimeout = hostDefault.CloseTimeout;
                TimeSpan openTimeout = hostDefault.OpenTimeout;

                ServiceAuthorizationBehavior authorization = hostDefault.Authorization;
                ServiceCredentials credentials = hostDefault.Credentials;
                ServiceDescription description = hostDefault.Description;

                int manualFlowControlLimit =
                        hostDefault.ManualFlowControlLimit;

                NetTcpBinding portsharingBinding = new NetTcpBinding();
                hostDefault.AddServiceEndpoint(typeof(RemoteControlService.IRemoteControlService), portsharingBinding,
                            "net.tcp://localhost/RemoteControlService");

                int newLimit = hostDefault.IncrementManualFlowControlLimit(100);

                RemoteControlService.RemoteControlService rcs = new RemoteControlService.RemoteControlService();
                rcs.OnPlay += new EventHandler(rcs_OnPlay);
                rcs.OnAddToPlaylist += new RemoteControlService.AddToPlaylistHandler(rcs_OnAddToPlaylist);
                rcs.OnAddToWishlist += new AddToWishlistHandler(rcs_OnAddToWishlist);
                rcs.DataBase = dataBase;
                ServiceHost serviceHost = new ServiceHost(rcs);

                ServiceHost crossDomainserviceHost = new ServiceHost(typeof(CrossDomainService));

                serviceHost.Open();
                crossDomainserviceHost.Open();
                // The service can now be accessed.

                // Wartet so lange, bis die Hauptapplikation endet.
                RemoteHostEvent.WaitOne();

                // Close the ServiceHost.
                serviceHost.Close();
                crossDomainserviceHost.Close();
            }
            catch       // Zuerst mal die Fehler ignorieren
            {
            }
            /*using (ServiceHost serviceHost = new ServiceHost(typeof(RemoteControlService.RemoteControlService)))
            {
                try
                {
                    // Open the ServiceHost to start listening for messages.
                    serviceHost.Open();
                    object o = serviceHost.SingletonInstance;
                    // The service can now be accessed.
                    MessageBox.Show("Remote Control Host ready!");

                    // Close the ServiceHost.
                    serviceHost.Close();
                }
                catch (TimeoutException timeProblem)
                {
                    Console.WriteLine(timeProblem.Message);
                    Console.ReadLine();
                }
                catch (CommunicationException commProblem)
                {
                    Console.WriteLine(commProblem.Message);
                    Console.ReadLine();
                }
            }*/
        }

        static void rcs_OnAddToWishlist(object sender, AddToWishlistEventArgs e)
        {
            if (OnAddToWishlist != null)
                OnAddToWishlist(sender, e);
        }

        static void rcs_OnPlay(object sender, EventArgs e)
        {
            if (OnPlay != null)
                OnPlay(sender, e);
        }

        static void rcs_OnAddToPlaylist(string soundFilename, AddToPlaylistEventArgs e)
        {
            if (OnAddToPlaylist != null)
                OnAddToPlaylist(soundFilename, e);
        }
    }
}
