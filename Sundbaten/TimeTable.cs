using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;

namespace Sundbaten
{
    public class TimeTable : INotifyPropertyChanged
    {
        public TimeTable()
        {
            BaseTimeTable = new TimeTableCollection();
            LoadAsync();
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                NotifyPropertyChanged(nameof(NesteAvgang));
                NotifyPropertyChanged(nameof(VisNesteAvgang));
                NotifyPropertyChanged(nameof(NesteAvgangStr));
                return true;
            });
        }

        bool loading;

        public bool Loading
        {
            get
            {
                return loading;
            }

            set
            {
                loading = value;
                NotifyPropertyChanged();
            }
        }

        public async Task LoadAsync()
        {
            Loading = true;
            await BaseTimeTable.LoadAsync();
            Loading = false;
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
                if (NesteAvgang.Value.Hours == 0)
                {
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
                return BaseTimeTable.OrderBy(x => x.Kirklandet).FirstOrDefault(x => !x.HarForlattKirkelandet)?.Kirklandet - DateTimeOffset.UtcNow;
            }
        }

        public TimeTableCollection BaseTimeTable
        {
            get;
            set;
        }
    }

}
