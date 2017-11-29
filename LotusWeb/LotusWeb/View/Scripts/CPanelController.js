﻿lotus.controller("CPanelController", ["$scope", "$http", "$window", "$cookies", "$websocket", "uuid", function ($scope, $http, $window, $cookies, $websocket, uuid) {
    if (!$scope.authenticated) {
        console.log("WARN: Not authenticated!");
        $window.location.href = "/Login";
    }

    $scope.Cthumbs = [];
    $scope.selected = {};
    var requests = {};

    /*
        COMMUNICATION SYSTEM (WEBSOCKET)
    */

    var dataStream = $websocket("ws://localhost:8888/COP");

    dataStream.onMessage(function (message) {
        var data = message.data;
        var response = angular.fromJson(data);
        for (var request in requests) {
            if (request === response.ID) {
                var text = null;
                try {
                    text = angular.fromJson(response.Data);
                } catch (e) {
                    text = response.Data;
                }
                requests[request](text);
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

    $scope.sendClientRequest = function (thumb, request, callback) {
        var built = $scope.buildRequest(request, thumb.CIdentifier);
        $scope.sendRequest(built, callback);
    };

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

    /*
        TAB SYSTEM
    */

    $scope.sidebar = [
    {
        title: "overview",
        caption: "",
        icon: "mif-apps"
    },
    {
        title: "clients",
        caption: "0 clients",
        icon: "mif-stack"
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

    $scope.activeTab = $scope.sidebar[0];

    $scope.sidebarClick = function (tab) {
        $scope.activeTab = tab;
    };

    /*
        CLIENT CONTROL
    */

    $scope.clientSelect = function (section, client) {
        $scope.selected[section] = client;
    }

    $scope.clientLogoff = function () {
        var select = $scope.selected['Clients'];
        $scope.sendClientRequest(select, 'CLOGOFF', function (result) {
            $.Notify({
                caption: "Logoff command sent",
                content: "Successful command execution",
                type: 'info'
            });
        });
    };

    $scope.clientShutdown = function () {
        var select = $scope.selected['Clients'];
        $scope.sendClientRequest(select, 'CSHUTDOWN', function (result) {
            $.Notify({
                caption: "Shutdown command sent",
                content: "Successful command execution",
                type: 'info'
            });
        });
    };

    $scope.clientRestart = function () {
        var select = $scope.selected['Clients'];
        $scope.sendClientRequest(select, 'CRESTART', function (result) {
            $.Notify({
                caption: "Restart command sent",
                content: "Successful command execution",
                type: 'info'
            });
        });
    };
}]);