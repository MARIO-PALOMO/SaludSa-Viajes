
/**
 * Gestiona la lógica de control de la vista DevueltaSolicitanteEdit del módulo Tarea.
 */
app_tarea.controller('TareaDevueltaSolicitanteEdit', [
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

		$scope.RequerimientosAdjuntosPrevisualizarPk = 1;

		$scope.ObtenerMetadatos = function () {

			$scope.SolicitudId = $('#SolicitudId').val();
			$scope.TareaId = $('#TareaId').val();
			$scope.DataAccion = $('#Accion').val();

			if ($scope.DataAccion == 'show') {
				$scope.TareaProcesada = true;
			}

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
						$scope.Solicitud.RequerimientosAdjuntosPrevisualizar = [];

						$scope.Solicitud.FechaSolicitudAux = $scope.Solicitud.FechaSolicitud;
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
					}
				});
		}

		$scope.CambiaMontoEstimado = function () {
			if (parseFloat($scope.Solicitud.MontoEstimado) < 2500) {
				$scope.Solicitud.AprobacionSubgerenteArea = {};
				$scope.Solicitud.AprobacionGerenteArea = {};
				$scope.Solicitud.AprobacionVicePresidenteFinanciero = {};
				$scope.Solicitud.AprobacionGerenteGeneral = {};
			}
			else if (parseFloat($scope.Solicitud.MontoEstimado) < 5000) {
				$scope.Solicitud.AprobacionGerenteArea = {};
				$scope.Solicitud.AprobacionVicePresidenteFinanciero = {};
				$scope.Solicitud.AprobacionGerenteGeneral = {};
			}
			else if (parseFloat($scope.Solicitud.MontoEstimado) < 10000) {
				$scope.Solicitud.AprobacionVicePresidenteFinanciero = {};
				$scope.Solicitud.AprobacionGerenteGeneral = {};
			}
			else if (parseFloat($scope.Solicitud.MontoEstimado) < 120000) {
				$scope.Solicitud.AprobacionGerenteGeneral = {};
			}
		}

		$scope.CambiarEmpresaParaLaQueSeCompra = function () {
			$scope.Solicitud.Detalles = [];
			$scope.DetallesPk = 1;

			$http.get($rootScope.pathURL + '/SolicitudCompra/ObtenerProductos?EmpresaCodigo=' + $scope.Solicitud.EmpresaParaLaQueSeCompra.Codigo)
				.success(function (response) {

					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						$scope.Metadatos.Bienes = [];
						$scope.Metadatos.Servicios = [];
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos.Bienes = response.Bienes;
						$scope.Metadatos.Servicios = response.Servicios;

						$scope.AdicionarDetalle();
					}
				});
		}

        $scope.AdicionarDetalle = function () {
			$scope.Solicitud.Detalles.push({
				Pk: $scope.DetallesPk++,
				Tipo: '0',
				Cantidad: '0.00'
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

		$scope.PrevisualizarRequerimientosAdjuntos = function (event) {
			var files = event.target.files;

			angular.forEach(files, function (file) {

				var ext = file.name.match(/\.([^\.]+)$/);
				ext = ext ? ext[1].toLowerCase() : ext;

				if (file.size <= 5242880 &&
					(ext == 'pdf'
					|| ext == 'doc'
					|| ext == 'docx'
					|| ext == 'xls'
					|| ext == 'xlsx'
					|| ext == 'csv'
					|| ext == 'bmp'
					|| ext == 'dib'
					|| ext == 'jpe'
					|| ext == 'jpeg'
					|| ext == 'jpg'
					|| ext == 'png'
					|| ext == 'tif'
					|| ext == 'tiff')) {
					file.pk = $scope.RequerimientosAdjuntosPrevisualizarPk++;

					$scope.Solicitud.RequerimientosAdjuntosPrevisualizar.push({
						pk: file.pk,
						nombre: file.name
					});
					var reader = new FileReader();
					reader.onload = function () {
						$scope.$apply();
					};
					reader.readAsDataURL(file);
				}
			});
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

		$scope.DescargarAdjunto = function (adjunto, Tipo) {
			$window.open($rootScope.pathURL + '/SolicitudCompra/DescargarAdjunto?SolicitudId=' + $scope.Solicitud.Id + '&AdjuntoId=' + adjunto.IdAdjunto + '&AdjuntoNombre=' + adjunto.Nombre + '&Tipo=' + Tipo, '_blank');
		}

		$scope.EliminarRequerimientoAdjuntoSalvado = function (adjunto) {
			bootbox.confirm('<i class="fa fa-question-circle fa-3x text-danger" style="position: relative; top:10px;"></i> ¿Seguro que desea eliminar el elemento?', function (result) {
				if (result) {
					$http.post($rootScope.pathURL + '/SolicitudCompra/EliminarAdjunto', { AdjuntoId: adjunto.Id, SolicitudId: $scope.Solicitud.Id, Tipo: 'requerimiento' })
						.success(function (response) {
							var mensajes = '';

							angular.forEach(response.validacion, function (value, index) {
								mensajes = mensajes + '<li>' + value + '</li>';
							});

							if (mensajes.length > 1) {
								bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
							} else {
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

								toastr['success']("Elemento eliminado.", "Confirmación");
							}
						});
				}
			});
		}

		$scope.SubirRequerimientosAdjuntos = function () {

			Persistencia.SubirRequerimientosAdjuntos($scope.Solicitud.RequerimientosAdjuntos, $scope.Solicitud.Id)
				.then(function (response) {
					var mensajes = '';

					angular.forEach(response.data.error, function (value) {
						mensajes = mensajes + '<li>' + value.error + '</li>';
					});

					angular.forEach(response.data.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						$scope.Solicitud.RequerimientosAdjuntos = [];
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					}
					else {
						toastr['success']("Requerimientos adjuntos subidos.", "Confirmación");

						$scope.Solicitud.RequerimientosAdjuntosSalvados = [];
						$scope.Solicitud.RequerimientosAdjuntos = [];
						$scope.Solicitud.RequerimientosAdjuntosPrevisualizar = [];

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

							$scope.Solicitud.RequerimientosAdjuntosSalvados.push(adjuntoTem);
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
		}

		$scope.AbandonoCantidadBien = function (detalle) {
			if (!detalle.Cantidad) {
				detalle.Cantidad = '0';
			}
		}

		$scope.ValidarEnviar = function () {
			var errores = '';

			if (!$scope.Solicitud.EmpresaParaLaQueSeCompra || !$scope.Solicitud.EmpresaParaLaQueSeCompra.Codigo) {
				errores = errores + '<li>El campo "Empresa para la que se compra" es obligatorio.</li>';
			}

			if (!$scope.Solicitud.Frecuencia) {
				errores = errores + '<li>El campo "Frecuencia" es obligatorio.</li>';
			}

			if (!$scope.Solicitud.MontoEstimado) {
				$scope.Solicitud.MontoEstimado = '0.00';
			}

			if (!(parseFloat($scope.Solicitud.MontoEstimado) > 0)) {
				errores = errores + '<li>El campo "Monto estimado" debe tener un valor numérico mayor que cero.</li>';
			}

			if ($scope.Solicitud.SolicitanteObj.Departamento == 'GERENCIA DE MERCADEO' || $scope.Solicitud.SolicitanteObj.Departamento == 'GERENCIA DE MERCADEO Y VENTAS') {
				if (!$scope.Solicitud.ProductoMercadeo || !$scope.Solicitud.ProductoMercadeo.Codigo) {
					errores = errores + '<li>El campo "Producto mercadeo" es obligatorio.</li>';
				}
			}

			if (!$scope.Solicitud.Descripcion) {
				errores = errores + '<li>El campo "Descripción" es obligatorio.</li>';
			}

			var ErrorDetalleRepetido = false;

			var ErrorTipo = false;
			var ErrorProducto = false;
			var ErrorObservacion = false;
			var ErrorCantidad = false;

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

			if (!$scope.Solicitud.AprobacionJefeArea || !$scope.Solicitud.AprobacionJefeArea.Usuario) {
				errores = errores + '<li>El campo "Jefe de Área" es obligatorio.</li>';
			}

			if (parseFloat($scope.Solicitud.MontoEstimado) >= 2500 && (!$scope.Solicitud.AprobacionSubgerenteArea || !$scope.Solicitud.AprobacionSubgerenteArea.Usuario)) {
				errores = errores + '<li>El campo "Subgerente de Área" es obligatorio.</li>';
			}

			if (parseFloat($scope.Solicitud.MontoEstimado) >= 5000 && (!$scope.Solicitud.AprobacionGerenteArea || !$scope.Solicitud.AprobacionGerenteArea.Usuario)) {
				errores = errores + '<li>El campo "Gerente de Área" es obligatorio.</li>';
			}

			if (parseFloat($scope.Solicitud.MontoEstimado) >= 10000 && (!$scope.Solicitud.AprobacionVicePresidenteFinanciero || !$scope.Solicitud.AprobacionVicePresidenteFinanciero.Usuario)) {
				errores = errores + '<li>El campo "Vicepresidente Financiero" es obligatorio.</li>';
			}

			if (parseFloat($scope.Solicitud.MontoEstimado) >= 120000 && (!$scope.Solicitud.AprobacionGerenteGeneral || !$scope.Solicitud.AprobacionGerenteGeneral.Usuario)) {
				errores = errores + '<li>El campo "Gerente General" es obligatorio.</li>';
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
						Distribuciones: Distribuciones
					};

					Detalles.push(NuevoDetalle);
				});

				var Cabecera = {
					Id: $scope.Solicitud.Id,
					NumeroSolicitud: $scope.Solicitud.NumeroSolicitud,
					FechaSolicitud: $scope.Solicitud.FechaSolicitudAux,
					SolicitanteUsuario: $scope.Solicitud.SolicitanteObj.Usuario,
					SolicitanteNombreCompleto: $scope.Solicitud.SolicitanteObj.NombreCompleto,
					SolicitanteCiudadCodigo: $scope.Solicitud.SolicitanteObj.CiudadCodigo,
					EmpresaCodigo: $scope.Solicitud.EmpresaParaLaQueSeCompra.Codigo,
					EmpresaNombre: $scope.Solicitud.EmpresaParaLaQueSeCompra.Nombre,
					ProveedorSugerido: $scope.Solicitud.ProveedorSugerido ? $scope.Solicitud.ProveedorSugerido.toUpperCase() : null,
					Frecuencia: $scope.Solicitud.Frecuencia,
					MontoEstimado: $scope.Solicitud.MontoEstimado ? parseFloat($scope.Solicitud.MontoEstimado) : parseFloat(0),
					ProductoMercadeoCodigo: $scope.Solicitud.ProductoMercadeo ? ($scope.Solicitud.ProductoMercadeo.Codigo ? $scope.Solicitud.ProductoMercadeo.Codigo : null) : null,
					ProductoMercadeoNombre: $scope.Solicitud.ProductoMercadeo ? ($scope.Solicitud.ProductoMercadeo.Nombre ? $scope.Solicitud.ProductoMercadeo.Nombre : null) : null,
					Descripcion: $scope.Solicitud.Descripcion ? $scope.Solicitud.Descripcion.toUpperCase() : null,
					AprobacionJefeArea: $scope.Solicitud.AprobacionJefeArea ? ($scope.Solicitud.AprobacionJefeArea.Usuario ? $scope.Solicitud.AprobacionJefeArea.Usuario : null) : null,
					AprobacionSubgerenteArea: $scope.Solicitud.AprobacionSubgerenteArea ? ($scope.Solicitud.AprobacionSubgerenteArea.Usuario ? $scope.Solicitud.AprobacionSubgerenteArea.Usuario : null) : null,
					AprobacionGerenteArea: $scope.Solicitud.AprobacionGerenteArea ? ($scope.Solicitud.AprobacionGerenteArea.Usuario ? $scope.Solicitud.AprobacionGerenteArea.Usuario : null) : null,
					AprobacionVicePresidenteFinanciero: $scope.Solicitud.AprobacionVicePresidenteFinanciero ? ($scope.Solicitud.AprobacionVicePresidenteFinanciero.Usuario ? $scope.Solicitud.AprobacionVicePresidenteFinanciero.Usuario : null) : null,
					AprobacionGerenteGeneral: $scope.Solicitud.AprobacionGerenteGeneral ? ($scope.Solicitud.AprobacionGerenteGeneral.Usuario ? $scope.Solicitud.AprobacionGerenteGeneral.Usuario : null) : null,
					EstadoId: 1,

					Detalles: Detalles
				};

				Persistencia.EditDevueltaSolicitante($scope.Solicitud.RequerimientosAdjuntos, Cabecera, $scope.TareaId, 'Editada')
					.then(function (response) {
						var mensajes = '';

						angular.forEach(response.data.error, function (value) {
							mensajes = mensajes + '<li>' + value.error + '</li>';
						});

						angular.forEach(response.data.validacion, function (value) {
							mensajes = mensajes + '<li>' + value + '</li>';
						});

						if (mensajes.length > 1) {
							bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						}
						else {
							//$scope.Solicitud.Tareas = response.data.tareas;
							//toastr['success']("Elemento procesado correctamente.", "Confirmación");
							//$scope.TareaProcesada = true;
							$window.open($rootScope.pathURL + '/Tarea/Index?mensaje=Elemento procesado correctamente.', '_self');
						}
					});
			}
		}

    }]);