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

namespace Whos_Home
{
    class LocationsListAdapter : BaseAdapter<List<Tuple<string, string>>>
    {
        private List<Tuple<string, string>> m_users;
        private Activity m_context;

        public LocationsListAdapter(Activity context, List<Tuple<string, string>> users)
        {
            m_context = context;
            m_users = users;

        }
        public override List<Tuple<string, string>> this[int position]
        {
            get
            {
                return m_users;
            }
        }

        public override int Count
        {
            get
            {
                return m_users.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is supplied
            if (view == null) // otherwise create a new one
                view = m_context.LayoutInflater.Inflate(Resource.Layout.LocationsCustomLayout, null);

            view.FindViewById<TextView>(Resource.Id.LocationsUsername).Text = m_users[position].Item1;

            if (m_users[position].Item2 != null)
            {
                view.FindViewById<TextView>(Resource.Id.LocationsLocation).Text = m_users[position].Item2;
                view.FindViewById<TextView>(Resource.Id.LocationsLocation).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Green));
            }
            else
            {
                view.FindViewById<TextView>(Resource.Id.LocationsLocation).Text = "offline";
                view.FindViewById<TextView>(Resource.Id.LocationsLocation).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
            }



            return view;
        }
    }
}