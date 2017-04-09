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

using Newtonsoft.Json.Linq;
using Whos_Home.Helpers;
using System.Threading.Tasks;

namespace Whos_Home
{
    class ListAddItem : DialogFragment
    {
        private Button B_Confrim, B_Cancel;
        private EditText m_editText;
        private ListsObj m_list;
        View m_view;

        public ListAddItem(ListsObj list)
        {
            m_list = list;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            m_view = inflater.Inflate(Resource.Layout.ListAddItem, container, false);

            //set values for private attributes
            B_Confrim = m_view.FindViewById<Button>(Resource.Id.buttonConfirmListAddItem);
            B_Cancel = m_view.FindViewById<Button>(Resource.Id.buttonCancelListAddItem);
            m_editText = m_view.FindViewById<EditText>(Resource.Id.edittextListAddItem);

            //set click functions
            B_Confrim.Click += BConfrim_Click;
            B_Cancel.Click += BCancel_Click;

            return m_view;

        }

        private async Task PostItem()
        {
            RequestHandler request = new RequestHandler(Context);
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;
            var response = await request.PostNewListItem(token, groupid, m_list.Topicid, m_editText.Text);
            if((int)response.StatusCode == 200)
            {
                Toast.MakeText(Context, "Item Posted", ToastLength.Long);
            }
            else
            {
                Toast.MakeText(Context, "Post Failed", ToastLength.Long);

            }
        }

        private void BCancel_Click(object sender, EventArgs e)
        {
            ((List)Activity).UpdateListView();
            Dismiss();
        }

        private async void BConfrim_Click(object sender, EventArgs e)
        {
            //Implement new list item functionality
            string itemname = m_editText.Text;
            await PostItem();
            Toast.MakeText(m_view.Context, "Item Added: " + itemname,
               ToastLength.Short).Show();

            m_editText.Text = "";

            //Dismiss();
        }
    }
}