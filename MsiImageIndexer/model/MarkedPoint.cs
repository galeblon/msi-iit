using System.Windows.Media;

namespace MsiImageIndexer.model
{
    public class MarkedPoint
    {
        double X { get; set; }
        double Y { get; set; }
        Colors Colour { get; set; }
        NamedPoint NamedPoint {get; set;}
    }
}
