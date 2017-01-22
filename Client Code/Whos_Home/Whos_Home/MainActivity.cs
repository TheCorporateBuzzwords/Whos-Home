using Android.App;
using Android.Widget;
using Android.OS;

namespace Whos_Home
{
    [Activity(Label = "Who\'s Home?", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button BCreateAccount; //CreateAccount button
        private Button BSignIn; //Sign in button
        private Button BTest; //Button used exclusively for testing
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);


            //Creates instance of Create Account button
            BCreateAccount = FindViewById<Button>(Resource.Id.buttonCreateAccount);
            BCreateAccount.Click += BCreateAccount_Click;

            //Creates instance of Sign in button
            BSignIn = FindViewById<Button>(Resource.Id.buttonSignIn);
            BSignIn.Click += BSignIn_Click;

            //Creates instance of the Test button REMOVE IN DEPLOYMENTS
            BTest = FindViewById<Button>(Resource.Id.buttonTest);
            BTest.Click += BTest_Click;
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
            this.StartActivity(typeof(BulletinBoard));

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

