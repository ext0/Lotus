var lotus = angular.module("Lotus", ["ngCookies", "ngWebSocket"]);
lotus.controller("BootstrapperController", ["$scope", "$rootScope", "$http", "$window", function ($scope, $rootScope, $http, $window) {
    $rootScope.authenticated = authenticated;
    $rootScope.email = email;
    $scope.logout = function () {
        $http.post("/Logout/").then(function (data) {
            $window.location = "/";
        }, function (error) {
            console.log(error);
        });
    };
}]);