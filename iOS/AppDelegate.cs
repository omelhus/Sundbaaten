using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarinos.AdMob.Forms;
using Xamarinos.AdMob.Forms.iOS;

namespace Sundbaten.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
			LoadApplication(new App());
			AdBannerRenderer.Init ();

			//Initialize Interstitial Manager with a Specific AdMob Key
			CrossAdmobManager.Init ("ca-app-pub-9559020873366734~1109314808");
			return base.FinishedLaunching(app, options);
		}
	}
}
