lotus.service("ClientStore", ["$cookies", "$websocket", "uuid", function ($cookies, $websocket, uuid) {
    var vm = this;

    vm.cthumbs = [];
    vm.requests = {};

    var dataStream = $websocket("ws://localhost:8888/COP");

    dataStream.onMessage(function (message) {
        var data = message.data;
        var response = angular.fromJson(data);
        for (var request in vm.requests) {
            if (request === response.ID) {
                var text = null;
                try {
                    text = angular.fromJson(response.Data);
                } catch (e) {
                    text = response.Data;
                }
                vm.requests[request](text);
                delete vm.requests[request];
                return;
            }
        }
        console.log("WARN: unknown response id: " + response.ID);
    });

    vm.updateClientList = function (success) {
        var request = vm.buildRequest("GETCTHUMBS");
        vm.sendRequest(request, function (result) {
            vm.cthumbs = result;
            if (success) {
                success();
            }
            /*
            for (var thumb of result) {
                var built = $scope.buildRequest("CGETDRIVES", thumb.CIdentifier);
                $scope.sendRequest(built, function (result) {
                    console.log(result);
                });
            }
            */
        });
    };

    vm.sendClientRequest = function (thumb, request, callback) {
        var built = vm.buildRequest(request, thumb.CIdentifier);
        vm.sendRequest(built, callback);
    };

    vm.sendRequest = function (request, callback) {
        vm.requests[request.ID] = callback;
        dataStream.send(angular.toJson(request));
    };

    vm.buildRequest = function (command) {
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
}]);