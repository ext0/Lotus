lotus.controller("PluginController", ["$scope", "$http", "$window", "$cookies", "$websocket", "uuid", "ClientStore", "PluginStore", function ($scope, $http, $window, $cookies, $websocket, uuid, ClientStore, PluginStore) {
    var vm = $scope;

    vm.pluginStore = PluginStore;

    vm.pluginUploadFormOpen = false;
    vm.pluginUpload = {};
    vm.pluginUploadErrorMessage;

    vm.pluginUploading = false;

    vm.pluginUpdateFormOpen = false;
    vm.pluginUpdate = {};
    vm.pluginUpdateErrorMessage;

    vm.pluginUpdating = false;

    vm.pluginStore.loadPlugins(function () {
        vm.plugins = vm.pluginStore.plugins;
    });

    vm.pluginStore.loadUserPlugins(function () {
        vm.userPlugins = vm.pluginStore.userPlugins;
    });

    vm.refreshPlugins = function () {
        vm.pluginStore.loadPlugins(function () {
            vm.plugins = vm.pluginStore.plugins;
        });

        vm.pluginStore.loadUserPlugins(function () {
            vm.userPlugins = vm.pluginStore.userPlugins;
        });
    }

    vm.topRightButtonAction = function () {
        if (!vm.pluginUpdateFormOpen && !vm.pluginUploadFormOpen) {
            vm.pluginUploadFormOpen = true;
        } else {
            vm.pluginUploadFormOpen = false;
            vm.pluginUpdateFormOpen = false;
        }
    }

    vm.isEnabledPlugin = function (plugin) {
        if (!vm.userPlugins) {
            return false;
        }
        return vm.userPlugins.filter(x => x.Name === plugin.Name && x.Version === plugin.Version).length !== 0;
    }

    vm.openUpdateForm = function (plugin) {
        vm.pluginUpdateFormOpen = true;
        vm.pluginUpdate = angular.copy(plugin);

        //why
        vm.pluginUpdate.Lass = vm.pluginUpdate.AbsoluteClassPathName;
        delete vm.pluginUpdate.AbsoluteClassPathName;
        vm.pluginUpdate.Name2 = vm.pluginUpdate.TabHeader;
        delete vm.pluginUpdate.TabHeader;
        vm.pluginUpdate.Name3 = vm.pluginUpdate.TabIcon;
        delete vm.pluginUpdate.TabIcon;
    };

    vm.togglePlugin = function (plugin) {
        var isEnabled = vm.isEnabledPlugin(plugin);
        if (!isEnabled) {
            vm.pluginStore.enablePlugin(plugin, function (data) {
                $.Notify({
                    caption: "Success",
                    content: "successfully enabled " + plugin.Name,
                    type: 'info'
                });
                vm.refreshPlugins();
            }, function (err) {
                $.Notify({
                    caption: "Failed to enable plugin",
                    content: err.data.Error,
                    type: 'alert'
                });
            });
        } else {
            vm.pluginStore.disablePlugin(plugin, function (data) {
                $.Notify({
                    caption: "Success",
                    content: "successfully disabled " + plugin.Name,
                    type: 'info'
                });
                vm.refreshPlugins();
            }, function (err) {
                $.Notify({
                    caption: "Failed to disable plugin",
                    content: err.data.Error,
                    type: 'alert'
                });
            });
        }
    };

    vm.updatePlugin = function () {
        if (!vm.pluginUpdate.Dass || !vm.pluginUpdate.Template || !vm.pluginUpdate.Ontroller) {
            vm.pluginUpdateErrorMessage = "Missing file uploads";
            return;
        }
        // the stupid names here are because for some reason Json.JsonObject freaks out with certain strings...

        vm.pluginUpdateErrorMessage = undefined;

        var pluginUpdateCopy = angular.copy(vm.pluginUpdate);

        pluginUpdateCopy.Dass = vm.pluginUpdate.Dass.data.split(',')[1];
        pluginUpdateCopy.Template = vm.pluginUpdate.Template.data.split(',')[1];
        pluginUpdateCopy.Ontroller = vm.pluginUpdate.Ontroller.data.split(',')[1];
        // is updating
        pluginUpdateCopy.Name4 = true;

        vm.pluginUpating = true;

        PluginStore.uploadPlugin(pluginUpdateCopy, function (data) {
            $.Notify({
                caption: "Successfully updated " + pluginUpdateCopy.Name + "!",
                content: " ",
                type: 'info'
            });
            vm.pluginUpdateFormOpen = false;
            vm.pluginUpdating = false;
            vm.refreshPlugins();
        }, function (err) {
            $.Notify({
                caption: "Failed to update plugin",
                content: err.data.Error,
                type: 'alert'
            });
            vm.pluginUpdating = false;
        });
    }

    vm.uploadPlugin = function () {
        if (!vm.pluginUpload.Dass || !vm.pluginUpload.Template || !vm.pluginUpload.Ontroller) {
            vm.pluginUploadErrorMessage = "Missing file uploads";
            return;
        }
        // the stupid names here are because for some reason Json.JsonObject freaks out with certain strings...

        vm.pluginUploadErrorMessage = undefined;

        var pluginUploadCopy = angular.copy(vm.pluginUpload);

        pluginUploadCopy.Dass = vm.pluginUpload.Dass.data.split(',')[1];
        pluginUploadCopy.Template = vm.pluginUpload.Template.data.split(',')[1];
        pluginUploadCopy.Ontroller = vm.pluginUpload.Ontroller.data.split(',')[1];
        // is updating
        pluginUploadCopy.Name4 = false;

        vm.pluginUploading = true;

        PluginStore.uploadPlugin(pluginUploadCopy, function (data) {
            vm.pluginUploadFormOpen = false;
            vm.pluginUploading = false;
            vm.refreshPlugins();
        }, function (err) {
            $.Notify({
                caption: "Failed to upload plugin",
                content: err.data.Error,
                type: 'alert'
            });
            vm.pluginUploading = false;
        });
    }
}]);