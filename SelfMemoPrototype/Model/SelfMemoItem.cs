using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SelfMemoPrototype.Model
{
    [DataContract]
    public class SelfMemoItem
    {
        public ReactivePropertySlim<string> KeywordR { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Keyword2R { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> DescriptionR { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> CategoryR { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<DateTime> DateR { get; set; } = new ReactivePropertySlim<DateTime>();

        [DataMember]
        private string Keyword
        {
            get { return KeywordR.Value; }
            set
            {
                if (KeywordR == null) KeywordR = new ReactivePropertySlim<string>(value);
                else KeywordR.Value = value;
            }
        }

        [DataMember]
        private string Keyword2 { get { return Keyword2R.Value; }
            set
            {
                if (Keyword2R == null) Keyword2R = new ReactivePropertySlim<string>(value);
                else Keyword2R.Value = value;
            }
        }

        [DataMember]
        private string Description { get { return DescriptionR.Value; }
            set
            {
                if (DescriptionR == null) DescriptionR = new ReactivePropertySlim<string>(value);
                else DescriptionR.Value = value;
            }
        }

        [DataMember]
        private string Category { get { return CategoryR.Value; }
            set
            {
                if (CategoryR == null) CategoryR = new ReactivePropertySlim<string>(value);
                else CategoryR.Value = value;
            }
        }

        [DataMember]
        private DateTime Date { get { return DateR.Value; }
            set
            {
                if (DateR == null) DateR = new ReactivePropertySlim<DateTime>(value);
                else DateR.Value = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is SelfMemoItem)
            {
                bool chk1 = (obj as SelfMemoItem).Keyword.Equals(Keyword);
                bool chk2 = (obj as SelfMemoItem).Description.Equals(Description);
                bool chk3 = (obj as SelfMemoItem).Keyword2.Equals(Keyword2);
                bool chk4 = (obj as SelfMemoItem).Category.Equals(Category);

                return chk1 && chk2 && chk3 && chk4;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (Keyword + Description + Keyword2 + Category).GetHashCode();
        }

        public SelfMemoItem(string keyword, string shortkwd, string description, string category)
        {
            Initialize(keyword, shortkwd, description, category);
        }

        public void Initialize(string keyword, string shortkwd, string description, string category)
        {
            KeywordR.Value = keyword;
            Keyword2R.Value = shortkwd;
            DescriptionR.Value = description;
            CategoryR.Value = category;
            DateR.Value = DateTime.Now;

            Initialize();
        }

        public void Initialize(string keyword, string shortkwd, string description, string category, DateTime date)
        {
            Initialize(keyword, shortkwd, description, category);
            DateR.Value = date;
        }

        public void Initialize()
        {
            Format();
            SetHandler();
        }

        /// <summary>
        /// 登録文字列を独自規定に従い整形する
        /// </summary>
        private void Format()
        {
            KeywordR.Value = KeywordR.Value.Trim();
            Keyword2R.Value = Keyword2R.Value.Trim();
            CategoryR.Value = CategoryR.Value.Trim().Replace("\r\n", ",").Replace("\n", ",");
        }

        private void SetHandler()
        {
            KeywordR.PropertyChanged += (_, __) => { DateR.Value = DateTime.Now; };
            Keyword2R.PropertyChanged += (_, __) => { DateR.Value = DateTime.Now; };
            DescriptionR.PropertyChanged += (_, __) => { DateR.Value = DateTime.Now; };
            CategoryR.PropertyChanged += (_, __) => { DateR.Value = DateTime.Now; };
        }
    }

    public class SelfMemoList
    {
        public static ReactiveCollection<SelfMemoItem> ItemsList
        {
            get
            {
                if (_itemsList == null)
                {
                    _itemsList = new ReactiveCollection<SelfMemoItem>();
                    ItemsList.CollectionChanged += (s, e) =>
                    {
                        if (_categoryList != null) UpdateCategoryList();
                    };
                }
                return _itemsList;
            }
        }

        private static ReactiveCollection<SelfMemoItem> _itemsList = null;

        public static ReactiveCollection<string> CategoryList
        {
            get
            {
                if (_categoryList == null)
                {
                    _categoryList = new ReactiveCollection<string>();
                    UpdateCategoryList();
                }
                return _categoryList;
            }
        }
        private static ReactiveCollection<string> _categoryList = null;

        /// <summary>
        /// カテゴリ文字列を全件検索して、更新があればカテゴリリストを更新する
        /// </summary>
        /// <returns>リストを更新したらtrueを返す</returns>
        public static bool UpdateCategoryList()
        {
            var list = new ReactiveCollection<string>();
            bool updated = false;

            // ItemsListを全件検索
            foreach (var item in ItemsList)
            {
                if (!list.Contains(item.CategoryR.Value))
                {
                    list.Add(item.CategoryR.Value);
                }
            }

            // 現行のリストと比較し、無いものは追加
            foreach(var item in list)
            {
                if (!_categoryList.Contains(item))
                {
                    _categoryList.Add(item);
                    updated = true;
                }
            }

            // 現行リストにあるけど消えてるものは削除
            var remList = new List<string>();
            foreach (var item in _categoryList)
            {
                if (!list.Contains(item))
                {
                    remList.Add(item);
                    updated = true;
                }
            }
            foreach(var item in remList)
            {
                _categoryList.Remove(item);
            }
            return updated;
        }

        /// <summary>
        /// ファイルからメモリストの情報を読み出して引数のリストに追加する
        /// </summary>
        /// <param name="memoList">追加する対象のリスト</param>
        /// <param name="filename">読み出すファイル</param>
        public static void LoadMemoFile(ReactiveCollection<SelfMemoItem> memoList, string filename)
        {
            ReactiveCollection<SelfMemoItem> _memo;
            try
            {
                using (var ms = new FileStream(filename, FileMode.Open))
                {
                    var serializer = new DataContractJsonSerializer(typeof(ReactiveCollection<SelfMemoItem>));
                    _memo = (ReactiveCollection<SelfMemoItem>)serializer.ReadObject(ms);
                }

                foreach (var m in _memo)
                {
                    m.Initialize();
                    memoList.Add(m);
                }
            }
            catch (Exception e)
            {
                //error
            }

        }

        /// <summary>
        /// 指定のメモリストの内容をファイルに出力する
        /// </summary>
        /// <param name="memoList">出力する対象のリスト</param>
        /// <param name="filename">出力ファイル名</param>
        public static void SaveMemoFile(ReactiveCollection<SelfMemoItem> memoList, string filename)
        {
            StreamWriter writer = new StreamWriter(filename, false, new System.Text.UTF8Encoding(false));

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReactiveCollection<SelfMemoItem>));
            serializer.WriteObject(writer.BaseStream, memoList);
            writer.Close();
        }
    }
}
