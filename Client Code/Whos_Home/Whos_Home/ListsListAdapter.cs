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
    class ListsListAdapter : BaseAdapter<List<ListsObj>>
    {

        private List<ListsObj> m_lists;
        private Activity context;

        public ListsListAdapter(Activity context, List<ListsObj> lists) : base()
        {
            this.context = context;
            m_lists = lists;
        }
        public override List<ListsObj> this[int position]
        {
            get
            {
                return m_lists;
            }
        }

        public override int Count
        {
            get
            {
                return m_lists.Count;
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
                view = context.LayoutInflater.Inflate(Resource.Layout.CustomGroupView, null);

            view.FindViewById<TextView>(Resource.Id.GroupText1).Text = m_lists[position].Title;

            view.FindViewById<TextView>(Resource.Id.GroupText2).Text = m_lists[position].Date;
           
            return view;
        }
    }
}