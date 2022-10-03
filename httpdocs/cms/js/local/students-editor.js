$(document).ready(function () {
    $('#ddlFamily').change(function () {
        $('#hdnSelectedFamily').val($(this).val());
    });
    $('#lnkAddFamily').click(function () {
        var me = $(this);
        if (Page_ClientValidate("valAddFamily")) {
            var username = $(this).parents('.modal-content').find('#txtFamilyUserName').val();
            var btn = $(this);
            var text = btn.html();
            btn.html('<i class="fa fa-spinner fa-spin"></i>');
            $.ajax("/ajax/AutoAdd/Family", {
                type: "POST",
                data: {
                    "username": username,
                    "language": $('#hdnLanguageId').val()
                },
                error: function (a, b, c) {
                    alert(c);
                    btn.html(text);
                },
                success: function (a, b, c) {
                    var ret = $.parseJSON(a)[0];
                    if (ret.HasError) alert(ret.ErrorMessage);
                    else {
                        var data = ret.ReturnData;
                        $('#ddlFamily').append(
                            $("<option>").attr("value", data.id).text(data.text)
                        ).val(data.id).trigger("change");
                        $('#hdnSelectedFamily').val(data.id);
                        me.parents('.modal').modal('hide');
                    }
                    btn.html(text);
                }
            });
        }
        return false;
    });
    $('#ddlArea').change(function () {
        $('#hdnSelectedArea').val($(this).val());
    });
    $('#lnkAddArea').click(function () {
        var me = $(this);
        if (Page_ClientValidate("valAddArea")) {
            var title = $(this).parents('.modal-content').find('#txtAreaTitle').val();
            var btn = $(this);
            var text = btn.html();
            btn.html('<i class="fa fa-spinner fa-spin"></i>');
            $.ajax("/ajax/AutoAdd/Area", {
                type: "POST",
                data: {
                    "title": title,
                    "language": $('#hdnLanguageId').val()
                },
                error: function (a, b, c) {
                    alert(c);
                    btn.html(text);
                },
                success: function (a, b, c) {
                    var ret = $.parseJSON(a)[0];
                    if (ret.HasError) alert(ret.ErrorMessage);
                    else {
                        var data = ret.ReturnData;
                        $('#ddlArea').append(
                            $("<option>").attr("value", data.id).text(data.text)
                        ).val(data.id).trigger("change");
                        $('#hdnSelectedArea').val(data.id);
                        me.parents('.modal').modal('hide');
                    }
                    btn.html(text);
                }
            });
        }
        return false;
    });
});