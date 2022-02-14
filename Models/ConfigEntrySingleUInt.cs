using System.Xml.Serialization;

namespace RFLobbyModifier.Models
{
    public class ConfigEntrySingleUInt
    {
        [XmlAttribute]
        public bool Edit { get; set; }
        [XmlAttribute]
        public uint Value { get; set; }
        public ConfigEntrySingleUInt()
        {
            
        }

        public ConfigEntrySingleUInt(bool edit, uint value)
        {
            Edit = edit;
            Value = value;
        }
    }
}