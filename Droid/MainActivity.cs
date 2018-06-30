using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Sundbaten.Droid
{
    [Activity(Label = "Sundbaten.Droid", Icon = "@drawable/icon", Theme = "@style/Sundbaten", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
            
			global::Xamarin.Forms.Forms.Init(this, bundle);
			LoadApplication(new App());
            base.OnCreate(bundle);
		}
	}
}
