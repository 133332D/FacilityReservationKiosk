﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace FacilityReservationKiosk
{
	public partial class DisplayPage : ContentPage
	{
		public DisplayPage ()
		{
			InitializeComponent ();

			UniqueID.Text = "Device Unique ID: " + ConfigurationSettings.Hardware.DeviceId;
			string errorMessage = ConfigurationSettings.Hardware.LoadOrGenerateKeys("publickey.Txt", "privatekey.Txt");
			if(errorMessage != null)
				DisplayAlert("Error", errorMessage, "OK");

			GeneratedKey.Text = "Loaded Or Generated Keys: ";
			PublicKey.Text = "Public Key: " + ConfigurationSettings.Hardware.PublicKey;
			PrivateKey.Text = "Private Key: " + ConfigurationSettings.Hardware.PrivateKey;

//			UniqueID.Text = "Device Unique ID: " + FacilityReservationKiosk.App.Hardware.DeviceId;
//			FacilityReservationKiosk.App.Hardware.LoadOrGenerateKeys("publickey.Txt", "privatekey.Txt");
//			GeneratedKey.Text = "Loaded Or Generated Keys: ";
//			PublicKey.Text = "Public Key: " + FacilityReservationKiosk.App.Hardware.PublicKey;
//			PrivateKey.Text = "Private Key: " + FacilityReservationKiosk.App.Hardware.PrivateKey;
		}
	}
}

