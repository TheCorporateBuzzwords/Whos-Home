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
    class BillsListAdapter : BaseAdapter<List<BillObj>>
    {
        private List<BillObj> Bills;
        private Activity context;

        public BillsListAdapter(Activity context, List<BillObj> bills)
        {
            this.context = context;
            Bills = bills;
        }
        public override List<BillObj> this[int position]
        {
            get
            {
                return Bills;
            }
        }

        public override int Count
        {
            get
            {
                return Bills.Count;
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
                view = context.LayoutInflater.Inflate(Resource.Layout.BillsCustomView, null);

            view.FindViewById<TextView>(Resource.Id.BillsCustomBillTitle).Text = Bills[position].Title;
            view.FindViewById<TextView>(Resource.Id.BillsCustomAmount).Text = "$" + Bills[position].Amount;

            view.FindViewById<TextView>(Resource.Id.BillsCustomBillDate).Text = "Due: " + Bills[position].Date.ToShortDateString();

            if (Bills[position].Date <= DateTime.Now)
                view.FindViewById<TextView>(Resource.Id.BillsCustomBillDate).SetTextColor(Android.Graphics.Color.Red);
            else
                view.FindViewById<TextView>(Resource.Id.BillsCustomBillDate).SetTextColor(Android.Graphics.Color.Green);



            return view;
        }
    }
}