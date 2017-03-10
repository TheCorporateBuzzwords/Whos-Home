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

namespace Whos_Home
{
    class BillsNewSpecifyAmountListAdapter : BaseAdapter<List<User>>
    {
        public override List<User> this[int position]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long GetItemId(int position)
        {
            throw new NotImplementedException();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            throw new NotImplementedException();
        }
    }
}