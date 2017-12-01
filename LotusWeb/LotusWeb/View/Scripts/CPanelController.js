lotus.controller("CPanelController", ["$scope", "$http", "$window", "$cookies", "$websocket", "uuid", "ClientStore", function ($scope, $http, $window, $cookies, $websocket, uuid, ClientStore) {
    if (!$scope.authenticated) {
        console.log("WARN: Not authenticated!");
        $window.location.href = "/Login";
        return;
    }

    $scope.selected = {};
    $scope.refreshing = false;

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

    $scope.clientStore = ClientStore;

    ClientStore.updateClientList();

    $scope.clientSelect = function (section, client) {
        $scope.selected[section] = client;
    }

    $scope.clientRefresh = function () {
        $scope.refreshing = true;
        ClientStore.updateClientList(function () {
            $scope.refreshing = false;
        });
    }
    /*
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
    */
}]);