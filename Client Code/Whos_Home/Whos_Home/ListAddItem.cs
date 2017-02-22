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
        View view;
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

        private async Task<List<ItemObj>> GetItems()
        {
            RequestHandler request = new RequestHandler(this);
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;
            var response = await request.GetListItems(token, groupid, listid);
            JArray preParse = JArray.Parse(response.Content);

            List<ListsObj> groupLists = ParseToLists(preParse);

            return groupLists;

        }
        private List<ListsObj> ParseToLists(JArray jarr)
        {
            List<ListsObj> postParse = new List<ListsObj>();

            foreach (JToken tok in jarr)
            {
                string posttime = (string)tok["PostTime"];
                string username = (string)tok["UserName"];
                string title = (string)tok["Title"];
                string listid = (string)tok["ListID"];
                string firstname = (string)tok["FirstName"];
                string lastname = (string)tok["LastName"];

                postParse.Add(new ListsObj(posttime, username, title, listid, firstname, lastname));
            }

            return postParse;
        }
        private void BCancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void BConfrim_Click(object sender, EventArgs e)
        {
            //Implement new list item functionality
            string itemname = editText.Text;

            Toast.MakeText(view.Context, "Item Added: " + itemname,
               ToastLength.Short).Show();

            editText.Text = "";
        }
    }
}