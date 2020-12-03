using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Data;

namespace SelfMemoPrototype.ViewModel
{
    class MainViewModel : BindableBase
    {
        public ReactiveCollection<SelfMemoItem> MemoList { get; set; } = new ReactiveCollection<SelfMemoItem>();

        public ReactivePropertySlim<string> Word { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> ShortWord { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Description { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Category { get; set; } = new ReactivePropertySlim<string>("");

        public ReactivePropertySlim<bool> LockGridEdit { get; set; } = new ReactivePropertySlim<bool>(true);

        public ReactivePropertySlim<string> FilterStr { get; set; } = new ReactivePropertySlim<string>("");

        public ICollectionView FilteredItems
        {
            get
            {
                return filteredItemsSource.View;
            }
        }

        private CollectionViewSource filteredItemsSource;

        #region MemoFileControl
        private static readonly string MemoFileName = "selfmemo.json";

        private void LoadMemoFile()
        {
            ReactiveCollection<SelfMemoItem> _memo;
            try
            {
                using (var ms = new FileStream(MemoFileName, FileMode.Open))
                {
                    var serializer = new DataContractJsonSerializer(typeof(ReactiveCollection<SelfMemoItem>));
                    _memo = (ReactiveCollection<SelfMemoItem>)serializer.ReadObject(ms);
                }

                foreach (var m in _memo)
                {
                    MemoList.Add(m);
                }
            }
            catch (Exception e)
            {
                //error
            }

        }

        private void SaveMemoFile()
        {
            StreamWriter writer = new StreamWriter(MemoFileName, false, new System.Text.UTF8Encoding(false));

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReactiveCollection<SelfMemoItem>));
            serializer.WriteObject(writer.BaseStream, MemoList);
            writer.Close();
        }
        #endregion

        public MainViewModel()
        {
            // 表示するリスト（filteredItemsSource）のソースとフィルタの設定
            filteredItemsSource = new CollectionViewSource { Source = MemoList };
            filteredItemsSource.Filter += (s, e) =>
            {
                var item = e.Item as SelfMemoItem;
                e.Accepted = CheckFilterStr(FilterStr.Value, item);
            };

            // Filter文字列が更新されたら、Filterされたアイテムリストを更新
            FilterStr.PropertyChanged += (s, e) =>
            {
                FilteredItems.Refresh();
            };

            // ファイルが有ればロードしてMemoListを更新
            if (File.Exists(MemoFileName))
            {
                LoadMemoFile();
            }
            else
            {
                MemoList.Add(new SelfMemoItem("用語", "正式名称、別名、訳語など", "用語の解説", "カテゴリ"));
                MemoList.Add(new SelfMemoItem("SelfMemo", "ど忘れ用メモアプリ", "キーワードと関連情報（訳語、正式名称、説明など）を記録して\n再参照しやすくするアプリです。", "本アプリの説明"));
                MemoList.Add(new SelfMemoItem("SelfMemo", "ど忘れ用メモアプリ", "上の検索窓で、キーワード類の検索ができます。", "本アプリの説明"));
                MemoList.Add(new SelfMemoItem("SelfMemo", "ど忘れ用メモアプリ", "このキーワード表は、「ロック」チェックボックスを外すと直接編集可能です。", "本アプリの説明"));
                MemoList.Add(new SelfMemoItem("SelfMemo", "ど忘れ用メモアプリ", "「Register」タブからキーワードの追加ができます。", "本アプリの説明"));
            }

            // MemoListのコレクションが更新されたらファイルに保存
            MemoList.CollectionChanged += (s, e) =>
            {
                SaveMemoFile();
            };
        }

        ~MainViewModel()
        {
            // Finalizer でファイル保存を実行
            SaveMemoFile();
        }

        /// <summary>
        /// フィルタ文字列を解釈（スペース区切りでAND検索）して
        /// 引数のSelfMemoItemがフィルタに引っかかるかどうかを返す
        /// </summary>
        /// <param name="filter">スペース区切りのフィルタ文字列</param>
        /// <param name="memo">フィルタをかける対象のSelfMemoItem</param>
        /// <returns></returns>
        private bool CheckFilterStr(string filter, SelfMemoItem memo)
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
                else if (memo.Keyword2.Contains(f)) found++ ;
                else if (memo.Description.Contains(f)) found++;
                else if (memo.Category.Contains(f)) found++;
            }

            return found == filters.Length;
        }

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
