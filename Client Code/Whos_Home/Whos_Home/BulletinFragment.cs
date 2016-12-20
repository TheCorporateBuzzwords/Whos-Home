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
    class BulletinFragment : DialogFragment
    {
        string title, message;
        TextView Title, Message;
        //overloaded constructor that accepts the title t and message m for the dialog box
        public BulletinFragment(string t, string m)
        {
            title = t;
            message = m;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BulletinDialog, container, false);

            //find the corresponding textview for title and message
            Title = view.FindViewById<TextView>(Resource.Id.textviewBulletinTitleDialog);
            Message = view.FindViewById<TextView>(Resource.Id.textviewBulletinMessageDialog);

            //set the text values to the values received by the constructor
            Title.Text = title;
            Message.Text = message;

            return view;
        }
    }
}