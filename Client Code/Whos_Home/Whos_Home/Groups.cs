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
using Newtonsoft.Json;

namespace Whos_Home
{
    [Activity(Label = "Groups")]
    public class Groups : Activity
    {
        private Button BCreateGroup;
        private ListView listView;
        private TextView textView;
        private List<string> groupnames = new List<string>();
        private List<string> nummembers = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Groups);          
            InitializeToolbars();
            InitializeFormat();
        }

        private void InitializeFormat()
        {
            //create mock group names

            DB_Singleton db = DB_Singleton.Instance;
            List<UserGroup> userGroupList = db.GetUserGroups();
            foreach(UserGroup group in userGroupList)
            {
                //number of members in each group also needs to be added here
                groupnames.Add(group.GroupName);
            }

            listView = FindViewById<ListView>(Resource.Id.listviewGroups);
            listView.Adapter = new GroupListAdapter(this, groupnames, nummembers);
            textView = FindViewById<TextView>(Resource.Id.textviewGroups);

            try
            {
                textView.Text = "Current Group: " + db.GetActiveGroup().GroupName;
            }
            catch
            {
                Console.WriteLine("No active group selected: Groups");
            }
            //Set itemclick function for when a group is selected
            listView.ItemClick += OnGroupItemClick;

            BCreateGroup = FindViewById<Button>(Resource.Id.NewGroupButton);
            BCreateGroup.Click += BCreateGroup_Click;

        }

        private void BCreateGroup_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            GroupNew Dialog = new GroupNew();
            Dialog.Show(transaction, "dialog fragment new group");
        }

        private void OnGroupItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            GroupSelectDialog Dialog = new GroupSelectDialog(groupnames[position]);
            Dialog.Show(transaction, "dialog fragment new message");
        }

        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Groups";


            //initialize bottom toolbar
            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            //editToolbar.Title = "Navigate";
            editToolbar.InflateMenu(Resource.Menu.edit_menus);
            editToolbar.MenuItemClick += NavigateMenu;

            //(sender, e) => {
            //Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            //};


        }

        //Method is used to navigate between activities using the bottom toolbar
        private void NavigateMenu(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            //Start the bulletin activity
            if (e.Item.ToString() == "Bulletins")
                this.StartActivity(typeof(BulletinBoard));

            //Start the Locations activity
            if (e.Item.ToString() == "Locations")
                this.StartActivity(typeof(Locations));

            //Start the Lists activity
            if (e.Item.ToString() == "Lists")
                this.StartActivity(typeof(Lists));

            //Start the Lists activity
            if (e.Item.ToString() == "Bills")
                this.StartActivity(typeof(Bills));
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

            //loads notifications
            if (item.ToString() == "Notifications")
                this.StartActivity(typeof(Notifications));

            //Loads settings menu if preferences is selected
            if (item.ToString() == "Preferences")
                this.StartActivity(typeof(SettingsMenu));

            //Loads Groups menu if selected
            if (item.ToString() == "Groups")
                this.StartActivity(typeof(Groups));

            return base.OnOptionsItemSelected(item);
        }
    }
}