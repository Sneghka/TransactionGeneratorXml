using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionGeneratorXML.Model
{
    public class ValidatedFeeding
    {
        public ValidatedFeeding()
        {
            UnencryptedContent = new UnencryptedContent();
        }
        public UnencryptedContent UnencryptedContent { get; set; }
    }
}
