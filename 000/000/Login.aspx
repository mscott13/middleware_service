<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="_000.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div>
            <h1>Log in</h1>
            <hr />
            <div class="login_form">
                <div class="s1">
                      <asp:Label ID="lbl_usr" runat="server" Text="Username"></asp:Label>
                <asp:TextBox ID="txtUsr" runat="server" CssClass="form-control"></asp:TextBox>

                </div>
              
                <div class="s1">
                    <asp:Label ID="lbl_psw" runat="server" Text="Password"></asp:Label>
                     <asp:TextBox ID="txtPsw" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                </div>
                <asp:Button ID="Button1" runat="server" Text="Login" class="btn btn-default" OnClick="btnLogin_Click"/>
               
                Not a user? Register <a href="Register.aspx">here.</a>
                <hr />
                
                     <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>