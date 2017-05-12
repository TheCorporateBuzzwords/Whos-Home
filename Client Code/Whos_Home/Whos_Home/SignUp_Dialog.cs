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
using Whos_Home.Helpers;

namespace Whos_Home
{
    class SignUp_Dialog : DialogFragment
    {
        private Button B_SignUp;
        private string url = null;

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.sign_up, container, false);

            url = Context.Resources.GetString(Resource.String.url);

            B_SignUp = view.FindViewById<Button>(Resource.Id.buttonConfirm);

            //sets click function for the confirm button;
            B_SignUp.Click += SignUpAttempt;

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

            User user = new User(firstname, lastname, username, email, password, passCheck);
            if (await CredentialHandler.SignUp(user))
                this.Activity.StartActivity(typeof(Locations));

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

    }
}