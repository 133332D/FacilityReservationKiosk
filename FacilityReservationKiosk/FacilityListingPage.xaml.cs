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
		string date = "2015-AUG-13";

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

		void TapL_Tapped ()
		{
			
			//if( == "Block L Level 2"){
			block = "L";
			//ImageL.Opacity = 1;
			//ImageM.Opacity = 0;
			this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			//Navigation.PushModalAsync (new FacilityListingPage ());
			//}
		}

		void TapM_Tapped (object sender, EventArgs e)
		{
			block = "M";
			level = "3";
			name = "M.3";
			//ImageL.Opacity = 0;
			//ImageM.Opacity = 1;
			//GetFacilityTable ();
			this.SetValue (MasterDetailPage.IsPresentedProperty, (object)false);
			//Navigation.PopModalAsync (true);
			GetFacilityTable ();

			//this.SetValue (MasterDetailPage.IsPresentedProperty, (object)false);

			//remove the rows from grid
			//facGrid.Children.RemoveAt(0);
			//facGrid.Children.Clear();

			//Navigation.PushModalAsync (new FacilityListingPage ());
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
								Text = "   " + filterList[m].filterName,
								YAlign = TextAlignment.Center,
								GestureRecognizers = {
									new TapGestureRecognizer() {
										Command = new Command(() => { 
											block = f.block;
											level = f.level;
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

		//method to call** to run the grid loop
		public void GetFacilityTable ()
		{
			//get datetime of today
			string dateToday = DateTime.Today.ToString ("D");

			//set the label eg. School Of IT, level 4
			//set based on filter***
			title.Text = "School Of IT, Level " + level + "\n" + dateToday;
			title.FontAttributes = FontAttributes.Bold;

			//activity indicator
//			activityIndicator.IsRunning = true;
//			activityIndicator.IsVisible = true;
//			activityIndicator.BindingContext = this;
//			activityIndicator.SetBinding (ActivityIndicator.IsVisibleProperty, "IsBusy");
//			this.IsBusy = true;

			facilityList = new List<FacObject> ();
		
			facGrid.Children.Clear ();
			facGrid.RowDefinitions.Clear ();
			facGrid.ColumnDefinitions.Clear ();

			//call webservice to get facility and reservation*
			//string urlFac = @"http://crowd.sit.nyp.edu.sg/FRSIPad/GetFacilities.aspx?DepartmentID=" + departmentID
			//+ "&Block=" + block + "&Level=" + level + "&Name=" + name + "&DeviceID=&Hash=";
			string urlFac = ConfigurationSettings.urliPad + "GetFacilities.aspx?DepartmentID=" + departmentID
			                + "&Block=" + block + "&Level=" + level + "&Name=" + name + "&DeviceID=&Hash=";

			string urlRes = ConfigurationSettings.urliPad + "GetFacilityReservations.aspx?DepartmentID=" + departmentID
			                + "&Block=" + block + "&Level=" + level + "&Name=" + name + "&Date=" + date + "&DeviceID=&Hash=";

			//to get all the facility and insert to an c# object
			using (var client = new HttpClient ()) {
				HttpResponseMessage responseMsg = client.GetAsync (urlFac).Result;

				//var json = client.GetStringAsync(string.Format(url));
				var json = responseMsg.Content.ReadAsStringAsync ();
				json.Wait ();
				FacilityList list = JsonConvert.DeserializeObject<FacilityList> (json.Result);
				//List<Facility> list = JsonConvert.DeserializeObject<List<Facility>>(json.ToString());

				foreach (Facility fac in list.Facilities) {
					FacObject facObject = new FacObject (fac.facilityID, fac.departmentID, fac.description, fac.block,
						                      fac.level, fac.name, fac.openHours, fac.closeHours, fac.maxBkTime, fac.maxBkUnits, fac.minBkTime,
						                      fac.maxBkUnits);
					facilityList.Add (facObject);
				}
			}

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

			//create new rows and column for the grid
			facGrid.RowDefinitions = new RowDefinitionCollection ();
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


			//for loop to change according to database**
			//number of facility from database
			//edit to change label to database value
			for (int i = 0; i < facilityList.Count; i++) {
				//for (int i = 0; i < facSample.Count; i++) {
				facGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (1, GridUnitType.Absolute) });
				facGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength (30, GridUnitType.Absolute) });

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
				tapFac.Tapped += (object sender, EventArgs e) => 
					Navigation.PushModalAsync (new FacilityDetailsPage (labelFac.Text));
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

						BoxView boxReservation = new BoxView { BackgroundColor = Color.Silver };
						//start, end , top, bottom
						//facGrid.Children.Add (box, 3, 243, (i*2) + 1 , (i*2)+2);
						facGrid.Children.Add (boxReservation, start, end, (i * 2) + 1, (i * 2) + 2);

						//facGrid.Children.Add (facBut, start, end, (i*2) + 1, (i*2)+2);
						Label labelRes =  new Label {
							Text = text,
							TextColor = Color.Black,
							FontSize = 13,
							XAlign = TextAlignment.Center,
							YAlign = TextAlignment.Center
						};
						facGrid.Children.Add (labelRes, start, end, (i * 2) + 1, (i * 2) + 2);

						Image imageArrow;
						imageArrow = new Image { Source = ImageSource.FromFile("arrowg.png") };
						facGrid.Children.Add (imageArrow, start, start + 20, (i * 2) + 2 , (i * 2) + 4);
						imageArrow.Opacity = 0;

						Image imageBox;
						imageBox = new Image { BackgroundColor = Color.Silver };
						facGrid.Children.Add (imageBox, start, start + 50, 6 , 11);	
						imageBox.Opacity = 0;

						//click button gesture
						//pop out box
						//pass in Facility id 

						//display details when boxview is tapped
						//
						var tapDes = new TapGestureRecognizer ();
						tapDes.Tapped += (object sender, EventArgs e) => {
							//Navigation.PushModalAsync (new FacilityDetailsPage (labelFac.Text));
							//imageArrow.Opacity = 0;

							imageBox.Opacity = 1;
							imageArrow.Opacity = 1;
						};
						labelRes.GestureRecognizers.Add (tapDes);
					}

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
				

				//draw vertical lines
				//8am,9am,10am
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 3, 4, i * 2, (i * 2) + 3);
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 27, 28, i * 2, (i * 2) + 3);
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 51, 52, i * 2, (i * 2) + 3);
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 75, 76, i * 2, (i * 2) + 3);
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 99, 100, i * 2, (i * 2) + 3);
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 123, 124, i * 2, (i * 2) + 3);
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 147, 148, i * 2, (i * 2) + 3);
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 171, 172, i * 2, (i * 2) + 3);
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 195, 196, i * 2, (i * 2) + 3);
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 219, 220, i * 2, (i * 2) + 3);
				facGrid.Children.Add (new BoxView { BackgroundColor = Color.Black }, 243, 244, i * 2, (i * 2) + 3);

				//Xamarin.Forms.Device.StartTimer (TimeSpan.FromSeconds(1), () => {

					//refresh red box
					//boxLine.BackgroundColor = Color.Transparent;

					BoxView bl =  new BoxView { BackgroundColor = Color.Red };

					boxLine = bl;

					facGrid.Children.Add (bl, linestart, linestart + 1, i * 2, (i * 2) + 3);

					//return true;

					//});
				}
			
			}

