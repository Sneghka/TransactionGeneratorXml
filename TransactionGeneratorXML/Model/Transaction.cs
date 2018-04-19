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

        public Transaction(List<StockPosition> stockPositionList, string cashPointNumber, DateTime startDate, bool isStockTransactionRequired = false)
        {
            foreach (var position in stockPositionList)
            {
                int localQuantity = 0;

                //if stock position direction = recycle then create two transaction lines 
                if (position.Direction == 3)
                {
                    var transactionLineIssue = new TransactionLine(position, "issue", "recycle", startDate, isStockTransactionRequired);
                    var transactionLineCollect = new TransactionLine(position, "collect", "recycle", startDate, isStockTransactionRequired);

                    TransactionLinesList.Add(transactionLineIssue);
                    if (!isStockTransactionRequired) TransactionLinesList.Add(transactionLineCollect);

                    localQuantity = transactionLineCollect.Quantity - transactionLineIssue.Quantity;
                    position.Quantity += localQuantity;
                }
                else
                {
                    var lineType = position.Direction == 1 ? "issue" : "collect"; // line type depends on stock position direction
                    var direction = position.Direction == 1 ? "issue" : "collect";

                    var transactionLine = new TransactionLine(position, lineType, direction, startDate, isStockTransactionRequired);
                    TransactionLinesList.Add(transactionLine);

                    localQuantity = transactionLine.Quantity;
                    position.Quantity = position.Direction == 1 ? position.Quantity - localQuantity : position.Quantity + localQuantity;
                }

                if (isStockTransactionRequired == false)
                {
                    IsStockTransactionRequiredMethod(position, localQuantity);
                }

                else
                {
                    position.Quantity = position.Direction == 1 ? position.Capacity : position.Direction == 2 ? 0 : position.Capacity / 2;
                }
            }

            var transactionQuantity = Helper.GetQuantityFromTransactionLinesList(TransactionLinesList);
            var startDateStock = startDate.AddDays(-1).AddSeconds(10).ToString("yyyy-MM-dd HH:mm:ss");

            MachineNumber = cashPointNumber;
            StartDate = isStockTransactionRequired == false ? startDate.ToString("yyyy-MM-dd HH:mm:ss") : startDateStock;
            Quantity = transactionQuantity;
            Replenishment = "no";
            TransactionType = isStockTransactionRequired == true ? "stock" : transactionQuantity >= 0 ? "issue" : "collect";
        }

        void IsStockTransactionRequiredMethod(StockPosition position, int localQuantity)
        {
            var cassetteQuantity = 0;           
            if (position.Direction == 1 || position.Direction == 3) cassetteQuantity = position.Quantity - localQuantity;
            if (position.Direction == 2 || position.Direction == 3) cassetteQuantity = position.Quantity + localQuantity;

            if ((cassetteQuantity < 0 && (position.Direction == 1 || position.Direction == 3)) || (cassetteQuantity > position.Capacity && (position.Direction == 2 || position.Direction == 3)))
            {
                IsStockTransactionRequired = true;             
            }          
        }
    }
}
