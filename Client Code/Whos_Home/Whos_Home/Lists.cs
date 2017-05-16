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
using Whos_Home.Helpers.ListObjects;
using Newtonsoft.Json;

namespace Whos_Home
{
    [Activity(Label = "Lists")]
    public class Lists : BaseActivity
    {
        private Button B_NewList;
        private ListView m_listView;
        List<ListsObj> m_listoflists;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Lists);

            InitializeToolbars();
            InitializeFormat();

            tab3Button.SetColorFilter(selectedColor);
            ActionBar.Title = "Lists";
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
            B_NewList = FindViewById<Button>(Resource.Id.NewListButton);
            B_NewList.Click += NewListButton_Click;
        }

        public async Task UpdateLists()
        {
            //TODO: Add this behavior to all List Objects in activities.
            //Simplifies from a dozen lines to one
            m_listoflists = await new ListList().UpdateList();

            //find listview and set adapter and click function
            m_listView = FindViewById<ListView>(Resource.Id.listlistview);
            m_listView.Adapter = new ListsListAdapter(this, m_listoflists);
            m_listView.ItemClick += ListView_ItemClick;
            m_listView.ItemLongClick += ListView_LongClick; 
        }

        private void ListView_LongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("Are you sure you want to delete this list?");

            alert.SetPositiveButton("Yes", async (senderAlert, args) =>
            {

                DB_Singleton db = DB_Singleton.Instance;
                
                //TODO Test this when fixed on the server
                var response = await new RequestHandler(this).DeleteList(db.Retrieve("Token"), db.GetActiveGroup().GroupID, m_listoflists[e.Position].Topicid);

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

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            //creates an intent for a List activity
            Intent i = new Intent(this, typeof(List));
            string testserial = JsonConvert.SerializeObject(m_listoflists[position]);
            Console.WriteLine(testserial);
            //sample code to put a list object into the intent
            i.PutExtra("ListObject", JsonConvert.SerializeObject(m_listoflists[position]));

            StartActivity(i);
        }
    }
}