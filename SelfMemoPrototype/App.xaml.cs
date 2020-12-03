using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SelfMemoPrototype
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        // Mutexの名前
        // 本来は一意になるような名前にします。
        private static readonly string mutexName = "SelfMemoPrototype0000";

        // Mutexを生成します。
        private static readonly Mutex mutex = new Mutex(false, mutexName);

        // Mutexの所有権の有無を保持するフラグ(true:所有権有り/false:所有権無し)
        private static bool hasHandle = false;

        /**
         * @breif Startupイベントを発生させます。
         * 
         * @param [in] e イベント
         */
        protected override void OnStartup(StartupEventArgs e)
        {
            // Mutexの所有権を取得します。
            // 最初に起動したアプリのみ所有権を取得できます。
            hasHandle = mutex.WaitOne(0, false);

            // 取得できない場合、すでに起動済みとみなします。
            if (!hasHandle)
            {
                MessageBox.Show("既に起動済みです。");

                // アプリケーションを終了します。
                this.Shutdown();
                return;
            }

            // 親クラスのメソッドを呼び出します。
            base.OnStartup(e);
        }

        /**
         * @breif Exitイベントを発生させます。
         * 
         * @param [in] e イベント
         */
        protected override void OnExit(ExitEventArgs e)
        {
            // 親クラスのメソッドを呼び出します。
            base.OnExit(e);

            // Mutexの所有権を保持している場合
            if (hasHandle)
            {
                // Mutexを解放します。
                mutex.ReleaseMutex();
            }

            // Mutexをクローズします。
            mutex.Close();
        }

        /**
         * @brief アプリケーションの起動処理を行います。
         *
         * @param [in] sender
         * @param [in] e イベント
         */
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            // 画面を表示します。
            var windows = new MainWindow();
            windows.Show();
        }
    }
}
