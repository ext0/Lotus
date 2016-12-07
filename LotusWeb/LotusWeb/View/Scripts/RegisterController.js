lotus.directive("compareTo", function () {
    return {
        require: "ngModel",
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, element, attributes, ngModel) {
            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue == scope.otherModelValue;
            };
            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
});

lotus.controller("RegisterController", ["$scope", "$http", "$window", function ($scope, $http, $window) {
    $scope.Registration = {};
    $scope.submit = function () {
        if ($scope.RegistrationForm.$valid) {
            $http.post("/RegisterUser/", $scope.Registration).then(function (data) {
                $.Notify({
                    caption: "Success!",
                    content: "Successfully registered!",
                    type: 'success'
                });
                $window.location = "/";
            }, function (error) {
                $.Notify({
                    caption: "Failed to register!",
                    content: error.data.Error,
                    type: 'alert'
                });
            });
        }
    }
}]);