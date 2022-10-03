$(document).ready(function () {
    $('[data-id] select').change(function () {
        var roleId = $(this).val();
        var id = $(this).parent().data("id");
        UpdateRole(id, roleId);
    });
});

function UpdateRole(id, roleid) {
    var lst = $.parseJSON($('#hdnValues').val());
    $(lst).each(function () {
        var item = this;
        if (item.UserType == id) item.RoleId = roleid;
    });
    $('#hdnValues').val(JSON.stringify(lst));
}