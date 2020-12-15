
/**
 * Gestiona la lógica de control de la vista index para el módulo de Anular Recepción.
 */
app_anulacion_recepcion.controller('AnulacionRecepcionIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
	'$http',
	'json_js_datetime',
	'json_js_date',
	'$window',
	function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http, json_js_datetime, json_js_date, $window) {

		$scope.RecepcionFiltro = {};
		$scope.Recepciones = [];
		$scope.Recepcion = {};
		$scope.Solicitud = {};
		$scope.mostrarMensajeNoExisten = false;

		$scope.BuscarRecepciones = function () {
			if (!$scope.NumeroSolicitud) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> No ha entrado un número de solicitud.');
			}
			else {
				$scope.RecepcionFiltro = {};
				$scope.Recepciones = [];
				$scope.Recepcion = {};
				$scope.Solicitud = {};
				$http.get($rootScope.pathURL + '/AnulacionRecepcion/BuscarRecepciones?NumeroSolicitud=' + $scope.NumeroSolicitud)
					.success(function (response) {
						var mensajes = '';

						angular.forEach(response.validacion, function (value) {
							mensajes = mensajes + '<li>' + value + '</li>';
						});

						if (mensajes.length > 1) {
							bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						}
						else {
							$scope.Recepciones = response.recepciones;
							$scope.Solicitud = response.solicitud;
							$scope.RecepcionFiltro = {};

							if (!$scope.Recepciones || $scope.Recepciones.length == 0) {
								$scope.mostrarMensajeNoExisten = true;
							}
							else {
								$scope.mostrarMensajeNoExisten = false;
							}
						}
					});
			}
		}

		$scope.CargarRecepcion = function () {
			$http.get($rootScope.pathURL + '/AnulacionRecepcion/CargarRecepcion?Id=' + $scope.RecepcionFiltro.Id)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						$scope.Recepcion = response.recepcion;
						$scope.Recepcion.FechaRecepcion = json_js_date.convertir($scope.Recepcion.FechaRecepcion);
						$scope.Recepcion.NumeroRecepcion = $scope.Solicitud.NumeroSolicitud + '-' + $scope.Recepcion.NumeroRecepcion;

						angular.forEach($scope.Recepcion.RecepcionLineas, function (item1) {
							angular.forEach($scope.Solicitud.Detalles, function (item2) {
								if (item1.SolicitudCompraDetalleId == item2.Id) {
									item1.CantidadRecepcion = parseFloat(item1.Cantidad).toFixed(2);
									item1.ValorRecepcion = parseFloat(item1.Valor).toFixed(2);
									item1.Observacion = item2.Observacion;
									item1.Impuesto = item2.Impuesto;
									item1.Cantidad = item2.Tipo == '0' ? parseInt(item2.Cantidad) : parseFloat(item2.Cantidad).toFixed(2);
									item1.Valor = parseFloat(item2.Valor).toFixed(2);
									item1.Total = parseFloat(item2.Total).toFixed(2);
									item1.Saldo = parseFloat(item2.Saldo).toFixed(2);
									item1.Tipo = item2.Tipo;
									item1.ProductoBien = item2.ProductoBien;
									item1.ProductoServicio = item2.ProductoServicio
								}
							});
						});	

						$scope.SumarTotalCantidadRecepcion();
						$scope.SumarTotalValorRecepcion();
					}
				});
		}

		$scope.ObtenerDatos = function () {
			if (!$scope.RecepcionFiltro || !$scope.RecepcionFiltro.Id) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> No ha seleccionado una recepción.');
			}
			else {
				$http.get($rootScope.pathURL + '/AnulacionRecepcion/ObtenerDatos?SolicitudId=' + $scope.Solicitud.Id + '&OrdenMadreId=' + $scope.RecepcionFiltro.OrdenMadreId)
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

							$scope.Solicitud.FechaSolicitud = json_js_datetime.convertir($scope.Solicitud.FechaSolicitud);
							$scope.Solicitud.RequerimientosAdjuntos = [];
							$scope.Solicitud.MontoEstimado = parseFloat($scope.Solicitud.MontoEstimado).toFixed(2);

							angular.forEach($scope.Solicitud.Detalles, function (item) {
								item.Pk = $scope.DetallesPk++;
								item.Cantidad = item.Tipo == '0' ? parseInt(item.Cantidad) : parseFloat(item.Cantidad).toFixed(2);
								item.Valor = parseFloat(item.Valor).toFixed(2);
								item.Total = parseFloat(item.Total).toFixed(2);
								item.Saldo = parseFloat(item.Saldo).toFixed(2);
								item.CantidadRecepcion = '0.00';
								item.ValorRecepcion = '0.00';
							});

							$scope.CargarRecepcion();
						}
					});
			}
		}

		$scope.SumarTotalCantidadRecepcion = function () {

			var suma = 0;

			angular.forEach($scope.Recepcion.RecepcionLineas, function (item) {
				suma = (parseFloat(suma) + parseFloat(item.CantidadRecepcion ? item.CantidadRecepcion : 0)).toFixed(2);
			});

			$scope.TotalCantidadRecepcion = suma;
		}

		$scope.SumarTotalValorRecepcion = function () {

			var suma = 0;

			angular.forEach($scope.Recepcion.RecepcionLineas, function (item) {
				suma = (parseFloat(suma) + parseFloat(item.ValorRecepcion ? item.ValorRecepcion : 0)).toFixed(2);
			});

			$scope.TotalValorRecepcion = suma;
		}

		$scope.AdicionarDistribucionRecepcion = function (Detalle) {

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
							Empresa: $scope.Solicitud.EmpresaParaLaQueSeCompra
							//Lineas
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
							Empresa: $scope.Solicitud.EmpresaParaLaQueSeCompra
							//Lineas
						}
					}
				});

				modalInstance.result.then(function (result) {
					Detalle.PlantillaDistribucionDetalle = result;
				}, function () {

				});
			}
		}

		$scope.Cancelar = function () {
			$scope.RecepcionFiltro = {};
			$scope.Recepciones = [];
			$scope.Recepcion = {};
			$scope.Solicitud = {};
			$scope.NumeroSolicitud = '';
		}

		$scope.ValidarFiltros = function () {
			var errores = '';

			if (!$scope.Observacion) {
				errores = errores + '<li>No ha entrado un motivo.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		}

		$scope.Crear = function () {
			if ($scope.ValidarFiltros()) {
				$http.post($rootScope.pathURL + '/AnulacionRecepcion/AnularRecepcion', { Id: $scope.RecepcionFiltro.Id, Motivo: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null) })
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
							$scope.RecepcionFiltro = {};
							$scope.Recepciones = [];
							$scope.Recepcion = {};
							$scope.Solicitud = {};
							$scope.NumeroSolicitud = '';
							toastr['success']("Elemento procesado correctamente.", "Confirmación");
						}
					});
			}
		}

    }]);