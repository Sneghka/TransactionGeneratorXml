using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionGeneratorXML
{
    class Program
    {
        static void Main(string[] args)
        {
            var cashPointNumber = "7106";
            var startDate = new DateTime(2018, 07, 01);
            var endDate = new DateTime(2018, 08, 10);
            var pathFolder = $@"E:\Transactions\Transaction{cashPointNumber}_";
            var dbConnectionSC = Helper.connectionStrg_SC;
            var dbConnectionLTF = Helper.connectionStrgLTF;
            var dbConnectionV06SP1 = Helper.dbConnectionV06SP1;

            var converter = new Converter();

            converter.CreateTransactions(cashPointNumber, startDate, endDate, pathFolder, dbConnectionV06SP1);

            Console.ReadKey();
         
        }
    }
}
