using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Big3.Hitbase.CDUtilities.Amazon
{
	public class AmazonSigningEndpointBehavior : IEndpointBehavior {
		private string	_accessKeyId	= "";
		private string	_secretKey	= "";

        public AmazonSigningEndpointBehavior()
        {
            this._accessKeyId = "0FN016GTSMZHJD0C7YG2";
            this._secretKey = "1TC2Dytk/uauXGMmyivyMA4S4MZkQ4dYlyWtuxuA";
        }

		public AmazonSigningEndpointBehavior(string accessKeyId, string secretKey) {
			this._accessKeyId	= accessKeyId;
			this._secretKey		= secretKey;
		}

		public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime) {
			clientRuntime.MessageInspectors.Add(new AmazonSigningMessageInspector(_accessKeyId, _secretKey));
		}

		public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher) { return; }
		public void Validate(ServiceEndpoint serviceEndpoint) { return; }
		public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters) { return; }
	}
}
