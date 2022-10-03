$(document).ready(function () {
    $('[data-type="student"]').change(function () {
        $('#hdnStudent').val($(this).val())
    });
    $('#ddlYear').change(function () {
        var Year = $(this).val();
        var s = $('#hdnStudent').val()
        var target = $('#ddlTerm');
        target.append($('<option value="0">Loading...</option>'));
        $.ajax("/ajax/CMSAutoComplete/STerm", {
            type: "GET",
            data: {
                "Year": Year,
                "SID": s
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                target.empty();
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    $(ret.ReturnData).each(function () {
                        target.append($('<option value="' + this.text + '">' + this.text + '</option>'));
                    });
                    target.val(target.find("option:first").val()).trigger('change');
                }
            }
        });
    })

});
$('#ddlTerm').change(function () {
    var Year = $('#ddlYear').val();
    var Term = $('#ddlTerm').val();
    var s = $('#hdnStudent').val();
    var target = $('#hdnClass');
    //alert("The Term is " + Term)
    //target.append($('<option value="0">Loading...</option>'));
    $.ajax("/ajax/CMSAutoComplete/SClass", {
        type: "GET",
        data: {
            "Term": Term,
            "Year": Year,
            "SID": s
        },
        error: function (a, b, c) { alert(c); },
        success: function (a) {
            target.empty();
            var ret = $.parseJSON(a)[0];
            if (ret.HasError) alert(ret.ErrorMessage);
            else {
                $(ret.ReturnData).each(function () {
                    target.val($(this).val());
                });
                target.val(target.find("option:first").val()).trigger('change');
            }
        }
    });
    alert("My class is : " + target.val());
}).on('click', '[data-toggle="load"]', function () {
    var html = $(this).html();
    $(this).html('<span class="fa fa-spin fa-spinner"></span>').addClass('disabled');
    var studentId = parseInt($('#hdnStudent').val());
    var id = $(this).val();
    if (studentId == NaN || studentId == 0) {
        $('#pnlError').addClass('in');
        $(this).html(html).removeClass('disabled');
        return false;
    } else {
        $('#pnlError').removeClass('in');
    }
}).on('click', '[data-toggle="add-note"]', function () {
    var note = $('[data-type="note"]').val();
    var date = $('[data-type="note-date"]').val();
    var userId = $('#hdnUserId').val();
    var studentId = $('#hdnStudent').val();
    var $this = $(this);
    var html = $this.html();
    $this.html('<span class="fa fa-spin fa-spinner"></span>').addClass('disabled');
    $.ajax('/ajax/CMSMisc/AddInternalNote', {
        type: 'POST',
        data: {
            "user": userId,
            "student": studentId,
            "note": note,
            "date": date
        },
        error: function (a, b, c) { alert(c); },
        success: function (a) {
            var ret = JSON.parse(a)[0];
            if (ret.HasError) alert(ret.ErrorMessage);
            else {
                var d = ret.ReturnData;
                var target = $('.internal-notes');
                target.prepend(function () {
                    return $('<div>').addClass('row').addClass(function () {
                        if (target.children('.row').length % 2 == 0) return 'row-alt-bg';
                    }).append(function () {
                        return $('<div>').addClass('col-md-12').append(function () {
                            return $('<div>').addClass('row note-header').append(function () {
                                return $('<div>').addClass('col-sm-6').html('<b>' + d.UserName + '</b>')
                            }).append(function () {
                                return $('<div>').addClass('col-sm-6 text-right').html(d.NoteDate);
                            });
                        }).append(function () {
                            return $('<div>').addClass('row marginTop10').append(function () {
                                return $('<div>').addClass('col-sm-12 note-text').text(d.Note);
                            });
                        });
                    });
                });
            }
        },
        complete: function () {
            $this.removeClass('disabled').html(html);
        }
    });
    return false;
});