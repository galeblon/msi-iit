using System.Windows.Media;

namespace MsiImageIndexer.model
{
    public class MarkedPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Color Colour { get; set; }
        public NamedPoint NamedPoint {get; set;}
    }
}
