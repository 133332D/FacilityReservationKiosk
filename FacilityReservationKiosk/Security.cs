using System;

namespace FacilityReservationKiosk
{
	public class Security
	{
		public Security ()
		{
		}


		public static string SignHttpRequest(string url)
		{
			string urlWithDeviceID = url + "&_DeviceID=" + ConfigurationSettings.Hardware.DeviceId + "&_DT=" + DateTime.Now.ToString("yyyyMMddHHmmss");

			string signature = ConfigurationSettings.Hardware.SignString (urlWithDeviceID);	

			return urlWithDeviceID + "&_SIGN=" + signature;

		}
	}
}

