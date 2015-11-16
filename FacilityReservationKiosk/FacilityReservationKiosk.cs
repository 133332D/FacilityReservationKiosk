using System;

using Xamarin.Forms;

namespace FacilityReservationKiosk
{
	public class App : Application
	{
		public static FacilityListingPage facilityListingPage = new FacilityListingPage();

		public App ()
		{
			// The root page of your application
			//call to determine which page should run as main
			//MainPage = new NavigationPage(new FacilityDetailsPage());
			MainPage = new FacilityListingPage();
			//MainPage = new FacilityDetailsPage();
			//MainPage = new FacilityReservationPage();
			//MainPage = new FacilityCancelPage("L.424");
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

