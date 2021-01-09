using Hnx8.ReadJEnc;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SelfMemoPrototype.Model
{
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
            var newlist = new List<string>();

            if (_categoryList == null)
            {
                _categoryList = new ReactiveCollection<string>();
            }

            // ItemsListを全件検索
            foreach (var item in ItemsList)
            {
                if (!newlist.Contains(item.CategoryR.Value))
                {
                    newlist.Add(item.CategoryR.Value);
                }
            }

            newlist.Sort();
            _categoryList.Clear();
            foreach (var s in newlist) { _categoryList.Add(s); }

            return true;
        }

        /// <summary>
        /// ファイルからメモリストの情報を読み出して引数のリストに追加する
        /// </summary>
        /// <param name="memoList">追加する対象のリスト</param>
        /// <param name="filename">読み出すファイル</param>
        /// <returns>追加された項目数</returns>
        public static int LoadMemoFile(ReactiveCollection<SelfMemoItem> memoList, string filename)
        {
            int ret = 0;
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
                    if (!memoList.Contains(m))
                    {
                        memoList.Add(m);
                        ret++;
                    }
                }
            }
            catch (Exception e)
            {
                //error
            }
            return ret;
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


        /// <summary>
        /// csvからメモリストを読み込んで指定のリストに追加する
        /// </summary>
        /// <param name="memoList">追加する対象のリスト</param>
        /// <param name="filename">csvファイル</param>
        /// <returns>追加された項目数</returns>
        public static int AddMemoFromCsv(ReactiveCollection<SelfMemoItem> memoList, string filename)
        {
            if (Path.GetExtension(filename).ToLower() != ".csv") return 0;
            int ret = 0;

            FileInfo info = new FileInfo(filename);
            Encoding enc;

            // 文字コードを判別
            using (FileReader reader = new FileReader(info))
            {
                enc = reader.Read(info).GetEncoding();
            }

            try
            {
                StreamReader sr = new StreamReader(filename, enc);
                string line = "";

                while (true)
                {
                    int cnt = 0;
                    bool detectLineBreak = false;

                    line = sr.ReadLine();
                    if (line == null) break;
                    cnt = CountOf(line, "\""); // ダブルクオートの数（改行の有無判断に使用）

                    do
                    {
                        if (detectLineBreak)
                        {
                            // 改行が検出された。もう一行読み出して末尾につなげる
                            var str = sr.ReadLine();
                            if (str == null) break;
                            line += "\n" + str;

                            // ダブルクオートの数が偶数なら改行フラグ継続
                            cnt = CountOf(str, "\"");
                            detectLineBreak = (cnt % 2 == 0);
                        }
                        else
                        {
                            // 通常なら、ダブルクオート数の偶奇で改行の有無を判断
                            detectLineBreak = !(cnt % 2 == 0);
                        }
                    } while (detectLineBreak);

                    var separated = line.Split(',');
                    if (separated.Length >= 4)
                    {
                        var m = new SelfMemoItem(separated[0].Trim('\"'), separated[1].Trim('\"'), separated[2].Trim('\"'), separated[3].Trim('\"'));
                        if (!memoList.Contains(m))
                        {
                            memoList.Add(m);
                            ret++;
                        }
                    }
                }

                sr.Close();

            }
            catch (Exception e)
            {
                //error
            }
            return ret;
        }

        /// <summary>
        /// 文字列に特定の文字がいくつ含まれるかを返す
        /// </summary>
        /// <param name="target">対象の文字列</param>
        /// <param name="strArray">個数を数える対象の文字列</param>
        /// <returns></returns>
        private static int CountOf(string target, params string[] strArray)
        {
            int count = 0;

            foreach (string str in strArray)
            {
                int index = target.IndexOf(str, 0);
                while (index != -1)
                {
                    count++;
                    index = target.IndexOf(str, index + str.Length);
                }
            }

            return count;
        }

    }
}
