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
        private Button bConfrim, bCancel;
        private EditText editText;
        private ListsObj m_list;
        View view;

        public ListAddItem(ListsObj list)
        {
            m_list = list;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            view = inflater.Inflate(Resource.Layout.ListNew, container, false);

            //set values for private attributes
            bConfrim = view.FindViewById<Button>(Resource.Id.buttonConfirmListNew);
            bCancel = view.FindViewById<Button>(Resource.Id.buttonCancelListNew);
            editText = view.FindViewById<EditText>(Resource.Id.edittextListNew);

            //set click functions
            bConfrim.Click += BConfrim_Click;
            bCancel.Click += BCancel_Click;

            return view;

        }

        private async Task PostItem()
        {
            RequestHandler request = new RequestHandler(Context);
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;
            var response = await request.PostNewListItem(token, groupid, m_list.Topicid, editText.Text);
            if((int)response.StatusCode == 200)
            {
                Toast.MakeText(Context, "Item Posted", ToastLength.Long);
            }
            else
            {
                Toast.MakeText(Context, "Post Failed", ToastLength.Long);

            }
            //JArray preParse = JArray.Parse(response.Content);

        }

        private void BCancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private async void BConfrim_Click(object sender, EventArgs e)
        {
            //Implement new list item functionality
            string itemname = editText.Text;
            await PostItem();
            Toast.MakeText(view.Context, "Item Added: " + itemname,
               ToastLength.Short).Show();

            editText.Text = "";

            Dismiss();
        }
    }
}