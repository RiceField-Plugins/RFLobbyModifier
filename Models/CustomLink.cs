using System.Xml.Serialization;

namespace RFLobbyModifier.Models
{
    public class CustomLink
    {
        [XmlAttribute]
        public string Message { get; set; }
        [XmlAttribute]
        public string Url { get; set; }

        public CustomLink()
        {
            
        }

        public CustomLink(string message, string url)
        {
            Message = message;
            Url = url;
        }
    }
}