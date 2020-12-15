
/**
 * Gestiona la lógica de control de la vista index del módulo Reasignacion de Originador Compra.
 */
app_reasignacion_originador_compra.controller('ReasignacionOriginadorCompraIndex', [
	'$scope',
	'$rootScope',
	'DTOptionsBuilder',
	'DTColumnDefBuilder',
	'$uibModal',
	'$log',
	'$http',
	function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http) {

		$scope.Metadatos = [];

		$scope.Solicitudes = [];

		$scope.SeleccionarTodo = false;

		$scope.ObtenerMetadatos = function () {
			$http.get($rootScope.pathURL + '/ReasignacionOriginadorCompra/ObtenerMetadatos')
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos = response;

						$scope.ResponsableActual = {};
						$scope.ResponsableNuevo = {};
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.ObtenerSolicitudesPorResponsable = function () {
			if ($scope.ResponsableActual && $scope.ResponsableActual.Usuario) {
				$http.get($rootScope.pathURL + '/ReasignacionOriginadorCompra/ObtenerSolicitudesPorResponsable?UsuarioResponsable=' + $scope.ResponsableActual.Usuario)
					.success(function (response) {
						var mensajes = '';

						angular.forEach(response.validacion, function (value) {
							mensajes = mensajes + '<li>' + value + '</li>';
						});

						if (mensajes.length > 1) {
							bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						} else {
							$scope.Solicitudes = response.Solicitudes;
						}
					});
			}
		}

		$scope.SeleccionarTodoChange = function () {
			angular.forEach($scope.Solicitudes, function (item) {
				item.Seleccionada = $scope.SeleccionarTodo;
			});
		}

		$scope.Reasignar = function () {
			var errores = '';

			if (!$scope.ResponsableActual || !$scope.ResponsableActual.Usuario) {
				errores = errores + '<li>Debe seleccionar el "Originador actual".</li>';
			}

			if (!$scope.ResponsableNuevo || !$scope.ResponsableNuevo.Usuario) {
				errores = errores + '<li>Debe seleccionar el "Originador nuevo".</li>';
			}

			var SolicitudesReasignar = [];

			angular.forEach($scope.Solicitudes, function (item) {
				if (item.Seleccionada) {
					SolicitudesReasignar.push(item.Id);
				}
			});

			if (SolicitudesReasignar.length == 0) {
				errores = errores + '<li>Debe seleccionar al menos una solicitud para reasignar.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
			}
			else {
				$http.post($rootScope.pathURL + '/ReasignacionOriginadorCompra/Reasignar', { SolicitudesReasignar: SolicitudesReasignar, ResponsableNuevo: $scope.ResponsableNuevo.Usuario, ResponsableAnterior: $scope.ResponsableActual.Usuario })
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
							$scope.SeleccionarTodo = false;
							$scope.ResponsableActual = {};
							$scope.ResponsableNuevo = {};
							$scope.Solicitudes = [];

							toastr['success']("Solicitudes reasignadas correctamente.", "Confirmación");
						}
					});
			}
		}

	}]);