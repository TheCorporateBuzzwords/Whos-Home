using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using Newtonsoft.Json.Linq;

namespace Whos_Home
{
    [Activity(Label = "Groups")]
    public class Groups : Activity
    {
        private Button B_CreateGroup;
        private ListView m_listView;
        private TextView m_textView;
        private List<string> m_groupnames = new List<string>();
        private List<string> m_nummembers = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Groups);          
            InitializeToolbars();
            InitializeFormat();
        }

        private void InitializeFormat()
        {
            UpdateGroups();

            B_CreateGroup = FindViewById<Button>(Resource.Id.NewGroupButton);
            B_CreateGroup.Click += BCreateGroup_Click;
        }

        public async void UpdateGroups()
        {
            m_groupnames = new List<string>();
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            List<UserGroup> userGroupList = await PullGroups(token);
            foreach (UserGroup group in userGroupList)
            {
                //number of members in each group also needs to be added here
                m_groupnames.Add(group.GroupName);
            }

            m_listView = FindViewById<ListView>(Resource.Id.listviewGroups);
            m_listView.Adapter = new GroupListAdapter(this, m_groupnames, m_nummembers);
            m_textView = FindViewById<TextView>(Resource.Id.textviewGroups);

            try
            { m_textView.Text = "Current Group: " + db.GetActiveGroup().GroupName; }

            catch
            { Console.WriteLine("No active group selected: Groups"); }

            //Set itemclick function for when a group is selected
            m_listView.ItemClick += OnGroupItemClick;
        }

        private async Task<List<UserGroup>> PullGroups(string token)
        {
            RequestHandler request = new RequestHandler(this);
            List<UserGroup> groups = new List<UserGroup>();
            var response = await request.PullGroups(token);

            if ((int)response.StatusCode == 200)
                groups = ParseContent(response.Content);
            
            return groups;
        }

        private List<UserGroup> ParseContent(string content)
        {
            List<UserGroup> groups = new List<UserGroup>();

            JArray arr = JArray.Parse(content);
            foreach(JToken tok in arr)
            {
                string groupname = (string)tok["GroupName"];
                string groupid = (string)tok["GroupID"];

                groups.Add(new UserGroup(groupname, groupid));
            }

            return groups;
        }

        private void BCreateGroup_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            GroupNew Dialog = new GroupNew();
            Dialog.Show(transaction, "dialog fragment new group");
            UpdateGroups();
        }

        private void OnGroupItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            GroupSelectDialog Dialog = new GroupSelectDialog(m_groupnames[position]);
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