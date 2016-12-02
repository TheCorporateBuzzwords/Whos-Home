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

namespace Whos_Home
{
    class SignIn_Dialog : DialogFragment
    {
        private Button SignIn;
        private string url = "96.41.173.205:8080";
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.sign_in, container, false);
            SignIn = view.FindViewById<Button>(Resource.Id.buttonlogin);

            //sets click function for the sign in button;
            SignIn.Click += new EventHandler(delegate (object sender, System.EventArgs e)
            {
                //retrieves data from dialog box
                var username = view.FindViewById<EditText>(Resource.Id.signinusername).Text;
                var password = view.FindViewById<EditText>(Resource.Id.signinpassword).Text;

                //create an alert box to show data that was entered (For testing)
                AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
                alert.SetTitle("Information");
                alert.SetMessage(string.Format(username + '\n' + password));

                //create an instance of a user and initialize it
                User user = new User(username, password);
                string json = JsonConvert.SerializeObject(user);

                //uploads json string to server
                //(new WebClient()).UploadString(url + "/users/", "PUT", json);


                //alert.SetPositiveButton("Continue", (senderAlert, args) =>
                //{
                //    Toast.MakeText(this.Context, "Deleted!", ToastLength.Short).Show();
                //});


                Dialog dialog = alert.Create();
                dialog.Show();
            });

            return view;

        }
    }
}