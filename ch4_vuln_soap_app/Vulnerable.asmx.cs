
using System;
using System.Data;
using System.Web;
using System.Web.Services;
using Npgsql;
using System.Collections.Generic;

namespace SQLInjectionSOAPService
{
	public class VulnerableService : System.Web.Services.WebService
	{
		[WebMethod]
		public string AddUser (string username, string password)
		{
			NpgsqlConnection conn = new NpgsqlConnection ("Server=127.0.0.1;Port=5432;User Id=postgres;Password=secret;Database=vulnerable;");
			conn.Open ();


			NpgsqlCommand command = new NpgsqlCommand ("insert into users values('" + username + "' , '" + password + "');", conn);
			command.ExecuteNonQuery ();

			conn.Close ();
  
			return "Excellent!";
		}

		[WebMethod]
		public List<string> ListUsers ()
		{		   
			List<string> users = new List<string> ();
			NpgsqlConnection conn = new NpgsqlConnection ("Server=127.0.0.1;Port=5432;User Id=postgres;Password=secret;Database=vulnerable;");
			conn.Open ();

			NpgsqlCommand command = new NpgsqlCommand ("select * from users;", conn);	
			NpgsqlDataReader dr = command.ExecuteReader ();
			while (dr.Read()) {
				for (int i = 0; i < dr.FieldCount; i+=2) {
					users.Add ((string)dr [i] + ":" + (string)dr [i + 1]);
				}
			}
		
			conn.Close ();
  
			return users;
		}

		[WebMethod]
		public string GetUser (string username)
		{
NpgsqlConnection conn = new NpgsqlConnection ("Server=127.0.0.1;Port=5432;User Id=postgres;Password=secret;Database=vulnerable;");
			conn.Open ();

			NpgsqlCommand command = new NpgsqlCommand ("select * from users where username = '" + username + "';", conn);	
			NpgsqlDataReader dr = command.ExecuteReader ();
			dr.Read();
		
			conn.Close ();
  			
			if (dr.HasRows) 
				return (string)dr[0] + ":" + (string)dr[1];
			else 
				return "User not found";
		}

		[WebMethod]
		public bool DeleteUser (string username)
		{
			NpgsqlConnection conn = new NpgsqlConnection ("Server=127.0.0.1;Port=5432;User Id=postgres;Password=secret;Database=vulnerable;");
			conn.Open ();


			NpgsqlCommand command = new NpgsqlCommand ("delete from users where username = '" + username + "';", conn);
			int rows = command.ExecuteNonQuery ();

			conn.Close ();

			return (rows > 0);
		}
	}
}

