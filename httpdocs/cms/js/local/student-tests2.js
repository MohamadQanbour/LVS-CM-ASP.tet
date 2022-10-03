$(document).ready(function () {
    $('.content [data-toggle="tab"][href="#Tab1"]').click(function () {
        $('#egvStudentBox').hide();
    });
    $('.content [data-toggle="tab"][href="#Tab2"]').click(function () {
        $('#egvMaterialBox').hide();
    });
    $('.content [data-toggle="tab"][href="#Tab3"]').click(function () {
        $('#egvMaterialBox').hide();
        $('#egvStudentBox').hide();
    });
    $('#ddlClass').each(function () {
        var t = $(this);
        t.select2({
            allowClear: false,
            ajax: {
                delay: 250,
                url: t.data("autocompletesource"),
                data: function (params) {
                    return {
                        "q": params.term,
                        "userid": $('#hdnUserId').val()
                    };
                },
                processResults: function (data) {
                    var d = eval(data)[0];
                    if (d.HasError) alert(d.ErrorMessage); else {
                        return { results: d.ReturnData }
                    }
                }
            }
        }).change(function () {
            $('#egvMaterialBox').hide();
            var id = $(this).val();
            var target = $('#ddlSection');
            target.prop("disabled", false);
            target.append($('<option value="0">Loading...</option>'));
            $.ajax("/ajax/CMSAutoComplete/TeacherSections", {
                type: "GET",
                data: {
                    "classid": id,
                    "userid": $('#hdnUserId').val(),
                    "currentyear": true
                },
                error: function (a, b, c) { alert(c); },
                success: function (a) {
                    target.empty();
                    var ret = $.parseJSON(a)[0];
                    if (ret.HasError) alert(ret.ErrorMessage);
                    else {
                        $(ret.ReturnData).each(function () {
                            target.append($('<option value="' + this.id + '">' + this.text + '</option>'));
                        });
                        target.val(target.find("option:first").val()).trigger('change');
                    }
                    checkStatus();
                }
            });
        });
    });
    $('#ddlClassPublish').each(function () {
        var t = $(this);
        t.select2({
            allowClear: false,
            ajax: {
                delay: 250,
                url: t.data("autocompletesource"),
                data: function (params) {
                    return {
                        "q": params.term,
                        "userid": $('#hdnUserId').val()
                    };
                },
                processResults: function (data) {
                    var d = eval(data)[0];
                    if (d.HasError) alert(d.ErrorMessage); else {
                        return { results: d.ReturnData }
                    }
                }
            }
        }).change(function () {
            var id = $(this).val();
            var target = $('#ddlSectionPublish');
            target.prop("disabled", false);
            target.append($('<option value="0">Loading...</option>'));
            $.ajax("/ajax/CMSAutoComplete/TeacherSections", {
                type: "GET",
                data: {
                    "classid": id,
                    "userid": $('#hdnUserId').val(),
                    "currentyear": true
                },
                error: function (a, b, c) { alert(c); },
                success: function (a) {
                    target.empty();
                    var ret = $.parseJSON(a)[0];
                    if (ret.HasError) alert(ret.ErrorMessage);
                    else {
                        $(ret.ReturnData).each(function () {
                            target.append($('<option value="' + this.id + '">' + this.text + '</option>'));
                        });
                        target.val(target.find("option:first").val()).trigger('change');
                    }
                    checkStatus();
                }
            });
        });
    });
    $('#ddlSection').change(function () {
        $('#egvMaterialBox').hide();
        $('#hdnSelectedSection').val($(this).val());
        var id = $('#ddlClass').val();
        var secId = $(this).val();
        var target = $('#ddlMaterial');
        target.prop("disabled", false);
        target.append($('<option value="0">Loading...</option>'));
        $.ajax("/ajax/CMSAutoComplete/TeacherMaterials", {
            type: "GET",
            data: {
                "classid": id,
                "userid": $('#hdnUserId').val(),
                "sectionid": secId
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                target.empty();
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    $(ret.ReturnData).each(function () {
                        target.append($('<option value="' + this.id + '">' + this.text + '</option>'));
                    });
                    target.val(target.find("option:first").val()).trigger('change');
                }
                checkStatus();
            }
        });
    });
    $('#ddlSectionPublish').change(function () {
        $('#hdnSelectedSectionPublish').val($(this).val());
        var id = $('#ddlClassPublish').val();
        var secId = $(this).val();
        var target = $('#ddlMaterialPublish');
        target.prop("disabled", false);
        target.append($('<option value="0">Loading...</option>'));
        $.ajax("/ajax/CMSAutoComplete/TeacherMaterials", {
            type: "GET",
            data: {
                "classid": id,
                "userid": $('#hdnUserId').val(),
                "sectionid": secId
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                target.empty();
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    $(ret.ReturnData).each(function () {
                        target.append($('<option value="' + this.id + '">' + this.text + '</option>'));
                    });
                    target.val(target.find("option:first").val()).trigger('change');
                }
                checkStatus();
            }
        });
    });
    $('#ddlMaterial').change(function () {
        $('#egvMaterialBox').hide();
        checkStatus();
        $('#hdnSelectedMaterial').val($(this).val());
    });
    $('#ddlMaterialPublish').change(function () {
        checkStatus();
        $('#hdnSelectedMaterialPublish').val($(this).val());
        var matId = $(this).val();
        var target = $('#ddlExamPublish');
        target.prop("disabled", false);
        target.append($('<option value="0">Loading...</option>'));
        $.ajax("/ajax/CMSAutoComplete/MaterialExams", {
            type: "GET",
            data: {
                "material_id": matId
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                target.empty();
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    $(ret.ReturnData).each(function () {
                        target.append($('<option value="' + this.id + '">' + this.text + '</option>'));
                    });
                    target.val(target.find("option:first").val()).trigger('change');
                }
                checkStatus();
            }
        });
    });
    $('#ddlExamPublish').change(function () {
        checkStatus();
        $('#hdnSelectedExamPublish').val($(this).val());
    });
    checkStatus();
    BindFunctions();
    LoadData();
});

