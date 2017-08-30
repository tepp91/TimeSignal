using System.Linq;
using System.Windows;
namespace TimeSignal.Views
{
	public class ViewModule
	{
		/// <summary>
		/// 通知ウィンドウを開きます
		/// </summary>
		/// <param name="position">表示位置（右下基準）</param>
		/// <param name="secTimeToClose">閉じるまでの時間</param>
		public void OpenNotificationWindow( Point position, int secTimeToClose )
		{
			if( !IsOpenedNotificationWindow() ) {
				var view = new NotificationWindow( position, secTimeToClose );
				view.Show();
			}
		}

		/// <summary>
		/// 通知ウィンドウが開いていればtrueを返します
		/// </summary>
		/// <returns></returns>
		public bool IsOpenedNotificationWindow()
		{
			return Application.Current.Windows.OfType<NotificationWindow>().Any();
		}

		/// <summary>
		/// 設定ウィンドウを開きます
		/// </summary>
		public void OpenSettingWindow()
		{
			if ( IsOpenedSettingWindow() ) {
				SettingWindow view = Application.Current.Windows.OfType<SettingWindow>().Single();
				view.Focus();
			}
			else {
				var view = new SettingWindow();
				view.Show();
			}
		}

		/// <summary>
		/// 設定ウィンドウが開いていればtrueを返します
		/// </summary>
		/// <returns></returns>
		public bool IsOpenedSettingWindow()
		{
			return Application.Current.Windows.OfType<SettingWindow>().Any();
		}
	}
}
