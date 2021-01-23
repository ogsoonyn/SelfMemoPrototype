using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;
using SelfMemoPrototype.View;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace SelfMemoPrototype.ViewModel
{
    class MainViewModel : BindableBase
    {
        /// <summary>
        /// メモリスト本体
        /// </summary>
        public ReactiveCollection<SelfMemoItem> MemoList { get { return SelfMemoList.ItemsList; } }

        /// <summary>
        /// カテゴリリスト本体
        /// </summary>
        public ReactiveCollection<string> CategoryList { get { return SelfMemoList.CategoryList; } }

        /// <summary>
        /// カテゴリでフィルタする機能のON/OFFフラグ
        /// </summary>
        public ReadOnlyReactivePropertySlim<bool> UseCategoryList { get; }

        /// <summary>
        /// カテゴリでフィルタする機能で選択されているカテゴリ文字列
        /// </summary>
        public ReactivePropertySlim<string> CategoryListSelected { get; set; } = new ReactivePropertySlim<string>("");

        /// <summary>
        /// DataGridの直接編集をロックする機能のON/OFFフラグ
        /// </summary>
        public ReactivePropertySlim<bool> LockGridEdit { get; set; } = new ReactivePropertySlim<bool>(false);

        /// <summary>
        /// 検索（フィルタ）文字列
        /// </summary>
        public ReactivePropertySlim<string> FilterStr { get; set; } = new ReactivePropertySlim<string>("");

        /// <summary>
        /// フィルタ文字列があればTrueになるフラグ
        /// </summary>
        public ReadOnlyReactivePropertySlim<bool> UseFilterStr { get; }

        /// <summary>
        /// Viewに表示する用のフィルタ済みItemリスト
        /// </summary>
        public CollectionView FilteredItems { get { return FilteredItemsSource.View as CollectionView; } }

        /// <summary>
        /// MemoListにフィルタをつけたもの
        /// </summary>
        private CollectionViewSource FilteredItemsSource;

        /// <summary>
        /// タイトルバーに表示するアプリ名
        /// </summary>
        public ReactivePropertySlim<string> AppName { get; } = new ReactivePropertySlim<string>();

        /// <summary>
        /// 検索フォームの文字列を登録フォームにコピーする機能のON/OFFフラグ
        /// </summary>
        public ReactivePropertySlim<bool> CopySearchWordToRegister { get; set; } = new ReactivePropertySlim<bool>(true);

        /// <summary>
        /// DataGrid上の操作で項目を削除することを許容するかどうか
        /// </summary>
        public ReactivePropertySlim<bool> AllowDeleteItem { get; set; } = new ReactivePropertySlim<bool>(false);

        /// <summary>
        /// 辞書データファイルの名前
        /// </summary>
        private static readonly string MemoFileName = "selfmemo.json";

        /// <summary>
        /// FilteredItemsの更新に使用するタイマー
        /// </summary>
        DispatcherTimer FilteredItemsRefreshTimer = new DispatcherTimer();

        /// <summary>
        /// 選択中の項目プレビューに使用する
        /// </summary>
        public ReactivePropertySlim<SelfMemoItem> SelectedItem { get; set; } = new ReactivePropertySlim<SelfMemoItem>();

        public MainViewModel()
        {
            // タイトルに表示する文字列を指定
            var asm = Assembly.GetExecutingAssembly().GetName();
            AppName.Value = asm.Name + " - " + asm.Version.Major + "." + asm.Version.Minor;

            // 表示するリスト（filteredItemsSource）のソースとフィルタの設定
            FilteredItemsSource = new CollectionViewSource { Source = MemoList };
            FilteredItemsSource.Filter += (s, e) =>
            {
                var item = e.Item as SelfMemoItem;
                e.Accepted = CheckFilterStr(FilterStr.Value, item) && CheckCategoryFilter(item);
            };

            // ファイルが有ればロードしてMemoListを更新
            if (File.Exists(MemoFileName))
            {
                SelfMemoList.LoadMemoFile(MemoList, MemoFileName);
            }

            // MemoListが空なら、ヘルプメッセージ的な項目を追加する
            if (SelfMemoList.ItemsList.Count == 0)
            {
                MemoList.Add(new SelfMemoItem("用語", "正式名称、別名、訳語など", "用語の解説", "カテゴリ"));
                MemoList.Add(new SelfMemoItem("SelfMemo", "概要", "キーワードと関連情報(訳語、正式名称、説明など)を登録し、ど忘れした時に見返しやすくするアプリです。", "本アプリの説明"));
                MemoList.Add(new SelfMemoItem("SelfMemo", "検索機能", "検索フォームからキーワード検索ができます。", "本アプリの説明"));
                MemoList.Add(new SelfMemoItem("SelfMemo", "ショートカット", "グローバルホットキー(デフォルトでAlt+Shift+F2)でいつでもアプリ起動します。", "本アプリの説明"));
                MemoList.Add(new SelfMemoItem("SelfMemo", "項目追加", "メニューの「登録ダイアログを開く(Ctrl+R)」からキーワードの追加ができます。", "本アプリの説明"));
                MemoList.Add(new SelfMemoItem("SelfMemo", "項目編集", "編集ビューから登録内容の編集ができます。", "本アプリの説明"));
                MemoList.Add(new SelfMemoItem("SelfMemo", "カテゴリフィルタ", "「カテゴリ」メニューでカテゴリ毎の表示フィルタリングができます。", "本アプリの説明"));
            }

            // Filter文字列が更新されたら、Filterされたアイテムリストを更新
            FilterStr.Subscribe(_ =>
            {
                // 既にタイマーが走ってたら、一旦止める
                if (FilteredItemsRefreshTimer.IsEnabled)
                {
                    FilteredItemsRefreshTimer.Stop();
                }

                // タイマー開始
                FilteredItemsRefreshTimer.Interval = TimeSpan.FromMilliseconds(300);
                FilteredItemsRefreshTimer.Tick += (s, e) =>
                {
                    FilteredItems.Refresh();
                    FilteredItemsRefreshTimer.Stop();
                };
                FilteredItemsRefreshTimer.Start();
            });

            // Filter文字列の有無フラグを連動
            UseFilterStr = FilterStr.Select(x => !string.IsNullOrWhiteSpace(x)).ToReadOnlyReactivePropertySlim();

            // カテゴリ選択ComboBoxが更新されたら、Filterされたアイテムリスト更新
            CategoryListSelected.Subscribe(_ =>
            {
                if (!FilteredItemsRefreshTimer.IsEnabled)
                {
                    FilteredItemsRefreshTimer.Interval = TimeSpan.FromMilliseconds(300);
                    FilteredItemsRefreshTimer.Tick += (s, e) =>
                    {
                        FilteredItems.Refresh();
                        FilteredItemsRefreshTimer.Stop();
                    };
                    FilteredItemsRefreshTimer.Start();
                }
            });

            // 選択項目をSelectedItemに入れる処理
            FilteredItems.CurrentChanged += (s, e) =>
            {
                SelectedItem.Value = FilteredItems.CurrentItem as SelfMemoItem;
            };

            // UseCategoryListはカテゴリリストからなんか選択されてたらTrue
            UseCategoryList = CategoryListSelected.Select(x => !string.IsNullOrEmpty(x)).ToReadOnlyReactivePropertySlim();

            // カテゴリリストのON/OFFを切り替えるタイミングでもカテゴリリストの内容更新
            UseCategoryList.Subscribe(_ =>
            {
                SelfMemoList.UpdateCategoryList();
            });

            // MemoListのコレクションが更新されたらファイルに保存
            MemoList.CollectionChanged += (s, e) =>
            {
                SelfMemoList.SaveMemoFile(MemoList, MemoFileName);
            };
        }

        ~MainViewModel()
        {
            // Finalizer でファイル保存を実行
            SelfMemoList.SaveMemoFile(MemoList, MemoFileName);
        }

        /// <summary>
        /// フィルタ文字列を解釈（スペース区切りでAND検索）して
        /// 引数のSelfMemoItemがフィルタに引っかかるかどうかを返す
        /// </summary>
        /// <param name="filter">スペース区切りのフィルタ文字列</param>
        /// <param name="memo">フィルタをかける対象のSelfMemoItem</param>
        /// <returns>フィルタに引っかかればTrue</returns>
        private bool CheckFilterStr(string filter, SelfMemoItem memo)
        {
            // フィルターが空文字列ならチェック通す
            if (string.IsNullOrEmpty(filter)) return true;

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
                string fl = f.ToLower();

                if (memo.KeywordR.Value.ToLower().Contains(fl)) found++;
                else if (memo.Keyword2R.Value.ToLower().Contains(fl)) found++ ;
                else if (memo.DescriptionR.Value.ToLower().Contains(fl)) found++;
                else if (memo.CategoryR.Value.ToLower().Contains(fl)) found++;
            }

            return found == filters.Length;
        }

        /// <summary>
        /// カテゴリフィルタの文字列を解釈して
        /// 引数のSelfMemoItemがフィルタに引っかかるかどうかを返す
        /// </summary>
        /// <param name="memo">フィルタをかける対象</param>
        /// <returns>フィルタに引っかかればTrue</returns>
        private bool CheckCategoryFilter(SelfMemoItem memo)
        {
            // フィルタが無効なら全て通す
            //if (UseCategoryList.Value == false) return true;

            // フィルタの選択肢がNULLならすべて通す
            if (string.IsNullOrEmpty(CategoryListSelected.Value)) return true;

            // 指定されたCategoryの項目のみTrueを返す
            return (memo.CategoryR.Value.Equals(CategoryListSelected.Value));
        }

        /// <summary>
        /// 登録フォームを表示するコマンド
        /// </summary>
        #region OpenRegisterWindow
        private DelegateCommand _openRegisterWindowCmd;
        public DelegateCommand OpenRegisterWindowCmd
        {
            get { return _openRegisterWindowCmd = _openRegisterWindowCmd ?? new DelegateCommand(OpenRegisterWindow); }
        }

        private void OpenRegisterWindow()
        {
            var win = new RegisterWindow();
            if (CopySearchWordToRegister.Value)
            {
                // Search枠に入力された文字列を登録フォームのKeyword枠にコピーする
                (win.DataContext as RegisterViewModel).Word.Value = FilterStr.Value;
            }
            win.ShowDialog();
        }
        #endregion

        /// <summary>
        /// 設定ダイアログを表示するコマンド
        /// </summary>
        #region OpenSettingWindow
        private DelegateCommand _openSettingWindowCmd;
        public DelegateCommand OpenSettingWindowCmd
        {
            get { return _openSettingWindowCmd = _openSettingWindowCmd ?? new DelegateCommand(OpenSettingWindow); }
        }

        private void OpenSettingWindow()
        {
            var win = new SettingWindow();
            win.ShowDialog();
        }
        #endregion

        private DelegateCommand _saveFilteredItemsCmd;
        public DelegateCommand SaveFilteredItemsCmd
        {
            get { return _saveFilteredItemsCmd = _saveFilteredItemsCmd ?? new DelegateCommand(SaveFilteredItems); }
        }

        private void SaveFilteredItems()
        {
            var list = new ReactiveCollection<SelfMemoItem>();
            foreach(var item in FilteredItems)
            {
                list.Add(item as SelfMemoItem);
            }

            SelfMemoList.SaveMemoFile(list, "filtered.json");
        }
    }
}
