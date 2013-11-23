using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using Livet.Messaging.Windows;

namespace Grabacr07.KanColleViewer.ViewModels.Docks
{
	public class RepairingDockViewModel : ViewModel
	{
		private readonly RepairingDock source;
		private System.Media.SoundPlayer notifySoundPlayer;

		public int Id
		{
			get { return this.source.Id; }
		}

		public string Ship
		{
			get
			{
				return null != this.source.Ship
					? this.source.Ship.Info.Name
					: "----";
			}
		}

		public string CompleteTime
		{
			get
			{
				return this.source.CompleteTime.HasValue
					? this.source.CompleteTime.Value.LocalDateTime.ToString("MM/dd HH:mm")
					: "--/-- --:--:--";
			}
		}

		public string Remaining
		{
			get
			{
				return this.source.Remaining.HasValue
					? this.source.Remaining.Value.ToString(@"hh\:mm\:ss")
					: "--:--:--";
			}
		}

		public RepairingDockState State
		{
			get { return this.source.State; }
		}

		#region IsNotifyCompleted 変更通知プロパティ

		private bool _IsNotifyCompleted;

		public bool IsNotifyCompleted
		{
			get { return this._IsNotifyCompleted; }
			set
			{
				if (this._IsNotifyCompleted != value)
				{
					this._IsNotifyCompleted = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public RepairingDockViewModel(RepairingDock source)
		{
			this.source = source;
			this.CompositeDisposable.Add(new PropertyChangedEventListener(source, (sender, args) => this.RaisePropertyChanged(args.PropertyName)));

			if (Helper.IsWindows8OrGreater)
			{
				source.Completed += (sender, args) =>
				{
					if (this.IsNotifyCompleted)
					{
						Toast.Show(
							"整備完了",
							string.Format("入渠第 {0} ドックでの「{1}」の整備が完了しました。", this.Id, this.Ship),
							() => this.Messenger.Raise(new WindowActionMessage(WindowAction.Active, "Window/Activate")));

						var pathStr = Settings.Current.RepairingCompleteSoundFile;
						if (null != pathStr
							&& string.Empty != pathStr
							&& System.IO.File.Exists(pathStr))
						{
							try
							{
								if (null != notifySoundPlayer)
								{
									notifySoundPlayer.Stop();
									notifySoundPlayer.SoundLocation = pathStr;
								}
								else
								{
									// 新しくインスタンスを生成
									notifySoundPlayer = new System.Media.SoundPlayer(pathStr);
								}
								notifySoundPlayer.Play();
							}
							catch (Exception e)
							{
								// とりあえず握りつぶし。あとで考える
								;
							}

						}
					}
				};
			}
		}
	}
}
