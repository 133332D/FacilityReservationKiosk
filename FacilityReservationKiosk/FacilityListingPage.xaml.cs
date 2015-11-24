using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using AdvancedTimer;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Globalization;

using Xamarin.Forms;

namespace FacilityReservationKiosk
{
	public partial class FacilityListingPage : MasterDetailPage
	{
		//private readonly ObservableCollection<FilterItem> filterListItem = new ObservableCollection<FilterItem> ();

		//filter
		string departmentID = ConfigurationSettings.departmentID;
		string block = "L";
		string level = "4";
		string name = "L.4";
		//yyyy-MMM-dd
		//check with webservice to confirm format again
		string date = ""; 

		string availability = "";

		Label timeLabel = new Label{};

		Label boxRed = new Label{};
		BoxView boxLine = new BoxView{};

		//create a list of facility and reservation
		List<FacObject> facilityList;
		List<ResObject> reservationList = new List<ResObject> ();
		List<FilObject> filterList = new List<FilObject>();

		int linestart, boxstart;
		string dateTiming;

		//activity indicator

		class FacObject
		{
			public string facilityID { get; set; }

			public string departmentID { get; set; }

			public string description { get; set; }

			public string block { get; set; }

			public string level { get; set; }

			public string name { get; set; }

			public string openHours { get; set; }

			public string closeHours { get; set; }

			public string maxBkTime { get; set; }

			public string maxBkUnits { get; set; }

			public string minBkTime { get; set; }

			public string minBkUnits { get; set; }

			public FacObject (string facid, string depid, string desc, string b, string l, string n
				, string o, string c, string maxt, string maxu, string mint, string minu)
			{
				this.facilityID = facid;
				this.departmentID = depid;
				this.description = desc;
				this.block = b;
				this.level = l;
				this.name = n;
				this.openHours = o;
				this.closeHours = c;
				this.maxBkTime = maxt;
				this.maxBkUnits = maxu;
				this.minBkTime = mint;
				this.minBkUnits = minu;
			}
		}

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

		class FilObject
		{
			public int departmentFilterID { get; set; }

			public string departmentID { get; set; }

			public string filterName { get; set; }

			public string block { get; set; }

			public string level { get; set; }

			public string name { get; set; }

			public FilObject ( int dfid, string did, string fn, string b, string l, string n)
			{
				this.departmentFilterID = dfid;
				this.departmentID = did;
				this.filterName = fn;
				this.block = b;
				this.level = l;
				this.name = n;
			}
		}

		public class Facility
		{
			public string facilityID { get; set; }

			public string departmentID { get; set; }

			public string description { get; set; }

			public string block { get; set; }

			public string level { get; set; }

			public string name { get; set; }

			public string openHours { get; set; }

			public string closeHours { get; set; }

			public string maxBkTime { get; set; }

			public string maxBkUnits { get; set; }

			public string minBkTime { get; set; }

			public string minBkUnits { get; set; }
		}

		public class FacilityList
		{
			public List<Facility> Facilities { get; set; }
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

		public class Filter
		{
			public int departmentFilterID { get; set; }

			public string departmentID { get; set; }

			public string filterName { get; set; }

			public string block { get; set; }

			public string level { get; set; }

			public string name { get; set; }
		}

		public class FilterList
		{
			public List<Filter> Filters { get; set; }
		}

		void todaytap_Tapped (object sender, EventArgs e)
		{
			//"2015-AUG-13"
			date = DateTime.Now.ToString("yyyy-MMM-dd");
			checkToday.Opacity = 1;
			this.SetValue (MasterDetailPage.IsPresentedProperty, (object)false);
			//Navigation.PushModalAsync (new FacilityListingPage ());
			GetFacilityTable ();
		}
			

		//method to get departmentfilters
		public void GetFilters () 
		{
			string urlFil = ConfigurationSettings.urliPad + "GetDepartmentFilters.aspx?DepartmentID=" + departmentID;

			urlFil = Security.SignHttpRequest (urlFil);

			using (var client = new HttpClient ()) {
				HttpResponseMessage responseMsg3 = client.GetAsync (urlFil).Result;

				//var json = client.GetStringAsync(string.Format(url));
				var json3 = responseMsg3.Content.ReadAsStringAsync ();
				json3.Wait ();
				FilterList list = JsonConvert.DeserializeObject<FilterList> (json3.Result);
				//List<Facility> list = JsonConvert.DeserializeObject<List<Facility>>(json.ToString());

				foreach (Filter fil in list.Filters) {
					FilObject filObject  = new FilObject (fil.departmentFilterID, fil.departmentID, fil.filterName,
						fil.block, fil.level, fil.name);
					///
					filterList.Add (filObject);

					//FilterItem filterItem = new FilterItem (fil.filterName);
					//filterListItem.Add (filterItem);
				}
			}
		}
			
