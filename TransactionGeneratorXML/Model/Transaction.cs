using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TransactionGeneratorXML.Model
{


    public class Transaction
    {
        [XmlAttribute("act")]
        public int Act { get; set; } = 0;

        [XmlAttribute("mapper")]
        public string Mapper { get; set; } = "";

        [XmlElement(ElementName = "MachineNumber", Order = 1)]
        public string MachineNumber { get; set; }

        [XmlElement(ElementName = "Type", Order = 2)]
        public string TransactionType { get; set; }

        [XmlElement(ElementName = "StartDate", Order = 3)]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "EndDate", Order = 4)]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "Quantity", Order = 5)]
        public int Quantity { get; set; }

        [XmlElement(ElementName = "Replenishment", Order = 6)]
        public string Replenishment { get; set; }

        [XmlElement(ElementName = "WP_CM_TransactionLine", Order = 7)]
        public List<TransactionLine> TransactionLinesList = new List<TransactionLine>();

        [XmlIgnore]
        public bool IsStockTransactionRequired { get; set; } = false;

        public Transaction() { }
        public Transaction(string cashPointNumber, DateTime startDate, List<TransactionLine> transactionLinesList, bool isStockTransactionRequired = false)
        {
            var startDateStock = startDate.AddDays(-1).AddSeconds(10).ToString("yyyy-MM-dd HH:mm:ss");
            var transactionQuantity = Helper.GetQuantityFromTransactionLinesList(transactionLinesList);

            MachineNumber = cashPointNumber;
            StartDate = isStockTransactionRequired == false ? startDate.ToString("yyyy-MM-dd HH:mm:ss") : startDateStock;
            Quantity = transactionQuantity;
            Replenishment = "no";
            TransactionType = isStockTransactionRequired == true ? "Stock" : transactionQuantity >= 0 ? "Issue" : "Collect";
            TransactionLinesList = transactionLinesList;
        }   
    }
}
