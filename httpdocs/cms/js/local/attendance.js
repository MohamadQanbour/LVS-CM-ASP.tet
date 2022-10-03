$(document).ready(function () {
    if ($('#ddlClasses').prop("disabled") || $('#ddlSections').prop("disabled")) $('#btnLoad').addClass("disabled"); else $('#btnLoad').removeClass("disabled");
    $('#ddlClasses').change(function () {
        var val = $(this).val();
        $('#ddlSections').empty().append("<option>Loading...</option>");
        $.ajax("/ajax/CMSAutoComplete/SectionsAttendance", {
            type: "POST",
            data: {
                classid: val,
                currentyear: true,
                userid: $('#hdnUserId').val()
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = eval(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var d = ret.ReturnData;
                    $('#ddlSections').empty();
                    if (d.length > 0) {
                        $('#ddlSections').prop("disabled", false);
                        $(d).each(function () {
                            $('#ddlSections').append('<option value="' + this.id + '">' + this.text + '</option>');
                        });
                        $('#hdnSelectedSection').val($('#ddlSections').val());
                        $('#btnLoad').removeClass("disabled");
                    } else {
                        $('#btnLoad').addClass("disabled");
                        $('#ddlSections').prop("disabled", true);
                    }
                }
            }
        });
        $('#egvBox').hide();
    });
    $('#ddlSections').change(function () {
        $('#hdnSelectedSection').val($(this).val());
        $('#egvBox').hide();
    });
    if ($('#hdnValues').length) {
        var lst = $.parseJSON($('#hdnValues').val());
        $(lst).each(function () {
            var item = this;
            if (item.StudentAttend) {
                $('#chkAttend' + item.StudentId).iCheck("check");
            }
        });
    }
    $('[data-type="attend"]').on("ifChecked", function () {
        var studentId = $(this).data("studentid");
        var attend = $(this).is(":checked");
        ToggleStudentAttend(studentId, attend);
    }).on("ifUnchecked", function () {
        var studentId = $(this).data("studentid");
        var attend = $(this).is(":checked");
        ToggleStudentAttend(studentId, attend);
    });
});

function ToggleStudentAttend(studentId, attend) {
    var lst = $.parseJSON($('#hdnValues').val());
    $(lst).each(function () {
        var item = this;
        if (item.StudentId == studentId) item.StudentAttend = attend;
    });
    $('#hdnValues').val(JSON.stringify(lst));
}