using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionGeneratorXML.Enums;
using TransactionGeneratorXML.Model;

namespace TransactionGeneratorXML
{

    public class DataGenerator
    {

        public void GenerateData(List<RootElement> rootElementList, string cashPointNumber, DateTime startDate, DateTime endDate, string dbConnection)
        {
            var stockPositionList = Helper.GetStockPositionsList(cashPointNumber, dbConnection);

            while (startDate < endDate)
            {
                var transactionLinesList = new List<TransactionLine>();
                bool isStockTransactionRequired = false;

                CreateTransactionLinesList(stockPositionList, startDate, ref isStockTransactionRequired, transactionLinesList);

                var transaction = new Transaction(cashPointNumber, startDate, transactionLinesList);
                var rootElement = new RootElement();

                rootElement.ValidatedFeeding.UnencryptedContent.Transaction = transaction;
                rootElementList.Add(rootElement);
                Console.WriteLine(startDate + " Transaction type / quantity: " + transaction.TransactionType + " " + transaction.Quantity);

                if (isStockTransactionRequired)
                {
                    var transactionLinesListForStock = new List<TransactionLine>();

                    CreateTransactionLinesList(stockPositionList, startDate, ref isStockTransactionRequired, transactionLinesListForStock);

                    var transactionStock = new Transaction(cashPointNumber, startDate, transactionLinesListForStock, true);
                    var rootElementStock = new RootElement();

                    rootElementStock.ValidatedFeeding.UnencryptedContent.Transaction = transactionStock;
                    rootElementList.Add(rootElementStock);

                    Console.WriteLine(startDate + "Transaction type / quantity: " + transactionStock.TransactionType + " " + transactionStock.Quantity);
                }

                startDate = startDate.AddDays(1);
            }
        }

        void CreateTransactionLinesList(List<StockPosition> stockPositionList, DateTime startDate, ref bool isStockTransactionRequired, List<TransactionLine> transactionLinesList)
        {
            var localIsStockTransactionRequired = false;

            foreach (var position in stockPositionList)
            {
                // generate transaction for the certain Stock Position
                //if (position.StockPositionId != 8003738) continue;

                    int localQuantity = 0;                

                //if stock position direction = recycle then create two transaction lines 
                if (position.Direction == (int)Direction.Recycle)
                {
                    
                    var transactionLineIssue = new TransactionLine(position, "Issue", "Recycle", startDate, isStockTransactionRequired);
                    var transactionLineCollect = new TransactionLine(position, "Collect", "Recycle", startDate, isStockTransactionRequired);

                    transactionLinesList.Add(transactionLineIssue);
                    if (!isStockTransactionRequired) transactionLinesList.Add(transactionLineCollect);

                    localQuantity = transactionLineCollect.Quantity - transactionLineIssue.Quantity;

                    if (!isStockTransactionRequired)
                    {
                        position.Quantity += localQuantity;
                    }
                    else
                    {
                        position.Quantity = position.Capacity / 2;
                    }
                }
                else
                {
                    var lineType = position.Direction == (int)Direction.Issue ? "Issue" : "Collect"; // line type depends on stock position direction
                    var direction = position.Direction == (int)Direction.Issue ? "issue" : "collect";

                    var transactionLine = new TransactionLine(position, lineType, direction, startDate, isStockTransactionRequired);
                    transactionLinesList.Add(transactionLine);

                    localQuantity = transactionLine.Quantity;
                    if (!isStockTransactionRequired)
                    {
                        position.Quantity = position.Direction == (int)Direction.Issue ? position.Quantity - localQuantity : position.Quantity + localQuantity;
                    }
                    else
                    {
                        position.Quantity = position.Direction == (int)Direction.Issue ? position.Capacity : 0;
                    }
                }

                if (!localIsStockTransactionRequired) localIsStockTransactionRequired = Helper.GetIsStockTransactionRequired(position, localQuantity);

            }

            if(localIsStockTransactionRequired)isStockTransactionRequired = true;
        }
    }
}
