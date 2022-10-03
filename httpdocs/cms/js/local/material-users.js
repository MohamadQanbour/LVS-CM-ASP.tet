$(document).ready(function () {
    $('#ddlClasses').change(function () {
        var val = $(this).val();
        $('#ddlSections').empty().append("<option>Loading...</option>");
        $.ajax("/ajax/CMSAutoComplete/Sections", {
            type: "POST",
            data: {
                classid: val,
                currentyear: true
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = eval(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var d = ret.ReturnData;
                    $('#ddlSections').empty();
                    $(d).each(function () {
                        $('#ddlSections').append('<option value="' + this.id + '">' + this.text + '</option>');
                    });
                    $('#hdnSelectedSection').val($('#ddlSections').val());
                }
            }
        });
        $('#egvBox').hide();
    });
    $('#ddlSections').change(function () {
        $('#hdnSelectedSection').val($(this).val());
        $('#egvBox').hide();
    });
    $('[data-materialid] select').change(function () {
        var materialId = $(this).parent().data("materialid");
        var userId = $(this).val();
        ChangeMaterialUser(materialId, userId);
    });
    var lst = $.parseJSON($('#hdnValues').val());
    $(lst).each(function () {
        var materialId = this.MaterialId;
        var userId = this.UserId;
        $('[data-materialid=' + materialId + '] select').val(userId);
    });
});

function ChangeMaterialUser(materialId, userId) {
    var lst = $.parseJSON($('#hdnValues').val());
    $(lst).each(function () {
        if (this.MaterialId == materialId) this.UserId = userId;
    });
    $('#hdnValues').val(JSON.stringify(lst));
}