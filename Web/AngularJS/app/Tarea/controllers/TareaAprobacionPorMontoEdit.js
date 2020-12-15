﻿
/**
 * Gestiona la lógica de control de la vista AprobacionPorMontoEdit del módulo Tarea.
 */
app_tarea.controller('TareaAprobacionPorMontoEdit', [
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
			$scope.Tipo = $('#Tipo').val();
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
							item.Valor = parseFloat(item.Valor).toFixed(2);
							item.Total = parseFloat(item.Total).toFixed(2);
						});

						$scope.SumarTotal();

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

						$scope.Solicitud.CotizacionesAdjuntosSalvados = [];

						angular.forEach(response.cotizacionesAdjuntos, function (item1) {

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

							$scope.Solicitud.CotizacionesAdjuntosSalvados.push(adjuntoTem);
						});

						$scope.Solicitud.EvaluacionesAdjuntosSalvados = [];

						angular.forEach(response.evaluacionesAdjuntos, function (item1) {

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

							$scope.Solicitud.EvaluacionesAdjuntosSalvados.push(adjuntoTem);
						});

						if ($scope.DataAccion == 'show') {
							$scope.TareaProcesada = true;

							$scope.Accion = $scope.Tarea.Accion;
							$scope.Observacion = $scope.Tarea.Observacion;
						}
					}
				});
		}

		$scope.AdicionarDistribucion = function (Detalle) {

			var Lineas = [];

			angular.forEach($scope.Solicitud.Detalles, function (item, index) {
				if (item.Tipo == Detalle.Tipo && item.PlantillaDistribucionDetalle) {
					var nueva = {
						Id: item.Pk,
						Descripcion: 'Detalle ' + (parseInt(index) + 1) + (item.Observacion ? (' (' + item.Observacion.toUpperCase() + ')') : ''),
						Detalles: item.PlantillaDistribucionDetalle
					};

					Lineas.push(nueva);
				}
			});

			if (Detalle.PlantillaDistribucionDetalle && Detalle.PlantillaDistribucionDetalle.length > 0) {
				var modalInstance = $uibModal.open({
					animation: true,
					templateUrl: $rootScope.pathURL + '/AngularJS/app/PlantillaDistribucion/views/form.html',
					controller: 'PlantillaDistribucionCreate',
					backdrop: 'static',
					size: 'lg',
					resolve: {
						data: {
							TituloPag: 'Detalle de distribución',
							Accion: 'detail',
							DesdeSolicitud: true,
							Detalles: Detalle.PlantillaDistribucionDetalle,
							TipoProducto: Detalle.Tipo,
							Lineas: Lineas,
							Empresa: $scope.Solicitud.EmpresaParaLaQueSeCompra
						}
					}
				});

				modalInstance.result.then(function (result) {
					Detalle.PlantillaDistribucionDetalle = result;
				}, function () {

				});
			}
			else {
				var modalInstance = $uibModal.open({
					animation: true,
					templateUrl: $rootScope.pathURL + '/AngularJS/app/PlantillaDistribucion/views/form.html',
					controller: 'PlantillaDistribucionCreate',
					backdrop: 'static',
					size: 'lg',
					resolve: {
						data: {
							TituloPag: 'Detalle de distribución',
							Accion: 'detail',
							DesdeSolicitud: true,
							TipoProducto: Detalle.Tipo,
							Lineas: Lineas,
							Empresa: $scope.Solicitud.EmpresaParaLaQueSeCompra
						}
					}
				});

				modalInstance.result.then(function (result) {
					Detalle.PlantillaDistribucionDetalle = result;
				}, function () {

				});
			}
		}

		$scope.AbrirUrl = function (detalle) {
			if (detalle.Url) {
				$window.open(detalle.Url, '_blank');
			}
		}

		$scope.SumarTotal = function () {

			var suma = 0;

			angular.forEach($scope.Solicitud.Detalles, function (item) {
				suma = (parseFloat(suma) + parseFloat(item.Total ? item.Total : 0)).toFixed(2);
			});

			$scope.TotalDetalles = suma;
		}

		$scope.DescargarAdjunto = function (adjunto, Tipo) {
			$window.open($rootScope.pathURL + '/SolicitudCompra/DescargarAdjunto?SolicitudId=' + $scope.Solicitud.Id + '&AdjuntoId=' + adjunto.IdAdjunto + '&AdjuntoNombre=' + adjunto.Nombre + '&Tipo=' + Tipo, '_blank');
		}

		$scope.Validar = function () {
			var errores = '';

			if (!$scope.Accion) {
				errores = errores + '<li>No ha seleccionado una acción.</li>';
			}

			if ($scope.Accion != 'Aprobar' && !$scope.Observacion) {
				errores = errores + '<li>No ha entrado un comentario.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		}

		$scope.Crear = function () {
			if ($scope.Validar()) {

				if ($scope.Tipo == 16) { // APROBACION_POR_MONTO_JEFE_AREA
					$http.post($rootScope.pathURL + '/Tarea/TareaAprobacionPorMontoJefeAreaEdit', { TareaId: $scope.TareaId, Accion: $scope.Accion, Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
								//$scope.Solicitud.Tareas = response.tareas;
								//$scope.TareaProcesada = true;
								//toastr['success']("Elemento procesado correctamente.", "Confirmación");
								$window.open($rootScope.pathURL + '/Tarea/Index?mensaje=Elemento procesado correctamente.', '_self');
							}
						});
				}
				else if ($scope.Tipo == 17) { // APROBACION_POR_MONTO_SUBGERENTE_AREA
					$http.post($rootScope.pathURL + '/Tarea/TareaAprobacionPorMontoSubgerenteAreaEdit', { TareaId: $scope.TareaId, Accion: $scope.Accion, Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
								//$scope.Solicitud.Tareas = response.tareas;
								//$scope.TareaProcesada = true;
								//toastr['success']("Elemento procesado correctamente.", "Confirmación");
								$window.open($rootScope.pathURL + '/Tarea/Index?mensaje=Elemento procesado correctamente.', '_self');
							}
						});
				}
				else if ($scope.Tipo == 18) { // APROBACION_POR_MONTO_GERENTE_AREA
					$http.post($rootScope.pathURL + '/Tarea/TareaAprobacionPorMontoGerenteAreaEdit', { TareaId: $scope.TareaId, Accion: $scope.Accion, Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
								//$scope.Solicitud.Tareas = response.tareas;
								//$scope.TareaProcesada = true;
								//toastr['success']("Elemento procesado correctamente.", "Confirmación");
								$window.open($rootScope.pathURL + '/Tarea/Index?mensaje=Elemento procesado correctamente.', '_self');
							}
						});
				}
				else if ($scope.Tipo == 19) { // APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO
					$http.post($rootScope.pathURL + '/Tarea/TareaAprobacionPorMontoVicepresidenteFinancieroEdit', { TareaId: $scope.TareaId, Accion: $scope.Accion, Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
								//$scope.Solicitud.Tareas = response.tareas;
								//$scope.TareaProcesada = true;
								//toastr['success']("Elemento procesado correctamente.", "Confirmación");
								$window.open($rootScope.pathURL + '/Tarea/Index?mensaje=Elemento procesado correctamente.', '_self');
							}
						});
				}
				else if ($scope.Tipo == 20) { // APROBACION_POR_MONTO_GERENTE_GENERAL
					$http.post($rootScope.pathURL + '/Tarea/TareaAprobacionPorMontoGerenteGeneralEdit', { TareaId: $scope.TareaId, Accion: $scope.Accion, Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
								//$scope.Solicitud.Tareas = response.tareas;
								//$scope.TareaProcesada = true;
								//toastr['success']("Elemento procesado correctamente.", "Confirmación");
								$window.open($rootScope.pathURL + '/Tarea/Index?mensaje=Elemento procesado correctamente.', '_self');
							}
						});
				}
			}
		}

    }]);