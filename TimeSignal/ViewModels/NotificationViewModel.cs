using Livet.EventListeners;

namespace TimeSignal.ViewModels
{
	/// <summary>
	/// 通知ウィンドウのViewModel
	/// </summary>
	public class NotificationViewModel : ViewModel
	{
		/// <summary>時間</summary>
		private string time_;
		/// <summary>イベントリスナ</summary>
		private PropertyChangedEventListener eventListener_;

		/// <summary>
		/// 時間
		/// </summary>
		public string Time
		{
			get { return time_; }
			set { time_ = value;  RaisePropertyChanged(); }
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize()
		{
			eventListener_ = new PropertyChangedEventListener( Models.TimeSignal.Instance );
			eventListener_.RegisterHandler( "CurrentTimeText", (s, e) => OnChangeDateTime() );
			OnChangeDateTime();
		}

		/// <summary>
		/// 時間が変更された際の処理
		/// </summary>
		private void OnChangeDateTime()
		{
			Time = Models.TimeSignal.Instance.CurrentTimeText;
		}
	}
}
