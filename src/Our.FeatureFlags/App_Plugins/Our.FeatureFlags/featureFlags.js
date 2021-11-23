(function () {
    "use strict";
    var eventName = "OurFeatureFlagsChanged";

    function featureFlaggedController($scope, umbPropEditorHelper) {
        if (!$scope.model.config.featureEnabled) { return }
        $scope.view = umbPropEditorHelper.getViewPath($scope.model.config.dataTypeSettings.view);
        var unsubscribe = $scope.$on(eventName, (_, delta) => { $scope.model.value = delta });
        $scope.$on("$destroy", () => { unsubscribe() });
    }

    angular.module("umbraco").controller("Our.FeatureFlags.Controller", featureFlaggedController);

    angular.module("umbraco.directives").directive("featureFlagsHider", function () {
        var link = function (scope, element) {
            if (scope.hide) {
                element.parents("umb-property").hide();
            }
        };

        return {
            restrict: "E",
            replace: false,
            scope: { hide: "=" },
            link: link
        }
    });

    angular.module("umbraco.directives").directive("featureFlagsProperty", function () {
        var link = function (scope) {
            scope.model = {};
            scope.model.config = scope.config;
            scope.model.alias = scope.propertyAlias;
            scope.model.value = scope.value;
            scope.model.validation = scope.validation;

            scope.$watch("model.value", function (newValue) {
                scope.$emit(eventName, newValue);
            }, true);
        };

        return {
            restrict: "E",
            rep1ace: true,
            link: link,
            templateUrl: "views/components/property/umb-property-editor.html",
            scope: {
                propertyEditorView: "=view",
                config: "=",
                propertyAlias: "=",
                value: "=",
                validation: "="
            }
        };
    });
})();