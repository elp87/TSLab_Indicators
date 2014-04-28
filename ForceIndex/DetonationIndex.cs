using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;

namespace elp87.TSLab.Indicators
{
    [HandlerName("Индекс взрыва")]
    public class DetonationIndex : IBar2DoubleHandler
    {

        public IList<double> Execute(ISecurity source)
        {
            List<double> indicator = new List<double>();
            for (int i = 0; i < source.Bars.Count; i++)
            {
                Bar bar = source.Bars[i];
                double val;
                if (bar.Close > bar.Open)
                { 
                    val = bar.High - bar.Open; 
                }
                else 
                { 
                    val = bar.Low - bar.Open; 
                }
                indicator.Add(val);
            }
            return indicator;
        }
    }
}
