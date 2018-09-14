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
            var cashPointNumber = "7102";
            var startDate = new DateTime(2016, 01, 01);
            var endDate = new DateTime(2018, 09, 14);
            var pathFolder = $@"E:\Transactions\Transaction{cashPointNumber}_";
            var dbConnectionSC = Helper.connectionStrg_SC;
            var dbConnectionLTF = Helper.connectionStrgLTF;
            var dbConnectionV6_01 = Helper.dbConnectionV503HQ;

            var converter = new Converter();

            converter.CreateTransactions(cashPointNumber, startDate, endDate, pathFolder, dbConnectionV6_01);

            Console.ReadKey();
         
        }
    }
}
