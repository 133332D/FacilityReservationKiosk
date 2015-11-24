using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xamarin.Forms;

namespace FacilityReservationKiosk
{
	public partial class VerifyingPage : ContentPage
	{
		public VerifyingPage(){
			InitializeComponent ();
			string message, status;
		//Web Service 
		//url & data to be pass to be saved in the database 
		string urlVerify = ConfigurationSettings.urliPad + "VerifyIpad.aspx?";
			urlVerify = Security.SignHttpRequest (urlVerify);
			using (var client2 = new HttpClient ()) {
				HttpResponseMessage responseMsg2 = client2.GetAsync (urlVerify).Result;

				var json2 = responseMsg2.Content.ReadAsStringAsync ();
				json2.Wait ();

				string jsonString = json2.Result.ToString ();
				var obj = JObject.Parse (jsonString);
				status = (string)obj.SelectToken ("Result");
				message = (string)obj.SelectToken ("Message");


				test.Text = status + " " + message;

				DisplayAlert ("Verify IPad", message, "OK");
			}
		}
	}
}

