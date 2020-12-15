﻿
/**
 * Gestiona la lógica de control de la vista create del módulo Tipo Pago.
 */
app_tipo_pago_pago.controller('TipoPagoPagoCreate', [
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
		$scope.TipoPago = {};

        $scope.Ok = function (result) {
            $uibModalInstance.close(result);
        };

        $scope.Cerrar = function () {
            $uibModalInstance.dismiss();
		};

		$scope.ObtenerMetadatos = function () {
			$http.get($rootScope.pathURL + '/TipoPagoPago/ObtenerMetadatos')
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

						$scope.TipoPago.Estado = {};
						$scope.TipoPago.CuentaContable = {};
						$scope.TipoPago.Empresa = response.EmpresaActiva;
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.CambiarEmpresa = function () {
			$scope.ObtenerCuentasContable();
		}

		$scope.ObtenerCuentasContable = function () {
			$http.get($rootScope.pathURL + '/TipoPagoPago/ObtenerCuentasContable?EmpresaCodigo=' + $scope.TipoPago.Empresa.Codigo)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						$scope.Metadatos.CuentasContable = response.CuentasContable;
						$scope.TipoPago.CuentaContable = {};
					}
				});
		}

		$scope.Validar = function () {
			var errores = '';

			if (!$scope.TipoPago.Empresa || !$scope.TipoPago.Empresa.Codigo) {
				errores = errores + '<li>No ha seleccionado una empresa.</li>';
			}

			if (!$scope.TipoPago.CuentaContable || !$scope.TipoPago.CuentaContable.Codigo) {
				errores = errores + '<li>No ha seleccionado una cuenta contable.</li>';
			}

			if (!$scope.TipoPago.Referencia) {
				errores = errores + '<li>No ha entrado una referencia.</li>';
			}

			if (!$scope.TipoPago.Estado || !$scope.TipoPago.Estado.Id) {
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
				var TipoPago = {
					Id: 0,
					Referencia: $scope.TipoPago.Referencia ? $scope.TipoPago.Referencia.toUpperCase() : null,
					CuentaContableCodigo: $scope.TipoPago.CuentaContable.Codigo,
					CuentaContableNombre: $scope.TipoPago.CuentaContable.Nombre,
					CuentaContableTipo: $scope.TipoPago.CuentaContable.Tipo,
					EsReembolso: $scope.TipoPago.EsReembolso ? $scope.TipoPago.EsReembolso : false,
					EstadoId: $scope.TipoPago.Estado.Id,
					EmpresaCodigo: $scope.TipoPago.Empresa.Codigo
				};

				$http.post($rootScope.pathURL + '/TipoPagoPago/Create', { TipoPago: TipoPago })
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
							$scope.TipoPago.Id = response.Id;
							$scope.Ok($scope.TipoPago);
							toastr['success']("Elemento procesado correctamente.", "Confirmación");
						}
					});
			}
		}

    }]);