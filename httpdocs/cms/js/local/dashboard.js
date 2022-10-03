$(document).ready(function () {
    if ($('[data-autocompletesource]').length) {
        $('[data-autocompletesource]').each(function () {
            var t = $(this);
            t.select2("destroy").select2({
                allowClear: true,
                ajax: {
                    delay: 250,
                    url: t.data('autocompletesource'),
                    data: function (params) {
                        return {
                            "q": params.term,
                            "userid": t.data("userid"),
                            "usertype": t.data("usertype")
                        };
                    },
                    processResults: function (data) {
                        var d = eval(data)[0];
                        if (d.HasError) alert(d.ErrorMessage); else {
                            return { results: d.ReturnData }
                        }
                    }
                }
            });
        });
    }
    $('#ddlTo').change(function () {
        $('#hdnSelectedTo').val("[" + $(this).val() + "]");
    });
    $('#btnExportToAccess').click(function () {
        $('#pnlExport').slideToggle();
        return false;
    });
    $('[data-function="export"]').click(function () {
        var html = $(this).html();
        $(this).html('<span class="fa fa-spinner fa-spin"></span>');
        var _this = $(this);
        $.ajax("/ajax/Import/CreateNewMDBFile", {
            type: "POST",
            data: {},
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = JSON.parse(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var d = ret.ReturnData;
                    _this.hide();
                    $('#pnlProgress').slideDown();
                    StartExport(d);
                }
            },
            complete: function () {
                _this.html(html);
            }
        });
        return false;
    });
});

function StartExport(fileName) {
    $.ajax("/ajax/Import/MigrateList", {
        type: "POST",
        data: {},
        error: function (a, b, c) { alert(c); },
        success: function (a) {
            var ret = JSON.parse(a)[0];
            if (ret.HasError) alert(ret.ErrorMessage);
            else {
                var d = ret.ReturnData;
                DoStep(d, 0, fileName);
            }
        }
    });
}

function DoStep(lst, index, fileName) {
    var step = Math.ceil(parseFloat(100) / parseFloat(lst.length));
    $('#litStep').text(lst[index]);
    $.ajax("/ajax/Import/Migrate", {
        type: "GET",
        data: { "step": lst[index], "file": fileName },
        error: function (a, b, c) { alert(c); },
        success: function (a) {
            var ret = JSON.parse(a)[0];
            if (ret.HasError) alert(ret.ErrorMessage);
            else {
                IncreaseProgress(step, lst);
                if (index + 1 < lst.length) DoStep(lst, index + 1, fileName);
                else {
                    CloseExport(fileName);
                }
            }
        }
    });
}

function CloseExport(fileName) {
    $.ajax("/ajax/Import/Migrate", {
        type: "GET",
        data: { "step": "Close", "file": fileName },
        error: function (a, b, c) { alert(c); },
        success: function (a) {
            var ret = JSON.parse(a)[0];
            if (ret.HasError) alert(ret.ErrorMessage);
            else {
                var d = ret.ReturnData;
                if (d != "") {
                    $('#pnlProgress').slideUp();
                    $('[data-function="download"]').attr("href", d);
                    $('#pnlDownload').slideDown();
                    $('#pnlPercentage').text('100%');
                }
            }
        }
    });
}

function IncreaseProgress(val, total) {
    var curValue = parseFloat($('#pnlProgress .progress-bar').attr("aria-valuenow"));
    if (isNaN(curValue) || curValue == undefined) curValue = 0;
    var step = 100 / total.length;
    curValue = curValue + step;
    var shouldClose = false;
    if (curValue >= 100) {
        curValue = 100;
        shouldClose = true;
        $('#pnlProgress .progress-bar').removeClass("active");
    }
    $('#pnlProgress .progress-bar').attr("aria-valuenow", curValue).css("width", curValue + "%");
    $('#pnlPercentage').text(parseInt(curValue) + '%');
}