using System.Collections.ObjectModel;
using System.Linq;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.Windows;

namespace TimeSignal.ViewModels
{
	/// <summary>
	/// 設定ウィンドウのViewModel
	/// </summary>
	public class SettingViewModel : ViewModel
	{
		/// <summary>
		/// 頻度
		/// </summary>
		private Models.Frequency frequency_;

		/// <summary>
		/// 音声通知
		/// </summary>
		private bool voiceNotification_;

		/// <summary>
		/// GUI通知
		/// </summary>
		private bool guiNotification_;

		/// <summary>
		/// 12時間制
		/// </summary>
		private bool twelveHourClock_;

		/// <summary>
		/// 閉じるまでの時間
		/// </summary>
		private int timeToClose_;

		/// <summary>
		/// 適用可能か
		/// </summary>
		private bool canApply_;

		/// <summary>
		/// ユーザー時間追加項目の時
		/// </summary>
		private int userTimeAddHours_;

		/// <summary>
		/// ユーザー時間追加項目の分
		/// </summary>
		private int userTimeAddMinutes_;

		/// <summary>
		/// 頻度
		/// </summary>
		public Models.Frequency Frequency
		{
			get { return frequency_; }
			set
			{
				frequency_ = value;
				RaisePropertyChanged();
				RefreshButtonState();
			}
		}

		/// <summary>
		/// 音声通知
		/// </summary>
		public bool VoiceNotification
		{
			get { return voiceNotification_; }
			set
			{
				voiceNotification_ = value;
				RaisePropertyChanged();
				RefreshButtonState();
			}
		}

		/// <summary>
		/// GUI通知
		/// </summary>
		public bool GuiNotification
		{
			get { return guiNotification_; }
			set
			{
				guiNotification_ = value;
				RaisePropertyChanged();
				RefreshButtonState();
			}
		}

		/// <summary>
		/// 12時間制
		/// </summary>
		public bool TwelveHourClock
		{
			get { return twelveHourClock_; }
			set
			{
				twelveHourClock_ = value;
				RaisePropertyChanged();
				RefreshButtonState();
			}
		}

		/// <summary>
		/// 閉じるまでの時間
		/// </summary>
		public int TimeToClose
		{
			get { return timeToClose_; }
			set
			{
				timeToClose_ = value;
				RaisePropertyChanged();
				RefreshButtonState();
			}
		}

		/// <summary>
		/// ユーザー時間追加項目の時
		/// </summary>
		public int UserTimeAddHours
		{
			get { return userTimeAddHours_; }
			set { userTimeAddHours_ = value; RaisePropertyChanged(); }
		}

		/// <summary>
		/// ユーザー時間追加項目の分
		/// </summary>
		public int UserTimeAddMinutes
		{
			get { return userTimeAddMinutes_; }
			set { userTimeAddMinutes_ = value; RaisePropertyChanged(); }
		}

		/// <summary>
		/// ユーザーによる設定時間
		/// </summary>
		public ObservableCollection<Models.UserTime> UserTimeList { get; private set; } = new ObservableCollection<Models.UserTime>();

		/// <summary>
		/// 適用可能か
		/// </summary>
		public bool CanApply
		{
			get { return canApply_; }
			set { canApply_ = value; RaisePropertyChanged(); }
		}

		#region コマンド

		/// <summary>
		/// テスト通知コマンド
		/// </summary>
		public ViewModelCommand TestNotifyCommand { get; private set; }

		/// <summary>
		/// OKコマンド
		/// </summary>
		public ViewModelCommand OkCommand { get; private set; }

		/// <summary>
		/// キャンセルコマンド
		/// </summary>
		public ViewModelCommand CancelCommand { get; private set; }

		/// <summary>
		/// 設定適用コマンド
		/// </summary>
		public ViewModelCommand ApplyCommand { get; private set; }

		/// <summary>
		/// ユーザー時間追加コマンド
		/// </summary>
		public ViewModelCommand AddUserTimeCommand { get; private set; }

