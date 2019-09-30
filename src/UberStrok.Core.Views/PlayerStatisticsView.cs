using System;
using System.Text;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class PlayerStatisticsView
	{
		public PlayerStatisticsView()
		{
            PersonalRecord = new PlayerPersonalRecordStatisticsView();
            WeaponStatistics = new PlayerWeaponStatisticsView();
		}

		public PlayerStatisticsView(
            int cmid, 
            int splats,
            int splatted,
            long shots,
            long hits,
            int headshots,
            int nutshots,
            PlayerPersonalRecordStatisticsView personalRecord,
            PlayerWeaponStatisticsView weaponStatistics)
		{
            Cmid = cmid;
            Hits = hits;
			Level = 0;
			Shots = shots;
			Splats = splats;
			Splatted = splatted;
			Headshots = headshots;
			Nutshots = nutshots;
			Xp = 0;
			PersonalRecord = personalRecord;
			WeaponStatistics = weaponStatistics;
		}

		public PlayerStatisticsView(
            int cmid,
            int splats,
            int splatted,
            long shots,
            long hits,
            int headshots,
            int nutshots,
            int xp,
            int level,
            PlayerPersonalRecordStatisticsView personalRecord,
            PlayerWeaponStatisticsView weaponStatistics)
		{
			Cmid = cmid;
			Hits = hits;
			Level = level;
			Shots = shots;
			Splats = splats;
			Splatted = splatted;
			Headshots = headshots;
			Nutshots = nutshots;
			Xp = xp;
			PersonalRecord = personalRecord;
			WeaponStatistics = weaponStatistics;
		}

		public override string ToString()
		{
            var builder = new StringBuilder().Append("[PlayerStatisticsView: ")
                .Append("[Cmid: ").Append(Cmid)
                .Append("][Hits: ").Append(Hits)
                .Append("][Level: ").Append(Level)
                .Append("][Shots: ").Append(Shots)
                .Append("][Splats: ").Append(Splats)
                .Append("][Splatted: ").Append(Splatted)
                .Append("][Headshots: ").Append(Headshots)
                .Append("][Nutshots: ").Append(Nutshots)
                .Append("][Xp: ").Append(Xp)
                .Append("]")
                .Append(PersonalRecord)
                .Append(WeaponStatistics)
            .Append("]");
			return builder.ToString();
		}

		public int Cmid { get; set; }
		public int Headshots { get; set; }
		public long Hits { get; set; }
		public int Level { get; set; }
		public int Nutshots { get; set; }
		public PlayerPersonalRecordStatisticsView PersonalRecord { get; set; }
		public long Shots { get; set; }
		public int Splats { get; set; }
		public int Splatted { get; set; }
		public int TimeSpentInGame { get; set; }
		public PlayerWeaponStatisticsView WeaponStatistics { get; set; }
		public int Xp { get; set; }
	}
}