		List<Image> filterImages = new List<Image>();

		public void addViewCell () 
		{
			filterImages.Clear ();
			// cmment
			for (int m = 0; m < filterList.Count; m++) {
				var f = filterList [m];

				var image = new Image () {
					Source = FileImageSource.FromFile ("check_mark.jpg"),
					HorizontalOptions = LayoutOptions.EndAndExpand
				};
				filterImages.Add (image);

				//default filter
				for (int k = 0; k < filterImages.Count; k++){
					filterImages[k].Opacity = 0;
				}
				filterImages [0].Opacity = 1;

				ViewCell vc = new ViewCell {
					View = new StackLayout {
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.Start,
						Children = {
							new Label () {
								Text = "   " + filterList[m].filterName + "           ",
								YAlign = TextAlignment.Center,
								GestureRecognizers = {
									new TapGestureRecognizer() {
										Command = new Command(() => { 
											block = f.block;
											level = f.level;
											name = f.name;
											for (int k = 0; k < filterImages.Count; k++){
												filterImages[k].Opacity = 0;
											}
											image.Opacity = 1;
											GetFacilityTable();
											this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
										}),
									}
								}
							},
							image
						}
					}
				};
				tableView.Root [0].Add (vc);
			}
		}

		DateTime? dateStartAvail = null;
		DateTime? dateEndAvail = null;
		DateTime dateConvert;

		public void filterAvailability()
		{
			for (int f = 0; f < reservationList.Count; f++) {
				if (((dateStartAvail <= reservationList [f].startDateTime &&
					reservationList [f].startDateTime < dateEndAvail)
					|| (dateStartAvail <= reservationList [f].endDateTime &&
						reservationList [f].endDateTime < dateEndAvail)
					|| (reservationList [f].startDateTime <= dateStartAvail &&
						dateEndAvail <= reservationList [f].endDateTime))) {
					facilityList.RemoveAll (x => x.facilityID == reservationList[f].facilityID);
				}
			}
		}

