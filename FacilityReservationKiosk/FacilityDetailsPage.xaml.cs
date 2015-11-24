using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Globalization;

using Xamarin.Forms;

namespace FacilityReservationKiosk
{
	public partial class FacilityDetailsPage : ContentPage
	{

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

		public void UpdateCamera(string passFacilityID)
		{
			string urlRes = ConfigurationSettings.urliPad + "GetCameraID.aspx?FacilityID=" + passFacilityID;

			using (var client2 = new HttpClient ()) {
				HttpResponseMessage responseMsg2 = client2.GetAsync (urlRes).Result;

				var json2 = responseMsg2.Content.ReadAsStringAsync ();
				json2.Wait ();

				string[] CameraIDstring = json2.Result.Split (',');

				DensityLabel.Text = CameraIDstring [CameraIDstring.Length - 1].Replace ("d", "") + "%";
				CameraImages.Children.Clear();

				int cameraCount = 0;
				for (var i = 0; i < CameraIDstring.Length - 1; i++) {
					if (CameraIDstring [i] != "") {
						string Imageurl = ConfigurationSettings.urliPad + "images/image-" + CameraIDstring [i] + ".jpg?r=" + DateTime.Now.ToString ("yyyyMMddhhmmss");

						var image = new Image {
							Source = ImageSource.FromUri (new Uri (Imageurl))
						};
						CameraImages.Children.Add (image);
						cameraCount++;
					}
				}
				if (cameraCount == 0) {
					CameraImagesContainer.IsVisible = false;
					DensityContainer.IsVisible = false;
				}

			}
		}

		public FacilityDetailsPage (string passFacilityID, string datePass)
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

			//set the label
			//set based on filter***
			title.Text = " \n" + "Facility Details";
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
			reserve.Text = "Available";
			reserve.YAlign = TextAlignment.Center;
			reserve.FontSize = 34;
			reserve.FontAttributes = FontAttributes.Bold;

			//set box colour based on reserved or no
			//box.BackgroundColor = Color.Red;
			box.BackgroundColor = Color.Lime;

			//a list to store reservation objects
			List<ResObject> reservationList = new List<ResObject> ();

			//url
			string urlRes = ConfigurationSettings.urliPad + "GetFacilityReservations.aspx?DepartmentID=" + departmentID
				+ "&Block=" + block + "&Level=" + level + "&Name=" + name + "&Date=" + date;
			
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


			UpdateCamera (passFacilityID);
			Xamarin.Forms.Device.StartTimer (TimeSpan.FromSeconds(20), () => {

				UpdateCamera(passFacilityID);


				return true;

			});



