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
    public class List : BaseActivity
    {
        private Button B_NewListItem;
        private ListView m_listView;
        private List<string> m_listItems;
        List<ItemObj> m_ListItemObjs = new List<ItemObj>();
        ListsObj m_list;

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
            B_NewListItem = FindViewById<Button>(Resource.Id.NewListItemButton);
            B_NewListItem.Click += NewListItemButton_Click;
        }

        public async Task UpdateItems()
        {
            m_listItems = new List<string>();

            string test = Intent.GetStringExtra("ListObject");
            string trimmed = Regex.Unescape(test);
            Console.WriteLine(test);
            m_list = new ListsObj().DirtyParse(JToken.Parse(test));

            m_ListItemObjs = await GetItems();

            m_listView = FindViewById<ListView>(Resource.Id.listitemslistview);          
            m_listView.Adapter = new ListListAdapter(this, m_ListItemObjs);   

            //sets the selection mode for the listview to multiple choice
            m_listView.ChoiceMode = Android.Widget.ChoiceMode.Multiple;

            for (int i = 0; i < m_ListItemObjs.Count; ++i)
            {
                if (m_ListItemObjs[i].IsDone == "1")
                    m_listView.SetItemChecked(i, true);
            }

            //Sets the function to be called on click to the custom function OnLocationItemClick
            //This Function will select and deselect location values based on the item clicked.
            m_listView.ItemClick += ListView_ItemClick;
            m_listView.ItemLongClick += ListView_ItemLongClick;
        }

        private void ListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("Are you sure you want to delete this item?");

            alert.SetPositiveButton("Yes", async (senderAlert, args) =>
            {

                DB_Singleton db = DB_Singleton.Instance;

                //TODO Test this when fixed on the server
                var response = await new RequestHandler(this).DeleteListItem(db.Retrieve("Token"), db.GetActiveGroup().GroupID, m_ListItemObjs[e.Position].Id);

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
            m_listItems = new List<string>();

            m_list = JsonConvert.DeserializeObject<ListsObj>(Intent.GetStringExtra("ListObject"));

            m_ListItemObjs = await GetItems();

           //Find listview and set adapter
            m_listView = FindViewById<ListView>(Resource.Id.listitemslistview);
            m_listView.Adapter = new ListListAdapter(this, m_ListItemObjs);

            //sets the selection mode for the listview to multiple choice
            m_listView.ChoiceMode = Android.Widget.ChoiceMode.Multiple;

            for (int i = 0; i < m_ListItemObjs.Count; ++i)
            {
                if (m_ListItemObjs[i].IsDone == "1")
                    m_listView.SetItemChecked(i, true);
            }

            //Sets the function to be called on click to the custom function OnLocationItemClick
            //This Function will select and deselect location values based on the item clicked.
            m_listView.ItemClick += ListView_ItemClick;
        }

        private async Task<List<ItemObj>> GetItems()
        {
            return await new ItemList(m_list.Topicid).UpdateList();
        }
      
        private void NewListItemButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            ListAddItem NewListItemDialog = new ListAddItem(m_list);
            NewListItemDialog.Show(transaction, "dialog fragment new list item");
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            bool selected = listView.IsItemChecked(position);
            m_ListItemObjs.ElementAt(position).IsDone = selected.ToString();
            FireCheckItem(m_ListItemObjs.ElementAt(position).Id, selected);

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
            var response = await request.PutListItem(token, groupid, m_list.Topicid, itemid, selected);
            if ((int)response.StatusCode == 200)
            {
                Toast.MakeText(this, "Item Updated", ToastLength.Long);
            }
            else
            {
                Toast.MakeText(this, "Connection Failed", ToastLength.Long);
            }
        }
    }
}