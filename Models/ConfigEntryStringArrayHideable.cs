using System.Xml.Serialization;

namespace RFLobbyModifier.Models
{
    public class ConfigEntryStringArrayHideable
    {
        [XmlAttribute]
        public bool Hide { get; set; }
        [XmlAttribute]
        public bool Edit { get; set; }
        [XmlArrayItem("Text")]
        public string[] Values { get; set; }
        public ConfigEntryStringArrayHideable()
        {
            
        }

        public ConfigEntryStringArrayHideable(bool hide, bool edit, string[] values)
        {
            Hide = hide;
            Edit = edit;
            Values = values;
        }
    }
}