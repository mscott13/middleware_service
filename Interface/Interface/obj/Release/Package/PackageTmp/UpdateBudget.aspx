<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UpdateBudget.aspx.cs" Inherits="_000.UpdateBudget" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <br />
    <asp:label runat="server" id="lblError"></asp:label>
    <br />
    <asp:TextBox ID="txtCcnum" placeholder="Customer Number" runat="server"></asp:TextBox>
    <p>
    </p>
    <asp:TextBox ID="txtBudget" placeholder="New Budget For Customer" runat="server"></asp:TextBox>
    <br />
    <asp:button runat="server" id="btnSubmit" text="Save Changes" OnClick="btnSubmit_Click" />
</asp:Content>
