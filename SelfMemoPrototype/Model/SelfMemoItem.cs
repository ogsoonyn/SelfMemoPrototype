using Reactive.Bindings;
using System;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SelfMemoPrototype.Model
{
    [DataContract]
    public class SelfMemoItem
    {
        public ReactivePropertySlim<int> ID_R { get; set; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<string> Keyword_R { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Keyword2_R { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Description_R { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Category_R { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<DateTime> Date_R { get; set; } = new ReactivePropertySlim<DateTime>();

        public ReactivePropertySlim<BitmapSource> ImageSource_R { get; set; }

        public ReadOnlyReactivePropertySlim<bool> HasImageSource { get; private set; }

        [DataMember]
        private int ID
        {
            get
            {
                if (ID_R == null) ID_R = new ReactivePropertySlim<int>(-1);
                return ID_R.Value;
            }
            set {
                if (ID_R == null) ID_R = new ReactivePropertySlim<int>(value);
                else ID = value;
            }
            
        }

        [DataMember]
        private string Keyword
        {
            get { return Keyword_R.Value; }
            set
            {
                if (Keyword_R == null) Keyword_R = new ReactivePropertySlim<string>(value);
                else Keyword_R.Value = value;
            }
        }

        [DataMember]
        private string Keyword2 { get { return Keyword2_R.Value; }
            set
            {
                if (Keyword2_R == null) Keyword2_R = new ReactivePropertySlim<string>(value);
                else Keyword2_R.Value = value;
            }
        }

        [DataMember]
        private string Description { get { return Description_R.Value; }
            set
            {
                if (Description_R == null) Description_R = new ReactivePropertySlim<string>(value);
                else Description_R.Value = value;
            }
        }

        [DataMember]
        private string Category { get { return Category_R.Value; }
            set
            {
                if (Category_R == null) Category_R = new ReactivePropertySlim<string>(value);
                else Category_R.Value = value;
            }
        }

        [DataMember]
        private DateTime Date { get { return Date_R.Value; }
            set
            {
                if (Date_R == null) Date_R = new ReactivePropertySlim<DateTime>(value);
                else Date_R.Value = value;
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return (Keyword_R, Keyword2_R, Description_R, Category_R).GetHashCode();
        }

        public SelfMemoItem(string keyword, string shortkwd, string description, string category, int id=-1)
        {
            Initialize(keyword, shortkwd, description, category, id);
        }

        public void Initialize(string keyword, string shortkwd, string description, string category, int id=-1)
        {
            ID_R.Value = id;
            Keyword_R.Value = keyword;
            Keyword2_R.Value = shortkwd;
            Description_R.Value = description;
            Category_R.Value = category;
            Date_R.Value = DateTime.Now;

            Initialize();
        }

        public void Initialize()
        {
            if (ID < 0) ID_R.Value = SelfMemoList.GetNextID();
            ImageSource_R = new ReactivePropertySlim<BitmapSource>();
            ImageSource_R.Value = ImageManager.GetBitmapSource(ID_R.Value);

            Format();
            SetHandler();

            HasImageSource = ImageSource_R.Select(x => x != null).ToReadOnlyReactivePropertySlim<bool>();
        }

        /// <summary>
        /// 登録文字列を独自規定に従い整形する
        /// </summary>
        private void Format()
        {
            Keyword_R.Value = Keyword_R.Value.Trim();
            Keyword2_R.Value = Keyword2_R.Value.Trim();
            Category_R.Value = Category_R.Value.Trim().Replace("\r\n", ",").Replace("\n", ",");
        }

        private void SetHandler()
        {
            Keyword_R.PropertyChanged += (_, __) => { Date_R.Value = DateTime.Now; };
            Keyword2_R.PropertyChanged += (_, __) => { Date_R.Value = DateTime.Now; };
            Description_R.PropertyChanged += (_, __) => { Date_R.Value = DateTime.Now; };
            Category_R.PropertyChanged += (_, __) => { Date_R.Value = DateTime.Now; };
        }
    }
}
