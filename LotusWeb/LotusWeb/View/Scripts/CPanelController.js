lotus.controller("CPanelController", ["$scope", "$http", "$window", "$cookies", "$websocket", "uuid", "ClientStore", "PluginStore", "$sce", function ($scope, $http, $window, $cookies, $websocket, uuid, ClientStore, PluginStore, $sce) {
    var vm = $scope;
    if (!vm.authenticated) {
        console.log("WARN: Not authenticated!");
        $window.location.href = "/Login";
        return;
    }

    vm.selected = null;
    vm.refreshing = false;

    vm.clientStore = ClientStore;
    vm.pluginStore = PluginStore;

    /*
        TAB SYSTEM
    */

    vm.sidebar = [
        {
            title: "overview",
            caption: () => { return "" },
            icon: "mif-apps"
        },
        {
            title: "clients",
            caption: () => { return vm.clientStore.cthumbs.length + " client" + ((vm.clientStore.cthumbs.length === 1) ? "" : "s") },
            icon: "mif-stack"
        },
        {
            title: "plugins",
            caption: () => {
                return vm.pluginStore.plugins.length + " plugin" + ((vm.pluginStore.plugins.length === 1) ? "" : "s") + " available"
            },
            icon: "mif-versions"
        },
        {
            title: "world map",
            caption: () => { return "" },
            icon: "mif-earth"
        },
        {
            title: "configuration",
            caption: () => { return "" },
            icon: "mif-cogs"
        }
    ];

    vm.activeTab = vm.sidebar[0];

    vm.sidebarClick = function (tab) {
        vm.activeTab = tab;
    };

    /*
        CLIENT CONTROL
    */

    PluginStore.loadPlugins();
    PluginStore.loadUserPlugins();

    ClientStore.updateClientList();

    vm.buttonsLoading = {};

    vm.clientSelected = function () {
        return vm.selected !== null;
    }

    vm.clientDetach = function () {
        vm.clientSelect(null);
    }

    vm.clientSelect = function (client) {
        vm.selected = client;
        window.selectedClient = client;
    }

    vm.clientHasPlugin = function (plugin, client) {
        if (!client) {
            return false;
        }
        return client.InstalledPlugins.filter(x => { return x.Name === plugin.Name && x.Version === plugin.Version }).length !== 0;
    }

    vm.markAsLoadingPlugin = function (plugin, client) {
        if (!vm.buttonsLoading[client]) {
            vm.buttonsLoading[client] = {};
        }
        vm.buttonsLoading[client][plugin] = true;
    }

    vm.markAsDoneLoadingPlugin = function (plugin, client) {
        if (!vm.buttonsLoading[client]) {
            vm.buttonsLoading[client] = {};
        }
        vm.buttonsLoading[client][plugin] = false;
    }

    vm.isLoadingPlugin = function (plugin, client) {
        if (!vm.buttonsLoading[client]) {
            return false;
        }
        return !!vm.buttonsLoading[client][plugin];
    }

    vm.getLocalizedPlugins = function (client) {
        if (!client) {
            return [];
        }
        var val = client.InstalledPlugins.map(function (plugin) {
            return vm.pluginStore.userPlugins.filter(x => { return x.Name === plugin.Name && x.Version === plugin.Version })[0];
        });
        return val.filter(plugin => !!plugin);
    };

    vm.toggleActivePlugin = function (plugin, client) {
        var hasPlugin = vm.clientHasPlugin(plugin, client);
        vm.markAsLoadingPlugin(plugin, client);
        if (!hasPlugin) {
            var request = vm.clientStore.buildRequest("INSTALLPLUGIN", client.CIdentifier, plugin.Name);
            vm.clientStore.sendRequest(request, function (data) {
                vm.markAsDoneLoadingPlugin(plugin, client);
                var success = data === "SUCCESS";
                if (!success) {
                    $.Notify({
                        caption: "Failed to install plugin",
                        content: " ",
                        type: 'alert'
                    });
                } else {
                    client.InstalledPlugins.push(plugin);
                }
            });
        } else {
            var request = vm.clientStore.buildRequest("DISABLEPLUGIN", client.CIdentifier, plugin.Name);
            vm.clientStore.sendRequest(request, function (data) {
                vm.markAsDoneLoadingPlugin(plugin, client);
                var success = data === "SUCCESS";
                if (!success) {
                    $.Notify({
                        caption: "Failed to uninstall plugin",
                        content: " ",
                        type: 'alert'
                    });
                } else {
                    client.InstalledPlugins = client.InstalledPlugins.filter(x => { return x.Name !== plugin.Name && x.Version !== plugin.Version });
                }
            });
        }
    }

    vm.clientRefresh = function () {
        vm.refreshing = true;
        ClientStore.updateClientList(function () {
            vm.refreshing = false;
        });
    }
}]);