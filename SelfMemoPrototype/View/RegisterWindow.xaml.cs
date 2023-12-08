using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using SelfMemoPrototype.Model;
using MahApps.Metro.Controls;

namespace SelfMemoPrototype.View
{
    /// <summary>
    /// RegisterWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterWindow : MetroWindow
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach(var name in files)
            {
                int val = 0;
                switch (Path.GetExtension(name).ToLower())
                {
                    case ".csv":
                        val = SelfMemoList.AddMemoFromCsv(name);
                        break;
                    case ".json":
                        val = SelfMemoList.LoadMemoFile(name);
                        break;
                }

                if (val > 0)
                {
                    MessageBox.Show(name + " から " + val + " 件追加しました", "ファイルから登録", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelfMemoList.SaveMemoFile();
                    SelfMemoList.BackupMemoFile();
                }
                else
                {
                    MessageBox.Show("追加する項目が見つかりませんでした", "ファイルから登録", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
        }
    }
}
