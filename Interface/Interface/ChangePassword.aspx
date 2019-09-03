<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Interface.ChangePassword" %>

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
                <div class="login-form">
                    <div class="icon-login">
                        <img src="GridImages/user.png" />
                    </div>
                    <div class="login-title">
                        <h2>Change Password</h2>
                    </div>
                    <div class="login-controls">
                        <h3>New Password</h3>
                        <asp:TextBox ID="txtPsw" CssClass="txt" runat="server" TextMode="Password"></asp:TextBox>

                        <h3>Confirm Password</h3>
                        <asp:TextBox ID="txtCPsw" CssClass="txt" runat="server" TextMode="Password"></asp:TextBox>

                        <asp:Button ID="btnLogIn" runat="server" CssClass="btnLogIn" Text="Confirm" OnClick="btnLogIn_Click"/>
                        <div class="link">
                            <a href="/MenuGrid.aspx">Main Menu</a>
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
