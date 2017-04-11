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
using System.Threading.Tasks;
using RestSharp;

namespace Whos_Home.Helpers
{
    /********************************
     * Lists that implement the IListObjHelper 
     * are lists of a specific helper object.
     * Each of these lists have the ability to update
     * and delete items in the list. It simplifies
     * the activities and keeps much of the logic 
     * away from the activity itself.
     * ******************************/
    interface IListObjHelper<T>
    {
        Task<List<T>> UpdateList();
        void DeleteItem(string itemId);
        void DeleteList();
        Task<IRestResponse> UpdateRequest();
        Task<IRestResponse> DeleteRequest(string id);
    }
}