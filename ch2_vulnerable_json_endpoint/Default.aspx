<%@ Page Language="C#" Inherits="ch2_vulnerable_json_endpoint.Default" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title>Default</title>
</head>
<body>
	<script>
	function createNewUser(){
	 var data = {
	 username: document.getElementById('txtUsername').value,
	 password: document.getElementById('txtPassword').value,
	 method: 'create'
	 };

  // construct an HTTP request
  var xhr = new XMLHttpRequest();
  xhr.open('post', 'Vulnerable.ashx', true);
  xhr.setRequestHeader('Content-Type', 'application/json; charset=UTF-8');

  // send the collected data as JSON
  xhr.send(JSON.stringify(data));

	}
	</script>
	<form id="form1" runat="server">
		<div>
			<div>Username</div>
			<div><asp:TextBox id="txtUsername" runat="server" /></div>
			<br />
			<div>Password</div>
			<div><asp:TextBox id="txtPassword" runat="server" /></div>
			<br />
			<asp:Button id="btnSubmitNewUser" runat="server" OnClientClick="createNewUser(); return false;" />
		</div>
	</form>
</body>
</html>

