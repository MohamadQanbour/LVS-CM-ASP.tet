$(document).ready(function () {
    //open tab that has an error
    ShowErrorTab();
    //focus on first form element
    $('.egv-input-form input[type=text]:not([readonly])').first().focus();
    //Auto hide notifier
    setTimeout(function () {
        var _this = $('.alert.alert-success.alert-dismissible').slideUp("500", function () {
            _this.find('[data-dismiss]').click();
        });
    }, 2000);
    //icheck
    if ($('.egv-savecancel').length) {
        $('.egv-savecancel .btn-primary').click(function () {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate('valSave');
            }
            if (Page_IsValid) {
                $('.egv-savecancel .btn-primary').addClass("disabled");
                $('.egv-savecancel .btn-primary').html("<span class=\"fa fa-spinner fa-spin\"></span>");
                return true;
            } else return false;
        });
    }
    try {
        $('.icheck input').iCheck({
            checkboxClass: 'icheckbox_square-aero',
            radioClass: 'iradio_square-aero',
            increaseArea: '20%'
        });
    } catch (e) { console.log(e.message); }
    //input mask
    if ($("[data-mask]").length) {
        $("[data-mask]").inputmask();
    }
    //date picker
    if ($('.datepicker').length) {
        var options = {
            autoclose: true,
            format: 'yyyy-mm-dd',
            todayHighlight: true
        };
        $('.datepicker').each(function () {
            if ($(this).hasClass("datepicker-rtl")) options["rtl"] = true;
            $(this).datepicker(options).keypress(function () {
                return false;
            });
        });
    }
    //auto complete
    //if ($('[data-autocompletesource]').length) {
    //    $('[data-autocompletesource]').each(function () {
    //        var t = $(this);
    //        t.select2({
    //            allowClear: true,
    //            ajax: {
    //                delay: 250,
    //                url: t.data('autocompletesource'),
    //                data: function(params){
    //                    return {
    //                        "q": params.term,
    //                        "exclude": t.data("excludeid"),
    //                        "allownull": (t.data("allownull").toString().toLowerCase() == "true" ? true: false)
    //                    };
    //                },
    //                processResults: function (data) {
    //                    var d = eval(data)[0];
    //                    if (d.HasError) alert(d.ErrorMessage); else {
    //                        return {results: d.ReturnData}
    //                    }
    //                }
    //            }
    //        });
    //    });
    //}
    if ($('[data-autocompletesource]').length) {
        function GetAutoCompleteParams(elem) {
            var additionalData = {};
            $.each(elem[0].attributes, function (i, e) {
                var name = e.name;
                var val = e.value;
                if (name.startsWith("data-")) {
                    if (name !== "data-excludeid" && name !== "data-allownull" && name !== "data-autocompeltesource") {
                        var paramName = name.replace('data-', '');
                        if (val.startsWith('fn:')) additionalData[paramName] = eval(val.replace('fn:', ''));
                        else additionalData[paramName] = elem.attr(name);
                    }
                }
            });
            return additionalData;
        }
        $('[data-autocompletesource]').each(function () {
            var t = $(this);
            t.select2({
                allowClear: true,
                ajax: {
                    delay: 250,
                    type: "post",
                    url: t.data('autocompletesource'),
                    data: function (params) {
                        return $.extend(GetAutoCompleteParams(t), {
                            "q": params.term,
                            "exclude": t.data("excludeid"),
                            "allownull": (t.data("allownull").toString().toLowerCase() == "true" ? true : false)
                        });
                    },
                    processResults: function (data) {
                        var d = eval(data)[0];
                        console.log(d);
                        if (d.HasError) EGVAlert(d.ErrorMessage, 'danger'); else {
                            return { results: d.ReturnData }
                        }
                    }
                }
            });
        });
    }
    //save cancel
    CheckSaveCancel();
    $('[role=save]').click(function (e) {
        ShowErrorTab();
    });
    //select tab on load
    var tabid = GetParameterValue("select-tab");
    if (tabid != "") {
        $('[data-toggle=tab][href$=Tab' + tabid + ']').click();
    }
    //URL input mask
    $('[data-egvinput=url]').off("keydown").keydown(function (event) {
        if (event.which === 32) {
            event.preventDefault();
            $(this).val($(this).val() + "-");
        }
    }).off("blur").blur(function () {
        $(this).val($(this).val().toString().toLowerCase());
        var allowed = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZذدجحخهعغفقثصضطكمنتالبيسشظزوةىلارؤءئآلآلأأإلإèéÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÑÒÓÔÕÖÙÚÛÜÝàáâãäåæçèéêëìíîïñòóôõöùúûüŞş";
        var newVal = "";
        var val = $(this).val();
        for (i = 0; i < val.length; i++) {
            if (allowed.indexOf(val[i]) >= 0) newVal += val[i]; else newVal += "-";
        }
        newVal = newVal.trim();
        newVal = newVal.replace(/-+/g, "-");
        if (newVal.startsWith("-")) newVal = newVal.substr(1);
        if (newVal.endsWith("-")) newVal = newVal.substr(0, newVal.length - 2);
        $(this).val(newVal);
    });
    $('.sidebar').click(function () {
        var _this = $(this);
        setTimeout(function () {
            if (_this.get(0).scrollHeight > _this.get(0).clientHeight) {
                $('.sidebar-scroller').addClass("in");
            } else {
                $('.sidebar-scroller').removeClass("in");
            }
        },500);
    });
    $('.sidebar-scroller a:first-child').click(function () {
        var scrollTop = $('.sidebar').scrollTop() - 50;
        $('.sidebar').animate({
            "scrollTop": scrollTop
        });
        return false;
    });
    $('.sidebar-scroller a:last-child').click(function () {
        var scrollTop = $('.sidebar').scrollTop() + 50;
        $('.sidebar').animate({
            "scrollTop": scrollTop
        });
        return false;
    });
});

$(window).scroll(function () {
    CheckSaveCancel();
});

function ValidateNumber(obj, val) {
    var ret = true;
    try { var i = parseInt(val.Value); ret = !isNaN(i); } catch (e) { ret = false; }
    val.IsValid = ret;
}

function comparePasswords(obj, val) {
    var ret = true;
    try { ret = $('[id*=' + $(obj).data('controltocompare') + ']').val() == val.Value; } catch (e) { ret = false; }
    val.IsValid = ret;
}

function ReplaceAll(source, needle, replace) {
    return source.replace(new RegExp(needle, "g"), replace);
}

function CheckSaveCancel() {
    if ($('.box-footer').length > 0 && $('.egv-savecancel').length > 0) {
        var doc = parseInt($(window).height());
        var body = parseInt($('body').scrollTop());
        var box = parseInt($('.box-footer').offset().top);
        var height = parseInt($('.egv-savecancel').height());
        if (box > doc && box > (doc + body - height)) $('.egv-savecancel').addClass("sticky");
        else $('.egv-savecancel').removeClass("sticky");
    }
}

function ShowErrorTab() {
    //select first tab that has an error
    var found = false;
    var target = undefined;
    $('.err').each(function () {
        if (!found && $(this).css("display") != "none") {
            found = true;
            target = $(this);
        }
    });
    if (target != undefined) {
        var input = target.parent().find("input, select, textarea").first();
        var tabId = target.parents(".tab-pane").attr("id");
        if (tabId != undefined) {
            $('[data-toggle=tab][href$=' + tabId + ']').click();
        }
        input.focus();
    }
}

function GetParameterValue(param) {
    var url = window.location.href.slice(window.location.href.indexOf("?") + 1).split("&");
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
    return "";
}