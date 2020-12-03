using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SelfMemoPrototype
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIconEx _notify;
        private HotKeyHelper _hotkey;

        public MainWindow()
        {
            InitializeComponent();

            // 常駐（アイコン）の設定
            var iconPath = new Uri("pack://application:,,,/SelfMemoPrototype;component/memo.ico", UriKind.Absolute);
            var menu = (ContextMenu)this.FindResource("sampleWinMenu");
            this._notify = new NotifyIconEx(iconPath, "SelfMemo", menu);

            this._notify.DoubleClick += (_, __) => { this.ShowWindow(); };

            // HotKeyの登録
            this._hotkey = new HotKeyHelper(this);
            this._hotkey.Register(ModifierKeys.Alt | ModifierKeys.Shift, Key.F2,
                (_, __) => { this.ShowWindow(); });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // クローズ処理をキャンセルして、タスクバーの表示も消す
            e.Cancel = true;
            this.WindowState = System.Windows.WindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // ウィンドウを閉じる際に、タスクトレイのアイコンを削除する。
            this._notify.Dispose();

            // HotKeyの登録解除
            this._hotkey.Dispose();
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

        /*
        private void btnSetTitle_Click(object sender, RoutedEventArgs e)
        {
            this._notify.Text = this.txtTitle.Text;
        }

        private void btnShowBalloon_Click(object sender, RoutedEventArgs e)
        {
            var icon = (ToolTipIconEx)this.cmbIcon.SelectedValue;
            var title = this.txtBalloonTitle.Text;
            var text = this.txtBalloonText.Text;

            this._notify.ShowBalloonTip(100, title, text, icon);
        }
        */


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
