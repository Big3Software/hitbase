using System.Runtime.InteropServices;
using System;

namespace Big3.Hitbase.Miscellaneous
{
#if false
    /// <summary>
    /// Type d'autoplay
    /// </summary>
    /// <remarks></remarks>
    [Flags]
    public enum AutorunContent
    {
        AutorunInf = 2,
        AudioCD = 4,
        DVDMovie = 8,
        BlankCD = 16,
        BlankDVD = 32,
        UnknownContent = 64,
        AutoPlayPictures = 128,
        AutoPlayMusics = 256,
        AutoPlayMovies = 512
    }

    /// <summary>
    /// Interface permettant d'être notifier d'une tentative d'Autoplay et de pouvoir l'annuler
    /// </summary>
    /// <remarks></remarks>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("DDEFE873-6997-4e68-BE26-39B633ADBE12")]
    public interface IQueryCancelAutoPlay
    {
        [PreserveSig]
        int AllowAutoPlay([In, MarshalAs(UnmanagedType.LPWStr)]
      [In()]
      [In(), MarshalAs(UnmanagedType.LPWStr)]
      [In()]
    string pszPath, [In(), MarshalAs(UnmanagedType.LPWStr)]
      [In()]
      [In(), MarshalAs(UnmanagedType.LPWStr)]
      [In()]
    AutorunContent dwContentType, [In(), MarshalAs(UnmanagedType.LPWStr)]
      [In()]
      [In(), MarshalAs(UnmanagedType.LPWStr)]
      [In()]
    string pszLabel, [In(), MarshalAs(UnmanagedType.LPWStr)]
      [In()]
      [In(), MarshalAs(UnmanagedType.LPWStr)]
      [In()]
    int dwSerialNumber);
    }


    /// <summary>
    /// Contient les informations sur un Autoplay
    /// </summary>
    /// <remarks>La réponse pour annulation ne doit pas prendre plus de 3 secondes. Au delà, la réponse est ignoré et l'Autoplay lancé</remarks>
    public class AutorunEventArgs : System.ComponentModel.CancelEventArgs
    {
        private string m_DrivePath;
        /// <summary>
        /// Lecteur déclencheur de l'Autoplay
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string DrivePath
        {
            get { return m_DrivePath; }
        }
        private AutorunContent m_ContentType;
        /// <summary>
        /// Type de contenu lancé par l'Autoplay
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public AutorunContent ContentType
        {
            get { return m_ContentType; }
        }
        private string m_Label;
        /// <summary>
        /// Label du média
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Label
        {
            get { return m_Label; }
        }
        private int m_SerialNumber;
        /// <summary>
        /// Numéro de série du média
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int SerialNumber
        {
            get { return m_SerialNumber; }
        }

        public AutorunEventArgs(string drivePath, AutorunContent contentType, string label, int serial)
        {
            base.New(false);
            this.m_ContentType = contentType;
            this.m_DrivePath = drivePath;
            this.m_Label = label;
            this.m_SerialNumber = serial;
        }
    }

    // <summary>
    // Objet exposé à COM pour être notifié et annuler l'Autoplay sou XP et suivants
    // </summary>
    // <remarks>Cet objet sera inscrit dans la Running Object Table pour être appelé par le Shell</remarks>
    [ComVisible(true), Guid("5C262509-3D83-4181-B1B4-CB3A8E3EA5C0")]
    public class AutoplayControler : IDisposable, IQueryCancelAutoPlay
    {
        //indique que l'objet doit rester vivant autant de temps que possible
        private const Int32 ROTFLAGS_REGISTRATIONKEEPSALIVE = 1;
        //crée un IMoniker pour surveiller et encadrer l'instance de cet objet
        // ERROR: Not supported in C#: DeclareDeclaration
        //récupère l'interface d'accès à la Running Object Table
        // ERROR: Not supported in C#: DeclareDeclaration
        //code de retour de l'appel à notre méthode de notification
        private const Int32 S_FALSE = 1;
        private const Int32 S_OK = 0;

        /// <summary>
        /// Evènement déclenché lorsqu'un Autoplay se produit
        /// </summary>
        /// <param name="sender">Cet objet</param>
        /// <param name="e">Informations sur l'Autoplay</param>
        /// <remarks></remarks>
        public event WantAutorunEventHandler WantAutorun;
        public delegate void WantAutorunEventHandler(object sender, AutorunEventArgs e);

