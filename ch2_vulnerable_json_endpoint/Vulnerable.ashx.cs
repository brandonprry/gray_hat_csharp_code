
using System;
using System.Web;
using System.Web.UI;
using Npgsql;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace vulnerable_json_sqli
{
	public class Vulnerable : System.Web.IHttpHandler
	{
		
		string _connstr = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=secret;Database=vulnerable_json;";

		public virtual bool IsReusable {
			get {
				return false;
			}
		}

		public virtual void ProcessRequest (HttpContext context)
		{
			JObject obj = JObject.Parse (new System.IO.StreamReader (context.Request.InputStream).ReadToEnd ());

			string method = (string)obj ["method"];

			if (string.IsNullOrEmpty (method)) {
				context.Response.Write ("Need to have a method of list, delete, or create");
				return;
			}

			if (method == "list") {
				JObject[] users = ListUsers (obj ["username"].Value<string>());

				context.Response.Write (JsonConvert.SerializeObject (users));
				

			} else if (method == "create") {
				string username = (string)obj ["username"];
				string password = (string)obj ["password"];
				string age = (string)obj ["age"];
				string line1 = (string)obj ["line1"];
				string line2 = (string)obj ["line2"];
				string city = (string)obj ["city"];
				string state = (string)obj ["state"];
				string zip = (string)obj ["zip"];
				string first = (string)obj ["first"];
				string middle = (string)obj ["middle"];
				string last = (string)obj ["last"];

				if (string.IsNullOrEmpty (username) || string.IsNullOrEmpty (password)) {
					context.Response.Write ("Need a username and a password");
					return;
				}

				bool success = CreateUser (username, password, age, line1, line2, city, state, zip, first, middle, last);

				context.Response.Write ("{ success : " + success.ToString () + " }");
				return;
			} else if (method == "delete") {
				string username = (string)obj ["username"];

				if (string.IsNullOrEmpty (username)) {
					context.Response.Write ("Need a username");
					return;
				}

				bool success = DeleteUser (username);

				context.Response.Write ("{ success : " + success.ToString () + " }");
				return;
			} else {
				context.Response.Write ("Don't recognize: " + method + ". Need list, create, or delete.");
				return;
			}
		}

		private bool CreateUser (string username, string password, string age, string line1, string line2, string city, string state, string zip, string first, string middle, string last)
		{
			NpgsqlConnection conn = new NpgsqlConnection (_connstr);

			try {
				conn.Open ();

				string sql = "INSERT INTO USERS VALUES ('";
				sql += username + "','" + password + "," + age + ",'" + line1 + "','" + line2 + "','";
				sql += city + "','" + state + "'," + zip + "');";
				NpgsqlCommand cmd = new NpgsqlCommand (sql, conn);
				cmd.ExecuteNonQuery ();
			} finally {
				conn.Close ();
			}
		
			return true;
		}

		private bool DeleteUser (string username)
		{
			NpgsqlConnection conn = new NpgsqlConnection (_connstr);

			try {
				conn.Open ();

				NpgsqlCommand cmd = new NpgsqlCommand ("DELETE FROM USERS WHERE USERNAME='" + username + "';", conn);
				cmd.ExecuteNonQuery ();
			} finally {
				conn.Close ();
			}

			return true;
		}

		private JObject[] ListUsers (string username)
		{

			NpgsqlConnection conn = new NpgsqlConnection (_connstr);
			List<JObject> users = new List<JObject> ();

			try {
				conn.Open ();

				NpgsqlCommand cmd = new NpgsqlCommand ("SELECT username FROM USERS WHERE USERNAME LIKE '%" + username + "%';", conn);
				NpgsqlDataReader rdr = cmd.ExecuteReader ();

				while (rdr.Read ()) {
					JObject obj = new JObject ();
					obj ["username"] = (string)rdr [0];
					users.Add (obj);
				}

			} finally {
				conn.Close ();
			}

			return users.ToArray();
		}
	}
}

