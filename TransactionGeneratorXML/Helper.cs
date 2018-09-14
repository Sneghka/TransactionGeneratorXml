using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionGeneratorXML.Enums;
using TransactionGeneratorXML.Model;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;

namespace TransactionGeneratorXML
{
    public static class DataReaderExtensions
    {
        public static string GetStringOrNull(this IDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        public static string GetStringOrNull(this IDataReader reader, string columnName)
        {
            return reader.GetStringOrNull(reader.GetOrdinal(columnName));
        }

    }
    public static class Helper
    {



        private static readonly Random getrandom = new Random();

        public static string connectionStrgLTF = "Data Source=portal;Initial Catalog=v5.03_SP3.LTF;Integrated Security=False;User ID=cwc-user2;Password=cwc-user2;Connection Timeout=60;";
        public static string connectionStrg_SC = "Data Source=portal;Initial Catalog=SC_v5.04_HQ;Integrated Security=False;User ID=cwc-user2;Password=cwc-user2;Connection Timeout=60;";
        public static string dbConnectionV06SP1 = "Data Source=portal;Initial Catalog=v6.01_SP1_HQ;Integrated Security=False;User ID=cwc-user2;Password=cwc-user2;Connection Timeout=60;";
        public static string dbConnectionV503HQ = "Data Source=portal;Initial Catalog=Edsson_WebPortal_v5.03_HQ;Integrated Security=False;User ID=cwc-user2;Password=cwc-user2;Connection Timeout=60;";

        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }

        public static double GetRandomNumberDouble()
        {
            lock (getrandom) // synchronize
            {
                return getrandom.NextDouble()*0.9998+0.0001;
            }
        }

        public static string GetActualStockPositionListQuery(string cashPointNumber)
        {

            return " Select sp.StockPositionId, sp.Direction, m.denomination, sp.CassetteNumber, sp.CassetteExternalNumber, sp.Capacity, sp.MaterialId, sp.IsMixed, sp.Quantity, sp.ProductCode from WP_StockPosition sp " +
                    " LEFT JOIN Material m on m.materialID = sp.MaterialId" +
                    " LEFT JOIN Product pr on pr.ProductCode = sp.ProductCode" +
                    $" where sp.CoinMachineId = (Select MachineId from WP_CoinMachine where Number = '{cashPointNumber}')" +
                    " AND sp.Type = 1 and sp.Totals = 1 ";
        }

        public static List<StockPosition> GetStockPositionsList(string cashPointNumber, string dbConnection)
        {

            var stockPositionList = new List<StockPosition>();

            using (var connection = new SqlConnection(dbConnection))
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
                                Denomination = reader.IsDBNull(2) ? null : (decimal?)reader.GetDecimal(2),
                                   //Denomination = reader.GetDecimal(2),
                                CassetteNumber = reader.GetInt32(3),
                                CassetteExternalNumber = reader.GetStringOrNull(4),
                                Capacity = reader.GetInt32(5),
                                MaterialId = reader.GetStringOrNull(6),
                                ProductId = reader.GetStringOrNull(9),
                                    //MaterialId = reader.GetString(6) == null ? string.Empty : reader.GetString(6),
                                Quantity = reader.GetInt32(8),
                                    //ProductId = reader.GetString(9) == null ? string.Empty : reader.GetString(9)
                            };