		/// <summary>
		/// ユーザー時間削除コマンド
		/// </summary>
		public ListenerCommand<int> RemoveUserTimeCommand { get; private set; }

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SettingViewModel()
		{
			TestNotifyCommand = new ViewModelCommand( OnTestNotify );
			OkCommand = new ViewModelCommand( OnOk );
			CancelCommand = new ViewModelCommand( OnCancel );
			ApplyCommand = new ViewModelCommand( OnApply );
			AddUserTimeCommand = new ViewModelCommand( OnAddUserTime );
			RemoveUserTimeCommand = new ListenerCommand<int>( OnRemoveUserTime );

			UserTimeAddHours = 0;
			UserTimeAddMinutes = 0;
			Refresh();
		}

		/// <summary>
		/// パラメータをリセットする
		/// </summary>
		private void Refresh()
		{
			Models.Setting setting = Models.TimeSignal.Instance.Setting;
			Frequency = setting.Frequency;
			VoiceNotification = setting.VoiceNotification;
			GuiNotification = setting.GuiNotification;
			TwelveHourClock = setting.TwelveHourClock;
			TimeToClose = setting.TimeToClose;
			UserTimeList = new ObservableCollection<Models.UserTime>( setting.UserTimeList );

			RefreshButtonState();
		}

		/// <summary>
		/// 選択中の設定と現在の設定が違えば適用ボタンを有効にします
		/// </summary>
		private void RefreshButtonState()
		{
			bool canApply = false;
			
			Models.Setting setting = Models.TimeSignal.Instance.Setting;

			if ( Frequency != setting.Frequency ||
				 VoiceNotification != setting.VoiceNotification ||
				 GuiNotification != setting.GuiNotification ||
				 TwelveHourClock != setting.TwelveHourClock ||
				 TimeToClose != setting.TimeToClose ||
				 !UserTimeList.SequenceEqual( setting.UserTimeList ) )
			{
				canApply = true;
			}

			CanApply = canApply;
		}

		/// <summary>
		/// 現在の設定でテスト通知します
		/// </summary>
		private void OnTestNotify()
		{
			Models.TimeSignal.Instance.Notify( VoiceNotification, GuiNotification, TwelveHourClock, TimeToClose );
		}

		/// <summary>
		/// 適用してウィンドウを閉じます
		/// </summary>
		private void OnOk()
		{
			OnApply();
			Messenger.Raise( new WindowActionMessage( WindowAction.Close, "Close" ) );
		}

		/// <summary>
		/// キャンセル。ウィンドウを閉じます
		/// </summary>
		private void OnCancel()
		{
			Messenger.Raise( new WindowActionMessage( WindowAction.Close, "Close" ) );
		}

		/// <summary>
		/// 適用
		/// </summary>
		private void OnApply()
		{
			Models.Setting setting = Models.TimeSignal.Instance.Setting;
			setting.Frequency = Frequency;
			setting.VoiceNotification = VoiceNotification;
			setting.GuiNotification = GuiNotification;
			setting.TwelveHourClock = TwelveHourClock;
			setting.TimeToClose = TimeToClose;

			setting.UserTimeList.Clear();
			foreach( Models.UserTime time in UserTimeList ) {
				setting.UserTimeList.Add( time );
			}

			setting.Save();
			RefreshButtonState();
		}

		/// <summary>
		/// ユーザー時間の追加
		/// </summary>
		private void OnAddUserTime()
		{
			var time = new Models.UserTime { Hours = UserTimeAddHours, Minutes = UserTimeAddMinutes };
			UserTimeList.Add( time );

			RefreshButtonState();
		}

		/// <summary>
		/// ユーザー時間の削除
		/// </summary>
		/// <param name="userTime"></param>
		private void OnRemoveUserTime( int index )
		{
			UserTimeList.RemoveAt( index );

			RefreshButtonState();
		}
	}
}
