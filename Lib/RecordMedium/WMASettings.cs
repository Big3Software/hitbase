using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.MediaRipper.WMFSdk;
using System.Runtime.InteropServices;


namespace Big3.Hitbase.RecordMedium
{
    public partial class WMASettings : Form
    {
        public WMASettings()
        {
            InitializeComponent();
            //listBoxWMACodecs.Items.AddRange(
            uint profCount;
            IWMProfileManager2 profManager;
            IWMProfile profile;
            IWMProfile3 profile3;
            IWMStreamConfig streamConf;
            
            profManager = (IWMProfileManager2)WM.CreateProfileManager();

            profManager.CreateEmptyProfile(WMT_VERSION.WMT_VER_9_0, out profile);

            Guid mediaTypeGuid = WmMediaTypeId.Audio;
            IWMCodecInfo codecInfo = (IWMCodecInfo)profManager;
            IWMStreamConfig streamConfig;
            codecInfo.GetCodecFormat(ref mediaTypeGuid, (uint)0, (uint)74, out streamConfig);
            streamConfig.SetStreamNumber(1);
            streamConfig.SetStreamName("AudioStream1");
            streamConfig.SetConnectionName("Audio1");
        

        }
    }

    [ComVisible(true), ComImport,
    Guid("A970F41E-34DE-4a98-B3BA-E4B3CA7528F0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IWMCodecInfo
    {
        void GetCodecInfoCount(
            [In] ref Guid guidType,
            [Out] out uint pcCodecs);

        void GetCodecFormatCount(
            [In] ref Guid guidType,
            [In] uint dwCodecIndex,
            [Out] out uint pcFormat);

        void GetCodecFormat(
            [In] ref Guid guidType,
            [In] uint dwCodecIndex,
            [In] uint dwFormatIndex,
            [Out, MarshalAs(UnmanagedType.Interface)] out IWMStreamConfig
    ppIStreamConfig);
    };

    internal class WmMediaTypeId
    {
        private WmMediaTypeId()
        {
        }

        public static readonly Guid Audio = new
    Guid("73647561-0000-0010-8000-00AA00389B71");
        public static readonly Guid Video = new
    Guid("73646976-0000-0010-8000-00AA00389B71");
    }

    internal class WmFormatId
    {
        public static readonly Guid WaveFormatEx = new
    Guid("05589f81-c356-11ce-bf01-00aa0055595a");
    }

}
