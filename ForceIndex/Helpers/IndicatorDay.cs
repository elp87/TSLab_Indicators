using System;

namespace elp87.TSLab.Indicators.Helpers
{
    public class IndicatorDay : IComparable
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }

        public int CompareTo(object obj)
        {           
            return this.Date.CompareTo(((IndicatorDay)obj).Date);
        }
    }
}
