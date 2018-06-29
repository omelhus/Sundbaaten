using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Text;

namespace Sundbaten
{
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
                    return $"{Kirklandet:HH:mm} Kirklandet";
                if (!HarForlattInnlandet)
                    return $"{Innlandet:HH:mm} Innlandet";
                if (!HarForlattNordlandet)
                    return $"{Nordlandet:HH:mm} Nordlandet";
                if (!HarForlattGomalandet)
                    return $"{Gomalandet:HH:mm} Goma";
                return $"{ReturKirklandet:HH:mm} Retur Kirklandet";
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
                    b.AppendFormat("Goma {0:HH:mm}, ", Gomalandet);
                if (!HarForlattGomalandet)
                    b.AppendFormat("Retur Kirklandet {0:HH:mm}.", ReturKirklandet);

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
            get
            {
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
