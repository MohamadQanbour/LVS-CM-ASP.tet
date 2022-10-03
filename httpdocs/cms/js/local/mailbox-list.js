$(document).ready(function () {
    $('.mailbox-messages input[type="checkbox"]').iCheck({
        checkboxClass: 'icheckbox_square-aero',
        radioClass: 'iradio_square-aero'
    });
    $(".checkbox-toggle").click(function () {
        var clicks = $(this).data('clicks');
        if (clicks) {
            $(".mailbox-messages input[type='checkbox']").iCheck("uncheck");
            $(".fa", this).removeClass("fa-check-square-o").addClass('fa-square-o');
        } else {
            $(".mailbox-messages input[type='checkbox']").iCheck("check");
            $(".fa", this).removeClass("fa-square-o").addClass('fa-check-square-o');
        }
        $(this).data("clicks", !clicks);
    });
    $('.mailbox-messages input[type="checkbox"]').on("ifToggled", function () {
        var id = $(this).data("id");
        ToggleValue(id);
    });
    $(".mailbox-star").click(function (e) {
        e.preventDefault();
        var $this = $(this).find("a > i");
        var $a = $this.parent();
        var fa = $this.hasClass("fa");
        var isStarred = $this.hasClass("fa-star");
        $this.removeClass("fa-star").removeClass("fa-star-o").addClass("fa-spinner fa-spin");
        $.ajax("/ajax/Mailbox/StarMessage", {
            type: "GET",
            data: {
                "message_id": $a.data("id"),
                "star": !isStarred,
                "message_type": $('#hdnMessageType').val()
            },
            error: function (a, b, c) { alert(c); },
            success: function (a) {
                var ret = $.parseJSON(a)[0];
                if (ret.HasError) alert(ret.ErrorMessage);
                else {
                    if (ret.ReturnData) $this.removeClass("fa-spinner fa-spin").addClass((isStarred ? "fa-star-o" : "fa-star"));
                }
            }
        });        
    });
});

function ToggleValue(id) {
    var lst = $('#hdnSelected').val().toString();
    if (lst.length > 0) lst = lst.split(","); else lst = [];
    var found = false;
    var newLst = [];
    for (i = 0; i < lst.length; i++) {
        if (!found && lst[i] == id) {
            newLst = $.grep(lst, function (n) {
                return n != id;
            });
            found = true;
        }
    }
    if (!found) {
        lst.push(id);
        newLst = lst;
    }
    $('#hdnSelected').val(newLst.join(","));
}