
/**
 * Gestiona la lógica de control de la vista index del módulo Reasignacion de Tareas Pago.
 */
app_reasignacion_tareas_pago.controller('ReasignacionTareasPagoIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
    '$http',
	function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http) {

		$scope.Metadatos = [];

		$scope.Tareas = [];

		$scope.SeleccionarTodo = false;

		$scope.ObtenerMetadatos = function () {
			$http.get($rootScope.pathURL + '/ReasignacionTareasPago/ObtenerMetadatos')
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

		$scope.ObtenerTareasPorResponsable = function () {
			if ($scope.ResponsableActual && $scope.ResponsableActual.Usuario) {
				$http.get($rootScope.pathURL + '/ReasignacionTareasPago/ObtenerTareasPorResponsable?UsuarioResponsable=' + $scope.ResponsableActual.Usuario)
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

		$scope.SeleccionarTodoChange = function () {
			angular.forEach($scope.Tareas, function (item) {
				item.Seleccionada = $scope.SeleccionarTodo;
			});
		}

		$scope.Reasignar = function () {
			var errores = '';

			if (!$scope.ResponsableActual || !$scope.ResponsableActual.Usuario) {
				errores = errores + '<li>Debe seleccionar el "Responsable actual".</li>';
			}

			if (!$scope.ResponsableNuevo || !$scope.ResponsableNuevo.Usuario) {
				errores = errores + '<li>Debe seleccionar el "Nuevo responsable".</li>';
			}

			var TareasReasignar = [];

			angular.forEach($scope.Tareas, function (item) {
				if (item.Seleccionada) {
					TareasReasignar.push(item.Id);
				}
			});

			if (TareasReasignar.length == 0) {
				errores = errores + '<li>Debe seleccionar al menos una tarea para reasignar.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
			}
			else {
				$http.post($rootScope.pathURL + '/ReasignacionTareasPago/Reasignar', { TareasReasignar: TareasReasignar, ResponsableNuevo: $scope.ResponsableNuevo.Usuario, ResponsableAnterior: $scope.ResponsableActual.Usuario })
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
							$scope.Tareas = [];

							toastr['success']("Tareas reasignadas correctamente.", "Confirmación");
						}
					});
			}
		}

    }]);