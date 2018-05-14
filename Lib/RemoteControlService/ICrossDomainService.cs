using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.ServiceModel;


namespace Big3.Hitbase.RemoteControlService
{
    // NOTE: If you change the interface name "ICrossDomainService" here, you must also update the reference to "ICrossDomainService" in App.config.
    [ServiceContract]
    public interface ICrossDomainService
    {
        [OperationContract]
        [WebGet(UriTemplate = "ClientAccessPolicy.xml")]
        Message ProvidePolicyFile();
    }
}