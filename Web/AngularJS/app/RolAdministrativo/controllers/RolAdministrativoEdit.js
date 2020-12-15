
/**
 * Gestiona la lógica de control de la vista edit del módulo Rol Administrativo.
 */
app_rol_administrativo.controller('RolAdministrativoEdit', [
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
        $scope.Rol = {};
		$scope.Metadatos = {};

        $scope.Ok = function (result) {
            $uibModalInstance.close(result);
        };

        $scope.Cerrar = function () {
            $uibModalInstance.dismiss();
		};

		$scope.ObtenerMetadatos = function () {
			$http.get($rootScope.pathURL + '/RolAdministrativo/ObtenerMetadatos')
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos = response;

						$scope.CargarDatos();
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.CargarDatos = function () {
			$http.get($rootScope.pathURL + '/RolAdministrativo/Detalle?Id=' + $scope.Data.Id)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Rol = response.rol;
					}
				});
		}

		$scope.CambiarTipoRol = function () {
			if ($scope.Rol.RolObj && $scope.Rol.RolObj.Tipo == 'A') {
				$scope.Rol.EmpresaObj = {};
				$scope.Rol.CiudadObj = {};
			}
		}

		$scope.Validar = function () {
			var errores = '';

			if (!$scope.Rol.RolObj || !$scope.Rol.RolObj.Id) {
				errores = errores + '<li>No ha seleccionado un rol.</li>';
			}

			if (!$scope.Rol.ColaboradorObj || !$scope.Rol.ColaboradorObj.Usuario) {
				errores = errores + '<li>No ha seleccionado un colaborador.</li>';
			}

			if ($scope.Rol.RolObj && $scope.Rol.RolObj.Id && $scope.Rol.RolObj.Tipo == 'P') {
				if (!$scope.Rol.EmpresaObj || !$scope.Rol.EmpresaObj.Codigo) {
					errores = errores + '<li>No ha seleccionado una empresa.</li>';
				}

				if (!$scope.Rol.CiudadObj || !$scope.Rol.CiudadObj.Id) {
					errores = errores + '<li>No ha seleccionado una ciudad.</li>';
				}
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		}

		$scope.Crear = function () {
			if ($scope.Validar()) {

				var Rol = {
					Id: $scope.Rol.Id,
					ColaboradorUsuario: $scope.Rol.ColaboradorObj.Usuario,
					ColaboradorNombreCompleto: $scope.Rol.ColaboradorObj.NombreCompleto,
					EmpresaCodigo: ($scope.Rol.EmpresaObj && $scope.Rol.RolObj.Tipo == 'P') ? ($scope.Rol.EmpresaObj.Codigo ? $scope.Rol.EmpresaObj.Codigo : null) : null,
					EmpresaNombre: ($scope.Rol.EmpresaObj && $scope.Rol.RolObj.Tipo == 'P') ? ($scope.Rol.EmpresaObj.Nombre ? $scope.Rol.EmpresaObj.Nombre : null) : null,
					RolId: $scope.Rol.RolObj.Id,
					CiudadId: ($scope.Rol.CiudadObj && $scope.Rol.RolObj.Tipo == 'P') ? ($scope.Rol.CiudadObj.Id ? $scope.Rol.CiudadObj.Id : null) : null,
					CreadoPor: $scope.Rol.CreadoPor,
					FechaCreacion: json_js_date.convertir($scope.Rol.FechaCreacion)
				};

				$http.post($rootScope.pathURL + '/RolAdministrativo/Edit', { Rol: Rol })
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
							$scope.Ok($scope.Rol);
							toastr['success']("Elemento procesado correctamente.", "Confirmación");
						}
					});
			}
		}

    }]);