using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;

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

        public ReactivePropertySlim<string> Word { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> ShortWord { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Description { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Category { get; set; } = new ReactivePropertySlim<string>("");

        #region AddMemoItemCommand
        private DelegateCommand _addMemoItemCmd;
        public DelegateCommand AddMemoItemCmd
        {
            get { return _addMemoItemCmd = _addMemoItemCmd ?? new DelegateCommand(AddMemoToList); }
        }

        /// <summary>
        /// プロパティに保持中の情報を新規SelfMemoとして追加する
        /// </summary>
        private void AddMemoToList()
        {
            SelfMemoItem newmemo = new SelfMemoItem(Word.Value, ShortWord.Value, Description.Value, Category.Value);

            if (!MemoList.Contains(newmemo))
            {
                MemoList.Add(newmemo);

                // プロパティを空白で初期化
                Word.Value = "";
                ShortWord.Value = "";
                Description.Value = "";
                Category.Value = "";

                //SaveMemoFile(); // MemoListのハンドラで実施されるのでここでは不要
            }
        }
        #endregion
    }
}