		//method to call** to run the grid loop
		public void GetFacilityTable ()
		{
			//get datetime of today
			string dateToday = DateTime.Today.ToString ("dd-MMM-yyyy");
			string[] sDateSeperate = date.Split (new[] { "-" }, StringSplitOptions.None);

			//set the label eg. School Of IT, level 4
			//set based on filter***
			if (date == "") {
				title.Text = "School Of IT, Level " + level + "\n" + "Today" + "\n" + dateToday;
			} else {
				title.Text = "School Of IT, Level " + level + "\n" + sDateSeperate[2] + "-" + sDateSeperate[1] + "-" + sDateSeperate[0];
			}
			title.FontAttributes = FontAttributes.Bold;

			//activity indicator
//			activityIndicator.IsRunning = true;
//			activityIndicator.IsVisible = true;
//			activityIndicator.BindingContext = this;
//			activityIndicator.SetBinding (ActivityIndicator.IsVisibleProperty, "IsBusy");
//			this.IsBusy = true;

			facilityList = new List<FacObject> ();
			reservationList = new List<ResObject> ();
		
			facGrid.IsVisible = false;
			facGrid.Children.Clear ();
			boxGrid.Children.Clear ();
			facGrid.RowDefinitions.Clear ();
			//facGrid.ColumnDefinitions.Clear ();
			facGrid.IsVisible = true;

			//string urlFac = @"http://crowd.sit.nyp.edu.sg/FRSIPad/GetFacilities.aspx?DepartmentID=" + departmentID
			//+ "&Block=" + block + "&Level=" + level + "&Name=" + name + "&DeviceID=&Hash=";

			//call webservice to get facility and reservation*
			string urlFac = ConfigurationSettings.urliPad + "GetFacilities.aspx?DepartmentID=" + departmentID
			                + "&Block=" + block + "&Level=" + level + "&Name=" + name;

			string urlRes = ConfigurationSettings.urliPad + "GetFacilityReservations.aspx?DepartmentID=" + departmentID
			                + "&Block=" + block + "&Level=" + level + "&Name=" + name + "&Date=" + date;


			//to get all the facility and icnsert to an c# object
			using (var client = new HttpClient ()) {
				HttpResponseMessage responseMsg = client.GetAsync (urlFac).Result;

				var json = responseMsg.Content.ReadAsStringAsync ();
				json.Wait ();
				FacilityList list = JsonConvert.DeserializeObject<FacilityList> (json.Result);

				foreach (Facility fac in list.Facilities) {
					FacObject facObject = new FacObject (fac.facilityID, fac.departmentID, fac.description, fac.block,
						                      fac.level, fac.name, fac.openHours, fac.closeHours, fac.maxBkTime, 
												fac.maxBkUnits, fac.minBkTime, fac.maxBkUnits);
					facilityList.Add (facObject);
				}
			}

			//to get all the reservations and insert to an c# object
			using (var client2 = new HttpClient ()) {
				HttpResponseMessage responseMsg2 = client2.GetAsync (urlRes).Result;

				//var json = client.GetStringAsync(string.Format(url));
				var json2 = responseMsg2.Content.ReadAsStringAsync ();
				json2.Wait ();
				ReservationList list2 = JsonConvert.DeserializeObject<ReservationList> (json2.Result);
				//List<Facility> list = JsonConvert.DeserializeObject<List<Facility>>(json.ToString());

				foreach (Reservation res in list2.Reservations) {
					ResObject resObject = new ResObject (res.facilityReservationID, res.facilityID, res.startDateTime, res.endDateTime,
						                      res.useShortDescription, res.useDescription);
					reservationList.Add (resObject);
				}
			}
				
			if (date == "") {
				date = DateTime.Now.ToString ("yyyy-MMM-dd");
				dateConvert = Convert.ToDateTime (date);
			} else {
				dateConvert = Convert.ToDateTime (date);
			}

			if (availability == "89") {
				dateStartAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 08, 00, 00);
				dateEndAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 09, 00, 00);
				filterAvailability ();

			}
			if (availability == "910") {
				dateStartAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 09, 00, 00);
				dateEndAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 10, 00, 00);
				filterAvailability ();
			}
			if (availability == "1011") {
				dateStartAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 10, 00, 00);
				dateEndAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 11, 00, 00);
				filterAvailability ();
			}
			if (availability == "1112") {
				dateStartAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 11, 00, 00);
				dateEndAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 12, 00, 00);
				filterAvailability ();
			}
			if (availability == "1213") {
				dateStartAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 12, 00, 00);
				dateEndAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 13, 00, 00);
				filterAvailability ();
			}
			if (availability == "1314") {
				dateStartAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 13, 00, 00);
				dateEndAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 14, 00, 00);
				filterAvailability ();
			}
			if (availability == "1415") {
				dateStartAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 14, 00, 00);
				dateEndAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 15, 00, 00);
				filterAvailability ();
			}
			if (availability == "1516") {
				dateStartAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 15, 00, 00);
				dateEndAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 16, 00, 00);
				filterAvailability ();
			}
			if (availability == "1617") {
				dateStartAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 16, 00, 00);
				dateEndAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 17, 00, 00);
				filterAvailability ();
			}
			if (availability == "1718") {
				dateStartAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 17, 00, 00);
				dateEndAvail = new DateTime (dateConvert.Year, dateConvert.Month, dateConvert.Day, 18, 00, 00);
				filterAvailability ();
			}

			//create new rows and column for the grid
			facGrid.RowDefinitions = new RowDefinitionCollection ();

			if (facGrid.ColumnDefinitions.Count == 0) {
				facGrid.ColumnDefinitions = new ColumnDefinitionCollection ();

				//create the columns
				//0,1,2
				facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (10, GridUnitType.Absolute) });
				facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (20, GridUnitType.Absolute) });
				facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) });

				//column with interval of 5 minutes
				facGrid.ColumnSpacing = 0;

				for (int k = 0; k < 10; k++) { 
					//00
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					//15
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					//30
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					//45
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
					facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (4, GridUnitType.Absolute) });
				}

				//6pm (243)
				facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });

				//spacing of 10 (244,245)
				facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (30, GridUnitType.Absolute) });
				facGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (10, GridUnitType.Absolute) });
			}

			for (int i = 0; i < facilityList.Count; i++) {
				//for (int i = 0; i < facSample.Count; i++) {
				facGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (1, GridUnitType.Absolute) });
				facGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });
			}

			//for loop to change according to database**
			//number of facility from database
			//edit to change label to database value
			for (int i = 0; i < facilityList.Count; i++) {
				//for (int i = 0; i < facSample.Count; i++) {
				//facGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (1, GridUnitType.Absolute) });
				//facGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });

				//number = (left,right,top,bottom)
				//black horizonal line
				//facility label
				//facGrid.Children.Add (facBut,2,((i*2)+1));
				Label labelFac = new Label {
					Text = facilityList [i].facilityID,
					TextColor = Color.Black,
					FontAttributes = FontAttributes.Bold,
					YAlign = TextAlignment.Center
				};
				var tapFac = new TapGestureRecognizer ();
				tapFac.Tapped += (object sender, EventArgs e) =>{
					Navigation.PushModalAsync (new FacilityDetailsPage (labelFac.Text, date));
				};
				labelFac.GestureRecognizers.Add (tapFac);

				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 1, 245, (i * 2), ((i * 2) + 1));
				facGrid.Children.Add (labelFac, 2, ((i * 2) + 1));
				//facGrid.Children.Add (new Label { Text = facSample[i], TextColor = Color.Black }, 2,((i*2)+1));


				for (int j = 0; j < reservationList.Count; j++) {
					//booking reserved
					//break if facilityID dont match
					if (reservationList [j].facilityID == facilityList [i].facilityID) {

						string text;
						if (reservationList [j].useShortDescription == null) {

							string temptext = reservationList [j].useDescription;
							string str = temptext.Substring (0, 4);
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

						//BoxView boxReservation = new BoxView { BackgroundColor = Color.Silver };
						//start, end , top, bottom
						//facGrid.Children.Add (box, 3, 243, (i*2) + 1 , (i*2)+2);
						//facGrid.Children.Add (boxReservation, start, end, (i * 2) + 1, (i * 2) + 2);

						//facGrid.Children.Add (facBut, start, end, (i*2) + 1, (i*2)+2);

						Label labelRes =  new Label {
							Text = text,
							TextColor = Color.Black,
							FontSize = 13,
							XAlign = TextAlignment.Center,
							YAlign = TextAlignment.Center,
							BackgroundColor = Color.Silver
						};
						facGrid.Children.Add (labelRes, start, end, (i * 2) + 1, (i * 2) + 2);

						/*
						Image imageArrow;
						imageArrow = new Image { Source = ImageSource.FromFile("arrowg.png") };
						facGrid.Children.Add (imageArrow, start, start + 20, (i * 2) + 2 , (i * 2) + 4);
						imageArrow.Opacity = 0;

						Image imageBox;
						imageBox = new Image { BackgroundColor = Color.Silver };
						facGrid.Children.Add (imageBox, start, start + 50, 6 , 11);	
						imageBox.Opacity = 0;
*/
						//click button gesture
						//pop out box
						//pass in Facility id 

						//display details when boxview is tapped
						//
						var tapDes = new TapGestureRecognizer ();
						tapDes.Tapped += (object sender, EventArgs e) => {
							//Navigation.PushModalAsync (new FacilityDetailsPage (labelFac.Text));
							//imageArrow.Opacity = 0;

							//imageBox.Opacity = 1;
							//imageArrow.Opacity = 1;
						};
						labelRes.GestureRecognizers.Add (tapDes);
					}


				//Xamarin.Forms.Device.StartTimer (TimeSpan.FromSeconds(1), () => {

					//refresh red box
					//boxLine.BackgroundColor = Color.Transparent;

					//return true;

					//});
				}
			
			}


			//draw vertical lines
			//8am,9am,10am

			int endCol = ((facilityList.Count - 1) * 2) + 3;
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 3, 4, 0, endCol);
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 27, 28, 0, endCol);
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 51, 52, 0, endCol);
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 75, 76, 0, endCol);
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 99, 100, 0, endCol);
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 123, 124, 0, endCol);
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 147, 148, 0, endCol);
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 171, 172, 0, endCol);
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 195, 196, 0, endCol);
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 219, 220, 0, endCol);
			facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 243, 244, 0, endCol);


			//date now
			string dateNowLine = DateTime.Now.ToString ("dd-MM-yyyy HH:mm:ss");
			dateTiming = DateTime.Now.ToString ("hh.mm tt");
			string[] linetoken = dateNowLine.Split (new[] { " " }, StringSplitOptions.None);
			//time
			string[] linetiming = linetoken [1].Split (new[] { ":" }, StringSplitOptions.None);
			//timing[0] = 08 (hour) //timing[1] = 30 (mins)
			int linehour = Convert.ToInt16 (linetiming [0]);
			int linemin = Convert.ToInt16 (linetiming [1]);

			int boxhour = Convert.ToInt16 (linetiming [0]);
			int boxmin = Convert.ToInt16 (linetiming [1]);

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

			if (date == "" || date == DateTime.Now.ToString ("yyyy-MMM-dd")) {
				BoxView bl = new BoxView { BackgroundColor = Color.Red };

				boxLine = bl;

				facGrid.Children.Add (bl, linestart, linestart + 1, 0, ((facilityList.Count - 1) * 2) + 3);
			}