			timeline.RowDefinitions.Add (new RowDefinition { Height = new GridLength (10, GridUnitType.Absolute) });
			timeline.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });

			Label labelNow;
			labelNow = new Label { TextColor = Color.Black };
			timeline.Children.Add (labelNow, 1, 1);
			labelNow.FontAttributes = FontAttributes.Bold;
			//labelNow.Text = "";

			//check if display label as today or other date
			if (date == DateTime.Now.ToString ("yyyy-MMM-dd") || date == "") {
				labelNow.Text = "Today";
			} 
			else {
				string [] arrayDate = date.Split (new[] { "-" }, StringSplitOptions.None);
				labelNow.Text = arrayDate [2] + "-" + arrayDate [1] + "-" + arrayDate [0];
			}

			TimeSpan dateNowHourStatus;
			TimeSpan dateStartStatus;
			TimeSpan dateEndStatus;

			//loop the reservations
			for (int i = 0; i < reservationList.Count; i++) {
				timeline.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });

				string startTime, endTime;
				string start = reservationList [i].startDateTime.ToString ("dd-MM-yyyy HH:mm:ss");
				string end = reservationList [i].endDateTime.ToString ("dd-MM-yyyy HH:mm:ss");
				string dateNow = DateTime.Now.ToString ("dd-MM-yyyy HH:mm:ss");

				if (start == dateNow) {
					startTime = "Now";
				} else {
					//time
					string[] stoken = start.Split (new[] { " " }, StringSplitOptions.None);
					//timing[0] = 08 (hour) //timing[1] = 30 (mins)
					string[] stiming = stoken [1].Split (new[] { ":" }, StringSplitOptions.None);
					startTime = stiming [0] + "." + stiming [1];
				}

				//time (end)
				string[] etoken = end.Split (new[] { " " }, StringSplitOptions.None);
				//timing[0] = 08 (hour) //timing[1] = 30 (mins)
				string[] etiming = etoken [1].Split (new[] { ":" }, StringSplitOptions.None);
				endTime = etiming [0] + "." + etiming [1];

				Label labelTime;
				labelTime = new Label { TextColor = Color.Black };
				timeline.Children.Add (labelTime, 1, i + 2);
				labelTime.Text = startTime + " to " + endTime;
				labelTime.FontAttributes = FontAttributes.Bold;

				Label desc;
				desc = new Label { TextColor = Color.Black };
				timeline.Children.Add (desc, 2, i + 2);
				desc.Text = reservationList[i].useShortDescription + " " + reservationList [i].useDescription;
				desc.FontAttributes = FontAttributes.Bold;
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

			//add image boxes
			//tap gesture for pop up
			Image eightnine = new Image { BackgroundColor = Color.Transparent };
			Image nineten = new Image { BackgroundColor = Color.Transparent };
			Image teneleven = new Image { BackgroundColor = Color.Transparent };
			Image eleventwelve = new Image { BackgroundColor = Color.Transparent };
			Image twelve13 = new Image { BackgroundColor = Color.Transparent };
			Image thirteen14 = new Image { BackgroundColor = Color.Transparent };
			Image fourteen15 = new Image { BackgroundColor = Color.Transparent };
			Image fifteen16 = new Image { BackgroundColor = Color.Transparent };
			Image sixteen17 = new Image { BackgroundColor = Color.Transparent };
			Image seventeen18 = new Image { BackgroundColor = Color.Transparent };

			lineGrid.Children.Add (eightnine, 3, 27, 0, 3);
			lineGrid.Children.Add (nineten, 27, 51, 0, 3);
			lineGrid.Children.Add (teneleven, 51, 75, 0, 3);
			lineGrid.Children.Add (eleventwelve, 75, 99, 0, 3);
			lineGrid.Children.Add (twelve13, 99, 123, 0, 3);
			lineGrid.Children.Add (thirteen14, 123, 147, 0, 3);
			lineGrid.Children.Add (fourteen15, 147, 171, 0, 3);
			lineGrid.Children.Add (fifteen16, 171, 195, 0, 3);
			lineGrid.Children.Add (sixteen17, 195, 219, 0, 3);
			lineGrid.Children.Add (seventeen18, 219, 243, 0, 3);

			var tap89 = new TapGestureRecognizer ();
			tap89.Tapped += (object sender, EventArgs e) => {
				Navigation.PushModalAsync (new FacilityReservationPage (passFacilityID, "89", date));
			};
			eightnine.GestureRecognizers.Add (tap89);

			var tap910 = new TapGestureRecognizer ();
			tap910.Tapped += (object sender, EventArgs e) => {
				Navigation.PushModalAsync (new FacilityReservationPage (passFacilityID, "910", date));
			};
			nineten.GestureRecognizers.Add (tap910);

			var tap1011 = new TapGestureRecognizer ();
			tap1011.Tapped += (object sender, EventArgs e) => {
				Navigation.PushModalAsync (new FacilityReservationPage (passFacilityID, "1011", date));
			};
			teneleven.GestureRecognizers.Add (tap1011);

			var tap1112 = new TapGestureRecognizer ();
			tap1112.Tapped += (object sender, EventArgs e) => {
				Navigation.PushModalAsync (new FacilityReservationPage (passFacilityID, "1112", date));
			};
			eleventwelve.GestureRecognizers.Add (tap1112);

			var tap1213 = new TapGestureRecognizer ();
			tap1213.Tapped += (object sender, EventArgs e) => {
				Navigation.PushModalAsync (new FacilityReservationPage (passFacilityID, "1213", date));
			};
			twelve13.GestureRecognizers.Add (tap1213);

			var tap1314 = new TapGestureRecognizer ();
			tap1314.Tapped += (object sender, EventArgs e) => {
				Navigation.PushModalAsync (new FacilityReservationPage (passFacilityID, "1314", date));
			};
			thirteen14.GestureRecognizers.Add (tap1314);

			var tap1415 = new TapGestureRecognizer ();
			tap1415.Tapped += (object sender, EventArgs e) => {
				Navigation.PushModalAsync (new FacilityReservationPage (passFacilityID, "1415", date));
			};
			fourteen15.GestureRecognizers.Add (tap1415);

			var tap1516 = new TapGestureRecognizer ();
			tap1516.Tapped += (object sender, EventArgs e) => {
				Navigation.PushModalAsync (new FacilityReservationPage (passFacilityID, "1516", date));
			};
			fifteen16.GestureRecognizers.Add (tap1516);

			var tap1617 = new TapGestureRecognizer ();
			tap1617.Tapped += (object sender, EventArgs e) => {
				Navigation.PushModalAsync (new FacilityReservationPage (passFacilityID, "1617", date));
			};
			sixteen17.GestureRecognizers.Add (tap1617);

			var tap1718 = new TapGestureRecognizer ();
			tap1718.Tapped += (object sender, EventArgs e) => {
				Navigation.PushModalAsync (new FacilityReservationPage (passFacilityID, "1718", date));
			};
			seventeen18.GestureRecognizers.Add (tap1718);

			for (int j = 0; j < reservationList.Count; j++) {

				if (reservationList [j].facilityID == passFacilityID) {
				//booking reserved
				string text;
				if (reservationList [j].useShortDescription == null) {
					text = "";
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
					Label resLabelTap = new Label {
						Text = text,
						TextColor = Color.Black,
						FontSize = 11,
						XAlign = TextAlignment.Center,
						YAlign = TextAlignment.Center
					}; 
					lineGrid.Children.Add (resLabelTap, start, end, 1, 2);

					Label id = new Label ();
					id.Text = reservationList [j].facilityReservationID;
					id.IsVisible = false;

					//add gesture to tap
					var bosRes = new TapGestureRecognizer ();
					bosRes.Tapped += (object sender, EventArgs e) => {
						//pass in reservation ID to know which reservation is clicked
						Navigation.PushModalAsync (new FacilityCancelPage (passFacilityID, id.Text, date));
					};
					resLabelTap.GestureRecognizers.Add (bosRes);
				}	

				//comeback and do again
				//tell if available or reserved
				dateNowHourStatus = DateTime.Now.TimeOfDay;
				dateStartStatus = reservationList [j].startDateTime.TimeOfDay;
				dateEndStatus = reservationList [j].endDateTime.TimeOfDay;

				//come back and do again
				if (dateStartStatus <= dateNowHourStatus && dateEndStatus >= dateNowHourStatus) {
					//reservation status
					reserve.Text = "Reserved";
					reserve.YAlign = TextAlignment.Center;
					reserve.FontSize = 34;
					reserve.FontAttributes = FontAttributes.Bold;

					//set box colour based on reserved or no
					box.BackgroundColor = Color.Red;
					//box.BackgroundColor = Color.Lime;
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

			int linestart,boxstart;

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

			//only show the line if the rdate is clicked today
			//if (date == DateTime.Now.ToString ("yyyy-MMM-dd")) {
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

			if (date == DateTime.Now.ToString ("yyyy-MMM-dd")) {
				//draw red line 
				lineGrid.Children.Add (new BoxView { BackgroundColor = Color.Red }, linestart, linestart + 1, 0, 3);
			}
			//}
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
			if (date == DateTime.Now.ToString ("yyyy-MMM-dd")) {
				boxGrid.Children.Add (new BoxView { BackgroundColor = Color.Red }, boxstart - 9, boxstart + 10, 0, 1);
				boxGrid.Children.Add (new Label {
					Text = dateTiming,
					TextColor = Color.Black,
					FontSize = 9.5,
					XAlign = TextAlignment.Center,
					YAlign = TextAlignment.Center
				}, boxstart - 9, boxstart + 10, 0, 1);
			}
		}
	}
}

