using System.Xml.Serialization;

namespace RFLobbyModifier.Models
{
    public class ConfigEntrySingleString
    {
        [XmlAttribute]
        public bool Edit { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
        public ConfigEntrySingleString()
        {
            
        }

        public ConfigEntrySingleString(bool edit, string value)
        {
            Edit = edit;
            Value = value;
        }
    }
}