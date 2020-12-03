using Reactive.Bindings;
using System;
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
            Keyword = keyword;
            Keyword2 = shortkwd;
            Description = description;
            Category = category;
            Date = DateTime.Now;
        }
    }

    public class SelfMemoList
    {
        public static ReactiveCollection<SelfMemoItem> ItemsList
        {
            get
            {
                if (_itemsList == null) _itemsList = new ReactiveCollection<SelfMemoItem>();
                return _itemsList;
            }
        }

        private static ReactiveCollection<SelfMemoItem> _itemsList = null;
    }
}
