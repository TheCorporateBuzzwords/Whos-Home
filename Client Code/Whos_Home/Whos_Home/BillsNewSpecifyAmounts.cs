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
    class BillsNewSpecifyAmounts : DialogFragment
    {
        private List<string> m_users;
        private Button m_BConfirm, m_BCancel;
        private ListView m_listview;

        public BillsNewSpecifyAmounts(List<string> users)
        {
            m_users = users;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BillsNewSpecifyAmounts, container, false);

            m_BConfirm = view.FindViewById<Button>(Resource.Id.buttonBillsNewSpecifyAmountsConfirm);
            m_BCancel = view.FindViewById<Button>(Resource.Id.buttonBillsNewSpecifyAmountsCancel);

            m_BConfirm.Click += M_BConfirm_Click;
            m_BCancel.Click += M_BCancel_Click;

            m_listview = view.FindViewById<ListView>(Resource.Id.BillsNewSpecifyAmountsListView);
            m_listview.Adapter = new BillsNewSpecifyAmountListAdapter(this.Activity, m_users);

            return view;
        }

        private void M_BCancel_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void M_BConfirm_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}