﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xamarin.Forms;

namespace FacilityReservationKiosk
{
	public partial class FacilityCancelPage : ContentPage
	{
		string userID;
		string facilityReservationID;
		string reason;
		string password;

		static string eight = "08:00:00";
		static string nine = "09:00:00";
		static string ten = "10:00:00";
		static string eleven = "11:00:00";
		static string twelve = "12:00:00";
		static string thirteen = "13:00:00";
		static string fourteen = "14:00:00";
		static string fifteen = "15:00:00";
		static string sixteen = "16:00:00";
		static string seventeen = "17:00:00";
		static string eighteen = "18:00:00";

		TimeSpan eighttime = Convert.ToDateTime(eight).TimeOfDay;
		TimeSpan ninetime = Convert.ToDateTime(nine).TimeOfDay;
		TimeSpan tentime = Convert.ToDateTime(ten).TimeOfDay;
		TimeSpan eleventime = Convert.ToDateTime(eleven).TimeOfDay;
		TimeSpan twelvetime = Convert.ToDateTime(twelve).TimeOfDay;
		TimeSpan thirteentime = Convert.ToDateTime(thirteen).TimeOfDay;
		TimeSpan fourteentime = Convert.ToDateTime(fourteen).TimeOfDay;
		TimeSpan fifteentime = Convert.ToDateTime(fifteen).TimeOfDay;
		TimeSpan sixteentime = Convert.ToDateTime(sixteen).TimeOfDay;
		TimeSpan seventeentime = Convert.ToDateTime(seventeen).TimeOfDay;
		TimeSpan eighteentime = Convert.ToDateTime(eighteen).TimeOfDay;

		class ResObject
		{
			public string facilityReservationID { get; set; }

			public string facilityID { get; set; }

			public DateTime startDateTime { get; set; }

			public DateTime endDateTime { get; set; }

			public string useShortDescription { get; set; }

			public string useDescription { get; set; }

			public ResObject (string resid, string facid, DateTime s, DateTime e, string sd, string d)
			{
				this.facilityReservationID = resid;
				this.facilityID = facid;
				this.startDateTime = s;
				this.endDateTime = e;
				this.useShortDescription = sd;
				this.useDescription = d; 
			}
		}

		public class Reservation
		{
			public string facilityReservationID { get; set; }

			public string facilityID { get; set; }

			public DateTime startDateTime { get; set; }

			public DateTime endDateTime { get; set; }

			public string useShortDescription { get; set; }

			public string useDescription { get; set; }
		}

		public class ReservationList
		{
			public List<Reservation> Reservations { get; set; }
		}

