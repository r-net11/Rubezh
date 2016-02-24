(function () {

	'use strict';

	var app = angular.module('gkApp.controllers').controller('restartDetailsCtrl',
        ['$scope', '$http', '$timeout', '$uibModalInstance',
        function ($scope, $http, $timeout, $uibModalInstance) {

        	$scope.time = 0;
        	$scope.max = 60;

        	$scope.ok = function () {
        		location.reload();
        		$uibModalInstance.close();
        	};

        	$scope.cancel = function () {
        		$uibModalInstance.dismiss('cancel');
        	};

        	$scope.tick = function () {
        		if ($scope.time <= $scope.max) {
        			$scope.time += 1;
        			$timeout($scope.tick, 1000);
        		}
        		else {
        			$scope.ok();
        		}
        	}

        	$timeout($scope.tick, 1000);
        }]
    );
}());
