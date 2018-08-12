using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TransactionGeneratorXML.Model;

namespace TransactionGeneratorXML
{
    public class Converter
    {

        public void CreateTransactions(string cashPointNumber, DateTime startDate, DateTime endDate, string pathFolder, string dbConnection)
        {

            XmlSerializer s = new XmlSerializer(typeof(RootElement));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var rootElementList = new List<RootElement>();
            var generator = new DataGenerator();
            generator.GenerateData(rootElementList, cashPointNumber, startDate, endDate, dbConnection);
            var timestamp = DateTime.UtcNow;

            foreach (var transaction in rootElementList)
            {
                timestamp = timestamp.AddMilliseconds(1);
                var path = string.Concat(pathFolder, timestamp.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture), ".xml");
                using (var writer = new StreamWriter(path))
                {
                    s.Serialize(writer, transaction, ns);
                }
            }
        }

    }
}
