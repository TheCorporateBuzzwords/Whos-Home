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

namespace Whos_Home
{
    [Activity(Label = "Bills")]
    class Bills : Activity
    {
        private Button BNewBill, BillsHistory, CurrentBills;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Bills);

            InitializeFormat();
            InitializeToolbars();
        }

        private void InitializeFormat()
        {
            //Set new bill button
            BNewBill = FindViewById<Button>(Resource.Id.buttonNewBill);
            BNewBill.Click += BNewBill_Click;

            BillsHistory = FindViewById<Button>(Resource.Id.buttonBillHistory);
            BillsHistory.Click += BillsHistory_Click; ;

            CurrentBills = FindViewById<Button>(Resource.Id.buttonCurrentBills);
            CurrentBills.Click += CurrentBills_Click;
            CurrentBills.LongClick += CurrentBills_LongClick;

        }

        private void CurrentBills_LongClick(object sender, View.LongClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CurrentBills_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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