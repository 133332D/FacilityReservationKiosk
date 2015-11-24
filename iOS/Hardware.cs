using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace FacilityReservationKiosk.iOS
{
	public class Hardware : IHardware
	{

		private static Hardware _Default;

		string status;
		string message;

		public static Hardware Default {
			get {
				return _Default ?? (_Default = new Hardware ());
			}
		}

		//Create public string property
		public string DeviceId {
			get {
				return GetDeviceIdInternal();
			}
		}

		private string GetDeviceIdInternal()
		{
			var id = default(string);

			//For android device
			//id = Android.OS.Build.Serial;

			//For IOS device 
			id = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();

			return id;
		}

		//Constructor - To declare what are the attributes needed for the method
		string publickey;
		string privatekey;

		public string PublicKey {
			get {
				return publickey;
			}
		}

		public string PrivateKey {
			get {
				return privatekey;
			}
		}

		public string SignString(string s)
		{
			LoadOrGenerateKeys("publickey.Txt", "privatekey.Txt");
			RSACryptoServiceProvider rsaProvider = null;

			rsaProvider = new RSACryptoServiceProvider ();
			rsaProvider.FromXmlString (privatekey);

			string signatureResult = "";
			byte[] signature = rsaProvider.SignData(System.Text.UTF8Encoding.UTF8.GetBytes(s.ToCharArray()), "SHA256");
			for(int i=0; i<signature.Length; i++)
				signatureResult += String.Format("{0:X2}", signature[i]);

			return signatureResult;
		}

		public string LoadOrGenerateKeys (string publicKeyFileName, string privateKeyFileName)
		{
			if (publickey != null)
				return null;
			
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine (documentsPath, publicKeyFileName);

			//System.IO.File.Delete(filePath);
			//Check if public key pairs have been generated
			if (File.Exists (filePath)) {

				//Loading public key pair from file 
				filePath = Path.Combine (documentsPath, publicKeyFileName);
				publickey = System.IO.File.ReadAllText (filePath);

				//Loading private key pair from file 
				filePath = Path.Combine (documentsPath, privateKeyFileName);
				privatekey = System.IO.File.ReadAllText (filePath);

				return null;
			}

			//Generate a new public key pair if it have not been generated 
			else {
				CspParameters cspParams = null;
				RSACryptoServiceProvider rsaProvider = null;
				StreamWriter publicKeyFile = null;
				StreamWriter privateKeyFile = null;

				try {
					//Create a new key pair on target CSP
					cspParams = new CspParameters();
					cspParams.ProviderType = 1; //PROV_RSA_FULL
					cspParams.Flags = CspProviderFlags.NoPrompt;
					cspParams.KeyNumber = (int)KeyNumber.Exchange;
					rsaProvider = new RSACryptoServiceProvider (cspParams);

					//Export Public Key
					publickey = rsaProvider.ToXmlString(false);

					RSAParameters p = rsaProvider.ExportParameters(true);

					//Export Private Key 
					privatekey = rsaProvider.ToXmlString(true);

					//Web Service 
					//url & data to be pass to be saved in the database 
					string urlAdmin = ConfigurationSettings.urladmin + "Registration.aspx?UniqueID=" + ConfigurationSettings.Hardware.DeviceId
						+ "&PublicKey=" + System.Web.HttpUtility.UrlEncode(publickey);
					
					using (var client2 = new HttpClient ()) {
						HttpResponseMessage responseMsg2 = client2.GetAsync (urlAdmin).Result;

						var json2 = responseMsg2.Content.ReadAsStringAsync ();
						json2.Wait ();

						string jsonString = json2.Result.ToString ();
						var obj = JObject.Parse (jsonString);
						status = (string)obj.SelectToken ("Result");
						message = (string)obj.SelectToken ("Message");
						}
						
					//Saving public key to file
					filePath = Path.Combine (documentsPath, publicKeyFileName);
					System.IO.File.WriteAllText (filePath, publickey);

					//Saving private key to file
					filePath = Path.Combine (documentsPath, privateKeyFileName);
					System.IO.File.WriteAllText (filePath, privatekey);

					if (status == "OK") 
					{
						return null;
					}
					else 
					{
						return ("Error: " + message);
					}


				}
				catch (Exception ex) {
					//Any errors? Show them 
					Console.WriteLine ("Exception generating a new key pair! More info: ");
					Console.WriteLine (ex.Message);

					return ("Exception generating a new key pair! More info: " + ex.Message);
				}
				finally {
				}

			}
		}

//		private static Hardware _Default;
//
//		public static Hardware Default {
//			get {
//				return _Default ?? (_Default = new Hardware ());
//			}
//		}
//
//		//Create public string property
//		public string DeviceId {
//			get {
//				return GetDeviceIdInternal();
//			}
//		}
//
//		private string GetDeviceIdInternal()
//		{
//			var id = default(string);
//
//			//For android device
//			//id = Android.OS.Build.Serial;
//
//			//For IOS device 
//			id = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();
//
//			return id;
//		}
//
//		//Constructor - To declare what are the attributes needed for the method
//		string publickey;
//		string privatekey;
//
//		public string PublicKey {
//			get {
//				return publickey;
//			}
//		}
//
//		public string PrivateKey {
//			get {
//				return privatekey;
//			}
//		}
//
//		public void LoadOrGenerateKeys (string publicKeyFileName, string privateKeyFileName)
//		{
//			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
//			var filePath = Path.Combine (documentsPath, publicKeyFileName);
//
//			//System.IO.File.Delete(filePath);
//
//			//Check if public key pairs have been generated
//			if (File.Exists (filePath)) {
//
//				//Loading public key pair from file 
//				filePath = Path.Combine (documentsPath, publicKeyFileName);
//				publickey = System.IO.File.ReadAllText (filePath);
//
//				//Loading private key pair from file 
//				filePath = Path.Combine (documentsPath, privateKeyFileName);
//				privatekey = System.IO.File.ReadAllText (filePath);
//			}
//
//			//Generate a new public key pair if it have not been generated 
//			else {
//				CspParameters cspParams = null;
//				RSACryptoServiceProvider rsaProvider = null;
//				StreamWriter publicKeyFile = null;
//				StreamWriter privateKeyFile = null;
//
//				try {
//					//Create a new key pair on target CSP
//					cspParams = new CspParameters();
//					cspParams.ProviderType = 1; //PROV_RSA_FULL
//					cspParams.Flags = CspProviderFlags.NoPrompt;
//					cspParams.KeyNumber = (int)KeyNumber.Exchange;
//					rsaProvider = new RSACryptoServiceProvider (cspParams);
//
//					//Export Public Key
//					publickey = rsaProvider.ToXmlString(false);
//
//					RSAParameters p = rsaProvider.ExportParameters(true);
//
//					//Export Private Key 
//					privatekey = rsaProvider.ToXmlString(true);
//
//					//Consume web service 
//					//url & data to be pass to be saved in the database 
//					string urlAdmin = ConfigurationSettings.urladmin + "Registration.aspx?UniqueID=" + DeviceId
//						+ "&PublicKey=" + publickey;
//
//
////					using (var client2 = new HttpClient ()) {
////						HttpResponseMessage responseMsg2 = client2.GetAsync (urlAdmin).Result;
////
////						var json2 = responseMsg2.Content.ReadAsStringAsync ();
////						json2.Wait ();
////
////						ReservationList list2 = JsonConvert.DeserializeObject<ReservationList> (json2.Result);
////						}
//
//
//					//Saving public key to file
//					filePath = Path.Combine (documentsPath, publicKeyFileName);
//					System.IO.File.WriteAllText (filePath, publickey);
//
//					//Saving private key to file
//					filePath = Path.Combine (documentsPath, privateKeyFileName);
//					System.IO.File.WriteAllText (filePath, privatekey);
//				}
//				catch (Exception ex) {
//					//Any errors? Show them 
//					Console.WriteLine ("Exception generating a new key pair! More info: ");
//					Console.WriteLine (ex.Message);
//				}
//				finally {
//				}
//
//			}
//		}

	}
}

