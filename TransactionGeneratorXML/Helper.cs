using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionGeneratorXML.Model;

namespace TransactionGeneratorXML
{
    public static class Helper
    {

        private static readonly Random getrandom = new Random();

        public static string connectionStrg = "Data Source=portal;Initial Catalog=v5.03_SP3.LTF;Integrated Security=False;User ID=cwc-user2;Password=cwc-user2;Connection Timeout=60;";


        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }     

        public static string GetActualStockPositionListQuery(string cashPointNumber)
        {

            return " Select sp.StockPositionId, sp.Direction, m.denomination, sp.CassetteNumber, sp.CassetteExternalNumber, sp.Capacity, sp.MaterialId, sp.IsMixed, sp.Quantity from WP_StockPosition sp " +
                    " LEFT JOIN Material m on m.materialID = sp.MaterialId" +
                    $" where sp.CoinMachineId = (Select MachineId from WP_CoinMachine where Number = {cashPointNumber})" +
                    " AND sp.Type = 1 and sp.Totals = 1 ";
        }

        public static List<StockPosition> GetStockPositionsList(string cashPointNumber)
        {

            var stockPositionList = new List<StockPosition>();

            using (var connection = new SqlConnection(Helper.connectionStrg))
            {
                using (var command = new SqlCommand(GetActualStockPositionListQuery(cashPointNumber), connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //if stock position IsMixed = 'yes' skip creating transaction. 
                            //TO DO add creating transaction if mixed stock position doesn't have child 
                            if (reader.GetBoolean(7) == true) continue;

                            var stockPosition = new StockPosition()
                            {
                                StockPositionId = reader.GetInt32(0),
                                Direction = reader.GetInt32(1),
                                Denomination = reader.GetDecimal(2),
                                CassetteNumber = reader.GetInt32(3),
                                CassetteExternalNumber = reader.GetString(4),
                                Capacity = reader.GetInt32(5),
                                MaterialId = reader.GetString(6),
                                Quantity = reader.GetInt32(8)
                            };


                            stockPositionList.Add(stockPosition);
                        }
                    }
                }
            }
            return stockPositionList;
        }

        public static int GetRandomQuantity(DateTime startDate, int stockPositionCapacity)
        {
            // TODO getrandomNumber link to the stockPositionCapacity
            int quantity = 0;
            var weekdayName = startDate.DayOfWeek.ToString();
            var year = startDate.Year.ToString();

            if (weekdayName == "Monday") quantity = year == "2016" ? GetRandomNumber(30, 50) : year == "2017" ? (int)(GetRandomNumber(25, 50) * 1.10) : (int)(GetRandomNumber(30, 50) * 1.20);
            if (weekdayName == "Tuesday") quantity = year == "2016" ? GetRandomNumber(50, 80) : year == "2017" ? (int)(GetRandomNumber(45, 80) * 1.10) : (int)(GetRandomNumber(50, 80) * 1.20);
            if (weekdayName == "Wednesday") quantity = year == "2016" ? GetRandomNumber(100, 200) : year == "2017" ? (int)(GetRandomNumber(130, 200) * 1.10) : (int)(GetRandomNumber(100, 200) * 1.20);
            if (weekdayName == "Thursday") quantity = year == "2016" ? GetRandomNumber(70, 90) : year == "2017" ? (int)(GetRandomNumber(65, 90) * 1.10) : (int)(GetRandomNumber(70, 90) * 1.20);
            if (weekdayName == "Friday") quantity = year == "2016" ? GetRandomNumber(140, 180) : year == "2017" ? (int)(GetRandomNumber(145, 180) * 1.10) : (int)(GetRandomNumber(140, 180) * 1.20);
            if (weekdayName == "Saturday") quantity = year == "2016" ? GetRandomNumber(220, 280) : year == "2017" ? (int)(GetRandomNumber(220, 265) * 1.10) : (int)(GetRandomNumber(220, 280) * 1.20);
            if (weekdayName == "Sunday") quantity = year == "2016" ? GetRandomNumber(120, 170) : year == "2017" ? (int)(GetRandomNumber(120, 170) * 1.10) : (int)(GetRandomNumber(120, 170) * 1.20);

            return quantity;
        }

        public static int GetQuantityFromTransactionLinesList(List<TransactionLine> list)
        {
            var quantity = 0;

            foreach (var line in list)
            {
                quantity += line.TransactionLineType == "collect" ? line.Quantity * (-1) : line.Quantity;
            }

            return quantity;
        }

    }
}
