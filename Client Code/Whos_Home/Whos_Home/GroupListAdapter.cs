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
    class GroupListAdapter : BaseAdapter<List<string>>
    {

        private List<string> groupName;
        private List<string> numMembers;
        private Activity context;

        public GroupListAdapter(Activity context, List<string> groupname, List<string> nummembers) : base()
        {
            this.context = context;
            groupName = groupname;
            numMembers = nummembers;
        }
        public override List<string> this[int position]
        {
            get
            {
                return groupName;
            }
        }

        public override int Count
        {
            get
            {
                return groupName.Count;
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

            //crashes here without this if statement not sure why
            
                view.FindViewById<TextView>(Resource.Id.GroupText1).Text = groupName[position];
            if (numMembers.Count == groupName.Count)
                view.FindViewById<TextView>(Resource.Id.GroupText2).Text = numMembers[position].ToString();
            else
                view.FindViewById<TextView>(Resource.Id.GroupText2).Text = "Error loading members";





            return view;
        }
    }
}