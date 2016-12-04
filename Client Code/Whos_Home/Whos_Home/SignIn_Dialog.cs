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
        private Button SignInButton;
        private string url = "http://96.41.173.205:3000";
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.sign_in, container, false);
            SignInButton = view.FindViewById<Button>(Resource.Id.buttonlogin);

            //sets click function for the sign in button;
            SignInButton.Click += SignInAttempt;

            return view;

        }
        public async void SignInAttempt(object sender, System.EventArgs e)
        {
            //retrieves data from dialog box
            View view = this.View;
            var username = view.FindViewById<EditText>(Resource.Id.signinusername).Text;
            var password = view.FindViewById<EditText>(Resource.Id.signinpassword).Text;

            Console.WriteLine("INFO:");
            Console.WriteLine("User Name: " + username);
            Console.WriteLine("Password: " + password);

            if (password != null && username != null && password != "" && username != "")
            {
                User user = new User(username, password);
                string json = JsonConvert.SerializeObject(user);

                var client = new RestClient(url);

                var request = new RestRequest("/session", Method.POST);
                request.AddObject(user);
                var response = await client.ExecuteTaskAsync(request);
                HttpStatusCode code = response.StatusCode;
                int code_num = (int)code;
                if (code_num == 200)
                {
                    Toast.MakeText(this.Context, "Login Successful", ToastLength.Long).Show();
                    this.Activity.StartActivity(typeof(MessageBoard));
                }
                else
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
                    alert.SetTitle("Login Failed");

                    if (response.Content != "")
                        alert.SetMessage(response.Content);
                    else
                        alert.SetMessage("Connection Error");

                    alert.SetPositiveButton("Retry", (senderAlert, args) => { });

                    alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
            }
            else
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
                alert.SetTitle("Login Failed");
                alert.SetMessage("Cannot leave either field blank");

                alert.SetPositiveButton("Retry", (senderAlert, args) => { });

                alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                    this.Activity.FragmentManager.Dispose();
                });
                Dialog dialog = alert.Create();
                dialog.Show();

            }
        }
    }
}