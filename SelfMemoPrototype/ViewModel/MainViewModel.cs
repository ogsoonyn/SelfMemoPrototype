using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;
using SelfMemoPrototype.View;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Windows.Data;

namespace SelfMemoPrototype.ViewModel
{
    class MainViewModel : BindableBase
    {
        public ReactiveCollection<SelfMemoItem> MemoList
        {
            get { return SelfMemoList.ItemsList; }
        }
        public ReactiveCollection<string> CategoryList
        {
            get { return SelfMemoList.CategoryList; }
        }
        public ReactivePropertySlim<bool> UseCategoryList { get; set; } = new ReactivePropertySlim<bool>(false);

        public ReactivePropertySlim<string> CategoryListSelected { get; set; } = new ReactivePropertySlim<string>("");

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

        public ReactivePropertySlim<string> AppName { get; } = new ReactivePropertySlim<string>();

        public ReactivePropertySlim<bool> CopySearchWordToRegister { get; set; } = new ReactivePropertySlim<bool>(true);

        #region MemoFileControl
        private static readonly string MemoFileName = "selfmemo.json";


        #endregion

        public MainViewModel()
        {
            // タイトルに表示する文字列を指定
            var asm = Assembly.GetExecutingAssembly().GetName();
            AppName.Value = asm.Name + " - " + asm.Version.Major + "." + asm.Version.Minor;

            // 表示するリスト（filteredItemsSource）のソースとフィルタの設定
            filteredItemsSource = new CollectionViewSource { Source = MemoList };
            filteredItemsSource.Filter += (s, e) =>
            {
                var item = e.Item as SelfMemoItem;
                e.Accepted = CheckFilterStr(FilterStr.Value, item) && CheckCategoryFilter(item);
            };

            // Filter文字列が更新されたら、Filterされたアイテムリストを更新
            FilterStr.PropertyChanged += (s, e) =>
            {
                FilteredItems.Refresh();
            };

            // カテゴリ選択ComboBoxが更新されたら、Filterされたアイテムリスト更新
            CategoryListSelected.PropertyChanged += (s, e) =>
            {
                FilteredItems.Refresh();
            };

            // カテゴリ選択ComboBoxのEnable設定が更新されたらアイテムリスト更新
            UseCategoryList.PropertyChanged += (s, e) =>
            {
                SelfMemoList.UpdateCategoryList();
                FilteredItems.Refresh();
            };

            // ファイルが有ればロードしてMemoListを更新
            if (File.Exists(MemoFileName))
            {
                SelfMemoList.LoadMemoFile(MemoList, MemoFileName);
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
                string fl = f.ToLower();

                if (memo.Keyword.ToLower().Contains(fl)) found++;
                else if (memo.Keyword2.ToLower().Contains(fl)) found++ ;
                else if (memo.Description.ToLower().Contains(fl)) found++;
                else if (memo.Category.ToLower().Contains(fl)) found++;
            }

            return found == filters.Length;
        }

        private bool CheckCategoryFilter(SelfMemoItem memo)
        {
            // フィルタが無効なら全て通す
            if (UseCategoryList.Value == false) return true;

            // フィルタの選択肢がNULLならすべて通す
            if (CategoryListSelected.Value?.Length == 0) return true;

            // 指定されたCategoryの項目のみTrueを返す
            return (memo.Category.Equals(CategoryListSelected.Value));

        }

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
    }
}