function checkStatus() {
    var hasclasses = $('#ddlClass option').length > 0;
    var pubhasclasses = $('#ddlClassPublish option').length > 0;
    var hassections = $('#ddlSection option').length > 0;
    var pubhassections = $('#ddlSectionPublish option').length > 0;
    var hasmaterials = $('#ddlMaterial option').length > 0;
    var pubhasmaterials = $('#ddlMaterialPublish option').length > 0;
    var pubhasexams = $('#ddlExamsPublish option').length > 0;
    if (!hasclasses) $('#ddlClass').prop("disabled", true); else $('#ddlClass').prop("disabled", false);
    if (!pubhasclasses) $('#ddlClassPublish').prop("disabled", true); else $('#ddlClassPublish').prop("disabled", false);
    if (hassections && hasclasses) $('#ddlSection').prop("disabled", false); else $('#ddlSection').prop("disabled", true);
    if (pubhassections && pubhasclasses) $('#ddlSectionPublish').prop("disabled", false); else $('#ddlSectionPublish').prop("disabled", true);
    if (hasclasses && hassections && hasmaterials) $('#ddlMaterial').prop("disabled", false); else $('#ddlMaterial').prop("disabled", true);
    if (pubhasclasses && pubhassections && pubhasmaterials) $('#ddlMaterialPublish').prop("disabled", false); else $('#ddlMaterialPublish').prop("disabled", true);
    if (hasclasses && hassections && hasmaterials) $('#lnkLoad1').removeClass("disabled"); else $('#lnkLoad1').addClass("disabled");
    if (pubhasclasses && pubhassections && pubhasmaterials && pubhasexams) $('#lnkPublish').removeClass("disabled"); else $('#lnkPublish').addClass("disabled");
    var hasstudents = $('#ddlStudent option').length > 0;
    if (!hasstudents) $('#ddlStudent').prop("disabled", true); else $('#ddlStudent').prop("disabled", false);
    if (hasstudents) $('#lnkLoad').removeClass("disabled"); else $('#lnkLoad').addClass("disabled");
    var haserror1 = $('#egvMaterialBox input.error').length > 0;
    if (haserror1) $('#lnkSaveMaterial').addClass("disabled"); else $('#lnkSaveMaterial').removeClass("disabled");
    var haserror2 = $('#egvStudentBox input.error').length > 0;
    if (haserror2) $('#lnkSaveStudent').addClass("disabled"); else $('#lnkSaveStudent').removeClass("disabled");
}

