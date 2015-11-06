$(document).ready(function() {
    function imageFormat(cellvalue, options, rowObject) {
        if (rowObject.IsOrganisation) {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Organisation.png" />' + cellvalue;
        } else {
            return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Employee.png" />' + cellvalue;
        }
    };

    $("#jqGridEmployees").jqGrid({
        datastr: null,
        datatype: "jsonstring",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'ParentUID', name: 'ParentUID', hidden: true, sortable: false },
            { label: 'OrganisationUID', name: 'OrganisationUID', hidden: true, sortable: false },
            { label: 'ФИО', name: 'Name', width: 100, sortable: false, formatter: imageFormat },
            { label: 'Подразделение', name: 'DepartmentName', width: 100, sortable: false },
            { label: 'IsOrganisation', name: 'IsOrganisation', hidden: true, sortable: false },
            { label: 'LastName', name: 'LastName', hidden: true, sortable: false },
            { label: 'FirstName', name: 'FirstName', hidden: true, sortable: false },
            { label: 'SecondName', name: 'SecondName', hidden: true, sortable: false },
            { label: 'Phone', name: 'Phone', hidden: true, sortable: false },
            { label: 'Description', name: 'Description', hidden: true, sortable: false },
            { label: 'PositionName', name: 'PositionName', hidden: true, sortable: false },
            { label: 'DepartmentName', name: 'DepartmentName', hidden: true, sortable: false },
            { label: 'OrganisationName', name: 'OrganisationName', hidden: true, sortable: false },
            { label: 'RemovalDate', name: 'RemovalDate', hidden: true, sortable: false },
            { label: 'IsDeleted', name: 'IsDeleted', hidden: true, sortable: false }
        ],
        width: jQuery(window).width() - 242,
        height: 300,
        rowNum: 100,
        viewrecords: true,

        treeGrid: true,
        ExpandColumn: "Name",
        treedatatype: "local",
        treeGridModel: "adjacency",
        loadonce: false,
        treeReader: {
            parent_id_field: "ParentUID",
            level_field: "Level",
            leaf_field: "IsLeaf",
            expanded_field: "IsExpanded"
        }
    });
});

