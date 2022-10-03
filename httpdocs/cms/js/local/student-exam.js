$(document).ready(function () {
    CheckVisibleStatus();
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
        var id = $('#ddlClass').val();
        var secId = $(this).val();
        var target = $('#ddlMaterial');
        target.append($('<option value="0">Loading.==..</option>'));
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
                CheckVisibleStatus();
            }
        });
    });
    $('#ddlSectionPublish').change(function () {
        var id = $('#ddlClassPublish').val();
        var secId = $(this).val();
        var target = $('#ddlMaterialPublish');
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
                CheckVisibleStatus();
            }
        });
    });
    //material
    $('#ddlMaterial').change(function () {
        $('[data-command="box"]').hide();
        var id = $(this).val();
        var target = $('#ddlTemplateItem');
        target.append($('<option value="0">Loading...</option>'));
        $.ajax('/ajax/CMSAutoComplete/MaterialExams', {
            type: 'GET',
            data: { "material_id": id, "allownull": true, "onlynumbers": true },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                target.empty();
                var ret = JSON.parse(a)[0];
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
    $('#ddlMaterialPublish').change(function () {
        var id = $(this).val();
        var target = $('#ddlExamsPublish');
        target.append($('<option value="0">Loading...</option>'));
        $.ajax("/ajax/CMSAutoComplete/MaterialExams", {
            type: "GET",
            data: { "material_id": id, "onlynumbers": false, "allownull": false },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                target.empty();
                var ret = JSON.parse(a)[0];
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
    //load
    $('[data-command="load"][data-argument="material"]').click(function () {
        var _this = $(this);
        var html = _this.html();
        _this.html('<span class="fa fa-spinner fa-spin"></span>').addClass("disabled");
        $.ajax("/ajax/CMSMisc/LoadExamMaterialInfo", {
            type: "GET",
            data: {
                "material": $('#ddlMaterial').val()
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = JSON.parse(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var d = ret.ReturnData;
                    $('[data-command="material-max-mark-text"]').text(d);
                }
            }
        });
        $.ajax("/ajax/CMSMisc/LoadExamMaterialResults", {
            type: "GET",
            data: {
                "class": $('#ddlClass').val(),
                "section": $('#ddlSection').val(),
                "material": $('#ddlMaterial').val(),
                "item": $('#ddlTemplateItem').val()
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = JSON.parse(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var d = ret.ReturnData;
                    $('[data-command="box"][data-argument="material"]').attr("data-materialid", $('#ddlMaterial').val());
                    $('[data-command="box"][data-argument="material"] .box-title').text($('#ddlClass option:selected').text() + " *-* " + $('#ddlSection option:selected').text() + " - " + $('#ddlMaterial option:selected').text());
                    var target = $('[data-command="box"][data-argument="material"] table tbody');
                    ClearRows(target);
                    $(d).each(function () {
                        var item = this;
                        target.append(AddMaterialRow(item));
                    });
                    _this.html(html);
                    _this.removeClass("disabled");
                    $('[data-command="box"][data-argument="material"]').slideDown();
                    BindEvents();
                    $('[data-student]').each(function (i, v) {
                        CalculateFields($(v));
                    });
                }
            }
        });
        return false;
    });
    $('[data-command="load"][data-argument="student"]').click(function () {
        var studentId = $('#ddlStudent').val();
        var _this = $(this);
        var html = _this.html();
        _this.html('<span class="fa fa-spinner fa-spin"></span>').addClass("disabled");
        $.ajax("/ajax/CMSMisc/StudentClassId", {
            type: "GET",
            data: { "student": studentId },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = JSON.parse(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var d = ret.ReturnData;
                    $('[data-command="box"][data-argument="student"]').attr("data-student", studentId).attr("data-classid", d);
                    $.ajax("/ajax/CMSMisc/StudentExams", {
                        type: "GET",
                        data: {
                            "student": studentId,
                            "user": $('#hdnUserId').val(),
                            "class": d
                        },
                        error: function (a, b, c) { alert(c); },
                        success: function (a) {
                            var ret = JSON.parse(a)[0];
                            if (ret.HasError) alert(ret.ErrorMessage);
                            else {
                                $('[data-command="box"][data-argument="student"] .box-title').text($('#ddlStudent option:selected').text());
                                var target = $('[data-command="box"][data-argument="student"] table tbody');
                                ClearRows(target);
                                $(ret.ReturnData).each(function () {
                                    var item = this;
                                    target.append(AddStudentRow(item));
                                });
                                _this.html(html);
                                _this.removeClass("disabled");
                                $('[data-command="box"][data-argument="student"]').slideDown();
                                BindEvents();
                            }
                        }
                    });
                }
            }
        });
        return false;
    });
    function SaveExamMark(lst, indx, total, type) {
        var data = lst[indx];
        $.ajax("/ajax/CMSMisc/SaveExamMark", {
            type: "GET",
            data: data,
            error: function (a, b, c) { alert(a + ' - ' + b + ' - ' + c); },
            success: function (a) {
                var ret = JSON.parse(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    var d = ret.ReturnData;
                    if (d) {
                        IncreaseProgress(type, total);
                        indx = indx + 1;
                        if (indx >= lst.length - 1) {
                            $('[data-command="progress"]').hide();
                            var msg = $('#hdnSaveSuccess').val();
                            var cont = $('<div>').addClass("alert alert-success").css({ "float": "none", "position": "static" }).text(msg);
                            $('[data-command="box"][data-argument="' + type + '"]').parents('.tab-pane').append(cont);
                            setTimeout(function () { cont.remove(); }, 3000);
                        } else {
                            setTimeout(function () { SaveExamMark(lst, indx, total, type); }, 500);
                        }
                    }
                }
            }
        });
    }
    //save
    $('[data-command="save"][data-argument="material"]').click(function () {
        var _this = $(this);
        var html = _this.html();
        _this.html('<span class="fa fa-spinner fa-spin"></span>').addClass("disabled");
        var exams = [];
        $('[data-command="box"][data-argument="material"] input[type="text"][data-changed="yes"]').each(function () {
            var _this = $(this);
            var mark = _this.val();
            var studentId = _this.parents('[data-student]').attr("data-student");
            var materialId = $('[data-command="box"][data-argument="material"]').attr("data-materialid");
            var itemid = _this.attr("data-key");
            var userId = $('#hdnUserId').val();
            exams.push({
                "student": studentId,
                "material": materialId,
                "exam": itemid,
                "mark": mark,
                "user": userId
            });
        });
        var total = $('input[type="text"][data-type]').length;
        $('[data-command="box"]').slideUp();
        $('[data-command="progress"][data-argument="material"]').slideDown();
        $('[data-command="progress"][data-argument="material"] .progress-bar').attr("aria-valuenow", 0).css("width", "0%");
        var i = 0;
        SaveExamMark(exams, i, total, "material");
        //$(exams).each(function () {
        //    var data = this;
        //    setTimeout(function () {
        //        $.ajax("/ajax/CMSMisc/SaveExamMark", {
        //            type: "GET",
        //            data: data,
        //            error: function (a, b, c) { alert(a + ' - ' + b + ' - ' + c); },
        //            beforeSend: function () { setTimeout(1000); },
        //            success: function (a) {
        //                var ret = JSON.parse(a)[0];
        //                if (ret.HasError) alert(ret.ErrorMessage);
        //                else {
        //                    var d = ret.ReturnData;
        //                    if (d) {
        //                        IncreaseProgress("material", total);
        //                        i = i + 1;
        //                        if (i >= exams.length - 1) {
        //                            $('[data-command="progress"]').hide();
        //                            var msg = $('#hdnSaveSuccess').val();
        //                            var cont = $('<div>').addClass("alert alert-success").css({ "float": "none", "position": "static" }).text(msg);
        //                            $('[data-command="box"][data-argument="material"]').parents('.tab-pane').append(cont);
        //                            setTimeout(function () { cont.remove(); }, 3000);
        //                        }
        //                    }
        //                }
        //            }
        //        });
        //    }, 1000);
        //});
        _this.html(html).removeClass("disabled");
        return false;
    });
    $('[data-command="save"][data-argument="student"]').click(function () {
        var _this = $(this);
        var html = _this.html();
        _this.html('<span class="fa fa-spinner fa-spin"></span>').addClass("disabled");
        var exams = [];
        $('[data-command="box"][data-argument="student"] input[type=text][data-changed="yes"]').each(function () {
            var _this = $(this);
            var mark = _this.val();
            var studentId = $('[data-command="box"][data-argument="student"]').attr("data-student");
            var materialId = _this.parents('[data-materialid]').attr("data-materialid");
            var itemid = _this.attr("data-key");
            var userId = $('#hdnUserId').val();
            exams.push({
                "student": studentId,
                "material": materialId,
                "exam": itemid,
                "mark": mark,
                "user": userId
            });
        });
        var total = $('input[type="text"][data-type]').length;
        $('[data-command="box"]').slideUp();
        $('[data-command="progress"][data-argument="student"]').slideDown();
        $('[data-command="progress"][data-argument="student"] .progress-bar').attr("aria-valuenow", 0).css("width", "0%");
        var i = 0;
        SaveExamMark(exams, i, total, "student");
        //$(exams).each(function () {
        //    var data = this;
        //    setTimeout(function () {
        //        $.ajax("/ajax/CMSMisc/SaveExamMark", {
        //            type: "GET",
        //            data: data,
        //            error: function (a, b, c) { alert(a + ' - ' + b + ' - ' + c); },
        //            success: function (a) {
        //                var ret = JSON.parse(a)[0];
        //                if (ret.HasError) alert(ret.ErrorMessage);
        //                else {
        //                    var d = ret.ReturnData;
        //                    if (d) {
        //                        IncreaseProgress("student", total);
        //                        i = i + 1;
        //                        if (i >= exams.length - 1) {
        //                            $('[data-command="progress"]').hide();
        //                            var msg = $('#hdnSaveSuccess').val();
        //                            var cont = $('<div>').addClass("alert alert-success").css({ "float": "none", "position": "static" }).text(msg);
        //                            $('[data-command="box"][data-argument="student"]').parents('.tab-pane').append(cont);
        //                            setTimeout(function () { cont.remove(); }, 3000);
        //                        }
        //                    }
        //                }
        //            }
        //        });
        //    }, 1000);
        //});
        _this.html(html).removeClass("disabled");
        return false;
    });
    //publish
    $('[data-command="publish"]').click(function () {
        var _this = $(this);
        var html = _this.html();
        _this.html('<span class="fa fa-spinner fa-spin"></span>').addClass("disabled");
        $.ajax("/ajax/CMSMisc/PublishMarks", {
            type: "GET",
            data: {
                "material": $('#ddlMaterialPublish').val(),
                "exam": $('#ddlExamsPublish').val(),
                "section": $('#ddlSectionPublish').val()
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = JSON.parse(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    if (ret.ReturnData) alert($('#hdnPublishSuccess').val());
                    _this.html(html).removeClass("disabled");
                }
            }
        });
        return false;
    });
});

function IncreaseProgress(type, total) {
    var target = $('[data-command="progress"][data-argument="' + type + '"] .progress-bar');
    var curValue = target.attr("aria-valuenow");
    if (isNaN(curValue)) curValue = 0;
    var step = parseFloat(100 / parseInt(total));
    curValue = parseFloat(curValue) + step;
    target.attr("aria-valuenow", curValue).css("width", curValue + "%");
}

function BindEvents() {
    $("[data-mask]").inputmask();
    $('[data-command="box"] input[type="text"][data-type]').change(function () {
        $(this).attr("data-changed", "yes");
    });
    //material
    var materialMaxMark = $('[data-command="material-max-mark-text"]').text();
    $('[data-command="box"][data-argument="material"] [data-type="Number"]').change(function () {
        var val = $(this).val();
        var mymaxmark = $(this).parents('[data-exam]').find('[data-type="maxmark"]').text();
        if (mymaxmark == undefined || mymaxmark == "") mymaxmark = materialMaxMark;
        if (parseFloat(val) > parseFloat(mymaxmark)) $(this).addClass("error"); else $(this).removeClass("error");
        CalculateFields($(this).parents('[data-student]'), true);
        CheckSaveStatus();
    });
    //student
    $('[data-command="box"][data-argument="student"] [data-maxmark]').each(function () {
        $(this).find('[data-type="Number"]').change(function () {
            var val = $(this).val();
            var maxMark = $(this).parents('[data-maxmark]').attr("data-maxmark");
            var mymaxmark = $(this).parents('[data-exam]').find('[data-type="maxmark"]').text();
            if (mymaxmark == undefined || mymaxmark == "") mymaxmark = maxMark;
            if (parseFloat(val) > parseFloat(mymaxmark)) $(this).addClass("error"); else $(this).removeClass("error");
            CalculateFields($(this).parents('[data-maxmark]'), true);
            CheckSaveStatus();
        });
    });
}

function CheckSaveStatus() {
    if ($('[data-command="box"][data-argument="material"] .error').length > 0) $('[data-command="save"][data-argument="material"]').addClass("disabled"); else $('[data-command="save"][data-argument="material"]').removeClass("disabled");
    if ($('[data-command="box"][data-argument="student"] .error').length > 0) $('[data-command="save"][data-argument="student"]').addClass("disabled"); else $('[data-command="save"][data-argument="student"]').removeClass("disabled");
}

function CheckVisibleStatus() {
    $('[data-command="box"]').hide();
    if ($('#ddlClass').val() == "0" || $('#ddlSection').val() == "0" || $('#ddlMaterial').val() == "0") $('[data-argument="material"]').addClass("disabled"); else $('[data-argument="material"]').removeClass("disabled");
    if ($('#ddlClass').val() == "0") $('#ddlSection').prop("disabled", true); else $('#ddlSection').prop("disabled", false);
    if ($('#ddlSection').val() != undefined && $('#ddlSection').val() != "0") $('#ddlMaterial').prop("disabled", false); else $('#ddlMaterial').prop("disabled", true);
    if ($('#ddlClassPublish').val() == "0" || $('#ddlSectionPublish').val() == "0" || $('#ddlMaterialPublish').val() == "0" || $('#ddlExamsPublish').val() == "0") $('[data-command="publish"]').addClass("disabled"); else $('[data-command="publish"]').removeClass("disabled");
    $('#ddlSectionPublish').prop("disabled", $('#ddlClassPublish').val() == "0");
    $('#ddlMaterialPublish').prop("disabled", !($('#ddlSectionPublish').val() != undefined && $('#ddlSectionPublish').val() != "0"));
    $('#ddlExamsPublish').prop("disabled", !($('#ddlMaterialPublish').val() != undefined && $('#ddlMaterialPublish').val() != "0"));
}

function CheckCardinality(item, lst) {
    var cardinality = 0;
    $.each(item.related, function (i, v) {
        var target = $.grep(lst, function (d) { return d.key === v; });
        if (target.length === 1) {
            target = target[0];
            if (target.type === "Total" || target.type === "Average") {
                cardinality += 1;
                cardinality += CheckCardinality(target, lst);
            }
        }
    });
    return cardinality;
}

function CalculateFields(jo, repeat) {
    var lst = [];
    jo.find('[data-key]').each(function (i,v) {
        var me = $(v);
        var key = parseInt(me.attr('data-key'));
        var type = me.attr('data-type');
        var related = [];
        if (type === "Total" || type === "Average") related = JSON.parse('[' + me.parent().find('[data-type="itemrelated"]').val() + ']');
        lst.push({
            element: me,
            key: key,
            type: type,
            related: related
        });
    });
    var operated = [];
    $.each(lst, function (i, v) {
        if (v.type === "Total" || v.type === "Average") {
            operated.push({
                element: v.element,
                key: v.key,
                type: v.type,
                related: v.related,
                cardinality: CheckCardinality(v, lst)
            });
        }
    });
    var cardinality = 0;
    while (operated.length > 0) {
        var current = [];
        var updated = [];
        while (operated.length > 0) {
            var item = operated.pop();
            if (item.cardinality === cardinality) current.push(item); else updated.push(item);
        }
        operated = updated;
        $.each(current, function (i, v) {
            if (v.type === "Total") {
                var total = 0;
                var old = parseFloat(v.element.val());
                $.each(v.related, function (j, b) {
                    var target = jo.find('[data-key="' + b + '"]');
                    var val = target.val();
                    if (val !== "") total = total + parseFloat(val);
                });
                v.element.val(total);
                if (total != old) v.element.attr('data-changed', 'yes');
            } else if (v.type === "Average") {
                var count = v.related.length;
                var total = 0;
                var old = parseFloat(v.element.val());
                $.each(v.related, function (j, b) {
                    var target = jo.find('[data-key="' + b + '"]');
                    var val = target.val();
                    if (val !== "") total = total + parseFloat(val);
                });
                var cv = parseFloat(total / count);
                var intV = parseInt(cv);
                if (cv - intV > 0) cv = cv.toFixed(2);
                if (!(cv > 0)) cv = parseInt(cv);
                cv = Math.ceil(cv);
                v.element.val(cv);
                if (cv != old) v.element.attr("data-changed", "yes");
            }
        });
        cardinality += 1;
    }
    //repeat = repeat != undefined ? repeat : false;
    //var secondRound = [];
    ////total
    //jo.find('[data-type="Total"]').each(function () {
    //    var me = $(this);
    //    var related = $.parseJSON('[' + me.parent().find('[data-type="itemrelated"]').val() + ']');
    //    var total = 0;
    //    var old = parseFloat(me.val());
    //    var forSecondRound = false;
    //    $(related).each(function () {
    //        if (!forSecondRound) {
    //            var target = jo.find('[data-key=' + this + ']');
    //            var val = target.val();
    //            if (target.attr('data-type') == "Number") {
    //                if (val != "") total = total + parseFloat(val);
    //            } else {
    //                secondRound.push(me);
    //                forSecondRound = true;
    //            }
    //        }
    //    });
    //    if (!forSecondRound) {
    //        me.val(total);
    //        if (total != old) me.attr("data-changed", "yes");
    //    }
    //});
    ////average
    //jo.find('[data-type="Average"]').each(function () {
    //    var me = $(this);
    //    var related = $.parseJSON('[' + me.parent().find('[data-type="itemrelated"]').val() + ']');
    //    var total = 0;
    //    var count = related.length;
    //    var old = parseFloat(me.val());
    //    var forSecondRound = false;
    //    $(related).each(function () {
    //        if (!forSecondRound) {
    //            var target = jo.find('[data-key=' + this + ']');
    //            var val = target.val();
    //            if (target.attr('data-type') == "Number") {
    //                if (val != "") total = total + parseFloat(val);
    //            } else {
    //                secondRound.push(me);
    //                forSecondRound = true;
    //            }
    //        }
    //    });
    //    if (!forSecondRound) {
    //        var v = parseFloat(total / count);
    //        var intV = parseInt(v);
    //        if (v - intV > 0) v = v.toFixed(2);
    //        if (!(v > 0)) v = parseInt(v);
    //        v = Math.ceil(v);
    //        me.val(v);
    //        if (v != old) me.attr("data-changed", "yes");
    //    }
    //});
    //if (secondRound.length > 0) {
    //    $.each(secondRound, function (i, item) {
    //        var related = $.parseJSON('[' + item.parent().find('[data-type="itemrelated"]').val() + ']');
    //        var total = 0;
    //        var count = related.length;
    //        var old = parseFloat(item.val());
    //        $(related).each(function () {
    //            var target = jo.find('[data-key=' + this + ']');
    //            var val = target.val();
    //            if (val != "") total = total + parseFloat(val);
    //        });
    //        if (item.attr('data-type') == "Total") {
    //            item.val(total);
    //            if (total != old) item.attr("data-changed", "yes");
    //        }
    //        if (item.attr('data-type') == "Average") {
    //            var v = parseFloat(total / count);
    //            var intV = parseInt(v);
    //            if (v - intV > 0) v = v.toFixed(2);
    //            if (!(v > 0)) v = parseInt(v);
    //            v = Math.ceil(v);
    //            item.val(v);
    //            if (v != old) item.attr("data-changed", "yes");
    //        }
    //    });
    //}
    //if (repeat) CalculateFields(jo, false);
}

function ClearRows(table) {
    table.find('tr:not([data-command="title"])').remove();
}

function AddStudentRow(d) {
    var canUpdate = $('#hdnCanUpdate').val() == "True";
    var tr = $('<tr>');
    tr.attr("data-maxmark", d.MaxMark).attr("data-materialid", d.Id);
    tr.append($('<td>').addClass("bg-olive valign-top").html(d.MaterialTitle + "<br /><small>" + $('#hdnMaxMarkResource').val() + ": <b>" + d.MaxMark + "</b></small>"));
    var td = $('<td>').attr("colspan", "3");
    tr.append(td);
    $(d.Exams).each(function () {
        var item = this;
        var div = $('<div>').addClass("row").attr("data-exam", item.Id);
        div.append(
            $('<div>').addClass("col-md-8").append(
                $('<div>').append(
                    $('<span>').addClass("control-label").text(item.Title)
                )
            ).append(
                $('<div>').append(
                    $('<input>').attr("type", "hidden").attr("data-type", "itemrelated").val(item.Related)
                ).append(
                    $('<input>').attr("type", "text")
                        .attr("data-type", item.Type).attr("data-key", item.Id).addClass("form-control text-right")
                        .attr("data-inputmask", "'alias': 'decimal'").attr("data-mask", "").val(item.Mark.Mark)
                        .prop("disabled", item.Type != "Number" || !canUpdate)
                )
            )
        ).append(
            $('<div>').addClass("col-md-1").append($('<br>')).append(
                $('<div>').addClass('help-block text-small').append(
                    $('<div>').attr("data-type", "maxvalue").append(
                        $('<b>').attr("data-type", "maxmark").text(item.MaxMark)
                    )
                )
            )
        ).append(
            $('<div>').addClass("col-md-3").append($('<br>')).append(
                $('<div>').addClass("help-block text-small").attr("data-type", "user").html(getUserText(item.Mark))
            )
        );
        td.append(div);
    });
    return tr;
}

function AddMaterialRow(d) {
    var canUpdate = $('#hdnCanUpdate').val() == "True";
    var tr = $('<tr>');
    tr.attr("data-student", d.Id);
    tr.append($('<td>').addClass("bg-olive valign-top").text(d.IdName));
    var td = $('<td>').attr("colspan", 3);
    tr.append(td);
    $(d.Exams).each(function () {
        var item = this;
        var div = $('<div>').attr("data-exam", item.Id).addClass("row");
        td.append(div);
        div.append(
            $('<div>').addClass("col-md-8").append(
                $('<div>').append('<span>').addClass("control-label").text(item.Title)
            ).append(
                $('<div>').append(
                    $('<input>').attr("type", "hidden").attr("data-type", "itemrelated").val(item.Related)
                ).append(
                    $('<input>').attr("type", "text")
                        .attr("data-type", item.Type).attr("data-key", item.Id).addClass("form-control text-right")
                        .attr("data-inputmask", "'alias': 'decimal'").attr("data-mask", "").val(item.Mark.Mark)
                        .prop("disabled", item.Type != "Number" || !canUpdate)
                )
            )
        ).append(
            $('<div>').addClass("col-md-1").append($('<br>')).append(
                $('<div>').addClass('help-block text-small').append(
                    $('<div>').attr("data-type", "maxvalue").append(
                        $('<b>').attr("data-type", "maxmark").text(item.MaxMark)
                    )
                )
            )
        ).append(
            $('<div>').addClass("col-md-3").append($('<br>')).append(
                $('<div>').addClass("help-block text-small").attr("data-type", "user").html(getUserText(item.Mark))
            )
        );
    });
    return tr;
}

function getUserText(exam) {
    var username = "";
    var examdate = "";
    if (exam.ModifiedUser != "" && exam.ModifiedUser != null) {
        username = exam.ModifiedUser;
        examdate = exam.ModifiedDate;
    } else if (exam.CreatedUser != "" && exam.CreatedUser != null) {
        username = exam.CreatedUser;
        examdate = exam.CreatedDate;
    }
    if (username != "") return "<b>" + username + "</b><br />" + examdate; else return "";
}