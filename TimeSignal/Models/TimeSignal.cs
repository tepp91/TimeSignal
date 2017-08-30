using System;
using System.Windows.Threading;
using Livet;

namespace TimeSignal.Models
{
	public class TimeSignal : NotificationObject
	{
		/// <summary>タイマー</summary>
		private DispatcherTimer timer_ = new DispatcherTimer( DispatcherPriority.Normal );
		/// <summary>現在の時間</summary>
		private string currentTimeText_ = "";
		/// <summary>通知ウィンドウの位置</summary>
		private System.Windows.Point position_ = new System.Windows.Point();

		/// <summary>設定</summary>
		public Setting Setting { get; private set; } = new Setting();

		/// <summary>
		/// 唯一のインスタンス
		/// </summary>
		public static TimeSignal Instance { get; private set; } = new TimeSignal();

		/// <summary>
		/// 現在の時間
		/// </summary>
		public DateTime CurrentTime { get; private set; }

		/// <summary>
		/// 現在の時間の文字列
		/// </summary>
		public string CurrentTimeText
		{
			get { return currentTimeText_; }
			private set { currentTimeText_ = value; RaisePropertyChanged(); }
		}

		/// <summary>
		/// 表示位置のオフセット
		/// </summary>
		public System.Windows.Point Position
		{
			get { return position_; }
			private set { position_ = value; RaisePropertyChanged(); }
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize()
		{
			Setting.Load();

			System.Windows.Point position = new System.Windows.Point();
			position.X = System.Windows.SystemParameters.WorkArea.Width;
			position.Y = System.Windows.SystemParameters.WorkArea.Height;
			Position = position;

			CurrentTime = DateTime.Now;
			timer_.Interval = new TimeSpan( 0, 0, 1 );
			timer_.Tick += new EventHandler( OnTick );
			timer_.Start();
		}

		/// <summary>
		/// 通知します
		/// </summary>
		public void Notify(bool voice, bool gui, bool twelveHourClock, int timeToClose )
		{
			if( SoundPlayer.CanPlay() && !App.Instance.ViewModule.IsOpenedNotificationWindow() ) {
				if( gui ) {
					App.Instance.ViewModule.OpenNotificationWindow( Position, timeToClose );
				}

				if( voice ) {
					SoundPlayer.Play( CurrentTime, twelveHourClock );
				}
			}
		}

		/// <summary>
		/// コンストラクタ
		/// インスタンス生成制限のためにprivate化
		/// </summary>
		private TimeSignal()
		{
		}

		/// <summary>
		/// 毎秒イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTick(object sender, EventArgs e)
		{
			DateTime oldTime = CurrentTime;
			CurrentTime = DateTime.Now;

			string timeText = CurrentTime.ToString( "HH:mm:ss" );
			if( Setting.TwelveHourClock ) {
				string am = Properties.Resources.TEXT_NOTIFICATION_AM;
				string pm = Properties.Resources.TEXT_NOTIFICATION_PM;
				timeText += string.Format( " {0}", CurrentTime.Hour < 12 ? am : pm );
			}
			CurrentTimeText = timeText;

			var startUnixTime = new DateTime( 1970, 1, 1 );

			int oldTotalMinutes = ( int )(oldTime - startUnixTime).TotalMinutes;
			int newTotalMinutes = ( int )(CurrentTime - startUnixTime).TotalMinutes;

			if( oldTotalMinutes != newTotalMinutes ) {
				// 定期
				int frequency = ( int )Setting.Frequency % 60;
				if ( frequency != 0 && CurrentTime.Minute % frequency == 0 ) {
					NotifyCurrentSetting();
					return;
				}

				// ユーザー時間
				foreach( UserTime userTime in Setting.UserTimeList ) {
					if ( CurrentTime.Hour == userTime.Hours && CurrentTime.Minute == userTime.Minutes ) {
						NotifyCurrentSetting();
						return;
					}
				}
			}
		}

		/// <summary>
		/// 現在の設定で通知します
		/// </summary>
		private void NotifyCurrentSetting()
		{
			Notify( Setting.VoiceNotification, Setting.GuiNotification, Setting.TwelveHourClock, Setting.TimeToClose );
		}
	}
}
