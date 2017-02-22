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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;

using Whos_Home.Helpers;

namespace Whos_Home
{
    [Activity(Label = "List")]
    public class List : Activity
    {
        private Button NewListItemButton;
        private ListView listView;
        private List<string> listItems;
        ListsObj list;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.List);
            InitializeFormat();
            InitializeToolbars();
        }

        private void InitializeFormat()
        {
            listItems = new List<string>();


            //sample code to retrieve list object from lists.cs
            list = JsonConvert.DeserializeObject<ListsObj>(Intent.GetStringExtra("ListObject"));

            //Find button and add click function
            NewListItemButton = FindViewById<Button>(Resource.Id.NewListItemButton);
            NewListItemButton.Click += NewListItemButton_Click;

            //Find listview and set adapter
            listView = FindViewById<ListView>(Resource.Id.listitemslistview);
            listView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItemMultipleChoice, listItems);

            //sets the selection mode for the listview to multiple choice
            listView.ChoiceMode = Android.Widget.ChoiceMode.Multiple;

            //Sets the function to be called on click to the custom function OnLocationItemClick
            //This Function will select and deselect location values based on the item clicked.
            listView.ItemClick += ListView_ItemClick;
        }
        private async Task<List<ItemObj>> GetItems()
        {
            RequestHandler request = new RequestHandler(this);
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;
            var response = await request.GetListItems(token, groupid, list.Topicid);
            if ((int)response.StatusCode == 200)
            {
                Toast.MakeText(this, "Item Posted", ToastLength.Long);
            }
            else
            {
                Toast.MakeText(this, "Post Failed", ToastLength.Long);

            }
            JArray preParse = JArray.Parse(response.Content);

            List<ItemObj> groupLists = ParseToLists(preParse);

            return groupLists;

        }
        private List<ItemObj> ParseToLists(JArray jarr)
        {
            List<ItemObj> postParse = new List<ItemObj>();

            foreach (JToken tok in jarr)
            {
                string posttime = (string)tok["PostTime"];
                string username = (string)tok["UserName"];
                string title = (string)tok["Title"];
                string isdone = (string)tok["Done"];


                postParse.Add(new ItemObj(username, posttime, title, bool.Parse(isdone)));
            }

            return postParse;
        }
        private void NewListItemButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            ListAddItem NewListItemDialog = new ListAddItem(list);
            NewListItemDialog.Show(transaction, "dialog fragment new list item");
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            bool selected = listView.IsItemChecked(position);

            if (selected)
                listView.SetItemChecked(position, true);
            else
                listView.SetItemChecked(position, false);
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