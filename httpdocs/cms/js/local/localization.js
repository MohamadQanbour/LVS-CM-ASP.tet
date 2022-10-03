$(document).ready(function () {
    //bind buttons events
    $('[data-toggle="tab"]').click(function () {
        $('[data-toggle="tab"]').removeClass("disabled");
        $(this).addClass("disabled");
        $('#hdnType').val($(this).data("command"));
    });
    $('[data-command="filter"]').click(function () {
        $('[data-command="filter"]').removeClass("disabled");
        $(this).addClass("disabled");
        $('#hdnFilter').val($(this).data("argument"));
        var arg = $(this).data("argument");
        if (arg == "untranslated") {
            var target = $('[data-type="translation"]:not(.bg-maroon)');
            target.find(".row").slideUp();
            target.parent().find('[data-command="save"]').slideUp();
        } else {
            $('[data-type="translation"] .row').slideDown();
            $('[data-command="save"]').slideDown();
        }
    });
    $('[data-command="search"]').click(function () {
        var search = $('#txtSearch').val();
        $('#hdnSearch').val(search);
        ApplySearch(search);
        return false;
    });
    $('[data-command="save"]').click(function () {
        var icon = $(this).find("i").attr("class");
        var spinIcon = "fa fa-spinner fa-spin"
        $(this).addClass("disabled").find("i").removeClass(icon).addClass(spinIcon);
        var type = $('#hdnTypeId').val();
        var file = $('#hdnFile').val();
        var id = $(this).data("id");
        var lang = $(this).data("language");
        var userid = $('#hdnUserId').val();
        var target = $(this).parents('tr').find('[data-type="translation"]');
        var value = target.find('input[type="text"]').val();
        var me = $(this);
        $.ajax("/ajax/Translation/Translate", {
            type: "GET",
            data: {
                "userid": userid,
                "type": type,
                "file": file,
                "lang": lang,
                "id": id,
                "value": value
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    if (ret.ReturnData) {
                        target.removeClass("bg-maroon").addClass("bg-orange");
                        me.removeClass("disabled").find("i").removeClass(spinIcon).addClass(icon);
                    }
                }
            }
        });
        return false;
    });
    $('[data-command="savestatic"]').click(function () {
        var icon = $(this).find("i").attr("class");
        var spinIcon = "fa fa-spinner fa-spin"
        $(this).addClass("disabled").find("i").removeClass(icon).addClass(spinIcon);
        var type = $('#hdnTypeId').val();
        var file = $('#hdnFile').val();
        var key = $(this).data("id");
        var lang = $(this).data("language");
        var target = $(this).parents('tr').find('[data-type="translation"]');
        var value = target.find('input[type="text"]').val();
        var me = $(this);
        $.ajax("/ajax/Translation/Translate", {
            type: "GET",
            data: {
                "type": type,
                "file": file,
                "langcode": lang,
                "key": key,
                "value": value
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    if (ret.ReturnData) {
                        target.addClass("bg-orange");
                        me.removeClass("disabled").find("i").removeClass(spinIcon).addClass(icon);
                    }
                }
            }
        });
        return false;
    });
    $('[data-command="saveall"]').click(function () {
        var dynamics = $('[data-command="save"]');
        var statics = $('[data-command="savestatic"]')
        if (dynamics.length) dynamics.click();
        if (statics.length) statics.click();
        $(this).addClass("disabled");
    });
    //load defaults
    LoadDefaults();
});

function LoadDefaults() {
    var filter = $('#hdnFilter').val();
    var search = $('#hdnSearch').val();
    var type = $('#hdnType').val();
    var file = $('#hdnFile').val();

    $('[data-argument="' + filter + '"]').click();
    $('#txtSearch').val(search);
    $('[data-command="search"]').click();
    $('[data-command="' + type + '"]').click();
    LoadFile(file);
}

function ApplySearch(search) {
    $('[data-original]').show();
    if (search != "")
        $('[data-original]:not([data-original*="' + search + '"]').hide();
}

function LoadFile(file) {
    $('[data-item="' + file + '"]').addClass("active");
}