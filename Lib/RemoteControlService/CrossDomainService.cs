using System.Xml;
using System.ServiceModel.Channels;
using System.IO;

namespace Big3.Hitbase.RemoteControlService
{
    public class CrossDomainService : ICrossDomainService
    {
        public System.ServiceModel.Channels.Message ProvidePolicyFile()
        {
            XmlReader reader = XmlReader.Create(@"ClientAccessPolicy.xml");
            System.ServiceModel.Channels.Message result = Message.CreateMessage(MessageVersion.None, "", reader);
            return result; 
        }
    }
}