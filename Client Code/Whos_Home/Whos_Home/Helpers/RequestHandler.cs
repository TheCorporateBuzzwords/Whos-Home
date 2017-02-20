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
using Couchbase.Lite;

namespace Whos_Home.Helpers
{
    class RequestHandler
    {
        private string url = null;
        IRestRequest request = null;
        RestClient client = null;
        public RequestHandler(Context context)
        {

            url = context.Resources.GetString(Resource.String.url);
            client = new RestClient(url);
        }

       public async Task<IRestResponse> SignIn(User user)
        {

            request = new RestRequest("/session", Method.POST);

            request.AddObject(user);

            var response = await client.ExecuteTaskAsync(request);

            return response;
        }

        public async Task<IRestResponse> SignUp(User user)
        {
            request = new RestRequest("/users", Method.POST);

            request.AddObject(user);

            return await client.ExecuteTaskAsync(request);
        }

        public async Task<IRestResponse> CreateGroup(string groupName, string token)
        {
            request = new RestRequest("/groups", Method.POST);

            request.AddParameter("groupName", groupName);
            request.AddHeader("x-access-token", token);
            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
       
        /*public async Task<IRestResponse> InviteToGroup(string token, string groupID, string username)
        {
            request = new RestRequest("/groups/{groupid}/invitation/?{recipient}", Method.GET);
            request.AddUrlSegment("groupid", groupID);
            request.AddUrlSegment("recipient", username);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;

        }*/

        public async Task<IRestResponse> PullGroups(string token)
        {
            request = new RestRequest("/users/groups", Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        
        public async Task<IRestResponse> RespondInvitation(string token, string groupid, bool deny)
        {
            request = new RestRequest(string.Format("/groups/{0}/invitation/?deny={1}", groupid, deny.ToString().ToLower()), Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        public async Task<IRestResponse> SendInvitation(string token, string groupid, string username)
        {
            request = new RestRequest(string.Format("/groups/{0}/invitation/", groupid), Method.POST);
            request.AddUrlSegment("groupid", groupid);
            request.AddParameter("recipient", username);

            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);

            return response;
        }

        public async Task<IRestResponse> AddLocation(string token, string SSID, string locationName, string groupid)
        {
            request = new RestRequest("groups/{groupid}/location/", Method.POST);
            request.AddUrlSegment("groupid", groupid);
            request.AddParameter("ssid", SSID);
            request.AddParameter("locationName", locationName);

            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);

            return response;
        }

        public async Task<IRestResponse> GetLocations(string token, string groupid)
        {
            request = new RestRequest("/groups/{groupid}/locations/", Method.GET);
            request.AddHeader("x-access-token", token);

            request.AddUrlSegment("groupid", groupid);

            var response = await client.ExecuteTaskAsync(request);

            return response;
        }

        public async Task<IRestResponse> UpdateLocation(string token, string SSID)
        {
            //SSID can be null if user is offline
            request = new RestRequest("/users/locations", Method.PUT);

            request.AddHeader("x-access-token", token);
            request.AddParameter("ssid", SSID);

            var response = await client.ExecuteTaskAsync(request);

            return response;
        }

        public async Task<IRestResponse> GetInvitations(string token)
        {
            request = new RestRequest("/users/invites", Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        public async Task<IRestResponse> GetMessages(string token, string groupid)
        {
            request = new RestRequest(string.Format("/groups/{0}/messagetopic", groupid), Method.GET);
            request.AddHeader("x-access-token", token);
            
            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        public async Task<IRestResponse> PostMessages(string token, string groupid, string title, string message)
        {
            request = new RestRequest(string.Format("/groups/{0}/messagetopic", groupid), Method.POST);
            request.AddHeader("x-access-token", token);
            request.AddParameter("title", title);
            request.AddParameter("msg", message);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        public async Task<IRestResponse> GetMessageReplies(string token, string groupid, string topicid)
        {
            request = new RestRequest(string.Format("/groups/{0}/messages/{1}", groupid, topicid), Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
            
        }

        public async Task<IRestResponse> PostMessageReply(string token, string groupid, string topicid, string message)
        {
            request = new RestRequest(string.Format("/groups/{0}/messages", groupid), Method.POST);
            request.AddHeader("x-access-token", token);
            request.AddParameter("topicid", topicid);
            request.AddParameter("msg", message);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        public async Task<IRestResponse> PostNewList(string token, string groupid, string title)
        {
            request = new RestRequest(string.Format("/groups/{0}/lists", groupid), Method.POST);
            request.AddHeader("x-access-token", token);
            request.AddParameter("title", title);

            var response = await client.ExecuteTaskAsync(request);
            return response;

        }

        public async Task<IRestResponse> PostNewListItem(string token, string groupid, string listid, string content)
        {
            request = new RestRequest(string.Format("/groups/{0}/lists/{1}", groupid, listid), Method.POST);
            request.AddHeader("x-access-token", token);
            request.AddParameter("content", content);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> GetLists(string token, string groupid)
        {
            request = new RestRequest(string.Format("/groups/{0}/lists/", groupid), Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> GetListItems(string token, string groupid, string listid)
        {
            request = new RestRequest(string.Format("/groups/{0}/lists/{1}", groupid, listid), Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        //public async Task<IRestResponse> UpdateListItem(string token, string groupid, string item) { }

    }
}