//			activityIndicator.IsRunning = false;
//			activityIndicator.IsVisible = false;
//			this.IsBusy = false;
			//box view grid
			//create new rows and column for the grid
			boxGrid.RowDefinitions = new RowDefinitionCollection ();
			boxGrid.ColumnDefinitions = new ColumnDefinitionCollection ();

			//create the columns
			//0,1,2
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Absolute) });
			boxGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) });

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

			//Xamarin.Forms.Device.StartTimer (TimeSpan.FromSeconds(1), () => {

			//refresh red box
			if (date == "" || date == DateTime.Now.ToString ("yyyy-MMM-dd")) {
				//boxRed.BackgroundColor = Color.Transparent;
				Label b = new Label { BackgroundColor = Color.Red };

				dateTiming = DateTime.Now.ToString ("hh.mm tt");
				Label l = new Label {
					Text = dateTiming,
					TextColor = Color.Black,
					FontSize = 9.5,
					XAlign = TextAlignment.Center,
					YAlign = TextAlignment.Center
				};
				//boxRed = b;

				boxGrid.Children.Add (b, boxstart - 9, boxstart + 10, 0, 1);

				//refresh label
				//timeLabel.Text = "";

				//timeLabel = l;

				boxGrid.Children.Add (l, boxstart - 9, boxstart + 10, 0, 1);
			}

			//return true;
			//});
			facGrid.ForceLayout();
		}



		public FacilityListingPage ()
		{
			InitializeComponent ();

			GetFilters ();
			addViewCell ();
			//default filter**
			//date = DateTime.Now.ToString("yyyy-MMM-dd");

			//filter items
			//lvFilter.ItemsSource = filterListItem;
			//filterList.Add(new FilObject(1, "", "ABC", "", "", ""));
			//filterList.Add(new FilObject(1, "", "DEF", "", "", ""));
			//viewCellListView.ItemsSource = filterList;
			//filterName.SetBinding(

			//menu filter
			titlename.Text = "Filter";
			titlename.TextColor = Color.White;
			titlename.YAlign = TextAlignment.Center;
			titlename.FontSize = 23;

			//gesture
			//block
//			var tapL = new TapGestureRecognizer ();
//			tapL.Tapped += TapL_Tapped;
			//filterName.GestureRecognizers.Add (tapL);

//			var tapM = new TapGestureRecognizer ();
//			tapM.Tapped += TapM_Tapped;
			//M.GestureRecognizers.Add (tapM);

			checkAll.Opacity = 1;
			checkmark89.Opacity = 0;
			checkmark910.Opacity = 0;
			checkmark1011.Opacity = 0;
			checkmark1112.Opacity = 0;
			checkmark1213.Opacity = 0;
			checkmark1314.Opacity = 0;
			checkmark1415.Opacity = 0;
			checkmark1516.Opacity = 0;
			checkmark1617.Opacity = 0;
			checkmark1718.Opacity = 0;

			//availability
			var tapView89 = new TapGestureRecognizer ();
			tapView89.Tapped += (object sender, EventArgs e) => {
				checkAll.Opacity = 0;
				checkmark89.Opacity = 1;
				checkmark910.Opacity = 0;
				checkmark1011.Opacity = 0;
				checkmark1112.Opacity = 0;
				checkmark1213.Opacity = 0;
				checkmark1314.Opacity = 0;
				checkmark1415.Opacity = 0;
				checkmark1516.Opacity = 0;
				checkmark1617.Opacity = 0;
				checkmark1718.Opacity = 0;
				availability = "89";
				GetFacilityTable();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};;
			lbl_eightnine.GestureRecognizers.Add (tapView89);

			var tapView910 = new TapGestureRecognizer ();
			tapView910.Tapped += (object sender, EventArgs e) => {
				checkAll.Opacity = 0;
				checkmark89.Opacity = 0;
				checkmark910.Opacity = 1;
				checkmark1011.Opacity = 0;
				checkmark1112.Opacity = 0;
				checkmark1213.Opacity = 0;
				checkmark1314.Opacity = 0;
				checkmark1415.Opacity = 0;
				checkmark1516.Opacity = 0;
				checkmark1617.Opacity = 0;
				checkmark1718.Opacity = 0;
				availability = "910";
				GetFacilityTable();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};;
			lbl_nineten.GestureRecognizers.Add (tapView910);

			var tapView1011 = new TapGestureRecognizer ();
			tapView1011.Tapped += (object sender, EventArgs e) => {
				checkAll.Opacity = 0;
				checkmark89.Opacity = 0;
				checkmark910.Opacity = 0;
				checkmark1011.Opacity = 1;
				checkmark1112.Opacity = 0;
				checkmark1213.Opacity = 0;
				checkmark1314.Opacity = 0;
				checkmark1415.Opacity = 0;
				checkmark1516.Opacity = 0;
				checkmark1617.Opacity = 0;
				checkmark1718.Opacity = 0;
				availability = "1011";
				GetFacilityTable();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};;
			lbl_teneleven.GestureRecognizers.Add (tapView1011);

			var tapView1112 = new TapGestureRecognizer ();
			tapView1112.Tapped += (object sender, EventArgs e) => {
				checkAll.Opacity = 0;
				checkmark89.Opacity = 0;
				checkmark910.Opacity = 0;
				checkmark1011.Opacity = 0;
				checkmark1112.Opacity = 1;
				checkmark1213.Opacity = 0;
				checkmark1314.Opacity = 0;
				checkmark1415.Opacity = 0;
				checkmark1516.Opacity = 0;
				checkmark1617.Opacity = 0;
				checkmark1718.Opacity = 0;
				availability = "1112";
				GetFacilityTable();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};;
			lbl_eleven12.GestureRecognizers.Add (tapView1112);

			var tapView1213 = new TapGestureRecognizer ();
			tapView1213.Tapped += (object sender, EventArgs e) => {
				checkmark89.Opacity = 0;
				checkmark910.Opacity = 0;
				checkmark1011.Opacity = 0;
				checkmark1112.Opacity = 0;
				checkmark1213.Opacity = 1;
				checkmark1314.Opacity = 0;
				checkmark1415.Opacity = 0;
				checkmark1516.Opacity = 0;
				checkmark1617.Opacity = 0;
				checkmark1718.Opacity = 0;
				availability = "1213";
				GetFacilityTable();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};;
			lbl_twelve13.GestureRecognizers.Add (tapView1213);

			var tapView1314 = new TapGestureRecognizer ();
			tapView1314.Tapped += (object sender, EventArgs e) => {
				checkAll.Opacity = 0;
				checkmark89.Opacity = 0;
				checkmark910.Opacity = 0;
				checkmark1011.Opacity = 0;
				checkmark1112.Opacity = 0;
				checkmark1213.Opacity = 0;
				checkmark1314.Opacity = 1;
				checkmark1415.Opacity = 0;
				checkmark1516.Opacity = 0;
				checkmark1617.Opacity = 0;
				checkmark1718.Opacity = 0;
				availability = "1314";
				GetFacilityTable();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};;
			lbl_onetwo.GestureRecognizers.Add (tapView1314);

			var tapView1415 = new TapGestureRecognizer ();
			tapView1415.Tapped += (object sender, EventArgs e) => {
				checkAll.Opacity = 0;
				checkmark89.Opacity = 0;
				checkmark910.Opacity = 0;
				checkmark1011.Opacity = 0;
				checkmark1112.Opacity = 0;
				checkmark1213.Opacity = 0;
				checkmark1314.Opacity = 0;
				checkmark1415.Opacity = 1;
				checkmark1516.Opacity = 0;
				checkmark1617.Opacity = 0;
				checkmark1718.Opacity = 0;
				availability = "1415";
				GetFacilityTable();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};;
			lbl_twothree.GestureRecognizers.Add (tapView1415);

			var tapView1516 = new TapGestureRecognizer ();
			tapView1516.Tapped += (object sender, EventArgs e) => {
				checkAll.Opacity = 0;
				checkmark89.Opacity = 0;
				checkmark910.Opacity = 0;
				checkmark1011.Opacity = 0;
				checkmark1112.Opacity = 0;
				checkmark1213.Opacity = 0;
				checkmark1314.Opacity = 0;
				checkmark1415.Opacity = 0;
				checkmark1516.Opacity = 1;
				checkmark1617.Opacity = 0;
				checkmark1718.Opacity = 0;
				availability = "1516";
				GetFacilityTable();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};;
			lbl_threefour.GestureRecognizers.Add (tapView1516);

			var tapView1617 = new TapGestureRecognizer ();
			tapView1617.Tapped += (object sender, EventArgs e) => {
				checkAll.Opacity = 0;
				checkmark89.Opacity = 0;
				checkmark910.Opacity = 0;
				checkmark1011.Opacity = 0;
				checkmark1112.Opacity = 0;
				checkmark1213.Opacity = 0;
				checkmark1314.Opacity = 0;
				checkmark1415.Opacity = 0;
				checkmark1516.Opacity = 0;
				checkmark1617.Opacity = 1;
				checkmark1718.Opacity = 0;
				availability = "1617";
				GetFacilityTable();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};;
			lbl_fourfive.GestureRecognizers.Add (tapView1617);

			var tapView1718 = new TapGestureRecognizer ();
			tapView1718.Tapped += (object sender, EventArgs e) => {
				checkAll.Opacity = 0;
				checkmark89.Opacity = 0;
				checkmark910.Opacity = 0;
				checkmark1011.Opacity = 0;
				checkmark1112.Opacity = 0;
				checkmark1213.Opacity = 0;
				checkmark1314.Opacity = 0;
				checkmark1415.Opacity = 0;
				checkmark1516.Opacity = 0;
				checkmark1617.Opacity = 0;
				checkmark1718.Opacity = 1;
				availability = "1718";
				GetFacilityTable();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};;
			lbl_fivesix.GestureRecognizers.Add (tapView1718);

			//Date filter
			var todaytap = new TapGestureRecognizer ();
			todaytap.Tapped += todaytap_Tapped;
			lbl_today.GestureRecognizers.Add (todaytap);

			entry.Tapped += (object sender, EventArgs e) => 
				checkToday.Opacity = 0;
			entry.Completed += (object sender, EventArgs e) => {
				string dateCapture = entry.Text;
				string[] dateSplittedC = dateCapture.Split (new[] { "-" }, StringSplitOptions.None);
				date = dateSplittedC[2] + "-" + dateSplittedC[1] + "-" + dateSplittedC[0];
				GetFacilityTable();
				checkToday.Opacity = 0;
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};

			//availability
			var alltap = new TapGestureRecognizer ();
			alltap.Tapped += (object sender, EventArgs e) => {
				checkAll.Opacity = 1;
				checkmark89.Opacity = 0;
				checkmark910.Opacity = 0;
				checkmark1011.Opacity = 0;
				checkmark1112.Opacity = 0;
				checkmark1213.Opacity = 0;
				checkmark1314.Opacity = 0;
				checkmark1415.Opacity = 0;
				checkmark1516.Opacity = 0;
				checkmark1617.Opacity = 0;
				checkmark1718.Opacity = 1;
				availability = "";
				GetFacilityTable ();
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};
			lbl_All.GestureRecognizers.Add (alltap);

			//Facility Reservation Kiosk Name
			appName.Text = "Facility Reservation Kiosk";
			appName.FontAttributes = FontAttributes.Bold;


			//filter button to filter the list of facility
			filterButton.Image = "filter.png";
			filterButton.BackgroundColor = Color.Transparent;
			//press and the filter page would show
			filterButton.Clicked += (object sender, EventArgs e) => 
				this.SetValue (MasterDetailPage.IsPresentedProperty, (object)true);

			GetFacilityTable ();
		}
	}


	public class FastGrid : Grid
	{
		protected override bool ShouldInvalidateOnChildAdded (View child)
		{
			return false;
		}

		protected override bool ShouldInvalidateOnChildRemoved (View child)
		{
			return false;
		}
	}
}