function EmployeesViewModel(parentViewModel) {
    var self = {};

    self.ParentViewModel = parentViewModel;

    self.UID = ko.observable();
    self.OrganisationUID = ko.observable();
    self.DepartmentName = ko.observable();
    self.IsOrganisation = ko.observable(true);
    self.IsGuest = ko.observable();
    self.LastName = ko.observable();
    self.FirstName = ko.observable();
    self.SecondName = ko.observable();
    self.Phone = ko.observable();
    self.Description = ko.observable();
    self.PositionName = ko.observable();
    self.OrganisationName = ko.observable();
    self.RemovalDate = ko.observable();
    self.IsDeleted = ko.observable();
    self.IsRowSelected = ko.observable(false);
    self.ItemRemovingName = ko.computed(function() {
        return self.IsGuest() ? "посетителя" : "сотрудника";
    }, self);
    self.AddCommandToolTip = ko.computed(function() {
        return "Добавить " + self.ItemRemovingName();
    }, self);
    self.RemoveCommandToolTip = ko.computed(function () {
        return "Удалить " + self.ItemRemovingName();
    }, self);
    self.EditCommandToolTip = ko.computed(function () {
        return "Редактировать " + self.ItemRemovingName();
    }, self);
    self.CanAdd = ko.computed(function() {
        return self.IsRowSelected();
    }, self);
    self.CanRemove = ko.computed(function () {
        return self.IsRowSelected() && !self.IsOrganisation();
    }, self);
    self.CanEdit = ko.computed(function() {
        return self.IsRowSelected() && !self.IsOrganisation();
    }, self);

    self.Init = function () {
        var filter = { PersonType: parentViewModel.SelectedPersonType() };
        self.IsGuest(parentViewModel.SelectedPersonType() === "Guest");
        self.IsOrganisation(true);
        self.IsRowSelected(false);

        $.ajax({
            url: "/Employees/GetOrganisations",
            type: "post",
            contentType: "application/json",
            data: ko.toJSON(filter),
            success: function (data) {
                self.UpdateTree(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("request failed");
            },
        });

    };

    self.UpdateTree = function (data) {
        $("#jqGridEmployees").setGridParam({
            datastr: data,
            datatype: "jsonstring",
            treedatatype: "jsonstring",
        });
        $("#jqGridEmployees").trigger("reloadGrid");
        $("#jqGridEmployees").jqGrid("resetSelection");
        $("#jqGridEmployees").setCell(0, 'Name', "<i>asd</i>");
    };

    $('#jqGridEmployees').on('jqGridSelectRow', function (event, id, selected) {

        if (selected) {
            var myGrid = $('#jqGridEmployees');

            self.UID(id);
            self.OrganisationUID(myGrid.jqGrid('getCell', id, 'OrganisationUID'));
            self.DepartmentName(myGrid.jqGrid('getCell', id, 'DepartmentName'));
            self.IsOrganisation(myGrid.jqGrid('getCell', id, 'IsOrganisation') == "true");
            self.LastName(myGrid.jqGrid('getCell', id, 'LastName'));
            self.FirstName(myGrid.jqGrid('getCell', id, 'FirstName'));
            self.SecondName(myGrid.jqGrid('getCell', id, 'SecondName'));
            self.Phone(myGrid.jqGrid('getCell', id, 'Phone'));
            self.Description(myGrid.jqGrid('getCell', id, 'Description'));
            self.PositionName(myGrid.jqGrid('getCell', id, 'PositionName'));
            self.OrganisationName(myGrid.jqGrid('getCell', id, 'OrganisationName'));
            self.RemovalDate(myGrid.jqGrid('getCell', id, 'RemovalDate'));
            self.IsDeleted(myGrid.jqGrid('getCell', id, 'IsDeleted') == "true");

            if (!self.IsOrganisation()) {
                self.EmployeeCards.InitCards(self.UID());
            } else {
                self.EmployeeCards.InitCards(null);
            }

            self.IsRowSelected(true);
        }
    });

    self.ShowEmployee = function(box) {
        //Fade in the Popup
        $(box).fadeIn(300, function() {
            $(this).trigger("fadeInComplete");
        });

        //Set the center alignment padding + border see css style
        var popMargTop = ($(box).height() + 24) / 2;
        var popMargLeft = ($(box).width() + 24) / 2;

        $(box).css({
            'margin-top': -popMargTop,
            'margin-left': -popMargLeft
        });

        // Add the mask to body
        $('body').append('<div id="mask"></div>');
        $('#mask').fadeIn(300);

        return false;
    };

    self.AddEmployeeClick = function(data, e, box) {
        $.getJSON("/Employees/GetEmployeeDetails/", function(emp) {
            ko.mapping.fromJS(emp, {}, self.EmployeeDetails);
            $.getJSON("/Employees/GetOrganisation/" + self.OrganisationUID(), function(org) {
                self.EmployeeDetails.Organisation = org;
                self.EmployeeDetails.Init(true, self.ParentViewModel.SelectedPersonType(), self);
                self.ShowEmployee(box);
            });
        });
    };

    self.EditEmployeeClick = function(data, e, box) {
        $.getJSON("/Employees/GetEmployeeDetails/" + self.UID(), function(emp) {
            ko.mapping.fromJS(emp, {}, self.EmployeeDetails);
            $.getJSON("/Employees/GetOrganisation/" + self.OrganisationUID(), function(org) {
                self.EmployeeDetails.Organisation = org;
                self.EmployeeDetails.Init(false, self.ParentViewModel.SelectedPersonType(), self);
                self.ShowEmployee(box);
            });
        });
    };

    return self;
}
