
/**
 * Gestiona la lógica de control de la vista buscar_factura_electronica del módulo Tarea.
 */
app_tarea.controller('BuscarFacturasElectronicas', [
    '$scope',
    '$rootScope',
    '$uibModalInstance',
    '$log',
	'$http',
	'$window',
	'data',
	function ($scope, $rootScope, $uibModalInstance, $log, $http, $window, data) {

		$scope.Data = data;
		$scope.seleccionar_todo = false;
		$scope.facturas = [];

		$scope.ruc = $scope.Data.ruc;

		$scope.Ok = function (result) {
			$uibModalInstance.close(result);
		};

		$scope.Cerrar = function () {
			$uibModalInstance.dismiss();
		};

		$scope.SeleccionarTodo = function () {
			angular.forEach($scope.facturas, function (item) {
				item.seleccionada = $scope.seleccionar_todo;
			});
		};

		$scope.ValidarFiltros = function () {
			var errores = '';

			if (!$scope.ruc && !$scope.documento_est && !$scope.documento_pte && !$scope.documento_sec && !$scope.fecha_desde && !$scope.fecha_hasta) {
				errores = errores + '<li>Debe entrar algún criterio de búsqueda.</li>';
			}

			var errorDocumento = false;

			if ($scope.documento_est) {
				if (!$scope.documento_pte || !$scope.documento_sec) {
					errorDocumento = true;
				}
			}

			if ($scope.documento_pte) {
				if (!$scope.documento_est || !$scope.documento_sec) {
					errorDocumento = true;
				}
			}

			if ($scope.documento_sec) {
				if (!$scope.documento_pte || !$scope.documento_est) {
					errorDocumento = true;
				}
			}

			if (errorDocumento) {
				errores = errores + '<li>Debe entrar los tres campos que conforman el número de documento.</li>';
			}

			var errorFecha = false;

			if ($scope.fecha_desde) {
				if (!$scope.fecha_hasta) {
					errorFecha = true;
					errores = errores + '<li>Debe entrar el campo fecha hasta.</li>';
				}
			}

			if ($scope.fecha_hasta) {
				if (!$scope.fecha_desde) {
					errorFecha = true;
					errores = errores + '<li>Debe entrar el campo fecha desde.</li>';
				}
			}

			if (!errorFecha) {
				var fecha_ini = $scope.fecha_desde.split('/');
				var fecha_fin = $scope.fecha_hasta.split('/');

				if ((fecha_ini[2] + fecha_ini[1] + fecha_ini[0]) > (fecha_fin[2] + fecha_fin[1] + fecha_fin[0])) {
					errores = errores + '<li>La fecha hasta no debe ser inferior a la fecha desde.</li>';
				}
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		};

		$scope.Buscar = function () {
			if ($scope.ValidarFiltros()) {
				$http.post($rootScope.pathURL + '/ComprobanteElectronico/Buscar', { ruc: ($scope.ruc ? $scope.ruc : null), establecimiento: ($scope.documento_est ? $scope.documento_est : null), puntoEmision: ($scope.documento_pte ? $scope.documento_pte : null), secuencial: ($scope.documento_sec ? $scope.documento_sec : null), fechaInicio: ($scope.fecha_desde ? $scope.fecha_desde : null), fechaFin: ($scope.fecha_hasta ? $scope.fecha_hasta : null) })
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
							$scope.facturas = response.comprobantes;
						}
					});
			}
		};

		$scope.Crear = function () {

			var comprobantesSeleccionados = [];

			angular.forEach($scope.facturas, function (item) {
				if (item.seleccionada) {
					comprobantesSeleccionados.push(item);
				}
			});

			if (comprobantesSeleccionados.length > 0) {
				$scope.Ok(comprobantesSeleccionados);
			}
			else {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> No ha seleccionado ningún comprobante electrónico.');
			}
		};

    }]);