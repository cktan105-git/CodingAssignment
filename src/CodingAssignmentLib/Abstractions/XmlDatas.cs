using System.Xml.Serialization;

namespace CodingAssignmentLib.Abstractions
{
    [XmlRoot("Datas")]
    public class XmlDatas
    {
        [XmlElement("Data")]
        public List<XmlData> DataList { get; set; } = new List<XmlData>();
    }

    public class XmlData
    {
        [XmlElement("Key")]
        public string? Key { get; set; }

        [XmlElement("Value")]
        public string? Value { get; set; }
    }
}
