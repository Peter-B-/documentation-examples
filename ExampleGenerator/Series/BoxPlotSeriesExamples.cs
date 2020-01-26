using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ExampleGenerator.Series
{
    public class BoxPlotSeriesExamples
    {
        [Export("Series/BoxPlotSeries")]
        public static PlotModel BoxPlot()
        {
            const int boxes = 10;

            var model = new PlotModel { Title = string.Format("BoxPlot (n={0})", boxes) };

            var s1 = new BoxPlotSeries
            {
                Title = "BoxPlotSeries",
                BoxWidth = 0.3,
                Fill = model.DefaultColors[0],
            };

            var random = new Random(31);
            for (var i = 0; i < boxes; i++)
            {
                double x = i;
                var points = 5 + random.Next(15);
                var values = new List<double>();
                for (var j = 0; j < points; j++)
                {
                    values.Add(random.Next(0, 20));
                }

                values.Sort();
                var median = GetMedian(values);
                var mean = values.Average();
                int r = values.Count % 2;
                double firstQuartil = GetMedian(values.Take((values.Count + r) / 2));
                double thirdQuartil = GetMedian(values.Skip((values.Count - r) / 2));

                var iqr = thirdQuartil - firstQuartil;
                var step = iqr * 1.5;
                var upperWhisker = thirdQuartil + step;
                upperWhisker = values.Where(v => v <= upperWhisker).Max();
                var lowerWhisker = firstQuartil - step;
                lowerWhisker = values.Where(v => v >= lowerWhisker).Min();

                var outliers = new[] { upperWhisker + random.Next(1, 10), lowerWhisker - random.Next(1, 10) };

                s1.Items.Add(new BoxPlotItem(x, lowerWhisker, firstQuartil, median, thirdQuartil, upperWhisker) { Mean = mean, Outliers = outliers });
            }

            model.Series.Add(s1);
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, MinimumPadding = 0.1, MaximumPadding = 0.1 });
            return model;
        }

        /// <summary>
        /// Gets the median.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private static double GetMedian(IEnumerable<double> values)
        {
            var sortedInterval = new List<double>(values);
            sortedInterval.Sort();
            var count = sortedInterval.Count;
            if (count % 2 == 1)
            {
                return sortedInterval[(count - 1) / 2];
            }

            return 0.5 * sortedInterval[count / 2] + 0.5 * sortedInterval[(count / 2) - 1];
        }




        [Export("Series/BoxPlotSeries_DateTimeAxis")]
        public static PlotModel BoxPlotSeries_DateTimeAxis()
        {
            var m = new PlotModel();
            var x0 = DateTimeAxis.ToDouble(new DateTime(2013, 05, 04));
            m.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = x0 - 0.9,
                Maximum = x0 + 1.9,
                IntervalType = DateTimeIntervalType.Days,
                MajorStep = 1,
                MinorStep = 1,
                StringFormat = "yyyy-MM-dd",
            });
            var boxPlotSeries = new BoxPlotSeries
            {
                TrackerFormatString = "X: {1:yyyy-MM-dd}\nUpper Whisker: {2:0.00}\nThird Quartil: {3:0.00}\nMedian: {4:0.00}\nFirst Quartil: {5:0.00}\nLower Whisker: {6:0.00}\nMean: {7:0.00}",
                Fill = m.DefaultColors[0],
            };
            boxPlotSeries.Items.Add(new BoxPlotItem(x0, 10, 14, 16, 20, 22) { Mean = 17, Outliers = new[] { 23.5 } });
            boxPlotSeries.Items.Add(new BoxPlotItem(x0 + 1, 11, 13, 14, 15, 18) { Outliers = new[] { 23.4 } });
            m.Series.Add(boxPlotSeries);
            return m;
        }
    }
}