lotus.service("PluginStore", ["$http", function ($http) {
    var vm = this;

    vm.plugins = [];

    vm.loadPlugins = function() {
        $http.post('/GetPlugins/').then(function (data) {
            vm.plugins = data.data;
        });
    }
}]);