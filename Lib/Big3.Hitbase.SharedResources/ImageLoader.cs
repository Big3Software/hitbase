using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;

namespace Big3.Hitbase.SharedResources
{
    public class ImageLoader
    {
        public static ImageSource FromResource(string filename)
        {
            string resourceUri;
            
            if (filename.StartsWith("pack://"))
                resourceUri = filename;
            else
                resourceUri = string.Format("pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/{0}", filename);

            if (Path.GetExtension(filename).ToLower() == ".jpg")
            {
                var ibd = new JpegBitmapDecoder(
                    new Uri(resourceUri),
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);
                
                return ibd.Frames[0];
            }
            else
            {
                var ibd = new PngBitmapDecoder(
                    new Uri(resourceUri),
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                return ibd.Frames[0];
            }
        }

        public static ImageSource FromResource(string assembly, string filename)
        {
            string resourceUri = string.Format("pack://application:,,,/{0};component/Images/{1}", assembly, filename);

            if (Path.GetExtension(filename).ToLower() == ".jpg")
            {
                var ibd = new JpegBitmapDecoder(
                    new Uri(resourceUri),
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                return ibd.Frames[0];
            }
            else
            {
                var ibd = new PngBitmapDecoder(
                    new Uri(resourceUri),
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                return ibd.Frames[0];
            }
        }

        public static Image CreateImageFromResource(string filename)
        {
            Image newImage = new Image();

            newImage.Source = FromResource(filename);

            return newImage;
        }

        public static BitmapImage GetBitmapImageFromMemoryStream(MemoryStream m)
        {
            BitmapImage bi = new BitmapImage();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.BeginInit();
            bi.StreamSource = m;
            bi.EndInit();
            bi.Freeze();

            return bi;
        }

        public static string GetImageFilenameOrDefault(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                return "pack://application:,,,/Big3.Hitbase.SharedResources;component/Images/CDCover.png";
            }
            else
            {
                return filename;
            }
        }

        /// <summary>
        /// Liefert ein Thumbnail des Images zurück.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="freeze"></param>
        /// <returns></returns>
        public static BitmapImage GetThumbnailImage(string filename, int width = 100, int height = 100, bool freeze = true)
        {
            try
            {
                BitmapImage bi = new BitmapImage();

                bi.BeginInit();
                bi.DecodePixelWidth = width;
                bi.DecodePixelHeight = height;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = new Uri(filename);
                bi.EndInit();
                
                if (freeze)
                    bi.Freeze();

                return bi;
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// Wandelt ein WinForms (GDI) Image in ein WPF Image um.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static ImageSource ConvertDrawingImageToWPFImage(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();

            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();

            bImg.BeginInit();

            bImg.StreamSource = new MemoryStream(ms.ToArray());

            bImg.EndInit();

            bImg.Freeze();

            return bImg;
        }
    }
}
