using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Media.Imaging;

namespace SelfMemoPrototype.ViewModel
{
    class RegisterViewModel : BindableBase
    {
        private ReactiveCollection<SelfMemoItem> MemoList
        {
            get
            {
                return SelfMemoList.ItemsList;
            }
        }

        public ReactiveCollection<string> CategoryList
        {
            get
            {
                return SelfMemoList.CategoryList;
            }
        }

        public ReactivePropertySlim<string> Word { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> ShortWord { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Description { get; set; } = new ReactivePropertySlim<string>("");
        public ReactivePropertySlim<string> Category { get; set; } = new ReactivePropertySlim<string>("");

        public ReactivePropertySlim<bool> IsSelected { get; set; } = new ReactivePropertySlim<bool>();

        public ReactivePropertySlim<BitmapSource> ImageSource { get; set; } = new ReactivePropertySlim<BitmapSource>();

        public ReactivePropertySlim<RegisterTemplate> Template { get; set; } = new ReactivePropertySlim<RegisterTemplate>();

        public ReactiveCollection<RegisterTemplate> Templates { get; set; } = new ReactiveCollection<RegisterTemplate>();


        public RegisterViewModel()
        {
            var items = RegisterTemplate.LoadTemplateJson("template.json");
            if(items != null)
            {
                foreach(var item in items)
                {
                    Templates.Add(item);
                }
            }
            if (Templates.Count == 0) Templates.Add(new RegisterTemplate());
            Template.Value = Templates.ElementAt(0);
        }

        #region AddMemoItemCommand
        private DelegateCommand _addMemoItemCmd;
        public DelegateCommand AddMemoItemCmd
        {
            get { return _addMemoItemCmd = _addMemoItemCmd ?? new DelegateCommand(AddMemoToList); }
        }

        private DelegateCommand _pasteImageCmd;

        public DelegateCommand PasteImageCmd
        {
            get => _pasteImageCmd = _pasteImageCmd ?? new DelegateCommand(() =>
            {
                ImageSource.Value = ClipboardCapture.GetBitmap();
            });
        }

        /// <summary>
        /// プロパティに保持中の情報を新規SelfMemoとして追加する
        /// </summary>
        private void AddMemoToList()
        {
            SelfMemoItem newmemo = new SelfMemoItem(Word.Value, ShortWord.Value, Description.Value, Category.Value);

            if (!MemoList.Contains(newmemo))
            {
                // Imageがあれば保存
                if(ImageSource.Value != null)
                {
                    ImageManager.SaveImageFile(ImageSource.Value, newmemo.ID_R.Value);
                    newmemo.ImageSource_R.Value = ImageSource.Value;
                }

                // MemoListに追加
                MemoList.Add(newmemo);

                SelfMemoList.SaveMemoFile();

                // 前回バックアップから1日以上経っていたらバックアップ実行
                if ((DateTime.Now - SelfMemoList.LastBackupDate).TotalDays > 1.0)
                    SelfMemoList.BackupMemoFile();


                // プロパティを空白で初期化
                Word.Value = "";
                ShortWord.Value = "";
                Description.Value = "";
                Category.Value = "";
                ImageSource.Value = null;

                // WordのTextboxをフォーカスする
                IsSelected.Value = true;
            }
        }
        #endregion
    }

    [DataContract]
    class RegisterTemplate
    {
        [DataMember]
        public string Name { get; set; } = "デフォルト";

        [DataMember]
        public string Keyword1Hint { get; set; } = "キーワード(用語)";

        [DataMember]
        public string Keyword2Hint { get; set; } = "キーワード2(略語・別名・訳語など)";

        [DataMember]
        public string DescriptionHint { get; set; } = "説明";

        public override string ToString()
        {
            return Name;
        }

        public RegisterTemplate(string name, string kw1, string kw2, string desc)
        {
            Name = name;
            Keyword1Hint = kw1;
            Keyword2Hint = kw2;
            DescriptionHint = desc;
        }

        public RegisterTemplate() { }

        public static ReactiveCollection<RegisterTemplate> LoadTemplateJson(string filename)
        {
            if(!File.Exists(filename)) return null;
            ReactiveCollection<RegisterTemplate> items;
            using (var ms = new FileStream(filename, FileMode.Open))
            { 
                var serializer = new DataContractJsonSerializer(typeof(ReactiveCollection<RegisterTemplate>));
                items = (ReactiveCollection<RegisterTemplate>)serializer.ReadObject(ms);
            }

            return items;
        }
    }
}
