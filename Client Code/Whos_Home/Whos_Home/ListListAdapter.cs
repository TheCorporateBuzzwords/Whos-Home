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
    class ListListAdapter : BaseAdapter<List<ItemObj>>
    {
        private List<ItemObj> m_items;
        private Activity m_context;

        public ListListAdapter(Activity context, List<ItemObj> items)
        {
            m_context = context;
            m_items = items;
        }
        public override List<ItemObj> this[int position]
        {
            get
            {
                return m_items;
            }
        }

        public override int Count
        {
            get
            {
                return m_items.Count;
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
                view = m_context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemChecked, null);

            view.FindViewById<TextView>(Resource.Id.GroupText1).Text = m_items[position].Message;

            return view;
        }
    }
}