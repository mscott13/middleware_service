<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DeferredIncome.aspx.cs" Inherits="_000.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="gridTop">
              <div class="bar-title">
            <div class="left">
                <h2>Deffered Income Report</h2>
            </div>
           
        </div>
         <div class="logout">
                <asp:Button ID="btnLogout" runat="server" CssClass="logoutBtn" Text="Main" OnClick="btnLogout_Click" />
            </div>
  
    </div>
    <div class="containIt">
        <br /><br />
        <div class="message">
             <asp:Label ID="lblError" runat="server" Text="" ForeColor="Red"></asp:Label>
        </div>
        <div class="deferred-income">
            <div class="generateOptions_">
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
                </asp:DropDownList>
                <asp:Button ID="Button1" runat="server" Text="Generate Report" OnClick="Button1_Click" CssClass="btnLogIn" />
              
                 <asp:Button ID="Button2" runat="server" Text="View Deferred Income Report (PDF)" OnClick="btnDeferred_Click" CssClass="btnLogIn" />

                
            </div>

            <div class="update-budget">
              
                <asp:Label runat="server" ID="Label1"></asp:Label>
               
                <asp:dropdownlist ID="ddlccnum" runat="server" DataSourceID="SqlDataSource1" DataTextField="invoice_id" DataValueField="invoice_id"></asp:dropdownlist>
               
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:Integrationn %>" SelectCommand="SELECT [invoice_id] FROM [DeferredBudget] order by invoice_id desc"></asp:SqlDataSource>
               
                <asp:TextBox ID="txtBudget" placeholder="New Budget For Invoice" runat="server"  CssClass="txt_"></asp:TextBox>
               
                <asp:Button runat="server" ID="btnSubmit" Text="Update Budget" OnClick="btnSubmit_Click" CssClass="btnLogIn"/>
                 <asp:Button ID="Button3" runat="server" Text="View Deferred Income Totals Report (PDF)" OnClick="btnDeferred_Totals" CssClass="btnLogIn" />
            </div>
        </div>

        <br />
        <br /><br />
        <br /><br />
        <br />
        <asp:GridView ID="GridView1" runat="server" Width="1600px" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
            <AlternatingRowStyle BackColor="#FFCCCC" BorderColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
            <EditRowStyle BackColor="#99CCFF" BorderColor="#FF9966" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#0099FF" BorderColor="#FF0066" ForeColor="Black" />
            <RowStyle BackColor="#FFFFCC" BorderColor="#CC3399" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>
        <br />
        <br />



        <br />

        <center> <asp:Label ID="CellLabel" runat="server" Text="Cellular"></asp:Label> </center>
        <br />
        <asp:GridView ID="CellGrid" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#3399FF" />
            <RowStyle BackColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>

        <br />
        <hr style="border: none; height: 1px; color: #E3E8EA; background-color: #E3E8EA" />
        <center> <asp:Label ID="BBLabel" runat="server" Text="P/R Commercial (Broadband)"></asp:Label> </center>
        <br />
        <asp:GridView ID="BBGrid" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#3399FF" />
            <RowStyle BackColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>

        <br />
        <hr style="border: none; height: 1px; color: #E3E8EA; background-color: #E3E8EA" />
        <center> <asp:Label ID="AeroLbl" runat="server" Text="P/R - Aeronautical"></asp:Label> </center>
        <br />

        <asp:GridView ID="AeroGrid" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#3399FF" />
            <RowStyle BackColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>

        <br />
        <hr style="border: none; height: 1px; color: #E3E8EA; background-color: #E3E8EA" />

        <center> <asp:Label ID="VsatLbl" runat="server" Text="Vsat"></asp:Label> </center>
        <br />
        <asp:GridView ID="VsatGrid" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#3399FF" />
            <RowStyle BackColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>

        <br />
        <hr style="border: none; height: 1px; color: #E3E8EA; background-color: #E3E8EA" />
        <center> <asp:Label ID="MarineLbl" runat="server" Text="P/R - Marine"></asp:Label> </center>
        <br />
        <asp:GridView ID="MarineGrid" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#3399FF" />
            <RowStyle BackColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>

        <br />

        <hr style="border: none; height: 1px; color: #E3E8EA; background-color: #E3E8EA" />

        <center> <asp:Label ID="dsLabel" runat="server" Text="P/R Commercial (Data & Services)"></asp:Label> </center>
        <br />

        <asp:GridView ID="DServicesGrid" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#3399FF" />
            <RowStyle BackColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>

        <br />
        <hr style="border: none; height: 1px; color: #E3E8EA; background-color: #E3E8EA" />

        <center> <asp:Label ID="trunklbl" runat="server" Text="P/R - Trunking"></asp:Label> </center>
        <br />
        <asp:GridView ID="TrunkGrid" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#3399FF" />
            <RowStyle BackColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>

        <br />
        <hr style="border: none; height: 1px; color: #E3E8EA; background-color: #E3E8EA" />


        <center> <asp:Label ID="lblMicro" runat="server" Text="P/R Commercial (Microwave)"></asp:Label> </center>
        <br />

        <asp:GridView ID="MicroGrid" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#3399FF" />
            <RowStyle BackColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>
        <br />
        <hr style="border: none; height: 1px; color: #E3E8EA; background-color: #E3E8EA" />

        <center> <asp:Label ID="lblOther" runat="server" Text="Other P/R Non-Commercial Clients"></asp:Label> </center>
        <br />

        <asp:GridView ID="OtherGrid" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#3399FF" />
            <RowStyle BackColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>

        <br />
        <hr style="border: none; height: 1px; color: #E3E8EA; background-color: #E3E8EA" />
        <asp:GridView ID="GridView2" runat="server">
            <AlternatingRowStyle BackColor="#FFFF99" Font-Italic="True" Font-Names="Century Gothic" />
            <HeaderStyle BackColor="#3399FF" />
            <RowStyle BackColor="#FFCC99" Font-Italic="True" Font-Names="Century Gothic" />
        </asp:GridView>
        <br />
    </div>
     <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
     <script>
        var main = function () {
            function DisplayError() {
                $('.message').slideDown(700, "swing", function () { });
            }
        }

        $(document).ready(main);
    </script>
</asp:Content>
