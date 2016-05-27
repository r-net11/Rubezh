(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('HRCtrl', 
        ['$scope', '$uibModal', '$timeout', '$filter', '$q', '$window', 'authData', 'employeesService', 'dateFilter', 'dialogService',
        function ($scope, $uibModal, $timeout, $filter, $q, $window, authData, employeesService, dateFilter, dialogService) {
            $scope.authData = authData;

            $scope.canEmployeesView = function () {
                return authData.checkPermission('Oper_SKD_Employees_View');
            };

            $scope.canGuestsView = function () {
                return authData.checkPermission('Oper_SKD_Guests_View');
            };

            $scope.employeesHeader = function() {
                return ($scope.selectedPersonType === "Employee") ? "Сотрудники" : "Посетители";
            };

            $scope.canSelectEmployees = function() {
                return authData.checkPermission('Oper_SKD_Employees_View') || authData.checkPermission('Oper_SKD_Guests_View');
            };
            $scope.canSelectPositions = function () {
                return authData.checkPermission('Oper_SKD_Positions_View');
            };
            $scope.canSelectDepartments = function () {
                return authData.checkPermission('Oper_SKD_Departments_View');
            };
            $scope.canSelectCards = function () {
                return authData.checkPermission('Oper_SKD_Cards_View');
            };
            $scope.canSelectAccessTemplates = function () {
                return authData.checkPermission('Oper_SKD_AccessTemplates_View');
            };
            $scope.canSelectOrganisations = function () {
                return authData.checkPermission('Oper_SKD_Organisations_View');
            };

            $scope.$watch('selectedPersonType', function (newValue, oldValue) {
                $scope.filter.UIDs = [];
                $scope.filter.PositionUIDs = [];
                $scope.initializeEmployeeFilter();
            });

            $scope.initializeEmployeeFilter = function () {
                $scope.filter.PersonType = $scope.selectedPersonType;
            };

            if ($scope.canEmployeesView()) {
                $scope.selectedPersonType = "Employee";
            } else if ($scope.canGuestsView()) {
                $scope.selectedPersonType = "Guest";
            }

            $scope.isWithDeleted = function() {
                return $scope.filter.LogicalDeletationType === "All";
            };

            $scope.filter = {
                LogicalDeletationType: "Active",
                OrganisationUIDs: [],
                DepartmentUIDs: [],
                PositionUIDs: [],
                UIDs: [],
                LastName: '',
                FirstName: '',
                SecondName: ''
            };

            $scope.editFilter = function () {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Hr/HrFilter',
                    controller: 'HRFilterCtrl',
                    backdrop: 'static',
                    resolve: {
                        filter: function () {
                            return $scope.filter;
                        },
                        personType: function () {
                            return $scope.selectedPersonType;
                        },
                        showEmployeeFilter: function() {
                            return $scope.activeTab === 0 || $scope.activeTab === 3;
                        }
                    }
                });

                modalInstance.result.then(function (filter) {
                    $scope.filter = filter;
                });
            };

            $scope.updateOrganisation = function(organisations, organisation) {
                for (var i = 0; i < organisations.length; i++) {
                    if (organisations[i].UID === organisation.UID) {
                        organisations[i].Name = organisation.Name;
                        organisations[i].Description = organisation.Description;
                        break;
                    }
                }
            }

            $scope.removeOrganisation = function(organisations, organisation) {
                var orgIndex = -1;
                var orgFound = false;
                // поиск организации
                for (var i = 0; i < organisations.length; i++) {
                    if (organisations[i].UID === organisation.UID) {
                        orgIndex = i;
                        orgFound = true;
                        break;
                    }
                }
                if (orgFound)
                {
                    // поиск дочерних элментов организации
                    var childCount = 0;
                    for (var j = orgIndex + 1; j < organisations.length; j++) {
                        if (organisations[j].OrganisationUID === organisation.UID) {
                            childCount++;
                        } else {
                            break;
                        }
                    }
                    if ($scope.isWithDeleted()) {
                        for (var k = orgIndex; k < orgIndex + childCount + 1; k++) {
                            organisations[k].IsDeleted = true;
                            organisations[k].IsOrganisationDeleted = true;
                            if (!organisations[k].RemovalDate) {
                                organisations[k].RemovalDate = dateFilter(new Date(), 'dd.MM.yyyy');
                            }
                        }
                    } else {
                        organisations.splice(orgIndex, childCount + 1);
                    }
                }
            }

            $scope.addOrganisation = function (gridApi, organisations, organisation) {
                var newOrganisation = {
                    UID: organisation.UID,
                    OrganisationUID: organisation.UID,
                    Name: organisation.Name,
                    $$treeLevel: 0,
                    IsOrganisation: true,
                    Description: organisation.Description
                };
                organisations.push(newOrganisation);
                $timeout(function () {
                    gridApi.treeBase.toggleRowTreeState(gridApi.grid.getRow(newOrganisation));
                });
            }

            $scope.restoreOrganisation = function (gridApi, organisations, newOrganisations, organisation) {
                newOrganisations = $filter('filter')(newOrganisations, { OrganisationUID: organisation.UID });
                if (newOrganisations.length > 0) {
                    // удаление старых элементов
                    var oldOrganisations = $filter('filter')(organisations, { OrganisationUID: organisation.UID });
                    if (oldOrganisations.length) {
                        for (var i = 0; i < organisations.length; i++) {
                            if (organisations[i].OrganisationUID === organisation.UID) {
                                organisations.splice(i, oldOrganisations.length);
                                break;
                            }
                        }
                    }
                    // добавление новых элементов
                    angular.forEach(newOrganisations, function (value) {
                        organisations.push(value);
                        value.$$treeLevel = value.Level;
                    });
                    $timeout(function () {
                        angular.forEach(newOrganisations, function (value) {
                            gridApi.treeBase.toggleRowTreeState(gridApi.grid.getRow(value));
                        });
                    });
                }
            }

            $scope.restoreElement = function (elementName, elements, selectedElement) {
                var deferred = $q.defer();

                if (dialogService.showConfirm("Вы уверены, что хотите восстановить " + elementName + "?")) {
                    var items = elements;
                    var equalItemFound = false;
                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        if (item.Name === selectedElement.Name &&
                            item.UID !== selectedElement.UID &&
                            item.OrganisationUID === selectedElement.OrganisationUID &&
                            !item.IsOrganisation &&
                            !item.IsDeleted) {
                            $window.alert("Существует неудалённый элемент с таким именем");
                            equalItemFound = true;
                            break;
                        } 
                    }
                    if (equalItemFound) {
                        deferred.reject();
                    } else {
                        deferred.resolve();
                    }
                } else {
                    deferred.reject();
                }

                return deferred.promise;
            };
        }]
    );

}());
