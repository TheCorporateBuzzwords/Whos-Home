using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Whos_Home.Helpers;

namespace Whos_Home
{
    class SignIn_Dialog : DialogFragment
    {
        private Button B_SignIn;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.sign_in, container, false);

            B_SignIn = view.FindViewById<Button>(Resource.Id.buttonlogin);

            //sets click function for the sign in button;
            B_SignIn.Click += SignInAttempt;

            return view;
        }

        public async void SignInAttempt(object sender, EventArgs e)
        {
            if (await CredentialHandler.SignIn(View.FindViewById<EditText>(Resource.Id.signinusername).Text, View.FindViewById<EditText>(Resource.Id.signinpassword).Text))
            {
                Toast.MakeText(Context, "Sign in succesful", ToastLength.Long);
                Activity.StartActivity(typeof(Locations));
            }
            else
            {
                InvalidInput();
            }
        }

        private void InvalidInput()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Login Failed");
            alert.SetMessage("Error Logging In. Please try again");

            alert.SetPositiveButton("Retry", (senderAlert, args) => { });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                Dismiss();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }
    }
}