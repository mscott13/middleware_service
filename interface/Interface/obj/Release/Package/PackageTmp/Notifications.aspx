<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="Interface.Notifications" %>--%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="style.css" rel="stylesheet" />
    <link href="jquery.mCustomScrollbar.css" rel="stylesheet" />
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">

        <div class="head-bar">
            <div class="item">
                <h2>Notifications</h2>
            </div>
            <div class="information">

                <div class="rate">
                    <h3 class="xrate"></h3>
                </div>
            </div>
            <div id="stat">
                <img src="Images/load.png" width="16" height="16" draggable="false" />
            </div>
        </div>

        <div class="Dashboard">

            <div class="dContain">
                <div class="d" id="d1">
                    <div class="front">
                        <div class="imgc">
                            <img src="Images/invoice.png" draggable="false" />
                        </div>
                        <div class="number">
                            <h2>0</h2>
                        </div>
                        <div class="text">
                        </div>
                    </div>

                    <div class="back">
                        <div class="totals">
                             <div class="color-strip"></div>
                            <div class="totals-inv">
                               
                                <div id="totals-contain1">
                                    <div id="renewal-spec-count" class="count-format">
                                        <p>-</p>
                                    </div>
                                    <div class="amount" id="renewal-spec-amount">
                                        <p>--</p>
                                    </div>
                                </div>
                                <div id="totals-contain2">
                                    <div id="renewal-reg-count" class="count-format">
                                        <p>-</p>
                                    </div>
                                    <div class="amount" id="renewal-reg-amount">
                                        <p>--</p>
                                    </div>
                                </div>
                                <div id="totals-contain3">
                                    <div id="maj-count" class="count-format">
                                        <p>-</p>
                                    </div>
                                    <div class="amount" id="maj-amount">
                                        <p>--</p>
                                    </div>
                                </div>
                                <div id="totals-contain4">
                                    <div id="nonmaj-count" class="count-format">
                                        <p>-</p>
                                    </div>
                                    <div class="amount" id="nonmaj-amount">
                                        <p>--</p>
                                    </div>
                                </div>
                                <div id="totals-contain5">
                                    <div id="typeapproval-count" class="count-format">
                                        <p>-</p>
                                    </div>
                                    <div class="amount" id="typeapproval-amount">
                                        <p>--</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="totals-desc">
                            <div id="desc-title-1" class="desc">
                                <p>RENEWAL SPEC</p>
                            </div>
                            <div id="desc-title-2" class="desc">
                                <p>RENEWAL REG</p>
                            </div>
                            <div id="desc-title-3" class="desc">
                                <p>MAJ</p>
                            </div>
                            <div id="desc-title-4" class="desc">
                                <p>NON MAJ</p>
                            </div>
                            <div id="desc-title-5" class="desc">
                                <p>TYPE APPROVAL</p>
                            </div>
                        </div>
                        <div class="month-total">
                             <div class="icon-pay-left">
                                <div class="b2-payIcon">
                                    <img src="Images/money-bag-2.png" draggable="false" />
                                </div>
                            </div>
                            <div class="right-total-2">
                                <h2>--</h2>
                                <div class="right-total-foot">
                                    <p>Total</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="overlay">
                    </div>
                </div>
            </div>

            <div class="dContain">
                <div class="d" id="d2">
                    <div class="front2">
                        <div class="imgc">
                            <img src="Images/receipt.png" draggable="false" />
                        </div>
                        <div class="number">
                            <h2>0</h2>
                        </div>
                        <div class="text">
                        </div>
                    </div>
                    <div class="back2">
                        <div class="total">

                            <div class="icon-pay-left">
                                <div class="b2-payIcon">
                                    <img src="Images/money-bag.png" draggable="false" />
                                </div>
                            </div>
                            <div class="right-total">
                                <h2></h2>
                                <div class="right-total-foot">
                                    <p>Total</p>
                                </div>
                            </div>
                            <div class="icon-bank-left">
                                <div class="b2-payIcon">
                                    <img src="Images/bank.png" draggable="false" />
                                </div>
                            </div>
                            <div class="bank-info">
                                <div class="bank1">
                                    <div class="bank-name">
                                        <h3>FGBJMREC</h3>
                                    </div>
                                    <div class="batch">
                                        <h3></h3>
                                    </div>
                                    <div class="count">
                                        <h3></h3>
                                    </div>
                                </div>
                                <div class="bank2">
                                    <div class="bank-name">
                                        <h3>FGBUSMRC</h3>
                                    </div>
                                    <div class="batch">
                                        <h3></h3>
                                    </div>
                                    <div class="count">
                                        <h3></h3>
                                    </div>
                                </div>
                                <div class="bank3">
                                    <div class="bank-name">
                                        <h3>NCBJMREC</h3>
                                    </div>
                                    <div class="batch">
                                        <h3></h3>
                                    </div>
                                    <div class="count">
                                        <h3></h3>
                                    </div>
                                </div>
                                 <div class="bank4">
                                    <div class="bank-name">
                                        <h3>NCBUSMR</h3>
                                    </div>
                                    <div class="batch">
                                        <h3></h3>
                                    </div>
                                    <div class="count">
                                        <h3></h3>
                                    </div>
                                </div>
                            </div>
                            <div class="pay-expiry">
                                <h3><span>
                                    <img src="Images/expire-calendar.png" draggable="false" style="width: 16px; height: 16px" /></span></h3>
                            </div>
                        </div>
                    </div>
                    <div class="overlay">
                    </div>
                </div>
            </div>

            <div class="dContain">
                <div class="d" id="d3">
                    <div class="front3">
                        <div class="imgc">
                            <img src="Images/user.png" draggable="false" />
                        </div>
                        <div class="number">
                            <h2>0</h2>
                        </div>
                        <div class="text">
                        </div>
                    </div>
                    <div class="back3">

                        <div class="number">
                            <h2>0</h2>
                        </div>
                        <div class="text">
                        </div>
                    </div>
                    <div class="overlay">
                    </div>
                </div>
            </div>

            <div class="dContain">
                <div class="d" id="d4">
                    <div class="front4">
                        <div class="imgc">
                            <img src="Images/safe-transaction.png" draggable="false" />
                        </div>
                        <div class="number">
                            <h2>0</h2>
                        </div>
                        <div class="text">
                        </div>
                    </div>
                    <div class="back4">
                        <div class="pending1">
                            <div class="clientid">
                                <h3></h3>
                            </div>
                            <div class="client-name">
                                <h3></h3>
                            </div>
                        </div>
                        <div class="pending2">
                            <div class="clientid">
                                <h3></h3>
                            </div>
                            <div class="client-name">
                                <h3></h3>
                            </div>
                        </div>
                        <div class="pending3">
                            <div class="clientid">
                                <h3></h3>
                            </div>
                            <div class="client-name">
                                <h3></h3>
                            </div>
                        </div>
                        <div class="pending4">
                            <div class="clientid">
                                <h3></h3>
                            </div>
                            <div class="client-name">
                                <h3></h3>
                            </div>
                        </div>
                        <div class="pending5">
                            <div class="clientid">
                                <h3></h3>
                            </div>
                            <div class="client-name">
                                <h3></h3>
                            </div>
                        </div>
                    </div>
                    <div class="overlay">
                    </div>
                </div>
            </div>

            <div class="messages">
                <div class="_title">
                    <div class="t1">
                        <h3>Transferred Invoices</h3>
                    </div>
                    <div class="t2">
                        <h3>Transferred Payments</h3>
                    </div>
                    <div class="t3">
                        <h3>Created Customers</h3>
                    </div>
                    <div class="t4">
                        <h3>Cancellations / Credit Notes</h3>
                    </div>
                </div>
                <div class="_content">
                    <div class="_content-invoice">
                    </div>
                    <div class="_content-payments">
                    </div>
                    <div class="_content-customers">
                    </div>
                    <div class="_content-memo_cancellation">
                    </div>
                </div>
            </div>
        </div>

    </form>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="scripts/jquery.mCustomScrollbar.concat.min.js"></script>

    <script type="text/javascript">
        var main = function () {


            var length_ = 0;
            var appendIt = false;


            $('#d1').click(function () {
                $('#d1').toggleClass('flipped');
            })

            $('#d2').click(function () {
                $('#d2').toggleClass('flipped');
            })

            function checkRemoveInvoice(invoiceId) {
                var index = 0;
                inv = $('._content ._content-pending .invoice-pending-detail');
                var inv = $('._content ._content-pending .invoice-pending-detail');
                $(inv).each(function () {

                    if ($(this).find(".quad1 .invoiceId").text() == invoiceId) {
                        inv.eq(index).slideUp(700, "swing", function () { $(this).remove(); });

                        inv = $('._content ._content-pending .invoice-pending-detail');

                    }
                    else {
                        index = index + 1;
                    }
                });
                length_ = inv.length;
            }


            function sendInvoiceMessage(data) {

                var max = 100
                var len = $('._content ._content-pending .invoice-pending-detail');
                if (len.length == 0) {

                    setTimeout(function () {
                        for (i = 0; i < data.d.length; i++) {
                            var div = '<div class="invoice-pending-detail">' +
                            '<div class="pending-col">' +
                            '</div>' +
                            '<div class="invoice-left">' +
                                '<div class="quad1">' +
                                    '<h1>' + data.d[i].clientName + '</h1>' +
                                    '<p>' + data.d[i].clientId + '</p>' + ' ' +
                                    '&middot;' + ' ' +
                                   '<p class="invoiceId">' + data.d[i].invoiceId + '</p>' +
                                    '<div class="trans-icon">' +
                                        '<img src="Images/diskette.png" />' +
                                    '</div>' +
                                '</div>' +
                                '<div class="quad2">' +
                                    '<p>' + data.d[i].formattedDate + '</p>' +
                                    '<p>' + data.d[i].Author + '</p>' +
                                '</div>' +
                                '<div class="quad3">' +
                                    '<p>' + data.d[i].amount + '</p>' +
                               '</div>' +
                            '</div>' +
                        '</div>'

                            $(div).appendTo('._content ._content-pending').hide().slideDown(700, 'swing', function () { });
                        }
                        $("._content-pending").mCustomScrollbar({
                            theme: "minimal-dark"
                        });
                        $("._content-pending").mCustomScrollbar();
                    }, 400);

                }
                else {
                    tryPrependPending(data, max);
                }
            }

            function sendPaymentMessage(data) {
                var max = 100
                var len = $('._content ._content-payments .invoice-payment-detail');
                if (len.length == 0) {
                    setTimeout(function () {
                        for (i = 0; i < data.d.length; i++) {

                            var div = '<div class="invoice-payment-detail" data-sequence="' + data.d[i].sequence + '">' +
                            '<div class="payment-col">' +
                            '</div>' +
                            '<div class="invoice-left">' +
                                '<div class="quad1">' +
                                    '<h1>' + data.d[i].clientName + '</h1>' +
                                    '<p>' + data.d[i].clientId + '</p>' + ' ' +
                                    '&middot;' + ' ' +
                                   '<p class="PinvoiceId">' + data.d[i].invoiceId + '</p>' +
                                    '<div class="trans-icon">' +
                                        '<img src="Images/transfer-icon.png" />' +
                                    '</div>' +
                                '</div>' +
                                '<div class="quad2">' +
                                    '<p>' + data.d[i].formattedDate + '</p>' +
                                '</div>' +
                                '<div class="quad3">' +
                                    '<p>' + data.d[i].amount + '</p>' +
                               '</div>' +
                            '</div>' +

                        '</div>'

                            $(div).appendTo('._content ._content-payments').hide().slideDown('slow', 'swing', function () { });
                        }

                        $("._content-payments").mCustomScrollbar({
                            theme: "minimal-dark"
                        });

                        $("._content-payments").mCustomScrollbar();
                    }, 200);
                }
                else {
                    tryPrependPayments(data, max);
                }
            }

            function sendCustomerMessage(data) {
                var max = 100
                var len = $('._content ._content-customers .customer-detail');
                if (len.length == 0) {

                    setTimeout(function () {
                        for (i = 0; i < data.d.length; i++) {
                            var div = '<div class="customer-detail">' +
                            '<div class="customer-col">' +
                            '</div>' +
                            '<div class="invoice-left">' +
                                '<div class="quad1">' +
                                    '<h1>' + data.d[i].Name + '</h1>' +
                                    '<p class="customer-id">' + data.d[i].ClientId + '</p>' + ' ' +
                                    ' ' + ' ' +
                                    '<div class="trans-icon">' +
                                        '<img src="Images/newcustomer.png" />' +
                                    '</div>' +
                                '</div>' +
                                '<div class="quad2">' +
                                    '<p>' + data.d[i].formattedDate + '</p>' +
                                '</div>' +
                                '<div class="quad3">' +
                               '</div>' +
                            '</div>' +
                        '</div>'

                            $(div).appendTo('._content ._content-customers').hide().slideDown('slow', 'swing', function () { });
                        }


                        $("._content-customers").mCustomScrollbar({
                            theme: "minimal-dark"
                        });

                        $("._content-customers").mCustomScrollbar();
                    }, 350);

                }
                else {
                    tryPrependCustomer(data, max);
                }
            }

            function sendInvoiceTMessage(data) {
                var max = 100;
                var div = '';
                var len = $('._content ._content-invoice .invoice-transfer-detail');
                if (len.length == 0)
                {      
                    for (i = 0; i < data.d.length; i++)
                    {
                        if (data.d[i].state !== 'updated')
                        {

                            div = '<div class="invoice-transfer-detail" data-sequence="' + data.d[i].sequence + '">' +
                        '<div class="invoice-col">' +
                        '</div>' +
                        '<div class="invoice-left">' +
                            '<div class="quad1">' +
                                '<h1>' + data.d[i].clientName + '</h1>' +
                                '<p>' + data.d[i].clientId + '</p>' + ' ' +
                                '&middot;' + ' ' +
                               '<p class="TinvoiceId">' + data.d[i].invoiceId + '</p>' +
                                '<div class="trans-icon">' +
                                    '<img src="Images/transferred.png" />' +
                                '</div>' +
                            '</div>' +
                            '<div class="quad2">' +
                                '<p>' + data.d[i].formattedDate + '</p>' +
                                '<p>' + data.d[i].Author + '</p>' +
                            '</div>' +
                            '<div class="quad3">' +
                                '<p>' + data.d[i].amount + '</p>' +
                           '</div>' +
                        '</div>' +
                    '</div>'
                        }
                        else
                        {

                            div = '<div class="invoice-transfer-detail" data-sequence="' + data.d[i].sequence + '">' +

                        '<div class="invoice-col">' +
                        '</div>' +
                        '<div class="invoice-left">' +
                         '<div class="overlay-update">' +
                                    '<p>' + 'Updated' + '</p>' +
                                '</div>' +
                            '<div class="quad1">' +
                                '<h1>' + data.d[i].clientName + '</h1>' +
                                '<p>' + data.d[i].clientId + '</p>' + ' ' +
                                '&middot;' + ' ' +
                               '<p class="TinvoiceId">' + data.d[i].invoiceId + '</p>' +
                                '<div class="trans-icon">' +
                                    '<img src="Images/transferred.png" />' +
                                '</div>' +
                            '</div>' +
                            '<div class="quad2">' +
                                '<p>' + data.d[i].formattedDate + '</p>' +
                                '<p>' + data.d[i].Author + '</p>' +
                            '</div>' +
                            '<div class="quad3">' +
                                '<p>'+ data.d[i].amount + '</p>' +
                           '</div>' +
                        '</div>' +
                    '</div>'
                        }

                        checkRemoveInvoice(data.d[i].invoiceId);
                        $(div).appendTo('._content ._content-invoice').hide().slideDown('slow', 'swing', function () { });
                    }


                    $("._content-invoice").mCustomScrollbar({
                        theme: "minimal-dark"
                    });

                    $("._content-invoice").mCustomScrollbar();
                }
                else {
                    tryPrependTransferred(data, max);
                }

            }

            function shouldPrependTransfer(data) {
                var divCompare = ""
                var divs = ""
                var b = 0
                var arrs = [];
                arrs.push(1);
                var canPrepend = true;


                var lenx = $('._content ._content-invoice .invoice-transfer-detail');
                maintainList(100, lenx.length, 'transfer');
                var len4 = $('._content ._content-invoice .invoice-transfer-detail');
                var ln = len4.length;

                $(len4).each(function () {
                    divCompare = '<div class="invoice-col">' +
                   '</div>' +
                   '<div class="invoice-left">' +
                       '<div class="quad1">' +
                           '<h1>' + data.clientName + '</h1>' +
                           '<p>' + data.clientId + '</p>' + ' ' +
                           '·' + ' ' +
                          '<p class="TinvoiceId">' + data.invoiceId + '</p>' +
                           '<div class="trans-icon">' +
                               '<img src="Images/transferred.png">' +
                           '</div>' +
                       '</div>' +
                       '<div class="quad2">' +
                           '<p>' + data.formattedDate + '</p>' +
                           '<p>' + data.Author + '</p>' +
                       '</div>' +
                       '<div class="quad3">' +
                           '<p>' + data.amount + '</p>' +
                      '</div>' +
                   '</div>'


                    if ($(this).find(".quad1 .TinvoiceId").text() == data.invoiceId) {

                        canPrepend = false;
                    }
                    else {
                        if (canPrepend != false) {
                            canPrepend = true;

                        }
                    }
                });
                return canPrepend;
            }

            function dataSequenceUniqueInvoices(sequence) {
                var canPrepend = true;
                var invoice = $('._content ._content-invoice .invoice-transfer-detail');
                var currentDigit = 0;

                $(invoice).each(function () {
                    currentDigit = $(this).attr("data-sequence");

                    if (currentDigit != sequence && canPrepend != false) {
                        canPrepend = true;
                    }
                    else {
                        canPrepend = false;
                    }
                });
                return canPrepend;
            }

            function dataSequenceUniquePInvoicesTransferred(sequence) {
                var canPrepend = true;
                var invoice = $('._content ._content-payments .invoice-payment-detail');
                var currentDigit = 0;

                $(invoice).each(function () {
                    currentDigit = $(this).attr("data-sequence");

                    if (currentDigit != sequence && canPrepend != false) {
                        canPrepend = true;
                    }
                    else {
                        canPrepend = false;
                    }
                });
                return canPrepend;
            }

            function shouldPrependCustomer(data) {
                var divCompare = ""
                var divs = ""
                var b = 0
                var arrs = [];
                arrs.push(1);
                var canPrepend = true;


                var lenx = $('._content ._content-customers .customer-detail');
                maintainList(100, lenx.length, 'customer');


                var len2 = $('._content ._content-customers .customer-detail');
                var ln = len2.length;

                $(len2).each(function () {
                    divCompare = '<div class="payment-col">' +
                   '</div>' +
                   '<div class="invoice-left">' +
                       '<div class="quad1">' +
                            '<h1>' + data.Name + '</h1>' +
                                '<p class="customer-id">' + data.ClientId + '</p>' + ' ' +
                           '·' + ' ' +
                           '<div class="trans-icon">' +
                               '<img src="Images/transfer-icon.png">' +
                           '</div>' +
                       '</div>' +
                       '<div class="quad2">' +
                           '<p>' + data.formattedDate + '</p>' +
                       '</div>' +
                       '<div class="quad3">' +
                      '</div>' +
                   '</div>'

                    if ($(this).find(".quad1 .customer-id").text() == data.ClientId) {

                        canPrepend = false;
                    }
                    else {
                        if (canPrepend != false) {
                            canPrepend = true;

                        }
                    }
                });
                return canPrepend;
            }

            function shouldPaymentTransfer(data) {
                var divCompare = ""
                var divs = ""
                var b = 0
                var arrs = [];
                arrs.push(1);
                var canPrepend = true;


                var lenx = $('._content ._content-payments .invoice-payment-detail');
                maintainList(100, lenx.length, 'payment');


                var len2 = $('._content ._content-payments .invoice-payment-detail');
                var ln = len2.length;

                for (var b = 0; b < ln; b++) {
                }

                $(len2).each(function () {
                    divCompare = '<div class="payment-col">' +
                   '</div>' +
                   '<div class="invoice-left">' +
                       '<div class="quad1">' +
                           '<h1>' + data.clientName + '</h1>' +
                           '<p>' + data.clientId + '</p>' + ' ' +
                           '·' + ' ' +
                          '<p class="PinvoiceId">' + data.invoiceId + '</p>' +
                           '<div class="trans-icon">' +
                               '<img src="Images/transfer-icon.png">' +
                           '</div>' +
                       '</div>' +
                       '<div class="quad2">' +
                           '<p>' + data.formattedDate + '</p>' +
                       '</div>' +
                       '<div class="quad3">' +
                           '<p>' + data.amount + '</p>' +
                      '</div>' +
                   '</div>'

                    if ($(this).find(".quad1 .PinvoiceId").text() == data.invoiceId) {

                        canPrepend = false;
                    }
                    else {
                        if (canPrepend != false) {
                            canPrepend = true;

                        }
                    }
                });
                return canPrepend;
            }

            function shouldPrepend(data) {

                var right = String(data.invoiceId);
                var left;
                var canPrepend = true;
                var lenx = $('._content ._content-pending .invoice-pending-detail');
                maintainList(100, lenx.length, 'pending');

                $(lenx).each(function () {

                    left = $(this).find(".quad1 .invoiceId").text();
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


            function maintainList(max, len, target) {
                if (target == 'transfer') {
                    var distance = len - max;
                    var startPos = len - 1;
                    var endPos = startPos - distance;

                    var i = 0;
                    if (len > max) {
                        for (var i = startPos; i > endPos; i--) {
                            $('._content ._content-invoice .invoice-transfer-detail').eq(i).remove();
                        }
                    }
                    else {

                    }
                }
                else if (target == 'pending') {
                    var distance = len - max;
                    var startPos = len - 1;
                    var endPos = startPos - distance;

                    var i = 0;
                    if (len > max) {
                        for (var i = startPos; i > endPos; i--) {
                            $('._content ._content-pending .invoice-pending-detail').eq(i).remove();
                        }
                    }
                    else {

                    }
                }
                else if (target == 'payment') {
                    var distance = len - max;
                    var startPos = len - 1;
                    var endPos = startPos - distance;

                    var i = 0;
                    if (len > max) {
                        for (var i = startPos; i > endPos; i--) {
                            $('._content ._content-payments .invoice-payment-detail').eq(i).remove();
                        }
                    }
                    else {

                    }
                }
                else if (target == 'customer') {
                    var distance = len - max;
                    var startPos = len - 1;
                    var endPos = startPos - distance;

                    var i = 0;
                    if (len > max) {
                        for (var i = startPos; i > endPos; i--) {
                            $('._content ._content-customers .customer-detail').eq(i).remove();
                        }
                    }
                    else {

                    }
                }
            }

            function tryPrependCustomer(data, max) {
                for (var x = 0; x < data.d.length; x++) {

                    if (shouldPrependCustomer(data.d[x]) == true) {

                        var sendC = '<div class="customer-detail">' +
                      '<div class="customer-col">' +
                      '</div>' +
                      '<div class="invoice-left">' +
                          '<div class="quad1">' +
                              '<h1>' + data.d[x].Name + '</h1>' +
                                '<p class="customer-id">' + data.d[x].ClientId + '</p>' + ' ' +
                              ' ' + ' ' +
                              '<div class="trans-icon">' +
                                  '<img src="Images/newcustomer.png" />' +
                              '</div>' +
                          '</div>' +
                          '<div class="quad2">' +
                              '<p>' + data.d[x].formattedDate + '</p>' +
                          '</div>' +
                          '<div class="quad3">' +
                         '</div>' +
                      '</div>' +

                  '</div>'

                        $(sendC).prependTo('._content ._content-customers').hide().slideDown(700, "swing", function () { });
                        $("._content-customers").mCustomScrollbar("destroy");
                    }
                    else {

                    }

                }
                $("._content-customers").mCustomScrollbar({
                    theme: "minimal-dark"
                });
                $("._content-customers").mCustomScrollbar();
            }

            function tryPrependPayments(data, max) {
                for (var x = 0; x < data.d.length; x++) {

                    if (dataSequenceUniquePInvoicesTransferred(data.d[x].sequence) == true) {
                        var sendPInvoice = '<div class="invoice-payment-detail" data-sequence="' + data.d[x].sequence + '">' +
                      '<div class="payment-col">' +
                      '</div>' +
                      '<div class="invoice-left">' +
                          '<div class="quad1">' +
                              '<h1>' + data.d[x].clientName + '</h1>' +
                              '<p>' + data.d[x].clientId + '</p>' + ' ' +
                              '&middot;' + ' ' +
                             '<p class="PinvoiceId">' + data.d[x].invoiceId + '</p>' +
                              '<div class="trans-icon">' +
                                  '<img src="Images/transfer-icon.png" />' +
                              '</div>' +
                          '</div>' +
                          '<div class="quad2">' +
                              '<p>' + data.d[x].formattedDate + '</p>' +
                          '</div>' +
                          '<div class="quad3">' +
                              '<p>' + data.d[x].amount + '</p>' +
                         '</div>' +
                      '</div>' +

                  '</div>'

                        $(sendPInvoice).prependTo('._content ._content-payments').hide().slideDown(700, "swing", function () { });
                        $("._content-payments").mCustomScrollbar("destroy");
                    }
                    else {
                    }
                }
                $("._content-payments").mCustomScrollbar({
                    theme: "minimal-dark"
                });
                $("._content-payments").mCustomScrollbar();
            }

            function tryPrependTransferred(data, max) {
                var div = '';
                for (var x = 0; x < data.d.length; x++)
                {

                    if (shouldPrependTransfer(data.d[x]) == true || data.d[x].state === "updated") {

                        if (data.d[x].state !== 'updated') {
                            div = '<div class="invoice-transfer-detail" data-sequence="' + data.d[x].sequence + '">' +
                        '<div class="invoice-col">' +
                        '</div>' +
                        '<div class="invoice-left">' +
                            '<div class="quad1">' +
                                '<h1>' + data.d[x].clientName + '</h1>' +
                                '<p>' + data.d[x].clientId + '</p>' + ' ' +
                                '&middot;' + ' ' +
                               '<p class="TinvoiceId">' + data.d[x].invoiceId + '</p>' +
                                '<div class="trans-icon">' +
                                    '<img src="Images/transferred.png" />' +
                                '</div>' +
                            '</div>' +
                            '<div class="quad2">' +
                                '<p>' + data.d[x].formattedDate + '</p>' +
                                '<p>' + data.d[x].Author + '</p>' +
                            '</div>' +
                            '<div class="quad3">' +
                                '<p>' + data.d[x].amount + '</p>' +
                           '</div>' +
                        '</div>' +
                    '</div>'

                            checkRemoveInvoice(data.d[x].invoiceId);
                            $(div).prependTo('._content ._content-invoice').hide().slideDown(700, "swing", function () { });
                            $("._content-invoice").mCustomScrollbar("destroy");
                        }
                        else {
                            if (dataSequenceUniqueInvoices(data.d[x].sequence)) {
                                div = '<div class="invoice-transfer-detail" data-sequence="' + data.d[x].sequence + '">' +

                       '<div class="invoice-col">' +
                       '</div>' +
                       '<div class="invoice-left">' +
                        '<div class="overlay-update">' +
                                   '<p>' + 'Updated' + '</p>' +
                               '</div>' +
                           '<div class="quad1">' +
                               '<h1>' + data.d[x].clientName + '</h1>' +
                               '<p>' + data.d[x].clientId + '</p>' + ' ' +
                               '&middot;' + ' ' +
                              '<p class="TinvoiceId">' + data.d[x].invoiceId + '</p>' +
                               '<div class="trans-icon">' +
                                   '<img src="Images/transferred.png" />' +
                               '</div>' +
                           '</div>' +
                           '<div class="quad2">' +
                               '<p>' + data.d[x].formattedDate + '</p>' +
                               '<p>' + data.d[x].Author + '</p>' +
                           '</div>' +
                           '<div class="quad3">' +
                               '<p>' + data.d[x].amount + '</p>' +
                          '</div>' +
                       '</div>' +
                   '</div>'


                                checkRemoveInvoice(data.d[x].invoiceId);
                                $(div).prependTo('._content ._content-invoice').hide().slideDown(700, "swing", function () { });
                                $("._content-invoice").mCustomScrollbar("destroy");
                            }

                        }

                    }

                }
                $("._content-invoice").mCustomScrollbar({
                    theme: "minimal-dark"
                });
                $("._content-invoice").mCustomScrollbar();
            }

            function tryPrependPending(data, max) {

                for (var x = data.d.length - 1; x >= 0; x--) {

                    if (shouldPrepend(data.d[x]) == true) {
                        console.log("should prepend");
                        var send = '<div class="invoice-pending-detail">' +
                      '<div class="pending-col">' +
                      '</div>' +
                      '<div class="invoice-left">' +
                          '<div class="quad1">' +
                              '<h1>' + data.d[x].clientName + '</h1>' +
                              '<p>' + data.d[x].clientId + '</p>' + ' ' +
                              '&middot;' + ' ' +
                             '<p class="invoiceId">' + data.d[x].invoiceId + '</p>' +
                              '<div class="trans-icon">' +
                                  '<img src="Images/diskette.png" />' +
                              '</div>' +
                          '</div>' +
                          '<div class="quad2">' +
                              '<p>' + data.d[x].formattedDate + '</p>' +
                              '<p>' + data.d[x].Author + '</p>' +
                          '</div>' +
                          '<div class="quad3">' +
                              '<p>' + data.d[x].amount + '</p>' +
                         '</div>' +
                      '</div>' +
                  '</div>';


                        var len_ = $('._content ._content-pending .invoice-pending-detail');

                        var invId1 = $(len_).eq(0).find(".quad1 .invoiceId").text();
                        var invId2 = data.d[x].invoiceId;

                        if (invId2 < invId1) {
                            appendIt = true;
                            console.log("append true");
                        }
                        else {
                            appendIt = false;
                            console.log("append false");
                        }


                        if (appendIt == true) {
                            $(send).appendTo('._content ._content-pending').hide().slideDown(700, "swing", function () { });
                            $("._content-pending").mCustomScrollbar("destroy");
                            appendIt = false;
                            console.log("appending");
                        }
                        else {
                            $(send).prependTo('._content ._content-pending').hide().slideDown(700, "swing", function () { });
                            $("._content-pending").mCustomScrollbar("destroy");
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


            function GetRate() {

                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/GetRate',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {

                        $('.xrate').text("Today's rate: $ " + Number(data.d).toFixed(4));
                        $('.rate').fadeIn(1300);
                    },
                    error: function (error) {
                        console.log(error);
                    }
                });
            }


            function GetTransferredInvoiceMessages() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/InvoiceTransferredMessages',
                    type: 'post',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {

                        if (data.d.length > 0) {
                            sendInvoiceTMessage(data);
                        }
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }

            function GetTransferredPaymentMessages() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/PaymentTransferredMessages',
                    type: 'post',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {

                        if (data.d.length > 0) {
                            sendPaymentMessage(data);
                        }
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }




            function GetCreatedCustomerMessages() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/CustomerCreatedMessages',
                    type: 'post',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {

                        if (data.d.length > 0) {
                            sendCustomerMessage(data);
                        }
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }

            function GetPendingInvoiceMessages() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/PendingInvoiceMessages',
                    type: 'post',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {

                        if (data.d.length > 0) {
                            sendInvoiceMessage(data);
                        }
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }


            function svc_local_GetRate() {

                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/GetRate',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {

                        $('.xrate').text("Today's rate: $ " + (data.d));
                        $('.rate').fadeIn(1300);
                    },
                    error: function () {
                        console.log("Retrieve Exchange Rate fail");
                    }
                });
            }

            function InvoiceCount() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/GetInvoiceCount',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {

                        $('.Dashboard #d1 .number h2').text(data.d);
                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function isOnline() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/IntegrationService.asmx/GetMonStat',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {
                        if (data.d == -1) {
                            $('#stat img').attr({ 'src': 'Images/offline.png' });
                        }
                        else if (data.d == 2 || data.d == 21) {

                            $('#stat img').attr({ 'src': 'Images/service-not-started.png' });
                        }
                        else if (data.d == 3 || data.d == 31) {

                            $('#stat img').attr({ 'src': 'Images/online.png' });
                        }
                    },
                    error: function () {
                        console.log("error");
                        $('#stat img').attr({ 'src': 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAIVBMVEX////VACvZACbYACfYACf/AADYACfYACfXACjYACcAAABiOUJsAAAACXRSTlMAJH6y5gFu9bQdqz9YAAAAAWJLR0QKaND0VgAAAAlwSFlzAAAN1wAADdcBQiibeAAAAAd0SU1FB+AFGQg4DC9bhy8AAABKSURBVAjXY2BgEDJxVmQAgrDKmTOnpzIwsHbOBIIZAQxsM8EggUEcwihk0IQwJjFYQhiTGTwhjCkIBlwKrhiuHW4g3Aq4pTBnAAAeiTmDuN/4+wAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAxNi0wNS0yNVQwODo1NjoxMiswMjowME8RE3AAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMTYtMDUtMjVUMDg6NTY6MTIrMDI6MDA+TKvMAAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAABJRU5ErkJggg==' });
                    }
                });
            }

            function PaymentCount() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/GetPaymentCount',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {
                        $('.Dashboard #d2 .number h2').text(data.d);
                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function CustomerCount() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/GetCustomerCount',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {
                        $('.Dashboard #d3 .number h2').text(data.d);
                        $('.back3 .number h2').text(data.d);

                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function PendingInvCount() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/GetPendingCount',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {
                        $('.Dashboard #d4 .number h2').text(data.d);
                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function InvoiceDetails() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/GetInvoiceDetail',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {
                        var i=0;
                        for (i = 0; i < data.d.length; i++)
                        {
                            if (data.d[i].batchType == 'Type Approval')
                            {
                                $('#typeapproval-count p').text(data.d[i].count);
                                $('#typeapproval-amount p').text("$" + data.d[i].amount);
                            }
                            else if (data.d[i].batchType == 'Non Maj')
                            {
                                $('#nonmaj-count p').text(data.d[i].count);
                                $('#nonmaj-amount p').text("$" + data.d[i].amount);
                            }
                            else if (data.d[i].batchType == 'Maj')
                            {
                                $('#maj-count p').text(data.d[i].count);
                                $('#maj-amount p').text("$" + data.d[i].amount);
                            }
                            else if (data.d[i].renstat == 'Spectrum')
                            {
                                $('#renewal-spec-count p').text(data.d[i].speccount);
                                $('#renewal-spec-amount p').text("$" + data.d[i].specamt);
                            }
                            else if (data.d[i].renstat == 'Regulatory')
                            {
                                $('#renewal-reg-count p').text(data.d[i].regcount);
                                $('#renewal-reg-amount p').text("$" + data.d[i].regamt);
                            }
                        }
                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function CancellationAndMemos() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/GetCancellationsAndMemos',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {
                        if (data.d.length > 0)
                        {
                            populateMemoAndCancellations(data);
                        }
                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function ReceiptDetails() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/GetReceiptDetail',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {

                        $('.right-total h2').text('$ ' + data.d[0]);
                        $('.bank1 .batch h3').text(data.d[1]);
                        $('.bank2 .batch h3').text(data.d[2]);
                        $('.bank3 .batch h3').text(data.d[3]);
                        $('.bank4 .batch h3').text(data.d[4]);

                        $('.bank1 .count h3').text(data.d[5]);
                        $('.bank2 .count h3').text(data.d[6]);
                        $('.bank3 .count h3').text(data.d[7]);
                        $('.bank4 .count h3').text(data.d[8]);
                        $('.pay-expiry h3').text(data.d[9]);
                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }




            function InvoiceBatchTotal() {
                $.ajax({
                    url: 'http://server-erp2.sma.gov.jm:8080/integrationservice.asmx/GetInvoiceTotalAmount',
                    type: 'POST',
                    contentType: 'application/json',
                    data: {},
                    dataType: 'json',
                    success: function (data) {

                        $('.right-total-2 h2').text("$ "+ data.d);
                    },
                    error: function () {
                        console.log("error");
                    }
                });
            }

            function populateMemoAndCancellations(data)
            {
                var max = 100;
                var div = '';
                var len = $('._content ._content-memo_cancellation .invoice-memo-cancellation');


                if (len.length == 0)
                {
                    for (i = 0; i < data.d.length; i++)
                    {
                        var doctype = data.d[i].docType;
                        var hexval = "";
                        
                        if (doctype == 'credit_memo')
                        {
                            hexval = "#309A8A";
                            doctype = "Credit Note";
                        } else if (doctype == 'cancelled_invoice')
                        {
                            hexval = "#33658A";
                            doctype = "Cancellation";
                        }

              

                        div = '<div class="invoice-memo-cancellation" data-sequence="' + data.d[i].sequence + '">' +
                                '<div class="invoice-col">' +
                                '</div>' +
                                '<div class="invoice-left">' +
                                '<div class="quad1">' +
                                  '<div class="overylay_cancellation_memo" style="position: relative; height: 26px; width: 90px;float:right;top: 55px;text-align:center; background-color: '+hexval+';"><p>'+doctype+'</p></div>' +
                                '<h1>' + data.d[i].clientName + '</h1>' +
                                '<p>' + data.d[i].clientId + '</p>' + ' ' +
                                '&middot;' + ' ' +
                                '<p class="TinvoiceId">' + data.d[i].invoiceId + '</p>' +
                                '<div class="trans-icon">' +
                                '<img src="Images/transferred.png" />' +
                                '</div>' +
                                '</div>' +
                                '<div class="quad2">' +
                                '<p>' + data.d[i].formattedDate + '</p>' +
                                '<p>' + data.d[i].Author + '</p>' +
                                '</div>' +
                                '<div class="quad3">' +
                                '<p>'+data.d[i].amount+ '</p>' +
                                '</div>' +
                                '</div>' +
                                '</div>'

                        $(div).appendTo('._content ._content-memo_cancellation').hide().slideDown('slow', 'swing', function () { });
                    }

                    $("._content-memo_cancellation").mCustomScrollbar({
                        theme: "minimal-dark"
                    });
                    $("._content-memo_cancellation").mCustomScrollbar();
                }
                else
                {
                    var cancellation_memos = $('._content ._content-memo_cancellation .invoice-memo-cancellation');
                    var dataOnUi = [];

                    $(cancellation_memos).each(function () {
                       dataOnUi.push($(this).attr("data-sequence"));
                    });

                    for (var x = 0; x < data.d.length; x++)
                    {
                        var canAppend = true;

                        for (var y = 0; y < dataOnUi.length; y++)
                        {
                            if (data.d[x].sequence == dataOnUi[y])
                            {
                                canAppend = false;
                            }
                        }

                        if (canAppend == false)
                        {
                            //record already exists
                        }
                        else
                        {
                            //add new data to the column
                            var doctype = data.d[x].docType;
                            var hexval = "";
                        
                            if (doctype == 'credit_memo')
                            {
                                hexval = "#309A8A";
                                doctype = "Credit Note";
                            } else if (doctype == 'cancelled_invoice')
                            {
                                hexval = "#33658A";
                                doctype = "Cancellation";
                            }

                            div = '<div class="invoice-memo-cancellation" data-sequence="' + data.d[x].sequence + '">' +
                                    '<div class="invoice-col">' +
                                    '</div>' +
                                    '<div class="invoice-left">' +
                                    '<div class="quad1">' +
                                      '<div class="overylay_cancellation_memo" style="position: relative; height: 26px; width: 90px;float:right;top: 55px;text-align:center; background-color: '+hexval+';"><p>'+doctype+'</p></div>' +
                                    '<h1>' + data.d[x].clientName + '</h1>' +
                                    '<p>' + data.d[x].clientId + '</p>' + ' ' +
                                    '&middot;' + ' ' +
                                    '<p class="TinvoiceId">' + data.d[x].invoiceId + '</p>' +
                                    '<div class="trans-icon">' +
                                    '<img src="Images/transferred.png" />' +
                                    '</div>' +
                                    '</div>' +
                                    '<div class="quad2">' +
                                    '<p>' + data.d[x].formattedDate + '</p>' +
                                    '<p>' + data.d[x].Author + '</p>' +
                                    '</div>' +
                                    '<div class="quad3">' +
                                    '<p>'+ data.d[x].amount + '</p>' +
                                    '</div>' +
                                    '</div>' +
                                    '</div>'

                            $(div).prependTo('._content ._content-memo_cancellation').hide().slideDown('slow', 'swing', function () { });
                            $("._content-memo_cancellation").mCustomScrollbar("destroy");
                        }

                    }

                    $("._content-memo_cancellation").mCustomScrollbar({
                        theme: "minimal-dark"
                    });
                    $("._content-memo_cancellation").mCustomScrollbar();
                }
            }


            function getData()  {

                isOnline();
                InvoiceCount();
                CustomerCount();
                PaymentCount();
                PendingInvCount();
                InvoiceDetails();
                ReceiptDetails();
                InvoiceBatchTotal();
                CancellationAndMemos();

                GetTransferredInvoiceMessages();
                GetTransferredPaymentMessages();
                GetCreatedCustomerMessages();
                GetPendingInvoiceMessages();
                GetRate();
            }

            getData();

            window.setInterval(function () {
                getData();
            }, 1000);
        }

        $(document).ready(main);
    </script>
</body>
</html>


