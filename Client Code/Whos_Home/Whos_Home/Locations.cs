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
using Android.Net.Wifi;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;

namespace Whos_Home
{
    [Activity(Label = "Locations")]
    public class Locations : Activity
    {
        private Button AddLocation;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Locations);
            InitializeToolbars();
            InitializeFormat();
        }
        private void InitializeFormat()
        {
            AddLocation = FindViewById<Button>(Resource.Id.NewLocationButton);
            AddLocation.Click += AddLocation_Click;

            string permission = Manifest.Permission.AccessFineLocation;
            if (CheckSelfPermission(permission) != (int)Permission.Granted)
            {
                string[] poop = new string[1];
                poop[0] = Manifest.Permission.AccessFineLocation;
                ActivityCompat.RequestPermissions(this, poop, 0);
            }
        }

        private void AddLocation_Click(object sender, EventArgs e)
        {
           

            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            AddLocationFragment AddLocationDialog = new AddLocationFragment();
            AddLocationDialog.Show(transaction, "dialog fragment create account");
        }

        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Locations";

            //initialize bottom toolbar
            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            //editToolbar.Title = "Navigate";
            editToolbar.InflateMenu(Resource.Menu.edit_menus);
            editToolbar.MenuItemClick += NavigateMenu;

            //(sender, e) => {
            //Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            //};


        }

        //Method is used to navigate between activities using the bottom toolbar
        private void NavigateMenu(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            //Start the bulletin activity
            if (e.Item.ToString() == "Bulletins")
                this.StartActivity(typeof(BulletinBoard));

            //Start the Locations activity
            if (e.Item.ToString() == "Locations")
                this.StartActivity(typeof(Locations));

        }

        //called to specify menu resources for an activity
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        //called when a menu item is tapped
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();

            //Loads settings menu if preferences is selected
            if (item.ToString() == "Preferences")
                this.StartActivity(typeof(SettingsMenu));

            //Loads Groups menu if selected
            if (item.ToString() == "Groups")
                this.StartActivity(typeof(Groups));

            return base.OnOptionsItemSelected(item);
        }
    }
}