using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TransactionGeneratorXML.Model
{
    [XmlRoot(ElementName = "DocumentElement")]
    public class RootElement
    {
        public RootElement()
        {
            ValidatedFeeding = new ValidatedFeeding();
        }

        [XmlElement("WP_ValidatedFeeding")]
        public ValidatedFeeding ValidatedFeeding { get; set; }
    }
}
