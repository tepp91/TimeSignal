using System.Windows;
using System.Windows.Threading;

namespace TimeSignal.Views
{
	/// <summary>
	/// NotificationWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class NotificationWindow : Window
	{
		/// <summary>閉じるためのタイマー</summary>
		private DispatcherTimer timer_ = new DispatcherTimer( DispatcherPriority.Normal );

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NotificationWindow(Point position, int secTimeToClose)
		{
			InitializeComponent();
			timer_.Interval = new System.TimeSpan( 0, 0, secTimeToClose );
			timer_.Tick += (s, e) => Close();
			timer_.Start();

			Top = position.Y - Height;
			Left = position.X - Width;
		}
	}
}