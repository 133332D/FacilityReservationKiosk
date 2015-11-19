using System;

namespace FacilityReservationKiosk
{
	public class ConfigurationSettings
	{
		public static IHardware Hardware = null;

		public static string urliPad = @"http://crowd.sit.nyp.edu.sg/FRSIPad/";
		public static string urladmin = @"http://crowd.sit.nyp.edu.sg/FRSAdmin/";

		//department (SIT)
		public static string departmentID = "SIT";

		public ConfigurationSettings ()
		{
			//
		}
	}
}

