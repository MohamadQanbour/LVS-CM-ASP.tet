(function ($) {
    $.fn.ImageSelector = function (options) {
        var defaults = {};
        if (typeof options == "object") $.extend(defaults, options);
        this.each(function () {
            var jo = $(this);
            UpdateImagePreviewPath(jo.find("[type=text]").val(), jo);
            jo.find("[data-openassets]").click(function () {
                var path = $(this).data("openassets");
                var target = $(this).data("target");
                if (jo.find("." + target).val() != "") {
                    var newpath = jo.find('.' + target).val();
                    newpath = newpath.substr(0, newpath.lastIndexOf("/"));
                    path = newpath.replace(path, "");
                }
                jo.parent().find('[egvcommand=assetsmanager]').AssetsManager("openassets", path, function (selected) {
                    var target = jo.find('.' + jo.find('[data-openassets]').data("target"));
                    target.val(selected);
                    UpdateImagePreviewPath(target.val(), jo);
                });
            });
            jo.find('[type=text]').blur(function () {
                UpdateImagePreviewPath($(this).val(), jo);
            });
            jo.find('a[data-assetpreview]').prettyPhoto({
                social_tools: '',
                allow_resize: true
            });
        });

        function UpdateImagePreviewPath(value, jo) {
            jo.find("a[data-assetpreview]").attr("href", value);
        }
    }
})(jQuery);

$(document).ready(function () {
    $('.egv-image-selector').ImageSelector();
});