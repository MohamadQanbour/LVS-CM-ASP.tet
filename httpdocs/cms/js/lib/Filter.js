$(document).ready(function () {
    $('.egv-filter [data-expression]').hide();
    var ddlExpression = $('#ddlFilterExpression');
    ReloadOperations(ddlExpression.val());
    ddlExpression.on("change", function () {
        ReloadOperations($(this).val());
    });
    ReloadAppliedFilters();
    $('[role=toggle-filter]').click(function () {
        ToggleFilter();
    });
    $('#btnFilterAdd').click(function () {
        AddAppliedFilter();
    });
});

function AddAppliedFilter() {
    var expression = $('#ddlFilterExpression').val();
    var operation = $('#ddlFilterOperation').val();
    var value = $('.egv-filter [data-expression="' + expression + '"]').val();
    var datatype = GetDataType(expression);
    if (value != "") {
        var lst = $.parseJSON($('#hdnFilterApplied').val());
        var obj = {
            "Expression": expression,
            "Operation": operation,
            "Value": value,
            "DataType": datatype
        };
        var found = false;
        $(lst).each(function () {
            if (this.Expression == expression) {
                found = true;
                this.Operation = obj.Operation;
                this.Value = obj.Value;
            }
        });
        if (!found) lst.push(obj);
        $('#hdnFilterApplied').val(JSON.stringify(lst));
        if (datatype != 3 && datatype != 5) $('.egv-filter [data-expression="' + expression + '"]').val("");
        ReloadAppliedFilters();
    }
}

function RemoveAppliedFilter(expression) {
    var lst = $.parseJSON($('#hdnFilterApplied').val());
    var newLst = $.grep(lst, function (n) {
        return !(n.Expression == expression);
    });
    $('#hdnFilterApplied').val(JSON.stringify(newLst));
    ReloadAppliedFilters();
}

function ReloadOperations(expression) {
    var lst = $.parseJSON($('#hdnFilterDef').val());
    $('#ddlFilterOperation').empty();
    $(lst).each(function () {
        if (this.Expression == expression) {
            $('.egv-filter [data-expression]').hide();
            $('.egv-filter [data-expression="' + this.Expression + '"]').show();
            $(this.AllowedTypes).each(function () {
                var item = this;
                $('#ddlFilterOperation')
                    .append($('<option>')
                        .attr('value', item.Value)
                        .text(item.Text)
                    );
            });
        }
    });
}

function ReloadAppliedFilters() {
    var lst = $.parseJSON($('#hdnFilterApplied').val());
    var container = $('#pnlAppliedFilters');
    container.empty();
    if (lst.length > 0) {
        $('.egv-filter').slideDown();
        $(lst).each(function () {
            var a = $("<a>")
                .attr("href", "javascript:;")
                .addClass("btn bg-olive btn-xs")
                .attr("data-appliedexpression", this.Expression)
                .html("<b>" + GetFilterDisplayTitle(this.Expression) + "</b> " + GetOperationDisplayTitle(this.Operation, this.Expression) + " " + GetValueDisplay(this.Value, this.Expression))
                .append($("<i>").addClass("fa fa-times"))
                .click(function () {
                    RemoveAppliedFilter($(this).data("appliedexpression"));
                });
            container.append(a);
        });
    }
}

function GetFilterDisplayTitle(expression) {
    return $('#ddlFilterExpression option[value="' + expression + '"]').text();
}

function GetOperationDisplayTitle(operation, expression) {
    var lst = $.parseJSON($('#hdnFilterDef').val());
    var ret = "";
    $(lst).each(function () {
        if (this.Expression == expression)
            $(this.AllowedTypes).each(function () {
                if (this.Value == operation) ret = this.Text;
            });
    });
    return ret;
}

function GetValueDisplay(value, expression) {
    var target = $('.egv-filter [data-expression="' + expression + '"]');
    if (target.is("select")) return target.find('option[value="' + value + '"]').text(); else return value;
}

function GetDataType(expression) {
    var lst = $.parseJSON($('#hdnFilterDef').val());
    var ret = 0;
    $(lst).each(function(){
        if (this.Expression == expression) ret = this.DataType;
    });
    return ret;
}

function ToggleFilter() {
    $('.egv-filter').slideToggle();
}