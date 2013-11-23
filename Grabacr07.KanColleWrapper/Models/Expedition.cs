using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 遠征を表します。
	/// </summary>
	public class Expedition : TimerNotificator, IIdentifiable
	{
		private readonly Fleet fleet;
		private bool notificated;

		#region IsInExecution 変更通知プロパティ

		private bool _IsInExecution;

		/// <summary>
		/// 現在遠征を実行中かどうかを示す値を取得します。
		/// </summary>
		public bool IsInExecution
		{
			get { return this._IsInExecution; }
			private set
			{
				if (this._IsInExecution != value)
				{
					this._IsInExecution = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Id 変更通知プロパティ

		private int _Id;

		public int Id
		{
			get { return this._Id; }
			private set
			{
				if (this._Id != value)
				{
					this._Id = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ReturnTime 変更通知プロパティ

		private DateTimeOffset? _ReturnTime;

		public DateTimeOffset? ReturnTime
		{
			get { return this._ReturnTime; }
			private set
			{
				if (this._ReturnTime != value)
				{
					this._ReturnTime = value;
					this.notificated = false;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Remaining 変更通知プロパティ

		private TimeSpan? _Remaining;

		public TimeSpan? Remaining
		{
			get { return this._Remaining; }
			set
			{
				if (this._Remaining != value)
				{
					this._Remaining = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Name 変更通知プロパティ

		private string _Name;

		public string Name
		{
			get { return this._Name; }
			private set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		/// <summary>
		/// 艦隊が遠征から帰ったときに発生します。
		/// </summary>
		public event EventHandler<ExpeditionReturnedEventArgs> Returned;


		public Expedition(Fleet fleet)
		{
			this.fleet = fleet;
		}

		internal void Update(long[] rawData)
		{
			if (rawData.Length != 4 || rawData.All(x => x == 0))
			{
				this.IsInExecution = false;
				this.ReturnTime = null;
				this.Remaining = null;
				this.Name = null;
			}
			else
			{
				this.Id = (int)rawData[1];
				this.ReturnTime = Definitions.UnixEpoch.AddMilliseconds(rawData[2]);
				this.IsInExecution = true;
				this.Name = ExpeditionNameDic.ContainsKey(this.Id) ? ExpeditionNameDic[this.Id] : unknownName;
				this.UpdateCore();
			}
		}

		private void UpdateCore()
		{
			if (this.ReturnTime.HasValue)
			{
				var remaining = this.ReturnTime.Value - TimeSpan.FromMinutes(1.0) - DateTimeOffset.Now;
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated && this.Returned != null && remaining.Ticks <= 0)
				{
					this.Returned(this, new ExpeditionReturnedEventArgs(this.fleet.Name));
					this.notificated = true;
				}
			}
			else
			{
				this.Remaining = null;
			}
		}

		protected override void Tick()
		{
			base.Tick();
			this.UpdateCore();
		}
		const string unknownName = "Unknown";
		protected Dictionary<int, string> ExpeditionNameDic = new Dictionary<int, string>() 
        { 
			{1,"練習航海"},
			{2,"長距離練習航海"},
			{3,"警備任務"},
			{4,"対潜警戒任務"},
			{5,"海上護衛任務"},
			{6,"防空射撃演習"},
			{7,"観艦式予行"},
			{8,"観艦式"},
			{9,"タンカー護衛任務"},
			{10,"強行偵察任務"},
			{11,"ボーキサイト輸送任務"},
			{12,"資源輸送任務"},
			{13,"鼠輸送作戦"},
			{14,"包囲陸戦隊撤収作戦"},
			{15,"囮機動部隊支援作戦"},
			{16,"艦隊決戦援護作戦"},
			{17,"敵地偵察作戦"},
			{18,"航空機輸送作戦"},
			{19,"北号作戦"},
			{20,"潜水艦哨戒任務"},
			{21,unknownName},
			{22,unknownName},
			{23,unknownName},
			{24,unknownName},
			{25,"通商破壊作戦"},
			{26,"敵母港空襲作戦"},
			{27,"潜水艦通商破壊作戦"},
			{28,unknownName},
			{29,unknownName},
			{30,unknownName},
			{31,unknownName},
			{32,unknownName},
			{33,"前衛支援任務"},
			{34,"艦隊決戦支援任務"},
			{35,"MO作戦"},
			{36,"水上機基地建設"}
        };

	}
}
