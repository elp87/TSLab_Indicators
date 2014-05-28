using elp87.TSLab.Indicators.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;

namespace elp87.TSLab.Indicators
{
    [HandlerName("Элдер Импульс (выгрузка тест)")]
    public class ElderImpulse_DLFull : IBar2DoubleHandler
    {
        public IList<double> Execute(ISecurity source)
        {
            for (int EmaPeriodCounter = 3; EmaPeriodCounter <= 30; EmaPeriodCounter += 2)
            {
                for (int Macd1PeriodCounter = 3; Macd1PeriodCounter < 30; Macd1PeriodCounter += 2)
                {
                    for (int Macd2PeriodCounter = Macd1PeriodCounter + 1; Macd2PeriodCounter <= 30; Macd2PeriodCounter += 2)
                    {
                        for (int MacdSignalPeriodCounter = 3; MacdSignalPeriodCounter <= 30; MacdSignalPeriodCounter += 2)
                        {
                            IList<double> elderList = CalcElderFilter(source, EmaPeriodCounter, Macd1PeriodCounter, Macd2PeriodCounter, MacdSignalPeriodCounter);
                            SaveFilterFile(source, elderList, EmaPeriodCounter, Macd1PeriodCounter, Macd2PeriodCounter, MacdSignalPeriodCounter);
                        }
                    }
                }
            }
            List<double> returnList = new List<double>(source.Bars.Count);
            returnList.ForEach(var => var = 1);
            return returnList;
        }

        private IList<double> CalcElderFilter(ISecurity source, int EMAPeriod, int MACD1Period, int MACD2Period, int MACDSignalPeriod)
        {
            IList<double> elderList = (IList<double>)new List<double>(source.ClosePrices.Count);
            IList<double> list2 = Series.EMA(source.ClosePrices, EMAPeriod);
            IList<double> list3 = (IList<double>)new List<double>(source.ClosePrices.Count);
            IList<double> list4 = (IList<double>)new List<double>(source.ClosePrices.Count);
            IList<double> list5 = (IList<double>)new List<double>(source.ClosePrices.Count);
            IList<double> list6 = Series.EMA(source.ClosePrices, MACD1Period);
            IList<double> list7 = Series.EMA(source.ClosePrices, MACD2Period);
            int num1 = Math.Max(MACD1Period, MACD2Period);
            int index1 = 0;
            while (index1 < source.ClosePrices.Count)
            {
                double num2 = index1 >= num1 ? list6[index1] - list7[index1] : 0.0;
                list3.Add(num2);
                checked { ++index1; }
            }
            IList<double> list8 = Series.EMA(list3, MACDSignalPeriod);
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
            return elderList;
        }

        private void SaveFilterFile(ISecurity source, IList<double> elderList, int EMAPeriod, int MACD1Period, int MACD2Period, int MACDSignalPeriod)
        {
            // Выгрузка данных
            int barCount = source.Bars.Count;
            int indCount = elderList.Count;
            int loadCount = Math.Min(barCount, indCount);

            int barEnumerator = barCount - 1;
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
                daysX.Add(new XElement("D",
                    new XElement("d", day.Date),
                    new XElement("v", day.Value)
                    ));
            }
            headX.Add(daysX);

            string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Elder" + dayList.Min(day => day.Date).ToString("yyMMdd") + "_-_" + dayList.Max(day => day.Date).ToString("yyMMdd");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            headX.Save(dirPath + @"\" + EMAPeriod.ToString() + "_" + MACD1Period.ToString() + "_" + MACD2Period + "_" + MACDSignalPeriod + ".xml");
        }
    }
}