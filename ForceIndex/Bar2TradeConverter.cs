using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TSLab.Script;
using TSLab.Script.Handlers;
using elp87.Helpers;

namespace elp87.TSLab.Indicators
{
    [HandlerName("Bar2TradeConverter")]
    public class Bar2TradeConverter : IBar2DoubleHandler
    {
        private const string _filename = "Bar2TradeExport.csv";
        public IList<double> Execute(ISecurity source)
        {
            
            if (!Directory.Exists(Environment.GetEnvironmentVariable("appdata") + "\\elpTSLab"))
			{
				Directory.CreateDirectory(Environment.GetEnvironmentVariable("appdata") + "\\elpTSLab");
			}

            List<Bar> barList = (List<Bar>)source.Bars;
            CSVWriter writer = new CSVWriter(barList);
            writer.AddColumnConst(@"Long/Short", "Длинная");
            writer.AddColumnConst("Symbol", "RTS");
            writer.AddColumnConst("Shares", "1");
            writer.AddColumnConst("Entry Signal", "LE");
            writer.AddColumnConst("Entry Bar", "42");
            writer.AddColumnProperty("Entry Date", "Date");
            writer.AddColumnProperty("Entry Price", "Open");
            writer.AddColumnConst("Exit Signal", "LS");
            writer.AddColumnConst("Exit Bar", "47");
            writer.AddColumnProperty("Exit Date", "Date");
            writer.AddColumnProperty("Exit Price", "Close");

            writer.SaveFile(Environment.GetEnvironmentVariable("appdata") + "\\elpTSLab\\" + _filename);

            return barList.Select(bar => bar.Close).ToList();
                
        }
    }
}