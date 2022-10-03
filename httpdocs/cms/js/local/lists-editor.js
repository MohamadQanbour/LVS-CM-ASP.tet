$(document).ready(function () {
    $('#btnAdd').click(function () {
        $('[id$=modalAdd]').modal();
    });
});

function ValidateText(obj, val) {
    var ret = true;
    try {
        var lst = $.parseJSON($('#hdnItemText').val());
        var found = false;
        $(lst).each(function () {
            if (!found && this.toString().toLowerCase() == val.Value.toString().toLowerCase()) found = true;
        });
        ret = !found;
    } catch (e) {
        ret = false;
    }
    val.IsValid = ret;
}

function ValidateValue(obj, val) {
    var ret = true;
    try {
        var lst = $.parseJSON($('#hdnItemValue').val());
        var found = false;
        $(lst).each(function () {
            if (!found && this.toString().toLowerCase() == val.Value.toString().toLowerCase()) found = true;
        });
        ret = !found;
    } catch (e) {
        ret = false;
    }
    val.IsValid = ret;
}