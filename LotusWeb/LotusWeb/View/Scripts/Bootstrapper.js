var lotus = angular.module("Lotus", ["ngCookies", "ngWebSocket", "angular-uuid"]);

lotus.config(
    function ($controllerProvider, $provide, $compileProvider) {
        // Since the "shorthand" methods for component
        // definitions are no longer valid, we can just
        // override them to use the providers for post-
        // bootstrap loading.

        // Let's keep the older references.
        lotus._controller = lotus.controller;
        lotus._service = lotus.service;
        lotus._factory = lotus.factory;
        lotus._value = lotus.value;
        lotus._directive = lotus.directive;
        // Provider-based controller.
        lotus.controller = function (name, constructor) {
            $controllerProvider.register(name, constructor);
            return (this);
        };
        // Provider-based service.
        lotus.service = function (name, constructor) {
            $provide.service(name, constructor);
            return (this);
        };
        // Provider-based factory.
        lotus.factory = function (name, factory) {
            $provide.factory(name, factory);
            return (this);
        };
        // Provider-based value.
        lotus.value = function (name, value) {
            $provide.value(name, value);
            return (this);
        };
        // Provider-based directive.
        lotus.directive = function (name, factory) {
            $compileProvider.directive(name, factory);
            return (this);
        };
        // NOTE: You can do the same thing with the "filter"
        // and the "$filterProvider"; but, I don't really use
        // custom filters.
    }
);

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