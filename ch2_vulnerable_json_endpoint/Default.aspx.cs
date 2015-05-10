using System;
using System.Web;
using System.Web.UI;

namespace ch2_vulnerable_json_endpoint
{
	
	public partial class Default : System.Web.UI.Page
	{
		public void button1Clicked (object sender, EventArgs args)
		{
			button1.Text = "You clicked me";
		}
	}
}

