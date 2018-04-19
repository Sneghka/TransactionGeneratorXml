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

            while (startDate < endDate)            {
                
                var transaction = new Transaction(stockPositionList,cashPointNumber, startDate);
                var rootElement = new RootElement();
                
                rootElement.ValidatedFeeding.UnencryptedContent.Transaction = transaction;
                rootElementList.Add(rootElement);
                Console.WriteLine(startDate + " Transaction type / quantity: " + transaction.TransactionType + " " + transaction.Quantity);

                if (transaction.IsStockTransactionRequired == true)
                {                   
                    var transactionStock = new Transaction(stockPositionList, cashPointNumber, startDate, true);
                    var rootElementStock = new RootElement();
                    
                    rootElementStock.ValidatedFeeding.UnencryptedContent.Transaction = transactionStock;
                    rootElementList.Add(rootElementStock);

                    Console.WriteLine(startDate + "Transaction type / quantity: " + transactionStock.TransactionType + " " + transactionStock.Quantity);
                }

                startDate = startDate.AddDays(1);
            }
        }
    }
}
