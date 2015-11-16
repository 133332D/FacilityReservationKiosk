using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FacilityReservationKiosk
{
	public class RestService : IRestService
	{
		public async Task<string> GetFacilities ()
		{
			try
			{
				var client = new HttpClient();
				client.BaseAddress = new Uri("http:

			}
			catch(Exception ex){}
		}
	}
}

