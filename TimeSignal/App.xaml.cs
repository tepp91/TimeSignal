using System.Windows;
using Livet;

namespace TimeSignal
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// インスタンス
		/// </summary>
		public static App Instance { get { return ( App )App.Current; } }

		/// <summary>
		/// ViewModule
		/// </summary>
		public Views.ViewModule ViewModule { get; } = new Views.ViewModule();

		/// <summary>
		/// エラーメッセージボックスを作成
		/// </summary>
		public void CreateErrorMessageBox(string message)
		{
			MessageBox.Show( message, TimeSignal.Properties.Resources.TEXT_APP_NAME, MessageBoxButton.OK, MessageBoxImage.Error );
		}

		/// <summary>
		/// 開始時の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			// 多重起動禁止
			var mutex = new System.Threading.Mutex( false, "PronamaChanTimeSignal" );
			if( !mutex.WaitOne( 0, false ) ) {
				Shutdown();
				return;
			}

			DispatcherHelper.UIDispatcher = Dispatcher;
			System.AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			Models.TimeSignal.Instance.Initialize();

			var notifyIcon = new Views.NotifyIcon();
			notifyIcon.CloseEvent += () => { Shutdown(); notifyIcon.Dispose(); };
			notifyIcon.SettingEvent += () => { ViewModule.OpenSettingWindow(); };
		}

		/// <summary>
		/// ハンドリングされなかった例外のハンドリング
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
		{
			// エラーメッセージ出して終了
			MessageBox.Show(
				TimeSignal.Properties.Resources.TEXT_MSG_ERROR_EXCEPTION,
				TimeSignal.Properties.Resources.TEXT_APP_NAME,
				MessageBoxButton.OK,
				MessageBoxImage.Error);

			System.Environment.Exit( 1 );
		}
	}
}
