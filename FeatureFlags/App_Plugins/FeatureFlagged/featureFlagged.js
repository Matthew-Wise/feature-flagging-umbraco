(function() {
    "use strict";

    var eventName = "flaggedValueChanged";

    function featureFlaggedController($scope, umbPropEditorHelper) {
        if ($scope.model.config.featureEnabled) {
            $scope.view = umbPropEditorHelper.getViewPath($scope.model.config.dataTypeSettings.view);
            $scope.$on(eventName,
                function(_, delta) {
                    $scope.model.value = delta;
                });
        }
    }
    
    angular.module("umbraco").controller("FeatureFlagged.Controller", featureFlaggedController);

    angular.module("umbraco.directives").directive("flaggedHider",
        function() {
            var link = function(scope, element) {
                if (scope.hide === true) {
                    element.parents("umb-property").hide();
                }
            };

            return {
                restrict:"E",
                replace:false,
                scope: {
                    hide: "="
                },
                link: link
            }
        });

    angular.module("umbraco.directives").directive("flaggedProperty",
        function () {
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