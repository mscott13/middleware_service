<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeleteUser.aspx.cs" Inherits="Interface.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
      <link href="grid.css" rel="stylesheet" />
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
     <div class="login-content">
            <div class="login-form-position">
                <div class="message">
                    <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                </div>
                <div class="login-form_2">
                    <div class="icon-login">
                        <img src="GridImages/user.png" />
                    </div>
                    <div class="login-title">
                        <h2>Delete User</h2>
                    </div>
                    <div class="login-controls">
                        <h3>Username</h3>
                        <asp:TextBox ID="txtUsr" CssClass="txt" runat="server"></asp:TextBox>

                        <asp:Button ID="btnLogIn" runat="server" CssClass="btnLogIn" Text="Delete User" OnClick="btnLogIn_Click"/>
                        <div class="link">
                            <a href="/Console.aspx">Console</a>
                        </div>
                    </div>

                    <div class="overlay">
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
