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
using Android;
using Android.Content.PM;
using Whos_Home.Helpers;

namespace Whos_Home
{
    class AddLocationFragment : DialogFragment
    {
        ListView NetworkList;
        Button BCancel, BConfirm;
        List<string> WifiNetworks = new List<string>();
        List<string> WifiNetworkKey = new List<string>();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.AddLocation, container, false);

            //Initialize listview and cancel button
            NetworkList = view.FindViewById<ListView>(Resource.Id.locationlistview);
            BCancel = view.FindViewById<Button>(Resource.Id.CancelAddNewLocationButton);


            BCancel.Click += BCancel_Click;
            NetworkList.ItemClick += NetworkList_ItemClick;



             //Set up a wifimanager to gather wifi scan results
             WifiManager wifimanager = (WifiManager)Context.GetSystemService(Context.WifiService);

            //Add each SSID of networks in range to the list of wifi networks
            string permission = Manifest.Permission.AccessFineLocation;
            if (view.Context.CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                var InRange = wifimanager.ScanResults;
                
                foreach (ScanResult network in InRange)
                {
                    WifiNetworks.Add(network.Ssid);
                    WifiNetworkKey.Add(network.Bssid);
                }
            }


            //Places scanned SSID values into the listview
            NetworkList.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleListItem1, WifiNetworks);
            return view;
        }

        private void NetworkList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;


            string selected = WifiNetworks.ElementAt<string>(position);

            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Add " + selected + " to your locations?");

            //Send new location information to database
            alert.SetPositiveButton("Confirm", async (senderAlert, args) =>
            {
                DB_Singleton db = DB_Singleton.Instance;
                var group = db.GetActiveGroup();
                RequestHandler request = new RequestHandler(View.Context);
                
                //adds location to the db using the db_singleton active groupID
                var response = await request.AddLocation(WifiNetworkKey.ElementAt<string>(position), selected, group.GroupID);
            });

            //Close dialog and cancel add
            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                Dismiss();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void BCancel_Click(object sender, EventArgs e)
        {
            //Close the dialog
            Dismiss();
        }
    }
}