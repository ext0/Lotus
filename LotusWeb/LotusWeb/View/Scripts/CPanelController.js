lotus.controller("CPanelController", ["$scope", "$http", "$window", "$cookies", "$websocket", function ($scope, $http, $window, $cookies, $websocket) {
    if (!$scope.authenticated) {
        console.log("Not authenticated!");
    }
    $scope.sidebar = [
        {
            title: "overview",
            caption: "",
            icon: "mif-apps"
        },
        {
            title: "all clients",
            caption: "0 clients",
            icon: "mif-stack"
        },
        {
            title: "live clients",
            caption: "0 online",
            icon: "mif-layers"
        },
        {
            title: "offline clients",
            caption: "0 offline",
            icon: "mif-layers-clear"
        },
        {
            title: "plugins",
            caption: "0 plugins available",
            icon: "mif-versions"
        },
        {
            title: "world map",
            caption: "",
            icon: "mif-earth"
        },
        {
            title: "configuration",
            caption: "",
            icon: "mif-cogs"
        }
    ];
    var dataStream = $websocket("ws://localhost:8888/COP");
    dataStream.onMessage(function (message) {
        var data = message.data;
        var response = angular.fromJson(data);
        var content = angular.fromJson(response.Data);
        console.log(content);
    });

    $scope.buildRequest = function (command) {
        var parameters = [];
        for (var i = 1; i < arguments.length; i++) {
            parameters.push(arguments[i]);
        }
        return angular.toJson({
            Auth: $cookies.get("LOTUS_SESSION_ID"),
            Command: command,
            Parameters: parameters
        });
    };

    $scope.getClientList = function () {
        dataStream.send($scope.buildRequest("GETCTHUMBS"));
    };

    $scope.getClientList();

    $scope.activeTab = $scope.sidebar[0];
    $scope.sidebarClick = function (tab) {
        $scope.activeTab = tab;
    };
}]);