        /// <summary>
        /// Cette méthode est appelée quand l'Autoplay se déclenche pour un média connecté
        /// Cela permet aussi d'empêcher la suite de l'exécution de l'Autoplay
        /// </summary>
        /// <remarks>Cette méthode a au maximum 3 secondes pour répondre</remarks>
        public int IQueryCancelAutoPlay.AllowAutoPlay(string pszPath, AutorunContent dwContentType, string pszLabel, int dwSerialNumber)
        {
            //récupère les informations
            AutorunEventArgs e = new AutorunEventArgs(pszPath, dwContentType, pszLabel, dwSerialNumber);
            if (WantAutorun != null)
            {
                WantAutorun(this, e);
            }

            //si on veut annuler
            if (e.Cancel)
            {
                return S_FALSE;
            }
            else
            {
                return S_OK;
            }
        }

        private bool m_IsRunning = false;
        /// <summary>
        /// Indique si l'objet est inscrit dans la Running Object Table et peut donc être notifié de l'Autoplay
        /// </summary>
        /// <value></value>
        /// <returns>true si l'objet a été enregistré dans la Running Object Table</returns>
        /// <remarks></remarks>
        public bool IsRunning
        {
            get { return m_IsRunning; }
        }

        //numéro d'enregistrement de notre instance dans la ROT
        private int m_RegisterNumber;
        //surveilleur/contrôleur/courtier de notre instance
        private ComTypes.IMoniker m_Moniker;
        public AutoplayControler()
        {
        }

        /// <summary>
        /// Permet d'enregistrer notre instance de la ROT et donc être avertit des Autoplay
        /// </summary>
        /// <remarks></remarks>
        public void RegisterAutoplayMonitor()
        {
            //si on est déjà dans la ROT, on ne fait rien
            if (this.IsRunning) return;

            //on crée un moniteur/contrôleur/courtier pour encadrer et associer notre instance
            int ret = CreateClassMoniker(new Guid("331F1768-05A9-4ddd-B86E-DAE34DDC998A"), m_Moniker);
            if (ret != 0) Marshal.ThrowExceptionForHR(ret);

            //on récupère l'interface d'accès à la ROT
            ComTypes.IRunningObjectTable rot = null;
            ret = GetRunningObjectTable(0, rot);
            if (ret != 0) Marshal.ThrowExceptionForHR(ret);

            //on enregistre notre instance dans le ROT pour être notifié
            m_RegisterNumber = rot.Register(ROTFLAGS_REGISTRATIONKEEPSALIVE, this, m_Moniker);
            this.m_IsRunning = true;
        }

        /// <summary>
        /// Permet de retirer notre instance de la ROT et donc de ne plus être notifié des Autoplay
        /// </summary>
        /// <remarks></remarks>
        public void UnregisterAutoplayMonitor()
        {
            //si on n'est pas dans la ROT, on ne fait rien
            if (!this.IsRunning) return;

            //on récupère l'interface d'accès à la Running Object Table
            System.Runtime.InteropServices.ComTypes.IRunningObjectTable rot = null;
            int ret = GetRunningObjectTable(0, rot);
            if (ret != 0) Marshal.ThrowExceptionForHR(ret);

            //retirer notre instance
            rot.Revoke(m_RegisterNumber);
            this.m_IsRunning = false;
        }

        private bool disposedValue = false;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: free managed resources when explicitly called
                }
                this.UnregisterAutoplayMonitor();
                // TODO: free shared unmanaged resources
            }
            this.disposedValue = true;
        }

        #region  IDisposable Support
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void IDisposable.Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Enregistrement notre classe exposant exposant IQueryCanceAutoPlay par COM en tant que AutoplayHandler/CancelAutoplay
        /// </summary>
        /// <param name="t"></param>
        /// <remarks>Théoriquement l'enregistrement de notre objet COM exposant IQueryCanceAutoPlay doit être fait dans les CancelHandlers mais il semble que ca marche aussi sans</remarks>
        [ComRegisterFunction()]
        private static void RegisterHandler(Type t)
        {
            Microsoft.Win32.RegistryKey cancelAutoplayKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\AutoplayHandlers\\CancelAutoplay\\CLSID");
            cancelAutoplayKey.SetValue(t.GUID.ToString("D").ToUpper(), string.Empty);
        }
        /// <summary>
        /// Retire l'enregistrement de notre classe exposant exposant IQueryCanceAutoPlay par COM en tant que AutoplayHandler/CancelAutoplay
        /// </summary>
        /// <param name="t"></param>
        /// <remarks>Théoriquement l'enregistrement de notre objet COM exposant IQueryCanceAutoPlay doit être fait dans les CancelHandlers mais il semble que ca marche aussi sans</remarks>
        [ComUnregisterFunction()]
        private static void UnregisterHandler(Type t)
        {
            Microsoft.Win32.RegistryKey cancelAutoplayKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\AutoplayHandlers\\CancelAutoplay\\CLSID");
            cancelAutoplayKey.DeleteValue(t.GUID.ToString("D").ToUpper());
        }
    }

#endif
}
