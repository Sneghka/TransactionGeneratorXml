using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionGeneratorXML.Model
{
    public class StockPosition
    {
        public int StockPositionId { get; set; }
        public int Direction { get; set; }    // Issue -1 , Collect - 2, Recycle - 3
        public decimal Denomination { get; set; }
        public int CassetteNumber { get; set; }
        public string CassetteExternalNumber { get; set; }
        public int Capacity { get; set; }
        public string MaterialId { get; set; }

        public int Quantity { get; set; }
    }
}
