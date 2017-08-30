using System;
using System.Threading.Tasks;

namespace TimeSignal.Models
{
	/// <summary>
	/// 時刻に合わせたサウンドを再生する
	/// </summary>
	public static class SoundPlayer
	{
		private static Task s_playTask;

		/// <summary>
		/// 再生可能か確認為ます
		/// </summary>
		/// <returns></returns>
		public static bool CanPlay()
		{
			return s_playTask == null || s_playTask.IsCompleted;
		}

		/// <summary>
		/// 再生します
		/// </summary>
		/// <param name="time"></param>
		public static void Play( DateTime time, bool twelveHourClock )
		{
			if( s_playTask != null && !s_playTask.IsCompleted ){
				return;
			}

			int hour = twelveHourClock ? time.Hour % 12 : time.Hour;
			string ampmPath = string.Format( "Voice/{0}.ogg", time.Hour < 12 ? "am" : "pm" );
			string hourPath = string.Format( "Voice/h{0:00}.ogg", hour );
			string minutePath = string.Format( "Voice/m{0:00}.ogg", time.Minute );

			Action playAction = () =>
			{
				// 時と分の再生がスムーズになるように先にデータを読み込んでおく
				System.IO.Stream ampmStream = null, hourStream = null, minuteStream = null;
				if( twelveHourClock ) {
					ampmStream = LoadFile( ampmPath );
				}
				hourStream = LoadFile( hourPath );
				if( time.Minute > 0 ) {
					minuteStream = LoadFile( minutePath );
				}

				System.Media.SoundPlayer player = new System.Media.SoundPlayer();

				// 午前・午後
				if( ampmStream != null ) {
					player.Stream = ampmStream;
					player.PlaySync();
				}

				// 時
				player.Stream = hourStream;
				player.PlaySync();

				// 分
				if ( minuteStream != null ) {
					player.Stream = minuteStream;
					player.PlaySync();
				}
			};
			s_playTask = Task.Run( playAction );
		}

		/// <summary>
		/// ファイルを読み込みます
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static System.IO.MemoryStream LoadFile(string path)
		{
			var argPath = new System.Text.StringBuilder( path );
			if( OggDecoder.OggInitialize( argPath ) ) {
				uint bufferSize = OggDecoder.OggExpandSize();
				byte[] buffer = new byte[ bufferSize ];
				OggDecoder.OggDecode( buffer );
				OggDecoder.OggFinalize();

				var stream = new System.IO.MemoryStream();
				stream.Write( buffer, 0, ( int )bufferSize );
				stream.Seek( 0, System.IO.SeekOrigin.Begin );

				return stream;
			}

			return null;
		}
	}
}
