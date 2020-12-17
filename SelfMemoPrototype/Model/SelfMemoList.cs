﻿using Hnx8.ReadJEnc;
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
            foreach (var item in list)
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
            foreach (var item in remList)
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

            FileInfo info = new FileInfo(filename);
            Encoding enc;

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
                    bool flag = false;

                    line = sr.ReadLine();
                    if (line == null) break;
                    cnt = CountOf(line, "\""); // "の数で改行の有無を判断

                    do
                    {
                        if (flag)
                        {
                            // 改行が検出された。もう一行読み出して末尾につなげる
                            var str = sr.ReadLine();
                            if (str == null) break;
                            line += "\n" + str;

                            // ダブルクオートの数が偶数なら改行フラグ継続
                            cnt = CountOf(str, "\"");
                            flag = (cnt % 2 == 0);
                        }
                        else
                        {
                            // 通常なら、ダブルクオート数が偶数なら改行無しと判断
                            flag = !(cnt % 2 == 0);
                        }
                    } while (flag);

                    var separated = line.Split(',');
                    if (separated.Length >= 4)
                    {
                        var m = new SelfMemoItem(separated[0].Trim('\"'), separated[1].Trim('\"'), separated[2].Trim('\"'), separated[3].Trim('\"'));
                        memoList.Add(m);
                        ret++;
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
