﻿using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace FacilityReservationKiosk.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			// Code for starting up the Xamarin Test Cloud Agent
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
			#endif


			ConfigurationSettings.Hardware = new Hardware();
			LoadApplication (new App ());

			//FacilityReservationKiosk.App.Hardware = new Hardware ();
			return base.FinishedLaunching (app, options);
		}
	}
}

