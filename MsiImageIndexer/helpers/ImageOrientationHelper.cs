using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MsiImageIndexer.helpers
{
    public static class ImageOrientationHelper
    {
        public static BitmapImage OrientImage(Uri image) 
        {
            Rotation rotation = Rotation.Rotate0;
            BitmapFrame bitmapFrame = BitmapFrame.Create(image);
            BitmapMetadata bitmapMetadata = bitmapFrame.Metadata as BitmapMetadata;

            if ((bitmapMetadata != null) && (bitmapMetadata.ContainsQuery("System.Photo.Orientation"))) 
            {
                object o = bitmapMetadata.GetQuery("System.Photo.Orientation");

                if (o != null) 
                {
                    switch((ushort)o)
                    {
                        case 6:
                            rotation = Rotation.Rotate90;
                            break;
                        case 3:
                            rotation = Rotation.Rotate180;
                            break;
                        case 8:
                            rotation = Rotation.Rotate270;
                            break;
                    }
                }
            }

            BitmapImage _image = new BitmapImage();
            _image.BeginInit();
            _image.UriSource = image;
            _image.Rotation = rotation;
            _image.EndInit();
            _image.Freeze();

            return _image;
        }
    }
}
