using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Shopify.ViewModels
{
    public class GoogleLoginModel
    {

        public Payload payload { get; set; }
    }
}
