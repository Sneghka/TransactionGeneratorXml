using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TransactionGeneratorXML.Model
{
    public class UnencryptedContent
    {
        [XmlElement("WP_CM_Transaction")]
        public Transaction Transaction { get; set; }
    }
}
