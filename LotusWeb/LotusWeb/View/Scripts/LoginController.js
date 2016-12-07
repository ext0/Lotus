lotus.controller("LoginController", ["$scope", "$http", "$window", function ($scope, $http, $window) {
    $scope.Login = {};
    $scope.submit = function () {
        if ($scope.LoginForm.$valid) {
            $http.post("/LoginUser/", $scope.Login).then(function (data) {
                $.Notify({
                    caption: "Success!",
                    content: "Successfully logged in!",
                    type: 'success'
                });
                $window.location = "/";
            }, function (error) {
                $.Notify({
                    caption: "Failed to login!",
                    content: error.data.Error,
                    type: 'alert'
                });
            });
        }
    }
}]);