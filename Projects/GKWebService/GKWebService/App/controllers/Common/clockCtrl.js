(function () {

    angular.module('gkApp.controllers').controller('clockCtrl',
        ['$scope', '$interval', 'dateFilter',
        function ($scope, $interval, dateFilter) {
            function updateTime() {
                $scope.time = dateFilter(new Date(), 'dd.MM.yyyy HH:mm:ss');
            }

            $interval(updateTime, 1000);
        }]
    );
}());