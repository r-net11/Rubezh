(function () {
    angular.module('gkApp.controllers').controller('journalFilterCtrl',
        function ($scope, $http, $uibModal, uiGridConstants) {
        	$scope.testStr = 'testStrtestStrtestStr';

        	$http.get("Journal/GetJournal").success(function (data) {
        		$scope.gridOptions.data = data;
        	});

        	$scope.gridOptions = {
        		enableFiltering: true,
        		onRegisterApi: function (gridApi) {
        			$scope.gridApi = gridApi;
        		},
        		columnDefs: [
					{ name: 'Дата в системе', field: 'SystemDate' },
					{ name: 'Дата в приборе', field: 'DeviceDate', cellFilter: 'date' },
					{
						name: 'Название', field: 'Name', filter: {
							type: uiGridConstants.filter.SELECT,
							selectOptions: [
								{ value: 'Отсутствует лицензия', label: 'Отсутствует лицензия - label' },
								{ value: 'Применение конфигурации', label: 'Применение конфигурации - label' },
								{ value: 'Команда оператора', label: 'Команда оператора - label' },
							],
						}
					},
					{ name: 'Уточнение', field: 'Desc' },
					{ name: 'Объект', field: 'Object', filter: { term: 'Нет' } },
        		],
        	};
        });
}());