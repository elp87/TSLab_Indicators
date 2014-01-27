using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.DataSource;
using TSLab.Script.Helpers;

namespace elp87.TsLabIndicators
{
    public abstract class ForceIndex
    {
        public IList<double> calcForceIndex(ISecurity source)
        {
            List<double> indicator = new List<double>();
            indicator.Add(0);

            for (int i = 1; i < source.Bars.Count; i++)
            {
                double curValue = (source.Bars[i].Close - source.Bars[i - 1].Close) * source.Bars[i].Volume;
                indicator.Add(curValue);
            }

            return indicator;
        }
    }

    [HandlerName("Индекс силы (чистый)")]
    public class ClearForceIndex : ForceIndex, IBar2DoubleHandler
    {
        public IList<double> Execute(ISecurity source)
        {
            return calcForceIndex(source);
        }
    }

    [HandlerName("Индекс силы (сглаженный)")]
    public class SmoothedForceIndex : ForceIndex, IBar2DoubleHandler
    {
        [HandlerParameter]
        public int Period { get; set; }

        public IList<double> Execute(ISecurity source)
        {
            IList<double> clFI = calcForceIndex(source);
            return Series.SMA(clFI, Period);
        }
    }
}