<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="Interface.SignUp" %>

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
            <div class="signup-form-position">
                <div class="message">
                    <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                </div>
                <div class="signUp-form">
                    <div class="icon-login">
                        <img src="GridImages/user.png" />
                    </div>
                    <div class="login-title">
                        <h2>Sign Up</h2>
                    </div>
                    <div class="login-controls">
                        <h3>Username</h3>
                        <asp:TextBox ID="txtUsr" CssClass="txt" runat="server"></asp:TextBox>

                        <h3>Password</h3>
                        <asp:TextBox ID="txtPsw" CssClass="txt" runat="server" TextMode="Password"></asp:TextBox>

                        <h3>Confirm Password</h3>
                        <asp:TextBox ID="txtPsw2" CssClass="txt" runat="server" TextMode="Password"></asp:TextBox>

                        <asp:Button ID="btnLogIn" runat="server" CssClass="btnLogIn" Text="Sign Up" OnClick="btnLogIn_Click" />
                        <div class="link">
                            <a href="/login.aspx">Login here</a>
                        </div>
                    </div>
                    <div class="overlay">
                        <div class="login-form">
                            <div class="icon-login">
                                <img src="GridImages/user.png" />
                            </div>
                            <div class="login-title">
                                <h2>Member Login</h2>
                            </div>
                            <div class="login-controls">
                                <h3>Username</h3>
                                <asp:TextBox ID="txtUname" CssClass="txt" runat="server"></asp:TextBox>

                                <h3>Password</h3>
                                <asp:TextBox ID="txtPsw_" CssClass="txt" runat="server" TextMode="Password"></asp:TextBox>

                                <asp:Button ID="Button1" runat="server" CssClass="btnLogIn" Text="Log In" OnClick="Button1_Click" />
                                <div class="link">
                                    <a href="/SignUp.aspx">Not a member? Sign up</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
