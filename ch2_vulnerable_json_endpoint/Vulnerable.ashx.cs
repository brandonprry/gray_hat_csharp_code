
using System;
using System.Web;
using System.Web.UI;
using Npgsql;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace vulnerable_json_sqli
{
	public class Vulnerable : System.Web.IHttpHandler
	{
		
		string _connstr = "Server=192.168.1.5;Port=5432;User Id=postgres;Password=secret;Database=vulnerable_json;";
		public virtual bool IsReusable {
			get {
				return false;
			}
		}
		
		public virtual void ProcessRequest (HttpContext context)
		{
			if (string.IsNullOrEmpty (context.Request ["JSON"])) {
				context.Response.Write ("Need a JSON parameter");
				return;
			}

			JObject obj = JObject.Parse (context.Request ["JSON"]);

			string method = (string)obj ["method"];

			if (string.IsNullOrEmpty (method)) {
				context.Response.Write ("Need to have a method of list, delete, or create");
				return;
			}

			if (method == "list") {
				List<JObject> users = ListUsers();

				foreach (JObject user in users) 
					context.Response.Write(user.ToString());
				

			} else if (method == "create") {
				string username = (string)obj["username"];
				string password = (string)obj["password"];

				if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
					context.Response.Write("Need a username and a password");
					return;
				}

				bool success = CreateUser(username, password);

				context.Response.Write("{ success : " + success.ToString() + " }");
				return;
			} else if (method == "delete") {
				string username = (string)obj["username"];

				if (string.IsNullOrEmpty(username)) {
					context.Response.Write("Need a username");
					return;
				}

				bool success = DeleteUser(username);

				context.Response.Write("{ success : " + success.ToString() + " }");
				return;
			} else {
				context.Response.Write("Don't recognize: " + method + ". Need list, create, or delete.");
				return;
			}
		}

		private bool CreateUser (string username, string password)
		{
			NpgsqlConnection conn = new NpgsqlConnection (_connstr);

			try {
				conn.Open ();

				NpgsqlCommand cmd = new NpgsqlCommand ("INSERT INTO USERS (USERNAME, PASSWORD) VALUES ('" + username + "', '" + password + "');", conn);
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

		private List<JObject> ListUsers ()
		{

			NpgsqlConnection conn = new NpgsqlConnection (_connstr);
			List<JObject> users = new List<JObject> ();

			try {
				conn.Open ();

			 
				NpgsqlCommand cmd = new NpgsqlCommand ("SELECT * FROM USERS;", conn);
				NpgsqlDataReader rdr = cmd.ExecuteReader ();


				while (rdr.Read()) {
					string json = "{ username : \"" + (string)rdr [0] + "\", password : \"" + (string)rdr [1] + "\" }";
					JObject obj = JObject.Parse (json);
					users.Add (obj);
				}

			} finally {
				conn.Close ();
			}

			return users;
		}
	}
}

