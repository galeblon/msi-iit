using MsiImageIndexer.model;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MsiImageIndexer.exporters
{
    public class IndexedImagesXMLExporter : IIndexedImagesExporter
    {
        public void ExportIndexedImages(string filePath, PointCollection points, List<IndexedImage> indexedImages)
        {
            XmlDocument doc = new XmlDocument();

            var rootNode = doc.AppendChild(doc.CreateElement("images"));
            foreach (IndexedImage indexedImage in indexedImages)
            {
                var imageNode = rootNode.AppendChild(doc.CreateElement("image"));
                imageNode.AppendChild(doc.CreateElement("name")).AppendChild(doc.CreateTextNode(indexedImage.Image.OriginalString));
                foreach(NamedPoint point in points.Points) 
                {
                    var pointNode = imageNode.AppendChild(doc.CreateElement(point.Name.Replace(' ', '_')));
                    MarkedPoint mp = indexedImage.MarkedPoints.Where(mp => mp.NamedPoint.Equals(point)).SingleOrDefault();
                    double? x, y;
                    x = mp?.X;
                    y = mp?.Y;
                    pointNode.AppendChild(doc.CreateElement("x")).AppendChild(doc.CreateTextNode(x.ToString()));
                    pointNode.AppendChild(doc.CreateElement("y")).AppendChild(doc.CreateTextNode(y.ToString()));
                }
            }

            doc.Save(filePath);
        }
    }
}