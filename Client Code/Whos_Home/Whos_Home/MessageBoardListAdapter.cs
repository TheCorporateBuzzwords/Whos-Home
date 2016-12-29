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

namespace Whos_Home
{
    public class MessageBoardListAdapter : BaseAdapter<List<string>>
    {
        List<string> titles;
        List<string> messages;
        Activity context;

        //overloaded constructor to accept values for the list
        public MessageBoardListAdapter(Activity context, List<string> t, List<string> m) : base()
        {
            this.context = context;
            titles = t;
            messages = m;

            //Reverses list order to display most recent posts first.
            t.Reverse();
            m.Reverse();
        }
       
        public override int Count
        {
            get
            {
                return titles.Count;
            }
        }

        public override List<string> this[int position]
        {
            get
            {
                return titles;
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
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = titles[position];

            string temp;
            //limit the size of the message preview to 100 characters
            if(messages[position].Count<char>() > 100)
            {
               temp = messages[position].Substring(0, 99);
               temp += "...";
               view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = temp;

            }
            else
                view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = messages[position];
            // return the view, populated with data, for display
            return view;
        }

    }
}