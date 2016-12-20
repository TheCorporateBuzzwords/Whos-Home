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
    [Activity(Label = "Bulletin")]
    public class Bulletin : Activity
    {
        ListView messagelistview, commentlistview;
        TextView message;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Bulletin);
            InitializeToolbars();
            InitializeFormat();
        }

        private void InitializeFormat()
        {
            string msg = "Message a;lskdfjl;asjdfl;as;kdfjl;asjdf;kajs;fkjas;kdfj;alskjdflkasjdf;kajsa;lskdfjl;asjdfl;as;kdfjl;asjdf;kajs;fkjas;kdfj;alskjdflkasjdf;kajsa;lskdfjl;asjdfl;as;kdfjl;asjdf;kajs;fkjas;kdfj;alskjdflkasjdf;kajsa;lskdfjl;asjdfl;as;kdfjl;asjdf;kajs;fkjas;kdfj;alskjdflkasjdf;kajsa;lskdfjl;asjdfl;as;kdfjl;asjdf;kajs;fkjas;kdfj;alskjdflkasjdf;kajsa;lskdfjl;asjdfl;as;kdfjl;asjdf;kajs;fkjas;kdfj;alskjdflkasjdf;kajsa;lskdfjl;asjdfl;as;kdfjl;asjdf;kajs;fkjas;kdfj;alskjdflkasjdf;kajsa;lskdfjl;asjdfl;as;kdfjl;asjdf;kajs;fkjas;kdfj;alskjdflkasjdf;kajsa;lskdfjl;asjdfl;as;kdfjl;asjdf;kajs;fkjas;kdfj;alskjdflkasjdf;kajsa;lskdfjl;asjdfl;as;kdfjl;asjdf;kajs;fkjas;kdfj;alskjdflkasjdf;kajs";
            List<string> comments = new List<string>();

            //generate fake comments
            for (int i = 1; i < 16; ++i)
            {
                comments.Add("Comment " + i.ToString());
            }

            //find the two views for message body and comment listview
            message = FindViewById<TextView>(Resource.Id.textviewBulletinMessage);
            commentlistview = FindViewById<ListView>(Resource.Id.BulletinCommentsListView);

            //set values for testing
            message.Text = msg;
            commentlistview.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, comments);

        }

        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Options";

            //initialize bottom toolbar
            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            //editToolbar.Title = "Navigate";
            editToolbar.InflateMenu(Resource.Menu.edit_menus);
            editToolbar.MenuItemClick += (sender, e) => {
                Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            };
        }

        //called to specify menu resources for an activity
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        //called when a menu item is tapped
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }
    }
}