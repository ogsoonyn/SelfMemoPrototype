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
//using System.Windows.Shapes;
using System.IO;
using SelfMemoPrototype.Model;

namespace SelfMemoPrototype.View
{
    /// <summary>
    /// RegisterWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach(var name in files)
            {
                int val = SelfMemoList.AddMemoFromCsv(SelfMemoList.ItemsList, name);
                if(val > 0)
                {
                    MessageBox.Show(val + "件追加しました", "csvから登録", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
