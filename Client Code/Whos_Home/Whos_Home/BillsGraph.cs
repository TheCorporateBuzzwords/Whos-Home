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
using OxyPlot.Xamarin.Android;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Reporting;
using Microsoft;
using Newtonsoft.Json;
using Whos_Home.Helpers;

namespace Whos_Home
{
    [Activity(Label = "BillsGraph")]
    public class BillsGraph : BaseActivity
    {
        private List<Tuple<string, float>> m_bills;
        private double m_total = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BillsGraph);

            m_bills = new List<Tuple<string, float>>();

            m_bills = JsonConvert.DeserializeObject<List<Tuple<string, float>>>(Intent.GetStringExtra("BillsList"));

            InitializeToolbars();

            //find total value for month
            foreach(var bill in m_bills)
            {
                m_total += bill.Item2;
            }

            PlotView view = FindViewById<PlotView>(Resource.Id.plot_view);
            view.Model = CreatePlotModel();
        }

        private PlotModel CreatePlotModel()
        {
            string title = "Monthly Expenses: $" + m_total.ToString();
            var plotModel = new PlotModel { Title = title };

            dynamic seriesP1 = new PieSeries { StrokeThickness = 2.0, InsideLabelPosition = 0.8, AngleSpan = 360, StartAngle = 0 };

            //create pie slices
            foreach(var bill in m_bills)
            {
                seriesP1.Slices.Add(new PieSlice(bill.Item1, bill.Item2));
            }

            plotModel.Series.Add(seriesP1);

            return plotModel;
        }  
    }
}