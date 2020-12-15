
/**
 * Gestiona la lógica de control de la vista buscar_beneficiario del módulo Solicitud Pago.
 */
app_solicitud_pago.controller('SolicitudPagoBuscarBeneficiario', [
    '$scope',
    '$rootScope',
	'$uibModal',
	'$uibModalInstance',
	'$http',
	'data',
	function ($scope, $rootScope, $uibModal, $uibModalInstance, $http, data) {

		$scope.Data = data;
		$scope.Beneficiarios = [];
		$scope.tipo = 'identificacion';
		$scope.texto = '';

		$scope.Ok = function (result) {
			$uibModalInstance.close(result);
		};

		$scope.Cerrar = function () {
			$uibModalInstance.dismiss();
		};

		$scope.ValidarBuscar = function () {
			var errores = '';

			if (!$scope.texto) {
				errores = errores + '<li>Debe entrar un criterio de búsqueda.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		}

		$scope.Buscar = function () {
			if ($scope.ValidarBuscar()) {
				$http.get($rootScope.pathURL + '/SolicitudPago/ObtenerProveedoresPagoViaje?EmpresaCodigo=' + $scope.Data.EmpresaCodigo + '&identificacion=' + ($scope.tipo == 'identificacion' ? $scope.texto : '') + '&nombresApellidos=' + ($scope.tipo == 'nombre' ? $scope.texto : ''))
					.success(function (response) {
						var mensajes = '';

						angular.forEach(response.validacion, function (value) {
							mensajes = mensajes + '<li>' + value + '</li>';
						});

						if (mensajes.length > 1) {
							bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						} else {
							$scope.Beneficiarios = response.ProveedoresPagoViaje;
						}
					});
			}
		}

		$scope.Seleccionar = function (obj) {
			$scope.Ok(obj);
		}
		
    }]);