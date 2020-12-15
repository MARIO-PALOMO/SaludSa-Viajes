
/**
 * Gestiona la lógica de control de la vista edit del módulo Impuesto Pago.
 */
app_impuesto_pago.controller('ImpuestoPagoEdit', [
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
        $scope.Impuesto = {};
		$scope.Metadatos = {};

        $scope.Ok = function (result) {
            $uibModalInstance.close(result);
        };

        $scope.Cerrar = function () {
            $uibModalInstance.dismiss();
		};

		$scope.ObtenerMetadatos = function () {
			$http.get($rootScope.pathURL + '/ImpuestoPago/ObtenerMetadatos')
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						$scope.Metadatos = response;

						$scope.CargarDatos();
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.CargarDatos = function () {
			$http.get($rootScope.pathURL + '/ImpuestoPago/Detalle?Id=' + $scope.Data.Id)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Impuesto = response.impuesto;

						$scope.Impuesto.Porcentaje = $scope.Impuesto.Porcentaje.toFixed(2);
						$scope.Impuesto.Compensacion = $scope.Impuesto.Compensacion.toFixed(2);
					}
				});
		}

		$scope.Validar = function () {
			var errores = '';

			if (!$scope.Impuesto.Descripcion) {
				errores = errores + '<li>No ha entrado una descripción.</li>';
			}

			if (!$scope.Impuesto.Porcentaje) {
				$scope.Impuesto.Porcentaje = '0.00';
			}

			if (!(parseFloat($scope.Impuesto.Porcentaje) > 0)) {
				errores = errores + '<li>El campo "Porcentaje" debe tener un valor numérico mayor que cero.</li>';
			}

			if (!$scope.Impuesto.Compensacion) {
				$scope.Impuesto.Compensacion = '0.00';
			}

			if (!(parseFloat($scope.Impuesto.Compensacion) > 0)) {
				errores = errores + '<li>El campo "Compensación" debe tener un valor numérico mayor que cero.</li>';
			}

			if (!$scope.Impuesto.Estado || !$scope.Impuesto.Estado.Id) {
				errores = errores + '<li>No ha seleccionado un estado.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		}

		$scope.Crear = function () {
			if ($scope.Validar()) {

				var Impuesto = {
					Id: $scope.Impuesto.Id,
					Descripcion: $scope.Impuesto.Descripcion ? $scope.Impuesto.Descripcion.toUpperCase() : null,
					Porcentaje: $scope.Impuesto.Porcentaje ? parseFloat($scope.Impuesto.Porcentaje) : parseFloat(0),
					Compensacion: $scope.Impuesto.Compensacion ? parseFloat($scope.Impuesto.Compensacion) : parseFloat(0),
					EstadoId: $scope.Impuesto.Estado.Id
				};

				$http.post($rootScope.pathURL + '/ImpuestoPago/Edit', { Impuesto: Impuesto })
					.success(function (response) {
						var mensajes = '';

						angular.forEach(response.error, function (value, index) {
							mensajes = mensajes + '<li>' + value.error + '</li>';
						});

						angular.forEach(response.validacion, function (value, index) {
							mensajes = mensajes + '<li>' + value + '</li>';
						});

						if (mensajes.length > 1) {
							bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						}
						else {
							$scope.Ok($scope.Impuesto);
							toastr['success']("Elemento procesado correctamente.", "Confirmación");
						}
					});
			}
		}

    }]);