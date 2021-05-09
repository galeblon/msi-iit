using MsiImageIndexer.model;
using System.Collections.Generic;

namespace MsiImageIndexer.exporters
{
    public interface IIndexedImagesExporter
    {
        public void ExportIndexedImages(string fileName, PointCollection points, List<IndexedImage> indexedImages);
    }
}
