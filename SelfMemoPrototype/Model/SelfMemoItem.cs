using Reactive.Bindings;
using System;
using System.Reactive.Linq;
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
        public ReactivePropertySlim<int> IDR { get; set; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<string> KeywordR { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Keyword2R { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> DescriptionR { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> CategoryR { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<DateTime> DateR { get; set; } = new ReactivePropertySlim<DateTime>();

        public ReactivePropertySlim<BitmapSource> ImageSourceR { get; set; }

        public ReadOnlyReactivePropertySlim<bool> HasImageSource { get; private set; }

        [DataMember]
        private int ID
        {
            get
            {
                if (IDR == null) IDR = new ReactivePropertySlim<int>(-1);
                return IDR.Value;
            }
            set {
                if (IDR == null) IDR = new ReactivePropertySlim<int>(value);
                else ID = value;
            }
            
        }

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
                var item = obj as SelfMemoItem;
                bool chk1 = item.Keyword == Keyword;
                bool chk2 = item.Description == Description;
                bool chk3 = item.Keyword2 == Keyword2;
                bool chk4 = item.Category == Category;

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

        public SelfMemoItem(string keyword, string shortkwd, string description, string category, int id=-1)
        {
            Initialize(keyword, shortkwd, description, category, id);
        }

        public void Initialize(string keyword, string shortkwd, string description, string category, int id=-1)
        {
            IDR.Value = id;
            KeywordR.Value = keyword;
            Keyword2R.Value = shortkwd;
            DescriptionR.Value = description;
            CategoryR.Value = category;
            DateR.Value = DateTime.Now;

            Initialize();
        }

        public void Initialize()
        {
            if (ID < 0) IDR.Value = SelfMemoList.GetNextID();
            ImageSourceR = new ReactivePropertySlim<BitmapSource>();
            ImageSourceR.Value = ImageManager.GetBitmapSource(IDR.Value);

            Format();
            SetHandler();

            HasImageSource = ImageSourceR.Select(x => x != null).ToReadOnlyReactivePropertySlim<bool>();
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
