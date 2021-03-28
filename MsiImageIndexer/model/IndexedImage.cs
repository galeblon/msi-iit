using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MsiImageIndexer.model
{
    public class IndexedImage
    {
        public Uri Image { get; set; }
        //public List<NamedPoint> PointsToMark { get; set; }
        public ObservableCollection<NamedPoint> PointsToMark { get; set; }
        public List<MarkedPoint> MarkedPoints { get; set; }
    }
}
