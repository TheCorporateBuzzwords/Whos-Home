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
using Whos_Home.Helpers;

namespace Whos_Home
{
    [Activity(Label = "Groups")]
    public class Groups : BaseActivity
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
            ActionBar.Title = "Groups";
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
    }
}