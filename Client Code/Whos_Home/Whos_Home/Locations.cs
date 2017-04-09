using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private Button B_AddLocation;
        private TextView m_CurrentLocation;
        private List<string> m_location_names = new List<string>();
        private ListView m_LocationList;
        private List<string> m_db_locations = new List<string>();
        private string m_current_location;
        private List<string> m_WifiNetworks = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Locations);
            InitializeToolbars();
            InitializeFormat();
        }
        private async void InitializeFormat()
        {
            B_AddLocation = FindViewById<Button>(Resource.Id.NewLocationButton);
            B_AddLocation.Click += AddLocation_Click;


            m_LocationList = FindViewById<ListView>(Resource.Id.locationlistview);
            m_CurrentLocation = FindViewById<TextView>(Resource.Id.textCurrentLocation);

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
            }
            catch
            {
                Console.WriteLine("No active group selected: Locations");
            }

            await GetLocations();
            var GroupMemberLocs = await GetActiveUsers();

            m_LocationList.Adapter = new LocationsListAdapter(this, GroupMemberLocs);
        }

        private async Task GetLocations()
        {
            DB_Singleton db = DB_Singleton.Instance;

            RequestHandler request = new RequestHandler(this);

            //get locations from database
            var response = await request.GetLocations(db.Retrieve("Token"), db.GetActiveGroup().GroupID);
            //convert locations from json format
            m_db_locations = ConvertJson(response);
            //add locations into list adapter
            //LocationList.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, db_locations);

            await UpdateLocation();
        }

        private async Task<List<Tuple<string, string>>> GetActiveUsers()
        {
            DB_Singleton db = DB_Singleton.Instance;
            RequestHandler request = new RequestHandler(this);

            List<Tuple<string, string>> tup_list = new List<Tuple<string, string>>();
            var response = await request.GetUserLocations(db.Retrieve("Token"), db.GetActiveGroup().GroupID);

            if (response.Content != "" && response.Content != "[]")
            {
                JArray members = JArray.Parse(response.Content);

                foreach (JToken member in members)
                {
                    string username = (string)member["UserName"];
                    string locationname = (string)member["NetName"];

                    tup_list.Add(new Tuple<string, string>(username, locationname));
                }
            }

            return tup_list;
        }

        public async Task UpdateLocation()
        {
            DB_Singleton db = DB_Singleton.Instance;
            if (db.IsOnline())
            {
                WifiManager wifimanager = (WifiManager)GetSystemService(Context.WifiService);

                string permission = Manifest.Permission.AccessFineLocation;
                if (CheckSelfPermission(permission) == (int)Permission.Granted)
                {
                    var InRange = wifimanager.ScanResults;

                    foreach (ScanResult network in InRange)
                    {
                        m_WifiNetworks.Add(network.Ssid);
                        //WifiNetworkKey.Add(network.Bssid);
                    }
                }

                RequestHandler request = new RequestHandler(this);

                var results = m_WifiNetworks.Intersect(m_db_locations);

                m_current_location = null;
                if (results.Count() != 0)
                    m_current_location = results.ElementAt(0);

                var response = await request.UpdateLocation(db.Retrieve("Token"), m_current_location);

                if (m_current_location != null)
                    m_CurrentLocation.Text = "Current location: " + m_current_location;
                else
                    m_CurrentLocation.Text = "Not in range of location";
                m_CurrentLocation.RefreshDrawableState();
            }
        }

        private List<string> ConvertJson(IRestResponse response)
        {
            m_location_names = new List<string>();
            JArray JLocations = JArray.Parse(response.Content);

            foreach (var loc in JLocations)
            {
                m_location_names.Add((string)loc["SSID"]);
            }

            return m_location_names;
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
            if (e.Item.ToString() == "Lists")
                this.StartActivity(typeof(Lists));

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