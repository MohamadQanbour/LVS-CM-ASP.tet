$(document).ready(function () {
    if ($('[data-autocompletesource]').length) {
        $('[data-autocompletesource]').each(function () {
            var t = $(this);
            t.select2("destroy").select2({
                allowClear: true,
                ajax: {
                    delay: 250,
                    url: t.data('autocompletesource'),
                    data: function (params) {
                        return {
                            "q": params.term,
                            "userid": t.data("userid"),
                            "usertype": t.data("usertype")
                        };
                    },
                    processResults: function (data) {
                        var d = eval(data)[0];
                        if (d.HasError) alert(d.ErrorMessage); else {
                            return { results: d.ReturnData }
                        }
                    }
                }
            });
        });
    }
    $('#ddlTo').change(function () {
        $('#hdnSelectedTo').val("[" + $(this).val() + "]");
    });
    $('#fileAttachment').change(function () {
        var names = $.map($(this).prop("files"), function (val) { return '<i class="fa fa-paperclip"></i> ' + val.name; });
        $('#selectedFiles').html(names.join("<br />"));
    });
});