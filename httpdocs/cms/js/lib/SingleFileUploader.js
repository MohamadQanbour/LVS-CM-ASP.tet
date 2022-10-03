(function ($) {
    $.fn.SingleFileUploader = function () {
        return this.each(function () {
            var input = $(this).find("input[type=file]");
            var remove = $(this).find('a.btn');
            var info = $(this).find('.help-block');
            var hdn = $(this).find('input[type=hidden]');
            input.change(function () {
                var selected = input.val().toString();
                info.text(selected.substr(selected.lastIndexOf("\\") + 1));
            });
            remove.click(function () {
                input.val("");
                info.text("");
                return false;
            });
        });
    }
})(jQuery);

$(document).ready(function () {
    $('[data-singlefileuploader]').SingleFileUploader();
});