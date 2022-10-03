$(document).ready(function () {
    //permissions
    var lst = $.parseJSON($('#hdnPermissions').val());
    $(lst).each(function () {
        var item = this;
        var pId = item.PermissionId;
        if (item.CanRead) $('#chkRead' + pId).iCheck("check");
        if (item.CanWrite) $('#chkWrite' + pId).iCheck("check");
        if (item.CanModify) $('#chkModify' + pId).iCheck("check");
        if (item.CanPublish) $('#chkPublish' + pId).iCheck("check");
        if (item.CanDelete) $('#chkDelete' + pId).iCheck("check");
    });
    $('[data-permission]').on('ifChecked', function () {
        var pid = $(this).data("permission");
        var type = $(this).data("type");
        AddPermissionType(pid, type);
    }).on('ifUnchecked', function () {
        var pid = $(this).data("permission");
        var type = $(this).data("type");
        RemovePermissionType(pid, type);
    });
    //languages
    var langs = $.parseJSON($('#hdnLanguages').val());
    $(langs).each(function (i, n) {
        $('#chkLanguage' + n).iCheck("check");
    });
    $('[data-language]').on('ifChecked', function () {
        var id = $(this).data('language');
        AddLanguage(id);
    }).on('ifUnchecked', function () {
        var id = $(this).data('language');
        RemoveLanguage(id);
    });
});

function AddPermissionType(pid, type) {
    var lst = $.parseJSON($('#hdnPermissions').val());
    var found = false;
    $(lst).each(function () {
        if (this.PermissionId == pid) {
            this["Can" + type] = true;
            found = true;
        }
    });
    if (!found) {
        var item = { "RoleId": 0, "PermissionId": pid, "CanRead": false, "CanWrite": false, "CanModify": false, "CanPublish": false, "CanDelete": false };
        item["Can" + type] = true;
        lst.push(item);
    }
    $('#hdnPermissions').val(JSON.stringify(lst));
}

function AddLanguage(id) {
    var lst = $.parseJSON($('#hdnLanguages').val());
    var found = false;
    $(lst).each(function (i,n) {
        if (n == id) found = true;
    });
    if (!found) lst.push(id);
    $('#hdnLanguages').val(JSON.stringify(lst));
}

function RemovePermissionType(pid, type) {
    var lst = $.parseJSON($('#hdnPermissions').val());
    $(lst).each(function () {
        if (this.PermissionId == pid) this["Can" + type] = false;
    });
    $('#hdnPermissions').val(JSON.stringify(lst));
}

function RemoveLanguage(id) {
    var lst = $.parseJSON($('#hdnLanguages').val());
    lst = $.grep(lst, function (n) {
        return n != id;
    });
    $('#hdnLanguages').val(JSON.stringify(lst));
}