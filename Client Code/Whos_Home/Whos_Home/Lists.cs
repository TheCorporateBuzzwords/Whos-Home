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

using Newtonsoft;
using Newtonsoft.Json.Linq;

using Whos_Home.Helpers;
using Newtonsoft.Json;

namespace Whos_Home
{
    [Activity(Label = "Lists")]
    public class Lists : Activity
    {
        private Button NewListButton;
        private ListView listView;
        List<ListsObj> listoflists;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Lists);
            InitializeToolbars();
            InitializeFormat();
        }

        private void NewListButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            ListNew NewListDialog = new ListNew();
            NewListDialog.Show(transaction, "dialog fragment new list");
        }

        public void InitializeFormat()
        {
            UpdateLists();
            //find button and assign click function
            NewListButton = FindViewById<Button>(Resource.Id.NewListButton);
            NewListButton.Click += NewListButton_Click;
            


        }

        public async Task UpdateLists()
        {
            //listnames = new List<string>();
            //remaining_items = new List<string>();
            listoflists = await GetLists();

            //find listview and set adapter and click function
            listView = FindViewById<ListView>(Resource.Id.listlistview);
            listView.Adapter = new ListsListAdapter(this, listoflists);
            listView.ItemClick += ListView_ItemClick;
            listView.ItemLongClick += ListView_LongClick; 
        }

        private void ListView_LongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("Are you sure you want to delete this list?");

            alert.SetPositiveButton("Yes", async (senderAlert, args) =>
            {

                DB_Singleton db = DB_Singleton.Instance;
                
                //TODO Test this when fixed on the server
                var response = await new RequestHandler(this).DeleteList(db.Retrieve("Token"), db.GetActiveGroup().GroupID, listoflists[e.Position].Topicid);

                if ((int)response.StatusCode == 200)
                    Toast.MakeText(this, "Succesfully Deleted", ToastLength.Long);
                else
                    Toast.MakeText(this, "Error Deleting", ToastLength.Long);

                await UpdateLists();
            });

            alert.SetNegativeButton("No", (senderAlert, args) => { });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private async Task<List<ListsObj>> GetLists()
        {
            RequestHandler request = new RequestHandler(this);
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;
            var response = await request.GetLists(token, groupid);
            JArray preParse = JArray.Parse(response.Content);

            List<ListsObj> groupLists = ParseToLists(preParse);

            return groupLists;
            
        }

        private List<ListsObj> ParseToLists(JArray jarr)
        {
            List<ListsObj> postParse = new List<ListsObj>();

            foreach(JToken tok in jarr)
            {
                string posttime = (string)tok["PostTime"];
                string username = (string)tok["UserName"];
                string title = (string)tok["Title"];
                string listid = (string)tok["ListID"];
                string firstname = (string)tok["FirstName"];
                string lastname = (string)tok["LastName"];

                postParse.Add(new ListsObj(posttime, username, title, listid, firstname, lastname));
            }

            return postParse; 
        }
        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            //creates an intent for a List activity
            Intent i = new Intent(Application.Context, typeof(List));

            //sample code to put a list object into the intent
            i.PutExtra("ListObject", JsonConvert.SerializeObject(listoflists[position]));

            StartActivity(i);

        }

        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Lists";


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