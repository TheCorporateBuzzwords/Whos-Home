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

namespace Whos_Home.Helpers
{
    public class UserLocation
    {
        private string ssid = null;
        private string locationName = null;

        public string SSID
        {
            get
            {
                return SSID;
            }

            set
            {
                SSID = value;
            }
        }

        public string LocationName
        {
            get
            {
                return locationName;
            }

            set
            {
                locationName = value;
            }
        }
    }
}