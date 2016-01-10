function InitGridPositionEmployeeList() {
    function imageFormat(cellvalue, options, rowObject) {
        var newCellValue = cellvalue;
        if (rowObject.IsDeleted) {
            newCellValue = '<span style="opacity:0.5">' + newCellValue + '</span>';
        };
        return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Employee.png" />' + newCellValue;
    };

    $("#jqGridPositionEmployeeList").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'ParentUID', name: 'ParentUID', hidden: true, sortable: false },
            { label: 'OrganisationUID', name: 'OrganisationUID', hidden: true, sortable: false },
            { label: 'ФИО', name: 'FIO', width: 100, sortable: false, formatter: imageFormat },
            { label: 'Name', name: 'Name', width: 100, hidden: true, sortable: false },
            { label: 'Подразделение', name: 'DepartmentName', hidden: false, width: 100, sortable: false },
            { label: 'LastName', name: 'LastName', hidden: true, sortable: false },
            { label: 'FirstName', name: 'FirstName', hidden: true, sortable: false },
            { label: 'SecondName', name: 'SecondName', hidden: true, sortable: false },
            { label: 'RemovalDate', name: 'RemovalDate', hidden: true, sortable: false },
            { label: 'IsDeleted', name: 'IsDeleted', hidden: true, sortable: false }
        ],
        width: jQuery(window).width() - 242,
        height: 250,
        rowNum: 100,
        viewrecords: true
    });
};

function PositionEmployeeListViewModel(parentViewModel) {
    var self = {};

    InitGridPositionEmployeeList();

    self.ParentViewModel = parentViewModel;

    self.UID = ko.observable();
    self.OrganisationUID = ko.observable();
    self.DepartmentName = ko.observable();
    self.LastName = ko.observable();
    self.FirstName = ko.observable();
    self.SecondName = ko.observable();
    self.Name = ko.observable();
    self.RemovalDate = ko.observable();
    self.IsDeleted = ko.observable();
    self.IsRowSelected = ko.observable(false);
    self.CanAdd = ko.computed(function () {
        return !self.ParentViewModel.IsDeleted();
    }, self);
    self.CanRemove = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.ParentViewModel.IsDeleted();
    }, self);
    self.CanEdit = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.ParentViewModel.IsDeleted();
    }, self);

    self.Init = function () {
        self.ReloadTree();
    };

    self.ReloadTree = function () {
        self.IsRowSelected(false);

        $.getJSON("/Positions/GetPositionEmployeeList/",
            {
                positionId: self.ParentViewModel.UID(),
                organisationId: self.ParentViewModel.OrganisationUID(),
                isWithDeleted: self.ParentViewModel.Filter.IsWithDeleted()
            },
            function (empList) {
                self.UpdateTree(empList);
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.UpdateTree = function (data) {
        $("#jqGridPositionEmployeeList").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring",
        });
        $("#jqGridPositionEmployeeList").trigger("reloadGrid");
        $("#jqGridPositionEmployeeList").jqGrid("resetSelection");
    };

    $('#jqGridPositionEmployeeList').on('jqGridSelectRow', function (event, id, selected) {

        if (selected) {
            var myGrid = $('#jqGridPositionEmployeeList');

            self.UID(id);
            self.OrganisationUID(myGrid.jqGrid('getCell', id, 'OrganisationUID'));
            self.DepartmentName(myGrid.jqGrid('getCell', id, 'DepartmentName'));
            self.LastName(myGrid.jqGrid('getCell', id, 'LastName'));
            self.FirstName(myGrid.jqGrid('getCell', id, 'FirstName'));
            self.SecondName(myGrid.jqGrid('getCell', id, 'SecondName'));
            self.Name(myGrid.jqGrid('getCell', id, 'Name'));
            self.RemovalDate(myGrid.jqGrid('getCell', id, 'RemovalDate'));
            self.IsDeleted(myGrid.jqGrid('getCell', id, 'IsDeleted') === "true");

            self.IsRowSelected(true);
        }
    });

    self.Add = function (data, e) {
        $.getJSON("/Hr/GetEmptyPositionEmployees/" + self.ParentViewModel.OrganisationUID(), function (emp) {
            ko.mapping.fromJS(emp, {}, app.Menu.HR.Common.EmployeeSelectionDialog);
            app.Menu.HR.Common.EmployeeSelectionDialog.Init(function (employee) {
                $.ajax({
                    url: "Positions/SaveEmployeePosition",
                    type: "post",
                    contentType: "application/json",
                    data: "{ 'employeeUID': '" + employee.UID() + "', 'positionUID': '" + self.ParentViewModel.UID() + "', 'name': '" + employee.Name() + "' }",
                    success: function (error) {
                        self.ReloadTree();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        ShowError(xhr.responseText);
                    }
                });
            }, true);
            ShowBox('#employee-selection-box');
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.Remove = function (data, e) {
        $.ajax({
            url: "Positions/SaveEmployeePosition",
            type: "post",
            contentType: "application/json",
            data: "{ 'employeeUID': '" + self.UID() + "', 'name': '" + self.Name() + "' }",
            success: function (error) {
                self.ReloadTree();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });
    };

    self.Edit = function (data, e) {
        var employeeDetails = app.Menu.HR.Employees.EmployeeDetails;
        $.getJSON("/Employees/GetEmployeeDetails/" + self.UID(), function (emp) {
            ko.mapping.fromJS(emp, {}, employeeDetails);
            $.getJSON("/Employees/GetOrganisation/" + self.OrganisationUID(), function (org) {
                employeeDetails.Organisation = org;
                employeeDetails.Init(false, "Employee", self.ReloadTree);
                ShowBox("#employee-details-box");
            })
            .fail(function (jqxhr, textStatus, error) {
                ShowError(jqxhr.responseText);
            });
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    return self;
}
