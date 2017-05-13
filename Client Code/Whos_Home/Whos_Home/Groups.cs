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
using Android.Graphics;

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
            ActionBar.Title = "Notifications";

            InitializeTabs();
        }

        //called to specify menu resources for an activity
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private ImageButton tab1Button, tab2Button, tab3Button, tab4Button;
        private TextView tab1Text, tab2Text, tab3Text, tab4Text, headingText;
        private Color selectedColor, deselectedColor;

        void InitializeTabs()
        {

            tab1Button = FindViewById<ImageButton>(Resource.Id.tab1_icon);
            tab2Button = this.FindViewById<ImageButton>(Resource.Id.tab2_icon);
            tab3Button = this.FindViewById<ImageButton>(Resource.Id.tab3_icon);
            tab4Button = this.FindViewById<ImageButton>(Resource.Id.tab4_icon);

            tab1Text = this.FindViewById<TextView>(Resource.Id.tab1_text);
            tab2Text = this.FindViewById<TextView>(Resource.Id.tab2_text);
            tab3Text = this.FindViewById<TextView>(Resource.Id.tab3_text);
            tab4Text = this.FindViewById<TextView>(Resource.Id.tab3_text);

            selectedColor = Resources.GetColor(Resource.Color.theme_blue);
            deselectedColor = Resources.GetColor(Resource.Color.white);

            deselectAll();

            tab1Button.Click += delegate {
                showTab1();
            };

            tab2Button.Click += delegate {
                showTab2();
            };

            tab3Button.Click += delegate {
                showTab3();
            };

            tab4Button.Click += delegate {
                showTab4();
            };
        }

        private void deselectAll()
        {
            tab1Button.SetColorFilter(deselectedColor);
            tab2Button.SetColorFilter(deselectedColor);
            tab3Button.SetColorFilter(deselectedColor);
            tab4Button.SetColorFilter(deselectedColor);

            tab1Text.SetTextColor(deselectedColor);
            tab2Text.SetTextColor(deselectedColor);
            tab3Text.SetTextColor(deselectedColor);
            tab4Text.SetTextColor(deselectedColor);

        }

        private void showTab1()
        {
            deselectAll();

            tab1Button.SetColorFilter(selectedColor);
            tab1Text.SetTextColor(selectedColor);

            this.StartActivity(typeof(Locations));
        }

        private void showTab2()
        {
            deselectAll();

            tab2Button.SetColorFilter(selectedColor);
            tab2Text.SetTextColor(selectedColor);

            this.StartActivity(typeof(BulletinBoard));
        }

        private void showTab3()
        {
            deselectAll();

            tab3Button.SetColorFilter(selectedColor);
            tab3Text.SetTextColor(selectedColor);

            this.StartActivity(typeof(Lists));
        }

        private void showTab4()
        {
            deselectAll();

            tab4Button.SetColorFilter(selectedColor);
            tab4Text.SetTextColor(selectedColor);

            this.StartActivity(typeof(Bills));
        }
    }
}