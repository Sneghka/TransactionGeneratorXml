using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TransactionGeneratorXML.Enums;

namespace TransactionGeneratorXML.Model
{
    public class TransactionLine
    {

        [XmlElement(ElementName = "Type", Order = 1)]  //issue = 1, collect = 2
        public string TransactionLineType { get; set; }

        [XmlElement(ElementName = "StockPositionDirection", Order = 2)]
        public string StockPositionDirection { get; set; }

        [XmlElement(ElementName = "CassetteNumber", Order = 3)]
        public int CassetteNumber { get; set; }

        [XmlElement(ElementName = "CassetteExternalNumber", Order = 4)]
        public string CassetteExternalNumber { get; set; }

        [XmlElement(ElementName = "IsMixed", Order = 5)]
        public string IsMixed { get; set; }

        [XmlElement(ElementName = "Quantity", Order = 6)]
        public int Quantity { get; set; }

        [XmlElement(ElementName = "MaterialID", Order = 7)]
        public string MaterialID { get; set; }
        public bool ShouldSerializeMaterialID()
        {
            return !String.IsNullOrEmpty(MaterialID);
        }

        [XmlElement(ElementName = "ProductCode", Order = 8)]
        public string ProductID { get; set; }
        public bool ShouldSerializeProductID()
        {
            return !String.IsNullOrEmpty(ProductID);
        }


        public TransactionLine() { }

        public TransactionLine(StockPosition stockPosition, string transactionLineType, string stockPositionDirection, DateTime startDate, bool isStockTransactionRequired)
        {
           
            
                TransactionLineType = transactionLineType;
                StockPositionDirection = stockPositionDirection;
                CassetteNumber = stockPosition.CassetteNumber;
                CassetteExternalNumber = stockPosition.CassetteExternalNumber;
                IsMixed = "no";
                if (isStockTransactionRequired == false)
                {

                    Quantity = transactionLineType == Enums.TransactionLineType.Collect.ToString() ? Helper.GetRandomQuantityDistributionCollect(startDate) : Helper.GetRandomQuantityDistributionIssue(startDate);
                }
                else
                {
                    Quantity = stockPosition.Direction == (int)Direction.Issue ? stockPosition.Capacity : stockPosition.Direction == (int)Direction.Collect ? 0 : stockPosition.Capacity / 2;
                }
                // Quantity = isStockTransactionRequired == false ? Helper.GetRandomQuantityDistribution(startDate, stockPosition) : stockPosition.Direction == (int)Direction.Issue ? stockPosition.Capacity : stockPosition.Direction == (int)Direction.Collect ? 0 : stockPosition.Capacity / 2;
                MaterialID = stockPosition.MaterialId;
                ProductID = stockPosition.ProductId;
            
        }

       

       

    }
}
