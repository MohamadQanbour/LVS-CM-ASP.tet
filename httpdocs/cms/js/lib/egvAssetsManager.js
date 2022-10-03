(function ($) {
    $.fn.AssetsManager = function (options, targetPath, targetCallback) {
        var defaults = {
            callbackFunction: undefined,
            ajaxPath: "",
            assetsPath: "",
            cmsPath: "",
            cms: false,
            imageRes: 0,
            maxWidth: 0,
            maxHeight: 0,
            loadingImage: "",
            fileUploadInitialized: false,
            error: false,
            pageindex: 0,
            pagesize: 50,
            pagersize: 10,
            sortcolumn: "",
            sortorder: "",
            showModal: true,
            returnFullURL: false,
            requestPath: ""
        };
        if (typeof options == "object") $.extend(defaults, options);
        this.each(function () {
            am                              =   $(this);
            defaults.ajaxPath               =   am.find('#hdnAMP').val();
            defaults.assetsPath             =   am.find('#hdnAF').val();
            defaults.cmsPath                =   am.find('#hdnCP').val();
            defaults.cms                    =   (am.find('#hdnIsCMS').val().toLowerCase() == "true" ? true : false);
            defaults.imageRes               =   parseInt(am.find('#hdnResolution').val());
            defaults.maxWidth               =   parseInt(am.find('#hdnMaxWidth').val());
            defaults.maxHeight              =   parseInt(am.find('#hdnMaxHeight').val());
            defaults.loadingImage           =   am.find('#hdnLoadingImage').val();
            defaults.fileUploadInitialized  =   false;
            defaults.error                  =   false;
            defaults.pageindex              =   parseInt(0);
            defaults.pagesize               =   parseInt(50);
            defaults.pagersize              =   parseInt(10);
            defaults.sortcolumn             =   "";
            defaults.sortorder              =   "";
            defaults.showModal              =   am.find('#hdnShowModal').val().toLowerCase() == "true" ? true : false;
            defaults.returnFullURL          =   am.find('#hdnReturnFullURL').val().toLowerCase() == "true" ? true : false;
            defaults.requestPath            =   am.find('#hdnRequestPath').val();

            AttachEvents(am);
            UpdateSelected(am);
            if (!defaults.showModal) LoadPathInExplorer("", am);

            if (typeof options == "string") {
                switch (options) {
                    case "openassets":
                        var path = (targetPath != undefined ? targetPath : "");
                        if (targetCallback != undefined) defaults.callbackFunction = targetCallback;
                        OpenAssetsManager(path, am);
                        break;
                    case "closeassets":
                        CloseAssetsManager(am);
                        break;
                }
            }
        });

        function OpenAssetsManager(path, jo) {
            if (path == undefined) path = "";
            LoadPathInExplorer(path, jo);
            jo.modal();
        }

        function CloseAssetsManager(jo) { jo.modal('hide'); }

        function AttachEvents(jo) {
            jo.find('.egv-assets-preview [egvcommand]').off("click").click(function () { destroyPreview(jo); });
            //ok
            jo.find('[egvcommand=ok]').off("click").click(function () {
                var path = jo.find('[egvcommand=selectedfile]').val();
                if (defaults.returnFullURL) path = defaults.requestPath + path;
                if (path == "") return;
                if (defaults.showModal) {
                    if (defaults.callbackFunction != undefined) defaults.callbackFunction(path);
                    CloseAssetsManager(jo);
                } else {
                    if (navigator.appName.indexOf('Microsoft') != -1) window.returnValue = path; else window.opener.setAssetValue(path);
                    self.close();
                }
            });
            //Refresh Button
            jo.find('[egvcommand=refresh]').off("click").click(function () {
                var path = jo.find('[egvcommand=filelist]').attr('path');
                LoadPathInExplorer(path, jo);
            });
            //Create Folder Button
            jo.find('[egvcommand=newfolder]').off("click").click(function () {
                var path = jo.find('[egvcommand=filelist]').attr("path");
                var name = prompt("Please specify folder name:", "untitled");
                if (name != "") {
                    var le = jo.find('.egv-assets-toolbar').loadingEffect(defaults.loadingImage);
                    $.ajax(defaults.ajaxPath + "AddFolder", {
                        type: "GET",
                        data: {
                            "name": name,
                            "folder": path,
                            "cms": defaults.cms
                        },
                        error: function (a, b, c) {
                            le.remove();
                            alert(c);
                        },
                        success: function (a, b, c) {
                            le.remove();
                            jo.find('[egvcommand=refresh]').click();
                        }
                    });
                }
            });
            //Update Files Button
            jo.find('[egvcommand=upload]').off("click").click(function () { ToggleUploadPanel(jo); });
            //Delete Button
            jo.find('[egvcommand=delete]').off("click").click(function () {
                if (confirm("Are you sure you want to delete " + GetSelectedCount(jo) + " files/folders?")) {
                    var lst = GetSelectedItems(jo);
                    while (lst.length > 0) {
                        var item = lst.pop();
                        var func = "";
                        if (item.type == 1) func = "DeleteFolder"; else func = "DeleteFile";
                        var le = jo.find('.egv-assets-toolbar').loadingEffect(defaults.loadingImage);
                        $.ajax(defaults.ajaxPath + func, {
                            type: "GET",
                            data: {
                                "folder": item.path,
                                "name": item.name,
                                "cms": defaults.cms
                            },
                            error: function (a, b, c) { le.remove(); alert(c); },
                            success: function () { le.remove(); LoadPathInExplorer(item.path.replace(item.name, ""), jo); }
                        });
                    }
                }
            });
            //Rename Button
            jo.find('[egvcommand=rename]').off("click").click(function () {
                if (confirm("Are you sure you want to rename the selected file/folder? this might break a link somewhere if the file of folder is linked to an object")) {
                    var lst = GetSelectedItems(jo);
                    var item = lst.pop();
                    var ext = "";
                    var tname = "";
                    if (item.name.lastIndexOf(".") >= 0) {
                        ext = item.name.substr(item.name.lastIndexOf("."));
                        tname = item.name.replace(ext, "");
                    } else tname = item.name;
                    var newName = prompt("Please specify the new name of the selected file/folder", tname);
                    if (newName != null && newName !== tname) {
                        var func = "";
                        if (item.type == 1) func = "RenameFolder"; else func = "RenameFile";
                        var le = jo.find('.egv-assets-toolbar').loadingEffect(defaults.loadingImage);
                        $.ajax(defaults.ajaxPath + func, {
                            type: "GET",
                            data: {
                                "folder": item.path,
                                "name": item.name,
                                "newname": newName + ext,
                                "cms": defaults.cms
                            },
                            error: function (a, b, c) { le.remove(); alert(c); },
                            success: function () { le.remove(); LoadPathInExplorer(item.path.replace(item.name, ""), jo); }
                        });
                    }
                }
            });
            //Search Buttons
            jo.find('[egvcommand=search]').off("click").click(function () {
                defaults.pageindex = 0;
                LoadPathInExplorer($('[egvcommand=filelist]').attr("path"), jo);
            });
            jo.find('[egvcommand=clearsearch]').off("click").click(function () {
                defaults.pageindex = 0;
                jo.find('[egvcommand=txtsearch]').val("");
                LoadPathInExplorer($('[egvcommand=filelist]').attr("path"), jo);
            });
            //Sort Buttons
            jo.find('[egvcommand=sort]').off("click").click(function () {
                var col = $(this).attr("egvargument");
                if (col == defaults.sortcolumn) {
                    if (defaults.sortorder == "asc") defaults.sortorder = "desc";
                    else defaults.sortorder = "asc";
                } else {
                    defaults.sortcolumn = col;
                    defaults.sortorder = "asc";
                }
                LoadPathInExplorer($('[egvcommand=filelist]').attr("path"), jo);
            });
        }

        function LoadBreadcrumb(path, jo) {
            path = path == undefined ? "/" : path;
            var breadcrumb = jo.find('[egvcommand=breadcrumb]');
            if (path.startsWith("/")) path = path.substr(1);
            if (path.endsWith("/")) path = path.substr(0, path.lastIndexOf("/"));
            var lst = path.toString().split('/');
            if (lst.length >= 1) {
                var sp = "";
                breadcrumb.empty();
                breadcrumb.append("/").append($("<a>").attr("href", "javascript:;").attr("path", "").text((defaults.cms ? "cms-" : "") + "assets").off("click").click(function () {
                    var path = $(this).attr("path");
                    pageindex = 0;
                    LoadPathInExplorer(path, jo);
                }));
                $(lst).each(function () {
                    if (this.length > 0 && this != defaults.assetsPath) {
                        sp += "/" + this;
                        breadcrumb.append("/").append($("<a>").attr("href", "javascript:;").attr("path", sp).text(this).off("click").click(function () {
                            var path = $(this).attr("path");
                            pageindex = 0;
                            LoadPathInExplorer(path, jo);
                        }));
                    }
                });
            } else breadcrumb.empty().append("/").append($("<a>").attr("href", "javascript:;").attr("path", "").text("assets").off("click").click(function () {
                var path = $(this).attr("path");
                pageindex = 0;
                LoadPathInExplorer(path, jo);
            }));
        }

        function LoadPathInExplorer(path, jo) {
            var search = jo.find('[egvcommand=txtsearch]').val();
            destroyPreview(jo);
            LoadBreadcrumb(path, jo);
            BuildPager(path, search, jo);
            AdjustSortColumns(jo);
            jo.find('[egvcommand=selectedfile]').val("");
            var target = jo.find('[egvcommand=filelist]');
            target.find(".egv-assets-loading-effect").remove();
            var le = target.loadingEffect(defaults.loadingImage);
            $.ajax(defaults.ajaxPath + "FileList", {
                type: "GET",
                data: {
                    "folder": path,
                    "q": search,
                    "pageindex": defaults.pageindex,
                    "col": defaults.sortcolumn,
                    "ord": defaults.sortorder,
                    "cms": defaults.cms
                },
                dataType: "json",
                error: function (a, b, c) {
                    le.remove();
                    alert(c);
                },
                success: function (a) {
                    var ret = eval(a)[0];
                    if (ret.HasError) {
                        le.remove();
                        alert(ret.ErrorMessage);
                    } else {
                        var d = ret.ReturnData;
                        target.attr("path", path);
                        var targetTable = target.find('table tbody');
                        targetTable.find("tr").not(".egv-assets-header").remove();
                        targetTable.append(RenderUp(jo));
                        var dirs = 0;
                        var files = 0;
                        $(d).each(function () {
                            targetTable.append(RenderFileItem(this, jo));
                            if (this.Type == 1) dirs++;
                            else files++;
                        });
                        le.remove();
                        RenderContentInfo(dirs, files, jo);
                        UpdateSelected(jo);
                    }
                }
            });
        }

        function RenderUp(jo) {
            return $("<tr>").append($("<td>").append(
                $("<input>").attr("type", "checkbox").attr("id", "select_all").attr("name", "select_all").change(function () {
                    if ($(this).is(":checked")) {
                        $(this).parents("tbody").find("tr").not(".selected").find("input[type=checkbox]").not("[id=select_all]").each(function () {
                            if (!$(this).is(":checked")) $(this).click();
                        });
                    } else {
                        $(this).parents("tbody").find("tr.selected").find("input[type=checkbox]").not("[id=select_all]").each(function () {
                            if ($(this).is(":checked")) $(this).click();
                        });
                    }
                })
            )).append(
                    $("<td>").append(
                        $("<a>").attr("href", "javascript:;").off("click").click(function () {
                            var path = jo.find('[egvcommand=filelist]').attr("path");
                            var newPath = path.substring(0, path.lastIndexOf("/"));
                            pageindex = 0;
                            LoadPathInExplorer(newPath,  jo);
                        }).append($("<span>").addClass("fa fa-level-up")).append("...")
                    )
                ).append($("<td>")).append($("<td>"));
        }

        function RenderFileItem(obj, jo) {
            return $("<tr>").append(
                    $("<td>").append(
                        $("<input>").attr("type", "checkbox").attr("id", "select" + ReplaceAll(ReplaceAll(obj.Name, ".", ""), " ", ""))
                        .attr("name", "select" + ReplaceAll(ReplaceAll(obj.Name, ".", ""), " ", "")).attr("path", obj.Path)
                        .attr("filename", obj.Name).attr("filetype", obj.Type).click(function (e) {
                            e.stopPropagation();
                            if ($(this).is(":checked")) $(this).parents("tr").addClass("selected");
                            else $(this).parents("tr").removeClass("selected");
                            UpdateSelected(jo);
                            UpdateCheckAll(jo);
                        })
                    )
                ).append(
                    $("<td>").append(
                        $("<a>").attr("href", "javascript:;").attr("path", obj.Path).off("click").click(function (e) {
                            e.stopPropagation();
                            var path = $(this).attr("path");
                            if (obj.Type == "1") {
                                pageindex = 0;
                                LoadPathInExplorer(path, jo);
                            } else {
                                jo.find('[egvcommand=selectedfile]').val("/" + (defaults.cms ? defaults.cmsPath + "/" : "") + defaults.assetsPath + "/" + obj.Path);
                                emptyPreview(jo);
                                jo.find('.egv-assets-preview section').append(function () {
                                    switch (GetFileType(obj["Name"])) {
                                        case "image":
                                            return $("<img>").attr("src", "/" + (defaults.cms ? defaults.cmsPath + "/" : "") + defaults.assetsPath + "/" + obj["Path"]);
                                            break;
                                        case "audio":
                                            return $("<audio>").prop("controls", true).width("100%").append(
                                                $("<source>").attr("src", "/" + (defaults.cms ? defaults.cmsPath + "/" : "") + defaults.assetsPath + "/" + obj["Path"])
                                            );
                                        case "video":
                                            return $("<video>").width("100%").height("100%").prop("controls", true).append(
                                                $("<source>").attr("src", "/" + (defaults.cms ? defaults.cmsPath + "/" : "") + defaults.assetsPath + "/" + obj["Path"])
                                            );
                                        case "pdf":
                                            return $("<embed>").attr("src", "/" + (defaults.cms ? defaults.cmsPath + "/" : "") + defaults.assetsPath + "/" + obj["Path"]).width("100%").height("100%");
                                        default:
                                            destroyPreview(jo);
                                            return "";
                                    }
                                });
                                openPreview(jo);
                            }
                        }).append(
                            $("<span>").addClass(function () {
                                if (obj.Type == "1") return "fa fa-folder";
                                else return GetListItemIcon(obj.Name);
                            })
                        ).append(obj.Name)
                    )
                ).append(
                    $("<td>").addClass("info").append(function () {
                        if (obj.Type == "2") return obj.FileDate;
                        else return "";
                    })
                ).append(
                    $("<td>").addClass("info").append(function () {
                        if (obj.Type == "2") return obj.FileSize;
                        else return "";
                    })
                ).on("click", function (e) {
                    destroyPreview(jo);
                    if ($(this).hasClass("selected")) {
                        var check = $(this).find('input[type=checkbox]');
                        if (check.is(":checked")) check.click();
                    } else {
                        var check = $(this).find('input[type=checkbox]');
                        if (!check.is(":checked")) check.click();
                    }
                });
        }

        function GetFileType(name) {
            var extension = name.toString().substring(name.lastIndexOf(".") + 1).toLowerCase();
            var myclass = "";
            switch (extension) {
                case "png":
                case "jpg":
                case "jpeg":
                case "gif":
                    myclass = "image";
                    break;
                case "mp3":
                case "wav":
                case "ogg":
                    myclass = "audio";
                    break;
                case "mp4":
                case "mov":
                case "webm":
                    myclass = "video";
                    break;
                case "pdf":
                    myclass = "pdf";
                    break;
                default:
                    myclass = "file";
                    break;
            }
            return myclass;
        }

        function GetListItemIcon(name) {
            var extension = name.toString().substring(name.lastIndexOf(".") + 1).toLowerCase();
            var myclass = "";
            switch (extension) {
                case "xlsx":
                case "xls":
                    myclass = "fa-file-excel-o";
                    break;
                case "png":
                case "jpg":
                case "jpeg":
                case "gif":
                    myclass = "fa-file-image-o";
                    break;
                case "zip":
                case "rar":
                case "gzip":
                    myclass = "fa-file-archive-o";
                    break;
                case "mp3":
                case "wav":
                case "ogg":
                    myclass = "fa-file-audio-o";
                    break;
                case "mp4":
                case "mov":
                case "webm":
                    myclass = "fa-file-movie-o";
                    break;
                case "pdf":
                    myclass = "fa-file-pdf-o";
                    break;
                case "doc":
                case "docx":
                    myclass = "fa-file-word-o";
                    break;
                default:
                    myclass = "fa-file";
                    break;
            }
            return "fa " + myclass;
        }

        function UpdateSelected(jo) {
            var count = GetSelectedCount(jo);
            if (count == 1) {
                jo.find('[selectmode=1], [selectmode=2]').removeClass("disabled")
            } else if (count > 1) {
                jo.find('[selectmode=1]').addClass("disabled");
                jo.find('[selectmode=2]').removeClass("disabled");
            } else jo.find('[selectmode=1], [selectmode=2]').addClass("disabled");
        }

        function GetSelectedCount(jo) { return jo.find("tr.selected").length; }

        function GetSelectedItems(jo) {
            var list = [];
            jo.find('tr.selected').each(function () {
                var target = $(this).find('input[type=checkbox]');
                var type = target.attr("filetype");
                var path = target.attr("path");
                var name = target.attr("filename");
                path = path.replace(name, "");
                list.push({ "path": path, "name": name, "type": type });
            });
            return list;
        }

        function UpdateCheckAll(jo) {
            var count = GetSelectedCount(jo);
            var selectAll = jo.find('#select_all');
            if (count == jo.find('[egvcommand=filelist] tbody tr').find('input[type=checkbox]').not('[id=select_all]').length) {
                if (!selectAll.is(":checked")) selectAll.prop("checked", true);
            } else {
                if (selectAll.is(":checked")) selectAll.prop("checked", false);
            }
        }

        //file upload functions
        function ToggleUploadPanel(jo) {
            destroyPreview(jo);
            if (!defaults.fileUploadInitialized) InitializeFileUpload(jo); else UpdateFileUploadPath(jo);
            var filesList = jo.find("[egvcommand=filelist]");
            var fileUpload = jo.find('[egvcommand=uploadpanel]');
            if (filesList.is(":visible")) {
                jo.find('.egv-assets-pager').hide();
                filesList.slideToggle("fast", function () {
                    fileUpload.slideToggle();
                });
            } else {
                fileUpload.find('tbody.files tr.fade').remove();
                fileUpload.slideToggle("fast", function () {
                    filesList.slideToggle();
                    jo.find('.egv-assets-pager').show();
                });
            }
        }

        function UpdateFileUploadPath(jo) {
            var path = jo.find('[egvcommand=filelist]').attr("path");
            jo.find('[egvcommand=uploadpanel]').fileupload('option', 'url', defaults.ajaxPath + "Upload?cms=" + defaults.cms + "&UploadFilePath=" + path);
        }

        function InitializeFileUpload(jo) {
            var path = jo.find("[egvcommand=filelist]").attr("path");
            var settings = {
                url: defaults.ajaxPath + "Upload?cms=" + defaults.cms + "&UploadFilePath=" + path,
                disableImageResize: (defaults.imageRes > 0 ? /Android(?!.*Chrome)|Opera/.test(window.navigator && navigator.userAgent) : true),
                acceptFileTypes: /(\.|\/)(gif|jpe?g|eps|png|mp3|ogg|wav|mp4|webm|mov|wmv|pdf|xlsx|xls|docx|doc|zip|rar|gzip|ppsx|ppt|pptx)$/i
            };
            if (defaults.maxWidth > 0) settings["imageMaxWidth"] = defaults.maxWidth;
            if (defaults.maxHeight > 0) settings["imageMaxHeight"] = defaults.maxHeight;
            jo.find('[egvcommand=uploadpanel]').fileupload(settings).bind("fileuploaddone", function (e, data) {
                var active = jo.find('[egvcommand=uploadpanel]').fileupload('active');
                var files = data.result.files;
                $(files).each(function () {
                    if (this['error'] != undefined) defaults.error = true;
                });
                if (!defaults.error && active <= 1) {
                    ToggleUploadPanel(jo);
                    pageindex = 0;
                    LoadPathInExplorer(jo.find('[egvcommand=filelist]').attr("path"), jo);
                }
            });
            defaults.fileUploadInitialized = true;
        }

        //Pager
        function BuildPager(path, search, jo) {
            search = search != undefined ? search : "";
            var le = jo.find('[egvcommand=pager]').loadingEffect(defaults.loadingImage);
            $.ajax(defaults.ajaxPath + "FolderCount", {
                type: "GET",
                data: {
                    "folder": path,
                    "q": search,
                    "cms": defaults.cms
                },
                error: function (a, b, c) { alert(c); },
                success: function (d) {
                    var ret = eval(d)[0];
                    if (ret.HasError) {
                        alert(ret.ErrorMessage);
                    } else {
                        var data = ret.ReturnData;
                        var total = parseInt(data);
                        RenderPager(total, defaults.pageindex, jo);
                        le.remove();
                    }
                }
            });
        }

        function RenderPager(total, index, jo) {
            index = index != undefined ? index : Math.floor(parseFloat(defaults.pageindex) / parseFloat(defaults.pagersize));
            var lst = [];
            var numOfPages = Math.ceil(parseFloat(total) / parseFloat(defaults.pagesize));
            var base = parseInt(index);
            if (base > 0) {
                lst.push({ "CommandName": "prev", "CommandArgument": base - 1 });
            }
            var limit = numOfPages > defaults.pagersize ? defaults.pagersize : numOfPages;
            for (var i = 1; i <= limit; i++) {
                if (i + (base * defaults.pagersize) <= numOfPages) {
                    lst.push({ "CommandName": "page", "CommandArgument": i + (base * defaults.pagersize) - 1 });
                }
            }
            if ((base * defaults.pagersize) + defaults.pagersize < numOfPages) {
                lst.push({ "CommandName": "next", "CommandArgument": base + 1 });
            }
            var target = jo.find('[egvcommand=pager]').empty();
            lst.reverse();
            while (lst.length > 0) {
                var item = lst.pop();
                target.append(
                    $("<a>").attr("href", "javascript:;").attr("commandargument", item.CommandArgument)
                    .attr("commandname", item.CommandName).append(function () {
                        if (item.CommandName == "prev") return $("<i>").addClass("glyphicon glyphicon-chevron-left");
                        if (item.CommandName == "page") return parseInt(item.CommandArgument) + 1;
                        if (item.CommandName == "next") return $("<i>").addClass("glyphicon glyphicon-chevron-right");
                    }).click(function () {
                        destroyPreview(jo);
                        var name = $(this).attr("commandname");
                        var argument = $(this).attr("commandargument");
                        if (name == "page") {
                            if (argument != defaults.pageindex) {
                                defaults.pageindex = argument;
                                LoadPathInExplorer(jo.find('[egvcommand=filelist]').attr("path"), jo);
                            }
                        } else if (name == "prev") {
                            RenderPager(total, argument, jo);
                        } else if (name == "next") {
                            RenderPager(total, argument, jo);
                        }
                    }).addClass(function () {
                        if ($(this).attr("commandname") == "page" && $(this).attr("commandargument") == defaults.pageindex) return "active";
                        else return "";
                    })
                );
            }
        }

        //Sort Columns
        function AdjustSortColumns(jo) {
            jo.find('[egvcommand=sort] span').remove();
            if (defaults.sortcolumn.length > 0) {
                jo.find('[egvargument=' + defaults.sortcolumn + ']').append(function () {
                    if (defaults.sortorder.length > 0) {
                        return $("<span>").addClass("glyphicon glyphicon-chevron-" + (defaults.sortorder == "asc" ? "up" : "down"));
                    } else return "";
                });
            }
        }

        //Content Info
        function RenderContentInfo(dirs, files, jo) {
            jo.find('[egvcommand=contentinfo]').empty().html(function () {
                var ret = [];
                if (dirs > 0) ret.push("<b>" + dirs + "</b> Folder" + (dirs > 1 ? "s" : ""));
                if (files > 0) ret.push("<b>" + files + "</b> File" + (files > 1 ? "s" : ""));
                if (ret.length > 0) return ret.join("<br />");
                else return "";
            });
        }

        //general functions
        function ReplaceAll(source, needle, replace) {
            var a = source;
            if (a != undefined) {
                var ret = "";
                while (a.length > 0) {
                    var pos = a.indexOf(needle);
                    if (pos >= 0) {
                        var part = a.substr(0, pos);
                        ret += part + replace;
                        a = a.replace(part + needle, "");
                    } else {
                        ret += a;
                        a = a.replace(a, "");
                    }
                }
                return ret;
            } return "";
        }

        function openPreview(jo) {
            jo.find('.egv-assets-preview').addClass("in");
            jo.find('.egv-assets-preview').animate({ "width": "300px" });
        }

        function destroyPreview(jo) {
            emptyPreview(jo);
            jo.find('.egv-assets-preview').animate({ "width": "0px" }, function () {
                jo.find('.egv-assets-preview').removeClass("in");
            });
        }

        function emptyPreview(jo) {
            jo.find('.egv-assets-preview video').each(function () {
                this.pause();
                delete (this);
                $(this).remove();
            });
            jo.find('.egv-assets-preview audio').each(function () {
                this.pause();
                delete (this);
                $(this).remove();
            });
            jo.find('.egv-assets-preview section').empty();
        }
    }
})(jQuery);

(function ($) {
    $.fn.loadingEffect = function (loadingImage) {
        var pnl;
        $(this).find('.egv-assets-loading-effect').remove();
        this.each(function () {
            var target = $(this);
            pnl = $("<div>").addClass('egv-assets-loading-effect').css({
                width: target.width(),
                height: target.height()
            }).append(
                $("<section>").addClass("text-center")
                .append(
                    $("<img>").attr("src", loadingImage)
                )
            );
            target.prepend(pnl);
        });
        return pnl;
    }
})(jQuery);

$(document).ready(function () {
    $('[egvcommand=assetsmanager]').AssetsManager();
});