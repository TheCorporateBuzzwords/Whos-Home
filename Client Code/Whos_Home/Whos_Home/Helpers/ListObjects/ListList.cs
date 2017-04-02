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

using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace Whos_Home.Helpers.ListObjects
{
    class ListList : IListObjHelper<ListsObj>
    {
        public ListList()
        {
            m_list = new List<ListsObj>();
        }
        public Task<IRestResponse> UpdateRequest()
        {
            RequestHandler request = new RequestHandler();
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;
            return request.GetLists(token, groupid);
        }
        public Task<IRestResponse> DeleteRequest(string id)
        {
            RequestHandler request = new RequestHandler();

            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            return request.DeletePost(token, db.GetActiveGroup().GroupID, id);
        }
        void Add(ListsObj bulletin)
        {
            m_list.Add(bulletin);
        }


        public async Task<List<ListsObj>> UpdateList()
        {
            var response = await UpdateRequest();
            if ((int)response.StatusCode != 200)
                throw new Exception("Error Updating lists");

            JArray preParse = JArray.Parse(response.Content);

            m_list = new List<ListsObj>();

            foreach (JToken tok in preParse)
                m_list.Add(new ListsObj(tok));

            return m_list;

        }

        public void DeleteItem(string itemid)
        {

        }

        public void DeleteList()
        {

        }

        List<ListsObj> m_list;
    }
}