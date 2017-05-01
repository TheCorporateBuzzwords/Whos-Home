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
    class BillsNewSpecifyAmountListAdapter : BaseAdapter<List<string>>
    {
        private List<string> m_users;
        private Activity m_context;

        public BillsNewSpecifyAmountListAdapter(Activity Context, List<string> users)
        {
            m_users = users;
            m_context = Context;
        }

        public override List<string> this[int position]
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
                view = m_context.LayoutInflater.Inflate(Resource.Layout.BillsNewSpecifyAmountsCustomView, null);

            view.FindViewById<TextView>(Resource.Id.BillsNewSpecifyAmountCustomViewTextView).Text = "Amount for " + m_users[position] + ":";


            return view;
        }
    }
}