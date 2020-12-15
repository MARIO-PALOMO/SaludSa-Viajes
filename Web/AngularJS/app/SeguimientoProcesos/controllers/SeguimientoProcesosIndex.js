
/**
 * Gestiona la lógica de control de la vista index del módulo Seguimiento de Procesos.
 */
app_seguimiento_procesos.controller('SeguimientoProcesosIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
    '$http',
	function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http) {

		$scope.Metadatos = {};
		$scope.Metadatos.Procesos = [{
			Id: 1,
			Nombre: 'Compras'
		}];

		$scope.todosOriginadores = false;
		$scope.todosSolicitudes = false;

		var date = new Date();
		var ultimoDia = (new Date(date.getFullYear(), date.getMonth() + 1, 0)).getDate();
		$scope.fechaDesde = '01/' + ('0' + (parseInt(date.getMonth()) + 1)).slice(-2)  + '/' + date.getFullYear();
		$scope.fechaHasta = ultimoDia + '/' + ('0' + (parseInt(date.getMonth()) + 1)).slice(-2) + '/' + date.getFullYear();

		$scope.Proceso = $scope.Metadatos.Procesos[0];

		$scope.ObtenerMetadatos = function () {
			$http.get($rootScope.pathURL + '/SeguimientoProcesos/ObtenerMetadatos')
				.success(function (response) {

					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos.Empresas = response.Empresas;
						$scope.Empresa = response.EmpresaActual;

						$scope.Metadatos.Originadores = response.Originadores;

						angular.forEach($scope.Metadatos.Originadores, function (item) {
							item.Seleccionado = false;
						});
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.ValidarEmpresaSeleccionada = function () {
			if (!$scope.Empresa || !$scope.Empresa.Codigo) {
				return false;
			}

			return true;
		}

		$scope.ObtenerOriginadores = function () {
			$scope.Metadatos.Originadores = [];
			$scope.Metadatos.Solicitudes = [];
			$scope.SolicitudesSeleccionadas = [];

			$scope.todosOriginadores = false;
			$scope.todosSolicitudes = false;

			if ($scope.ValidarEmpresaSeleccionada()) {
				$http.get($rootScope.pathURL + '/SeguimientoProcesos/ObtenerOriginadores?EmpresaCodigo=' + $scope.Empresa.Codigo)
					.success(function (response) {

						var mensajes = '';

						angular.forEach(response.validacion, function (value) {
							mensajes = mensajes + '<li>' + value + '</li>';
						});

						if (mensajes.length > 1) {
							bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						} else {
							$scope.Metadatos.Originadores = response.Originadores;
							$scope.Metadatos.Solicitudes = [];

							angular.forEach($scope.Metadatos.Originadores, function (item) {
								item.Seleccionado = false;
							});
						}
					});
			}
		}

		$scope.ValidarOriginadoresSeleccionados = function () {
			var originadiresSeleccionados = [];

			angular.forEach($scope.Metadatos.Originadores, function (item) {
				if (item.Seleccionado) {
					originadiresSeleccionados.push(item.Usuario);
				}
			});

			if (originadiresSeleccionados.length == 0) {
				return false;
			}

			return true;
		}

		$scope.ObtenerSolicitudes = function () {
			$scope.Metadatos.Solicitudes = [];
			$scope.SolicitudesSeleccionadas = [];

			$scope.todosSolicitudes = false;

			if ($scope.ValidarEmpresaSeleccionada()) {
				if ($scope.ValidarOriginadoresSeleccionados()) {
					var originadiresSeleccionados = [];

					angular.forEach($scope.Metadatos.Originadores, function (item) {
						if (item.Seleccionado) {
							originadiresSeleccionados.push(item.Usuario);
						}
					});

					$http.post($rootScope.pathURL + '/SeguimientoProcesos/ObtenerSolicitudes', {
						EmpresaCodigo: $scope.Empresa.Codigo,
						Originadores: originadiresSeleccionados,
						FechaDesde: $scope.fechaDesde,
						FechaHasta: $scope.fechaHasta
					})
						.success(function (response) {

							var mensajes = '';

							angular.forEach(response.validacion, function (value) {
								mensajes = mensajes + '<li>' + value + '</li>';
							});

							if (mensajes.length > 1) {
								bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
							} else {
								$scope.Metadatos.Solicitudes = response.Solicitudes;

								angular.forEach($scope.Metadatos.Solicitudes, function (item) {
									item.Seleccionado = false;
								});
							}
						});
				}
			}
		}

		$scope.Limpiar = function () {
			var date = new Date();
			var ultimoDia = (new Date(date.getFullYear(), date.getMonth() + 1, 0)).getDate();
			$scope.fechaDesde = '01/' + ('0' + (parseInt(date.getMonth()) + 1)).slice(-2) + '/' + date.getFullYear();
			$scope.fechaHasta = ultimoDia + '/' + ('0' + (parseInt(date.getMonth()) + 1)).slice(-2) + '/' + date.getFullYear();

			$scope.Empresa = {};
			$scope.Metadatos.Originadores = [];
			$scope.Metadatos.Solicitudes = [];
			$scope.SolicitudesSeleccionadas = [];

			$scope.todosOriginadores = false;
			$scope.todosSolicitudes = false;
		}

		$scope.toggleTodosOriginadores = function () {
			$scope.SolicitudesSeleccionadas = [];

			angular.forEach($scope.Metadatos.Originadores, function (item) {
				item.Seleccionado = $scope.todosOriginadores;
			});

			$scope.ObtenerSolicitudes();
		}

		$scope.toggleTodosSolicitudes = function () {
			angular.forEach($scope.Metadatos.Solicitudes, function (item) {
				item.Seleccionado = $scope.todosSolicitudes;
			});

			$scope.ActualizarSolicitudesSeleccionadas();
		}

		$scope.ActualizarSolicitudesSeleccionadas = function () {

			$scope.SolicitudesSeleccionadas = [];

			angular.forEach($scope.Metadatos.Solicitudes, function (item) {
				if (item.Seleccionado) {
					$scope.SolicitudesSeleccionadas.push(item);
				}
			});
		}

    }]);