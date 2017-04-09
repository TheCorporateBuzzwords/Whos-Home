using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net.Wifi;
using Android;
using Android.Content.PM;
using Whos_Home.Helpers;

using RestSharp;

namespace Whos_Home
{
    class LocationAddFragment : DialogFragment
    {
        ListView m_NetworkList;
        Button B_Cancel;
        List<string> WifiNetworks = new List<string>();
        List<string> WifiNetworkKey = new List<string>();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.AddLocation, container, false);

            //Initialize listview and cancel button
            m_NetworkList = view.FindViewById<ListView>(Resource.Id.locationlistview);
            B_Cancel = view.FindViewById<Button>(Resource.Id.CancelAddNewLocationButton);


            B_Cancel.Click += BCancel_Click;
            m_NetworkList.ItemClick += NetworkList_ItemClick;

             //Set up a wifimanager to gather wifi scan results
             WifiManager wifimanager = (WifiManager)Context.GetSystemService(Context.WifiService);

            //Add each SSID of networks in range to the list of wifi networks
            string permission = Manifest.Permission.AccessFineLocation;
            if (view.Context.CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                var InRange = wifimanager.ScanResults;
                
                foreach (ScanResult network in InRange)
                {
                    //if network has not been displayed
                    if (!WifiNetworks.Contains(network.Ssid) && network.Ssid != "")
                    {
                        WifiNetworks.Add(network.Ssid);
                        WifiNetworkKey.Add(network.Bssid);
                    }
                }
            }

            //Places scanned SSID values into the listview
            m_NetworkList.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleListItem1, WifiNetworks);
            return view;
        }

        private async void NetworkList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            string selected = WifiNetworks.ElementAt<string>(position);

            //Send new location information to database

            DB_Singleton db = DB_Singleton.Instance;
            var group = db.GetActiveGroup();

            //adds location to the db using the db_singleton active groupID
            FragmentTransaction transaction = FragmentManager.BeginTransaction();

            List<string> locations = await GetLocations();
            if (locations.Contains(selected))
                SsidTaken();
            else
            {
                LocationAddName AddLocationNameDialog = new LocationAddName(db.Retrieve("Token"), selected, group.GroupID);
                AddLocationNameDialog.Show(transaction, "dialog fragment AddLocationName");
            }
        }

        private void BCancel_Click(object sender, EventArgs e)
        {
            //Close the dialog
            Dismiss();
        }

        void SsidTaken()
        {
            //PUT YOUR ERROR ALERT HERE
            Toast.MakeText(Context, "SSID has already been assigned in this group", ToastLength.Long);
        }

        private async Task<List<string>> GetLocations()
        {
            DB_Singleton db = DB_Singleton.Instance;

            RequestHandler request = new RequestHandler(Context);

            //get locations from database
            var response = await request.GetLocations(db.Retrieve("Token"), db.GetActiveGroup().GroupID);
            //convert locations from json format
            List<string> db_locations = ConvertJson(response);
            //add locations into list adapter
            //LocationList.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, db_locations);

            ((Locations)Activity).UpdateLocation();

            return db_locations;
        }

        private List<string> ConvertJson(IRestResponse response)
        {
            List<string>location_names = new List<string>();
            JArray JLocations = JArray.Parse(response.Content);

            foreach (var loc in JLocations)
            {
                location_names.Add((string)loc["SSID"]);
            }

            return location_names;
        }
    }
}