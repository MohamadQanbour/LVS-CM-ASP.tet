(function ($) {
    $.fn.FileUploader = function (options) {
        var args = arguments;

        var defaults = {
            UploadPath: "",
            CallbackFunction: undefined
        };
        var params = {
            AjaxURL: "",
            AssetsPath: "",
            CMSPath: "",
            CMSUpload: false,
            ImageResolution: 0,
            MaxWidth: 0,
            MaxHeight: 0,
            Limit: 0,
            FileUploadError: false,
            FileUploaderInitialized: false
        };

        if (typeof options == "object") $.extend(defaults, options);

        return this.each(function () {
            var obj = $(this);
            if (!params.FileUploaderInitialized) {
                params.FileUploaderInitialized = true;
                LoadParameters(obj);
                InitializeUploader(obj);
            }

            if (typeof options == "string") {
                switch (options) {
                    case "change-path":
                        defaults.UploadPath = args[1];
                        InitializeUploader(obj, true);
                        break;
                    case "change-callback":
                        defaults.CallbackFunction = args[1];
                        InitializeUploader(obj, true);
                        break;
                }
            }
        });

        function LoadParameters(jo) {
            params.AjaxURL = jo.find('#hdnAMP').val();
            params.AssetsPath = jo.find('#hdnAF').val();
            params.CMSPath = jo.find('#hdnCP').val();
            params.CMSUpload = jo.find('#hdnIsCMS').val();
            params.ImageResolution = jo.find('#hdnResolution').val();
            params.MaxWidth = jo.find('#hdnMaxWidth').val();
            params.MaxHeight = jo.find('#hdnMaxHeight').val();
            params.Limit = jo.find('#hdnLimit').val();
            defaults.UploadPath = jo.find('#hdnCurFilePath').val();
        }

        function InitializeUploader(jo, destroy) {
            destroy = destroy != undefined ? destroy : false;
            var settings = {
                url: params.AjaxURL + "Upload?cms=" + params.CMSUpload + "&UploadFilePath=" + defaults.UploadPath,
                disableImageResize: (params.ImageResolution > 0 ? /Android(?!.*Chrome)|Opera/.test(window.navigator && navigator.userAgent) : true),
                acceptFileTypes: /(\.|\/)(gif|jpe?g|eps|png|mp3|ogg|wav|mp4|webm|mov|wmv|pdf|xlsx|xls|docx|doc|zip|rar|gzip|ppsx|ppt|pptx)$/i
            };
            if (params.MaxWidth > 0) settings.imageMaxWidth = params.MaxWidth;
            if (params.MaxHeight > 0) settings.imageMaxHeight = params.MaxHeight;
            if (params.Limit > 0) settings.maxNumberOfFiles = params.Limit;
            if (destroy) jo.find('[egvcommand=fileuploader').fileupload('destroy');
            jo.find('[egvcommand=fileuploader]').fileupload(settings).bind("fileuploaddone", function (e, data) {
                var active = jo.find('[egvcommand=fileuploader]').fileupload('active');
                var files = data.result.files;
                $(files).each(function () {
                    if (this['error'] != undefined) params.FileUploadError = true;
                });
                if (!params.FileUploadError && active <= 1 && defaults.CallbackFunction != undefined) {
                    defaults.CallbackFunction(data.result.files);
                }
            });
        }
    }
})(jQuery);