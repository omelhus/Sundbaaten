using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace Sundbaten
{
	public partial class SundbatenPage : ContentPage
	{
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var entry = (TimeTableEntry)e.SelectedItem;
                ((ListView)sender).SelectedItem = null;
                Analytics.TrackEvent("ClickedItem", new Dictionary<string, string> {
                    {"Item", entry.NesteSted}
                });
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(entry.NesteSted, entry.Details.Replace(",", Environment.NewLine), "Ferdig"));
            }
        }

		async void Handle_Clicked(object sender, System.EventArgs e)
		{
            Analytics.TrackEvent("AdClicked", new Dictionary<string, string>
            {
                {"Title", Ad.Title},
                {"Message", Ad.Message},
                {"Id", Ad.Id}
            });
            if (Ad?.Link?.StartsWith("http") == true){
                 Device.OpenUri(new Uri(Ad.Link));
            } else if(!string.IsNullOrEmpty(Ad?.Message)) {
                await DisplayAlert(Ad.Title, Ad.Message, "Ferdig");
            }
          
		}

		public SundbatenPage()
		{
			InitializeComponent();
		}

        public Ad Ad { get; set; }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await ((TimeTable)MainStackLayout.BindingContext).LoadAsync();
                Analytics.TrackEvent("Appearing");
                Ad = new Ad();
                AdButton.BindingContext = Ad;
                await Ad.LoadAsync();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                await DisplayAlert("Noe gikk galt", "Vi klarer dessverre ikke å vise deg rutetidene akkurat nå.", "Ok");
            }
        }
    }

    public class Ad : INotifyPropertyChanged
    {
        List<Ad> _ads;
        int _index;

        string title;

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
                OnPropertyChanged();
            }
        }   

        public string Link
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }
        public string Message { get; set; }

        public void Set(Ad ad){
            Link = ad.Link;
            Id = ad.Id;
            Title = ad.Title;
            Message = ad.Message;
        }

        public async Task LoadAsync(){
            try
            {
                _ads = await TimeTableClient.GetAdsTask();
            } catch(Exception e){
                Crashes.TrackError(e);
                _ads = new List<Ad> {
                    new Ad {
                        Title = "Appen er utviklet av ON IT AS",
                        Link = "https://on-it.no/sundbaten",
                        Id = "UNABLETOLOAD"
                    }
                };
            }
            if(_ads != null){
                Set(_ads.FirstOrDefault());
                Device.StartTimer(TimeSpan.FromSeconds(15), () =>
                {
                    _index++;
                    if (_index >= _ads.Count)
                        _index = 0;
                    Set(_ads[_index]);
                    return true;
                });
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string property = null)
        {
            var e = new PropertyChangedEventArgs(property);
            PropertyChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}