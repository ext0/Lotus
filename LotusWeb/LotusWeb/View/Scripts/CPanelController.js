lotus.controller("CPanelController", ["$scope", "$http", "$window", "$cookies", "$websocket", "uuid", function ($scope, $http, $window, $cookies, $websocket, uuid) {
    if (!$scope.authenticated) {
        console.log("WARN: Not authenticated!");
        $window.location.href = "/Login";
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

    $scope.Cthumbs = [];
    var requests = {};

    dataStream.onMessage(function (message) {
        var data = message.data;
        var response = angular.fromJson(data);
        for (var request in requests) {
            if (request === response.ID) {
                requests[request](angular.fromJson(response.Data));
                delete requests[request];
                return;
            }
        }
        console.log("WARN: unknown response id: " + response.ID);
    });

    $scope.buildRequest = function (command) {
        var parameters = [];
        for (var i = 1; i < arguments.length; i++) {
            parameters.push(arguments[i]);
        }
        var request = {
            ID: uuid.v4(),
            Auth: $cookies.get("LOTUS_SESSION_ID"),
            Command: command,
            Parameters: parameters
        };
        return request;
    };

    $scope.sendRequest = function (request, callback) {
        requests[request.ID] = callback;
        dataStream.send(angular.toJson(request));
    }

    $scope.getClientList = function () {
        var request = $scope.buildRequest("GETCTHUMBS");
        $scope.sendRequest(request, function (result) {
            $scope.Cthumbs = result;
            for (var thumb of result) {
                var built = $scope.buildRequest("CGETDRIVES", thumb.CIdentifier);
                $scope.sendRequest(built, function (result) {
                    console.log(result);
                });
            }
        });
    };

    $scope.getClientList();

    $scope.activeTab = $scope.sidebar[0];
    $scope.sidebarClick = function (tab) {
        $scope.activeTab = tab;
    };
}]);