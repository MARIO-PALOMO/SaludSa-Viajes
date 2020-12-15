
/**
 * Gestiona la lógica de control de la vista edit del módulo Parámetro Pago.
 */
app_parametro_pago.controller('ParametroPagoEdit', [
    '$scope',
    '$rootScope',
    '$uibModal',
    '$uibModalInstance',
    '$log',
    '$http',
	'data',
	'json_js_date',
	function ($scope, $rootScope, $uibModal, $uibModalInstance, $log, $http, data, json_js_date) {

        $scope.Data = data;
        $scope.Parametro = {};
		$scope.Metadatos = {};

        $scope.Ok = function (result) {
            $uibModalInstance.close(result);
        };

        $scope.Cerrar = function () {
            $uibModalInstance.dismiss();
		};

		

    }]);