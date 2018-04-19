using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionGeneratorXML.Model;

namespace TransactionGeneratorXML
{

    public class DataGenerator
    {

        public void GenerateData(List<RootElement> rootElementList, string cashPointNumber, DateTime startDate, DateTime endDate)
        {
            var stockPositionList = Helper.GetStockPositionsList(cashPointNumber);

            while (startDate < endDate)
            {
                var transactionLinesList = new List<TransactionLine>();
                bool isStockTransactionRequired = false;

                //create transaction lines for current date for each cash point stock position
                //add transaction lines to the list
                foreach (var position in stockPositionList)
                {
                    int localQuantity = 0;
                    int localRecycleQuantity = 0;

                    //if stock position direction = recycle then create two transaction lines 
                    if (position.Direction == 3)
                    {
                        var transactionLineIssue = new TransactionLine()
                        {
                            TransactionLineType = "issue",
                            StockPositionDirection = "recycle",
                            CassetteNumber = position.CassetteNumber,
                            CassetteExternalNumber = position.CassetteExternalNumber,
                            IsMixed = "no",
                            Quantity = Helper.GetRandomQuantity(startDate, position.Capacity),
                            MaterialID = position.MaterialId
                        };

                        var transactionLineCollect = new TransactionLine()
                        {
                            TransactionLineType = "collect",
                            StockPositionDirection = "recycle",
                            CassetteNumber = position.CassetteNumber,
                            CassetteExternalNumber = position.CassetteExternalNumber,
                            IsMixed = "no",
                            Quantity = Helper.GetRandomQuantity(startDate, position.Capacity),
                            MaterialID = position.MaterialId
                        };

                        transactionLinesList.Add(transactionLineIssue);
                        transactionLinesList.Add(transactionLineCollect);

                        localRecycleQuantity = transactionLineCollect.Quantity - transactionLineIssue.Quantity;
                        position.Quantity += localRecycleQuantity;
                    }

                    if (position.Direction != 3)
                    {
                        var transactionLine = new TransactionLine()
                        {
                            TransactionLineType = position.Direction == 1 ? "issue" : "collect",
                            StockPositionDirection = position.Direction == 1 ? "issue" : "collect",
                            CassetteNumber = position.CassetteNumber,
                            CassetteExternalNumber = position.CassetteExternalNumber,
                            IsMixed = "no",
                            Quantity = Helper.GetRandomQuantity(startDate, position.Capacity),
                            MaterialID = position.MaterialId
                        };
                        transactionLinesList.Add(transactionLine);

                        localQuantity = transactionLine.Quantity;
                        position.Quantity = position.Direction == 1 ? position.Quantity - localQuantity : position.Quantity + localQuantity;
                    }

                    if ((position.Quantity - localQuantity < 0 && (position.Direction == 1 || position.Direction == 3)) || (position.Quantity + localQuantity > position.Capacity && (position.Direction == 2 || position.Direction == 3))) isStockTransactionRequired = true;
                }

                var transactionQuantity = Helper.GetQuantityFromTransactionLinesList(transactionLinesList);

                //create tranaction
                //add transactionLineList
                var transaction = new Transaction()
                {
                    MachineNumber = cashPointNumber,
                    StartDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Quantity = transactionQuantity,
                    Replenishment = "no",
                    TransactionType = transactionQuantity >= 0 ? "issue" : "collect"
                };

                var rootElement = new RootElement();
                transaction.TransactionLinesList = transactionLinesList;
                rootElement.ValidatedFeeding.UnencryptedContent.Transaction = transaction;
                rootElementList.Add(rootElement);

                if (isStockTransactionRequired == true)
                {

                    var transactionLinesListForStock = new List<TransactionLine>();

                    foreach (var position in stockPositionList)
                    {
                        var transactionLine = new TransactionLine()
                        {
                            TransactionLineType = position.Direction == 1 ? "issue" : position.Direction == 2 ? "collect" : "issue", //in case Recycle type of stockPosition set ISSUE Transaction Line Type
                            StockPositionDirection = position.Direction == 1 ? "issue" : position.Direction == 2 ? "collect" : "recycle",
                            CassetteNumber = position.CassetteNumber,
                            CassetteExternalNumber = position.CassetteExternalNumber,
                            IsMixed = "no",
                            Quantity = position.Direction == 1 ? position.Capacity : position.Direction == 2 ? 0 : position.Capacity / 2,
                            MaterialID = position.MaterialId
                        };

                        position.Quantity = position.Direction == 1 ? position.Capacity : position.Direction == 2 ? 0 : position.Capacity / 2;
                        transactionLinesListForStock.Add(transactionLine);
                    }

                    var transactionQuantityStock = Helper.GetQuantityFromTransactionLinesList(transactionLinesListForStock);
                    var startDateStock = startDate.AddDays(-1).AddSeconds(10).ToString("yyyy-MM-dd HH:mm:ss");

                    var transactionStock = new Transaction()
                    {
                        MachineNumber = cashPointNumber,
                        StartDate = startDateStock,
                        Quantity = transactionQuantityStock,
                        Replenishment = "no",
                        TransactionType = "stock"
                    };

                    var rootElementStock = new RootElement();
                    transactionStock.TransactionLinesList = transactionLinesListForStock;
                    rootElementStock.ValidatedFeeding.UnencryptedContent.Transaction = transactionStock;
                    rootElementList.Add(rootElementStock);

                }

                startDate = startDate.AddDays(1);
            }
        }
    }
}
