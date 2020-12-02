using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfMemoPrototype.ViewModel
{
    class MainViewModel : BindableBase
    {
        public ReactiveCollection<SelfMemo> MemoList { get; set; } = new ReactiveCollection<SelfMemo>();

        public ReactivePropertySlim<string> Word { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> ShortWord { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Description { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Category { get; set; } = new ReactivePropertySlim<string>("");

        public MainViewModel()
        {
            MemoList.Add(new SelfMemo("a", "b", "c", "d"));
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
    }
}
