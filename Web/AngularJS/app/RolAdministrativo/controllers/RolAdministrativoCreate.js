
/**
 * Gestiona la lógica de control de la vista create del módulo Rol Administrativo.
 */
app_rol_administrativo.controller('RolAdministrativoCreate', [
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
        $scope.Rol = {};

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
						$scope.Rol.RolObj = {};
						$scope.Rol.ColaboradorObj = {};
						$scope.Rol.EmpresaObj = {};
						$scope.Rol.CiudadObj = {};
					}
				});
		}
		$scope.ObtenerMetadatos();

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
					Id: 0,
					ColaboradorUsuario: $scope.Rol.ColaboradorObj.Usuario,
					ColaboradorNombreCompleto: $scope.Rol.ColaboradorObj.NombreCompleto,
					EmpresaCodigo: ($scope.Rol.EmpresaObj && $scope.Rol.RolObj.Tipo == 'P') ? ($scope.Rol.EmpresaObj.Codigo ? $scope.Rol.EmpresaObj.Codigo : null) : null,
					EmpresaNombre: ($scope.Rol.EmpresaObj && $scope.Rol.RolObj.Tipo == 'P') ? ($scope.Rol.EmpresaObj.Nombre ? $scope.Rol.EmpresaObj.Nombre : null) : null,
					RolId: $scope.Rol.RolObj.Id,
					CiudadId: ($scope.Rol.CiudadObj && $scope.Rol.RolObj.Tipo == 'P') ? ($scope.Rol.CiudadObj.Id ? $scope.Rol.CiudadObj.Id : null) : null,
					CreadoPor: 'tem', // Cambiar en el backend
					FechaCreacion: new Date() // Cambiar en el backend
				};

				$http.post($rootScope.pathURL + '/RolAdministrativo/Create', { Rol: Rol })
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
							$scope.Rol.Id = response.Id;
							$scope.Ok($scope.Rol);
							toastr['success']("Elemento procesado correctamente.", "Confirmación");
						}
					});
			}
        }

    }]);