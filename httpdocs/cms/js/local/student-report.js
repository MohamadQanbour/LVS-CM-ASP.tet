$(document).ready(function () {
    CheckVisibleStatus();
    BuildTemplates();
    //classes
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
            $('[data-command="box"]').hide();
            var id = $(this).val();
            $('#hdnSelectedClass').val(id);
            BuildTemplates();
            var target = $('#ddlSection');
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
                    CheckVisibleStatus();
                }
            });
        });
    });
    //sections
    $('#ddlSection').change(function () {
        $('[data-command="box"]').hide();
        $('#hdnSeletedSection').val($(this).val());
    });
}).on('change', '[data-toggle="template-item"]', function () {
    var id = $(this).attr('itemid');
    var lst = JSON.parse($('#hdnSelectedTemplates').val());
    if ($(this)[0].checked) {
        if ($.inArray(id, lst) < 0) lst.push(id);
    } else {
        lst = $.grep(lst, function (b) {
            return b != id;
        });
    }
    $('#hdnSelectedTemplates').val(JSON.stringify(lst));
}).on('click', '#lnkLoadStudents', function () {
    var valid = true;
    valid = valid & ($('#hdnSelectedClass').val() != "" && $('#hdnSelectedClass').val() != "0");
    valid = valid & ($('#hdnSelectedSection').val() != "" && $('#hdnSelectedSection').val() != "0");
    var lst = JSON.parse($('#hdnSelectedTemplates').val());
    if (lst.length <= 0) alert('Please select template items');
    valid = valid & (lst.length > 0);
    valid = valid == 1 || valid == true ? true : false;
    return valid;
});

function CheckVisibleStatus() {
    if ($('#ddlClass').val() == "0") $('#ddlSection').prop("disabled", true);
    else $('#ddlSection').prop("disabled", false);
}

function BuildTemplates() {
    $('#hdnSelectedTemplates').val('[]');
    var tbl = $('#tblTemplates');
    tbl.empty();
    tbl.html('<span class="fa fa-spin fa-spinner"></span>');
    $.ajax('/ajax/CMSAutoComplete/ClassExamItems', {
        type: 'POST',
        data: { 'class_id': $('#hdnSelectedClass').val() },
        error: function (a, b, c) { alert(c); },
        success: function (a) {
            var ret = JSON.parse(a)[0];
            if (ret.HasError) alert(ret.ErrorMessage);
            else {
                var d = ret.ReturnData;
                tbl.empty();
                $(d).each(function (i, item) {
                    tbl.append(function () {
                        return $('<div>').addClass('col-md-3').append(function () {
                            return $('<input>').attr('type', 'checkbox').attr('itemid', item.id).attr('data-toggle', 'template-item');
                        }).append(function () {
                            return $('<label>').html('&nbsp;' + item.text);
                        });
                    });
                });
            }
        }
    });
}