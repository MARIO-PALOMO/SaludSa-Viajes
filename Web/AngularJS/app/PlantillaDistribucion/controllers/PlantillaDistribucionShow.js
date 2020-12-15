
/**
 * Eduardo: Gestiona la lógica de control de la vista create.
 */
app_platilla_distribucion.controller('PlantillaDistribucionShow', [
    '$scope',
    '$rootScope',
    '$uibModal',
    '$uibModalInstance',
    '$log',
    '$http',
    'data',
	function ($scope, $rootScope, $uibModal, $uibModalInstance, $log, $http, data) {

        $scope.Data = data;
        $scope.Distribucion = {};
		$scope.Distribucion.Detalles = [];

		$scope.TotalDetalle = '0.00';

        /**
         * Eduardo: Función que se invoca al guardar el modal.
         */
        $scope.Ok = function (result) {
            $uibModalInstance.close(result);
        };

        /**
         * Eduardo: Función que se invoca cuando se cancela el modal.
         */
        $scope.Cerrar = function () {
            $uibModalInstance.dismiss();
		};

		/**
         * Eduardo:
         */
		$scope.CalcularTotalDetalle = function () {
			$scope.TotalDetalle = '0.00';

			angular.forEach($scope.Data.Distribuciones, function (item) {
				var tem = item.Porcentaje ? item.Porcentaje : 0;
				$scope.TotalDetalle = (parseFloat($scope.TotalDetalle) + parseFloat(tem)).toFixed(2);
			});
		}
		$scope.CalcularTotalDetalle();
		
    }]);