function BindFunctions() {
    var maxMark1 = $('#hdnMaterialMaxMark').val();
    if (maxMark1 != undefined) {
        $('#egvMaterialBox [data-type="Number"]').change(function () {
            var val = $(this).val();
            var mymaxmark = $(this).parents('[data-exam]').find('[data-type="maxmark"]').text();
            if (mymaxmark == undefined || mymaxmark == "") mymaxmark = maxMark1;
            if (parseFloat(val) > parseFloat(mymaxmark)) $(this).addClass("error"); else $(this).removeClass("error");
            CalculateFields($(this).parents('tr'));
            checkStatus();
        });
    }
    $('#egvStudentBox [data-maxmark]').each(function () {
        $(this).find('[data-type="Number"]').change(function () {
            var val = $(this).val();
            var maxMark = $(this).parents('[data-maxmark]').data("maxmark");
            var mymaxmark = $(this).parents('[data-exam]').find('[data-type="maxmark"]').text();
            if (mymaxmark == undefined || mymaxmark == "") mymaxmark = maxMark;
            if (parseFloat(val) > parseFloat(mymaxmark)) $(this).addClass("error"); else $(this).removeClass("error");
            CalculateFields($(this).parents('[data-maxmark]'));
            checkStatus();
        });
    });
}

function CalculateFields(jo) {
    //total
    jo.find('[data-type="Total"]').each(function () {
        var related = $.parseJSON('[' + $(this).parent().find('[data-type="itemrelated"]').val() + ']');
        var total = 0;
        $(related).each(function () {
            var val = jo.find('[data-key=' + this + ']').val();
            if (val != "") total = total + parseFloat(val);
        });
        $(this).val(total);
        $(this).parent().find('#hdnValue').val(parseFloat(total).toFixed(2));
    });
    //average
    jo.find('[data-type="Average"]').each(function () {
        var related = $.parseJSON('[' + $(this).parent().find('[data-type="itemrelated"]').val() + ']');
        var total = 0;
        var count = related.length;
        $(related).each(function () {
            var val = jo.find('[data-key=' + this + ']').val();
            if (val != "") total = total + parseFloat(val);
        });
        var v = parseFloat(total / count);
        var intV = parseInt(v);
        if (v - intV > 0) v = v.toFixed(2);
        if (!(v > 0)) v = parseInt(v);
        v = Math.ceil(v);
        $(this).val(v);
        $(this).parent().find('#hdnValue').val(v);
    });
}

function LoadData() {
    var t1 = $('#egvMaterialBox');
    if (t1.length) {
        var lst = $.parseJSON($('#hdnResultsMaterial').val());
        $(lst).each(function () {
            var item = this;
            var target = $('tr[data-student=' + item.StudentId + '] [data-exam=' + item.ItemId + ']');
            target.find('[data-key=' + item.ItemId + ']').val(item.Mark);
            target.find('[data-key=' + item.ItemId + ']').parent().find('#hdnValue').val(item.Mark);
            //var user = item.CreatedUser;
            //var d = item.CreatedDate;
            //if (item.ModifiedUser != "") {
            //    user = item.ModifiedUser;
            //    d = item.ModifiedDate;
            //}
            //target.find('[data-type="user"]').html(user + "<br />" + d).show();
            var maxmark = item.MaxMark;
            var maxmarkcontainer = target.find('[data-type="maxvalue"]');
            if (maxmark > 0) {
                target.find('[data-type="maxmark"]').text(maxmark);
                maxmarkcontainer.show();
            } else maxmarkcontainer.parent().html("&nbsp;");
        });
    }
    var t2 = $('#egvStudentBox');
    if (t2.length) {
        var lst = $.parseJSON($('#hdnResults2').val());
        $(lst).each(function () {
            var item = this;
            var target = $('tr[data-materialid=' + item.MaterialId + '] [data-exam=' + item.ItemId + ']');
            target.find('[data-key=' + item.ItemId + ']').val(item.Mark).parent().find('#hdnValue').val(item.Mark);
            var user = item.CreatedUser;
            var d = item.CreatedDate;
            if (item.ModifiedUser != "") {
                user = item.ModifiedUser;
                d = item.ModifiedDate;
            }
            target.find('[data-type="user"]').html(user + "<br />" + d).show();
            var maxmark = item.MaxMark;
            var maxmarkcontainer = target.find('[data-type="maxvalue"]');
            if (maxmark > 0) {
                target.find('[data-type="maxmark"]').text(maxmark);
                maxmarkcontainer.show();
            } else maxmarkcontainer.parent().html("&nbsp;");
        });
    }
}