using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MsiImageIndexer.model
{
    public class ViewModel : INotifyPropertyChanged
    {
        public bool IsConfigLoaded { get { return pointCollection != null; } }
        public bool IsDataLoaded { get { return indexedImages.Count > 0;  } }

        private PointCollection pointCollection = null;
        public PointCollection PointCollection
        {
            get { return pointCollection; }
            set 
            {
                pointCollection = value;
                UpdateProperty("PointCollection");
                UpdateProperty("IsConfigLoaded");
            }
        }

        private string configNameLabel = "No configuration loaded.";
        public string ConfigNameLabel 
        {
            get { return configNameLabel; }
            set 
            {
                configNameLabel = value;
                UpdateProperty("ConfigNameLabel");
            }
        }


        // temporary
        private double x = 0;
        private double y = 0;
        public double X 
        {

            get { return x; }
            set 
            {
                x = value;
                UpdateProperty("CurrentPositionLabel");
            }
        }
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                UpdateProperty("CurrentPositionLabel");
            }
        }

        public string CurrentPositionLabel 
        {
            get { return $"X = [{x, 10}]\tY = [{y, 10}]"; }
        }

        private List<IndexedImage> indexedImages = new List<IndexedImage>();
        public List<IndexedImage> IndexedImages 
        {
            get { return indexedImages; }
            set 
            {
                indexedImages = value;
                UpdateProperty("IndexedImages");
                UpdateProperty("IsDataLoaded");
            }
        }

        private IndexedImage currentIndexedImage = null;
        public IndexedImage CurrentIndexedImage 
        {
            get 
            { 
                return currentIndexedImage;
            }
            set 
            {
                currentIndexedImage = value;
                UpdateProperty("CurrentIndexedImage");
                UpdateProperty("CurrentIndexedImageIndex");
                UpdateProperty("CurrentNamedPoint");
                UpdateProperty("CurrentNamedPointIndex");
                UpdateProperty("PointsToMark");
            }
        }
        public int CurrentIndexedImageIndex 
        {
            get { return indexedImages != null ? indexedImages.IndexOf(currentIndexedImage) : 0; }
            set 
            {
                currentIndexedImage = indexedImages[value];
                UpdateProperty("CurrentIndexedImage");
                UpdateProperty("CurrentIndexedImageIndex");
                UpdateProperty("CurrentNamedPoint");
                UpdateProperty("CurrentNamedPointIndex");
                UpdateProperty("PointsToMark");
            }
        }

        private NamedPoint currentNamedPoint = null;
        public NamedPoint CurrentNamedPoint 
        {
            get { return currentNamedPoint; }
            set 
            {
                currentNamedPoint = value;
                UpdateProperty("CurrentNamedPoint");
                UpdateProperty("CurrentNamedPointIndex");
            }
        }
        public int CurrentNamedPointIndex
        {  
            get { return currentIndexedImage != null ? currentIndexedImage.PointsToMark.IndexOf(currentNamedPoint) : 0; }
            set 
            {
                currentNamedPoint = currentIndexedImage.PointsToMark[value];
                UpdateProperty("CurrentNamedPoint");
                UpdateProperty("CurrentNamedPointIndex");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void UpdateProperty(string name) 
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
