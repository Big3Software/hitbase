using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Big3.Hitbase.RemoteControlService
{
    // NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in App.config.
    [ServiceContract]
    public interface IRemoteControlService
    {
        [OperationContract]
        void Play();

        [OperationContract]
        int AddToPlaylist(string soundFilename);

        [OperationContract]
        void AddToWishlist(int trackId, string wishBy, string comment);

        [OperationContract]
        List<SearchResultItem> Search(string searchFor);
    }

    [ServiceContract]
    public class SearchResultItem
    {
        public int TrackID { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public int Length { get; set; }

        public string SoundFilename { get; set; }

        public byte[] CoverImage { get; set; }
    }
}
