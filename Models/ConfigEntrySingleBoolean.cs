using System.Xml.Serialization;

namespace RFLobbyModifier.Models
{
    public class ConfigEntrySingleBoolean
    {
        [XmlAttribute]
        public bool Edit { get; set; }
        [XmlAttribute]
        public bool Value { get; set; }
        public ConfigEntrySingleBoolean()
        {
            
        }

        public ConfigEntrySingleBoolean(bool edit, bool value)
        {
            Edit = edit;
            Value = value;
        }
    }
}