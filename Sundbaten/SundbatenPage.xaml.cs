using System;
using Xamarin.Forms;
using Xamarinos.AdMob.Forms;

namespace Sundbaten
{
	public partial class SundbatenPage : ContentPage
	{
		void Handle_Clicked(object sender, System.EventArgs e)
		{
			Device.OpenUri(new Uri("https://on-it.no/sundbaten"));
		}

		public SundbatenPage()
		{
			InitializeComponent();
		}
	}
}
