using Reactive.Bindings;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace SelfMemoPrototype.Model
{
    [DataContract]
    public class SelfMemoItem
    {
        [DataMember]
        public string Keyword { get; set; }

        [DataMember]
        public string Keyword2 { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

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
            Keyword = keyword.Trim();
            Keyword2 = shortkwd.Trim();
            Description = description;
            Category = category.Trim().Replace("\r\n", ",").Replace("\n", ",");
            Date = DateTime.Now;
        }

        /// <summary>
        /// 登録文字列を独自規定に従い整形する
        /// </summary>
        public void Format()
        {
            Keyword = Keyword.Trim();
            Keyword2 = Keyword2.Trim();
            Category = Category.Trim().Replace("\r\n", ",").Replace("\n", ",");
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

        public static void UpdateCategoryList()
        {
            _categoryList.Clear();
            foreach (var item in ItemsList)
            {
                if (!_categoryList.Contains(item.Category))
                {
                    _categoryList.Add(item.Category);
                }
            }
        }

    }
}
