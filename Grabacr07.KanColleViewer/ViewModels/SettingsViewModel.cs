using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class SettingsViewModel : ViewModel
	{
		public string ScreenshotFolder
		{
			get { return Settings.Current.ScreenshotFolder; }
			set { Settings.Current.ScreenshotFolder = value; }
		}
		public string BuildingCompleteSoundFile
		{
			get { return Settings.Current.BuildingCompleteSoundFile; }
			set { Settings.Current.BuildingCompleteSoundFile = value; }
		}
		public string ExpeditionReturnedSoundFile
		{
			get { return Settings.Current.ExpeditionReturnedSoundFile; }
			set { Settings.Current.ExpeditionReturnedSoundFile = value; }
		}
		public string RepairingCompleteSoundFile
		{
			get { return Settings.Current.RepairingCompleteSoundFile; }
			set { Settings.Current.RepairingCompleteSoundFile = value; }
		}

		public SettingsViewModel()
		{
			this.CompositeDisposable.Add(new PropertyChangedEventListener(Settings.Current)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});
		}
	}
}
