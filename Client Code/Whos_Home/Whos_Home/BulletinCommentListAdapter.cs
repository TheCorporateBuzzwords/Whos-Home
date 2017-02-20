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
    class BulletinCommentListAdapter : BaseAdapter<List<CommentObj>>
    {
        private List<CommentObj> Comments;
        private Activity context;

        public BulletinCommentListAdapter(Activity context, List<CommentObj> comments) : base()
        {
            this.context = context;
            Comments = comments;
        }
        public override List<CommentObj> this[int position]
        {
            get
            {
                return Comments;
            }
        }

        public override int Count
        {
            get
            {
                return Comments.Count;
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
                view = context.LayoutInflater.Inflate(Resource.Layout.BulletinCommentCustomView, null);

            view.FindViewById<TextView>(Resource.Id.BulletinCommentText1).Text = Comments[position].Author;
            view.FindViewById<TextView>(Resource.Id.BulletinCommentText2).Text = Comments[position].Message;

            return view;
        }

    }
}