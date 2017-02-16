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
using Whos_Home.Helpers;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Whos_Home
{
    [Activity(Label = "Locations")]
    public class Locations : Activity
    {
        private Button AddLocation;
        private TextView CurrentLocation;
        private List<string> location_names = new List<string>();
        private ListView LocationList;
        private List<string> db_locations = new List<string>();
        private string current_location;
        private List<string> WifiNetworks = new List<string>();

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

            LocationList = FindViewById<ListView>(Resource.Id.locationlistview);
            CurrentLocation = FindViewById<TextView>(Resource.Id.textCurrentLocation);

            //This block asks the user for location permissions
            //Or checks if the user already gave permissions
            string permission = Manifest.Permission.AccessFineLocation;
            if (CheckSelfPermission(permission) != (int)Permission.Granted)
            {
                string[] request_permissions = new string[1];
                request_permissions[0] = Manifest.Permission.AccessFineLocation;
                ActivityCompat.RequestPermissions(this, request_permissions, 0);
            }
            DB_Singleton db = DB_Singleton.Instance;

            try
            {
                db.GetActiveGroup();
                GetLocations();
                

            }
            catch
            {
                Console.WriteLine("No active group selected: Locations");
            }
            
        }

        private async void GetLocations()
        {
            DB_Singleton db = DB_Singleton.Instance;

            RequestHandler request = new RequestHandler(this.BaseContext);

            //get locations from database
            var response = await request.GetLocations(db.Retrieve("Token"), db.GetActiveGroup().GroupID);
            //convert locations from json format
            db_locations = ConvertJson(response);
            //add locations into list adapter
            LocationList.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, db_locations);

            UpdateLocation();

        }

        private async void UpdateLocation()
        {
            WifiManager wifimanager = (WifiManager)GetSystemService(Context.WifiService);

            string permission = Manifest.Permission.AccessFineLocation;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                var InRange = wifimanager.ScanResults;

                foreach (ScanResult network in InRange)
                {
                    WifiNetworks.Add(network.Ssid);
                    //WifiNetworkKey.Add(network.Bssid);
                }
            }

            RequestHandler request = new RequestHandler(this.BaseContext);
            IRestResponse response;
            DB_Singleton db = DB_Singleton.Instance;

            //some garbage string
            current_location = "$$560091692749045729$#^^%$";

            for (int i = 0; i < WifiNetworks.Count; ++i)
            {
                for(int j = 0; j < db_locations.Count; ++j)
                {
                    if (WifiNetworks[i] == db_locations[j])
                    {
                        current_location = db_locations[j];
                        i = location_names.Count;
                        j = db_locations.Count;
                    }
                }
            }


            if (current_location != "$$560091692749045729$#^^%$") 
                response = await request.UpdateLocation(db.Retrieve("Token"), current_location);

            CurrentLocation.Text = "Current location: " + current_location;
            CurrentLocation.RefreshDrawableState();
        }

        private List<string> ConvertJson(IRestResponse response)
        {
            location_names = new List<string>();
            JArray JLocations = JArray.Parse(response.Content);

            foreach (var loc in JLocations)
            {

                //Console.WriteLine(invite.ToString());
                location_names.Add((string)loc["NetName"]);
            }

            return location_names;
        }

        private void AddLocation_Click(object sender, EventArgs e)
        {
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            LocationAddFragment AddLocationDialog = new LocationAddFragment();
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

            //Start the Lists activity
            if (e.Item.ToString() == "Bills")
                this.StartActivity(typeof(Bills));
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

            //loads notifications
            if (item.ToString() == "Notifications")
                this.StartActivity(typeof(Notifications));

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