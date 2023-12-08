using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SelfMemoPrototype.ViewModel
{
    class ImageViewerViewModel : BindableBase
    {
        public ReactiveProperty<SelfMemoItem> Item { get; set; } = new ReactiveProperty<SelfMemoItem>();

        //public ReactivePropertySlim<BitmapSource> ImageSource { get; set; } = new ReactivePropertySlim<BitmapSource>();

    }
}
