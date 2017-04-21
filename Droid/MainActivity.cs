using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarinos.AdMob.Forms.Android;
using Xamarinos.AdMob.Forms;

namespace Sundbaten.Droid
{
	[Activity(Label = "Sundbaten.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			AdBannerRenderer.Init ();

			//Initialize Interstitial Manager with a Specific AdMob Key
			CrossAdmobManager.Init ("ca-app-pub-9559020873366734~1109314808");
			LoadApplication(new App());
		}
	}
}
