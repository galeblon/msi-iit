using MsiImageIndexer.model;
using System.Collections.Generic;
using System.Linq;

namespace MsiImageIndexer.exporters
{
    public class IndexedImagesCSVExporter : IIndexedImagesExporter
    {
        public void ExportIndexedImages(string filePath, PointCollection points, List<IndexedImage> indexedImages)
        {
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(filePath)) 
            {
                // Header row
                writer.Write("\"file name\"");
                foreach(NamedPoint point in points.Points) 
                {
                    writer.Write($";\"{point.Name} X\"");
                    writer.Write($";\"{point.Name} Y\"");
                }
                writer.WriteLine();

                // Fill the rest
                foreach(IndexedImage indexedImage in indexedImages) 
                {
                    writer.Write($"\"{indexedImage.Image.LocalPath}\"");
                    foreach(NamedPoint point in points.Points) 
                    {
                        MarkedPoint mp = indexedImage.MarkedPoints.Where(mp => mp.NamedPoint.Equals(point)).SingleOrDefault();
                        double? x, y;
                        x = mp?.X;
                        y = mp?.Y;
                        writer.Write($";{x};{y}");
                    }
                    writer.WriteLine();
                }

            }
        }
    }
}
