﻿
/**
 * Gestiona la lógica de control de la vista edit del módulo Rol Gestor Compra.
 */
app_rol_gestor_compra.controller('RolGestorCompraEdit', [
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
			$http.get($rootScope.pathURL + '/RolGestorCompra/ObtenerMetadatos')
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

						$scope.CargarDatos();
					}
				});
		}

		$scope.ObtenerMetadatos();

		$scope.CargarDatos = function () {
			$http.get($rootScope.pathURL + '/RolGestorCompra/Detalle?Id=' + $scope.Data.Id)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Rol.Empresa = response.rol.Empresa;

						$http.get($rootScope.pathURL + '/RolGestorCompra/ObtenerTiposCompra?CodigoCompania=' + $scope.Rol.Empresa.Codigo)
							.success(function (response2) {
								var mensajes = '';

								angular.forEach(response2.validacion, function (value) {
									mensajes = mensajes + '<li>' + value + '</li>';
								});

								if (mensajes.length > 1) {
									bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
									$scope.Metadatos.TiposCompra = [];
									$scope.Rol = {};
								}
								else {
									$scope.Metadatos.TiposCompra = response2.TiposCompra;
									$scope.Rol = response.rol;
								}
							});
					}
				});
		}
		$scope.CargarDatos();

		$scope.ObtenerTiposCompra = function () {
			$http.get($rootScope.pathURL + '/RolGestorCompra/ObtenerTiposCompra?CodigoCompania=' + $scope.Rol.Empresa.Codigo)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						$scope.Metadatos.TiposCompra = [];
						$scope.Rol.TipoCompra = {};
					}
					else {
						$scope.Metadatos.TiposCompra = response.TiposCompra;
						$scope.Rol.TipoCompra = {};
					}
				});
		}

		$scope.Validar = function () {
			var errores = '';

			if (!$scope.Rol.Empresa || !$scope.Rol.Empresa.Codigo) {
				errores = errores + '<li>No ha seleccionado una empresa.</li>';
			}

			if (!$scope.Rol.TipoCompra || !$scope.Rol.TipoCompra.Codigo) {
				errores = errores + '<li>No ha seleccionado un tipo de compra.</li>';
			}

			if (!$scope.Rol.GestorSierra || !$scope.Rol.GestorSierra.Usuario) {
				errores = errores + '<li>No ha seleccionado un gestor para la sierra.</li>';
			}

			if (!$scope.Rol.GestorCosta || !$scope.Rol.GestorCosta.Usuario) {
				errores = errores + '<li>No ha seleccionado un gestor para la costa.</li>';
			}

			if (!$scope.Rol.GestorAustro || !$scope.Rol.GestorAustro.Usuario) {
				errores = errores + '<li>No ha seleccionado un gestor para austro.</li>';
			}

			if (!$scope.Rol.Estado || !$scope.Rol.Estado.Id) {
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

					var Rol = {
						Id: $scope.Rol.Id,
						CodigoEmpresa: $scope.Rol.Empresa.Codigo,
						NombreEmpresa: $scope.Rol.Empresa.Nombre,
						CodigoTipoCompra: $scope.Rol.TipoCompra.Codigo,
						NombreTipoCompra: $scope.Rol.TipoCompra.Nombre,
						UsuarioGestorSierra: $scope.Rol.GestorSierra.Usuario,
						NombreGestorSierra: $scope.Rol.GestorSierra.NombreCompleto,
						UsuarioGestorCosta: $scope.Rol.GestorCosta.Usuario,
						NombreGestorCosta: $scope.Rol.GestorCosta.NombreCompleto,
						UsuarioGestorAustro: $scope.Rol.GestorAustro.Usuario,
						NombreGestorAustro: $scope.Rol.GestorAustro.NombreCompleto,
						EstadoId: $scope.Rol.Estado.Id
					};

					$http.post($rootScope.pathURL + '/RolGestorCompra/Edit', { Rol: Rol })
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