using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace SelfMemoPrototype.Model
{


    public class ClipboardCapture
    {
        //Streamに一時保存方式、BmpBitmapのエンコーダーとデコーダーを使う
        public static BitmapSource GetBitmapEncDec()
        {
            BitmapSource source = Clipboard.GetImage();
            if (source == null)
            {
                return null;
            }

            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (var stream = new System.IO.MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                var decoder = new BmpBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                source = decoder.Frames[0];
            }
            return source;
        }

        /// <summary>
        /// 透明画像にならないようにクリップボードの画像取得、DeviceIndependentBitmapでToArrayの15番目がbpp、32未満ならBgr32へ変換
        /// </summary>
        /// <returns></returns>
        public static BitmapSource GetBitmap(bool checkExcel=true)
        {
            var data = Clipboard.GetDataObject();
            if (data == null) return null;

            var ms = data.GetData("DeviceIndependentBitmap") as System.IO.MemoryStream;
            if (ms == null) return null;

            //DeviceIndependentBitmapのbyte配列の15番目がbpp、
            //これが32未満ならBgr32へ変換、これでアルファの値が0でも255扱いになって表示される
            //エクセルからのコピーなのかも判定、そうならBgr32へ変換
            byte[] dib = ms.ToArray();
            if (dib[14] < 32 || (checkExcel && IsExcel()))
            {
                return new FormatConvertedBitmap(Clipboard.GetImage(), PixelFormats.Bgr32, null, 0);
            }
            else
            {
                return Clipboard.GetImage();
            }
        }

        //エクセルからのコピーなのかを判定、フォーマット形式にEnhancedMetafileがあればエクセル判定
        private static bool IsExcel()
        {
            string[] formats = Clipboard.GetDataObject().GetFormats();
            foreach (var item in formats)
            {
                if (item == "EnhancedMetafile")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