                            stockPositionList.Add(stockPosition);
                        }
                    }
                }
            }
            return stockPositionList;
        }

        public static int GetRandomQuantityIssue(DateTime startDate, StockPosition stockPosition)
        {
            // TODO getrandomNumber link to the stockPositionCapacity
            int capacity = stockPosition.Capacity;
            int quantity = 0;
            var weekdayName = startDate.DayOfWeek.ToString();
            var year = startDate.Year;
            var weekDayNameQuantity = 0;
            var season = startDate.Month == 9 || startDate.Month == 10 || startDate.Month == 11 ? "Autumn" : startDate.Month == 12 || startDate.Month == 1 || startDate.Month == 2 ? "Winter" : startDate.Month == 3 || startDate.Month == 4 || startDate.Month == 5 ? "Spring" : "Summer";
            var seasoncoefficient = 1.00m;
            var yearCofficient = 1.00m;


            switch (weekdayName)
            {
                case "Monday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.05), (int)(capacity * 0.15));
                    break;
                case "Tuesday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.15), (int)(capacity * 0.25));
                    break;
                case "Wednesday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.09), (int)(capacity * 0.20));
                    break;
                case "Thursday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.11), (int)(capacity * 0.28));
                    break;
                case "Friday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.2), (int)(capacity * 0.35));
                    break;
                case "Saturday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.3), (int)(capacity * 0.6));
                    break;
                case "Sunday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.2), (int)(capacity * 0.7));
                    break;
                default:
                    weekDayNameQuantity = 0;
                    break;
            }

            switch (season)
            {
                case "Autumn":
                    seasoncoefficient = 1.4m;
                    break;
                case "Winter":
                    seasoncoefficient = 0.8m;
                    break;
                case "Spring":
                    seasoncoefficient = 1.3m;
                    break;
                case "Summer":
                    seasoncoefficient = 1.2m;
                    break;
                default:
                    seasoncoefficient = 1.0m;
                    break;
            }

            switch (year)
            {
                case 2015:
                    yearCofficient = 0.8m;
                    break;
                case 2016:
                    yearCofficient = 1.1m;
                    break;
                case 2017:
                    yearCofficient = 0.9m;
                    break;
                case 2018:
                    yearCofficient = 1.3m;
                    break;
                default:
                    yearCofficient = 1.0m;
                    break;
            }

            quantity = (int)(weekDayNameQuantity * seasoncoefficient * yearCofficient);

            //if (weekdayName == "Monday") quantity = year == 2016 ? GetRandomNumber(10, 50) : year == 2017 ? (int)(GetRandomNumber(30, 50) * 1.30) : (int)(GetRandomNumber(30, 50) * 1.15);
            //if (weekdayName == "Tuesday") quantity = year == 2016 ? GetRandomNumber(60, 100) : year == 2017 ? (int)(GetRandomNumber(50, 80) * 1.30) : (int)(GetRandomNumber(50, 80) * 1.15);
            //if (weekdayName == "Wednesday") quantity = year == 2016 ? GetRandomNumber(130, 200) : year == 2017 ? (int)(GetRandomNumber(100, 200) * 1.30) : (int)(GetRandomNumber(100, 200) * 1.15);
            //if (weekdayName == "Thursday") quantity = year == 2016 ? GetRandomNumber(70, 100) : year == 2017 ? (int)(GetRandomNumber(70, 90) * 1.30) : (int)(GetRandomNumber(70, 90) * 1.15);
            //if (weekdayName == "Friday") quantity = year == 2016 ? GetRandomNumber(140, 180) : year == 2017 ? (int)(GetRandomNumber(145, 180) * 1.30) : (int)(GetRandomNumber(140, 180) * 1.15);
            //if (weekdayName == "Saturday") quantity = year == 2016 ? GetRandomNumber(220, 280) : year == 2017 ? (int)(GetRandomNumber(220, 280) * 1.30) : (int)(GetRandomNumber(220, 280) * 1.15);
            //if (weekdayName == "Sunday") quantity = year == 2016 ? GetRandomNumber(120, 170) : year == 2017 ? (int)(GetRandomNumber(120, 170) * 1.30) : (int)(GetRandomNumber(120, 170) * 1.15);


            return stockPosition.ProductId != null ? quantity < 100 ? quantity / 10 : quantity / 100 : quantity;
        }

        public static int GetRandomQuantityCollect(DateTime startDate, StockPosition stockPosition)
        {
            // TODO getrandomNumber link to the stockPositionCapacity
            int capacity = stockPosition.Capacity;
            int quantity = 0;
            var weekdayName = startDate.DayOfWeek.ToString();
            var year = startDate.Year;
            var weekDayNameQuantity = 0;
            var season = startDate.Month == 9 || startDate.Month == 10 || startDate.Month == 11 ? "Autumn" : startDate.Month == 12 || startDate.Month == 1 || startDate.Month == 2 ? "Winter" : startDate.Month == 3 || startDate.Month == 4 || startDate.Month == 5 ? "Spring" : "Summer";
            var seasoncoefficient = 1.00m;
            var yearCofficient = 1.00m;


            switch (weekdayName)
            {
                case "Monday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.07), (int)(capacity * 0.1));
                    break;
                case "Tuesday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.03), (int)(capacity * 0.09));
                    break;
                case "Wednesday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.09), (int)(capacity * 0.15));
                    break;
                case "Thursday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.1), (int)(capacity * 0.25));
                    break;
                case "Friday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.07), (int)(capacity * 0.13));
                    break;
                case "Saturday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.15), (int)(capacity * 0.3));
                    break;
                case "Sunday":
                    weekDayNameQuantity = GetRandomNumber((int)(capacity * 0.3), (int)(capacity * 0.6));
                    break;
                default:
                    weekDayNameQuantity = 0;
                    break;
            }

            switch (season)
            {
                case "Autumn":
                    seasoncoefficient = 0.4m;
                    break;
                case "Winter":
                    seasoncoefficient = 1.8m;
                    break;
                case "Spring":
                    seasoncoefficient = 1.3m;
                    break;
                case "Summer":
                    seasoncoefficient = 1m;
                    break;
                default:
                    seasoncoefficient = 1.0m;
                    break;
            }

            switch (year)
            {
                case 2015:
                    yearCofficient = 0.8m;
                    break;
                case 2016:
                    yearCofficient = 1.1m;
                    break;
                case 2017:
                    yearCofficient = 0.9m;
                    break;
                case 2018:
                    yearCofficient = 1.3m;
                    break;
                default:
                    yearCofficient = 1.0m;
                    break;
            }

            quantity = (int)(weekDayNameQuantity * seasoncoefficient * yearCofficient);

            //if (weekdayName == "Monday") quantity = year == 2016 ? GetRandomNumber(10, 50) : year == 2017 ? (int)(GetRandomNumber(30, 50) * 1.30) : (int)(GetRandomNumber(30, 50) * 1.15);
            //if (weekdayName == "Tuesday") quantity = year == 2016 ? GetRandomNumber(60, 100) : year == 2017 ? (int)(GetRandomNumber(50, 80) * 1.30) : (int)(GetRandomNumber(50, 80) * 1.15);
            //if (weekdayName == "Wednesday") quantity = year == 2016 ? GetRandomNumber(130, 200) : year == 2017 ? (int)(GetRandomNumber(100, 200) * 1.30) : (int)(GetRandomNumber(100, 200) * 1.15);
            //if (weekdayName == "Thursday") quantity = year == 2016 ? GetRandomNumber(70, 100) : year == 2017 ? (int)(GetRandomNumber(70, 90) * 1.30) : (int)(GetRandomNumber(70, 90) * 1.15);
            //if (weekdayName == "Friday") quantity = year == 2016 ? GetRandomNumber(140, 180) : year == 2017 ? (int)(GetRandomNumber(145, 180) * 1.30) : (int)(GetRandomNumber(140, 180) * 1.15);
            //if (weekdayName == "Saturday") quantity = year == 2016 ? GetRandomNumber(220, 280) : year == 2017 ? (int)(GetRandomNumber(220, 280) * 1.30) : (int)(GetRandomNumber(220, 280) * 1.15);
            //if (weekdayName == "Sunday") quantity = year == 2016 ? GetRandomNumber(120, 170) : year == 2017 ? (int)(GetRandomNumber(120, 170) * 1.30) : (int)(GetRandomNumber(120, 170) * 1.15);


            return stockPosition.ProductId != null ? quantity < 100 ? quantity / 10 : quantity / 100 : quantity;
        }


        public static int GetRandomQuantityDistributionIssue(DateTime startDate)
        {            
            int quantity = 0;
            var weekdayName = startDate.DayOfWeek.ToString();
            var year = startDate.Year;
            var weekDayNameQuantity = 0;           
            var yearCofficient = 1.00m;
            var seasoncoefficientWinter = 0.4m;
            var seasoncoefficientSpring = 1.3m;
            var seasoncoefficientSummer = 1m;
            var seasoncoefficientAutumn = 2m;


            switch (weekdayName)
            {
                case "Monday":
                    weekDayNameQuantity = 200;
                    break;
                case "Tuesday":
                    weekDayNameQuantity = 130;
                    break;
                case "Wednesday":
                    weekDayNameQuantity = 240;
                    break;
                case "Thursday":
                    weekDayNameQuantity = 180;
                    break;
                case "Friday":
                    weekDayNameQuantity = 300;
                    break;
                case "Saturday":
                    weekDayNameQuantity = 425;
                    break;
                case "Sunday":
                    weekDayNameQuantity = 360;
                    break;
                default:
                    weekDayNameQuantity = 0;
                    break;

            }

            switch (year)
            {
                case 2015:
                    yearCofficient = 0.8m;
                    break;
                case 2016:
                    yearCofficient = 1m;
                    break;
                case 2017:
                    yearCofficient = 0.9m;
                    break;
                case 2018:
                    yearCofficient = 1.3m;
                    break;
                default:
                    yearCofficient = 1.0m;
                    break;
            }            

            // *************************************************
            var uniformPropbability = GetRandomNumberDouble();
            var pList = new List<double>();
            decimal weight_winter = 0;
            decimal weight_autumn = 0;

            if (startDate.Date > new DateTime(year, 09, 01))
            {
                weight_winter = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year + 1, 1, 13)).Days)) / 90, 0);
            }
            else {
                weight_winter = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year, 1, 13)).Days)) / 90, 0);
            }

            var weight_spring = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year, 4, 16)).Days)) / 92, 0);
            
            var weight_summer = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year, 7, 16)).Days)) / 92, 0);

            if (startDate.Date < new DateTime(year, 04, 01))
            {
                weight_autumn = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year - 1, 10, 15)).Days)) / 91, 0);
            }
            else {
                weight_autumn = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year , 10, 15)).Days)) / 91, 0);
            }

            var seasonCoefficient = seasoncoefficientWinter * weight_winter + seasoncoefficientSpring * weight_spring + seasoncoefficientSummer * weight_summer + seasoncoefficientAutumn * weight_autumn;

            var expectedQuantity = weekDayNameQuantity * seasonCoefficient * yearCofficient;

            var Chart1 = new Chart();
            double zValueNormalDistribution = Chart1.DataManipulator.Statistics.InverseNormalDistribution(uniformPropbability);

            quantity = (int)(expectedQuantity + (decimal)(Math.Sqrt((double)expectedQuantity) * zValueNormalDistribution));

            //quantity = 0;

            return quantity;

        }

        public static int GetRandomQuantityDistributionCollect(DateTime startDate)
        {
            int quantity = 0;
            var weekdayName = startDate.DayOfWeek.ToString();
            var year = startDate.Year;
            var weekDayNameQuantity = 0;
            var yearCofficient = 1.00m;
            var seasoncoefficientWinter = 0.7m;
            var seasoncoefficientSpring = 3m;
            var seasoncoefficientSummer = 1m;
            var seasoncoefficientAutumn = 1.1m;


            switch (weekdayName)
            {
                case "Monday":
                    weekDayNameQuantity = 150;
                    break;
                case "Tuesday":
                    weekDayNameQuantity = 190;
                    break;
                case "Wednesday":
                    weekDayNameQuantity = 80;
                    break;
                case "Thursday":
                    weekDayNameQuantity = 250;
                    break;
                case "Friday":
                    weekDayNameQuantity = 165;
                    break;
                case "Saturday":
                    weekDayNameQuantity = 360;
                    break;
                case "Sunday":
                    weekDayNameQuantity = 425;
                    break;
                default:
                    weekDayNameQuantity = 0;
                    break;

            }

            switch (year)
            {
                case 2015:
                    yearCofficient = 0.8m;
                    break;
                case 2016:
                    yearCofficient = 1m;
                    break;
                case 2017:
                    yearCofficient = 0.9m;
                    break;
                case 2018:
                    yearCofficient = 1.3m;
                    break;
                default:
                    yearCofficient = 1.0m;
                    break;
            }

            // *************************************************
            var uniformPropbability = GetRandomNumberDouble();
            var pList = new List<double>();
            decimal weight_winter = 0;
            decimal weight_autumn = 0;

            if (startDate.Date > new DateTime(year, 09, 01))
            {
                weight_winter = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year + 1, 1, 13)).Days)) / 90, 0);
            }
            else
            {
                weight_winter = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year, 1, 13)).Days)) / 90, 0);
            }

            var weight_spring = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year, 4, 16)).Days)) / 92, 0);

            var weight_summer = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year, 7, 16)).Days)) / 92, 0);

            if (startDate.Date < new DateTime(year, 04, 01))
            {
                weight_autumn = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year - 1, 10, 15)).Days)) / 91, 0);
            }
            else
            {
                weight_autumn = Math.Max(1 - (decimal)(Math.Abs((startDate - new DateTime(year, 10, 15)).Days)) / 91, 0);
            }

            var seasonCoefficient = seasoncoefficientWinter * weight_winter + seasoncoefficientSpring * weight_spring + seasoncoefficientSummer * weight_summer + seasoncoefficientAutumn * weight_autumn;

            var expectedQuantity = weekDayNameQuantity * seasonCoefficient * yearCofficient;

            var Chart1 = new Chart();
            double zValueNormalDistribution = Chart1.DataManipulator.Statistics.InverseNormalDistribution(uniformPropbability);

            quantity = (int)(expectedQuantity + (decimal)(Math.Sqrt((double)expectedQuantity) * zValueNormalDistribution));

            //quantity = 100;
            //quantity = (int)(expectedQuantity + (decimal)(Math.Sqrt((double)expectedQuantity) * zValueNormalDistribution));

            return quantity;

        }



        public static int GetQuantityFromTransactionLinesList(List<TransactionLine> list)
        {
            var quantity = 0;

            foreach (var line in list)
            {
                quantity += line.TransactionLineType == "Issue" ? line.Quantity * (-1) : line.Quantity;
            }

            return Math.Abs(quantity);
        }

        public static bool GetIsStockTransactionRequired(StockPosition position, int localQuantity)
        {
            var cassetteQuantity = 0;
            if (position.Direction == (int)Direction.Issue || position.Direction == (int)Direction.Recycle) cassetteQuantity = position.Quantity - localQuantity;
            if (position.Direction == (int)Direction.Collect || position.Direction == (int)Direction.Recycle) cassetteQuantity = position.Quantity + localQuantity;

            if ((cassetteQuantity < 0 && (position.Direction == (int)Direction.Issue || position.Direction == (int)Direction.Recycle)) || (cassetteQuantity > position.Capacity && (position.Direction == (int)Direction.Collect || position.Direction == (int)Direction.Recycle)))
            {
                return true;
            }

            return false;
        }

   


    }
}
