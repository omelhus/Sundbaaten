using Xamarin.Forms;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

namespace Sundbaten
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new SundbatenPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
			MobileCenter.Start("ios=7d2ce875-70b8-487c-9ab5-84fea4679c3b;" + 
                   "android=6ddcd40e-d116-4aff-8706-69bb2b08aed4",
                   typeof(Analytics), typeof(Crashes));
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
