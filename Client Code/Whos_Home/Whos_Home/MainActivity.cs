using Android.App;
using Android.Widget;
using Android.OS;

namespace Whos_Home
{
    [Activity(Label = "Who\'s Home?", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button B_CreateAccount; //CreateAccount button
        private Button B_SignIn; //Sign in button

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Creates instance of Create Account button
            B_CreateAccount = FindViewById<Button>(Resource.Id.buttonCreateAccount);
            B_CreateAccount.Click += BCreateAccount_Click;

            //Creates instance of Sign in button
            B_SignIn = FindViewById<Button>(Resource.Id.buttonSignIn);
            B_SignIn.Click += BSignIn_Click;
        }

        //Function is called when sign in button is clicked
        private void BSignIn_Click(object sender, System.EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SignIn_Dialog SignInDialog = new SignIn_Dialog();
            SignInDialog.Show(transaction, "dialog fragment sign in");
            //throw new System.NotImplementedException();
        }

        //Function is called when create account button is clicked
        private void BCreateAccount_Click(object sender, System.EventArgs e)
        {
            //dialog box
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SignUp_Dialog CreateAccountDialog = new SignUp_Dialog();
            CreateAccountDialog.Show(transaction, "dialog fragment create account");
            //throw new System.NotImplementedException();
        }

        private void BTest_Click(object sender, System.EventArgs e)
        {
            this.StartActivity(typeof(Locations));
        }
    }

    [Activity(Label = "ButtonStyle", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
        }
    }
}

