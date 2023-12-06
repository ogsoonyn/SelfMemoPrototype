using Reactive.Bindings;
using System;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SelfMemoPrototype.Model
{
    [DataContract]
#pragma warning disable CS0659 // 型は Object.Equals(object o) をオーバーライドしますが、Object.GetHashCode() をオーバーライドしません
    public class SelfMemoItem
#pragma warning restore CS0659 // 型は Object.Equals(object o) をオーバーライドしますが、Object.GetHashCode() をオーバーライドしません
    {
        public ReactivePropertySlim<string> KeywordR { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Keyword2R { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> DescriptionR { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> CategoryR { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<DateTime> DateR { get; set; } = new ReactivePropertySlim<DateTime>();

        public ReactivePropertySlim<BitmapSource> ImageSourceR { get; set; } = new ReactivePropertySlim<BitmapSource>();
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

        /*
        public override int GetHashCode()
        {
            return (Keyword + Description + Keyword2 + Category).GetHashCode();
        }
        */

        public SelfMemoItem(string keyword, string shortkwd, string description, string category)
        {
            Initialize(keyword, shortkwd, description, category);
        }

        public void Initialize(string keyword, string shortkwd, string description, string category, BitmapSource source = null)
        {
            KeywordR.Value = keyword;
            Keyword2R.Value = shortkwd;
            DescriptionR.Value = description;
            CategoryR.Value = category;
            DateR.Value = DateTime.Now;
            ImageSourceR.Value = source;

            Initialize();
        }

        public void Initialize(string keyword, string shortkwd, string description, string category, DateTime date, BitmapSource source = null)
        {
            Initialize(keyword, shortkwd, description, category);
            DateR.Value = date;
            ImageSourceR.Value = source;
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
}
