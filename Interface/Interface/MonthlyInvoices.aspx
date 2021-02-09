<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MonthlyInvoices.aspx.cs" Inherits="_000.MonthlyInvoices" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="gridTop">
        <div class="bar-title">
            <div class="left">
                <h2>Monthly Invoices</h2>
            </div>

        </div>
        <div class="logout">
            <asp:Button ID="btnLogout" runat="server" CssClass="logoutBtn" Text="Main" OnClick="btnLogout_Click" />
        </div>
    </div>

    <div class="containIt">
        <div class="generateasp:ListItems">
            <asp:DropDownList ID="ddl1" runat="server">
                <asp:ListItem Selected="True" Value="1">January</asp:ListItem>
                <asp:ListItem Value="2">February</asp:ListItem>
                <asp:ListItem Value="3">March</asp:ListItem>
                <asp:ListItem Value="4">April</asp:ListItem>
                <asp:ListItem Value="5">May</asp:ListItem>
                <asp:ListItem Value="6">June</asp:ListItem>
                <asp:ListItem Value="7">July</asp:ListItem>
                <asp:ListItem Value="8">August</asp:ListItem>
                <asp:ListItem Value="9">September</asp:ListItem>
                <asp:ListItem Value="10">October</asp:ListItem>
                <asp:ListItem Value="11">November</asp:ListItem>
                <asp:ListItem Value="12">December</asp:ListItem>
            </asp:DropDownList>

            <asp:DropDownList ID="DropDownList1" runat="server">
                <asp:ListItem>2016</asp:ListItem>
                <asp:ListItem>2017</asp:ListItem>
                <asp:ListItem>2018</asp:ListItem>
                <asp:ListItem>2019</asp:ListItem>
                <asp:ListItem>2020</asp:ListItem>
                <asp:ListItem>2021</asp:ListItem>
                <asp:ListItem>2022</asp:ListItem>
                <asp:ListItem>2023</asp:ListItem>
                <asp:ListItem>2024</asp:ListItem>
                <asp:ListItem>2025</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnPayments" runat="server" Text="Generate" OnClick="GetInvoices" CssClass="btnLogIn" />
            <br />
        </div>

        <br />
        <br />
        <br />
        <br />
        <br />

        <asp:GridView ID="GridView1" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Names="AR CENA" />
            <HeaderStyle BackColor="#3399FF" Font-Names="AR CENA" />
            <RowStyle BackColor="#FFCC99" Font-Names="AR CENA" />
        </asp:GridView>
        <br />
       <center>
        <asp:Label ID="lblinvTotal" runat="server" Text="" Font-Bold="true"></asp:Label>
        <asp:Label ID="lblinvtot" runat="server" Font-Bold="true"></asp:Label>
           </center>
        <br />
        <br />
        <br />

        <asp:GridView ID="GridView2" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Names="AR CENA" />
            <HeaderStyle BackColor="#3399FF" Font-Names="AR CENA" />
            <RowStyle BackColor="#FFCC99" Font-Names="AR CENA" />
        </asp:GridView>
        <br />
        <center>
        <asp:Label ID="lblspecdesc" runat="server" Text="" Font-Bold="true"></asp:Label>
        <asp:Label ID="lblspecamt" runat="server" Font-Bold="true"></asp:Label>
            </center>
        <br />
        <br />
        
        <asp:GridView ID="GridView3" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Names="AR CENA" />
            <HeaderStyle BackColor="#3399FF" Font-Names="AR CENA" />
            <RowStyle BackColor="#FFCC99" Font-Names="AR CENA" />
        </asp:GridView>
        <br />
        <center>
        <asp:Label ID="lblTADesc" runat="server" Text="" Font-Bold="true"></asp:Label>
        <asp:Label ID="lblTAamt" runat="server" Font-Bold="true"></asp:Label>
       <br />
            <br />
        <asp:Label ID="lblOverview" runat="server" Text="" Font-Bold="true"></asp:Label>
          <br />
            <br />
        <asp:Label ID="lblOverviewdesc1" runat="server" Font-Bold="true" Text=""></asp:Label>
        <asp:Label ID="lblnuminvoices" runat="server" Font-Bold="true"></asp:Label>
        <br />
        <br />
        <asp:Label ID="lblOverviewdesc2" runat="server" Text="" Font-Bold="true"></asp:Label>
        <asp:Label ID="lbltotinv" runat="server" Font-Bold="true"></asp:Label>
            </center>
        <br />
    </div>
</asp:Content>
