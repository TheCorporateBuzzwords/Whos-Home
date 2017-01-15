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
using Newtonsoft.Json.Linq;
using System.Net;
using RestSharp;
using Couchbase.Lite;

namespace Whos_Home
{
    class SignUp_Dialog : DialogFragment
    {
        private Button SignUpButton;
        private string url = "http://75.142.141.235:3000";
        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.sign_up, container, false);
            SignUpButton = view.FindViewById<Button>(Resource.Id.buttonConfirm);

            //sets click function for the confirm button;
            SignUpButton.Click += SignUpAttempt;

            return view;

        }


        public async void SignUpAttempt(object sender, System.EventArgs e)
        {
            //retrieves data from dialog box
            Android.Views.View view = this.View;
            var firstname = view.FindViewById<EditText>(Resource.Id.firstnametext).Text;
            var lastname = view.FindViewById<EditText>(Resource.Id.lastnametext).Text;
            var email = view.FindViewById<EditText>(Resource.Id.emailtext).Text;
            var username = view.FindViewById<EditText>(Resource.Id.usernametext).Text;
            var password = view.FindViewById<EditText>(Resource.Id.passwordtext).Text;
            var passCheck = view.FindViewById<EditText>(Resource.Id.repeatpasswordtext).Text;

            bool fname_valid = (firstname != null && firstname != "");
            bool lname_valid = (lastname != null && lastname != "");
            bool email_valid = (email != null && email != "");
            bool uname_valid = (username != null && username != "");
            bool pass_valid = (password != null && password != "");
            bool all_valid = (fname_valid && lname_valid && email_valid && uname_valid && pass_valid);
            //create an alert box to show data that was entered (For testing)
            if (all_valid && password == passCheck)
            {
                User user = new User(firstname, lastname, username, email, password, passCheck);
                string json = JsonConvert.SerializeObject(user);

                var client = new RestClient(url);

                var request = new RestRequest("/users", Method.POST);
                request.AddObject(user);
                var response = await client.ExecuteTaskAsync(request);
                HttpStatusCode code = response.StatusCode;
                int code_num = (int)code;
                if (code_num == 201)
                {
                    Toast.MakeText(this.Context, "Account Created!", ToastLength.Long).Show();
                    this.Activity.StartActivity(typeof(MessageBoard));

                    InsertInDB(username, email, firstname, response);
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

        private void InvalidResponse(IRestResponse response)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Signup Failed");

            if (response.Content != "")
                alert.SetMessage(response.Content);
            else
                alert.SetMessage("Connection Error");

            alert.SetPositiveButton("Retry", (senderAlert, args) => { });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                Dismiss();
            });
            Dialog dialog = alert.Create();
            dialog.Show();

        }
        private void InvalidInput()
        {
                AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
                alert.SetTitle("Signup Failed");
                alert.SetTitle("Cannot leave any field blank");
                alert.SetPositiveButton("Retry", (senderAlert, args) => { });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    Dismiss();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
        }
        private void InsertInDB(string username, string email, string firstname, IRestResponse response)
        {
            var db = Manager.SharedInstance.GetDatabase("userinfo");

            Console.WriteLine(response.Content);

            JObject respJson = JObject.Parse(response.Content);

            string token = (string)respJson["token"];

            var vals = new Dictionary<String, Object>
                    {
                        {"username", username },
                        {"email", email },
                        {"firstname", firstname },
                        {"token", token }
                    };

            var doc = db.CreateDocument();

            try
            {
                doc.PutProperties(vals);
            }
            catch (CouchbaseLiteException)
            {
                throw new ApplicationException("Database Failure");
            }


            //Test Retrieval
            var view = db.GetView("token");

            view.SetMap((docret, emit) =>
            {
                emit(docret["token"], docret["token"]);
            }, "1");

            var query = db.GetView("token").CreateQuery();

            query.Descending = true;
            query.Limit = 1;
            var rows = query.Run();
            foreach (var row in rows)
            {
                var name = row.Key;
                var tok = row.Value;

                Console.WriteLine("DB Q: {0}, {1}", name, tok);
            }

        }
    }
}