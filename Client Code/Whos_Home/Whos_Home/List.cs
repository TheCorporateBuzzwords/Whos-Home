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
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Whos_Home.Helpers;
using Whos_Home.Helpers.ListObjects;

namespace Whos_Home
{
    [Activity(Label = "List")]
    public class List : Activity
    {
        private Button NewListItemButton;
        private ListView listView;
        private List<string> listItems;
        List<ItemObj> ListItemObjs = new List<ItemObj>();
        ListsObj list;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.List);
            InitializeToolbars();
            InitializeFormat();
            
        }

        private async void InitializeFormat()
        {
            await UpdateItems();

            //Find button and add click function
            NewListItemButton = FindViewById<Button>(Resource.Id.NewListItemButton);
            NewListItemButton.Click += NewListItemButton_Click;

            //Find listview and set adapter
        }

        public async Task UpdateItems()
        {
            listItems = new List<string>();

            string test = Intent.GetStringExtra("ListObject");
            string trimmed = Regex.Unescape(test);
            Console.WriteLine(test);
            list = new ListsObj().DirtyParse(JToken.Parse(test));
            //sample code to retrieve list object from lists.cs
            //list = JsonConvert.DeserializeObject<ListsObj>(Intent.GetStringExtra("ListObject"));

            ListItemObjs = await GetItems();

            listView = FindViewById<ListView>(Resource.Id.listitemslistview);
            
            listView.Adapter = new ListListAdapter(this, ListItemObjs);

            //sets the selection mode for the listview to multiple choice
            listView.ChoiceMode = Android.Widget.ChoiceMode.Multiple;

            //Sets the function to be called on click to the custom function OnLocationItemClick
            //This Function will select and deselect location values based on the item clicked.

            listView.ItemClick += ListView_ItemClick;
            listView.ItemLongClick += ListView_ItemLongClick;
        }

        private void ListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("Are you sure you want to delete this item?");

            alert.SetPositiveButton("Yes", async (senderAlert, args) =>
            {

                DB_Singleton db = DB_Singleton.Instance;

                //TODO Test this when fixed on the server
                var response = await new RequestHandler(this).DeleteListItem(db.Retrieve("Token"), db.GetActiveGroup().GroupID, ListItemObjs[e.Position].Id);

                if ((int)response.StatusCode == 200)
                {
                    Toast.MakeText(this, "Succesfully Deleted", ToastLength.Long);
                    await UpdateItems();
                }
                else
                    Toast.MakeText(this, "Error Deleting", ToastLength.Long);

                await GetItems();
            });

            alert.SetNegativeButton("No", (senderAlert, args) => { });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        public async void UpdateListView()
        {
            listItems = new List<string>();

            //ListItemObjs = await GetItems();
            //sample code to retrieve list object from lists.cs
            list = JsonConvert.DeserializeObject<ListsObj>(Intent.GetStringExtra("ListObject"));

            ListItemObjs = await GetItems();

           //Find listview and set adapter
            listView = FindViewById<ListView>(Resource.Id.listitemslistview);
            listView.Adapter = new ListListAdapter(this, ListItemObjs);

            //sets the selection mode for the listview to multiple choice
            listView.ChoiceMode = Android.Widget.ChoiceMode.Multiple;

            //Sets the function to be called on click to the custom function OnLocationItemClick
            //This Function will select and deselect location values based on the item clicked.

            listView.ItemClick += ListView_ItemClick;
        }

        private async Task<List<ItemObj>> GetItems()
        {
            return await new ItemList(list.Topicid).UpdateList();
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
            ListItemObjs.ElementAt(position).IsDone = selected.ToString();
            FireCheckItem(ListItemObjs.ElementAt(position).Id, selected);

            listView.SetItemChecked(position, selected);
        }

        //This may need to change. If selecting multiple times is an issue
        private async void FireCheckItem(string itemid, bool selected)
        {
            List<ItemObj> groupLists = new List<ItemObj>();
            RequestHandler request = new RequestHandler(this);
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;
            var response = await request.PutListItem(token, groupid, list.Topicid, itemid, selected);
            if ((int)response.StatusCode == 200)
            {
                Toast.MakeText(this, "Item Updated", ToastLength.Long);
            }
            else
            {
                Toast.MakeText(this, "Connection Failed", ToastLength.Long);
            }
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