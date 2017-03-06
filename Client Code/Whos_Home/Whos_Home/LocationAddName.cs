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
    class LocationAddName : DialogFragment
    {
        private Button Confirm;
        private Button Cancel;
        private EditText loc_name;
        private string invitee;
        private string m_token, m_ssid, m_location_name, m_group_id;

        public LocationAddName(string token, string ssid, string group_id)
        {
            m_token = token;
            m_ssid = ssid;
            m_group_id = group_id;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.LocationAddName, container, false);

            Confirm = view.FindViewById<Button>(Resource.Id.buttonConfirmLocationAddName);
            Cancel = view.FindViewById<Button>(Resource.Id.buttonCancelLocationAddName);
            loc_name = view.FindViewById<EditText>(Resource.Id.edittextLocationAddName);

            //add click functions for buttons
            Confirm.Click += Confirm_Click;
            Cancel.Click += Cancel_Click;

            return view;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private async void Confirm_Click(object sender, EventArgs e)
        {
            RequestHandler request = new RequestHandler(View.Context);

            RestSharp.IRestResponse response;
            if (loc_name.Text != "")
            {
                response = await request.AddLocation(m_token, m_ssid, loc_name.Text, m_group_id);
                Dismiss();
            }
            else
            {
                //display an error
                Console.WriteLine("Error: location not named");
            }

        }
    }
}