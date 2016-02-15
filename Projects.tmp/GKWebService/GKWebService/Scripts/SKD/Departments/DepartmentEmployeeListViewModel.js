function InitGridDepartmentEmployeeList() {
    function imageFormat(cellvalue, options, rowObject) {
        var newCellValue = cellvalue;
        if (rowObject.IsDeleted) {
            newCellValue = '<span style="opacity:0.5">' + newCellValue + '</span>';
        };
        if (rowObject.IsChief) {
            newCellValue = '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Chief.png" />' + newCellValue;
        }
        return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Employee.png" />' + newCellValue;
    };

    $("#jqGridDepartmentEmployeeList").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'ParentUID', name: 'ParentUID', hidden: true, sortable: false },
            { label: 'OrganisationUID', name: 'OrganisationUID', hidden: true, sortable: false },
            { label: 'ФИО', name: 'FIO', width: 100, sortable: false, formatter: imageFormat },
            { label: 'Name', name: 'Name', width: 100, hidden: true, sortable: false },
            { label: 'Подразделение', name: 'DepartmentName', hidden: true, width: 100, sortable: false },
            { label: 'LastName', name: 'LastName', hidden: true, sortable: false },
            { label: 'FirstName', name: 'FirstName', hidden: true, sortable: false },
            { label: 'SecondName', name: 'SecondName', hidden: true, sortable: false },
            { label: 'Phone', name: 'Phone', hidden: true, sortable: false },
            { label: 'Description', name: 'Description', hidden: true, sortable: false },
            { label: 'Должность', name: 'PositionName', hidden: false, sortable: false },
            { label: 'DepartmentName', name: 'DepartmentName', hidden: true, sortable: false },
            { label: 'OrganisationName', name: 'OrganisationName', hidden: true, sortable: false },
            { label: 'RemovalDate', name: 'RemovalDate', hidden: true, sortable: false },
            { label: 'IsChief', name: 'IsChief', hidden: true, sortable: false },
            { label: 'IsDeleted', name: 'IsDeleted', hidden: true, sortable: false }
        ],
        width: jQuery(window).width() - 242,
        height: 250,
        rowNum: 100,
        viewrecords: true
    });
};

