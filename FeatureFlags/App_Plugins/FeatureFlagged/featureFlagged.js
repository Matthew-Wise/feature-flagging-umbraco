(function() {
    "use strict";

    angular.module("umbraco").controller("FeatureFlagged.Controller",
        function($scope) {
            if ($scope.model.config.featureEnabled) {
                $scope.$on("flaggedValueChanged",
                    function(_, delta) {
                        $scope.model.value = delta;
                        console.log($scope.model.config);
                    });
            }
        });

    angular.module("umbraco.directives").directive("flaggedHider",
        function() {
            var link = function(scope, element) {
                console.log(scope);
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
                    scope.$emit("flaggedValueChanged", newValue);
                    console.log(newValue);
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