using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;
using System.Windows.Media.Imaging;

namespace SelfMemoPrototype.ViewModel
{
    class RegisterViewModel : BindableBase
    {
        private ReactiveCollection<SelfMemoItem> MemoList
        {
            get
            {
                return SelfMemoList.ItemsList;
            }
        }

        public ReactiveCollection<string> CategoryList
        {
            get
            {
                return SelfMemoList.CategoryList;
            }
        }

        public ReactivePropertySlim<string> Word { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> ShortWord { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Description { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Category { get; set; } = new ReactivePropertySlim<string>("");

        public ReactivePropertySlim<bool> IsSelected { get; set; } = new ReactivePropertySlim<bool>();

        public ReactivePropertySlim<BitmapSource> ImageSource { get; set; } = new ReactivePropertySlim<BitmapSource>();

        #region AddMemoItemCommand
        private DelegateCommand _addMemoItemCmd;
        public DelegateCommand AddMemoItemCmd
        {
            get { return _addMemoItemCmd = _addMemoItemCmd ?? new DelegateCommand(AddMemoToList); }
        }

        private DelegateCommand _pasteImageCmd;

        public DelegateCommand PasteImageCmd
        {
            get => _pasteImageCmd = _pasteImageCmd ?? new DelegateCommand(() =>
            {
                ImageSource.Value = ClipboardCapture.GetBitmap();
            });
        }

        /// <summary>
        /// プロパティに保持中の情報を新規SelfMemoとして追加する
        /// </summary>
        private void AddMemoToList()
        {
            SelfMemoItem newmemo = new SelfMemoItem(Word.Value, ShortWord.Value, Description.Value, Category.Value);

            if (!MemoList.Contains(newmemo))
            {
                // Imageがあれば保存
                if(ImageSource.Value != null)
                {
                    ImageManager.SaveImageFile(ImageSource.Value, newmemo.IDR.Value);
                    newmemo.ImageSourceR.Value = ImageSource.Value;
                }

                MemoList.Add(newmemo);

                // プロパティを空白で初期化
                Word.Value = "";
                ShortWord.Value = "";
                Description.Value = "";
                Category.Value = "";
                ImageSource.Value = null;

                //SaveMemoFile(); // MemoListのハンドラで実施されるのでここでは不要

                // WordのTextboxをフォーカスする
                IsSelected.Value = true;
            }
        }
        #endregion
    }
}
