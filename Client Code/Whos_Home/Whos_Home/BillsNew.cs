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


using Whos_Home.Helpers;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Whos_Home
{
    class BillsNew : DialogFragment
    {
        private ListView m_listview;
        private CheckBox m_checkbox;
        private Button B_confirm, B_cancel;
        private EditText m_title, m_amount;
        private Spinner m_select_user;
        private List<string> m_users;
        private List<string> m_userIDs = new List<string>();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BillsNew, container, false);
            
            InitializeFormat(view);

            return view;
        }

        private async void InitializeFormat(View view)
        {
            //initialize all components of dialog box
            B_cancel = view.FindViewById<Button>(Resource.Id.buttonBillsNewCancel);
            B_confirm = view.FindViewById<Button>(Resource.Id.buttonBillsNewConfirm);

            m_checkbox = view.FindViewById<CheckBox>(Resource.Id.BillsNewCheckbox);
            m_listview = view.FindViewById<ListView>(Resource.Id.BillsNewListView);
            m_select_user = view.FindViewById<Spinner>(Resource.Id.BillsNewSpinner);
            m_title = view.FindViewById<EditText>(Resource.Id.BillsNewEditText);
            m_amount = view.FindViewById<EditText>(Resource.Id.BillsNewEditTextAmount);

            //Get users to display in list
            m_users = await GetUsers();

            m_listview.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleListItemChecked, m_users);
            m_listview.ChoiceMode = Android.Widget.ChoiceMode.Multiple;

            //set click functions
            m_listview.ItemClick += M_listview_ItemClick;
            B_cancel.Click += M_Bcancel_Click;
            B_confirm.Click += M_Bconfirm_Click;

            //get categories for bill type
            var categories = GetCategories();

            m_select_user.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleSpinnerItem, categories);
        }

        private List<string> GetCategories()
        {
            List<string> categories = new List<string>();

            categories.Add("Rent");
            categories.Add("Utilities");
            categories.Add("Groceries");
            categories.Add("Other");

            return categories;
        }

        private void M_listview_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            bool selected = listView.IsItemChecked(position);

            listView.SetItemChecked(position, selected);
        }

        private async Task<List<string>> GetUsers()
        {
            DB_Singleton db = DB_Singleton.Instance;
            RequestHandler request = new RequestHandler(Context);

            List<string> users = new List<string>();
            var response = await request.GetUserLocations(db.Retrieve("Token"), db.GetActiveGroup().GroupID);

            if (response.Content != "" && response.Content != "[]")
            {
                JArray members = JArray.Parse(response.Content);

                foreach (JToken member in members)
                {
                    string username = (string)member["UserName"];
                    string userid = (string)member["UserID"];

                    users.Add((username));
                    m_userIDs.Add(userid);
                }
            }

            return users;
        }

        private async void M_Bconfirm_Click(object sender, EventArgs e)
        {       
            string user = "";
            string userid = "";
            string category = (string)m_select_user.SelectedItem;
            string title = m_title.Text;
            string amount = m_amount.Text;

            for (int i = 0; i < m_listview.Count; ++i)
            {
                if (m_listview.IsItemChecked(i))
                    //only used for testing
                    user = (string)m_listview.GetItemAtPosition(i);
                //users.Add((string)m_listview.GetItemAtPosition(i));

            }
            //get userid
            if (m_users.Contains(user))
                userid = m_userIDs.ElementAt(m_users.IndexOf(user));

            //Dismiss();
            //if the bill is not split equally, start the dialog that asks for splits
            if (!m_checkbox.Checked)
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                BillsNewSpecifyAmounts Dialog = new BillsNewSpecifyAmounts();
                Dialog.Show(transaction, "dialog fragment specify bill amounts");
            }

            DB_Singleton db = DB_Singleton.Instance;

            if (category == "Rent")
                category = "2";
            else if (category == "Utilities")
                category = "3";
            else if (category == "Groceries")
                category = "4";
            else
                category = "1";

            RequestHandler request = new RequestHandler(Context);
            await request.PutBill(db.Retrieve("Token"), db.GetActiveGroup().GroupID, userid, category, title, "description", amount, DateTime.Today);
            await ((Bills)Activity).UpdateAllBills(0);

            Dismiss();
        }

        private void M_Bcancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private async void NewBillReq(BillObj bill)
        {
            DB_Singleton db = DB_Singleton.Instance;

            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;

            RequestHandler request = new RequestHandler(Context);
            var response = await request.PutBill(token, groupid, bill.Recipientname, bill.Categoryid, bill.Title, bill.Description, bill.Amount, bill.Date);
        }
    }
}