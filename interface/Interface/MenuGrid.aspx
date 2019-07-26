<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuGrid.aspx.cs" Inherits="Interface.MenuGrid" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="grid.css" rel="stylesheet" />
</head>
<body>

    <form id="form1" runat="server">
        <div class="gridTop">
           
            <div class="logout">
                <asp:Button ID="btnLogout" runat="server" CssClass="logoutBtn" Text="Log Out" OnClick="btnLogout_Click" />
            </div>
        </div>
        <div class="gridContent">
            <div class="menu">
                <a href="/MonthlyDeferredIncome.html">
                    <div class="s1">
                        <div class="icon">
                            <img src="GridImages/report.png" />
                        </div>
                        <div class="title">
                            <h2>DEFERRED INCOME REPORT</h2>
                        </div>
                        <div class="info">
                            <p>Generate deferred reports for the current month. Choose the month then run the reporting tool.</p>
                        </div>
                    </div>
                </a>
                <a href="/Notifications.aspx">
                    <div class="s2">
                        <div class="icon">
                            <img src="GridImages/envelope.png" />
                        </div>
                        <div class="title">
                            <h2>NOTIFICATIONS</h2>
                        </div>
                        <div class="info">
                            <p>See the most recent updates of Invoices and Receipts.</p>
                        </div>
                    </div>
                </a>
                <a href="/MonthlyPayments.aspx">
                    <div class="s3">
                        <div class="icon">
                            <img src="GridImages/check.png" />
                        </div>
                        <div class="title">
                            <h2>MONTHLY PAYMENTS</h2>
                        </div>
                        <div class="info">
                            <p>View all payments transferred within a specified current month</p>
                        </div>
                    </div>
                </a>
                <a href="/MonthlyInvoices.aspx">
                    <div class="s4">
                        <div class="icon">
                            <img src="GridImages/invoice.png" />
                        </div>
                        <div class="title">
                            <h2>MONTHLY INVOICES</h2>
                        </div>
                        <div class="info">
                            <p>View all invoices transferred within a specified month</p>
                        </div>
                    </div>
                </a>
                <a href="/MonthlyCustomers.aspx">
                    <div class="s5">
                        <div class="icon">
                            <img src="GridImages/team.png" />
                        </div>
                        <div class="title">
                            <h2>CREATED CUSTOMERS</h2>
                        </div>
                        <div class="info">
                            <p>View newly created customers for a specific month</p>
                        </div>
                    </div>
                </a>
                <a href="/Console.aspx">
                    <div class="s6">
                        <div class="icon">
                            <img src="GridImages/wrench.png" />
                        </div>
                        <div class="title">
                            <h2>Admin Console</h2>
                        </div>
                        <div class="info">
                            <p>Provides access to elevated functions</p>
                        </div>
                    </div>
                </a>
            </div>
             <div class="chg-psw">
              <a href="/ChangePassword.aspx">Change Password</a>
            </div>
        </div>
    </form>
</body>
</html>
