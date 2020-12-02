using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;

namespace SelfMemoPrototype.ViewModel
{
    class MainViewModel : BindableBase
    {
        public ReactiveCollection<SelfMemo> MemoList { get; set; } = new ReactiveCollection<SelfMemo>();

        public ReactivePropertySlim<string> Word { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> ShortWord { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Description { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Category { get; set; } = new ReactivePropertySlim<string>("");

        public ReactivePropertySlim<bool> GridReadOnly { get; set; } = new ReactivePropertySlim<bool>(true);

        public MainViewModel()
        {
            MemoList.Add(new SelfMemo("a", "b", "c", "d"));
            MemoList.Add(new SelfMemo("hoge", "fuga", "piyo", "nyan"));
            MemoList.Add(new SelfMemo("aaa", "bbb", "ccc", "ddd"));
        }

        private DelegateCommand _cmd;
        public DelegateCommand Cmd
        {
            get { return _cmd = _cmd ?? new DelegateCommand(Add); }
        }

        private void Add()
        {
            SelfMemo newmemo = new SelfMemo(Word.Value, ShortWord.Value, Description.Value, Category.Value);

            if (!MemoList.Contains(newmemo))
            {
                MemoList.Add(newmemo);
                Word.Value = "";
                ShortWord.Value = "";
                Description.Value = "";
                Category.Value = "";
            }
        }

        private DelegateCommand _cmd2;
        public DelegateCommand Cmd2
        {
            get { return _cmd2 = _cmd2 ?? new DelegateCommand(Select); }
        }

        private void Select()
        {

        }

        private DelegateCommand _cmd3;
        public DelegateCommand Cmd3
        {
            get { return _cmd3 = _cmd3 ?? new DelegateCommand(Reset); }
        }

        private void Reset()
        {

        }
    }
}
