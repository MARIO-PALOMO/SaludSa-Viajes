
/**
 * Gestiona la lógica de control de la vista edit del módulo Plantilla Distribucion.
 */
app_platilla_distribucion.controller('PlantillaDistribucionEdit', [
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
        $scope.Distribucion = {};
		$scope.Distribucion.Detalles = [];
		$scope.Metadatos = {};
		$scope.Plantilla = {};

		$scope.DistribucionesPk = 1;

		$scope.TotalDetalle = '0.00';

        $scope.Ok = function (result) {
            $uibModalInstance.close(result);
        };

        $scope.Cerrar = function () {
            $uibModalInstance.dismiss();
		};

		$scope.CargarDatos = function () {
			$http.get($rootScope.pathURL + '/PlantillaDistribucion/Detalle?Id=' + $scope.Data.Id)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						$scope.Metadatos.Departamentos = [];
						$scope.Distribucion = {};
					} else {
						$scope.Metadatos.Departamentos = response.Departamentos;
						$scope.Metadatos.Empresas = response.Empresas;
						$scope.Distribucion = response.Distribucion;

						var empresaArr = $scope.Metadatos.Empresas.filter(function (item) {
							return item.Codigo == $scope.Distribucion.EmpresaCodigo;
						});

						$scope.Distribucion.Empresa = empresaArr[0];

						angular.forEach($scope.Distribucion.Detalles, function (item) {
							item.Pk = $scope.DistribucionesPk++;
							item.Porcentaje = parseFloat(item.Porcentaje).toFixed(2);
						});

						$scope.CalcularTotalDetalle();

						$scope.ObtenerPlantillasDistribucion();
					}
				});
		}
		$scope.CargarDatos();

		$scope.ObtenerPlantillasDistribucion = function () {
			$http.get($rootScope.pathURL + '/PlantillaDistribucion/ObtenerPlantillasDistribucion?CompaniaCodigo=' + $scope.Distribucion.Empresa.Codigo)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						$scope.Metadatos.Plantillas = [];
						$scope.Plantilla = {};
					} else {
						$scope.Metadatos.Plantillas = response.plantillas;
						$scope.Plantilla = {};
					}
				});
		}

        $scope.AdicionarDetalle = function () {
			$scope.Distribucion.Detalles.push({
				Id: 0,
				Pk: $scope.DistribucionesPk++,
				Departamento: {},
				CentroCosto: {},
				Proposito: {},
				Porcentaje: '0.00',
				EstadoId: 1,

				MetadatosCentrosCosto: [],
				MetadatosPropositos: []
			});

			$scope.CalcularTotalDetalle();
		}

		$scope.EliminarDetalle = function (Detalle) {
			Detalle.EstadoId = 2;

			$scope.CalcularTotalDetalle();
		}

		$scope.CambioDepartamento = function (Detalle) {
			$http.get($rootScope.pathURL + '/PlantillaDistribucion/ObtenerCentrosCosto?DepartamentoCodigo=' + Detalle.Departamento.Codigo + '&CompaniaCodigo=' + $scope.Distribucion.Empresa.Codigo)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						Detalle.MetadatosCentrosCosto = [];
						Detalle.MetadatosPropositos = [];
						Detalle.CentroCosto = {};
						Detalle.Proposito = {};
					} else {
						Detalle.MetadatosCentrosCosto = response.centrosCosto;
						Detalle.MetadatosPropositos = [];
						Detalle.CentroCosto = {};
						Detalle.Proposito = {};
					}
				});
		}

		$scope.CambioCentroCosto = function (Detalle) {
			$http.get($rootScope.pathURL + '/PlantillaDistribucion/ObtenerPropositos?DepartamentoCodigo=' + Detalle.Departamento.Codigo + '&CentroCostoCodigo=' + Detalle.CentroCosto.Codigo + '&CompaniaCodigo=' + $scope.Distribucion.Empresa.Codigo)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						Detalle.MetadatosPropositos = [];
						Detalle.Proposito = {};
					} else {
						Detalle.MetadatosPropositos = response.propositos;
						Detalle.Proposito = {};
					}
				});
		}

		$scope.CargarPlantilla = function () {
			$http.get($rootScope.pathURL + '/PlantillaDistribucion/CargarPlantilla?Id=' + $scope.Plantilla.Id + '&CompaniaCodigo=' + $scope.Distribucion.Empresa.Codigo)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						angular.forEach(response.Detalles, function (item) {
							angular.forEach($scope.Distribucion.Detalles, function (item) {
								if ((!item.Departamento || !item.Departamento.Codigo) && item.Id == 0) {
									$scope.EliminarDetalle(item);
								}
							});

							$scope.Distribucion.Detalles.push({
								Pk: $scope.DistribucionesPk++,
								Departamento: item.Departamento,
								CentroCosto: item.CentroCosto,
								Proposito: item.Proposito,
								Porcentaje: parseFloat(item.Porcentaje).toFixed(2),
								EstadoId: 1,

								MetadatosCentrosCosto: item.MetadatosCentrosCosto,
								MetadatosPropositos: item.MetadatosPropositos
							});
						});

						$scope.CalcularTotalDetalle();
					}
				});
		}

		$scope.CalcularTotalDetalle = function () {
			$scope.TotalDetalle = '0.00';

			angular.forEach($scope.Distribucion.Detalles, function (item) {
				if (item.EstadoId != 2) {
					var tem = item.Porcentaje ? item.Porcentaje : 0;
					$scope.TotalDetalle = (parseFloat($scope.TotalDetalle) + parseFloat(tem)).toFixed(2);
				}
			});
		}

		$scope.Validar = function () {
			var errores = '';

			if (!$scope.Distribucion.Descripcion) {
				errores = errores + '<li>No ha entrado una descripción.</li>';
			}

			var ErrorDepartamento = false;
			var ErrorCentroCosto = false;
			var ErrorProposito = false;
			var ErrorPorcentaje = false;

			if ($scope.Distribucion.Detalles.length == 0) {
				errores += '<li>No ha entrado ninguna distribución.</li>';
			}
			else {
				angular.forEach($scope.Distribucion.Detalles, function (item) {
					if (item.EstadoId == 1) {
						if (!item.Departamento || !item.Departamento.Codigo) {
							ErrorDepartamento = true;
						}

						if (!item.CentroCosto || !item.CentroCosto.Codigo) {
							ErrorCentroCosto = true;
						}

						if (!item.Proposito || !item.Proposito.Codigo) {
							ErrorProposito = true;
						}

						if (!item.Porcentaje || parseFloat(item.Porcentaje) == 0) {
							ErrorPorcentaje = true;
						}
					}
				});

				if (ErrorDepartamento) {
					errores += '<li>Existen errores en el campo "Departamento" de las distribuciones.</li>';
				}

				if (ErrorCentroCosto) {
					errores += '<li>Existen errores en el campo "Centro de costo" de las distribuciones.</li>';
				}

				if (ErrorProposito) {
					errores += '<li>Existen errores en el campo "Propósito" de las distribuciones.</li>';
				}

				if (ErrorPorcentaje) {
					errores += '<li>Existen errores en el campo "Porcentaje" de las distribuciones.</li>';
				}

				if (parseFloat($scope.TotalDetalle) > 100 || parseFloat($scope.TotalDetalle) < 100) {
					errores += '<li>La suma de los procentajes es distinta de 100.</li>';
				}
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		}

        $scope.Crear = function () {
			if ($scope.Data.Accion != 'detail' && $scope.Validar()) {

				var Detalles = [];

				angular.forEach($scope.Distribucion.Detalles, function (item) {
					Detalles.push({
						Id: item.Id,
						Porcentaje: parseFloat(item.Porcentaje),
						DepartamentoCodigo: item.Departamento.Codigo,
						DepartamentoDescripcion: item.Departamento.Descripcion,
						DepartamentoCodigoDescripcion: item.Departamento.CodigoDescripcion,
						CentroCostoCodigo: item.CentroCosto.Codigo,
						CentroCostoDescripcion: item.CentroCosto.Descripcion,
						CentroCostoCodigoDescripcion: item.CentroCosto.CodigoDescripcion,
						PropositoCodigo: item.Proposito.Codigo,
						PropositoDescripcion: item.Proposito.Descripcion,
						PropositoCodigoDescripcion: item.Proposito.CodigoDescripcion,
						EstadoId: item.EstadoId,
						DistribucionCabeceraId: $scope.Distribucion.Id
					});
				});

				var Cabecera = {
					Id: $scope.Distribucion.Id,
					Descripcion: $scope.Distribucion.Descripcion.toUpperCase(),
					EstadoId: $scope.Distribucion.EstadoId,
					UsuarioPropietario: $scope.Distribucion.UsuarioPropietario,
					DescripcionDepartamentoPropietario: $scope.Distribucion.DescripcionDepartamentoPropietario,
					EmpresaCodigo: $scope.Distribucion.EmpresaCodigo
				};

				$http.post($rootScope.pathURL + '/PlantillaDistribucion/Edit', { Cabecera: Cabecera, Detalles: Detalles })
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
							$scope.Ok($scope.Distribucion);
							toastr['success']("Elemento procesado correctamente.", "Confirmación");
						}
					});
			}
        }

    }]);