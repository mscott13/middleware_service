<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="_000.Notifications" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <hr />
        <asp:ScriptManager ID="scriptMgr" runat="server"></asp:ScriptManager>
         <h3>Notifications</h3>
        <asp:UpdatePanel ID="tableUpdatePanel" runat="server">
            <ContentTemplate>
                <asp:GridView ID="grdNotifications" runat="server" CssClass="table table-bordered" HeaderStyle-CssClass="header" RowStyle-CssClass="Rows" OnSelectedIndexChanged="grdNotifications_SelectedIndexChanged"></asp:GridView>
                <asp:Timer ID="Timer1" runat="server" Interval="1000" OnTick="Timer1_Tick"></asp:Timer>
                <asp:Label ID="dateLabel" runat="server" Text=""></asp:Label>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
            </Triggers>
        </asp:UpdatePanel>
        
        
    </div>
</asp:Content>


