using MahApps.Metro.Controls;
using SelfMemoPrototype.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SelfMemoPrototype
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private NotifyIconEx _notify;

        public MainWindow()
        {
            InitializeComponent();

            // 常駐（アイコン）の設定
            var iconPath = new Uri("pack://application:,,,/SelfMemoPrototype;component/memo.ico", UriKind.Absolute);
            var menu = (ContextMenu)this.FindResource("sampleWinMenu");
            this._notify = new NotifyIconEx(iconPath, "SelfMemo", menu);

            this._notify.DoubleClick += (_, __) => { this.ShowWindow(); };

            // HotKeyの登録
            HotKeyManager.Initialize(this, (_, __) => ShowWindow());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // クローズ処理をキャンセルして、タスクバーの表示も消す
            e.Cancel = true;
            //this.WindowState = System.Windows.WindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // ウィンドウを閉じる際に、タスクトレイのアイコンを削除する。
            this._notify.Dispose();

            // HotKeyの登録解除
            HotKeyManager.Dispose();
        }

        private void ShowWindow()
        {
            // ウィンドウ表示&最前面に持ってくる
            if (this.WindowState == System.Windows.WindowState.Minimized)
                this.WindowState = System.Windows.WindowState.Normal;

            this.Show();
            this.Activate();
            this.ShowInTaskbar = true;
            SearchTextBox.Focus(); // 検索窓にフォーカス当てる
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (var name in files)
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

        #region タスクトレイのContextMenu用のイベント定義
        private void MenuItem_Show_Click(object sender, RoutedEventArgs e)
        {
            this.ShowWindow();
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        /*
        private void MenuItem_Show_Balloon_Click(object sender, RoutedEventArgs e)
        {
            this._notify.ShowBalloonTip(1000, "tipTitle", "tipText", ToolTipIconEx.Info);
        }
        */
        #endregion
    }
}
