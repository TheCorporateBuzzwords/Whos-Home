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
    [Activity(Label = "Groups")]
    public class Groups : Activity
    {
        private Button BCreateGroup;
        private ListView listView;
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
            for (int i = 0; i < 5; ++i)
            {
                groupnames.Add("Group " + i.ToString());
                nummembers.Add(i.ToString() + " Members");
            }

            listView = FindViewById<ListView>(Resource.Id.listviewGroups);
            listView.Adapter = new GroupListAdapter(this, groupnames, nummembers);

            //Set itemclick function for when a group is selected
            listView.ItemClick += OnGroupItemClick;

            BCreateGroup = FindViewById<Button>(Resource.Id.NewGroupButton);
            BCreateGroup.Click += BCreateGroup_Click;

        }

        private void BCreateGroup_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            NewGroup Dialog = new NewGroup();
            Dialog.Show(transaction, "dialog fragment new group");
        }

        private void OnGroupItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            GroupSelectDialog Dialog = new GroupSelectDialog();
            Dialog.Show(transaction, "dialog fragment new message");
        }

        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Options";

            //initialize bottom toolbar
            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            editToolbar.InflateMenu(Resource.Menu.edit_menus);
            editToolbar.MenuItemClick += NavigateMenu;
        }

        //Method is used to navigate between activities using the bottom toolbar
        private void NavigateMenu(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            //Start the bulletin activity
            if (e.Item.ToString() == "Bulletins")
                this.StartActivity(typeof(MessageBoard));

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