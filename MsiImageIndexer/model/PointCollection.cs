using System;
using System.Xml.Serialization;

namespace MsiImageIndexer.model
{
    public class Point
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("description")]
        public string Description { get; set; }
        [XmlElement("imageRef")]
        public string ImageRefPath { get; set; }
    }

    [Serializable()]
    [XmlRoot("pointsCollection")]
    public class PointCollection
    {
        [XmlArray("points")]
        [XmlArrayItem("point", typeof(Point))]
        public Point[] Points { get; set; }
    }
}
