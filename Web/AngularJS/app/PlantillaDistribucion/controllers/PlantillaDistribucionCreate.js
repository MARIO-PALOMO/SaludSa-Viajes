
/**
 * Gestiona la lógica de control de la vista create del módulo Plantilla Distribucion.
 */
app_platilla_distribucion.controller('PlantillaDistribucionCreate', [
    '$scope',
    '$rootScope',
    '$uibModal',
    '$uibModalInstance',
    '$log',
    '$http',
    'data',
    function ($scope, $rootScope, $uibModal, $uibModalInstance, $log, $http, data) {

		$scope.Sesion = {};
		$scope.Metadatos = {};
        $scope.Data = data;
        $scope.Distribucion = {};
		$scope.Distribucion.Detalles = [];
		$scope.Metadatos = {};
		$scope.Plantilla = {};
		$scope.Linea = {};

		$scope.DistribucionesPk = 1;

		$scope.TotalDetalle = '0.00';

        $scope.Ok = function (result) {
            $uibModalInstance.close(result);
        };

        $scope.Cerrar = function () {
            $uibModalInstance.dismiss();
		};

		$http.get($rootScope.pathURL + '/Sesion/ObtenerSesion')
			.success(function (response) {
				$scope.Sesion = response;

				$scope.ObtenerMetadatos();
			});

		$scope.ObtenerMetadatos = function () {

			var emp = "-1";

			if ($scope.Data.DesdeSolicitud) {
				emp = $scope.Data.Empresa.Codigo;
			}

			$http.get($rootScope.pathURL + '/PlantillaDistribucion/ObtenerMetadatos?PlantillaId=-1&EmpresaCodigo=' + emp)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos = response;
						$scope.Distribucion.Empresa = response.EmpresaActiva;

						$scope.Plantilla = {};
						
						if ($scope.Data.DesdeSolicitud && $scope.Data.Detalles) { // VIENE DESDE LA SOLICITUD Y TIENE DETALLES
							
							$scope.Distribucion.Empresa = $scope.Data.Empresa;

							angular.forEach($scope.Data.Detalles, function (item) {
								$scope.Distribucion.Detalles.push({
									Pk: $scope.DistribucionesPk++,
									Departamento: {
										Codigo: item.DepartamentoCodigo,
										Descripcion: item.DepartamentoDescripcion,
										CodigoDescripcion: item.DepartamentoCodigoDescripcion
									},
									CentroCosto: {
										Codigo: item.CentroCostoCodigo,
										Descripcion: item.CentroCostoDescripcion,
										CodigoDescripcion: item.CentroCostoCodigoDescripcion
									},
									Proposito: {
										Codigo: item.PropositoCodigo,
										Descripcion: item.PropositoDescripcion,
										CodigoDescripcion: item.PropositoCodigoDescripcion
									},
									Porcentaje: parseFloat(item.Porcentaje).toFixed(2),
									EstadoId: item.EstadoId,

									MetadatosCentrosCosto: item.MetadatosCentrosCosto,
									MetadatosPropositos: item.MetadatosPropositos
								});
							});

							$scope.CalcularTotalDetalle();
						}
						else if ($scope.Data.DesdeSolicitud) { // VIENE DE LA SOLICITUD SIN DETALLES
							$scope.Distribucion.Empresa = $scope.Data.Empresa;
							
							$scope.Distribucion.Detalles = [];
							$scope.DistribucionesPk = 1;
							$scope.AdicionarDetalle();
						}
						else { // NO VIENE DE SOLICITUD
							$scope.AdicionarDetalle();
						}
					}
				});
		}

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

		$scope.ObtenerDepartamentos = function () {
			$http.get($rootScope.pathURL + '/PlantillaDistribucion/ObtenerDepartamentos?CompaniaCodigo=' + $scope.Distribucion.Empresa.Codigo)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						$scope.Metadatos.Departamentos = [];
					} else {
						$scope.Metadatos.Departamentos = response.departamentos;

						if (($scope.Data.DesdeSolicitud && (!$scope.Data.Detalles || $scope.Data.Detalles.length == 0)) || !$scope.Data.DesdeSolicitud) {
							$scope.AdicionarDetalle();
						}
					}
				});
		}

		$scope.CambiarEmpresa = function () {
			$scope.Distribucion.Detalles = [];
			$scope.DistribucionesPk = 1;

			$scope.ObtenerPlantillasDistribucion();
			$scope.ObtenerDepartamentos();
		}

        $scope.AdicionarDetalle = function () {
			$scope.Distribucion.Detalles.push({
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
			$scope.Distribucion.Detalles = $scope.Distribucion.Detalles.filter(function (item) {
				return item.Pk != Detalle.Pk;
			});

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
						angular.forEach($scope.Distribucion.Detalles, function (item) {
							if (!item.Departamento || !item.Departamento.Codigo) {
								$scope.EliminarDetalle(item);
							}
						});

						angular.forEach(response.Detalles, function (item) {
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

		$scope.CopiarDesdeDetalle = function () {
			if ($scope.Linea.Detalles) {

				if ($scope.Data.TipoProducto == '0') {
					$scope.Distribucion.Detalles = [];
				}
				else {
					angular.forEach($scope.Distribucion.Detalles, function (item) {
						if (!item.Departamento || !item.Departamento.Codigo) {
							$scope.EliminarDetalle(item);
						}
					});
				}

				angular.forEach($scope.Linea.Detalles, function (item) {
					$scope.Distribucion.Detalles.push({
						Pk: $scope.DistribucionesPk++,
						Departamento: {
							Codigo: item.DepartamentoCodigo,
							Descripcion: item.DepartamentoDescripcion,
							CodigoDescripcion: item.DepartamentoCodigoDescripcion
						},
						CentroCosto: {
							Codigo: item.CentroCostoCodigo,
							Descripcion: item.CentroCostoDescripcion,
							CodigoDescripcion: item.CentroCostoCodigoDescripcion
						},
						Proposito: {
							Codigo: item.PropositoCodigo,
							Descripcion: item.PropositoDescripcion,
							CodigoDescripcion: item.PropositoCodigoDescripcion
						},
						Porcentaje: parseFloat(item.Porcentaje).toFixed(2),
						EstadoId: item.EstadoId,

						MetadatosCentrosCosto: item.MetadatosCentrosCosto,
						MetadatosPropositos: item.MetadatosPropositos
					});
				});

				$scope.CalcularTotalDetalle();
			}
		}

		$scope.CalcularTotalDetalle = function () {
			$scope.TotalDetalle = '0.00';

			angular.forEach($scope.Distribucion.Detalles, function (item) {
				var tem = item.Porcentaje ? item.Porcentaje : 0;
				$scope.TotalDetalle = (parseFloat($scope.TotalDetalle) + parseFloat(tem)).toFixed(2);
			});
		}

		$scope.Validar = function () {
			var errores = '';

			if (!$scope.Distribucion.Descripcion && !$scope.Data.DesdeSolicitud) {
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
			if ($scope.Validar()) {

				var Detalles = [];

				angular.forEach($scope.Distribucion.Detalles, function (item) {
					var nuevo = {
						Id: 0,
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
						EstadoId: 1
					};

					if ($scope.Data.DesdeSolicitud) {
						nuevo.MetadatosCentrosCosto = item.MetadatosCentrosCosto;
						nuevo.MetadatosPropositos = item.MetadatosPropositos;
					}

					Detalles.push(nuevo);
				});

				if ($scope.Data.DesdeSolicitud) {
					$scope.Ok(Detalles);
					return;
				}

				var Cabecera = {
					Id: 0,
					Descripcion: $scope.Distribucion.Descripcion.toUpperCase(),
					Detalles: Detalles,
					EstadoId: 1,
					UsuarioPropietario: $scope.Sesion.usuario.Usuario,
					DescripcionDepartamentoPropietario: $scope.Sesion.usuario.Departamento,
					EmpresaCodigo: $scope.Distribucion.Empresa.Codigo
				};

				$http.post($rootScope.pathURL + '/PlantillaDistribucion/Create', { Cabecera: Cabecera })
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
							$scope.Distribucion.Id = response.Id;
							$scope.Ok($scope.Distribucion);
							toastr['success']("Elemento procesado correctamente.", "Confirmación");
						}
					});
			}
        }

    }]);