function DepartmentEmployeeListViewModel(parentViewModel) {
    var self = {};

    InitGridDepartmentEmployeeList();

    self.ParentViewModel = parentViewModel;

    self.UID = ko.observable();
    self.OrganisationUID = ko.observable();
    self.DepartmentName = ko.observable();
    self.LastName = ko.observable();
    self.FirstName = ko.observable();
    self.SecondName = ko.observable();
    self.Name = ko.observable();
    self.Phone = ko.observable();
    self.Description = ko.observable();
    self.PositionName = ko.observable();
    self.OrganisationName = ko.observable();
    self.RemovalDate = ko.observable();
    self.IsDeleted = ko.observable();
    self.IsRowSelected = ko.observable(false);
    self.IsChief = ko.observable(false);
    self.CanAdd = ko.computed(function () {
        return !self.ParentViewModel.IsDeleted() && app.Menu.HR.IsEmployeesEditAllowed();
    }, self);
    self.CanRemove = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.ParentViewModel.IsDeleted() && app.Menu.HR.IsEmployeesEditAllowed();
    }, self);
    self.CanEdit = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.ParentViewModel.IsDeleted() && app.Menu.HR.IsEmployeesEditAllowed();
    }, self);
    self.CanSetChief = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && !self.IsChief() && !self.ParentViewModel.IsDeleted() && app.Menu.HR.IsDepartmentsEditAllowed();
    }, self);
    self.CanUnSetChief = ko.computed(function () {
        return self.IsRowSelected() && !self.IsDeleted() && self.IsChief() && !self.ParentViewModel.IsDeleted() && app.Menu.HR.IsDepartmentsEditAllowed();
    }, self);

    self.Init = function () {
        self.ReloadTree();
    };

    self.ReloadTree = function () {
        self.IsRowSelected(false);

        $.getJSON("/Departments/GetDepartmentEmployeeList/",
            {
                departmentId: self.ParentViewModel.UID(),
                organisationId: self.ParentViewModel.OrganisationUID(),
                isWithDeleted: self.ParentViewModel.Filter.IsWithDeleted(),
                chiefId: self.ParentViewModel.ChiefUID()
            },
            function (empList) {
                self.UpdateTree(empList);
            })
            .fail(function (jqxhr, textStatus, error) {
               ShowError(jqxhr.responseText);
            });
    };

    self.UpdateTree = function (data) {
        $("#jqGridDepartmentEmployeeList").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring",
        });
        $("#jqGridDepartmentEmployeeList").trigger("reloadGrid");
        $("#jqGridDepartmentEmployeeList").jqGrid("resetSelection");
    };

    $('#jqGridDepartmentEmployeeList').on('jqGridSelectRow', function (event, id, selected) {

        if (selected) {
            var myGrid = $('#jqGridDepartmentEmployeeList');

            self.UID(id);
            self.OrganisationUID(myGrid.jqGrid('getCell', id, 'OrganisationUID'));
            self.DepartmentName(myGrid.jqGrid('getCell', id, 'DepartmentName'));
            self.LastName(myGrid.jqGrid('getCell', id, 'LastName'));
            self.FirstName(myGrid.jqGrid('getCell', id, 'FirstName'));
            self.SecondName(myGrid.jqGrid('getCell', id, 'SecondName'));
            self.Name(myGrid.jqGrid('getCell', id, 'Name'));
            self.Phone(myGrid.jqGrid('getCell', id, 'Phone'));
            self.Description(myGrid.jqGrid('getCell', id, 'Description'));
            self.PositionName(myGrid.jqGrid('getCell', id, 'PositionName'));
            self.OrganisationName(myGrid.jqGrid('getCell', id, 'OrganisationName'));
            self.RemovalDate(myGrid.jqGrid('getCell', id, 'RemovalDate'));
            self.IsChief(myGrid.jqGrid('getCell', id, 'IsChief') === "true");
            self.IsDeleted(myGrid.jqGrid('getCell', id, 'IsDeleted') === "true");

            self.IsRowSelected(true);
        }
    });

    self.Add = function (data, e) {
        $.getJSON("/Hr/GetEmptyDepartmentEmployees/" + self.ParentViewModel.OrganisationUID(), function (emp) {
            ko.mapping.fromJS(emp, {}, app.Menu.HR.Common.EmployeeSelectionDialog);
            app.Menu.HR.Common.EmployeeSelectionDialog.Init(function (employee) {
                $.ajax({
                    url: "Departments/SaveEmployeeDepartment",
                    type: "post",
                    contentType: "application/json",
                    data: "{ 'employeeUID': '" + employee.UID() + "', 'departmentUID': '" + self.ParentViewModel.UID() + "', 'name': '" + employee.Name() + "' }",
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
            url: "Departments/SaveEmployeeDepartment",
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

    self.SetChief = function (data, e) {
        $.ajax({
            url: "Departments/SaveDepartmentChief",
            type: "post",
            contentType: "application/json",
            data: "{ 'departmentUID': '" + self.ParentViewModel.UID() + "','employeeUID': '" + self.UID() + "', 'name': '" + self.ParentViewModel.NameData() + "' }",
            success: function (error) {
                self.ParentViewModel.SetChief(self.UID());
                self.ReloadTree();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });
    };

    self.UnSetChief = function (data, e) {
        $.ajax({
            url: "Departments/SaveDepartmentChief",
            type: "post",
            contentType: "application/json",
            data: "{ 'departmentUID': '" + self.ParentViewModel.UID() + "', 'name': '" + self.ParentViewModel.Name() + "' }",
            success: function (error) {
                self.ParentViewModel.SetChief("00000000-0000-0000-0000-000000000000");
                self.ReloadTree();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });
    };

    return self;
}
