using System;
using System.Windows.Forms;

namespace TimeSignal.Views
{
	delegate void NotifyIconSettingEventHandler();
	delegate void NotifyIconCloseEventHanlder();

	class NotifyIcon : System.IDisposable
	{
		/// <summary>アイコンイメージファイルのパス</summary>
		private readonly string IconImagePath = "pack://application:,,,/Resources/icon.ico";
		/// <summary>通知アイコン本体</summary>
		private System.Windows.Forms.NotifyIcon notifyIcon_;

		/// <summary>
		/// 設定イベント
		/// </summary>
		public event NotifyIconSettingEventHandler SettingEvent = delegate {};

		/// <summary>
		/// 終了イベント
		/// </summary>
		public event NotifyIconCloseEventHanlder CloseEvent = delegate {};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NotifyIcon()
		{
			notifyIcon_ = new System.Windows.Forms.NotifyIcon();
			notifyIcon_.Visible = true;

			System.IO.Stream stream = App.GetResourceStream( new System.Uri( IconImagePath ) ).Stream;
			notifyIcon_.Icon = new System.Drawing.Icon( stream );

			var menuStrip = new ContextMenuStrip();

			// 設定
			var itemSetting = new ToolStripMenuItem();
			itemSetting.Text = Properties.Resources.TEXT_NOTIFY_ICON_MENU_SETTING;
			itemSetting.Click += (s, e) => SettingEvent();
			menuStrip.Items.Add( itemSetting );

			// 閉じる
			var itemClose = new ToolStripMenuItem();
			itemClose.Text = Properties.Resources.TEXT_NOTIFY_ICON_MENU_CLOSE;
			itemClose.Click += (s, e) => CloseEvent();
			menuStrip.Items.Add( itemClose );

			notifyIcon_.ContextMenuStrip = menuStrip;
		}

		/// <summary>
		/// デストラクタ
		/// </summary>
		~NotifyIcon()
		{
			Dispose();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if( notifyIcon_ != null ) {
				notifyIcon_.Visible = false;
				notifyIcon_.Icon = null;
				notifyIcon_.Dispose();
				notifyIcon_ = null;
			}
		}
	}
}
