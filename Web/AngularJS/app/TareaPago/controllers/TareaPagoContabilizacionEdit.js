
/**
 * Gestiona la lógica de control de la vista ContabilizacionEdit del módulo Tarea Pago.
 */
app_tarea_pago.controller('TareaPagoContabilizacionEdit', [
	'$scope',
	'$rootScope',
	'$uibModal',
	'$log',
	'$http',
	'json_js_datetime',
	'json_js_date',
	'PersistenciaPago',
	'$window',
	function ($scope, $rootScope, $uibModal, $log, $http, json_js_datetime, json_js_date, PersistenciaPago, $window) {

		$scope.Sesion = {};
		$scope.Solicitud = {};
		$scope.Tarea = {};

		$scope.Metadatos = {};
		$scope.Data = {};

		$scope.Accion = 'Contabilizar';

		$scope.ObtenerMetadatos = function () {

			$scope.SolicitudId = $('#SolicitudId').val();
            $scope.TareaId = $('#TareaId').val();
            $scope.DataAccion = $('#Accion').val();

			$http.get($rootScope.pathURL + '/ContabilidadPago/ObtenerMetadatos?SolicitudId=' + $scope.SolicitudId)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos = response;

						$scope.Metadatos.TiposDiario = [
							{
								Codigo: '0',
								Descripcion: 'Diario General'
							},
							{
								Codigo: '10',
								Descripcion: 'Diario de Facturas'
							}
						];

						$scope.Tarea.TipoDiario = {};
						$scope.Tarea.Diario = {};
						$scope.Tarea.PerfilAsientoContable = {};
						$scope.Tarea.Departamento = {};
						$scope.Tarea.CatalogoPago = {};

                        $scope.ObtenerSolicitud();
                        $scope.ObtenerTarea();
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.ObtenerSolicitud = function () {
			$http.get($rootScope.pathURL + '/ContabilidadPago/ObtenerSolicitud?Id=' + $scope.SolicitudId + '&tareaId=' + $scope.TareaId)
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
							item.FechaEmision = json_js_date.convertir(item.FechaEmision);
                            item.FechaVencimiento = json_js_date.convertir(item.FechaVencimiento);

                            angular.forEach(item.FacturaDetallesPago, function (item2) {
                                if (item2.GrupoImpuestoCodigo) {
                                    item2.GrupoImpuesto = {
                                        Codigo: item2.GrupoImpuestoCodigo,
                                        Descripcion: item2.GrupoImpuestoDescripcion
                                    };

                                    item2.GrupoImpuestoArticulo = {
                                        Codigo: item2.GrupoImpuestoArticuloCodigo,
                                        Descripcion: item2.GrupoImpuestoArticuloDescripcion,
                                        CodigoDescripcion: item2.GrupoImpuestoArticuloCodigoDescripcion
                                    };

                                    item2.ImpuestoRentaGrupoImpuestoArticulo = {
                                        Codigo: item2.ImpuestoRentaGrupoImpuestoArticuloCodigo,
                                        Descripcion: item2.ImpuestoRentaGrupoImpuestoArticuloDescripcion,
                                        CodigoDescripcion: item2.ImpuestoRentaGrupoImpuestoArticuloCodigoDescripcion
                                    };

                                    item2.IvaGrupoImpuestoArticulo = {
                                        Codigo: item2.IvaGrupoImpuestoArticuloCodigo,
                                        Descripcion: item2.IvaGrupoImpuestoArticuloDescripcion,
                                        CodigoDescripcion: item2.IvaGrupoImpuestoArticuloCodigoDescripcion
                                    };

                                    item2.SustentoTributario = {
                                        Codigo: item2.SustentoTributarioCodigo,
                                        Descripcion: item2.SustentoTributarioDescripcion,
                                        CodigoDescripcion: item2.SustentoTributarioCodigoDescripcion
                                    };
                                }
                            });
						});

						$scope.Tarea.CatalogoPago = {
							Codigo: $scope.Solicitud.Facturas[0].TipoPagoObj.CuentaContableCodigo,
							Nombre: $scope.Solicitud.Facturas[0].TipoPagoObj.CuentaContableNombre,
							Tipo: $scope.Solicitud.Facturas[0].TipoPagoObj.CuentaContableTipo
						};
					}
				});
		}

		$scope.CambiaTipoDiario = function () {
			$http.get($rootScope.pathURL + '/ContabilidadPago/ObtenerDiarios?CompaniaCodigo=' + $scope.Solicitud.EmpresaParaLaQueSeCompra.Codigo + '&tipoDiario=' + $scope.Tarea.TipoDiario.Codigo)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos.Diarios = response.Diarios;

						$scope.Tarea.Diario = {};
					}
				});
		}

		$scope.MostrarFactura = function (Factura) {
			var modalInstance = $uibModal.open({
				animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/SolicitudPago/views/factura_fisica.html',
				controller: 'SolicitudPagoFacturaFisica',
				backdrop: 'static',
				size: 'full',
				resolve: {
					data: {
						TituloPag: Factura.Tipo == 'Física' ? 'Factura física' : 'Factura electrónica',
						EmpresaParaLaQueSeCompra: $scope.Solicitud.EmpresaParaLaQueSeCompra,
						TipoFactura: Factura.Tipo,
						Factura: Factura,
						EsReembolso: $scope.Solicitud.Facturas[0] ? ($scope.Solicitud.Facturas[0].TipoPagoObj.EsReembolso) : null,
                        Accion: ($scope.DataAccion == 'show' ? 'show' : 'detail'),
						SolicitudId: $scope.Solicitud.Id,
                        DesdeContabilizacion: true
					}
				}
			});

			modalInstance.result.then(function (result) {

				var Detalles = result.detalles;

                $scope.Solicitud.Facturas[0].NoLiquidacion = result.NoLiquidacion;
                $scope.Solicitud.Facturas[0].NoAutorizacion = result.NoAutorizacion;
                $scope.Solicitud.Facturas[0].FechaEmision = result.FechaEmision;
                $scope.Solicitud.Facturas[0].FechaVencimiento = result.FechaVencimiento;

                if ($scope.Solicitud.Facturas[0].TipoPagoObj.Id != result.TipoPagoObj.Id) {
                    $scope.Solicitud.Facturas[0].TipoPagoId = result.TipoPagoId;
                    $scope.Solicitud.Facturas[0].TipoPagoObj = result.TipoPagoObj;

                    $scope.Tarea.CatalogoPago = {
                        Codigo: $scope.Solicitud.Facturas[0].TipoPagoObj.CuentaContableCodigo,
                        Nombre: $scope.Solicitud.Facturas[0].TipoPagoObj.CuentaContableNombre,
                        Tipo: $scope.Solicitud.Facturas[0].TipoPagoObj.CuentaContableTipo
                    };

                }

				for (var i = 0; i < $scope.Solicitud.Facturas[0].FacturaDetallesPago.length; i++) {
					$scope.Solicitud.Facturas[0].FacturaDetallesPago[i].GrupoImpuesto = Detalles[i].GrupoImpuesto;
					$scope.Solicitud.Facturas[0].FacturaDetallesPago[i].GrupoImpuestoArticulo = Detalles[i].GrupoImpuestoArticulo;
					$scope.Solicitud.Facturas[0].FacturaDetallesPago[i].SustentoTributario = Detalles[i].SustentoTributario;
					$scope.Solicitud.Facturas[0].FacturaDetallesPago[i].ImpuestoRentaGrupoImpuestoArticulo = Detalles[i].ImpuestoRentaGrupoImpuestoArticulo;
                    $scope.Solicitud.Facturas[0].FacturaDetallesPago[i].IvaGrupoImpuestoArticulo = Detalles[i].IvaGrupoImpuestoArticulo;

                    $scope.Solicitud.Facturas[0].FacturaDetallesPago[i].PlantillaDistribucionDetalle = Detalles[i].PlantillaDistribucionDetalle;
				}
			}, function () {

			});
		}

		$scope.DescargarAdjuntoFactura = function (Factura) {
			if (Factura.Tipo == 'Física' && Factura.Id) {
				$window.open($rootScope.pathURL + '/SolicitudPago/DescargarAdjunto?SolicitudId=' + $scope.Solicitud.Id + '&FacturaId=' + Factura.Id + '&NoFactura=' + Factura.NoFactura, '_blank');
			}
			else {
				$window.open($scope.Metadatos.UrlVisorRidePdf + Factura.FacturaElectronica.claveAcceso, '_blank');
			}
		}

		$scope.Validar = function () {
			var errores = '';

			if (!$scope.Accion) {
				errores = errores + '<li>No ha seleccionado una acción.</li>';
			}
			else if ($scope.Accion == 'Contabilizar') {
				if (!$scope.Tarea.TipoDiario || !$scope.Tarea.TipoDiario.Codigo) {
					errores = errores + '<li>No ha seleccionado un tipo de diario.</li>';
				}

				if (!$scope.Tarea.Diario || !$scope.Tarea.Diario.Codigo) {
					errores = errores + '<li>No ha seleccionado un nombre de diario.</li>';
				}

				if (!$scope.Tarea.PerfilAsientoContable || !$scope.Tarea.PerfilAsientoContable.Codigo) {
					errores = errores + '<li>No ha seleccionado un perfil de asiento contable.</li>';
				}

				if (!$scope.Tarea.Departamento || !$scope.Tarea.Departamento.Codigo) {
					errores = errores + '<li>No ha seleccionado un departamento.</li>';
				}

				if (!$scope.Tarea.CatalogoPago || !$scope.Tarea.CatalogoPago.Codigo) {
					errores = errores + '<li>No ha seleccionado un catalogo de pagos.</li>';
				}

				var Factura = $scope.Solicitud.Facturas[0];

				if (Factura.TipoPagoObj.EsReembolso) {
					if (!Factura.NoLiquidacion) {
						errores = errores + '<li>No ha entrado un número de liquidación en la factura.</li>';
					}
					else {
						if (Factura.NoLiquidacion.length != 15) {
							errores = errores + '<li>El número de liquidación entrado en la factura no es válido.</li>';
						}
					}
				}

				var errorGrupoImpuesto = false;
				var errorGrupoImpuestoArticulo = false;
				var errorSustentoTributario = false;
				var errorImpuestoRentaGrupoImpuestoArticulo = false;
				var errorIvaGrupoImpuestoArticulo = false;

				angular.forEach(Factura.FacturaDetallesPago, function (item) {
					if (!item.GrupoImpuesto || !item.GrupoImpuesto.Codigo) {
						errorGrupoImpuesto = true;
					}

					if (!item.GrupoImpuestoArticulo || !item.GrupoImpuestoArticulo.Codigo) {
						errorGrupoImpuestoArticulo = true;
					}

					if (!item.SustentoTributario || !item.SustentoTributario.Codigo) {
						errorSustentoTributario = true;
					}

					if (!item.ImpuestoRentaGrupoImpuestoArticulo || !item.ImpuestoRentaGrupoImpuestoArticulo.Codigo) {
						errorImpuestoRentaGrupoImpuestoArticulo = true;
					}

					if (!item.IvaGrupoImpuestoArticulo || !item.IvaGrupoImpuestoArticulo.Codigo) {
						errorIvaGrupoImpuestoArticulo = true;
					}
				});

				if (errorGrupoImpuesto) {
					errores = errores + '<li>Se han detectado errores en la columna "Grupo impuesto" en los detalles de la factura.</li>';
				}

				if (errorGrupoImpuestoArticulo) {
					errores = errores + '<li>Se han detectado errores en la columna "Grupo impuesto artículo" en los detalles de la factura.</li>';
				}

				if (errorSustentoTributario) {
					errores = errores + '<li>Se han detectado errores en la columna "Sustento tributario" en los detalles de la factura.</li>';
				}

				if (errorImpuestoRentaGrupoImpuestoArticulo) {
					errores = errores + '<li>Se han detectado errores en la columna "Retención renta" en los detalles de la factura.</li>';
				}

				if (errorIvaGrupoImpuestoArticulo) {
					errores = errores + '<li>Se han detectado errores en la columna "Retención IVA" en los detalles de la factura.</li>';
				}
			}
			else {
				if (!$scope.Observacion) {
					errores = errores + '<li>No ha entrado un comentario.</li>';
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

				var Factura = $scope.Solicitud.Facturas[0];

				var InformacionContabilidadPago = {};

                var NoLiquidacion = null;
                var NoAutorizacion = null;
                var FechaEmision = null;
                var FechaVencimiento = null;
                var TipoPagoId = null;

				var Detalles = [];

				if ($scope.Accion == 'Contabilizar') {

					InformacionContabilidadPago = {
						Id: $scope.TareaId,
						TipoDiarioCodigo: $scope.Tarea.TipoDiario.Codigo,
						TipoDiarioDescripcion: $scope.Tarea.TipoDiario.Descripcion,
						DiarioCodigo: $scope.Tarea.Diario.Codigo,
						DiarioDescripcion: $scope.Tarea.Diario.Descripcion,
						PerfilAsientoContableCodigo: $scope.Tarea.PerfilAsientoContable.Codigo,
						PerfilAsientoContableDescripcion: $scope.Tarea.PerfilAsientoContable.Descripcion,
						DepartamentoCodigo: $scope.Tarea.Departamento.Codigo,
						DepartamentoDescripcion: $scope.Tarea.Departamento.Descripcion,
						DepartamentoCodigoDescripcion: $scope.Tarea.Departamento.CodigoDescripcion,
						CuentaContableCodigo: $scope.Tarea.CatalogoPago.Codigo,
						CuentaContableNombre: $scope.Tarea.CatalogoPago.Nombre,
						CuentaContableTipo: $scope.Tarea.CatalogoPago.Tipo,
						EsReembolso: Factura.TipoPagoObj.EsReembolso
					};

                    NoLiquidacion = Factura.NoLiquidacion;
                    NoAutorizacion = Factura.NoAutorizacion;
                    FechaEmision = Factura.FechaEmision;
                    FechaVencimiento = Factura.FechaVencimiento;
                    TipoPagoId = Factura.TipoPagoObj.Id;

					angular.forEach(Factura.FacturaDetallesPago, function (item) {
						Detalles.push({
							Id: item.Id,

							GrupoImpuestoCodigo: item.GrupoImpuesto.Codigo,
							GrupoImpuestoDescripcion: item.GrupoImpuesto.Descripcion,

							GrupoImpuestoArticuloCodigo: item.GrupoImpuestoArticulo.Codigo,
							GrupoImpuestoArticuloDescripcion: item.GrupoImpuestoArticulo.Descripcion,
							GrupoImpuestoArticuloCodigoDescripcion: item.GrupoImpuestoArticulo.CodigoDescripcion,

							SustentoTributarioCodigo: item.SustentoTributario.Codigo,
							SustentoTributarioDescripcion: item.SustentoTributario.Descripcion,
							SustentoTributarioCodigoDescripcion: item.SustentoTributario.CodigoDescripcion,

							ImpuestoRentaGrupoImpuestoArticuloCodigo: item.ImpuestoRentaGrupoImpuestoArticulo.Codigo,
							ImpuestoRentaGrupoImpuestoArticuloDescripcion: item.ImpuestoRentaGrupoImpuestoArticulo.Descripcion,
							ImpuestoRentaGrupoImpuestoArticuloCodigoDescripcion: item.ImpuestoRentaGrupoImpuestoArticulo.CodigoDescripcion,

							IvaGrupoImpuestoArticuloCodigo: item.IvaGrupoImpuestoArticulo.Codigo,
							IvaGrupoImpuestoArticuloDescripcion: item.IvaGrupoImpuestoArticulo.Descripcion,
                            IvaGrupoImpuestoArticuloCodigoDescripcion: item.IvaGrupoImpuestoArticulo.CodigoDescripcion,

                            PlantillaDistribucionDetalle: item.PlantillaDistribucionDetalle
						});
					});
                }
                console.error({
                    TareaId: $scope.TareaId,
                    Accion: $scope.Accion,
                    Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null),
                    InformacionContabilidadPago: InformacionContabilidadPago,
                    NoLiquidacion: NoLiquidacion,
                    NoAutorizacion: NoAutorizacion,
                    FechaEmision: FechaEmision,
                    FechaVencimiento: FechaVencimiento,
                    TipoPagoId: TipoPagoId,
                    Detalles: Detalles
                });

				$http.post($rootScope.pathURL + '/ContabilidadPago/Contabilizar', {
					TareaId: $scope.TareaId,
					Accion: $scope.Accion,
					Observacion: ($scope.Observacion ? $scope.Observacion.toUpperCase() : null),
					InformacionContabilidadPago: InformacionContabilidadPago,
                    NoLiquidacion: NoLiquidacion,
                    NoAutorizacion: NoAutorizacion,
                    FechaEmision: FechaEmision,
                    FechaVencimiento: FechaVencimiento,
                    TipoPagoId: TipoPagoId,
					Detalles: Detalles
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
                            $window.open($rootScope.pathURL + '/TareaPago/Index?mensaje=Elemento procesado correctamente.&numeroDiario=' + response.numeroDiario, '_self');
						}
					});
			}
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
                        $scope.Tarea = {
                            ...response.tarea,
                            CatalogoPago: $scope.Tarea.CatalogoPago
                        };

                        if ($scope.Tarea.InformacionContabilidadPago) {

                            if (!$scope.Metadatos.Diarios) {
                                $scope.Metadatos.Diarios = [];
                            }

                            $scope.Metadatos.Diarios.push({
                                Codigo: $scope.Tarea.InformacionContabilidadPago.DiarioCodigo,
                                Descripcion: $scope.Tarea.InformacionContabilidadPago.DiarioDescripcion
                            });

                            $scope.Tarea.TipoDiario = {
                                Codigo: $scope.Tarea.InformacionContabilidadPago.TipoDiarioCodigo,
                                Descripcion: $scope.Tarea.InformacionContabilidadPago.TipoDiarioDescripcion
                            };
                            $scope.Tarea.Diario = {
                                Codigo: $scope.Tarea.InformacionContabilidadPago.DiarioCodigo,
                                Descripcion: $scope.Tarea.InformacionContabilidadPago.DiarioDescripcion
                            };
                            $scope.Tarea.PerfilAsientoContable = {
                                Codigo: $scope.Tarea.InformacionContabilidadPago.PerfilAsientoContableCodigo,
                                Descripcion: $scope.Tarea.InformacionContabilidadPago.PerfilAsientoContableDescripcion
                            };
                            $scope.Tarea.Departamento = {
                                Codigo: $scope.Tarea.InformacionContabilidadPago.DepartamentoCodigo,
                                Descripcion: $scope.Tarea.InformacionContabilidadPago.DepartamentoDescripcion,
                                CodigoDescripcion: $scope.Tarea.InformacionContabilidadPago.DepartamentoCodigoDescripcion
                            };
                        }

                        if ($scope.DataAccion == 'show') {
                            $scope.TareaProcesada = true;

                            $scope.Accion = $scope.Tarea.Accion;
                            $scope.Observacion = $scope.Tarea.Observacion;
                        }
                    }
                });
        }

    }]);