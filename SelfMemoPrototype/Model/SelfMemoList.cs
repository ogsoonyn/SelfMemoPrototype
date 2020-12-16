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


        /// <summary>
        /// csvからメモリストを読み込んで指定のリストに追加する
        /// </summary>
        /// <param name="memoList">追加する対象のリスト</param>
        /// <param name="filename">csvファイル</param>
        /// <returns></returns>
        public static int AddMemoFromCsv(ReactiveCollection<SelfMemoItem> memoList, string filename)
        {
            if (Path.GetExtension(filename).ToLower() != ".csv") return 0;
            int ret = 0;

            try
            {
                // TODO: エンコード Shift-JISとUTF8を解釈したい
                StreamReader sr = new StreamReader(filename, Encoding.UTF8);
                string line = sr.ReadLine();

                while (line != null)
                {
                    // TODO: 改行が含まれるcsvファイルにも対応したい。項目がダブルクオートで囲われてたら取り払いたい
                    var separated = line.Split(',');
                    if(separated.Length >= 4)
                    {
                        var m = new SelfMemoItem(separated[0], separated[1], separated[2], separated[3]);
                        memoList.Add(m);
                        ret++;
                    }

                    line = sr.ReadLine();
                }

                sr.Close();

            }catch(Exception e)
            {
                //error
            }
            return ret;
        }

    }
}
