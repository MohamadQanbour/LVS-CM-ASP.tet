var errorList = [];
var errorList2 = [];

$(document).ready(function () {
    $('#btnMoveToClass').click(function () {
        $('#modalMove').modal("show");
    });
    $('[data-toggle="sync"], [data-toggle="sync-student"]').click(function () {
        $(this).html('<span class="fa fa-spin fa-spinner"></span>').addClass('disabled');
    });
});

function StartSync() {
    $('#pnlButton, [data-toggle="sync"]').hide();
    $('#pnlsync').show();
    $.ajax('/ajax/Import/StudentFileRecords', {
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

function StartStudentSync() {
    $('#pnlButton2, [data-toggle="sync-student"]').hide();
    $('#pnlsync2').show();
    $.ajax("/ajax/Import/StudentProfileFileRecords", {
        type: 'POST',
        data: { 'file': $('#hdnSelectedStudentFile').val() },
        error: function (a, b, c) { alert(c); },
        success: function (a) {
            var ret = JSON.parse(a)[0];
            if (ret.HasError) alert(ret.ErrorMessage);
            else {
                var d = ret.ReturnData;
                $('#pnlTotal2').text(d);
                var step = 100;
                var numOfPages = Math.ceil(parseFloat(d) / parseFloat(step));
                SyncStudentStep($('#hdnSelectedStudentFile').val(), 0, numOfPages, d, step);
            }
        }
    });
}

function SyncStep(file, index, pages, limit, step) {
    if (index < pages) {
        $.ajax('/ajax/Import/SyncStudent', {
            type: "POST",
            data: {
                "file": file,
                "page": index,
                "step": step
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

function SyncStudentStep(file, index, pages, limit, step) {
    if (index < pages) {
        $.ajax('/ajax/Import/SyncStudentProfile', {
            type: "POST",
            data: {
                "file": file,
                "page": index,
                "step": step
            },
            error: function (a, b, c) {
                RecordError2(c);
                ShowError2();
            },
            success: function (a) {
                var ret = JSON.parse(a)[0];
                if (ret.HasError) {
                    RecordError2(ret.ErrorMessage);
                    ShowError2();
                } else {
                    var old = parseInt($('#pnlProgress2').text());
                    if (old + step < limit) {
                        old = old + step;
                        $('#pnlProgress2').text(old);
                        SyncStudentStep(file, index + 1, pages, limit, step);
                    } else {
                        $('#pnlProgress2').text(limit);
                        DeleteProfileFile(file);
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
                var modal = $('.sync-students-modal');
                modal.on('hidden.bs.modal', function () {
                    window.location = $('#hdnReload').val();
                });
                modal.modal('hide');
            }
        }
    });
}

function DeleteProfileFile(file) {
    $.ajax('/ajax/Import/PaymentFileDelete', {
        type: 'POST',
        data: { 'file': file },
        error: function (a, b, c) {
            RecordError2(c);
            ShowError2();
        },
        success: function (a) {
            var ret = JSON.parse(a)[0];
            if (ret.HasError) {
                RecordError2(ret.ErrorMessage);
                ShowError2();
            } else {
                var modal = $('.sync-students-profile-modal');
                modal.on('hidden.bs.modal', function () {
                    window.location = $('#hdnReloadStudent').val();
                });
                modal.modal('hide');
            }
        }
    });
}

function RecordError(e) {
    errorList.push(e);
}

function RecordError2(e) {
    errorList2.push(e);
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

function ShowError2() {
    if (errorList2.length > 0) {
        $('#pnlErrors2').html("Could not complete the sync process, <b>" + errorList2.length + "</b> error(s) occurred during the process.");
        var lst = $('#resultErrorsList2');
        $(errorList2).each(function () {
            var $this = this;
            lst.append(function () { return $('<li>').html($this); });
        });
    }
}