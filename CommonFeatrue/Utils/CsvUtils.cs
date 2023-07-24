using CsvHelper;
using System.Globalization;

namespace International.Common.Methods
{
    /// <summary>
    /// CSV文件帮助类
    /// </summary>
    public static class CsvUtils
    {
        /// <summary>
        /// 将 Enumerable类型的转换为Csv
        /// </summary>
        /// <param name="records"></param>
        /// <param name="path"></param>
        public static void EnumerableToCsv<T>(this IEnumerable<T> records, string path)
        {
            using var writer = new StreamWriter(path);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(records);
        }
    }
}
