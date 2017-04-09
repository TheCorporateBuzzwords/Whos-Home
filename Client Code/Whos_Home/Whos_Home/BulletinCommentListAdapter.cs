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
        private List<CommentObj> m_Comments;
        private Activity m_context;

        public BulletinCommentListAdapter(Activity context, List<CommentObj> comments) : base()
        {
            m_Comments = new List<CommentObj>();
            this.m_context = context;
            if(comments.Count != 0)
                m_Comments = comments;
        }
        public override List<CommentObj> this[int position]
        {
            get
            {
                return m_Comments;
            }
        }

        public override int Count
        {
            get
            {
                return m_Comments.Count;
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
                view = m_context.LayoutInflater.Inflate(Resource.Layout.BulletinCommentCustomView, null);

            view.FindViewById<TextView>(Resource.Id.BulletinCommentText1).Text = m_Comments[position].Author;
            view.FindViewById<TextView>(Resource.Id.BulletinCommentText2).Text = m_Comments[position].Message;

            return view;
        }

    }
}