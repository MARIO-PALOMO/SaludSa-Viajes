
/**
 * Gestiona la lógica de control de la vista create del módulo Data Warehouse.
 */
app_data_warehouse_pago.controller('DataWarehousePagoCreate', [
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
		$scope.Reporte = {};

		$scope.Metadatos.Filtros = [
			{
				Nombre: 'Filtro 1'
			},
			{
				Nombre: 'Filtro 2'
			},
			{
				Nombre: 'Filtro 3'
			},
			{
				Nombre: 'Filtro 4'
			},
			{
				Nombre: 'Filtro 5'
			},
		];

		$scope.Metadatos.Columnas = [
			{
				Nombre: 'Columna 1'
			},
			{
				Nombre: 'Columna 2'
			},
			{
				Nombre: 'Columna 3'
			},
			{
				Nombre: 'Columna 4'
			},
			{
				Nombre: 'Columna 5'
			},
		];

        $scope.Ok = function (result) {
            $uibModalInstance.close(result);
        };

        $scope.Cerrar = function () {
            $uibModalInstance.dismiss();
		};

    }]);