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
using Java.Lang;
using Whos_Home.Helpers;

namespace Whos_Home
{
    public class BulletinListAdapter : BaseAdapter<List<BulletinPostObj>>
    {
        private List<BulletinPostObj> m_posts;
        private Activity context;

        //overloaded constructor to accept values for the list
        public BulletinListAdapter(Activity p_context, List<BulletinPostObj> posts) : base()
        {
            this.context = p_context;
            m_posts = posts;
        }
       
        public override int Count
        {
            get
            {
                return m_posts.Count;
            }
        }

        public override List<BulletinPostObj> this[int position]
        {
            get
            {
                return m_posts;
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
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);

            // set view properties to reflect data for the given row
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = m_posts[position].Title;

            string temp;
            //limit the size of the message preview to 100 characters
            if(m_posts[position].Message.Count<char>() > 100)
            {
               temp = m_posts[position].Message.Substring(0, 99);
               temp += "...";
               view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = temp;

            }
            else
                view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = m_posts[position].Message;
            // return the view, populated with data, for display
            return view;
        }


    }
}