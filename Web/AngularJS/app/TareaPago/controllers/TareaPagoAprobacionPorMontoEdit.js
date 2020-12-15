
/**
 * Gestiona la lógica de control de la vista AprobacionPorMontoEdit del módulo Tarea Pago.
 */
app_tarea_pago.controller('TareaPagoAprobacionPorMontoEdit', [
    '$scope',
    '$rootScope',
    '$uibModal',
    '$log',
	'$http',
	'json_js_datetime',
	'json_js_date',
	'$window',
	function ($scope, $rootScope, $uibModal, $log, $http, json_js_datetime, json_js_date, $window) {

		$scope.Sesion = {};
		$scope.Solicitud = {};

		$scope.Metadatos = {};
		$scope.Data = {};
		$scope.Accion = "Aprobar";

		$scope.ObtenerMetadatos = function () {

			$scope.SolicitudId = $('#SolicitudId').val();
			$scope.TareaId = $('#TareaId').val();
			$scope.Tipo = $('#Tipo').val();
			$scope.DataAccion = $('#Accion').val();

			$http.get($rootScope.pathURL + '/SolicitudPago/ObtenerMetadatos')
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos = response;

						$scope.ObtenerSolicitud();
						$scope.ObtenerTarea();
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.ObtenerSolicitud = function () {
			$http.get($rootScope.pathURL + '/SolicitudPago/Detalle?Id=' + $scope.SolicitudId + '&TareaId=' + $scope.TareaId)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						$scope.Data.Accion = $('#Accion').val();
						$scope.Solicitud = response.cabecera;

						$scope.Solicitud.FechaSolicitud = json_js_datetime.convertir($scope.Solicitud.FechaSolicitud);

						angular.forEach($scope.Solicitud.Facturas, function (item) {
							item.Pk = $scope.secFacturas++;
							item.FechaEmision = json_js_date.convertir(item.FechaEmision);
							item.FechaVencimiento = json_js_date.convertir(item.FechaVencimiento);
						});

						$scope.CalcularMontoTotal();
					}
				});
		}

		$scope.CalcularMontoTotal = function () {
			$scope.Solicitud.MontoTotal = 0;

			angular.forEach($scope.Solicitud.Facturas, function (item) {
				$scope.Solicitud.MontoTotal = parseFloat($scope.Solicitud.MontoTotal) + parseFloat(item.Total);
			});

			$scope.Solicitud.MontoTotal = parseFloat($scope.Solicitud.MontoTotal).toFixed(2);
		}

		$scope.ObtenerTarea = function () {
			$http.get($rootScope.pathURL + '/TareaPago/ObtenerTarea?TareaId=' + $scope.TareaId)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						$scope.Tarea = response.tarea;

						if ($scope.DataAccion == 'show') {
							$scope.TareaProcesada = true;

							$scope.Accion = $scope.Tarea.Accion;
							$scope.Observacion = $scope.Tarea.Observacion;
						}
					}
				});
		}

		$scope.MostrarFactura = function (Factura) {
			var modalInstance = $uibModal.open({
				animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/SolicitudPago/views/factura_fisica.html',
				controller: 'SolicitudPagoFacturaFisica',
				backdrop: 'static',
				size: 'lg',
				resolve: {
					data: {
						TituloPag: Factura.Tipo == 'Física' ? 'Factura física' : 'Factura electrónica',
						EmpresaParaLaQueSeCompra: $scope.Solicitud.EmpresaParaLaQueSeCompra,
						TipoFactura: Factura.Tipo,
						Factura: Factura,
						EsReembolso: $scope.Solicitud.Facturas[0] ? ($scope.Solicitud.Facturas[0].TipoPagoObj.EsReembolso) : null,
						Accion: 'detail',
						SolicitudId: $scope.Solicitud.Id
					}
				}
			});

			modalInstance.result.then(function (result) {

			}, function () {

			});
		}

		$scope.DescargarAdjuntoFactura = function (Factura) {
			if (Factura.Id && parseInt(Factura.Id) > 0) {
				if (Factura.Tipo == 'Física') {
					$window.open($rootScope.pathURL + '/SolicitudPago/DescargarAdjunto?SolicitudId=' + $scope.Solicitud.Id + '&FacturaId=' + Factura.Id + '&NoFactura=' + Factura.NoFactura, '_blank');
				}
				else {
					$window.open($scope.Metadatos.UrlVisorRidePdf + Factura.FacturaElectronica.claveAcceso, '_blank');
				}
			}
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

				if ($scope.Tipo == 3) { // APROBACION_POR_MONTO_JEFE_AREA
					$http.post($rootScope.pathURL + '/TareaPago/TareaAprobacionJefeInmediatoEdit', { TareaId: $scope.TareaId, Accion: $scope.Accion, Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
								$window.open($rootScope.pathURL + '/TareaPago/Index?mensaje=Elemento procesado correctamente.', '_self');
							}
						});
				}
				else if ($scope.Tipo == 4) { // APROBACION_POR_MONTO_SUBGERENTE_AREA
					$http.post($rootScope.pathURL + '/TareaPago/TareaAprobacionPorMontoSubgerenteAreaEdit', { TareaId: $scope.TareaId, Accion: $scope.Accion, Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
								$window.open($rootScope.pathURL + '/TareaPago/Index?mensaje=Elemento procesado correctamente.', '_self');
							}
						});
				}
				else if ($scope.Tipo == 5) { // APROBACION_POR_MONTO_GERENTE_AREA
					$http.post($rootScope.pathURL + '/TareaPago/TareaAprobacionPorMontoGerenteAreaEdit', { TareaId: $scope.TareaId, Accion: $scope.Accion, Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
								$window.open($rootScope.pathURL + '/TareaPago/Index?mensaje=Elemento procesado correctamente.', '_self');
							}
						});
				}
				else if ($scope.Tipo == 6) { // APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO
					$http.post($rootScope.pathURL + '/TareaPago/TareaAprobacionPorMontoVicepresidenteFinancieroEdit', { TareaId: $scope.TareaId, Accion: $scope.Accion, Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
								$window.open($rootScope.pathURL + '/TareaPago/Index?mensaje=Elemento procesado correctamente.', '_self');
							}
						});
				}
				else if ($scope.Tipo == 7) { // APROBACION_POR_MONTO_GERENTE_GENERAL
					$http.post($rootScope.pathURL + '/TareaPago/TareaAprobacionPorMontoGerenteGeneralEdit', { TareaId: $scope.TareaId, Accion: $scope.Accion, Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
								$window.open($rootScope.pathURL + '/TareaPago/Index?mensaje=Elemento procesado correctamente.', '_self');
							}
						});
				}
			}
		}

    }]);