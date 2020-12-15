
/**
 * Gestiona la lógica de control de la vista RecepcionEdit del módulo Tarea.
 */
app_tarea.controller('TareaRecepcionEdit', [
    '$scope',
    '$rootScope',
    '$uibModal',
    '$log',
	'$http',
	'json_js_datetime',
	'json_js_date',
	'$window',
	'Persistencia',
	function ($scope, $rootScope, $uibModal, $log, $http, json_js_datetime, json_js_date, $window, Persistencia) {

		$scope.Accion = "Aprobar";

		$scope.Sesion = {};
		$scope.Solicitud = {};
		$scope.Recepcion = {};
		$scope.Recepcion.ComprobantesElectronicos = [];

		$scope.Metadatos = {};

		$scope.DetallesPk = 1;

		$scope.RequerimientosAdjuntosPrevisualizarPk = 1;

		$scope.ComprobantesElectronicosPk = 1;

		$scope.TotalCantidadRecepcion = '0.00';
		$scope.TotalValorRecepcion = '0.00';

		$scope.ObtenerMetadatos = function () {
			$scope.SolicitudId = $('#SolicitudId').val();
			$scope.TareaId = $('#TareaId').val();
			$scope.Tipo = $('#Tipo').val();
			$scope.DataAccion = $('#Accion').val();

			$http.get($rootScope.pathURL + '/Tarea/ObtenerMetadatosRecepcion?SolicitudId=' + $scope.SolicitudId + '&TareaId=' + $scope.TareaId)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos = response;
						$scope.AprobadorDesembolso = {};

						angular.forEach($scope.Metadatos.HistorialRecepciones, function (item1) {
							angular.forEach(item1.RecepcionLineas, function (item2) {
								item2.Cantidad = parseFloat(item2.Cantidad).toFixed(2);
								item2.Valor = parseFloat(item2.Valor).toFixed(2);
								item2.PorcentajeImpuestoVigente = parseFloat(item2.PorcentajeImpuestoVigente).toFixed(2);
								item2.Saldo = parseFloat(item2.Saldo).toFixed(2);
								item2.ValorUnitario = parseFloat(item2.ValorUnitario).toFixed(2);
							});
						});

						$scope.ObtenerDatos();
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.ObtenerDatos = function () {
			$http.get($rootScope.pathURL + '/Tarea/ObtenerDatosRecepcion?SolicitudId=' + $scope.SolicitudId + '&TareaId=' + $scope.TareaId)
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
							item.Saldo = parseFloat(item.Saldo).toFixed(2);
							item.SaldoOriginal = parseFloat(item.Saldo).toFixed(2);
							item.CantidadRecepcion = '0.00';
							item.ValorRecepcion = '0.00';

							item.PlantillaDistribucionDetalleRecepcion = angular.copy(item.PlantillaDistribucionDetalle);
						});

						$scope.Recepcion.NumeroRecepcion = $scope.Solicitud.NumeroSolicitud + '-' + $scope.Metadatos.NumeroRecepcion;
						$scope.Recepcion.FechaRecepcion = json_js_date.convertir($scope.Metadatos.FechaRecepcion);

						$scope.SumarTotal();
						$scope.SumarTotalSaldo();

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

						$scope.Solicitud.FacturasAdjuntos = [];
						$scope.Solicitud.FacturasAdjuntosSalvados = [];

						angular.forEach(response.facturasAdjuntos, function (item1) {

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
									case "1088":
										adjuntoTem.RecepcionId = item2.Valor;
										break;
								}
							});

							$scope.Solicitud.FacturasAdjuntosSalvados.push(adjuntoTem);
						});

						$scope.ObtenerFacturasFisicas();

						if ($scope.DataAccion == 'show') {
							$scope.TareaProcesada = true;

							if ($scope.Tarea.UsuarioAprobadorDesembolso) {
								var tem = $scope.Metadatos.JefesAreas.filter(function (item) {
									return item.Usuario == $scope.Tarea.UsuarioAprobadorDesembolso;
								});

								$scope.AprobadorDesembolso = tem[0];
							}

							$scope.ObtenerRecepcion();
						}
					}
				});
		}

		$scope.ObtenerRecepcion = function () {
			if ($scope.Tarea.RecepcionId) {
				$http.get($rootScope.pathURL + '/Tarea/ObtenerRecepcion?RecepcionId=' + $scope.Tarea.RecepcionId)
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
										item1.ProductoServicio = item2.ProductoServicio;
									}
								});
							});

							$scope.Solicitud.FacturasAdjuntos = [];
							$scope.Solicitud.FacturasAdjuntosSalvados = [];

							angular.forEach(response.facturasAdjuntos, function (item1) {

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
										case "1088":
											adjuntoTem.RecepcionId = item2.Valor;
											break;
									}
								});

								$scope.Solicitud.FacturasAdjuntosSalvados.push(adjuntoTem);
							});

							$scope.SumarTotal();
							$scope.SumarTotalSaldo();
							$scope.SumarTotalCantidadRecepcion();
							$scope.SumarTotalValorRecepcion();
						}
					});
			}
		}

		$scope.ObtenerFacturasFisicas = function () {
			$http.get($rootScope.pathURL + '/Tarea/ObtenerFacturasFisicas?RecepcionId=' + $scope.Tarea.NumeroOrdenMadre + '-' + $scope.Recepcion.NumeroRecepcion)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						$scope.Solicitud.FacturasAdjuntosSalvados = [];
						$scope.Solicitud.FacturasAdjuntos = [];
						$scope.Solicitud.FacturasAdjuntosPrevisualizar = [];

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
									case "1088":
										adjuntoTem.RecepcionId = item2.Valor;
										break;
								}
							});

							$scope.Solicitud.FacturasAdjuntosSalvados.push(adjuntoTem);
						});
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

		$scope.AdicionarDistribucionRecepcion = function (Detalle) {

			var Lineas = [];

			angular.forEach($scope.Solicitud.Detalles, function (item, index) {
				if (item.Tipo == Detalle.Tipo && item.PlantillaDistribucionDetalleRecepcion) {
					var nueva = {
						Id: item.Pk,
						Descripcion: 'Detalle ' + (parseInt(index) + 1) + (item.Observacion ? (' (' + item.Observacion.toUpperCase() + ')') : ''),
						Detalles: item.PlantillaDistribucionDetalleRecepcion
					};

					Lineas.push(nueva);
				}
			});

			if (Detalle.PlantillaDistribucionDetalleRecepcion && Detalle.PlantillaDistribucionDetalleRecepcion.length > 0) {
				var modalInstance = $uibModal.open({
					animation: true,
					templateUrl: $rootScope.pathURL + '/AngularJS/app/PlantillaDistribucion/views/form.html',
					controller: 'PlantillaDistribucionCreate',
					backdrop: 'static',
					size: 'lg',
					resolve: {
						data: {
							TituloPag: 'Detalle de distribución',
							Accion: $scope.TareaProcesada ? 'detail' : 'create',
							DesdeSolicitud: true,
							Detalles: Detalle.PlantillaDistribucionDetalleRecepcion,
							TipoProducto: Detalle.Tipo,
							Lineas: Lineas,
							Empresa: $scope.Solicitud.EmpresaParaLaQueSeCompra
						}
					}
				});

				modalInstance.result.then(function (result) {
					Detalle.PlantillaDistribucionDetalleRecepcion = result;
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
							Accion: $scope.TareaProcesada ? 'detail' : 'create',
							DesdeSolicitud: true,
							TipoProducto: Detalle.Tipo,
							Lineas: Lineas,
							Empresa: $scope.Solicitud.EmpresaParaLaQueSeCompra
						}
					}
				});

				modalInstance.result.then(function (result) {
					Detalle.PlantillaDistribucionDetalleRecepcion = result;
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

		$scope.SumarTotalCantidadRecepcion = function () {

			var suma = 0;

			if ($scope.DataAccion == 'show') {
				angular.forEach($scope.Recepcion.RecepcionLineas, function (item) {
					suma = (parseFloat(suma) + parseFloat(item.CantidadRecepcion ? item.CantidadRecepcion : 0)).toFixed(2);
				});
			}
			else {
				angular.forEach($scope.Solicitud.Detalles, function (item) {
					suma = (parseFloat(suma) + parseFloat(item.CantidadRecepcion ? item.CantidadRecepcion : 0)).toFixed(2);
				});
			}

			$scope.TotalCantidadRecepcion = suma;
		}

		$scope.SumarTotalValorRecepcion = function () {

			var suma = 0;

			if ($scope.DataAccion == 'show') {
				angular.forEach($scope.Recepcion.RecepcionLineas, function (item) {
					suma = (parseFloat(suma) + parseFloat(item.ValorRecepcion ? item.ValorRecepcion : 0)).toFixed(2);
				});
			}
			else {
				angular.forEach($scope.Solicitud.Detalles, function (item) {
					suma = (parseFloat(suma) + parseFloat(item.ValorRecepcion ? item.ValorRecepcion : 0)).toFixed(2);
				});
			}

			$scope.TotalValorRecepcion = suma;
		}

		$scope.SumarTotalSaldo = function () {

			var suma = 0;

			angular.forEach($scope.Solicitud.Detalles, function (item) {
				suma = (parseFloat(suma) + parseFloat(item.Saldo ? item.Saldo : 0)).toFixed(2);
			});

			$scope.TotalSaldo = suma;
		}

		$scope.ActualizarValorRecepcion = function (detalle) {
			detalle.ValorRecepcion = parseFloat(detalle.CantidadRecepcion ? detalle.CantidadRecepcion : 0) * parseFloat(detalle.Valor ? detalle.Valor : 0);
			
			detalle.ValorRecepcion = (parseFloat(detalle.ValorRecepcion) + parseFloat(detalle.ValorRecepcion) * parseFloat(detalle.Impuesto.Porcentaje ? detalle.Impuesto.Porcentaje : 0) / 100).toFixed(2);
			
			detalle.Saldo = (parseFloat(detalle.SaldoOriginal) - parseFloat(detalle.ValorRecepcion ? detalle.ValorRecepcion : 0)).toFixed(2);

			$scope.SumarTotalCantidadRecepcion();
			$scope.SumarTotalValorRecepcion();
			$scope.SumarTotalSaldo();
		}

		$scope.ActualizarCantidadRecepcion = function (detalle) {
			detalle.CantidadRecepcion = (parseFloat(detalle.ValorRecepcion ? detalle.ValorRecepcion : 0) / (parseFloat(detalle.Total ? detalle.Total : 0) / (parseFloat(detalle.Cantidad ? detalle.Cantidad : 0)))).toFixed(2);

			detalle.Saldo = (parseFloat(detalle.SaldoOriginal) - parseFloat(detalle.ValorRecepcion ? detalle.ValorRecepcion : 0)).toFixed(2);

			$scope.SumarTotalCantidadRecepcion();
			$scope.SumarTotalValorRecepcion();
			$scope.SumarTotalSaldo();
		}

		$scope.DescargarAdjunto = function (adjunto, Tipo) {
			$window.open($rootScope.pathURL + '/SolicitudCompra/DescargarAdjunto?SolicitudId=' + $scope.Solicitud.Id + '&AdjuntoId=' + adjunto.IdAdjunto + '&AdjuntoNombre=' + adjunto.Nombre + '&Tipo=' + Tipo, '_blank');
		}

		$scope.SubirFacturasAdjuntos = function () {

			Persistencia.SubirFacturasAdjuntos($scope.Solicitud.FacturasAdjuntos, $scope.Solicitud.Id, $scope.Tarea.NumeroOrdenMadre + '-' + $scope.Recepcion.NumeroRecepcion)
				.then(function (response) {
					var mensajes = '';

					angular.forEach(response.data.error, function (value) {
						mensajes = mensajes + '<li>' + value.error + '</li>';
					});

					angular.forEach(response.data.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						$scope.Solicitud.FacturasAdjuntos = [];
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						toastr['success']("Facturas adjuntos subidos.", "Confirmación");

						$scope.Solicitud.FacturasAdjuntosSalvados = [];
						$scope.Solicitud.FacturasAdjuntos = [];
						$scope.Solicitud.FacturasAdjuntosPrevisualizar = [];

						angular.forEach(response.data.adjuntos, function (item1) {

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
									case "1088":
										adjuntoTem.RecepcionId = item2.Valor;
										break;
								}
							});

							$scope.Solicitud.FacturasAdjuntosSalvados.push(adjuntoTem);
						});
					}
				});
		}

		$scope.EliminarFacturaAdjuntoSalvado = function (adjunto) {
			bootbox.confirm('<i class="fa fa-question-circle fa-3x text-danger" style="position: relative; top:10px;"></i> ¿Seguro que desea eliminar el elemento?', function (result) {
				if (result) {
					$http.post($rootScope.pathURL + '/SolicitudCompra/EliminarAdjunto', { AdjuntoId: adjunto.Id, SolicitudId: $scope.Solicitud.Id, Tipo: 'factura' })
						.success(function (response) {
							var mensajes = '';

							angular.forEach(response.validacion, function (value, index) {
								mensajes = mensajes + '<li>' + value + '</li>';
							});

							if (mensajes.length > 1) {
								bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
							} else {
								$scope.Solicitud.FacturasAdjuntosSalvados = [];

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
											case "1088":
												adjuntoTem.RecepcionId = item2.Valor;
												break;
										}
									});

									$scope.Solicitud.FacturasAdjuntosSalvados.push(adjuntoTem);
								});

								toastr['success']("Elemento eliminado.", "Confirmación");
							}
						});
				}
			});
		}

		$scope.ActualizarFacturasAdjuntas = function () {

			if ($scope.Solicitud.FacturasAdjuntosSalvados.length > 0) {

				var FacturasActualizar = [];

				angular.forEach($scope.Solicitud.FacturasAdjuntosSalvados, function (item) {
					FacturasActualizar.push(item.IdAdjunto);
				});

				$http.post($rootScope.pathURL + '/SolicitudCompra/ActualizarFacturasAdjuntas', { FacturasActualizar: FacturasActualizar, SolicitudId: $scope.Solicitud.Id, RecepcionId: ($scope.Tarea.NumeroOrdenMadre + '-' + $scope.Recepcion.NumeroRecepcion) })
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
							$window.open($rootScope.pathURL + '/Tarea/Index?mensaje=Elemento procesado correctamente.', '_self');
						}
					});
			}
		}

		$scope.BuscarFacturasElectronicas = function () {
			var modalInstance = $uibModal.open({
				animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/Tarea/views/BuscarFacturasElectronicas.html',
				controller: 'BuscarFacturasElectronicas',
				backdrop: 'static',
				size: 'small',
				resolve: {
					data: {
						TituloPag: 'Buscar facturas electrónicas',
						ruc: $scope.Solicitud.Detalles[0].Proveedor.Identificacion
					}
				}
			});

			modalInstance.result.then(function (result) {

				angular.forEach(result, function (item) {
					item.Pk = $scope.ComprobantesElectronicosPk++;
					item.EstadoId = 1;
					$scope.Recepcion.ComprobantesElectronicos.push(item);
				});

			}, function () {

			});
		}

		$scope.EliminarFacturaElectronica = function (comprobante) {
			$scope.Recepcion.ComprobantesElectronicos = $scope.Recepcion.ComprobantesElectronicos.filter(function (item) {
				return item.Pk != comprobante.Pk;
			});
		}

		$scope.Validar = function () {
			var errores = '';

			if (!(parseFloat($scope.TotalValorRecepcion) > 0)) {
				errores = errores + '<li>No ha entrado ningún Biene/Servicio para recepcionar.</li>';
			}

			var ErrorSaldo = false;

			angular.forEach($scope.Solicitud.Detalles, function (item) {
				if (parseFloat(item.Saldo) < 0) {
					ErrorSaldo = true;
				}
			});

			if (ErrorSaldo) {
				errores = errores + '<li>Se han identificado errores en la Recepción de Bienes/Servicios en la columna "Saldo". Verifique que el saldo sea mayor o igual a cero.</li>';
			}

			if (!$scope.AprobadorDesembolso || !$scope.AprobadorDesembolso.Usuario) {
				errores = errores + '<li>El campo "Aprobador de desembolso" es obligatorio.</li>';
			}

			if ((!$scope.Recepcion.ComprobantesElectronicos || $scope.Recepcion.ComprobantesElectronicos.length == 0) && (!$scope.Solicitud.FacturasAdjuntosSalvados || $scope.Solicitud.FacturasAdjuntosSalvados.length == 0)) {
				errores = errores + '<li>No ha adjuntado ninguna factura.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		}

		$scope.Crear = function () {
			if ($scope.Validar()) {

				var RecepcionLineas = [];

				angular.forEach($scope.Solicitud.Detalles, function (item) {
					if (item.CantidadRecepcion && parseFloat(item.CantidadRecepcion) > 0) {

						var Distribuciones = [];

						if (item.PlantillaDistribucionDetalleRecepcion) {
							angular.forEach(item.PlantillaDistribucionDetalleRecepcion, function (item2) {
								Distribuciones.push({
									Id: 0,
									Porcentaje: item2.Porcentaje ? parseFloat(item2.Porcentaje) : parseFloat(0),
									DepartamentoCodigo: item2.DepartamentoCodigo,
									DepartamentoDescripcion: item2.DepartamentoDescripcion,
									DepartamentoCodigoDescripcion: item2.DepartamentoCodigoDescripcion,
									CentroCostoCodigo: item2.CentroCostoCodigo,
									CentroCostoDescripcion: item2.CentroCostoDescripcion,
									CentroCostoCodigoDescripcion: item2.CentroCostoCodigoDescripcion,
									PropositoCodigo: item2.PropositoCodigo,
									PropositoDescripcion: item2.PropositoDescripcion,
									PropositoCodigoDescripcion: item2.PropositoCodigoDescripcion,
									EstadoId: item2.EstadoId
								});
							});
						}

						var NuevaRecepcionLinea = {
							Id: 0,
							Cantidad: item.CantidadRecepcion ? parseFloat(item.CantidadRecepcion) : parseFloat(0),
							Valor: item.ValorRecepcion ? parseFloat(item.ValorRecepcion) : parseFloat(0),
							SolicitudCompraDetalleId: item.Id,
							EstadoId: 1,
							Distribuciones: Distribuciones,
							Saldo: item.Saldo ? parseFloat(item.Saldo) : parseFloat(0)
						};

						RecepcionLineas.push(NuevaRecepcionLinea);
					}
				});

				var Recepcion = {
					Id: 0,
					NumeroRecepcion: $scope.Metadatos.NumeroRecepcion,
					FechaRecepcion: $scope.Recepcion.FechaRecepcion,
					OrdenMadreId: $scope.Tarea.OrdenMadreId,
					EstadoId: 1,

					RecepcionLineas: RecepcionLineas,
					ComprobantesElectronicos: ($scope.Recepcion ? ($scope.Recepcion.ComprobantesElectronicos ? $scope.Recepcion.ComprobantesElectronicos : null) : null)
				};

				$http.post($rootScope.pathURL + '/Tarea/TareaRecepcionEdit', { TareaId: $scope.TareaId, AprobadorDesembolso: $scope.AprobadorDesembolso.Usuario, Recepcion: Recepcion })
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
							$scope.Solicitud.Tareas = response.tareas;
							$scope.TareaProcesada = true;
							var NumeroRecepcionAux = $scope.Solicitud.NumeroSolicitud + '-' + response.NumeroRecepcion;

							if ($scope.Recepcion.NumeroRecepcion != NumeroRecepcionAux) {
								$scope.Recepcion.NumeroRecepcion = NumeroRecepcionAux;

								$scope.ActualizarFacturasAdjuntas();
							}
							else {
								$window.open($rootScope.pathURL + '/Tarea/Index?mensaje=Elemento procesado correctamente.', '_self');
							}

							//toastr['success']("Elemento procesado correctamente.", "Confirmación");
						}
					});
			}
		}

    }]);