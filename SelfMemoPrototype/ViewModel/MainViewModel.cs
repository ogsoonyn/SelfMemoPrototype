using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;
using System.ComponentModel;
using System.Windows.Data;

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

        public ReactivePropertySlim<string> FilterStr { get; set; } = new ReactivePropertySlim<string>("");

        public ICollectionView AllItems
        {
            get
            {
                return allItemsSource.View;
            }
        }

        public ICollectionView FilteredItems
        {
            get
            {
                return filteredItemsSource.View;
            }
        }

        private CollectionViewSource allItemsSource;
        private CollectionViewSource filteredItemsSource;


        public MainViewModel()
        {
            allItemsSource = new CollectionViewSource { Source = MemoList };
            filteredItemsSource = new CollectionViewSource { Source = MemoList };

            filteredItemsSource.Filter += (s, e) =>
            {
                var item = e.Item as SelfMemo;
                e.Accepted = CheckFilterStr(FilterStr.Value, item);
            };

            // Filter文字列が更新されたら、Filterされたアイテムリストを更新
            FilterStr.PropertyChanged += (s, e) =>
            {
                FilteredItems.Refresh();
            };

            MemoList.Add(new SelfMemo("a", "b", "c", "d"));
            MemoList.Add(new SelfMemo("hoge", "fuga", "piyo", "nyan"));
            MemoList.Add(new SelfMemo("aaa", "bbb", "ccc", "ddd"));
            MemoList.Add(new SelfMemo("にほんご", "スペース 入った 文", "", "←空文字列"));
        }

        private bool CheckFilterStr(string filter, SelfMemo memo)
        {
            // フィルターが空文字列ならチェック通す
            if (filter.Length == 0) return true;

            string[] filters = filter.Split(new char[]{' ','　'});
            int found = 0;

            // いずれかのプロパティに該当文字列が含まれていたらカウントアップ
            foreach(string f in filters)
            {
                if(f.Length == 0)
                {
                    found++;
                    continue;
                }

                if (memo.Keyword.Contains(f)) found++;
                else if (memo.ShortKeyword.Contains(f)) found++ ;
                else if (memo.Description.Contains(f)) found++;
                else if (memo.Category.Contains(f)) found++;
            }

            return found == filters.Length;
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
    }
}
