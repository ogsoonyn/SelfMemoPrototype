using System.Windows.Media.Imaging;
using System.IO;
using System;

namespace SelfMemoPrototype.Model
{
    class ImageManager
    {
        public static readonly string ImageDirectory = ".\\images";

        public static BitmapSource GetBitmapSource(int id)
        {
            if (id < 0) return null;

            try
            {
                using (var stream = File.OpenRead(ID2JpgPath(id)))
                {
                    var source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    source.Freeze();
                    return source;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string[] ImageFileList
        {
            get
            {
                return Directory.GetFiles(ImageDirectory, "*.jpg");
            }
        }

        public static void SaveImageFile(BitmapSource source, int id)
        {
            if (source == null) return;

            if(!Directory.Exists(ImageDirectory)) Directory.CreateDirectory(ImageDirectory);

            using (var fileStream = new FileStream(ID2JpgPath(id), FileMode.Create))
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(fileStream);
            }
        }

        public static void RemoveImageFile(int id)
        {
            var path = ID2JpgPath(id);
            if (File.Exists(path)) File.Delete(path);
        }

        public static string ID2JpgPath(int id)
        {
            return ImageDirectory + "\\" + string.Format("{0:00000}", id) + ".jpg";
        }
    }
}
