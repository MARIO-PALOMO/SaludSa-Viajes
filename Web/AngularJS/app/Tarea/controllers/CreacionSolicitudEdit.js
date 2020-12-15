
/**
 * Gestiona la lógica de control de la vista CreacionSolicitudEdit del módulo Tarea.
 */
app_tarea.controller('CreacionSolicitudEdit', [
    '$scope',
    '$rootScope',
    '$uibModal',
    '$log',
	'$http',
	'json_js_datetime',
	'$window',
	function ($scope, $rootScope, $uibModal, $log, $http, json_js_datetime, $window) {

		$scope.Accion = "Aprobar";

		$scope.Sesion = {};
		$scope.Solicitud = {};

		$scope.Metadatos = {};

		$scope.DetallesPk = 1;

		$scope.RequerimientosAdjuntosPrevisualizarPk = 1;

		$scope.ObtenerMetadatos = function () {

			$scope.SolicitudId = $('#SolicitudId').val();
			$scope.TareaId = $('#TareaId').val();
			$scope.DataAccion = $('#Accion').val();

			$http.get($rootScope.pathURL + '/SolicitudCompra/ObtenerMetadatos?SolicitudId=' + $scope.SolicitudId)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos = response;

						$scope.ObtenerDatos();
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.ObtenerDatos = function () {
			$http.get($rootScope.pathURL + '/Tarea/ObtenerDatos?SolicitudId=' + $scope.SolicitudId + '&TareaId=' + $scope.TareaId)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						$scope.Solicitud = response.cabecera;
						$scope.Tarea = response.tarea;

						$scope.Solicitud.FechaSolicitud = json_js_datetime.convertir($scope.Solicitud.FechaSolicitud);
						$scope.Solicitud.RequerimientosAdjuntos = [];
						$scope.Solicitud.MontoEstimado = parseFloat($scope.Solicitud.MontoEstimado).toFixed(2);

						angular.forEach($scope.Solicitud.Detalles, function (item) {
							item.Pk = $scope.DetallesPk++;
							item.Cantidad = item.Tipo == '0' ? parseInt(item.Cantidad) : parseFloat(item.Cantidad).toFixed(2);
						});

						$scope.Solicitud.RequerimientosAdjuntosSalvados = [];

						angular.forEach(response.adjuntos, function (item1) {

							var adjuntoTem = {};
							adjuntoTem.Id = item1.Id;

							angular.forEach(item1.Propiedades, function (item2) {
								switch (item2.Codigo) {
									case "0":
										adjuntoTem.Nombre = item2.Valor;
										break;
									case "30":
										adjuntoTem.Tamano = item2.Valor;
										break;
									case "1087":
										adjuntoTem.IdAdjunto = item2.Valor;
										break;
								}
							});

							$scope.Solicitud.RequerimientosAdjuntosSalvados.push(adjuntoTem);
						});

						if ($scope.DataAccion == 'show') {
							$scope.TareaProcesada = true;

							$scope.Accion = $scope.Tarea.Accion;
							$scope.Observacion = $scope.Tarea.Observacion;
						}

						if ($scope.Solicitud.JsonOriginal) {
							$scope.JsonOriginal = JSON.parse($scope.Solicitud.JsonOriginal);
							$scope.CargarJsonOriginal();
						}
					}
				});
		}

		$scope.CargarJsonOriginal = function () {

			$scope.Solicitud.ProveedorSugerido = $scope.JsonOriginal.ProveedorSugerido;
			$scope.Solicitud.Frecuencia = $scope.JsonOriginal.Frecuencia;
			$scope.Solicitud.MontoEstimado = parseFloat($scope.JsonOriginal.MontoEstimado).toFixed(2);
			$scope.Solicitud.ProductoMercadeo = {
				Codigo: $scope.JsonOriginal.ProductoMercadeoCodigo,
				Nombre: $scope.JsonOriginal.ProductoMercadeoNombre
			};
			$scope.Solicitud.Descripcion = $scope.JsonOriginal.Descripcion;

			$scope.Solicitud.Detalles = [];
			angular.forEach($scope.JsonOriginal.Detalles, function (item) {
				item.Cantidad = item.Tipo == '0' ? parseInt(item.Cantidad) : parseFloat(item.Cantidad).toFixed(2);

				$scope.Solicitud.Detalles.push(item);
			});

			if ($scope.JsonOriginal.AprobacionJefeArea) {
				var tem = $scope.Metadatos.JefesAreas.filter(function (item) {
					return item.Usuario == $scope.JsonOriginal.AprobacionJefeArea;
				});

				$scope.Solicitud.AprobacionJefeArea = tem[0];
			}

			if ($scope.JsonOriginal.AprobacionSubgerenteArea) {
				var tem = $scope.Metadatos.SubgerentesAreas.filter(function (item) {
					return item.Usuario == $scope.JsonOriginal.AprobacionSubgerenteArea;
				});

				$scope.Solicitud.AprobacionSubgerenteArea = tem[0];
			}

			if ($scope.JsonOriginal.AprobacionGerenteArea) {
				var tem = $scope.Metadatos.GerentesAreas.filter(function (item) {
					return item.Usuario == $scope.JsonOriginal.AprobacionGerenteArea;
				});

				$scope.Solicitud.AprobacionGerenteArea = tem[0];
			}

			if ($scope.JsonOriginal.AprobacionVicePresidenteFinanciero) {
				var tem = $scope.Metadatos.VicepresidentesFinancieros.filter(function (item) {
					return item.Usuario == $scope.JsonOriginal.AprobacionVicePresidenteFinanciero;
				});

				$scope.Solicitud.AprobacionVicePresidenteFinanciero = tem[0];
			}

			if ($scope.JsonOriginal.AprobacionGerenteGeneral) {
				var tem = $scope.Metadatos.GerentesGenerales.filter(function (item) {
					return item.Usuario == $scope.JsonOriginal.AprobacionGerenteGeneral;
				});

				$scope.Solicitud.AprobacionGerenteGeneral = tem[0];
			}
		}

		$scope.AdicionarDistribucion = function (Detalle) {
			var modalInstance = $uibModal.open({
				animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/PlantillaDistribucion/views/show.html',
				controller: 'PlantillaDistribucionShow',
				backdrop: 'static',
				size: 'lg',
				resolve: {
					data: {
						TituloPag: 'Detalle de distribución',
						Distribuciones: Detalle.Distribuciones,
						Empresa: $scope.Solicitud.EmpresaParaLaQueSeCompra
					}
				}
			});

			modalInstance.result.then(function (result) {
				
			}, function () {

			});
		}

		$scope.DescargarAdjunto = function (adjunto, Tipo) {
			$window.open($rootScope.pathURL + '/SolicitudCompra/DescargarAdjunto?SolicitudId=' + $scope.Solicitud.Id + '&AdjuntoId=' + adjunto.IdAdjunto + '&AdjuntoNombre=' + adjunto.Nombre + '&Tipo=' + Tipo, '_blank');
		}

    }]);