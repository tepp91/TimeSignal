using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TimeSignal.Models
{
	/// <summary>
	/// 頻度
	/// </summary>
	public enum Frequency
	{
		[Display( Name = "なし" )]
		MinutesNone = 0,

		[Display( Name = "1分ごと" )]
		Minutes1 = 1,

		[Display( Name = "15分ごと" )]
		Minutes15 = 15,

		[Display( Name = "30分ごと" )]
		Minutes30 = 30,

		[Display( Name = "60分ごと" )]
		Minutes60 = 60,
	}

	/// <summary>
	/// ユーザー指定の時間
	/// </summary>
	public struct UserTime
	{
		/// <summary>
		/// 時
		/// </summary>
		public int Hours { get; set; }

		/// <summary>
		/// 分
		/// </summary>
		public int Minutes { get; set; }

		/// <summary>
		/// 文字列への変換
		/// </summary>
		public override string ToString()
		{
			return string.Format( "{0:00}:{1:00}", Hours, Minutes );
		}
	}

	/// <summary>
	/// 比較
	/// </summary>
	class UserTimeComparer : IEqualityComparer<UserTime>
	{
		public bool Equals(UserTime x, UserTime y)
		{
			// NULLチェック
			if( object.ReferenceEquals( x, null ) || object.ReferenceEquals( y, null ) ) {
				return false;
			}

			if( object.ReferenceEquals( x, y ) ) {
				return true;
			}

			return x.Hours == y.Hours && x.Minutes == y.Hours;
		}

		public int GetHashCode(UserTime obj)
		{
			if ( object.ReferenceEquals(obj, null ) ) {
				return 0;
			}

			return obj.Hours.GetHashCode() ^ obj.Minutes.GetHashCode();
		}
	}

	/// <summary>
	/// 設定
	/// </summary>
	public class Setting
	{
		/// <summary>
		/// 頻度
		/// </summary>
		public Frequency Frequency { get; set; } = Frequency.Minutes30;

		/// <summary>
		/// 音声による通知
		/// </summary>
		public bool VoiceNotification { get; set; } = true;

		/// <summary>
		/// GUIによる通知
		/// </summary>
		public bool GuiNotification { get; set; } = true;

		/// <summary>
		/// 12時間制
		/// </summary>
		public bool TwelveHourClock { get; set; } = false;

		/// <summary>
		/// GUI通知の表示時間
		/// </summary>
		public int TimeToClose { get; set; } = 5;

		/// <summary>
		/// ユーザーによる設定時間
		/// </summary>
		public Collection<UserTime> UserTimeList { get; set; } = new Collection<UserTime>();

		/// <summary>
		/// 保存します
		/// </summary>
		public void Save()
		{
			var serializer = new XmlSerializer( typeof( Setting ) );

			try {
				using( var writer = new StreamWriter( "setting.xml" ) ) {
					serializer.Serialize( writer, this );
				}
			}
			catch( System.Exception ) {
				App.Instance.CreateErrorMessageBox( Properties.Resources.TEXT_MSG_ERROR_FAILED_SAVE_SETTING_FILE );
			}
		}

		/// <summary>
		/// 読み込みます
		/// </summary>
		public void Load()
		{
			var serializer = new XmlSerializer( typeof( Setting ) );

			try {
				using( var reader = new StreamReader( "setting.xml" ) ) {
					Setting setting = ( Setting )serializer.Deserialize( reader );

					Frequency = setting.Frequency;
					VoiceNotification = setting.VoiceNotification;
					GuiNotification = setting.GuiNotification;
					TwelveHourClock = setting.TwelveHourClock;
					TimeToClose = setting.TimeToClose;
					UserTimeList = setting.UserTimeList;
				}
			}
			catch( FileNotFoundException ) {
				// ないならないでよい
			}
			catch( System.Exception ) {
				App.Instance.CreateErrorMessageBox( Properties.Resources.TEXT_MSG_ERROR_FAILED_LOAD_SETTING_FILE );
				// 次回以降も同じ事が起きないように上書き保存
				Save();
			}
		}
	}
}
