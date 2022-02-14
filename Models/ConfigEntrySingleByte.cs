using System.Xml.Serialization;

namespace RFLobbyModifier.Models
{
    public class ConfigEntrySingleByte
    {
        [XmlAttribute]
        public bool Edit { get; set; }
        [XmlAttribute]
        public byte Value { get; set; }
        public ConfigEntrySingleByte()
        {
            
        }

        public ConfigEntrySingleByte(bool edit, byte value)
        {
            Edit = edit;
            Value = value;
        }
    }
}