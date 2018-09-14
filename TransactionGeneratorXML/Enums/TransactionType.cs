using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionGeneratorXML.Enums
{
    public enum TransactionType
    {
        /// <summary>
        /// Issue = 0
        /// </summary>
        Issue = 0,
        /// <summary>
        /// Collect = 1
        /// </summary>
        Collect = 1,
        /// <summary>
        /// Stock = 2
        /// </summary>
        Stock = 2,
        /// <summary>
        /// Validate = 3
        /// </summary>
        Validate = 3
    }
}
