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

namespace Whos_Home
{
    class BillsNew : DialogFragment
    {
        private ListView m_listview;
        private CheckBox m_checkbox;
        private Button m_Bconfirm, m_Bcancel;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BillsNew, container, false);

            
            InitializeFormat(view);
            InitializeListView(view);

            return view;
        }

        private void InitializeFormat(View view)
        {
            m_Bcancel = view.FindViewById<Button>(Resource.Id.buttonBillsNewCancel);
            m_Bconfirm = view.FindViewById<Button>(Resource.Id.buttonBillsNewConfirm);

            m_Bcancel.Click += M_Bcancel_Click;
            m_Bconfirm.Click += M_Bconfirm_Click;

            m_checkbox = view.FindViewById<CheckBox>(Resource.Id.BillsNewCheckbox);
            m_listview = view.FindViewById<ListView>(Resource.Id.BillsNewListView);


        }

        private void M_Bconfirm_Click(object sender, EventArgs e)
        {
            Dismiss();
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            BillsNewSpecifyAmounts Dialog = new BillsNewSpecifyAmounts();
            Dialog.Show(transaction, "dialog fragment specify bill amounts");
        }

        private void M_Bcancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void InitializeListView(View view)
        {
            //Get users to display in listview
            
        }
    }
}