using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace Whos_Home.Helpers.ListObjects
{
    class ItemList : IListObjHelper<ItemObj>
    {
        public ItemList(string id)
        {
            topicId = id;
        }
        public void DeleteItem(string itemId)
        {
            throw new NotImplementedException();
        }

        public void DeleteList()
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> DeleteRequest(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ItemObj>> UpdateList()
        {
            m_list = new List<ItemObj>();
            var response = await UpdateRequest();
            if ((int)response.StatusCode != 200)
                throw new Exception("Error Updating List Items");

            JArray preParse = JArray.Parse(response.Content);

            foreach (JToken token in preParse)
                m_list.Add(new ItemObj(token));

            return m_list;
        }

        public Task<IRestResponse> UpdateRequest()
        {
            List<ItemObj> groupLists = new List<ItemObj>();
            RequestHandler request = new RequestHandler();
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;

            return request.GetListItems(token, groupid, topicId);
        }

        string topicId;
        List<ItemObj> m_list;
    }
}