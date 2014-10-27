(function () {
    'use strict';
    angular.module("TechEd.controllers")
        .controller('BeerCtrl', ['$scope', '$http', function ($scope, $http) {

            var __clientId = "14B1F22CFC4A75654230472242F6B8948426BF2D";
            var __clientSecret = "ADE6438ECA2F3DAA5BD6F8D7941E478490BB04D1";
            var __baseUrl = "http://api.untappd.com/v4/thepub/local" + "?client_id=" + __clientId + "&client_secret=" + __clientSecret;

            navigator.geolocation.getCurrentPosition(function (position) {

                var url = __baseUrl + "&lat=" + position.coords.latitude;
                url += "&lng=" + position.coords.longitude;

                $http.get(url)
                    .then(function (apiResponse) {
                        $scope.checkins = apiResponse.data.response.checkins.items;
                    });
            });


        }]);
})();