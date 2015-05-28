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
		xhr.open('post', 'Vulnerable.ashx', false);
		xhr.setRequestHeader('Content-Type', 'application/json; charset=UTF-8');

		// send the collected data as JSON
		xhr.send(JSON.stringify(data));
	}

	function listUsers() {
		var data = {
			username: document.getElementById('txtUserList').value,
			method: 'list'
		};

		// construct an HTTP request
		var xhr = new XMLHttpRequest();
		xhr.open('post', 'Vulnerable.ashx', false);
		xhr.setRequestHeader('Content-Type', 'application/json; charset=UTF-8');

		// send the collected data as JSON
		xhr.send(JSON.stringify(data));
		var response = JSON.parse(xhr.responseText);

		var html = '';
		for (i = 0; i < response.length; i++){
			html += "<div>"+response[i]["username"]+"&nbsp;<input type='submit' value='Delete User' onclick=\"deleteUser('"+response[i]["username"]+"');return false;\" /></div>";
		}

		var div = document.getElementById('divUsers');
		div.innerHTML = html;
	}

	function deleteUser(username) {
		var data = {
			username: username,
			method: 'delete'
		};

		// construct an HTTP request
		var xhr = new XMLHttpRequest();
		xhr.open('post', 'Vulnerable.ashx', false);
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
			<div>Age</div>
			<div><asp:TextBox id="txtAge" runat="server" /></div>
			<br />
			<div>Address Line 1</div>
			<div><asp:TextBox id="txtLine1" runat="server" /></div>
			<br />
			<div>Address Line 2</div>
			<div><asp:TextBox id="txtLine2" runat="server" /></div>
			<br />
			<div>City</div>
			<div><asp:TextBox id="txtCity" runat="server" /></div>
			<br />
			<div>State</div>
			<div><asp:TextBox id="txtState" runat="server" /></div>
			<br />
			<div>ZIP</div>
			<div><asp:TextBox id="txtZIP" runat="server" /></div>
			<br />
			<div>First Name</div>
			<div><asp:TextBox id="txtFirst" runat="server" /></div>
			<br />
			<div>Middle Name</div>
			<div><asp:TextBox id="txtMiddleName" runat="server" /></div>
			<br />
			<div>Last Name</div>
			<div><asp:TextBox id="txtLastName" runat="server" /></div>
			<br />
			<asp:Button id="btnSubmitNewUser" Text="Create User" runat="server" OnClientClick="createNewUser(); return false;" />
		</div>
		<div>
			<div>List Users</div>
			<div><asp:TextBox runat="server" id="txtUserList" /></div>
			<asp:Button id="btnListUsers" Text="List User" runat="server" OnClientClick="listUsers(); return false;" />
			<div id="divUsers">
			</div>
		</div>
	</form>
</body>
</html>

