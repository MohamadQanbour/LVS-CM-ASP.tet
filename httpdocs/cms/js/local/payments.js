$(document).ready(function () {
    $('#ddlStudent').change(function () {
        $('#hdnStudent').val($(this).val());
    });
    $('#txtDeposit, #txtTotal, #txtDiscount').change(function () {
        CalculateFields();
    });
    $('[commandname="Add"]').click(function () {
        $('#egvModal').modal("show");
    });
    $('[data-edit]').click(function () {
        var html = $(this).html();
        var _this = $(this);
        $(this).html('<span class="fa fa-spin fa-spinner"></span>').addClass("disabled");
        var id = $(this).attr("data-edit");
        $.ajax("/ajax/CMSMisc/StudentPayment", {
            type: "GET",
            data: { "paymentid": id },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var obj = ret.ReturnData;
                    $('#hdnId').val(obj.Id);
                    $('#txtPaymentNumber').val(obj.PaymentNumber);
                    $('#txtPaymentAmount').val(obj.PaymentAmount);
                    $('#txtPaymentDate').val(obj.PaymentDate);
                    $('#txtPaymentNote').val(obj.PaymentNote);
                    _this.html(html).removeClass("disabled");
                    $('#egvModal').modal("show");
                }
            }
        });
        return false;
    });
});

function CalculateFields() {
    var deposit = parseInt($('#txtDeposit').val().replace(",", ""));
    var total = parseInt($('#txtTotal').val().replace(",", ""));
    var discount = parseInt($('#txtDiscount').val().replace(",", ""));
    var netTotal = deposit + total - discount;
    $('#txtNetTotal').val(netTotal);
    $('#txtBalance').val(netTotal - parseInt($('#txtPaymentsSum').val().replace(",", "")));
}