
/**
 * Gestiona la lógica de control de la vista index del módulo Detener Solicitudes.
 */
app_detener_solicitudes.controller('DetenerSolicitudesIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
	'$http',
	'$window',
	function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http, $window) {

		$scope.Tareas = [];
		$scope.Metadatos = {};
		$scope.Metadatos.Procesos = [{
			Id: 1,
			Nombre: 'Compras'
		}];

		$scope.Proceso = $scope.Metadatos.Procesos[0];

		$scope.ObtenerMetadatos = function () {
			$http.get($rootScope.pathURL + '/DetenerSolicitud/ObtenerMetadatos')
				.success(function (response) {

					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos.Empresas = response.Empresas;
						$scope.Empresa = $scope.Metadatos.Empresas[0];
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.Validar = function () {
			var errores = '';

			if (!$scope.Empresa || !$scope.Empresa.Codigo) {
				errores = errores + '<li>No ha seleccionado una empresa.</li>';
			}

			if (!$scope.Proceso || !$scope.Proceso.Id) {
				errores = errores + '<li>No ha seleccionado un proceso.</li>';
			}

			if (!$scope.NumeroSolicitud) {
				errores = errores + '<li>No ha entrado un número de solicitud.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		}

		$scope.Detener = function () {
			if ($scope.Validar()) {

				bootbox.confirm('<i class="fa fa-question-circle fa-3x text-danger" style="position: relative; top:10px;"></i> ¿Confirma que desea detener la solicitud?', function (result) {
					if (result) {
						$http.post($rootScope.pathURL + '/DetenerSolicitud/Detener', { NumeroSolicitud: $scope.NumeroSolicitud, EmpresaCodigo: $scope.Empresa.Codigo })
							.success(function (response) {
								var mensajes = '';

								angular.forEach(response.validacion, function (value) {
									mensajes = mensajes + '<li>' + value + '</li>';
								});

								if (mensajes.length > 1) {
									bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
								} else {
									toastr['success']("Solicitud de compra detenida correctamente.", "Confirmación");
								}
							});
					}
				});
			}
		}

    }]);