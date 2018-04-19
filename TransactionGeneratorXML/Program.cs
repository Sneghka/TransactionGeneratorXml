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
            var cashPointNumber = "7202";
            var startDate = new DateTime(2018, 03, 01);
            var endDate = new DateTime(2018, 04, 13);
            var pathFolder = $@"E:\Transactions\Transaction{cashPointNumber}_";

            var converter = new Converter();

            converter.CreateTransactions(cashPointNumber, startDate, endDate, pathFolder);

            Console.ReadKey();
         
        }
    }
}
