lotus.directive('overview', function () {
    return {
        templateUrl: '/Templates/Overview'
    };
});

lotus.directive('allClients', function () {
    return {
        templateUrl: '/Templates/AllClients'
    };
});

lotus.directive('plugins', function () {
    return {
        templateUrl: '/Templates/Plugins'
    };
});

lotus.directive('dynamic', function ($compile) {
    return {
        restrict: 'A',
        replace: true,
        link: function (scope, ele, attrs) {
            scope.$watch(attrs.dynamic, function (html) {
                ele.html(html);
                $compile(ele.contents())(scope);
            });
        }
    };
});

lotus.directive("ngFileModel", [function () {
    return {
        scope: {
            ngFileModel: "="
        },
        link: function (scope, element, attributes) {
            element.bind("change", function (changeEvent) {
                var reader = new FileReader();
                reader.onload = function (loadEvent) {
                    scope.$apply(function () {
                        scope.ngFileModel = {
                            lastModified: changeEvent.target.files[0].lastModified,
                            lastModifiedDate: changeEvent.target.files[0].lastModifiedDate,
                            name: changeEvent.target.files[0].name,
                            size: changeEvent.target.files[0].size,
                            type: changeEvent.target.files[0].type,
                            data: loadEvent.target.result
                        };
                    });
                }
                reader.readAsDataURL(changeEvent.target.files[0]);
            });
        }
    }
}]);