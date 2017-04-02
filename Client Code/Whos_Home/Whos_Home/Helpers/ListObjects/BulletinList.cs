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

using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace Whos_Home.Helpers.ListObjects
{
    class BulletinList : IListObjHelper<BulletinPostObj>
    {
        public BulletinList()
        {
            m_list = new List<BulletinPostObj>();
        }

        public Task<IRestResponse> UpdateRequest()
        {
            RequestHandler request = new RequestHandler();
            DB_Singleton db = DB_Singleton.Instance;
            string user_token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;

            return request.GetMessages(user_token, groupid);
        }
        public Task<IRestResponse> DeleteRequest(string id)
        {
            RequestHandler request = new RequestHandler();

            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            return request.DeletePost(token, db.GetActiveGroup().GroupID, id);
        }
        void Add(BulletinPostObj bulletin)
        {
            m_list.Add(bulletin);
        }

        
        public async Task<List<BulletinPostObj>> UpdateList()
        {
            m_list = new List<BulletinPostObj>();
            var response = await UpdateRequest();

            if ((int)response.StatusCode == 200)
            {
                //Check to make sure List is not empty
                //If response.Content is empty .Parse will break
                if (response.Content != "[]")
                {
                    JArray JPosts = JArray.Parse(response.Content);

                    //Wanted to add JToken constructor, but
                    //adding a constructor that takes a JToken 
                    //Will break Activity
                    foreach (JToken token in JPosts)
                        m_list.Add(new BulletinPostObj().BuildPostToken(token));
                }
            }
            else
                throw new Exception("Connection Error");

            return m_list;
        }

        public void DeleteItem(string itemid)
        {

        }

        public void DeleteList()
        {

        }

        List<BulletinPostObj> m_list;
    }
}