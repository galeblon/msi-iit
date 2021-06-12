using System;
using System.Windows.Media;

namespace MsiImageIndexer.model
{
    [Serializable]
    public class MarkedPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        [NonSerialized]
        private Color colour;
        public Color Colour 
        {
            get { return colour; }
            set { colour = value; }
        }
        public NamedPoint NamedPoint {get; set;}
    }
}
