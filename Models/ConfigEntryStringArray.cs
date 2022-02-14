using System.Xml.Serialization;

namespace RFLobbyModifier.Models
{
    public class ConfigEntryStringArray
    {
        [XmlAttribute]
        public bool Edit { get; set; }
        [XmlArrayItem("Text")]
        public string[] Values { get; set; }
        public ConfigEntryStringArray()
        {
            
        }

        public ConfigEntryStringArray(bool edit, string[] values)
        {
            Edit = edit;
            Values = values;
        }
    }
}