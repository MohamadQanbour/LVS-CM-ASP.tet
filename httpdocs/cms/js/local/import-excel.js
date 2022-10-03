$(document).ready(function () {
    CheckValidation(false);
    $('input, select, textarea').on("change", function () {
        CheckValidation(true);
    });
    $('[data-singlefileuploader] .btn-danger').click(function () {
        CheckValidation(true);
    });
    $('.btn-upload').click(function () {
        $(this).html('<span class="fa fa-spin fa-spinner"></span>').addClass("disabled");
    });
    $('[data-command="startimport"]').click(function () {
        var file = $('#hdnSelectedFile').val();
        $(this).html('<span class="fa fa-spin fa-spinner"></span>').addClass("disabled");
        var _this = $(this);
        $.ajax("/ajax/Import/PaymentsTotalRecords", {
            type: "GET",
            data: {
                "file": file
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var d = ret.ReturnData;
                    _this.slideUp();
                    $('#pnlProgress').slideDown();
                    $('#pnlErrors').slideDown();
                    StartImporting(d);
                }
            }
        });
        return false;
    });
});

function StartImporting(limit) {
    var numOfPages = Math.ceil(parseFloat(limit) / parseFloat(10));
    var percent = Math.ceil(parseFloat(100) / parseFloat(numOfPages));
    var file = $('#hdnSelectedFile').val();
    for (var i = 0 ; i < numOfPages ; i++) {
        $.ajax("/ajax/Import/PaymentsImport", {
            type: "GET",
            data: {
                "file": file,
                "page": i
            },
            error: function (a, b, c) { alert(c); },
            success: function (d) {
                var ret = $.parseJSON(d)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var data = ret.ReturnData;
                    $(data).each(function () {
                        WriteErrors(this);
                    });
                    IncreaseProgress(percent);
                }
            }
        });
    }
}

function IncreaseProgress(val) {
    var curValue = parseInt($('#pnlProgress .progress-bar').attr("aria-valuenow"));
    if (isNaN(curValue)) curValue = 0;
    curValue = curValue + val;
    var shouldClose = false;
    if (curValue >= 100) {
        curValue = 100;
        shouldClose = true;
        $('#pnlProgress .progress-bar').removeClass("active");
    }
    $('#pnlProgress .progress-bar').attr("aria-valuenow", curValue).css("width", curValue + "%");
    if (shouldClose) {
        CloseProgress();
    }
}

function CloseProgress() {
    var file = $('#hdnSelectedFile').val();
    $.ajax("/ajax/Import/PaymentFileDelete", {
        type: "GET",
        data: {
            "file": file
        },
        error: function (a, b, c) { alert(c); },
        success: function (a) {
            var ret = $.parseJSON(a)[0];
            if (ret.HasError) alert(ret.ErrorMessage);
            else {
                var d = ret.ReturnData;
                if (d) {
                    $('#pnlClose').slideDown();
                }
            }
        }
    });
}

    function WriteErrors(error) {
        $('#pnlErrors').append($('<div>').text(error));
    }

    function CheckValidation(showValidations) {
        showValidations = showValidations == undefined ? false : showValidations;
        var valid = true;
        var file = $('[data-singlefileuploader]').first().find('input[type="file"]').val();
        valid = valid & file != "";
        var save = $('.btn-upload');
        if (valid) save.removeClass("disabled"); else save.addClass("disabled");
    }