<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Console.aspx.cs" Inherits="Interface.Console" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="grid.css" rel="stylesheet" />
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link href="jquery.mCustomScrollbar.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="gridTop">
             <div class="logout">
                <asp:Button ID="btnLogout" runat="server" CssClass="logoutBtn" Text="Main" OnClick="btnLogout_Click" />
            </div>
        </div>
        <div class="admin-panel">
            <div class="dash">
                <div class="top">
                    <div class="t1">
                        <h2>0</h2>
                        <h3>Total Users</h3>
                    </div>
                    <div class="t2">
                        <div class="icon-monitor">
                            <img id="mon-stat" src="GridImages/panel-offline.png" />
                        </div>
                        <h2>Status</h2>
                    </div>
                    <div class="t3">
                    </div>
                </div>

                <div class="admin-main-content">
                    <div class="users">
                        <asp:GridView ID="grdUsers" runat="server" CssClass="table table-bordered table-striped"></asp:GridView>
                    </div>
                    <div class="admin-functions">
                        <a href="#">
                            <div class="start-stop">
                                <div class="left-stat">
                                </div>
                                <div class="right-stat">
                                    <h2></h2>
                                    <div class="foot-info">
                                        <p></p>
                                    </div>
                                </div>

                            </div>
                        </a>

                        <a href="#">


                            <div class="hide-show">
                                <div class="left-stat">
                                </div>
                                <div class="right-stat">
                                    <h2></h2>
                                    <div class="foot-info">
                                        <p></p>
                                    </div>
                                </div>

                            </div>
                        </a>

                        <a href="/ResetPassword.aspx">
                            <div class="reset-password">
                                <h2>Reset Password</h2>
                            </div>
                        </a>

                        <a href="/DeleteUser.aspx">
                            <div class="delete-user">
                                <h2>Delete User</h2>
                            </div>
                        </a>

                    </div>
                </div>
                 <div class="chg-psw">
              <a href="/RegisterAdmin.aspx">Register Admin</a>
            </div>
            </div>
        </div>
    </form>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="scripts/jquery.mCustomScrollbar.concat.min.js"></script>
    <script src="scripts/bootstrap.min.js"></script>

    <script type="text/javascript">
        var main = function () {

            $(".users").mCustomScrollbar({
                theme: "minimal-dark"
            });
            $(".users").mCustomScrollbar();

            function maintainList(max, len) {
                var distance = len - max;
                var startPos = len - 1;
                var endPos = startPos - distance;

                var i = 0;
                if (len > max) {
                    for (var i = startPos; i > endPos; i--) {
                        $('.t3 log').eq(i).remove();
                    }
                }
            }

            function shouldPrepend(data) {

                var right = String(data.id);
                var left;
                var canPrepend = true;
                var lenx = $('.t3 .log');
                maintainList(4, lenx.length);

                $(lenx).each(function () {

                    left = $(this).find(".id p").text();
                    if (left === right) {

                        canPrepend = false;
                        console.log("Can prepend false");
                    }
                    else {
                        if (canPrepend != false) {
                            canPrepend = true;
                            console.log("Can prepend true");
                        }
                    }
                });
                return canPrepend;
            }

            function sendStopMsg() {
                $.ajax({
                    url: 'http://erp-srvr.sma.gov.jm:8080/integrationservice.asmx/SendMessage',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ msg: "0x69" }),
                    dataType: 'json',
                    success: function (data) {

                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function sendStartMsg() {
                $.ajax({
                    url: 'http://erp-srvr.sma.gov.jm:8080/integrationservice.asmx/SendMessage',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ msg: "0x63" }),
                    dataType: 'json',
                    success: function (data) {

                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function sendHideMsg() {
                $.ajax({
                    url: 'http://erp-srvr.sma.gov.jm:8080/integrationservice.asmx/SendMessage',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ msg: "0x68" }),
                    dataType: 'json',
                    success: function (data) {

                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function sendShowMsg() {
                $.ajax({
                    url: 'http://erp-srvr.sma.gov.jm:8080/integrationservice.asmx/SendMessage',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ msg: "0x65" }),
                    dataType: 'json',
                    success: function (data) {

                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function getUserCount() {
                $.ajax({
                    url: 'http://erp-srvr.sma.gov.jm:8080/integrationservice.asmx/GetUserCount',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {
                        $('.t1 h2').text(data.d);
                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function GetLogMessages() {
                $.ajax({
                    url: 'http://erp-srvr.sma.gov.jm:8080/IntegrationService.asmx/GetLog',
                    type: 'post',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {

                        if (data.d.length > 0) {
                            sendLogMessage(data);
                        }
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }

            function isOnline() {
                $.ajax({
                    url: 'http://erp-srvr.sma.gov.jm:8080/integrationservice.asmx/GetMonStat',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {
                        if (data.d == -1) {

                            $('.start-stop .right-stat h2').text('Offline');
                            $('.start-stop .right-stat .foot-info p').text('Monitor not running');
                            $('.start-stop .left-stat').css({ 'background-color': '#E63946' });
                            $('#mon-stat').attr('src', '/GridImages/panel-offline.png');

                            $('.hide-show .right-stat h2').text('Monitor Offline');
                            $('.hide-show .right-stat .foot-info p').text('Offline');

                        }
                        else if (data.d == 2 || data.d == 21) {

                            $('.start-stop .right-stat h2').text('Monitor running');
                            $('.start-stop .right-stat .foot-info p').text('Service not started');
                            $('.start-stop .left-stat').css({ 'background-color': '#F0A830' });
                            $('#mon-stat').attr('src', '/GridImages/panel-startserv.png');

                            if (data.d == 21) {
                                $('.hide-show .right-stat h2').text('Show Interface');
                                $('.hide-show .right-stat .foot-info p').text('Hidden');
                            }
                            else {
                                $('.hide-show .right-stat h2').text('Hide Interface');
                                $('.hide-show .right-stat .foot-info p').text('Visible');
                            }

                        }
                        else if (data.d == 3 || data.d == 31) {

                            $('.start-stop .right-stat h2').text('Monitor running');
                            $('.start-stop .right-stat .foot-info p').text('Service started');
                            $('.start-stop .left-stat').css({ 'background-color': '#02C39A' });
                            $('#mon-stat').attr('src', '/GridImages/panel-online.png');

                            if (data.d == 31) {
                                $('.hide-show .right-stat h2').text('Show Interface');
                                $('.hide-show .right-stat .foot-info p').text('Hidden');
                            }
                            else {
                                $('.hide-show .right-stat h2').text('Hide Interface');
                                $('.hide-show .right-stat .foot-info p').text('Visible');
                            }
                        }

                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }


            function sendLogMessage(data) {

                var max = 25;
                var len = $('.t3 .log');
                if (len.length == 0) {

                    for (i = 0; i < data.d.length; i++) {

                        var div = '<div class="log">' +
                                   '<div class="timestamp"> <p>' + data.d[i].formattedDate + '</p></div>' +
                                    '<div class="log-content"> <p>' + data.d[i].msg + '</p></div>' +
                                    '<div class="id"> <p>' + data.d[i].id + '</p></div>' +
                                  '</div>';

                        $(div).appendTo('.t3').hide().slideDown(700, 'swing', function () { });
                    }

                }
                else {
                    tryPrepend(data, max);
                }
            }


            function tryPrepend(data, max) {

                for (var x = data.d.length - 1; x >= 0; x--) {

                    if (shouldPrepend(data.d[x]) == true) {
                        console.log("should prepend");

                        var send = '<div class="log">' +
                                       '<div class="timestamp"> <p>' + data.d[x].formattedDate + '</p></div>' +
                                        '<div class="log-content"> <p>' + data.d[x].msg + '</p></div>' +
                                        '<div class="id"> <p>' + data.d[x].id + '</p></div>' +
                                      '</div>';


                        var len_ = $('.t3 .log');

                        var invId1 = $(len_).eq(0).find(".id p").text();
                        var invId2 = data.d[x].formattedDate;

                        if (invId2 < invId1) {
                            appendIt = true;
                            console.log("append true");
                        }
                        else {
                            appendIt = false;
                            console.log("append false");
                        }


                        if (appendIt == true) {
                            $(send).appendTo('.t3').hide().slideDown(700, "swing", function () { });
                            appendIt = false;
                            console.log("appending");
                        }
                        else {
                            $(send).prependTo('.t3').hide().slideDown(700, "swing", function () { });

                            console.log("prepending");
                        }

                    }
                    else {
                        console.log("Should not prepend")
                    }
                }

                $("._content-pending").mCustomScrollbar({
                    theme: "minimal-dark"
                });
                $("._content-pending").mCustomScrollbar();

            }

            $('.start-stop').click(function () {
                var status = $('.start-stop .right-stat .foot-info p').text()
                if (status == 'Service started') {
                    sendStopMsg();
                }
                else if (status == 'Service not started') {
                    sendStartMsg();
                }
            });

            $('.hide-show').click(function () {
                var status = $('.hide-show .right-stat .foot-info p').text()
                if (status == 'Visible') {
                    sendHideMsg();
                }
                else if (status == 'Hidden') {
                    sendShowMsg();
                }
            });


            function Update() {
                isOnline();
                getUserCount();
                GetLogMessages();
            }




            Update();
            window.setInterval(function () {
                Update();
            }, 1000);
        }

        $(document).ready(main);

    </script>

</body>
</html>
