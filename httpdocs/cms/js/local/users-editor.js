$(document).ready(function () {
    //selected role
    $('#ddlRole').change(function () {
        $('#hdnSelectedRole').val($(this).val());
    });
    $('.egv-image-editor').ImageEditor();
    //roles
    //var roles = $.parseJSON($('#hdnRoles').val());
    //$(roles).each(function (i, n) {
    //    $('#chkRole' + n).iCheck("check");
    //});
    //$('[data-role]').on('ifChecked', function () {
    //    var id = $(this).data('role');
    //    AddRole(id);
    //}).on('ifUnchecked', function () {
    //    var id = $(this).data('role');
    //    RemoveRole(id);
    //});
    //fileuploader
    $('.egv-file-uploader-container').FileUploader({
        CallbackFunction: function (files) {
            $('#egvFileUploader').slideToggle();
            var name = files[0].name.replace(/\\/g, "");
            $('.egv-image-editor').ImageEditor("load-image", name);
            $('.egv-image-editor').ImageEditor("show");
        }
    });
    //toolbar
    $('#btnImage').click(function () {
        $('#egvFileUploader').slideToggle();
    });
    $('#btnRemoveImage').click(function () {
        $('.egv-image-editor').ImageEditor("remove-image");
    });
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if ($(e.target).attr("href") == "#Tab3") {
            $('.egv-image-editor').ImageEditor("load-control");
        }
    })
});

function AddRole(id) {
    var lst = $.parseJSON($('#hdnRoles').val());
    var found = false;
    $(lst).each(function (i, n) {
        if (n == id) found = true;
    });
    if (!found) lst.push(id);
    $('#hdnRoles').val(JSON.stringify(lst));
}

function RemoveRole(id) {
    var lst = $.parseJSON($('#hdnRoles').val());
    lst = $.grep(lst, function (n) {
        return n != id;
    });
    $('#hdnRoles').val(JSON.stringify(lst));
}