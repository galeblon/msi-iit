using MsiImageIndexer.helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MsiImageIndexer.model
{
    [Serializable]
    public class ViewModel : INotifyPropertyChanged
    {
        public bool IsConfigLoaded { get { return pointCollection != null; } }
        public bool IsDataLoaded { get { return indexedImages.Count > 0; } }

        [NonSerialized]
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


        private double x = 0;
        private double y = 0;
        public double X
        {

            get { return x; }
            set
            {
                x = Math.Round(value, 1);
                UpdateProperty("CurrentPositionLabel");
            }
        }
        public double Y
        {
            get { return y; }
            set
            {
                y = Math.Round(value, 1);
                UpdateProperty("CurrentPositionLabel");
            }
        }

        private double x_scale = 0;
        private double y_scale = 0;

        public double XScale
        {
            get { return x_scale; }
        }

        public double YScale
        {
            get { return y_scale; }
        }


        public string CurrentPositionLabel
        {
            get { return $"X = [{x,10:0.##}]\tY = [{y,10:0.##}]"; }
        }

        [NonSerialized]
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

        [NonSerialized]
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
                var imageInfo = Image.FromStream(File.OpenRead(currentIndexedImage.Image.LocalPath), false, false);
                x_scale = imageInfo.PhysicalDimension.Width;
                y_scale = imageInfo.PhysicalDimension.Height;
                x = 0;
                y = 0;
                UpdateProperty("CurrentPositionLabel");
                precisionImageBrush = new ImageBrush()
                {
                    ImageSource = ImageOrientationHelper.OrientImage(value.Image)
                };
                UpdateProperty("CurrentIndexedImage");
                UpdateProperty("CurrentIndexedImageIndex");
                UpdateProperty("CurrentNamedPoint");
                UpdateProperty("CurrentNamedPointIndex");
                UpdateProperty("PointsToMark");
                UpdateProperty("PrecisionImageBrush");
            }
        }
        public int CurrentIndexedImageIndex
        {
            get { return indexedImages != null ? indexedImages.IndexOf(currentIndexedImage) : 0; }
            set
            {
                currentIndexedImage = indexedImages[value];
                var imageInfo = Image.FromStream(File.OpenRead(currentIndexedImage.Image.LocalPath), false, false);
                x_scale = imageInfo.PhysicalDimension.Width;
                y_scale = imageInfo.PhysicalDimension.Height;
                x = 0;
                y = 0;
                UpdateProperty("CurrentPositionLabel");
                precisionImageBrush = new ImageBrush()
                {
                    ImageSource = ImageOrientationHelper.OrientImage(currentIndexedImage.Image)
                };
                UpdateProperty("CurrentIndexedImage");
                UpdateProperty("CurrentIndexedImageIndex");
                UpdateProperty("CurrentNamedPoint");
                UpdateProperty("CurrentNamedPointIndex");
                UpdateProperty("PointsToMark");
                UpdateProperty("PrecisionImageBrush");
            }
        }

        [NonSerialized]
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

        [NonSerialized]
        private readonly int minimumZoomLevel = 50;
        [NonSerialized]
        private readonly int maximumZoomLevel = 1000;
        [NonSerialized]
        private int zoomLevel = 100;
        public int ZoomLevel
        {
            get { return zoomLevel; }
            set
            {
                zoomLevel = Math.Min(Math.Max(value, minimumZoomLevel), maximumZoomLevel);
                UpdateProperty("ZoomLevel");
            }
        }

        [NonSerialized]
        private ImageBrush precisionImageBrush = null;
        public ImageBrush PrecisionImageBrush
        {
            get { return precisionImageBrush; }
            set
            {
                precisionImageBrush = value;
                UpdateProperty("PrecisionImageBrush");
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
