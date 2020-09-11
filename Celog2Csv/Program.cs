using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CelogParserLib;
using CelogParserLib.Data;
using CsvHelper;
using CsvHelper.Configuration;

namespace Celog2Csv
{
    class Program
    {
        class TypeConverter : CsvHelper.TypeConversion.ITypeConverter
        {
            public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                throw new NotImplementedException();
            }

            public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
                => value switch
                {
                    byte[] bytes => "{" + BitConverter.ToString(bytes) + "}",
                    null => "",
                    _ => value.ToString(),
                };
        }
        public class CelogDataMapper : ClassMap<CelogData>
        {
            public CelogDataMapper(Celog _)
            {
                var index = 0;
                Map(record => record.Timestamp).Index(index++).Name(nameof(CelogData.Timestamp));
                Map(record => record.ConsumedTime).Index(index++).Name("ConsumedTime(μs)");
                Map(record => record.Cpu).Index(index++).Name(nameof(CelogData.Cpu));
                Map(record => record.Process).Index(index++).Name("Process Desc")
                    .ConvertUsing(logData => logData.Process?.Name ?? "");
                Map(record => record.CurrentThread.Handle).Index(index++).Name("Thread")
                    .ConvertUsing(logData => logData.CurrentThread?.Handle.ToString() ?? "");
                Map(record => record.CurrentThread.FriendlyName).Index(index++).Name("Thread Desc")
                    .ConvertUsing(logData =>
                    logData.CurrentThread != null ? logData.CurrentThread.GetFriendlyName(logData.Timestamp) : "Unknown Thread");
                Map(record => record.Id).Index(index++).Name(nameof(CelogData.Id));
                Map(record => record.Data).Index(index++).Name(nameof(CelogData.Data)).TypeConverter<TypeConverter>();
                Map(record => record.ContainsHadles).Index(index++).Name("Related Kernel Handles")
                    .ConvertUsing(record => string.Join(",", record.ContainsHadles));
            }
        }
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            var log = new CelogParser(args[0]).CeLogToObject((sender, progress) => Console.WriteLine($"{progress} % processed..."));
            Console.WriteLine($"{log.Timeline.Count}, {sw.ElapsedMilliseconds}");

            using var output = new StreamWriter(args[0] + ".timeline.csv", append: false, Encoding.UTF8);
            using var writer = new CsvHelper.CsvWriter(output)
            {
                Configuration = {
                       HasHeaderRecord = true,
                       ShouldQuote = (field,context) => true,
                },

            };
            writer.Configuration.RegisterClassMap(new CelogDataMapper(log));
            writer.WriteRecords(log.Timeline);

        }
    }
}
