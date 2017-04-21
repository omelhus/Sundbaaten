using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Sundbaten
{
	public class TimeTable : INotifyPropertyChanged
	{
		public TimeTable()
		{
			BaseTimeTable = new TimeTableCollection();
			Device.StartTimer(TimeSpan.FromSeconds(1), () =>
			{
                NotifyPropertyChanged(nameof(NesteAvgang));
				NotifyPropertyChanged(nameof(VisNesteAvgang));
                NotifyPropertyChanged(nameof(NesteAvgangStr));             
				return true;
			});

		}

		public event PropertyChangedEventHandler PropertyChanged;

		void NotifyPropertyChanged([CallerMemberName] string member = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(member));
		}
		public bool VisNesteAvgang => NesteAvgang.HasValue;
		public string NesteAvgangStr
		{
			get
			{
				if (NesteAvgang == null)
					return "Ingen fler avganger i dag";
				if (NesteAvgang.Value.Hours == 0){
					int min = (int)Math.Ceiling(NesteAvgang.Value.TotalMinutes);
					if (min == 0)
					{
						return "NÅ";
					}
					return $"{min:0} minutt{(min > 1 ? "er" : "")}";
				}
				else
				{
					var time = NesteAvgang.Value.Hours;
					var min = NesteAvgang.Value.Minutes;
					var dager = NesteAvgang.Value.Days;
					if (dager > 0)
					{
						return $"{dager:0} dag{(dager > 1 ? "er" : "")}, " +
								$"{time:0} time{(time > 1 ? "r" : "")} og " +
								$"{min:0} minutt{(min > 1 ? "er" : "")}";
						
					}
					else
					{
						return $"{time:0} time{(time > 1 ? "r" : "")} og {min:0} minutt{(min > 1 ? "er" : "")}";
					}
				}
			}
		}

		public TimeSpan? NesteAvgang
		{
			get
			{
				return BaseTimeTable.OrderBy(x=>x.Kirklandet).FirstOrDefault(x => !x.HarForlattKirkelandet)?.Kirklandet - DateTimeOffset.UtcNow;
			}
		}

		public TimeTableCollection BaseTimeTable
		{
			get;
			set;
		}
	}

	public class TimeTableCollection : ObservableCollection<TimeTableEntry>
	{
		DateTimeOffset day
		{
			get
			{
				var now = DateTimeOffset.UtcNow;
				var today = DateTime.Today;
				if (today.DayOfWeek == DayOfWeek.Sunday && ErSesong)
				{
					if (now > today.AddHours(15).AddMinutes(33))
					{
						return today.AddDays(1);
					}
				}
				else if (today.DayOfWeek == DayOfWeek.Sunday)
				{
					return today.AddDays(1);
				}
				else if (today.DayOfWeek == DayOfWeek.Saturday && now > today.AddHours(17).AddMinutes(03))
				{
					if (ErDatoSesong(now.AddDays(1)))
						return today.AddDays(1);
					return today.AddDays(2);
				}
				else if (now > today.AddHours(17).AddMinutes(03))
				{
					return today.AddDays(1);
				}
				return today;
			}
		}

		public TimeTableCollection() : base()
		{
			ResetTimeTable();
			Device.StartTimer(TimeSpan.FromSeconds(1), () =>
			{
				if (Items.Count > 0)
					HouseKeeping();
				else
					ResetTimeTable();
                NotifyPropertyChanged(nameof(Title));
				return true;
			});
		}

		void NotifyPropertyChanged([CallerMemberName] string member = null)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(member));
		}

		private bool ErSesong
		{
			get
			{
				var now = DateTimeOffset.Now;
				return ErDatoSesong(now);
			}
		}

		private bool ErDatoSesong(DateTimeOffset now)
		{
			var startSesong = new DateTimeOffset(now.Year, 5, 14, 0, 0, 0, TimeSpan.Zero);
			var sluttSesong = new DateTimeOffset(now.Year, 8, 27, 0, 0, 0, TimeSpan.Zero);
			return now >= startSesong && now <= sluttSesong;
		}

		 void ResetTimeTable()
		{
			IEnumerable<TimeTableEntry> entries = null;
			switch (day.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					if (ErSesong)
						entries = Sondag();
					break;
				case DayOfWeek.Saturday:
					entries = Lordag();
					break;
				default:
					entries = Hverdag();
					break;
			}
			if (entries == null) return;
			foreach (var item in entries)
				if (!item.HarReturnertKirkelandet)
					Add(item);
		}

		public string Title
		{
			get
			{
				var days = new[] { "Man", "Tirs", "Ons", "Tors", "Fre", "Lør", "Søn" };
				return $"{days[(int)day.DayOfWeek-1]}dag";
				//return day.DayOfWeek == DayOfWeek.Sunday && !ErSesong ? "Ingen ruter på Søndag" : 
				//	      day.DayOfWeek == DayOfWeek.Sunday && ErSesong ? "Søndag" :
				//	      day.DayOfWeek == DayOfWeek.Saturday ? "Lørdag" : 
				//	      $"Hverdager";
			}
		}

		void HouseKeeping()
		{
			if (Items.Count > 0)
			{
				foreach (var item in Items.ToArray())
				{
					if (item.HarReturnertKirkelandet)
					{
						Remove(item);
					}
				}
			}
		}

		IEnumerable<TimeTableEntry> Hverdag()
		{
			for (int i = 7; i <= 17; i++)
			{
				if (i <= 8)
				{
					yield return new TimeTableEntry(day, i, 0);
					yield return new TimeTableEntry(day,i, 20);
					yield return new TimeTableEntry(day,i, 40);
				}
				else if (i == 9)
				{
					yield return new TimeTableEntry(day,i, 0);
					yield return new TimeTableEntry(day, i, 20);
				}
				else if (i <= 16)
				{
					yield return new TimeTableEntry(day,i, 15);
					yield return new TimeTableEntry(day,i, 45);
				}
				else if (i == 17)
				{

					yield return new TimeTableEntry(day,i, 15);
				}
			}
		}

		IEnumerable<TimeTableEntry> Lordag()
		{
			for (int i = 9; i <= 16; i++)
			{
				if (i == 9)
				{
					yield return new TimeTableEntry(day,i, 30);
				}
				else
				{
					yield return new TimeTableEntry(day,i, 15);
					yield return new TimeTableEntry(day,i, 45);
				}
			}
		}

		IEnumerable<TimeTableEntry> Sondag()
		{
			for (int i = 11; i <= 15; i++)
			{
				if (i == 15)
				{
					yield return (new TimeTableEntry(day,i, 15));
				}
				else
				{
					yield return (new TimeTableEntry(day,i, 15));
					yield return (new TimeTableEntry(day,i, 45));
				}
			}
		}
	}

	public class TimeTableEntry : INotifyPropertyChanged
	{
		TimeSpan timeOfDay;
		DateTimeOffset _day;
		public TimeTableEntry(DateTimeOffset day, int hour, int min)
		{
			this._day = day;
			timeOfDay = TimeSpan.FromHours(hour).Add(TimeSpan.FromMinutes(min));
			Device.StartTimer(TimeSpan.FromSeconds(1), () =>
			{
                NotifyPropertyChanged(nameof(Now));
                NotifyPropertyChanged(nameof(HarForlattKirkelandet));
				NotifyPropertyChanged(nameof(HarForlattInnlandet));
                NotifyPropertyChanged(nameof(HarForlattNordlandet));
                NotifyPropertyChanged(nameof(HarForlattGomalandet));                
				NotifyPropertyChanged(nameof(HarReturnertKirkelandet));                
                NotifyPropertyChanged(nameof(Details));                
				NotifyPropertyChanged(nameof(NesteSted));                
				return true;
			});
		}
		public string NesteSted
		{
			get
			{
				if (!HarForlattKirkelandet)
					return $"{Kirklandet:HH:mm} Kirkelandet";
				if (!HarForlattInnlandet)
					return $"{Innlandet:HH:mm} Innlandet";
				if (!HarForlattNordlandet)
					return $"{Nordlandet:HH:mm} Nordlandet";
				if (!HarForlattGomalandet)
					return $"{Gomalandet:HH:mm} Gomalandet";
				return $"{ReturKirklandet:HH:mm} Retur Kirkelandet";
			}
		}

		public string Details
		{
			get
			{
				var b = new StringBuilder();
				if (!HarForlattKirkelandet)
					b.AppendFormat("Innlandet {0:HH:mm}, ", Innlandet);
				if (!HarForlattInnlandet)
					b.AppendFormat("Nordlandet {0:HH:mm}, ", Nordlandet);
				if (!HarForlattNordlandet)
					b.AppendFormat("Gomalandet {0:HH:mm}, ", Gomalandet);
				if (!HarForlattGomalandet)
					b.AppendFormat("Retur Kirkelandet {0:HH:mm}.", ReturKirklandet);

				return b.ToString();
			}
		}

		DateTimeOffset Now
		{
			get
			{
				return DateTimeOffset.UtcNow;
			}
		}

		public bool HarForlattKirkelandet
		{
			get
			{
				return Now > Kirklandet;
			}
		}


		public bool HarForlattInnlandet
		{
			get
			{
				return Now > Innlandet;
			}
		}

		public bool HarForlattNordlandet
		{
			get
			{
				return Now > Nordlandet;
			}
		}

		public bool HarForlattGomalandet
		{
			get
			{
				return Now > Gomalandet;
			}
		}

		public bool HarReturnertKirkelandet
		{
			get { return Now > ReturKirklandet; }
		}

		public DateTimeOffset Kirklandet
		{
			get { return _day.Add(timeOfDay); }
		}

		public DateTimeOffset Innlandet
		{
			get {
				return Kirklandet.Add(TimeSpan.FromMinutes(2));
			 }
		}

		public DateTimeOffset Nordlandet
		{
			get
			{
				return Kirklandet.Add(TimeSpan.FromMinutes(5));
			}
		}

		public DateTimeOffset Gomalandet
		{
			get
			{
				return Kirklandet.Add(TimeSpan.FromMinutes(10));
			}
		}

		public DateTimeOffset ReturKirklandet
		{
			get
			{
				return Kirklandet.Add(TimeSpan.FromMinutes(18));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string member = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(member));
		}
	}

}
