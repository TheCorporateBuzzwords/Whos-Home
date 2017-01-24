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
    class BulletinCommentListAdapter : BaseAdapter<List<string>>
    {
        private List<string> Usernames;
        private List<string> Comments;
        private Activity context;

        public BulletinCommentListAdapter(Activity context, List<string> usernames, List<string> comments) : base()
        {
            this.context = context;
            Usernames = usernames;
            Comments = comments;
        }
        public override List<string> this[int position]
        {
            get
            {
                return Usernames;
            }
        }

        public override int Count
        {
            get
            {
                return Usernames.Count;
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

            view.FindViewById<TextView>(Resource.Id.GroupText1).Text = Usernames[position];
            view.FindViewById<TextView>(Resource.Id.GroupText2).Text = Comments[position].ToString();

            return view;
        }

    }
}