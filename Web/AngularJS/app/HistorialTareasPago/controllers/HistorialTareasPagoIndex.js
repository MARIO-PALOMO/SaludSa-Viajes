
/**
 * Gestiona la lógica de control de la vista index del módulo Historial de Tareas de Pago.
 */
app_historial_tareas_pago.controller('HistorialTareasPagoIndex', [
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
			Nombre: 'Pagos'
		}];

		$scope.Proceso = $scope.Metadatos.Procesos[0];

		$scope.ObtenerMetadatos = function () {
			var EmpresaCodigo = $('#EmpresaCodigo').val();
			var NumeroSolicitud = $('#NumeroSolicitud').val();

			$http.get($rootScope.pathURL + '/HistorialTareasPago/ObtenerMetadatos?NumeroSolicitud=' + NumeroSolicitud + '&EmpresaCodigo=' + EmpresaCodigo)
				.success(function (response) {

					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos.Empresas = response.Empresas;

						if (EmpresaCodigo && NumeroSolicitud) {
							$scope.NumeroSolicitud = NumeroSolicitud;

							var emp = $scope.Metadatos.Empresas.filter(function (item) {
								return item.Codigo == EmpresaCodigo;
							});

							$scope.Empresa = (emp ? emp[0] : {});

							$scope.Tareas = response.Tareas;
						}
						else {
							$scope.Empresa = $scope.Metadatos.Empresas[0];
						}
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

		$scope.Buscar = function () {
			if ($scope.Validar()) {
				$http.get($rootScope.pathURL + '/HistorialTareasPago/Buscar?NumeroSolicitud=' + $scope.NumeroSolicitud + '&Empresa=' + $scope.Empresa.Codigo)
					.success(function (response) {
						var mensajes = '';

						angular.forEach(response.validacion, function (value) {
							mensajes = mensajes + '<li>' + value + '</li>';
						});

						if (mensajes.length > 1) {
							bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						} else {
							$scope.Tareas = response.Tareas;
						}
					});
			}
		}

    }]);