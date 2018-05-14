using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Configuration;

namespace Big3.Hitbase.CDUtilities.Amazon
{
    class AmazonSigningBehaviorExtensionElement : BehaviorExtensionElement
    {
        public AmazonSigningBehaviorExtensionElement()
        {
        }

        public override Type BehaviorType
        {
            get
            {
                return typeof(AmazonSigningEndpointBehavior);
            }
        }

        protected override object CreateBehavior()
        {
            // Set your AWS Access Key ID and AWS Secret Key here.
            // You can obtain them at:
            // http://aws-portal.amazon.com/gp/aws/developer/account/index.html?action=access-key

            return new AmazonSigningEndpointBehavior("0FN016GTSMZHJD0C7YG2", "1TC2Dytk/uauXGMmyivyMA4S4MZkQ4dYlyWtuxuA");
        }

    }
}
