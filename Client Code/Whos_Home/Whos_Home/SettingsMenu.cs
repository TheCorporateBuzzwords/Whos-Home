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
using Whos_Home.Helpers;

namespace Whos_Home
{
    [Activity(Label = "SettingsMenu")]
    public class SettingsMenu : BaseActivity
    {
        private List<String> items = new List<String>();
        private ListView listview;
        private string appendUsername, appendEmail;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SettingsMenu);

            InitializeToolbars();
            InitializeLocations();
            ActionBar.Title = "Preferences";
        }

        private void InitializeLocations()
        {

            DB_Singleton db = DB_Singleton.Instance;
            appendUsername = db.Retrieve("UserName");
            appendEmail = db.Retrieve("Email");

            TextView username = FindViewById<TextView>(Resource.Id.textviewSettingsMenuUsername);
            TextView email = FindViewById<TextView>(Resource.Id.textviewSettingsMenuEmail);

            //This is where the text values for the username and email are set
            username.Text = "Username: " + appendUsername;
            email.Text = "Email: " + appendEmail;


            //initializes a switch object to the one found in the settings page
            Switch s = FindViewById<Switch>(Resource.Id.SwitchSettingsMenu);

            //sets the function used when toggle is checked or unchecked
            s.CheckedChange += ToggleLocations;
        }

        
        private void ToggleLocations(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            
            int length = items.Count;
            //if the toggle is set to on
            if (e.IsChecked)
            {
                //set all locations to true if toggled on
                for (int i = 0; i < length; ++i)
                    listview.SetItemChecked(i, true);
            }
            else
            {
                //set all locations to false
                for (int i = 0; i < length; ++i)
                    listview.SetItemChecked(i, false);
            }
        }
    }
}