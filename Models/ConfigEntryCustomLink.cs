using System.Collections.Generic;
using System.Xml.Serialization;

namespace RFLobbyModifier.Models
{
    public class ConfigEntryCustomLink
    {
        [XmlAttribute]
        public bool Hide { get; set; }
        [XmlAttribute]
        public bool Edit { get; set; }
        [XmlArrayItem("Link")]
        public List<CustomLink> Values { get; set; }

        public ConfigEntryCustomLink()
        {
            
        }

        public ConfigEntryCustomLink(bool hide, bool edit, List<CustomLink> values)
        {
            Hide = hide;
            Edit = edit;
            Values = values;
        }
    }
}