using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;

namespace Sundbaten
{
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

        public async Task LoadAsync()
        {
            await ResetTimeTable();
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
         
        public string TimeTableSavePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tidtabell.json");
        string propertyKey = "timetable";
        public TimeTableResponse PropLoadTimeTable()
        {
            if(Application.Current.Properties.ContainsKey(propertyKey)){
                try
                {
                    Analytics.TrackEvent("LoadFromCache");
                    return JsonConvert.DeserializeObject<TimeTableResponse>((string)Application.Current.Properties[propertyKey]);
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                }
            }
            return null;
        }

        public void PropStoreTimeTable(TimeTableResponse tableResponse)
        {
            try
            {
                Application.Current.Properties[propertyKey] = JsonConvert.SerializeObject(tableResponse);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
       
        async Task ResetTimeTable()
        {
            var table = PropLoadTimeTable();  
            if (table == null || table.LastUpdate < DateTime.UtcNow.AddDays(-1)){
                Analytics.TrackEvent("UpdateTimeTable");
                table = await TimeTableClient.GetTask();
                table.LastUpdate = DateTime.UtcNow;
                PropStoreTimeTable(table);
                if(table == null)
                    throw new ArgumentNullException("table", "Timetable is missing");
            }
                
            IEnumerable<TimeTableEntry> entries = null;
            switch (day.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    if (ErSesong)
                        entries = CreateEntries(table.sondag);
                    break;
                case DayOfWeek.Saturday:
                    entries = CreateEntries(table.lordag);
                    break;
                default:
                    entries = CreateEntries(table.hverdag);
                    break;
            }
            if (entries == null) return;
            Clear();
            foreach (var item in entries)
                if (!item.HarReturnertKirkelandet)
                    Add(item);
        }

        public string Title
        {
            get
            {
                var days = new[] { "Man", "Tirs", "Ons", "Tors", "Fre", "Lør", "Søn" };
                return $"{days[(int)day.DayOfWeek - 1]}dag";
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

        IEnumerable<TimeTableEntry> CreateEntries(TimeTableResponseEntry tte)
        {
            if (tte == null)
                throw new ArgumentNullException(nameof(tte), "Mangler tidtabell i CreateEntries");
            if (tte.kirklandet == null) yield break;
            foreach (var time in tte.kirklandet)
            {
                if (time == null || time.IndexOf(':') == -1) continue;
                var s = time.Split(':');
                if (int.TryParse(s[0], out var h) && int.TryParse(s[1], out var m))
                    yield return new TimeTableEntry(day, h, m);
            }
        }
    }

}
