"use strict";

var WebApp = angular.module('WebApp', ['ng-fusioncharts']);

var LandingPageController = function($scope, $http) {

    $scope.inputText = "input";
    $scope.outputText = "output";
    $scope.offset = 1;
    $scope.propose = "";

    $scope.validate = function (event) {
        if ((event.keyCode < 48) || (event.keyCode > 57)) {
            event.preventDefault();
        }
        console.log(event);
    }

    $scope.encrypt = function() {
        $http({
            method: 'POST',
            url: 'Home/TranslateTextToCaesar',
            //url: 'TranslateTextToCaesar',
            data: $scope.data =
            {
                Text: $scope.inputText,
                Offset: $scope.offset
            }
        }).then(function successCallback(responseData) {
            $scope.outputText = responseData.data;
        });
    }

    $scope.decrypt = function() {
        $http({
            method: 'POST',
            url: 'Home/TranslateTextFromCaesar',
            //url: '/TranslateTextFromCaesar',
            data: $scope.data =
            {
                Text: $scope.inputText,
                Offset: $scope.offset
            }
        }).then(function successCallback(responseData) {
            $scope.outputText = responseData.data;
        });
    }

    $scope.getFrequency = function() {
        $http({
            method: 'POST',
            url: 'Home/CalculateFreq',
            //url: '/CalculateFreq',
            data: { text: $scope.inputText }
        }).then(function successCallback(responseData) {
            $scope.frequency = angular.fromJson(responseData.data);
            $scope.$apply();
        });
    }

    $scope.getPropose = function () {
        $http({
            method: 'POST',
            url: 'Home/MakePrediction',
            //url: '/MakePrediction',
            data: { text: $scope.inputText }
        }).then(function successCallback(responseData) {
                $scope.propose = angular.fromJson(responseData.data);
            });
    }
};
        
LandingPageController.$inject = ['$scope', '$http'];    