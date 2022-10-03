$(document).ready(function () {
    var templateid = $('#ddlTemplate').val();
    UpdateSelectedTemplate($('#ddlTemplate'));
    if (templateid == undefined || templateid == null || templateid == "0") $('#hypEditItems').addClass("disabled"); else $('#hypEditItems').removeClass("disabled");
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
    $('#ddlClass').change(function () {
        var val = $(this).val();
        var target = $('#ddlTemplate');
        target.empty().append($('<option value="0">Loading...</option>'));
        $.ajax("/ajax/CMSAutoComplete/ClassTemplates", {
            type: "GET",
            data: {
                "classid": val
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    target.empty();
                    if (ret.ReturnData.length > 0) {
                        $(ret.ReturnData).each(function () {
                            target.append('<option value="' + this.id + '">' + this.text + '</option>');
                        });
                    } else {
                        target.empty();
                    }
                }
                $('#ddlTemplate').change();
            }
        });
    });
    $('#ddlTemplate').change(function () {
        UpdateSelectedTemplate($(this));
    });
    $('#hypEditItems').click(function () {
        var text = $(this).html();
        $(this).html("<span class=\"fa fa-spin fa-spinner\"></span>").addClass("disabled");
        var me = $(this);
        var items = JSON.parse($('#hdnItems').val());
        if (items.length == 0) {
            $.ajax("/ajax/CMSMisc/MaterialExamItems", {
                type: "GET",
                data: {
                    "templateid": $('#ddlTemplate').val(),
                    "materialid": $('#hdnMaterialId').val(),
                    "maxmark": $('#txtTotalScore').val()
                },
                error: function (a, b, c) { alert(c); },
                success: function (a) {
                    var ret = $.parseJSON(a)[0];
                    if (ret.HasError) alert(ret.ErrorMessage);
                    else {
                        $('#hdnItems').val(JSON.stringify(ret.ReturnData));
                        me.html(text).removeClass("disabled");
                        UpdateItems(ret.ReturnData);
                        $('#egvItemsModal').modal('show');
                    }
                }
            });   
        } else {
            me.html(text).removeClass("disabled");
            UpdateItems(items);
            $('#egvItemsModal').modal('show');
        }
        return false;
    });
    $('#hypAddItems').click(function () {
        SaveItems();
        $('#egvItemsModal').modal('hide');
    });
});

function UpdateSelectedTemplate(jo) {
    var val = jo.val();
    $('#hdnSelectedTemplate').val(val);
    $('#hdnItems').val("[]");
    if ($('#txtTotalScore').val() === "") {
        jo.removeClass("error");
        $('[role=save]').addClass("disabled");
        if (val != null && val != undefined && val != "0") {
            $.ajax("/ajax/CMSAutoComplete/TemplateMaxMark", {
                type: "GET",
                data: {
                    "templateid": val
                },
                error: function (a, b, c) { alert(c); },
                success: function (a) {
                    var ret = $.parseJSON(a)[0];
                    if (ret.HasError) alert(ret.ErrorMessage);
                    else {
                        $('#txtTotalScore').val(ret.ReturnData);
                        $('[role=save]').removeClass("disabled");
                    }
                }
            });
            $('#hypEditItems').removeClass("disabled");
        } else { jo.empty().addClass("error"); $('#hypEditItems').addClass("disabled"); }
    }
}

function UpdateItems(items) {
    if (items == undefined) items = $.parseJSON($('#hdnItems').val());
    var target = $('#tblMarks tbody');
    var firstTr = target.find("tr").first().html();
    target.empty();
    target.append($('<tr>' + firstTr + '</tr>'));
    $(items).each(function () {
        var item = this;
        var tr = $("<tr>").attr("data-key", item.TemplateItemId).append(
            $('<td>').attr("data-type", "title").addClass("bg-olive").text(item.TemplateItemTitle)
        ).append(
            $("<td>").addClass("form-group").append(
                $('<input>')
                    .attr("type", "text")
                    .attr("data-type", "value")
                    .addClass("form-control")
                    .val(item.MaxMark)
                    .attr("data-mask", "")
                    .attr("data-inputmask", "'alias': 'integer'")
            )
        );
        target.append(tr);
    });
    $("[data-mask]").inputmask();
}

function SaveItems() {
    var target = $('#tblMarks');
    var lst = [];
    target.find('[data-key]').each(function () {
        var item = {
            "MaterialId": $('#hdnMaterialId').val(),
            "TemplateId": $('#ddlTemplate').val(),
            "TemplateItemId": $(this).data("key"),
            "MaxMark": $(this).find('[data-type="value"]').val(),
            "TemplateItemTitle": $(this).find('[data-type="title"]').text()
        };
        lst.push(item);
    });
    $('#hdnItems').val(JSON.stringify(lst));
}