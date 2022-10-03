$(document).ready(function () {
    $('#ddlStudent').change(function () {
        $('#hdnSelectedStudent').val($(this).val());
    });
});