//			activityIndicator.IsRunning = false;
//			activityIndicator.IsVisible = false;
//			this.IsBusy = false;
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

			var tapM = new TapGestureRecognizer ();
			tapM.Tapped += TapM_Tapped;
			//M.GestureRecognizers.Add (tapM);

			//Date filter
			var todaytap = new TapGestureRecognizer ();
			todaytap.Tapped += todaytap_Tapped;
			lbl_today.GestureRecognizers.Add (todaytap);

			entry.Tapped += (object sender, EventArgs e) => 
				checkToday.Opacity = 0;
			entry.Completed += (object sender, EventArgs e) => {
				date = entry.Text;
				GetFacilityTable();
				checkToday.Opacity = 0;
				this.SetValue(MasterDetailPage.IsPresentedProperty,(object) false);
			};

			//availability
			var alltap = new TapGestureRecognizer ();
			alltap.Tapped += (object sender, EventArgs e) => 
				checkAll.Opacity = 1;
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
				boxRed.BackgroundColor = Color.Transparent;

				Label b =  new Label { BackgroundColor = Color.Red };

				boxRed = b;

				boxGrid.Children.Add (b, boxstart - 9, boxstart + 10, 0, 1);

				//refresh label
				timeLabel.Text = "";

				dateTiming = DateTime.Now.ToString ("hh.mm tt");

				Label l = new Label {
					Text = dateTiming,
					TextColor = Color.Black,
					FontSize = 9.5,
					XAlign = TextAlignment.Center,
					YAlign = TextAlignment.Center
				};

				timeLabel = l;

				boxGrid.Children.Add (l, boxstart - 9, boxstart + 10, 0, 1);

				//return true;
			//});
		}
	}
}


