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

namespace Whos_Home.Helpers
{
    class ListObjHelper<T> where T : new()
    {
        ListObjHelper(string content, T type)
        {
            try
            {
                JArray content_array = JArray.Parse(content);

                foreach (JToken item in content_array)
                {
                    m_list.Add((T)Activator.CreateInstance(typeof(T), item));
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        List<T> GetList()
        {
            return m_list;
        }

        List<T> m_list;
    }
}