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
        private Button m_Bconfirm, m_Bcancel;
        private EditText m_title, m_amount;
        private Spinner m_select_user;
        private List<string> users;
        private List<string> userIDs = new List<string>();
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BillsNew, container, false);

            
            InitializeFormat(view);
            InitializeListView(view);

            return view;
        }

        private async void InitializeFormat(View view)
        {
            m_Bcancel = view.FindViewById<Button>(Resource.Id.buttonBillsNewCancel);
            m_Bconfirm = view.FindViewById<Button>(Resource.Id.buttonBillsNewConfirm);

            m_Bcancel.Click += M_Bcancel_Click;
            m_Bconfirm.Click += M_Bconfirm_Click;

            m_checkbox = view.FindViewById<CheckBox>(Resource.Id.BillsNewCheckbox);
            m_listview = view.FindViewById<ListView>(Resource.Id.BillsNewListView);
            m_select_user = view.FindViewById<Spinner>(Resource.Id.BillsNewSpinner);
            m_title = view.FindViewById<EditText>(Resource.Id.BillsNewEditText);
            m_amount = view.FindViewById<EditText>(Resource.Id.BillsNewEditTextAmount);



            users = await GetUsers();

            m_listview.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleListItemChecked, users);
            m_listview.ChoiceMode = Android.Widget.ChoiceMode.Multiple;

            m_listview.ItemClick += M_listview_ItemClick;

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
                    userIDs.Add(userid);
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
            if (users.Contains(user))
                userid = userIDs.ElementAt(users.IndexOf(user));

            //Dismiss();
            //if the bill is not split equally, start the dialog that asks for splits
            if (!m_checkbox.Checked)
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                BillsNewSpecifyAmounts Dialog = new BillsNewSpecifyAmounts();
                Dialog.Show(transaction, "dialog fragment specify bill amounts");
            }

            DB_Singleton db = DB_Singleton.Instance;

            //just used for testing
            category = "1";

            RequestHandler request = new RequestHandler(Context);
            await request.PutBill(db.Retrieve("Token"), db.GetActiveGroup().GroupID, userid, category, title, "description", amount, DateTime.Now.ToString());
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

        private void InitializeListView(View view)
        {
            //Get users to display in listview
            
        }
    }
}