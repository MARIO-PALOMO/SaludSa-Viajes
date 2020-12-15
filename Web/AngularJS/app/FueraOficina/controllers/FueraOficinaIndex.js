
/**
 * Gestiona la lógica de control de la vista index del módulo Fuera de Oficina.
 */
app_fuera_oficina.controller('FueraOficinaIndex', [
    '$scope',
    '$rootScope',
	'$http',
	'$window',
	function ($scope, $rootScope, $http, $window) {

		$scope.Metadatos = {};

		$scope.ObtenerMetadatos = function () {
			$http.get($rootScope.pathURL + '/FueraOficina/ObtenerMetadatos')
				.success(function (response) {

					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos = response;
						
						if (!$scope.Metadatos.UsuarioReasignacion) {
							$scope.SeleccionEnFueraOficina = 1;
							$scope.UsuarioReasignacion = {};
						}
						else {
							$scope.SeleccionEnFueraOficina = 2;
							$scope.UsuarioReasignacion = $scope.Metadatos.UsuarioReasignacion;
						}
					}
				});
		}

		$scope.ObtenerMetadatos();

		$scope.Validar = function () {
			var errores = '';

			if ($scope.SeleccionEnFueraOficina == 2 && (!$scope.UsuarioReasignacion || !$scope.UsuarioReasignacion.Usuario)) {
				errores = errores + '<li>Debe seleccionar un usuario al cual reasignar las tareas.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		}

		$scope.Crear = function () {
			if ($scope.Validar()) {
				$http.post($rootScope.pathURL + '/FueraOficina/Edit', {
					Proceso: $scope.SeleccionEnFueraOficina,
					Usuario: (($scope.SeleccionEnFueraOficina == 2) ? $scope.UsuarioReasignacion.Usuario : null)
				})
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
							$window.open($rootScope.pathURL + '/FueraOficina/Index?mensaje=Proceso realizado correctamente.', '_self');
						}
					});
			}
		}

    }]);