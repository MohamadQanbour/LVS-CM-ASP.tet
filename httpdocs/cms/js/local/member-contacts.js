$(document).ready(function () {
    var families = $.parseJSON($('#hdnFamilies').val());
    var students = $.parseJSON($('#hdnStudents').val());
    $(families).each(function () {
        var item = this;
        var roleId = item.RoleId;
        $('#chkFamilyContact' + roleId).iCheck("check");
        $('#chkFamilyClass' + roleId).iCheck((item.IsClassDependent ? "check" : "uncheck"));
    });
    $(students).each(function () {
        var item = this;
        var roleId = item.RoleId;
        $('#chkStudentContact' + roleId).iCheck("check");
        $('#chkStudentClass' + roleId).iCheck((item.IsClassDependent ? "check" : "uncheck"));
    });
    $('[data-type="family-contact"]').on("ifChecked", function () {
        var roleId = $(this).data("roleid");
        var classDependent = $('[data-type="family-class"][data-roleid=' + roleId + ']').is(":checked");
        AddContact(2, roleId, classDependent);
    }).on("ifUnchecked", function () {
        var roleId = $(this).data("roleid");
        $('[data-type="family-class"][data-roleid=' + roleId + ']').iCheck("uncheck");
        RemoveContact(2, roleId);
    });;
    $('[data-type="family-class"]').on("ifChecked", function () {
        var roleId = $(this).data("roleid");
        AddContact(2, roleId, $(this).is(":checked"));
    }).on("ifUnchecked", function () {
        var roleId = $(this).data("roleid");
        AddContact(2, roleId, $(this).is(":checked"));
    });
    $('[data-type="student-contact"]').on("ifChecked", function () {
        var roleId = $(this).data("roleid");
        var classDependent = $('[data-type="student-class"][data-roleid=' + roleId + ']').is(":checked");
        AddContact(1, roleId, classDependent);
    }).on("ifUnchecked", function () {
        var roleId = $(this).data("roleid");
        $('[data-type="student-class"][data-roleid=' + roleId + ']').iCheck("uncheck");
        RemoveContact(1, roleId);
    });
    $('[data-type="student-class"]').on("ifChecked", function () {
        var roleId = $(this).data("roleid");
        AddContact(1, roleId, $(this).is(":checked"));
    }).on("ifUnchecked", function () {
        var roleId = $(this).data("roleid");
        AddContact(1, roleId, $(this).is(":checked"));
    });
});

function AddContact(type, roleId, classDependent) {
    var lst = $.parseJSON($('#hdn' + (type == 2 ? "Families" : "Students")).val());
    var found = false;
    $(lst).each(function () {
        var item = this;
        if (item.RoleId == roleId) {
            item.IsClassDependent = classDependent;
            found = true;
        }
    });
    if (!found) {
        var item = {
            "MembershipType": type,
            "RoleId": roleId,
            "IsClassDependent": classDependent
        };
        lst.push(item);
    }
    $('#hdn' + (type == 2 ? "Families" : "Students")).val(JSON.stringify(lst));
}

function RemoveContact(type, roleId) {
    var lst = $.parseJSON($('#hdn' + (type == 2 ? "Families" : "Students")).val());
    lst = $.grep(lst, function (n) {
        return n.RoleId != roleId;
    });
    $('#hdn' + (type == 2 ? "Families" : "Students")).val(JSON.stringify(lst));
}