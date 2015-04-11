var app = angular.module("app", ['ngRoute']);

app.run(['$templateCache', '$http', '$rootScope', '$window', function ($templateCache, $http, $rootScope, $window) {
    $http.get('/Scripts/app/views/home.html', { cache: $templateCache });
    $http.get('/Scripts/app/views/login.html', { cache: $templateCache });
    $http.get('/Scripts/app/views/register.html', { cache: $templateCache });
    $http.get('/Scripts/app/views/todo-items.html', { cache: $templateCache });
     
    $rootScope.logout = function () {
        $window.sessionStorage.removeItem('accessToken');
        $window.location.href = '/#/login';
        $rootScope.loggedIn = false;
    }
}]);

app.config(['$routeProvider',
    function ($routeProvider) {
        $routeProvider.
            when('/', {
                templateUrl: '/Scripts/app/views/home.html'
            }).
            when('/login', {
                templateUrl: '/Scripts/app/views/login.html'
            }).
            when('/register', {
                templateUrl: '/Scripts/app/views/register.html'
            }).
            when('/todos', {
                templateUrl: '/Scripts/app/views/todo-items.html'
            }).
            otherwise({
                redirectTo: '/'
            });
    }
]);

app.controller("LogoutCtrl", ['$scope', '$window', '$location', function ($scope, $window, $location) {
    $window.sessionStorage.removeItem('accessToken');
    $location.path('/login');
}]);

app.controller("RegisterCtrl", ['$scope', '$http', '$location', function ($scope, $http, $location) {

    $scope.update = function (user) {

        var data = {
            Email: user.email,
            Password: user.password,
            ConfirmPassword: user.passwordConfirm
        };

        $http.post("/api/Account/Register", data)
            .success(function (data) {
                $location.path('/login');
            });
    }
}]);

app.controller("LoginCtrl", ['$rootScope', '$scope', '$http', '$window', '$location', function ($rootScope, $scope, $http, $window, $location) {
    $scope.login = function (user) {
        var data = {
            grant_type: 'password',
            username: user.email,
            password: user.password
        };

        $http({
            url: '/Token',
            method: 'POST',
            data: "username=" + user.email + "&password=" + user.password + "&grant_type=password"
        })
            .success(function (data) {
                $window.sessionStorage.setItem('accessToken', data.access_token);
                $rootScope.loggedIn = true;
                $location.path('/todos');
            });
    }
}]);

app.controller("TodoItemsCtrl", ['$scope', '$http', '$window', '$location', function ($scope, $http, $window, $location) {

    var token = $window.sessionStorage.getItem('accessToken');
    var headers = {};
    if (token) {
        headers.Authorization = 'Bearer ' + token;
    } else {
        $location.path('/login');
    }

    $http({
        method: 'GET',
        url: '/api/TodoItems',
        headers: headers
    })
    .success(function (data) {
        $scope.todos = data;
    });

    $scope.add = function (name) {
        $http({
            method: 'POST',
            url: '/api/TodoItems/',
            data: { Name: name },
            headers: headers
        })
        .success(function (data) {
            $scope.todos.push(data);
            $scope.name = '';
        });
    }

    $scope.remove = function (todo) {
        $http({
            method: 'DELETE',
            url: '/api/TodoItems/' + todo.Id,
            headers: headers
        })
        .success(function (data) {
            $scope.todos.splice($scope.todos.indexOf(todo), 1);
        });
    }
}]);