		public FacilityCancelPage (string passFacilityID, string reservationid, string datePass)
		{
			InitializeComponent ();

			string[] splitToken = passFacilityID.Split (new[] { "." }, StringSplitOptions.None);
			char[] charArray = splitToken [1].ToCharArray ();
			string departmentID = ConfigurationSettings.departmentID;
			string block = splitToken [0];
			string level = charArray [0].ToString ();
			string name = passFacilityID;
			//yyyy-MMM-dd
			//check with webservice to confirm format again
			string date = datePass;

			//Facility Reservation Kiosk Name
			appName.Text = "Facility Reservation Kiosk";
			appName.FontAttributes = FontAttributes.Bold;
			var tapL = new TapGestureRecognizer ();
			tapL.Tapped += (object sender, EventArgs e) => 
				Navigation.PushModalAsync(new FacilityListingPage());
			appName.GestureRecognizers.Add (tapL);

			//set the label
			//set based on filter***
			title.Text = " \n" + "Facility Reservation";
			title.FontAttributes = FontAttributes.Bold;

			//filter button to filter the list of facility
			arrowButton.Image = "Left Arrow-50.png";
			arrowButton.BackgroundColor = Color.Transparent;

			//return to FacilityListingPage
			arrowButton.Clicked += (object sender, EventArgs e) => 
				Navigation.PopModalAsync (true);

			//facility name
			facName.Text = passFacilityID;
			facName.YAlign = TextAlignment.Center;
			facName.FontSize = 34;
			facName.FontAttributes = FontAttributes.Bold;

			//reservation status
			reservation.Text = "Cancellation";
			reservation.YAlign = TextAlignment.Center;
			reservation.FontSize = 34;
			reservation.FontAttributes = FontAttributes.Bold;

			//set background colour
			box.BackgroundColor = Color.Silver;

			//a list to store reservation objects
			List<ResObject> reservationList = new List<ResObject> ();

			//url
			string urlRes = ConfigurationSettings.urliPad + "GetFacilityReservations.aspx?DepartmentID=" + departmentID
				+ "&Block=" + block + "&Level=" + level + "&Name=" + name + "&Date=" + date;

			urlRes = Security.SignHttpRequest (urlRes);

			//to get all the reservations and insert to an c# object
			using (var client2 = new HttpClient ()) {
				HttpResponseMessage responseMsg2 = client2.GetAsync (urlRes).Result;

				var json2 = responseMsg2.Content.ReadAsStringAsync ();
				json2.Wait ();
				ReservationList list2 = JsonConvert.DeserializeObject<ReservationList> (json2.Result);

				foreach (Reservation res in list2.Reservations) {
					ResObject resObject = new ResObject (res.facilityReservationID, res.facilityID, res.startDateTime, res.endDateTime,
						res.useShortDescription, res.useDescription);
					reservationList.Add (resObject);
				}
			}

			//add in column definitions

			//create new rows and column for the grid
			lineGrid.RowDefinitions = new RowDefinitionCollection ();
			lineGrid.ColumnDefinitions = new ColumnDefinitionCollection ();

			//create the columns
			//0,1,2
			lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (20, GridUnitType.Absolute) });
			lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (30, GridUnitType.Absolute) });
			lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (30, GridUnitType.Absolute) });

			//column with interval of 5 minutes
			lineGrid.ColumnSpacing = 0;

			for (int k = 0; k < 10; k++) { 
				//00
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				//15
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				//30
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				//45
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });

			} 

			//6pm (243)
			lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });

			//spacing of 10 (244,245)
			lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (30, GridUnitType.Absolute) });
			lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (30, GridUnitType.Absolute) });
			lineGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (10, GridUnitType.Absolute) });

			//rows for lineGrid
			lineGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (1, GridUnitType.Absolute) });
			lineGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });
			lineGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (1, GridUnitType.Absolute) });

			//drawing horizontal lines
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 2, 245, 0, 1);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 2, 245, 2, 3);

			//create new rows and column for the grid
			resGrid.RowDefinitions = new RowDefinitionCollection ();
			resGrid.ColumnDefinitions = new ColumnDefinitionCollection ();

			resGrid.ColumnSpacing = 0;
			resGrid.RowSpacing = 0;

			//create the columns
			//0,1,2
			resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (20, GridUnitType.Absolute) });
			resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (30, GridUnitType.Absolute) });
			resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (30, GridUnitType.Absolute) });

			//column with interval of 5 minutes
			resGrid.ColumnSpacing = 0;
			resGrid.RowSpacing = 0;

			for (int k = 0; k < 10; k++) { 
				//00
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				//15
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				//30
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				//45
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });

			} 

			//6pm (243)
			resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });

			//spacing of 10 (244,245)
			resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (30, GridUnitType.Absolute) });
			resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (30, GridUnitType.Absolute) });
			resGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (10, GridUnitType.Absolute) });

			//add row
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (19, GridUnitType.Absolute) });

			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (50, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (15, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (15, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (15, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (15, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (15, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (60, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });
			resGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (60, GridUnitType.Absolute) });

			//boxview background
			resGrid.Children.Add (new BoxView { BackgroundColor = Color.Silver }, 2, 245, 1, 17);

			for (int j = 0; j < reservationList.Count; j++) {

				if (reservationList [j].facilityID == passFacilityID) {
					//booking reserved
					string text;
					if (reservationList [j].useShortDescription == null) {
						string temptext = reservationList [j].useDescription;
						string str = temptext;
						if (temptext.Length > 6)
							temptext.Substring (0, 6);
						text = str;
					} else {
						text = reservationList [j].useShortDescription;
					}

					// add in reservations
					//startdatetime 2015-08-13 08:30:00.000
					//DateTime startdate = DateTime.ParseExact(reservationList[j].startDateTime, "dd-mm-yyyy HH:MM:SS"); 
					string sdateRes = reservationList [j].startDateTime.ToString ("dd-MM-yyyy HH:mm:ss");
					string[] stoken = sdateRes.Split (new[] { " " }, StringSplitOptions.None);
					//time
					string[] stiming = stoken [1].Split (new[] { ":" }, StringSplitOptions.None);
					//timing[0] = 08 (hour) //timing[1] = 30 (mins)
					//string[] sonehour = stiming [0].Split (new[] { "" }, StringSplitOptions.None);
					//onehour[1] = 8

					int shour = Convert.ToInt16 (stiming [0]);
					int smin = Convert.ToInt16 (stiming [1]);

					//enddatetime
					//DateTime enddate = DateTime.ParseExact(reservationList[j].endDateTime, "dd-mm-yyyy HH:MM:SS");
					string edateRes = reservationList [j].endDateTime.ToString ("dd-MM-yyyy HH:mm:ss");
					string[] etoken = edateRes.Split (new[] { " " }, StringSplitOptions.None);
					//time
					string[] etiming = etoken [1].Split (new[] { ":" }, StringSplitOptions.None);
					//timing[0] = 08 (hour) //timing[1] = 30 (mins)
					//string[] eonehour = etiming [0].Split (new[] { "" }, StringSplitOptions.None);
					//onehour[1] = 8

					int ehour = Convert.ToInt16 (etiming [0]);
					int emin = Convert.ToInt16 (etiming [1]);

					int start, end;

					if (smin == 0) {
						start = (((shour - 8) * 24) + 3);
					} else {
						start = (((shour - 8) * 24) + 3) + ((smin / 5) * 2);
					}

					if (emin == 0) {
						end = (((ehour - 8) * 24) + 3);
					} else {
						end = (((ehour - 8) * 24) + 3) + ((emin / 5) * 2);
					}

					BoxView box = new BoxView { BackgroundColor = Color.Silver };
					//start, end , top, bottom
					//lineGrid.Children.Add (box, 3, 243, 1 , 2);
					lineGrid.Children.Add (box, start, end, 1, 2);
					//facGrid.Children.Add (facBut, start, end, (i*2) + 1, (i*2)+2);
					lineGrid.Children.Add (new Label {
						Text = text,
						TextColor = Color.Black,
						FontSize = 11,
						XAlign = TextAlignment.Center,
						YAlign = TextAlignment.Center
					}, start, end, 1, 2);

					//if else or switch to determine the start and end time
					//if 8-9 then...
					//from time
					if (reservationList[j].facilityReservationID == reservationid) {

						resGrid.Children.Add (new Label {
							Text = reservationList[j].startDateTime.ToString("dd-MMM-yyyy")
						}, 75, 123, 3, 4);
						
						Label timepick = new Label {
							Text = reservationList[j].startDateTime.ToString("H.mm tt"),
							YAlign = TextAlignment.Center
						};
						resGrid.Children.Add (timepick, 75, 112, 5, 6);

						//to time
						resGrid.Children.Add (new Label {
							Text = reservationList[j].endDateTime.ToString("H.mm tt"),
							YAlign = TextAlignment.Center
						}, 173, 210, 5, 6);

						facilityReservationID = reservationList [j].facilityReservationID;

						if ((reservationList [j].startDateTime.TimeOfDay >= eighttime
						   && reservationList [j].startDateTime.TimeOfDay < ninetime)) {
							resGrid.Children.Add (new Image { Source = "arrowg.png" }, 3, 27, 0, 1);
						}

						if ((reservationList [j].startDateTime.TimeOfDay >= ninetime
							&& reservationList [j].startDateTime.TimeOfDay < tentime)) {
							resGrid.Children.Add (new Image { Source = "arrowg.png" }, 27, 51, 0, 1);
						}

						if ((reservationList [j].startDateTime.TimeOfDay >= tentime
							&& reservationList [j].startDateTime.TimeOfDay < eleventime)) {
							resGrid.Children.Add (new Image { Source = "arrowg.png" }, 51, 75, 0, 1);
						}

						if ((reservationList [j].startDateTime.TimeOfDay >= eleventime
							&& reservationList [j].startDateTime.TimeOfDay < twelvetime)) {
							resGrid.Children.Add (new Image { Source = "arrowg.png" }, 75, 99, 0, 1);
						}

						if ((reservationList [j].startDateTime.TimeOfDay >= twelvetime
							&& reservationList [j].startDateTime.TimeOfDay < thirteentime)) {
							resGrid.Children.Add (new Image { Source = "arrowg.png" }, 99, 123, 0, 1);
						}

						if ((reservationList [j].startDateTime.TimeOfDay >= thirteentime
							&& reservationList [j].startDateTime.TimeOfDay < fourteentime)) {
							resGrid.Children.Add (new Image { Source = "arrowg.png" }, 123, 147, 0, 1);
						}

						if ((reservationList [j].startDateTime.TimeOfDay >= fourteentime
							&& reservationList [j].startDateTime.TimeOfDay < fifteentime)) {
							resGrid.Children.Add (new Image { Source = "arrowg.png" }, 147, 171, 0, 1);
						}

						if ((reservationList [j].startDateTime.TimeOfDay >= fifteentime
							&& reservationList [j].startDateTime.TimeOfDay < sixteentime)) {
							resGrid.Children.Add (new Image { Source = "arrowg.png" }, 171, 195, 0, 1);
						}

						if ((reservationList [j].startDateTime.TimeOfDay >= sixteentime
							&& reservationList [j].startDateTime.TimeOfDay < seventeentime)) {
							resGrid.Children.Add (new Image { Source = "arrowg.png" }, 195, 219, 0, 1);
						}

						if ((reservationList [j].startDateTime.TimeOfDay >= seventeentime
							&& reservationList [j].startDateTime.TimeOfDay < eighteentime)) {
							resGrid.Children.Add (new Image { Source = "arrowg.png" }, 219, 243, 0, 1);
						}
					}

				}	
			}

			//date now
			string dateNowLine = DateTime.Now.ToString ("dd-MM-yyyy HH:mm:ss");
			string dateTiming = DateTime.Now.ToString ("hh.mm tt");
			string[] linetoken = dateNowLine.Split (new[] { " " }, StringSplitOptions.None);
			//time
			string[] linetiming = linetoken [1].Split (new[] { ":" }, StringSplitOptions.None);
			//timing[0] = 08 (hour) //timing[1] = 30 (mins)

			int linehour = Convert.ToInt16 (linetiming [0]);
			int linemin = Convert.ToInt16 (linetiming [1]);

			int boxhour = Convert.ToInt16 (linetiming [0]);
			int boxmin = Convert.ToInt16 (linetiming [1]);

			int linestart, boxstart;

			if (linemin == 0) {
				linestart = (((linehour - 8) * 24) + 3);
			} else {
				linestart = (((linehour - 8) * 24) + 3) + ((linemin / 5) * 2);
			}

			if (boxmin == 0) {
				boxstart = (((boxhour - 8) * 24) + 15);
			} else {
				boxstart = (((boxhour - 8) * 24) + 15) + ((boxmin / 5) * 2);
			}

			//draw vertical lines
			//8am,9am,10am
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 3, 4, 0, 3);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 27, 28, 0, 3);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 51, 52, 0, 3);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 75, 76, 0, 3);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 99, 100, 0, 3);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 123, 124, 0, 3);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 147, 148, 0, 3);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 171, 172, 0, 3);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 195, 196, 0, 3);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 219, 220, 0, 3);
			lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 243, 244, 0, 3);

			if (date == DateTime.Now.ToString ("yyyy-MMM-dd") || date == "") {
				//draw red line
				lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Red }, linestart, linestart + 1, 0, 3);
			}

			//box view grid
			//create new rows and column for the grid
			boxGrid.RowDefinitions = new RowDefinitionCollection ();
			boxGrid.ColumnDefinitions = new ColumnDefinitionCollection ();

			//create the columns
			//0,1,2
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (50, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });

			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (3, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (3, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });

			//column with interval of 5 minutes
			boxGrid.ColumnSpacing = 0;
			boxGrid.RowSpacing = 0;

			for (int k = 0; k < 10; k++) { 
				//00
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				//15
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				//30
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				//45
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
				boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });

			} 

			//6pm (243)
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
			//spacing of 10 (244,245)
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (10, GridUnitType.Absolute) });

			boxGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (15, GridUnitType.Absolute) });
			if (date == DateTime.Now.ToString ("yyyy-MMM-dd") || date == "") {
				boxGrid.Children.Add (new BoxView { BackgroundColor = Color.Red }, boxstart - 9, boxstart + 10, 0, 1);
				boxGrid.Children.Add (new Label {
					Text = dateTiming,
					TextColor = Color.Black,
					FontSize = 9.5,
					XAlign = TextAlignment.Center,
					YAlign = TextAlignment.Center
				}, boxstart - 9, boxstart + 10, 0, 1);
			}

			// add in label and textboxes to make new reservation
			resGrid.Children.Add (new Label {
				TextColor = Color.Black,
				Text = "Cancel Reservation",
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				FontAttributes = FontAttributes.Bold,
				FontSize = 20
			}, 75, 171, 1, 2);

			resGrid.Children.Add (new Label {
				TextColor = Color.Black,
				Text = "On",
				YAlign = TextAlignment.Center,
				FontAttributes = FontAttributes.Bold,
				FontSize = 20
			}, 27, 75, 3, 4);

			//Date label in for loop

			//from time label
			resGrid.Children.Add (new Label {
				TextColor = Color.Black,
				Text = "From",
				YAlign = TextAlignment.Center,
				FontAttributes = FontAttributes.Bold,
				FontSize = 20
			}, 27, 75, 5, 6);
					

			//to time label
			resGrid.Children.Add (new Label {
				TextColor = Color.Black,
				Text = "To",
				YAlign = TextAlignment.Center,
				FontAttributes = FontAttributes.Bold,
				FontSize = 20
			}, 147, 171, 5, 6);
					
			//reason
			resGrid.Children.Add (new Label {
				TextColor = Color.Black,
				Text = "Reason",
				YAlign = TextAlignment.Center,
				FontAttributes = FontAttributes.Bold,
				FontSize = 20
			}, 27, 75, 7, 8);

			//entry input of reason
			Entry cellInput = new Entry ();
			resGrid.Children.Add (cellInput, 75, 205, 7, 8);

			//usertype
			resGrid.Children.Add (new Label {
				TextColor = Color.Black,
				Text = "User Type",
				YAlign = TextAlignment.Center,
				FontAttributes = FontAttributes.Bold,
				FontSize = 20
			}, 27, 75, 9, 10);

			Picker usertypeInput = new Picker {SelectedIndex = 1};
			resGrid.Children.Add (usertypeInput, 75, 115, 9, 10);
			usertypeInput.Items.Add ("Staff");
			usertypeInput.Items.Add ("Students");

			//userid
			resGrid.Children.Add (new Label {
				TextColor = Color.Black,
				Text = "User ID",
				YAlign = TextAlignment.Center,
				FontAttributes = FontAttributes.Bold,
				FontSize = 20
			}, 27, 75, 11, 12);

			//entry input of userid
			Entry inputID = new Entry ();
			resGrid.Children.Add (inputID, 75, 123, 11, 12);

			//password
			resGrid.Children.Add (new Label {
				TextColor = Color.Black,
				Text = "Password",
				YAlign = TextAlignment.Center,
				FontAttributes = FontAttributes.Bold,
				FontSize = 20
			}, 27, 75, 13, 14);

			//entry input of password
			Entry inputPass = new Entry ();
			resGrid.Children.Add (inputPass, 75, 135, 13, 14);
			inputPass.IsPassword =  true;

			//button
			Button bookBut = new Button { Text="Confirm", BackgroundColor = Color.White };
			resGrid.Children.Add (bookBut, 147, 192, 15, 16);

			bookBut.Clicked += (object sender, EventArgs e) => {

				userID = inputID.Text;
				password = inputPass.Text;
				reason = cellInput.Text;

				//check user
				if (userID == "133332D" && password == "12345") {
					string status;
					string message;
					//url
					//facilityReservationID
					string urlCancel = ConfigurationSettings.urliPad + "CancelReservation.aspx?UserID=" + userID
						+ "&FacilityReservationID=" + facilityReservationID + "&Reason=" + reason;

					//to get all the reservations and insert to an c# object
					using (var client3 = new HttpClient ()) {
						HttpResponseMessage responseMsg3 = client3.GetAsync (urlCancel).Result;

						var json3 = responseMsg3.Content.ReadAsStringAsync ();
						json3.Wait ();
						string jsonString = json3.Result.ToString ();
						//ReservationList list2 = JsonConvert.DeserializeObject<ReservationList> (json3.Result);
						var obj = JObject.Parse (jsonString);
						status = (string)obj.SelectToken ("Result");
						message = (string)obj.SelectToken ("Message");
					}

					if (status == "OK") {
						ShowAlert ("Reservation", "Cancellation was sucessful!", "OK");
					} else {
						DisplayAlert ("Error", message, "OK");
					}
				}
			};

			Button cancelBut = new Button { Text="Cancel", BackgroundColor = Color.White 	};
			resGrid.Children.Add (cancelBut, 195, 240, 15, 16);

			cancelBut.Clicked += CancelBut_Clicked;
		}

		void CancelBut_Clicked (object sender, EventArgs e)
		{
			Navigation.PopModalAsync (true);
		}

		public async void ShowAlert(string title, string message, string button)
		{
			await DisplayAlert (title, message, button);
			await Navigation.PushModalAsync(new FacilityListingPage());
		}
	}
}

