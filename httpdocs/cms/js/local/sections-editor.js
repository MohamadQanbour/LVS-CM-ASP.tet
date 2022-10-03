$(document).ready(function () {
    $('#ddlClass').change(function () {
        $('#hdnSelectedClass').val($(this).val());
    });
    $('#lnkAddClass').click(function () {
        var me = $(this);
        if (Page_ClientValidate("valAddClass")) {
            var code = $(this).parents('.modal-content').find('#txtClassCode').val();
            var title = $(this).parents('.modal-content').find('#txtClassTitle').val();
            var btn = $(this);
            var text = btn.html();
            btn.html('<i class="fa fa-spinner fa-spin"></i>');
            $.ajax("/ajax/AutoAdd/StudyClass", {
                type: "POST",
                data: {
                    "code": code,
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
                        $('#ddlClass').append(
                            $("<option>").attr("value", data.id).text(data.text)
                        ).val(data.id).trigger("change");
                        $('#hdnSelectedClass').val(data.id);
                        me.parents('.modal').modal('hide');
                    }
                    btn.html(text);
                }
            });
        }
        return false;
    });
});