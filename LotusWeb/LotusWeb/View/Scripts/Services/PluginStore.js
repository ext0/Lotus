lotus.service("PluginStore", ["$http", "$sce", "$controller", function ($http, $sce, $controller) {
    var vm = this;

    vm.plugins = [];
    vm.userPlugins = [];

    vm.loadPlugins = function (callback) {
        $http.post('/GetPlugins/').then(function (data) {
            vm.plugins = data.data;
            if (callback) {
                callback();
            }
        });
    }

    vm.loadUserPlugins = function (callback) {
        $http.post('/GetMyPlugins/').then(function (data) {
            vm.userPlugins = data.data;
            vm.userPlugins.forEach(x => {
                var scope = window.eval(x.ControllerSource);
                lotus.controller(x.Name, scope);
            });
            if (callback) {
                callback();
            }
        });
    }

    vm.enablePlugin = function (plugin, success, error) {
        $http.post('/TogglePlugin/', {
            Action: "ENABLE",
            Plugin: plugin
        }).then(success, error);
    };

    vm.disablePlugin = function (plugin, success, error) {
        $http.post('/TogglePlugin/', {
            Action: "DISABLE",
            Plugin: plugin
        }).then(success, error);
    };

    vm.uploadPlugin = function (plugin, success, error) {
        $http.post("/UploadPlugin/", plugin).then(success, error);
    }

}]);