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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Whos_Home.Helpers;

namespace Whos_Home
{
    [Activity(Label = "Bills")]
    class Bills : Activity
    {
        private ListView listview;
        private Button BNewBill, BillsHistory, CurrentBills, CreateGraph;
        private List<BillObj> all_bill_objs = new List<BillObj>();
        private List<BillObj> user_bill_objs;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Bills);

            InitializeFormat();
            InitializeToolbars();
        }

        private async void InitializeFormat()
        {
            //Set new bill button
            BNewBill = FindViewById<Button>(Resource.Id.buttonNewBill);
            BNewBill.Click += BNewBill_Click;

            BillsHistory = FindViewById<Button>(Resource.Id.buttonBillHistory);
            BillsHistory.Click += BillsHistory_Click; ;

            CurrentBills = FindViewById<Button>(Resource.Id.buttonCurrentBills);

            CreateGraph = FindViewById<Button>(Resource.Id.buttonBillGraph);
            CreateGraph.Click += CreateGraph_Click;

            listview = FindViewById<ListView>(Resource.Id.listviewBills);

            await UpdateAllBills();
            CurrentBills.Click += CurrentBills_Click;
            //  CurrentBills.LongClick += CurrentBills_LongClick;


        }

        private void CreateGraph_Click(object sender, EventArgs e)
        {
            List<Tuple<string, float>> graph_vals = new List<Tuple<string, float>>();
            float other = 0;
            float rent = 0;
            float utilities = 0;
            float groceries = 0;

            foreach (BillObj bill in all_bill_objs)
            {
                switch(bill.Categoryid)
                {
                    case "1":
                        other += Convert.ToSingle(bill.Amount);
                        break;

                    case "2":
                        rent += Convert.ToSingle(bill.Amount);
                        break;

                    case "3":
                        utilities += Convert.ToSingle(bill.Amount);
                        break;

                    case "4":
                        groceries += Convert.ToSingle(bill.Amount);
                        break;

                    default:
                        Console.WriteLine("INVALID CATEGORY ID");
                        break;

                }
            }
            if(other != 0)
                graph_vals.Add(new Tuple<string, float>("Other", other));
            if(rent != 0)
                graph_vals.Add(new Tuple<string, float>("Rent", rent));
            if(utilities != 0)
                graph_vals.Add(new Tuple<string, float>("Utilities", utilities));
            if(groceries != 0)
                graph_vals.Add(new Tuple<string, float>("Groceries", groceries));


            Intent i = new Intent(Application.Context, typeof(BillsGraph));


            i.PutExtra("BillsList", JsonConvert.SerializeObject(graph_vals));

            StartActivity(i);
        }

        public async Task UpdateAllBills()
        {
            DB_Singleton db = DB_Singleton.Instance;
            RequestHandler request = new RequestHandler(this);
            var response = await request.GetBills(db.Retrieve("Token"), db.GetActiveGroup().GroupID);

            Console.WriteLine("!!!!!!!!!!!!!!!!!!!BILLS!!!!!!!!!!!!!!!!!!!!!!");
            JArray allbills = JArray.Parse(response.Content);
            foreach(JToken token in allbills)
            {
                all_bill_objs.Add(new BillObj(token));
                Console.WriteLine(new BillObj(token).ToString());
            }

            listview.Adapter = new BillsListAdapter(this, all_bill_objs);
        }
        /*
        public async Task UpdateUserBills()
        {
            DB_Singleton db = DB_Singleton.Instance;
            RequestHandler request = new RequestHandler(this);
           // var response = request.GetBills(db.Retrieve("Token"), db.Retrieve("Username"));
            
            
        }
        */

        void PushNewUsserList(string content)
        {
            if (content != "[]" && content != "")
            {
                JArray all_objs = JArray.Parse(content);
                user_bill_objs = new List<BillObj>();
                foreach(JToken tok in all_objs)
                {
                    //user_bill_objs.Add(new BillObj(tok[""]))
                }
            }
        }

        private void CurrentBills_LongClick(object sender, View.LongClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CurrentBills_Click(object sender, EventArgs e)
        {
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            BillsGraphMonth NewBillDialog = new BillsGraphMonth();
            NewBillDialog.Show(transaction, "dialog fragment bills graph month");
        }

        private void BillsHistory_Click(object sender, EventArgs e)
        {
            List<Tuple<string, float>> bills = new List<Tuple<string, float>>();

            bills.Add(new Tuple<string, float>("Rent", 500));
            bills.Add(new Tuple<string, float>("Groceries", 100));
            bills.Add(new Tuple<string, float>("Utilities", 60));
            bills.Add(new Tuple<string, float>("Other", 150));


            Intent i = new Intent(Application.Context, typeof(BillsGraph));


            i.PutExtra("BillsList", JsonConvert.SerializeObject(bills));

            StartActivity(i);

            //this.StartActivity(typeof(BillsGraph));
        }

        private void BNewBill_Click(object sender, EventArgs e)
        {
            
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            BillsNew NewBillDialog = new BillsNew();
            NewBillDialog.Show(transaction, "dialog fragment create new bill");
        }

        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Bulletins";


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