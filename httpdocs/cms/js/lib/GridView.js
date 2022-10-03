$(document).ready(function () {
    //iCheck
    $('[id*=chkBxHeader], .egvGrid-select-row input').iCheck({
        checkboxClass: 'icheckbox_square-aero',
        radioClass: 'iradio_square-aero',
        increaseArea: '20%'
    });
    //click
    $('.egvGrid-select-row input').on('ifChecked', function () {
        $(this).parents("tr").addClass("selected");
    });
    $('.egvGrid-select-row input').on('ifUnchecked', function () {
        $(this).parents("tr").removeClass("selected");
    });
    $('[id*=chkBxHeader]').on('ifChecked', function () {
        $('.egvGrid-select-row input').iCheck('check');
    });
    $('[id*=chkBxHeader]').on('ifUnchecked', function () {
        $('.egvGrid-select-row input').iCheck('uncheck');
    });
    $('.egvGrid tr').not(".bg-navy").click(function () {
        $(this).find("input[id*=chkBxSelect]").iCheck("toggle");
    });
    //Toolbar
    $('.egvGrid-select-row input').on("ifChanged", function () {
        UpdateToolbarButton();
    });
    //Column Selection
    $('.egv-col-select a').click(function (e) {
        var alias = $(this).data("columnalias");
        var name = $(this).data("columnname");
        if (!$(this).hasClass("bg-olive")) {
            AddSelectedCol(alias, name);
            $(this).addClass("bg-olive");
        } else {
            RemoveSelectedCol(alias, name);
            $(this).removeClass("bg-olive");
        }
        e.stopPropagation();
    });
});

function AddSelectedCol(alias, name) {
    var lst = $.parseJSON($('#hdnVisibleColumns').val());
    var col = { "ColumnAlias": alias, "ColumnName": name };
    lst.push(col);
    $('#hdnVisibleColumns').val(JSON.stringify(lst));
}

function RemoveSelectedCol(alias, name) {
    var lst = $.parseJSON($('#hdnVisibleColumns').val());
    var newLst = $.grep(lst, function (n) {
        return !(n.ColumnAlias == alias && n.ColumnName == name);
    });
    $('#hdnVisibleColumns').val(JSON.stringify(newLst));
}

function UpdateToolbarButton() {
    var num = 0;
    $('.egvGrid-select-row input').each(function () {
        if ($(this).is(":checked")) num++;
    });
    var none = $('[data-activestate=none]');
    var multi = $('[data-activestate=multi]');
    var single = $('[data-activestate=single]');
    var always = $('[data-activestate=always]');
    if (num == 0) {
        none.removeClass("disabled");
        single.addClass("disabled");
        multi.addClass("disabled");
    } else if (num == 1) {
        none.addClass("disabled");
        single.removeClass("disabled");
        multi.removeClass("disabled");
    } else if (num > 1) {
        none.addClass("disabled");
        single.addClass("disabled");
        multi.removeClass("disabled");
    }
    always.removeClass("disabled");
}