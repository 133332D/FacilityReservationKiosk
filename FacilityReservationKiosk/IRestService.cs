using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FacilityReservationKiosk
{
	public interface IRestService
	{
		Task<List<Facility>> GetFacilities ();
	}
}

