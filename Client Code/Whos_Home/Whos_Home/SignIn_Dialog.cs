using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System.Net;
using RestSharp;

namespace Whos_Home
{
    class SignIn_Dialog : DialogFragment
    {
        private Button SignIn;
        private string url = "https://jsonplaceholder.typicode.com";
        private string url1 = "https://96.41.173.205:8080";
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.sign_in, container, false);
            SignIn = view.FindViewById<Button>(Resource.Id.buttonlogin);

            //sets click function for the sign in button;
            SignIn.Click += testDel;

            return view;

        }
        public async void testDel(object sender, System.EventArgs e)
        {
            //retrieves data from dialog box
                View view = this.View;
                var username = view.FindViewById<EditText>(Resource.Id.signinusername).Text;
                var password = view.FindViewById<EditText>(Resource.Id.signinpassword).Text;

                Console.WriteLine("INFO:");
                Console.WriteLine("User Name: " + username);
                Console.WriteLine("Password: " + password);

                if(password != null && username != null)
                {
                    User user = new User(username, password);
                    string json = JsonConvert.SerializeObject(user);

                    var client = new RestClient(url);

                    var request = new RestRequest(url + "/users", Method.GET);
                    request.AddObject(user);
                    var response = await client.ExecuteTaskAsync(request);
                    Console.WriteLine("RESPONSE: " + response.Content);
                }

                //create an instance of a user and initialize it
            }
    }
}