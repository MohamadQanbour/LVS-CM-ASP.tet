$(document).ready(function () {
    UpdateCheckedItems();
    $('#btnAdd').click(function () {
        $('[id$=modalAdd]').modal();
    });
    $('[id$=rowRelations]').hide();
    $('[id$=ddlItemType]').change(function () {
        var val = parseInt($(this).val());
        if (val > 1) $('[id$=rowRelations]').slideDown(); else $('[id$=rowRelations]').slideUp();
        $('#hdnSelectedRelations').val("[]");
        UpdateCheckedItems();
    });
    $('[data-id] [type=checkbox]').on("ifChecked", function () {
        var id = $(this).parents('[data-id]').data("id");
        AddRelated(id);
    }).on("ifUnchecked", function () {
        var id = $(this).parents('[data-id]').data("id");
        RemoveRelated(id);
    });
    $('[data-editid] [type=checkbox]').on("ifChecked", function () {
        var id = $(this).parents('[data-editid]').data("editid");
        AddEditRelated(id);
    }).on("ifUnchecked", function () {
        var id = $(this).parents('[data-editid]').data("editid");
        RemoveEditRelated(id);
    });
    $('a[data-key]').click(function () {
        var spin = "fa fa-spinner fa-spin"
        var icon = $(this).find('span').attr("class");
        $(this).addClass("disabled").find("span").removeClass(icon).addClass(spin);
        var id = $(this).data("key");
        var me = $(this);
        $.ajax("/ajax/CMSMisc/TemplateItemRelations", {
            type: "GET",
            data: {
                "id": id
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var lst = ret.ReturnData;
                    $('#hdnSelectedEditRelations').val(JSON.stringify(lst));
                    $('#hdnEditId').val(id);
                    LoadEditModal();
                    me.removeClass("disabled").find("span").removeClass(spin).addClass(icon);
                }
            }
        });
        return false;
    });
    $('[data-field="Type"]').change(function () {
        UpdateEditRelatedButton();
    });
    UpdateEditRelatedButton();
});

function UpdateCheckedItems() {
    var lst = $.parseJSON($('#hdnSelectedRelations').val());
    $('[data-id] [type=checkbox]').iCheck("uncheck");
    $(lst).each(function () {
        var val = this;
        $('[data-id=' + val + '] [type=checkbox]').iCheck("check");
    });
}

function UpdateEditCheckedItems() {
    var lst = $.parseJSON($('#hdnSelectedEditRelations').val());
    $('[data-editid] [type=checkbox]').iCheck("uncheck");
    $(lst).each(function () {
        var val = this;
        $('[data-editid=' + val + '] [type=checkbox]').iCheck("check");
    });
}

function LoadEditModal() {
    UpdateEditCheckedItems();
    $('[id$=modalEdit]').modal();
}

function AddRelated(id) {
    var lst = $.parseJSON($('#hdnSelectedRelations').val());
    var found = false;
    $(lst).each(function (i, n) {
        if (n == id) found = true;
    });
    if (!found) lst.push(id);
    $('#hdnSelectedRelations').val(JSON.stringify(lst));
}

function RemoveRelated(id) {
    var lst = $.parseJSON($('#hdnSelectedRelations').val());
    lst = $.grep(lst, function (n) {
        return n != id;
    });
    $('#hdnSelectedRelations').val(JSON.stringify(lst));
}

function AddEditRelated(id) {
    var lst = $.parseJSON($('#hdnSelectedEditRelations').val());
    var found = false;
    $(lst).each(function (i, n) {
        if (n == id) found = true;
    });
    if (!found) lst.push(id);
    $('#hdnSelectedEditRelations').val(JSON.stringify(lst));
}

function RemoveEditRelated(id) {
    var lst = $.parseJSON($('#hdnSelectedEditRelations').val());
    lst = $.grep(lst, function (n) {
        return n != id;
    });
    $('#hdnSelectedEditRelations').val(JSON.stringify(lst));
}

function UpdateEditRelatedButton() {
    $('[data-field="Type"]').each(function () {
        if ($(this).val() == 1) {
            $(this).parents("tr").find(".btn-default[data-key]").addClass("disabled");
        } else $(this).parents("tr").find(".btn-default[data-key]").removeClass("disabled");
    });
}