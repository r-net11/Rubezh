using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FiresecAPI
{
    public static class PlanElementsHelper
    {
        public static Brush CreateBrush(byte[] backgroundPixels)
        {
            BitmapImage bitmapImage = null;
            using (var imageStream = new MemoryStream(backgroundPixels))
            {
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = imageStream;
                bitmapImage.EndInit();
            }
            return new ImageBrush(bitmapImage);
        }
    }
}
