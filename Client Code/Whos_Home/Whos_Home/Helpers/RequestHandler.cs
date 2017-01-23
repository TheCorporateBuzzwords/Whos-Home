using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using RestSharp;

namespace Whos_Home.Helpers
{
    class RequestHandler
    {
        private string url = null;
        IRestRequest request = null;
        RestClient client = null;
        public RequestHandler(Context context)
        {

            url = context.Resources.GetString(Resource.String.url);
            client = new RestClient(url);
        }

       public async Task<IRestResponse> SignIn(User user)
        {

            request = new RestRequest("/session", Method.POST);

            request.AddObject(user);

            var response = await client.ExecuteTaskAsync(request);

            return response;
        }

        public async Task<IRestResponse> SignUp(User user)
        {
            request = new RestRequest("/users", Method.POST);

            request.AddObject(user);

            return await client.ExecuteTaskAsync(request);
            
        }


    }
}