$(document).ready(function () {
    var lst = $.parseJSON($('#hdnCategories').val());
    $(lst).each(function () {
        $('[data-toggle=CollectionCategory][value=' + this + ']').iCheck("check");
    });
    $('[data-toggle=CollectionCategory]').on("ifChecked", function () {
        var val = $(this).val();
        AddToList(parseInt(val));
    }).on("ifUnchecked", function () {
        var val = $(this).val();
        RemoveFromList(parseInt(val));
    });
});

function AddToList(val) {
    var lst = $.parseJSON($('#hdnCategories').val());
    var found = false;
    $(lst).each(function () {
        if (this == val) found = true;
    });
    if (!found) lst.push(val);
    $('#hdnCategories').val(JSON.stringify(lst));
}

function RemoveFromList(val) {
    var lst = $.parseJSON($('#hdnCategories').val());
    lst = $.grep(lst, function (n) {
        return n != val;
    });
    $('#hdnCategories').val(JSON.stringify(lst));
}