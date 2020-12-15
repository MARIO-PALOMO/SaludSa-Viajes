
/**
 * Gestiona la lógica de control de la vista GestorCompraEdit del módulo Tarea.
 */
app_tarea.controller('TareaCotizacionSeleccionProveedorEdit', [
    '$scope',
    '$rootScope',
    '$uibModal',
    '$log',
	'$http',
	'json_js_datetime',
	'Persistencia',
	'$window',
	function ($scope, $rootScope, $uibModal, $log, $http, json_js_datetime, Persistencia, $window) {
		
		$scope.Sesion = {};
		$scope.Solicitud = {};
		
		$scope.Metadatos = {};

		$scope.DetallesPk = 1;

		$scope.TotalDetalles = '0.00';

		$scope.ObtenerMetadatos = function () {

			$scope.TareaId = $('#TareaId').val();
			$scope.SolicitudId = $('#SolicitudId').val();
			$scope.DataAccion = $('#Accion').val();

			$http.get($rootScope.pathURL + '/Tarea/ObtenerMetadatosConProveedoresEImpuestos?SolicitudId=' + $scope.SolicitudId)
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
						$scope.Solicitud.CotizacionesAdjuntos = [];
						$scope.Solicitud.EvaluacionesAdjuntos = [];
						$scope.Solicitud.MontoEstimado = parseFloat($scope.Solicitud.MontoEstimado).toFixed(2);

						angular.forEach($scope.Solicitud.Detalles, function (item) {
							item.Pk = $scope.DetallesPk++;
							item.Cantidad = item.Tipo == '0' ? parseInt(item.Cantidad) : parseFloat(item.Cantidad).toFixed(2);
							item.Valor = item.Valor ? parseFloat(item.Valor).toFixed(2) : '0.00';
							item.Total = item.Total ? parseFloat(item.Total).toFixed(2) : '0.00';

							item.Impuesto = item.Impuesto ? (item.Impuesto.Codigo ? item.Impuesto : $scope.Metadatos.Impuestos[0]) : $scope.Metadatos.Impuestos[0];
							item.Proveedor = item.Proveedor ? (item.Proveedor.Identificacion ? item.Proveedor : {}) : {};
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

							$scope.RetornaAJefeInmediato = $scope.Tarea.RetornaAJefeInmediato;
							$scope.Observacion = $scope.Tarea.Observacion;
						}
					}
				});
		}

        $scope.AdicionarDetalle = function () {
			$scope.Solicitud.Detalles.push({
				Pk: $scope.DetallesPk++,
				Tipo: '0',
				Cantidad: '0.00',
				Valor: '0.00',
				Total: '0.00',
				Impuesto: $scope.Metadatos.Impuestos[0],
				Proveedor: {}
			});
		}

		$scope.EliminarDetalle = function (Detalle) {
			$scope.Solicitud.Detalles = $scope.Solicitud.Detalles.filter(function (item) {
				return item.Pk != Detalle.Pk;
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
							TituloPag: $scope.TareaProcesada ? 'Detalle de distribución' : 'Adicionar distribución',
							Accion: $scope.TareaProcesada ? 'detail' : 'create',
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
							TituloPag: $scope.TareaProcesada ? 'Detalle de distribución' : 'Adicionar distribución',
							Accion: $scope.TareaProcesada ? 'detail' : 'create',
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

		$scope.EliminarRequerimientoAdjunto = function (adjunto) {
			$scope.Solicitud.RequerimientosAdjuntosPrevisualizar = $scope.Solicitud.RequerimientosAdjuntosPrevisualizar.filter(function (item) {
				return item.pk != adjunto.pk;
			});

			$scope.Solicitud.RequerimientosAdjuntos = $scope.Solicitud.RequerimientosAdjuntos.filter(function (item) {
				return item.pk != adjunto.pk;
			});
		}

		$scope.AbrirUrl = function (detalle) {
			if (detalle.Url) {
				$window.open(detalle.Url, '_blank');
			}
		}

		$scope.ActualizarTotal = function (detalle) {
			detalle.Total = parseFloat(detalle.Cantidad ? detalle.Cantidad : 0) * parseFloat(detalle.Valor ? detalle.Valor : 0);

			detalle.Total = (parseFloat(detalle.Total) + parseFloat(detalle.Total) * parseFloat(detalle.Impuesto.Porcentaje) / 100).toFixed(2);

			$scope.SumarTotal();
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

		$scope.SubirCotizacionesAdjuntos = function () {

			Persistencia.SubirCotizacionesAdjuntos($scope.Solicitud.CotizacionesAdjuntos, $scope.Solicitud.Id)
				.then(function (response) {
					var mensajes = '';

					angular.forEach(response.data.error, function (value) {
						mensajes = mensajes + '<li>' + value.error + '</li>';
					});

					angular.forEach(response.data.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						$scope.Solicitud.CotizacionesAdjuntos = [];
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						toastr['success']("Cotizaciones adjuntos subidos.", "Confirmación");

						$scope.Solicitud.CotizacionesAdjuntosSalvados = [];
						$scope.Solicitud.CotizacionesAdjuntos = [];
						$scope.Solicitud.CotizacionesAdjuntosPrevisualizar = [];

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
								}
							});

							$scope.Solicitud.CotizacionesAdjuntosSalvados.push(adjuntoTem);
						});
					}
				});
		}

		$scope.SubirEvaluacionesAdjuntos = function () {

			Persistencia.SubirEvaluacionesAdjuntos($scope.Solicitud.EvaluacionesAdjuntos, $scope.Solicitud.Id)
				.then(function (response) {
					var mensajes = '';

					angular.forEach(response.data.error, function (value) {
						mensajes = mensajes + '<li>' + value.error + '</li>';
					});

					angular.forEach(response.data.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						$scope.Solicitud.EvaluacionesAdjuntos = [];
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						toastr['success']("Evaluaciones adjuntos subidos.", "Confirmación");

						$scope.Solicitud.EvaluacionesAdjuntosSalvados = [];
						$scope.Solicitud.EvaluacionesAdjuntos = [];
						$scope.Solicitud.EvaluacionesAdjuntosPrevisualizar = [];

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
								}
							});

							$scope.Solicitud.EvaluacionesAdjuntosSalvados.push(adjuntoTem);
						});
					}
				});
		}

		$scope.EliminarCotizacionAdjuntoSalvado = function (adjunto) {
			bootbox.confirm('<i class="fa fa-question-circle fa-3x text-danger" style="position: relative; top:10px;"></i> ¿Seguro que desea eliminar el elemento?', function (result) {
				if (result) {
					$http.post($rootScope.pathURL + '/SolicitudCompra/EliminarAdjunto', { AdjuntoId: adjunto.Id, SolicitudId: $scope.Solicitud.Id, Tipo: 'cotización' })
						.success(function (response) {
							var mensajes = '';

							angular.forEach(response.validacion, function (value, index) {
								mensajes = mensajes + '<li>' + value + '</li>';
							});

							if (mensajes.length > 1) {
								bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
							} else {
								$scope.Solicitud.CotizacionesAdjuntosSalvados = [];

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

									$scope.Solicitud.CotizacionesAdjuntosSalvados.push(adjuntoTem);
								});

								toastr['success']("Elemento eliminado.", "Confirmación");
							}
						});
				}
			});
		}

		$scope.EliminarEvaluacionAdjuntoSalvado = function (adjunto) {
			bootbox.confirm('<i class="fa fa-question-circle fa-3x text-danger" style="position: relative; top:10px;"></i> ¿Seguro que desea eliminar el elemento?', function (result) {
				if (result) {
					$http.post($rootScope.pathURL + '/SolicitudCompra/EliminarAdjunto', { AdjuntoId: adjunto.Id, SolicitudId: $scope.Solicitud.Id, Tipo: 'evaluación' })
						.success(function (response) {
							var mensajes = '';

							angular.forEach(response.validacion, function (value, index) {
								mensajes = mensajes + '<li>' + value + '</li>';
							});

							if (mensajes.length > 1) {
								bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
							} else {
								$scope.Solicitud.EvaluacionesAdjuntosSalvados = [];

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

									$scope.Solicitud.EvaluacionesAdjuntosSalvados.push(adjuntoTem);
								});

								toastr['success']("Elemento eliminado.", "Confirmación");
							}
						});
				}
			});
		}

		$scope.CambiarCantidad = function (detalle) {
			detalle.PlantillaDistribucionDetalle = null;

			if (detalle.Tipo == '0') {
				if (!detalle.Cantidad) {
					detalle.Cantidad = '0';
				}
				else {
					detalle.Cantidad = parseInt(detalle.Cantidad);
				}
			}
			else if (detalle.Tipo == '2') {
				if (!detalle.Cantidad) {
					detalle.Cantidad = '0.00';
				}
				else {
					detalle.Cantidad = parseFloat(detalle.Cantidad).toFixed(2);
				}
			}

			$scope.ActualizarTotal(detalle);
		}

		$scope.AbandonoCantidadBien = function (detalle) {
			if (!detalle.Cantidad) {
				detalle.Cantidad = '0';
			}
		}

		$scope.CopiarProveedorClipBoard = function (proveedor) {
			if (proveedor && proveedor.Identificacion) {

				var texto = '(' + proveedor.TipoIdentificacion + '-' + proveedor.Identificacion + ') ' + proveedor.Nombre;

				const el = document.createElement('textarea');
				el.value = texto;
				el.setAttribute('readonly', '');
				el.style.position = 'absolute';
				el.style.left = '-9999px';
				document.body.appendChild(el);
				el.select();
				document.execCommand('copy');
				document.body.removeChild(el);

				toastr['success'](texto, "Proveedor copiado al portapapeles");
			}
		}

		$scope.ValidarEnviar = function () {
			var errores = '';

			var ErrorDetalleRepetido = false;

			var ErrorTipo = false;
			var ErrorProducto = false;
			var ErrorObservacion = false;
			var ErrorCantidad = false;

			var ErrorValor = false;
			var ErrorTotal = false;
			var ErrorImpuesto = false;
			var ErrorProveedor = false;

			var ErrorDistribucionDepartamento = false;
			var ErrorDistribucionCentroCosto = false;
			var ErrorDistribucionProposito = false;
			var ErrorDistribucionPorcentaje = false;
			var ErrorDistribucionPorcentajeSumaFinal = false;

			var ErrorDistribucionNoExiste = false;

			if (!$scope.Solicitud.Detalles || $scope.Solicitud.Detalles.length == 0) {
				errores = errores + '<li>No ha entrado ningún "Bien/Servicio".</li>';
			}

			angular.forEach($scope.Solicitud.Detalles, function (item) {
				var CantError = 0;

				if (!item.Observacion) {
					item.Observacion = null;
					ErrorObservacion = true;
				}

				if (item.Tipo != '0' && item.Tipo != '2') {
					ErrorTipo = true;
				}

				if (item.Tipo == '0' && (!item.ProductoBien || !item.ProductoBien.CodigoArticulo)) {
					ErrorProducto = true;
				}

				if (item.Tipo == '2' && (!item.ProductoServicio || !item.ProductoServicio.CodigoArticulo)) {
					ErrorProducto = true;
				}

				if (!item.Cantidad) {
					item.Cantidad = '0.00';
				}

				if (!(parseFloat(item.Cantidad) > 0)) {
					ErrorCantidad = true;
				}

				if (!item.Valor) {
					item.Valor = '0.00';
				}

				if (!(parseFloat(item.Valor) > 0)) {
					ErrorValor = true;
				}

				if (!item.Total) {
					item.Total = '0.00';
				}

				if (!(parseFloat(item.Total) > 0)) {
					ErrorTotal = true;
				}

				if (!item.Impuesto || !item.Impuesto.Codigo) {
					ErrorImpuesto = true;
				}

				if (!item.Proveedor || !item.Proveedor.Identificacion) {
					ErrorProveedor = true;
				}

				angular.forEach($scope.Solicitud.Detalles, function (item2) {

					if (!item2.Observacion) {
						item2.Observacion = null;
					}

					if (item.Tipo == item2.Tipo && item.Tipo == '0') {
						if (item.ProductoBien && item2.ProductoBien) {
							if (item.ProductoBien.CodigoArticulo == item2.ProductoBien.CodigoArticulo && item.Observacion == item2.Observacion) {
								CantError++;
							}
						}
					}
					else if (item.Tipo == item2.Tipo && item.Tipo == '2') {
						if (item.ProductoServicio && item2.ProductoServicio) {
							if (item.ProductoServicio.CodigoArticulo == item2.ProductoServicio.CodigoArticulo && item.Observacion == item2.Observacion) {
								CantError++;
							}
						}
					}
				});

				if (CantError > 1) {
					ErrorDetalleRepetido = true;
				}

				if (item.PlantillaDistribucionDetalle && item.PlantillaDistribucionDetalle.length > 0) {

					var PorcentajeFinal = 0;

					angular.forEach(item.PlantillaDistribucionDetalle, function (item2) {
						if (item2.EstadoId == 1) {

							if (!item2.Porcentaje) {
								item2.Porcentaje = '0.00';
							}

							PorcentajeFinal = (parseFloat(PorcentajeFinal) + parseFloat(item2.Porcentaje)).toFixed(2);

							if (!item2.DepartamentoCodigo) {
								ErrorDistribucionDepartamento = true;
							}

							if (!item2.CentroCostoCodigo) {
								ErrorDistribucionCentroCosto = true;
							}

							if (!item2.PropositoCodigo) {
								ErrorDistribucionProposito = true;
							}

							if (parseFloat(item2.Porcentaje) == 0) {
								ErrorDistribucionPorcentaje = true;
							}
						}
					});

					if (parseFloat(PorcentajeFinal) > 100 || parseFloat(PorcentajeFinal) < 100) {
						ErrorDistribucionPorcentajeSumaFinal = true;
					}
				}
				else {
					ErrorDistribucionNoExiste = true;
				}
			});

			if (ErrorTipo) {
				errores = errores + '<li>Se han identificado errores en los Bienes/Servicios a comprar en la columna "Tipo".</li>';
			}

			if (ErrorProducto) {
				errores = errores + '<li>Se han identificado errores en los Bienes/Servicios a comprar en la columna "Bien/Servicio".</li>';
			}

			if (ErrorObservacion) {
				errores = errores + '<li>Se han identificado errores en los Bienes/Servicios a comprar en la columna "Observación".</li>';
			}

			if (ErrorCantidad) {
				errores = errores + '<li>Se han identificado errores en los Bienes/Servicios a comprar en la columna "Cantidad".</li>';
			}
			
			if (ErrorValor) {
				errores = errores + '<li>Se han identificado errores en los Bienes/Servicios a comprar en la columna "Valor".</li>';
			}

			if (ErrorTotal) {
				errores = errores + '<li>Se han identificado errores en los Bienes/Servicios a comprar en la columna "Total".</li>';
			}

			if (ErrorImpuesto) {
				errores = errores + '<li>Se han identificado errores en los Bienes/Servicios a comprar en la columna "IVA".</li>';
			}

			if (ErrorProveedor) {
				errores = errores + '<li>Se han identificado errores en los Bienes/Servicios a comprar en la columna "Proveedor".</li>';
			}
			
			if (ErrorDetalleRepetido) {
				errores = errores + '<li>Existen Bienes/Servicios a comprar con el producto y la observación repetidas.</li>';
			}

			if (ErrorDistribucionNoExiste) {
				errores = errores + '<li>Existen "Bienes/Servicios a comprar" sin una distribución asignada.</li>';
			}

			if (ErrorDistribucionDepartamento) {
				errores = errores + '<li>Se han identificado errores en las distribuciones en el campo "Departamento".</li>';
			}

			if (ErrorDistribucionCentroCosto) {
				errores = errores + '<li>Se han identificado errores en las distribuciones en el campo "Centro de costo".</li>';
			}

			if (ErrorDistribucionProposito) {
				errores = errores + '<li>Se han identificado errores en las distribuciones en el campo "Propósito".</li>';
			}

			if (ErrorDistribucionPorcentaje) {
				errores = errores + '<li>Se han identificado errores en las distribuciones en el campo "Porcentaje".</li>';
			}

			if (ErrorDistribucionPorcentajeSumaFinal) {
				errores = errores + '<li>Se han identificado errores en las distribuciones "Suma de porcentajes distinta de 100".</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
		}

		$scope.Crear = function () {

			if ($scope.ValidarEnviar()) {
				var Detalles = [];

				angular.forEach($scope.Solicitud.Detalles, function (item) {
					console.log(item.Impuesto);
					var Distribuciones = [];

					if (item.PlantillaDistribucionDetalle) {
						angular.forEach(item.PlantillaDistribucionDetalle, function (item2) {
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

					var NuevoDetalle = {
						Id: 0,
						CompraInternacional: item.CompraInternacional ? true : false,
						Tipo: item.Tipo,
						Producto: item.Tipo == '0' ? (item.ProductoBien ? (item.ProductoBien.CodigoArticulo ? item.ProductoBien.CodigoArticulo : null) : null) : (item.ProductoServicio ? (item.ProductoServicio.CodigoArticulo ? item.ProductoServicio.CodigoArticulo : null) : null),
						ProductoNombre: item.Tipo == '0' ? (item.ProductoBien ? (item.ProductoBien.Nombre ? item.ProductoBien.Nombre : null) : null) : (item.ProductoServicio ? (item.ProductoServicio.Nombre ? item.ProductoServicio.Nombre : null) : null),
						GrupoProducto: item.Tipo == '0' ? (item.ProductoBien ? (item.ProductoBien.CodigoGrupoArticulo ? item.ProductoBien.CodigoGrupoArticulo : null) : null) : (item.ProductoServicio ? (item.ProductoServicio.CodigoGrupoArticulo ? item.ProductoServicio.CodigoGrupoArticulo : null) : null),
						GrupoProductoNombre: item.Tipo == '0' ? (item.ProductoBien ? (item.ProductoBien.Grupo ? item.ProductoBien.Grupo : null) : null) : (item.ProductoServicio ? (item.ProductoServicio.Grupo ? item.ProductoServicio.Grupo : null) : null),
						Observacion: item.Observacion ? item.Observacion.toUpperCase() : null,
						Cantidad: item.Cantidad ? parseFloat(item.Cantidad) : parseFloat(0),
						Url: item.Url,
						EstadoId: 1,
						Distribuciones: Distribuciones,

						Valor: item.Valor ? parseFloat(item.Valor) : parseFloat(0),
						Total: item.Total ? parseFloat(item.Total) : parseFloat(0),
						CodigoImpuestoVigente: (item.Impuesto ? (item.Impuesto.Codigo ? item.Impuesto.Codigo : null) : null),
						PorcentajeImpuestoVigente: (item.Impuesto ? item.Impuesto.Porcentaje : null),
						DescripcionImpuestoVigente: (item.Impuesto ? (item.Impuesto.Descripcion ? item.Impuesto.Descripcion : null) : null),
						IdentificacionProveedor: (item.Proveedor ? (item.Proveedor.Identificacion ? item.Proveedor.Identificacion : null) : null),
						NombreProveedor: (item.Proveedor ? (item.Proveedor.Nombre ? item.Proveedor.Nombre : null) : null),
						RazonSocialProveedor: (item.Proveedor ? (item.Proveedor.RazonSocial ? item.Proveedor.RazonSocial : null) : null),
						TipoIdentificacionProveedor: (item.Proveedor ? (item.Proveedor.TipoIdentificacion ? item.Proveedor.TipoIdentificacion : null) : null),
						BloqueadoProveedor: (item.Proveedor ? (item.Proveedor.Bloqueado ? item.Proveedor.Bloqueado : null) : null),
						CorreoProveedor: (item.Proveedor ? (item.Proveedor.Correo ? item.Proveedor.Correo : null) : null),
						TelefonoProveedor: (item.Proveedor ? (item.Proveedor.Telefono ? item.Proveedor.Telefono : null) : null),
						DireccionProveedor: (item.Proveedor ? (item.Proveedor.Direccion ? item.Proveedor.Direccion : null) : null)
					};

					Detalles.push(NuevoDetalle);
				});

				$http.post($rootScope.pathURL + '/Tarea/TareaGestorCompraEdit', {
					Detalles: Detalles,
					TareaId: $scope.TareaId,
					SolicitudId: $scope.SolicitudId,
					Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null),
					RetornaAJefeInmediato: ($scope.RetornaAJefeInmediato ? true : false)
				})
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

    }]);