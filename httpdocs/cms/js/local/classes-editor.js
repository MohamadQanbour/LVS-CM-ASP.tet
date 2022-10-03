$(document).ready(function () {
    UpdateCheckboxes();
    $('[data-template] [type=checkbox]').on("ifChecked", function () {
        var id = $(this).parents('[data-template]').data('template');
        CheckStatus();
        AddRelated(id);
    }).on("ifUnchecked", function () {
        var id = $(this).parents('[data-template]').data('template');
        CheckStatus();
        RemoveRelated(id);
    });
});

function UpdateCheckboxes() {
    var lst = $.parseJSON($('#hdnSelectedTemplates').val());
    $('[data-template] [type=checkbox]').iCheck("uncheck");
    $(lst).each(function () {
        $('[data-template=' + this + '] [type=checkbox]').iCheck("check");
    });
    $('#hdnSelectedTemplates').val(JSON.stringify(lst));
}

function CheckStatus() {
    var count = $('[data-template] [type=checkbox]:checked').length;
    $('[data-template] [type=checkbox]').prop("disabled", false);
    if (count >= 3) {
        $('[data-template] [type=checkbox]:not(:checked)').prop("disabled", true);
    }
}

function AddRelated(id) {
    var lst = $.parseJSON($('#hdnSelectedTemplates').val());
    var found = false;
    $(lst).each(function (i, n) {
        if (n == id) found = true;
    });
    if (!found) lst.push(id);
    $('#hdnSelectedTemplates').val(JSON.stringify(lst));
}

function RemoveRelated(id) {
    var lst = $.parseJSON($('#hdnSelectedTemplates').val());
    lst = $.grep(lst, function (n) {
        return n != id;
    });
    $('#hdnSelectedTemplates').val(JSON.stringify(lst));
}