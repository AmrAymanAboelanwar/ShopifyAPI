using Newtonsoft.Json;
using Shopify.ViewModels.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shopify.Services.Interfaces
{
    public class FacebookService
    {

      
      

        private const string tokenValidateUrl ="https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string userInfoUrl = "https://graph.facebook.com/me?fields=first_name,last_name,email&access_token={0}";
        private readonly FacebookSettings _facebookSettings;
        private readonly IHttpClientFactory _httpClientFactory;



        public FacebookService(FacebookSettings facebookSettings , IHttpClientFactory httpClientFactory)
        {
            _facebookSettings = facebookSettings;
            _httpClientFactory = httpClientFactory;
        }


     


         public async Task<ValideTokenResult> ValidateAccessTokenAsync(string accessToken)
         {
            var formatedUrl = String.Format(tokenValidateUrl, accessToken , _facebookSettings.AppId,_facebookSettings.AppSecret);
            var result =  await _httpClientFactory.CreateClient().GetAsync(formatedUrl);
            var validResult = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ValideTokenResult>(validResult);
        }


        public async Task<FacebookUserResource> GetUserDataAsync(string accessToken)
        {
            var formatedUrl = String.Format(userInfoUrl, accessToken);
            var result = await _httpClientFactory.CreateClient().GetAsync(formatedUrl);
            var userData = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserResource>(userData);
        }

    }
}
