var errorList = [];
$(document).ready(function () {
    $('[data-toggle="sync"]').on('click', function () {
        $(this).html('<span class="fa fa-spin fa-spinner"></span>').addClass('disabled');
    });
});

function StartSync() {
    $('#pnlButton, [data-toggle="sync"]').hide();
    $('#pnlsync').show();
    $.ajax('/ajax/Import/StudentAccountsRecords', {
        type: 'POST',
        data: { "file": $('#hdnSelectedFile').val() },
        error: function (a, v, c) { alert(c); },
        success: function (a) {
            var ret = JSON.parse(a)[0];
            if (ret.HasError) alert(ret.ErrorMessage);
            else {
                var d = ret.ReturnData;
                $('#pnlTotal').text(d);
                var step = 100;
                var numOfPages = Math.ceil(parseFloat(d) / parseFloat(step));
                SyncStep($('#hdnSelectedFile').val(), 0, numOfPages, d, step);
            }
        }
    });
}

function SyncStep(file, index, pages, limit, step) {
    if (index < pages) {
        $.ajax('/ajax/Import/SyncStudentAccount2', {
            type: "POST",
            data: {
                "file": file,
                "page": index,
                "step": step,
                "user": parseInt($('#hdnUserId').val())
            },
            error: function (a, b, c) {
                RecordError(c);
                ShowError();
            },
            success: function (a) {
                var ret = JSON.parse(a)[0];
                if (ret.HasError) {
                    RecordError(ret.ErrorMessage);
                    ShowError();
                } else {
                    var old = parseInt($('#pnlProgress').text());
                    if (old + step < limit) {
                        old = old + step;
                        $('#pnlProgress').text(old);
                        SyncStep(file, index + 1, pages, limit, step);
                    } else {
                        $('#pnlProgress').text(limit);
                        DeleteFile(file);
                    }
                }
            }
        });
    }
}

function DeleteFile(file) {
    $.ajax('/ajax/Import/PaymentFileDelete', {
        type: 'POST',
        data: { 'file': file },
        error: function (a, b, c) {
            RecordError(c);
            ShowError();
        },
        success: function (a) {
            var ret = JSON.parse(a)[0];
            if (ret.HasError) {
                RecordError(ret.ErrorMessage);
                ShowError();
            } else {
                var modal = $('.sync-accounts');
                modal.on('hidden.bs.modal', function () {
                    window.location = $('#hdnReload').val();
                });
                modal.modal('hide');
            }
        }
    });
}

function RecordError(e) {
    errorList.push(e);
}

function ShowError() {
    if (errorList.length > 0) {
        $('#pnlErrors').html("Could not complete the sync process, <b>" + errorList.length + "</b> error(s) occurred during the process.");
        var lst = $('#resultErrorsList');
        $(errorList).each(function () {
            var $this = this;
            lst.append(function () { return $('<li>').html($this); });
        });
    }
}