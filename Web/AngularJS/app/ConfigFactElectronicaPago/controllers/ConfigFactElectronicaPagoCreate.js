
/**
 * Gestiona la lógica de control de la vista create del módulo Configuración de Factura Electrónica.
 */
app_config_fact_electronica_pago.controller('ConfigFactElectronicaPagoCreate', [
    '$scope',
    '$rootScope',
    '$uibModal',
    '$uibModalInstance',
    '$log',
    '$http',
    'data',
	function ($scope, $rootScope, $uibModal, $uibModalInstance, $log, $http, data) {

		$scope.Metadatos = {};
        $scope.Data = data;
        $scope.Impuesto = {};

        $scope.Ok = function (result) {
            $uibModalInstance.close(result);
        };

        $scope.Cerrar = function () {
            $uibModalInstance.dismiss();
		};

    }]);