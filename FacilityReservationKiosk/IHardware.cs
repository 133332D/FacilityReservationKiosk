using System;
using System.Threading;

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

		string LoadOrGenerateKeys (string publicKeyFileName, string privateKeyFileName);

		string SignString (string s);
	}
}

