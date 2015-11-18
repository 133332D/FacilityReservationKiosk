using System;

namespace FacilityReservationKiosk
{
	public interface IHardware
	{
		string DeviceId {
			get;
		}

		string PublicKey {
			get;
		}

		string PrivateKey {
			get;
		}

		void LoadOrGenerateKeys (string publicKeyFileName, string privateKeyFileName);


	}
}

