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

namespace Whos_Home
{
    class SignUp_Dialog : DialogFragment
    {
        private Button confirm;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.sign_up, container, false);
            confirm = view.FindViewById<Button>(Resource.Id.buttonConfirm);
            

            return view;

        }

        public override void OnResume()
        {
            confirm.Click += new EventHandler(delegate (object sender, System.EventArgs e)
            {
                var view = this.Activity;
                var email = view.FindViewById<EditText>(Resource.Id.emailtext).Text;
                var username = view.FindViewById<EditText>(Resource.Id.usernametext).Text;
                var password = view.FindViewById<EditText>(Resource.Id.textpassword).Text;
                var passCheck = view.FindViewById<EditText>(Resource.Id.repeatpasswordtext).Text;

                AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
                alert.SetTitle("Information");
                alert.SetMessage(string.Format("Email: {0}", email));
                alert.SetMessage(string.Format("User Name: {0}", username));
                alert.SetMessage(string.Format("Pass 1: {0}", password));
                alert.SetMessage(string.Format("Pass 2: {0}", passCheck));

                alert.SetPositiveButton("Continue", (senderAlert, args) =>
                {
                    Toast.MakeText(this.Context, "Deleted!", ToastLength.Short).Show();
                });

                alert.SetNegativeButton("Continue", (senderAlert, args) =>
                {
                    Toast.MakeText(this.Context, "Cancelled!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            });


        }

        public void testDel(object sender, System.EventArgs e)
        {
            var view = this.Activity;
            var email = view.FindViewById<EditText>(Resource.Id.emailtext);
            var username = view.FindViewById<EditText>(Resource.Id.usernametext);
            var password = view.FindViewById<EditText>(Resource.Id.textpassword);
            var passCheck = view.FindViewById<EditText>(Resource.Id.repeatpasswordtext);

            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Information");
            alert.SetMessage(string.Format("Email: {0}", email));
            alert.SetMessage(string.Format("User Name: {0}", username));
            alert.SetMessage(string.Format("Pass 1: {0}", password));
            alert.SetMessage(string.Format("Pass 2: {0}", passCheck));

            alert.SetPositiveButton("Continue", (senderAlert, args) =>
            {
                Toast.MakeText(this.Context, "Deleted!", ToastLength.Short).Show();
            });

            alert.SetNegativeButton("Continue", (senderAlert, args) =>
            {
                Toast.MakeText(this.Context, "Cancelled!", ToastLength.Short).Show();
            });

            Dialog dialog = alert.Create();
            dialog.Show();

        }
    }
}