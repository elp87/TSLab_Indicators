using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;
using elp87.Helpers;
using elp87.TSLab.Indicators.Helpers;

namespace elp87.TSLab.Indicators
{
    [HandlerName("Элдер Импульс (выгрузка)")]
    public class ElderImpulse_DL : IBar2DoubleHandler
    {
        [HandlerParameter(true, "10", Max = "20", Min = "2", Step = "1")]
        public int EMAPeriod { get; set; }

        [HandlerParameter(true, "10", Max = "20", Min = "2", Step = "1")]
        public int MACD1Period { get; set; }

        [HandlerParameter(true, "10", Max = "20", Min = "2", Step = "1")]
        public int MACD2Period { get; set; }

        [HandlerParameter(true, "10", Max = "20", Min = "2", Step = "1")]
        public int MACDSignalPeriod { get; set; }

        public IList<double> Execute(ISecurity source)
        {
            IList<double> elderList = (IList<double>)new List<double>(source.ClosePrices.Count);
            IList<double> list2 = Series.EMA(source.ClosePrices, this.EMAPeriod);
            IList<double> list3 = (IList<double>)new List<double>(source.ClosePrices.Count);
            IList<double> list4 = (IList<double>)new List<double>(source.ClosePrices.Count);
            IList<double> list5 = (IList<double>)new List<double>(source.ClosePrices.Count);
            IList<double> list6 = Series.EMA(source.ClosePrices, this.MACD1Period);
            IList<double> list7 = Series.EMA(source.ClosePrices, this.MACD2Period);
            int num1 = Math.Max(this.MACD1Period, this.MACD2Period);
            int index1 = 0;
            while (index1 < source.ClosePrices.Count)
            {
                double num2 = index1 >= num1 ? list6[index1] - list7[index1] : 0.0;
                list3.Add(num2);
                checked { ++index1; }
            }
            IList<double> list8 = Series.EMA(list3, this.MACDSignalPeriod);
            int index2 = 0;
            while (index2 < source.ClosePrices.Count)
            {
                double num2 = list3[index2] - list8[index2];
                list5.Add(num2);
                checked { ++index2; }
            }
            int index3 = 0;
            
            while (index3 < source.ClosePrices.Count)
            {
                double num2 = -5.0;
                if (index3 == 0)
                {
                    num2 = 0.0;
                }
                else
                {
                    if (list5[index3] > list5[checked(index3 - 1)] && list2[index3] > list2[checked(index3 - 1)])
                        num2 = 1.0;
                    if (list5[index3] < list5[checked(index3 - 1)] && list2[index3] < list2[checked(index3 - 1)])
                        num2 = -1.0;
                    if (list5[index3] <= list5[checked(index3 - 1)] && list2[index3] >= list2[checked(index3 - 1)] || list5[index3] >= list5[checked(index3 - 1)] && list2[index3] <= list2[checked(index3 - 1)])
                        num2 = 0.0;
                }
                elderList.Add(num2);
                checked { ++index3; }
            }

            // Выгрузка данных
            int barCount = source.Bars.Count;
            int indCount = elderList.Count;
            int loadCount = Math.Min(barCount, indCount);

            int barEnumerator = barCount -1;
            int indEnumerator = indCount - 1;
            int loadEnumerator = loadCount - 1;

            List<IndicatorDay> dayList = new List<IndicatorDay>();
            while (loadEnumerator > 0)
            {
                IndicatorDay day = new IndicatorDay()
                {

                    Date = source.Bars[barEnumerator].Date,
                    Value = elderList[indEnumerator]
                };
                dayList.Add(day);

                barEnumerator--;
                indEnumerator--;
                loadEnumerator--;
            }
            dayList.Sort();

            XElement headX = new XElement("FilterSet");
            XElement paramX = new XElement("Params",
                new XElement("EMAPeriod", EMAPeriod),
                new XElement("MACD1Period", MACD1Period),
                new XElement("MACD2Period", MACD2Period),
                new XElement("MACDSignalPeriod", MACDSignalPeriod)
                );
            headX.Add(paramX);

            XElement daysX = new XElement("Days");
            foreach (IndicatorDay day in dayList)
            {
                daysX.Add(new XElement("Day",
                    new XElement("Date", day.Date),
                    new XElement("Value", day.Value)
                    ));
            }
            headX.Add(daysX);
            headX.Save(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Elder" + dayList.Min(day => day.Date).ToString("yyMMdd") + "_-_" + dayList.Max(day => day.Date).ToString("yyMMdd") + ".xml");

            return elderList;
        }

        
    }
}
