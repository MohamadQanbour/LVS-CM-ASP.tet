(function ($) {
    $.fn.ImageEditor = function (options) {
        var args = arguments;
        var wrapperWidth = 400;
        var controls = {
            Image: undefined,
            MaxWidth: undefined,
            MaxHeight: undefined,
            CropType: undefined,
            CropX: undefined,
            CropY: undefined,
            Scale: undefined,
            ImageControl: undefined,
            CropTypeDropDown: undefined,
            Wrapper: undefined
        };

        var flags = {
            ImageEditorInitialized: false,
            ImageEditorLoaded: false,
            DragInitialized: false,
            StateLoaded: false,
            AllowMove: false
        };

        var values = {
            Image: "",
            MaxWidth: 0,
            MaxHeight: 0,
            CropType: 0,
            CropX: 0,
            CropY: 0,
            Scale: 1
        };

        return this.each(function () {
            var obj = $(this);
            if (!flags.ImageEditorLoaded) {
                LoadDefaults(obj);
                LoadValues();
                obj.find('[egvcommand=croptype] option[value=' + values.CropType + ']').prop("selected", true);
            }
            if (typeof options == "string") {
                switch (options) {
                    case "load-image":
                        controls.Image.val(args[1]);
                        LoadValues();
                        LoadImage(obj);
                        break;
                    case "remove-image":
                        controls.Image.val("");
                        LoadValues();
                        LoadImage(obj);
                    case "load-control":
                        obj.slideDown();
                        LoadImage(obj);
                        break;
                    case "show":
                        obj.slideDown();
                        break;
                }
            } else {
                if (!flags.ImageEditorLoaded) {
                    flags.ImageEditorLoaded = true;
                    AttachEvents(obj);
                }
            }
        });

        function LoadDefaults(jo) {
            controls.Image = jo.find('#hdnImageEditorImage');
            controls.MaxWidth = jo.find("#hdnImageEditorMaxWidth");
            controls.MaxHeight = jo.find("#hdnImageEditorMaxHeight");
            controls.CropType = jo.find('#hdnImageEditorCropType');
            controls.CropX = jo.find('#hdnImageEditorCropX');
            controls.CropY = jo.find('#hdnImageEditorCropY');
            controls.Scale = jo.find('#hdnImageEditorScale');
            controls.ImageControl = jo.find(".egv-image-wrapper img");
            controls.CropTypeDropDown = jo.find('#ddlCropType');
            controls.Wrapper = jo.find('.egv-image-wrapper');
        }

        function LoadValues() {
            values.Image = controls.Image.val();
            values.MaxWidth = controls.MaxWidth.val() != "" ? parseFloat(controls.MaxWidth.val()) : 0;
            values.MaxHeight = controls.MaxHeight.val() != "" ? parseFloat(controls.MaxHeight.val()) : 0;
            values.CropType = controls.CropType.val() != "" ? parseInt(controls.CropType.val()) : 1;
            values.CropX = controls.CropX.val() != "" ? parseFloat(controls.CropX.val()) : 0;
            values.CropY = controls.CropY.val() != "" ? parseFloat(controls.CropY.val()) : 0;
            values.Scale = controls.Scale.val() != "" ? parseFloat(controls.Scale.val()) : 1;
        }

        function LoadControls() {
            controls.Image.val(values.Image);
            controls.MaxWidth.val(values.MaxWidth);
            controls.MaxHeight.val(values.MaxHeight);
            controls.CropType.val(values.CropType);
            controls.CropX.val(values.CropX);
            controls.CropY.val(values.CropY);
            controls.Scale.val(values.Scale);
        }

        function LoadImage(jo) {
            LoadValues();
            if (values.Image != "") {
                controls.ImageControl.attr("src", values.Image);
                var img = $('<img>').attr("src", values.Image).load(function () {
                    $('body').append(img);
                    controls.ImageControl.width(img.width()).height(img.height());
                    img.remove();
                    var ratio = values.MaxWidth / values.MaxHeight;
                    var newWidth = wrapperWidth;
                    var newHeight = newWidth / ratio;
                    controls.Wrapper.width(newWidth).height(newHeight);
                    var imgRatio = parseFloat(controls.ImageControl.width()) / parseFloat(controls.ImageControl.height());
                    var newImgWidth = newWidth;
                    var newImgHeight = newImgWidth / imgRatio;
                    if (newImgHeight < newHeight) {
                        newImgHeight = newHeight;
                        newImgWidth = newHeight * imgRatio;
                    }
                    newImgWidth = newImgWidth * values.Scale;
                    newImgHeight = newImgWidth / imgRatio;
                    controls.ImageControl.width(newImgWidth).height(newImgHeight);
                    var resizeRatio = newWidth / values.MaxWidth;
                    var newX = values.CropX * resizeRatio;
                    var newY = values.CropY * resizeRatio;
                    if (newX <= 0) newX = 0;
                    if (newY <= 0) newY = 0;
                    controls.ImageControl.css({
                        "left": -newX,
                        "top": -newY
                    });
                    if (!flags.StateLoaded) {
                        flags.StateLoaded = true;
                        ChangeState(controls.CropTypeDropDown.val(), jo, false);
                    }
                    flags.ImageEditorInitialized = true;
                    var axis = "xy";
                    if (newImgHeight === newHeight) axis = "x";
                    if (newImgWidth === newWidth) axis = "y";
                    var maxAllowedWidth = controls.ImageControl.width() - newWidth;
                    var maxAllowedHeight = controls.ImageControl.height() - newHeight;
                    if (flags.AllowMove) {
                        controls.ImageControl.draggable();
                        controls.ImageControl.draggable("option", "axis", axis);
                        controls.ImageControl.draggable("option", "drag", function (e, ui) {
                            var newLeft = ui.position.left;
                            var newTop = ui.position.top;
                            if (newLeft > 0) newLeft = 0;
                            if (newLeft < maxAllowedWidth * -1) newLeft = maxAllowedWidth * -1;
                            if (newTop > 0) newTop = 0;
                            if (newTop < maxAllowedHeight * -1) newTop = maxAllowedHeight * -1;
                            ui.position.left = newLeft;
                            ui.position.top = newTop;
                        });
                        controls.ImageControl.draggable("option", "stop", function (e, ui) {
                            controls.CropX.val((ui.position.left * -1) / resizeRatio);
                            controls.CropY.val((ui.position.top * -1) / resizeRatio);
                        });
                        flags.DragInitialized = true;
                    } else {
                        if (flags.DragInitialized) controls.ImageControl.draggable("disable");
                    }
                });
            } else jo.slideUp();
        }

        function AttachEvents(jo) {
            jo.find('[egvcommand=zoomin]').click(function () {
                LoadValues();
                values.Scale = values.Scale == undefined ? 1 : values.Scale;
                values.Scale = (parseFloat(values.Scale) + parseFloat(0.1)).toFixed(1);
                LoadControls();
                flags.ImageEditorInitialized = false;
                LoadImage(jo);
            });
            jo.find('[egvcommand=zoomout]').click(function () {
                LoadValues();
                values.Scale = values.Scale == undefined ? 1 : values.Scale;
                values.Scale = (parseFloat(values.Scale) - parseFloat(0.1)).toFixed(1);
                if (values.Scale < parseFloat(1)) {
                    values.Scale = parseFloat(1);
                    values.CropX = 0;
                    values.CropY = 0;
                }
                LoadControls();
                flags.ImageEditorInitialized = false;
                LoadImage(jo);
            });
            jo.find('[egvcommand=moveleft]').click(function () {
                LoadValues();
                var max = controls.ImageControl.width() - wrapperWidth;
                var newLeft = controls.ImageControl.position().left - 10;
                if (newLeft > 0) newLeft = 0;
                if (newLeft < max * -1) newLeft = max * -1;
                controls.ImageControl.css("left", newLeft);
                values.CropX = newLeft;
                LoadControls();
            });
            jo.find('[egvcommand=moveright]').click(function () {
                LoadValues();
                var max = controls.ImageControl.width() - wrapperWidth;
                var newLeft = controls.ImageControl.position().left + 10;
                if (newLeft > 0) newLeft = 0;
                if (newLeft < max * -1) newLeft = max * -1;
                controls.ImageControl.css("left", newLeft);
                values.CropX = newLeft;
                LoadControls();
            });
            jo.find('[egvcommand=moveup]').click(function () {
                LoadValues();
                var ratio = values.MaxWidth / values.MaxHeight;
                var newWidth = wrapperWidth;
                var newHeight = newWidth / ratio;
                var max = controls.ImageControl.height() - newHeight;
                var newTop = controls.ImageControl.position().top - 10;
                if (newTop > 0) newTop = 0;
                if (newTop < max * -1) newTop = max * -1;
                controls.ImageControl.css("top", newTop);
                values.CropY = newTop;
                LoadControls();
            });
            jo.find('[egvcommand=movedown]').click(function () {
                LoadValues();
                var ratio = values.MaxWidth / values.MaxHeight;
                var newWidth = wrapperWidth;
                var newHeight = newWidth / ratio;
                var max = controls.ImageControl.height() - newHeight;
                var newTop = controls.ImageControl.position().top + 10;
                if (newTop > 0) newTop = 0;
                if (newTop < max * -1) newTop = max * -1;
                controls.ImageControl.css("top", newTop);
                values.CropY = newTop;
                LoadControls();
            });
            jo.find('[egvcommand=croptype]').change(function () {
                controls.CropType.val($(this).val());
                LoadValues();
                ChangeState($(this).val(), jo);
            });
        }

        function disableButtons(jo) { jo.find('a[egvcommand]').addClass("disabled"); }
        function enableButtons(jo) { jo.find('a[egvcommand]').removeClass("disabled"); }

        function setTopLeft(jo, reload) {
            values.CropX = 0;
            values.CropY = 0;
            values.Scale = 1;
            LoadControls();
            disableButtons(jo);
            flags.AllowMove = false;
            flags.ImageEditorInitialized = false;
            if (reload) LoadImage(jo);
        }

        function setTopRight(jo, reload) {
            values.CropX = (controls.ImageControl.width() - wrapperWidth) * -1;
            values.CropY = 0;
            values.Scale = 1;
            LoadControls();
            disableButtons(jo);
            flags.AllowMove = false;
            flags.ImageEditorInitialized = false;
            if (reload) LoadImage(jo);
        }

        function setBottomLeft(jo, reload) {
            var ratio = values.MaxWidth / values.MaxHeight;
            var newWidth = wrapperWidth;
            var newHeight = newWidth / ratio;
            values.CropX = 0
            values.CropY = (controls.ImageControl.height() - newHeight) * -1;
            values.Scale = 1;
            LoadControls(jo);
            disableButtons(jo);
            flags.AllowMove = false;
            flags.ImageEditorInitialized = false;
            if (reload) LoadImage(jo);
        }

        function setBottomRight(jo, reload) {
            var ratio = values.MaxWidth / values.MaxHeight;
            var newWidth = wrapperWidth;
            var newHeight = newWidth / ratio;
            values.CropX = (controls.ImageControl.width() - newWidth) * -1;
            values.CropY = (controls.ImageControl.height() - newHeight) * -1;
            values.Scale = 1;
            LoadControls();
            disableButtons(jo);
            flags.AllowMove = false;
            flags.ImageEditorInitialized = false;
            if (reload) LoadImage(jo);
        }

        function setCenter(jo, reload) {
            var ratio = values.MaxWidth / values.MaxHeight;
            var newWidth = parseFloat(wrapperWidth);
            var newHeight = newWidth / ratio;
            var imgRatio = controls.ImageControl.width() / controls.ImageControl.height();
            var newImgWidth = newWidth;
            var newImgHeight = newImgWidth / imgRatio;
            if (newImgHeight < newHeight) {
                newImgHeight = newHeight;
                newImgWidth = newImgHeight * imgRatio;
            }
            values.CropX = ((parseFloat(newImgWidth) - newWidth) / 2) * -1;
            values.CropY = ((parseFloat(newImgHeight) - newHeight) / 2) * -1;
            values.Scale = 1;
            LoadControls();
            disableButtons(jo);
            flags.AllowMove = false;
            flags.ImageEditorInitialized = false;
            if (reload) LoadImage(jo);
        }

        function setManual(jo, reload) {
            enableButtons(jo);
            flags.AllowMove = true;
            flags.ImageEditorInitialized = false;
            if (reload) LoadImage(jo);
        }

        function ChangeState(val, jo, reload) {
            reload = reload == undefined ? true : reload;
            switch (parseInt(val)) {
                case 1:
                    setCenter(jo, reload);
                    break;
                case 2:
                    setTopLeft(jo, reload);
                    break;
                case 3:
                    setTopRight(jo, reload);
                    break;
                case 4:
                    setBottomLeft(jo, reload);
                    break;
                case 5:
                    setBottomRight(jo, reload);
                    break;
                case 6:
                    setManual(jo, reload);
                    break;
            }
        }
    }
})(jQuery);