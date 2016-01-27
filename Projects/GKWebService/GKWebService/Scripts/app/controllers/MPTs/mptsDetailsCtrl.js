(function () {
    var app = angular.module('canvasApp.controllers').controller('mptsDetailsCtrl', [
    function ($scope, $uibModalInstance, $http, mpt) {
        $scope.mpt = mpt;

    }])
});