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

namespace Whos_Home
{
    [Activity(Label = "BillsGraph")]
    public class BillsGraph : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BillsGraph);

            PlotView view = FindViewById<PlotView>(Resource.Id.plot_view);
            view.Model = CreatePlotModel();

            
        
        }

        private PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = "Monthly Expenses" };


            dynamic seriesP1 = new PieSeries { StrokeThickness = 2.0, InsideLabelPosition = 0.8, AngleSpan = 360, StartAngle = 0 };

            //seriesP1.Slices.Add(new PieSlice("Africa", 1030) { IsExploded = false, Fill = OxyColors.PaleVioletRed });
            seriesP1.Slices.Add(new PieSlice("Rent", 350));
            seriesP1.Slices.Add(new PieSlice("Groceries", 150));
            seriesP1.Slices.Add(new PieSlice("Utilities", 50));
            seriesP1.Slices.Add(new PieSlice("Other", 200));

            plotModel.Series.Add(seriesP1);

            return plotModel;
        }
    }
}