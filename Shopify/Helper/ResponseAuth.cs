using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Helper
{
    public class ResponseAuth
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
