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
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;



namespace Whos_Home
{
    class SignIn_Dialog : DialogFragment
    {
        private Button SignInButton;
        private string url = null;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.sign_in, container, false);

            url = Context.Resources.GetString(Resource.String.url);
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
                    this.Activity.StartActivity(typeof(BulletinBoard));
                    InsertInDB(DecodeToken(response));
                }
                else
                {
                    InvalidResponse(response);
                }
            }
            else
            {
                InvalidInput();
            }
        }

        private string[] DecodeToken(IRestResponse response)
        {
            JObject respJson = JObject.Parse(response.Content);

            string token = (string)respJson["token"];

            JwtSecurityToken jwtToken = new JwtSecurityToken(token);

            string pattern = @"{.*?}";
            string decodedJwt = jwtToken.ToString();
            List<string> regResults = new List<string>();

            foreach(Match m in Regex.Matches(decodedJwt, pattern))
            {
                regResults.Add(m.ToString());
            }

            JObject json = JObject.Parse(regResults[1]);
            string username = (string)json["Username"];
            string email = (string)json["Email"];
            string firstname = (string)json["First"];

            string[] decodedToken = { token, username, email, firstname };

            return decodedToken;
        }

        private void InsertInDB(string[] decodedToken)
        {
            
            DB_Singleton instance = DB_Singleton.Instance;

            instance.InitDB();

            instance.InitialInsert(decodedToken[0], decodedToken[1],
                decodedToken[2], decodedToken[3]);
        }

        private void InvalidResponse(IRestResponse response)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Login Failed");

            if (response.Content != "")
                alert.SetMessage(response.Content);
            else
                alert.SetMessage("Connection Error");

            alert.SetPositiveButton("Retry", (senderAlert, args) => { });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                Dismiss();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void InvalidInput()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Login Failed");
            alert.SetMessage("Cannot leave either field blank");

            alert.SetPositiveButton("Retry", (senderAlert, args) => { });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                Dismiss();
            });
            Dialog dialog = alert.Create();
            dialog.Show();

        }
    }
}