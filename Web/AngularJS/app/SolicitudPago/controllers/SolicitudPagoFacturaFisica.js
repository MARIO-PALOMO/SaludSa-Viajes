
/**
 * Gestiona la lógica de control de la vista factura_fisica del módulo Solicitud Pago.
 */
app_solicitud_pago.controller('SolicitudPagoFacturaFisica', [
    '$scope',
    '$rootScope',
	'$uibModal',
	'$uibModalInstance',
	'$http',
	'data',
	'$window',
	function ($scope, $rootScope, $uibModal, $uibModalInstance, $http, data, $window) {

		$scope.Data = data;
		$scope.Factura = {};
		$scope.Factura.Total = '0.00';
		$scope.Factura.Tipo = $scope.Data.TipoFactura;

		$scope.Factura.Detalles = [];
		$scope.Factura.FacturaAdjunta = {};
		
		$scope.Metadatos = {};
		$scope.MetadatosContabilizacionFactura = {};

		$scope.DetallesPk = 1;

		$scope.SumaSubtotales = '0.00';

		$scope.Ok = function (result) {
			$uibModalInstance.close(result);
		};

		$scope.Cerrar = function () {
			$uibModalInstance.dismiss();
		};

		$scope.ObtenerMetadatos = function () {
            $http.get($rootScope.pathURL + '/SolicitudPago/ObtenerMetadatosFactura?EmpresaCodigo=' + $scope.Data.EmpresaParaLaQueSeCompra.Codigo + '&EsReembolso=' + ($scope.Data.DesdeContabilizacion ? null : $scope.Data.EsReembolso))
				.success(function (response) {

					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Metadatos = response;

						$scope.Factura.TipoPago = {};
						
						if ($scope.Data.Factura) {
							$scope.CargarFactura();
						}
						else {
							$scope.AdicionarDetalle();
						}

						if ($scope.Data.FacturaElectronica) {
							$scope.CargarFacturaElectronica();
						}
					}
				});
		}
		$scope.ObtenerMetadatos();

		$scope.AdicionarDetalle = function () {
			$scope.Factura.Detalles.push({
				Pk: $scope.DetallesPk++,
				Descripcion: '',
				Valor: '0.00',
				Subtotal: '0.00',
				ImpuestoPago: {}
			});
		}

		$scope.EliminarDetalle = function (Detalle) {
			$scope.Factura.Detalles = $scope.Factura.Detalles.filter(function (item) {
				return item.Pk != Detalle.Pk;
			});

			$scope.SumarSubtotales();
		}

		$scope.CalcularSubtotal = function (Detalle) {
			if (Detalle.Valor && Detalle.ImpuestoPago && Detalle.ImpuestoPago.Id) {
				if (parseFloat(Detalle.ImpuestoPago.Porcentaje).toFixed(2) == parseFloat(0).toFixed(2)) {
					Detalle.Subtotal = parseFloat(Detalle.Valor).toFixed(2);
				}
				else {
					Detalle.Subtotal = (parseFloat(Detalle.Valor) + (parseFloat(Detalle.Valor) * parseFloat(Detalle.ImpuestoPago.Porcentaje) / 100)).toFixed(2);
				}
			}
			else {
				Detalle.Subtotal = '0.00';
			}

			$scope.SumarSubtotales();
		}

		$scope.SumarSubtotales = function () {

			$scope.SumaSubtotales = 0;

			angular.forEach($scope.Factura.Detalles, function (item) {
				if (!item.Subtotal) {
					item.Subtotal = '0.00';
				}

				$scope.SumaSubtotales = parseFloat($scope.SumaSubtotales) + parseFloat(item.Subtotal);
			});

			$scope.SumaSubtotales = parseFloat($scope.SumaSubtotales).toFixed(2);
		}

		$scope.AdicionarDistribucion = function (Detalle) {

			if ($scope.Data.ShowEstadoCreacion) {
				$scope.ShowDistribucionEstadoCreacion(Detalle);
			}
			else {
				var Lineas = [];

				angular.forEach($scope.Factura.Detalles, function (item, index) {
					if (item.PlantillaDistribucionDetalle) {
						var nueva = {
							Id: item.Pk,
							Descripcion: 'Detalle ' + (parseInt(index) + 1) + (item.Descripcion ? (' (' + item.Descripcion.toUpperCase() + ')') : ''),
							Detalles: item.PlantillaDistribucionDetalle
						};

						Lineas.push(nueva);
					}
				});

				if (Detalle.PlantillaDistribucionDetalle) {
					var modalInstance = $uibModal.open({
						animation: true,
						templateUrl: $rootScope.pathURL + '/AngularJS/app/PlantillaDistribucion/views/form.html',
						controller: 'PlantillaDistribucionCreate',
						backdrop: 'static',
						size: 'lg',
						resolve: {
							data: {
								TituloPag: 'Adicionar distribución',
                                Accion: (($scope.Data.Accion == 'detail' && !$scope.Data.DesdeContabilizacion) || $scope.Data.Accion == 'show') ? 'detail' : 'create',
								DesdeSolicitud: true,
								Detalles: Detalle.PlantillaDistribucionDetalle,
								TipoProducto: '1',
								Lineas: Lineas,
								Empresa: $scope.Data.EmpresaParaLaQueSeCompra
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
								TituloPag: 'Adicionar distribución',
                                Accion: (($scope.Data.Accion == 'detail' && !$scope.Data.DesdeContabilizacion) || $scope.Data.Accion == 'show') ? 'detail' : 'create',
								DesdeSolicitud: true,
								TipoProducto: '1',
								Lineas: Lineas,
								Empresa: $scope.Data.EmpresaParaLaQueSeCompra
							}
						}
					});

					modalInstance.result.then(function (result) {
						Detalle.PlantillaDistribucionDetalle = result;
					}, function () {

					});
				}
			}
		}

		$scope.DescargarAdjuntoFactura = function () {
			if ($scope.Factura.Id && parseInt($scope.Factura.Id) > 0) {
				$window.open($rootScope.pathURL + '/SolicitudPago/DescargarAdjunto?SolicitudId=' + $scope.Data.SolicitudId + '&FacturaId=' + $scope.Factura.Id + '&NoFactura=' + $scope.Factura.NoFactura, '_blank');
			}
		}

		$scope.Validar = function () {
			var errores = '';

			if (!$scope.Factura.NoFactura) {
				errores = errores + '<li>No ha entrado un número de factura.</li>';
			}
			else {
				if ($scope.Factura.NoFactura.length != 15) {
					errores = errores + '<li>El número de factura entrado no es válido.</li>';
				}
			}

			if (!$scope.Factura.NoAutorizacion) {
				errores = errores + '<li>No ha entrado un número de autorización.</li>';
			}
			else {
				if ($scope.Data.TipoFactura == 'Física') {
					if ($scope.Factura.NoAutorizacion.length != 10) {
						errores = errores + '<li>El número de autorización entrado no es válido.</li>';
					}
				}
			}

			if (!$scope.Factura.TipoPago || !$scope.Factura.TipoPago.Id) {
				errores = errores + '<li>No ha seleccionado un tipo de pago.</li>';
			}

			if (!$scope.Factura.FechaEmision) {
				errores = errores + '<li>No ha seleccionado una fecha de emisión.</li>';
			}
			else {
                if ($scope.Factura.TipoPago && !$scope.Factura.TipoPago.EsReembolso) {
					var fechaActual = new Date();

					var fechaEmisionArr = $scope.Factura.FechaEmision.split('/');
					
					if (fechaActual.getFullYear() != fechaEmisionArr[2] || ("0" + (fechaActual.getMonth() + 1)).slice(-2) != fechaEmisionArr[1]) {
						errores = errores + '<li>La fecha de emisión no puede estar fuera del mes actual.</li>';
					}
				}
			}

			if (!$scope.Factura.FechaVencimiento) {
				errores = errores + '<li>No ha seleccionado una fecha de vencimiento.</li>';
			}

			if ($scope.Factura.Tipo == 'Física' && !$scope.Factura.Id && (!$scope.Factura.FacturaAdjunta || !$scope.Factura.FacturaAdjunta.name)) {
				errores = errores + '<li>No ha subido un archivo.</li>';
			}

			if (!$scope.Factura.Concepto) {
				errores = errores + '<li>No ha entrado un concepto.</li>';
			}

			if (!$scope.Factura.Total) {
				$scope.Factura.Total = '0.00';
			}

			if (!(parseFloat($scope.Factura.Total) > 0)) {
				errores = errores + '<li>El campo "Total" debe tener un valor numérico mayor que cero.</li>';
			}

			if (!$scope.Factura.Detalles || $scope.Factura.Detalles.length == 0) {
				errores = errores + '<li>No ha entrado ningún detalle.</li>';
			}
			else {

				var ErrorDescripcion = false;
				var ErrorValor = false;
				var ErrorImpuesto = false;

				var ErrorDistribucionNoExiste = false;

				var ErrorDistribucionDepartamento = false;
				var ErrorDistribucionCentroCosto = false;
				var ErrorDistribucionProposito = false;
				var ErrorDistribucionPorcentaje = false;
				var ErrorDistribucionPorcentajeSumaFinal = false;

				angular.forEach($scope.Factura.Detalles, function (item) {

					if (!item.Descripcion) {
						ErrorDescripcion = true;
					}

					if (!item.Valor) {
						item.Valor = '0.00';
					}

					if (!(parseFloat(item.Valor) > 0)) {
						ErrorValor = true;
					}

					if (!item.ImpuestoPago || !item.ImpuestoPago.Id) {
						ErrorImpuesto = true;
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

				if (ErrorDescripcion) {
					errores = errores + '<li>Se han identificado errores en los detalles en la columna "Descripción".</li>';
				}

				if (ErrorValor) {
					errores = errores + '<li>Se han identificado errores en los detalles en la columna "Valor".</li>';
				}

				if (ErrorImpuesto) {
					errores = errores + '<li>Se han identificado errores en los detalles en la columna "Porcentaje".</li>';
				}

				if (ErrorDistribucionNoExiste) {
					errores = errores + '<li>Existen detalles sin una distribución asignada.</li>';
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
			}

			if (parseFloat($scope.Factura.Total) != parseFloat($scope.SumaSubtotales)) {
				errores = errores + '<li>El total entrado es distinto de la suma de los subtotales de los detalles.</li>';
            }

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
        }

		$scope.Crear = function () {
            if ($scope.Validar()) {
                if (!$scope.Factura.TipoPago.EsReembolso && $scope.Data.BeneficiarioIdentificacion) {
                    $http.get($rootScope.pathURL + '/SolicitudPago/VerificarSiExisteFactura?NoFactura=' + $scope.Factura.NoFactura + '&RUC=' + $scope.Data.BeneficiarioIdentificacion + '&SolicitudId=' + $scope.Data.SolicitudId)
                        .success(function (response) {
                            var errores = '';

                            angular.forEach(response.validacion, function (value) {
                                errores = errores + '<li>' + value + '</li>';
                            });

                            if (errores.length > 1) {
                                bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
                            }
                            else {
                                $scope.ConstruirRespuesta();
                            }
                        });
                }
                else {
                    $scope.ConstruirRespuesta();
                }
			}
        }

        $scope.ConstruirRespuesta = function () {
            var detalles = [];

            angular.forEach($scope.Factura.Detalles, function (item) {
                var nuevo = {
                    Id: 0,
                    Descripcion: item.Descripcion ? item.Descripcion.toUpperCase() : null,
                    Valor: item.Valor ? parseFloat(item.Valor) : parseFloat(0),
                    Subtotal: item.Subtotal ? parseFloat(item.Subtotal) : parseFloat(0),
                    ImpuestoPagoId: item.ImpuestoPago ? (item.ImpuestoPago.Id ? item.ImpuestoPago.Id : null) : null,
                    ImpuestoPagoObj: item.ImpuestoPago ? item.ImpuestoPago : null,
                    PlantillaDistribucionDetalle: item.PlantillaDistribucionDetalle
                };

                detalles.push(nuevo);
            });

            var factura = {
                Id: 0,
                NoFactura: $scope.Factura.NoFactura,
                NoAutorizacion: $scope.Factura.NoAutorizacion,
                Concepto: $scope.Factura.Concepto ? $scope.Factura.Concepto.toUpperCase() : null,
                FechaEmision: $scope.Factura.FechaEmision,
                FechaVencimiento: $scope.Factura.FechaVencimiento,
                Total: $scope.Factura.Total ? parseFloat($scope.Factura.Total) : parseFloat(0),
                TipoPagoId: $scope.Factura.TipoPago ? ($scope.Factura.TipoPago.Id ? $scope.Factura.TipoPago.Id : null) : null,
                TipoPagoObj: $scope.Factura.TipoPago ? $scope.Factura.TipoPago : null,
                FacturaDetallesPago: detalles,
                Tipo: $scope.Factura.Tipo,

                FacturaAdjunta: $scope.Factura.FacturaAdjunta,

                FacturaElectronicaCambiada: $scope.Factura.FacturaElectronicaCambiada ? true : false
            };

            if ($scope.Data.Factura) {
                factura.sec = $scope.Data.Factura.sec;
                factura.Id = $scope.Data.Factura.Id;
            }

            if ($scope.Factura.FacturaElectronica) {
                factura.FacturaElectronica = $scope.Factura.FacturaElectronica;
            }

            $scope.Ok(factura);
        }

		$scope.CargarFactura = function () {
			$scope.Factura.Id = $scope.Data.Factura.Id;
			$scope.Factura.NoFactura = $scope.Data.Factura.NoFactura;
			$scope.Factura.NoAutorizacion = $scope.Data.Factura.NoAutorizacion;
			$scope.Factura.Concepto = $scope.Data.Factura.Concepto;
			$scope.Factura.FechaEmision = $scope.Data.Factura.FechaEmision;
			$scope.Factura.FechaVencimiento = $scope.Data.Factura.FechaVencimiento;
			$scope.Factura.Total = parseFloat($scope.Data.Factura.Total).toFixed(2);
			$scope.Factura.TipoPago = $scope.Data.Factura.TipoPagoObj;
			$scope.Factura.Tipo = $scope.Data.Factura.Tipo;

			$scope.Factura.FacturaElectronica = $scope.Data.Factura.FacturaElectronica;

            $scope.Factura.FacturaAdjunta = $scope.Data.Factura.FacturaAdjunta;

            $scope.Factura.NoLiquidacion = $scope.Data.Factura.NoLiquidacion;

			angular.forEach($scope.Data.Factura.FacturaDetallesPago, function (item) {

				var nuevo = {};

				if ($scope.Data.ShowEstadoCreacion) {
					nuevo = {
						Pk: $scope.DetallesPk++,
						Descripcion: item.Descripcion,
						Valor: parseFloat(item.Valor).toFixed(2),
						Subtotal: parseFloat(item.Subtotal).toFixed(2),
						ImpuestoPago: item.ImpuestoPagoObj,
						Distribuciones: item.Distribuciones,
						PlantillaDistribucionDetalle: [{}]
					};
				}
				else {
					nuevo = {
						Pk: $scope.DetallesPk++,
						Descripcion: item.Descripcion,
						Valor: parseFloat(item.Valor).toFixed(2),
						Subtotal: parseFloat(item.Subtotal).toFixed(2),
						ImpuestoPago: item.ImpuestoPagoObj,
						PlantillaDistribucionDetalle: item.PlantillaDistribucionDetalle
					};

					if ($scope.Data.DesdeContabilizacion) {
						nuevo.GrupoImpuestoTem = item.GrupoImpuesto;
						nuevo.GrupoImpuestoArticuloTem = item.GrupoImpuestoArticulo;
						nuevo.SustentoTributarioTem = item.SustentoTributario;
						nuevo.ImpuestoRentaGrupoImpuestoArticuloTem = item.ImpuestoRentaGrupoImpuestoArticulo;
                        nuevo.IvaGrupoImpuestoArticuloTem = item.IvaGrupoImpuestoArticulo;
					}
				}

				$scope.Factura.Detalles.push(nuevo);
			});

			$scope.SumarSubtotales();

			if ($scope.Data.DesdeContabilizacion) {
				$scope.ObtenerMetadatosContabilizacionFactura();
			}
		}

		$scope.CargarFacturaElectronica = function () {
			$http.get($rootScope.pathURL + '/ComprobanteElectronico/ObtenerFechaEmision?claveAcceso=' + $scope.Data.FacturaElectronica.claveAcceso)
				.success(function (response) {

					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
                    } else {
                        var fechaEmisionArray = response.fechaEmision.split('/');

						$scope.Factura.NoFactura = $scope.Data.FacturaElectronica.establecimiento + $scope.Data.FacturaElectronica.puntoEmision + $scope.Data.FacturaElectronica.secuencial;
						$scope.Factura.NoAutorizacion = $scope.Data.FacturaElectronica.numeroAutorizacion;
						$scope.Factura.FechaEmision = response.fechaEmision;
                        $scope.Factura.FechaVencimiento = '31/12/' + fechaEmisionArray[2];
						$scope.Factura.Total = parseFloat($scope.Data.FacturaElectronica.valorTotal).toFixed(2);

						$scope.Factura.FacturaElectronica = $scope.Data.FacturaElectronica;
					}
				});
		}

		$scope.ShowDistribucionEstadoCreacion = function (Detalle) {
			
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
						Empresa: $scope.Data.EmpresaParaLaQueSeCompra.Codigo
					}
				}
			});

			modalInstance.result.then(function (result) {

			}, function () {

			});
		}

		$scope.CambiarFacturaElectronica = function () {
			var modalInstance = $uibModal.open({
				animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/SolicitudPago/views/BuscarFacturasElectronicas.html',
				controller: 'BuscarFacturasElectronicas',
				backdrop: 'static',
				size: 'small',
				resolve: {
					data: {
						TituloPag: 'Buscar facturas electrónicas',
						EmpresaParaLaQueSeCompra: $scope.Data.EmpresaParaLaQueSeCompra,
						TipoFactura: 'Electrónica'
					}
				}
			});

			modalInstance.result.then(function (result) {

				$scope.Factura.FacturaElectronicaCambiada = true;
				$scope.Factura.Detalles = [];
				$scope.DetallesPk = 1;
				$scope.AdicionarDetalle();
				$scope.SumarSubtotales();

				$scope.Data.FacturaElectronica = result;
				$scope.CargarFacturaElectronica();
			}, function () {

			});
		}

		$scope.ObtenerMetadatosContabilizacionFactura = function () {
			$http.get($rootScope.pathURL + '/ContabilidadPago/ObtenerMetadatosContabilizacionFactura?CompaniaCodigo=' + $scope.Data.EmpresaParaLaQueSeCompra.Codigo)
				.success(function (response) {

					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.MetadatosContabilizacionFactura = response;

						angular.forEach($scope.Factura.Detalles, function (item) {
							item.GrupoImpuesto = item.GrupoImpuestoTem ? item.GrupoImpuestoTem : {};
							item.GrupoImpuestoArticulo = item.GrupoImpuestoArticuloTem ? item.GrupoImpuestoArticuloTem : {};
                            item.SustentoTributario = item.SustentoTributarioTem ? item.SustentoTributarioTem : {};
                            
							if (item.GrupoImpuestoArticulo && item.GrupoImpuestoArticulo.Codigo) {
								$scope.ObtenerMetadatosContabilizacionFacturaComplementarios(item);
							}
						});
					}
				});
		}

		$scope.ObtenerMetadatosContabilizacionFacturaComplementarios = function (Detalle) {
			$http.get($rootScope.pathURL + '/ContabilidadPago/ObtenerMetadatosContabilizacionFacturaComplementarios?CompaniaCodigo=' + $scope.Data.EmpresaParaLaQueSeCompra.Codigo + '&codigoGrupoImpuestoArticulo=' + Detalle.GrupoImpuestoArticulo.Codigo)
				.success(function (response) {

					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						Detalle.MetadatosContabilizacionFacturaComplementarios = response;

						Detalle.ImpuestoRentaGrupoImpuestoArticulo = Detalle.ImpuestoRentaGrupoImpuestoArticuloTem ? Detalle.ImpuestoRentaGrupoImpuestoArticuloTem : {};
						Detalle.IvaGrupoImpuestoArticulo = Detalle.IvaGrupoImpuestoArticuloTem ? Detalle.IvaGrupoImpuestoArticuloTem : {};
					}
				});
		}

		$scope.ValidarParaContabilizar = function () {
			var errores = '';

			if ($scope.Data.DesdeContabilizacion && $scope.Factura.TipoPago.EsReembolso) {
				if (!$scope.Factura.NoLiquidacion) {
					errores = errores + '<li>No ha entrado un número de liquidación.</li>';
				}
				else {
					if ($scope.Factura.NoLiquidacion.length != 15) {
						errores = errores + '<li>El número de liquidación entrado no es válido.</li>';
					}
				}
            }

            if (!$scope.Factura.NoAutorizacion) {
                errores = errores + '<li>No ha entrado un número de autorización.</li>';
            }

            if (!$scope.Factura.FechaEmision) {
                errores = errores + '<li>No ha seleccionado una fecha de emisión.</li>';
            }

            if (!$scope.Factura.FechaVencimiento) {
                errores = errores + '<li>No ha seleccionado una fecha de vencimiento.</li>';
            }

			var errorGrupoImpuesto = false;
			var errorGrupoImpuestoArticulo = false;
			var errorSustentoTributario = false;
			var errorImpuestoRentaGrupoImpuestoArticulo = false;
            var errorIvaGrupoImpuestoArticulo = false;

            var ErrorDistribucionNoExiste = false;

            var ErrorDistribucionDepartamento = false;
            var ErrorDistribucionCentroCosto = false;
            var ErrorDistribucionProposito = false;
            var ErrorDistribucionPorcentaje = false;
            var ErrorDistribucionPorcentajeSumaFinal = false;

			angular.forEach($scope.Factura.Detalles, function (item) {
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

			if (errorGrupoImpuesto) {
				errores = errores + '<li>Se han detectado errores en la columna "Grupo impuesto".</li>';
			}

			if (errorGrupoImpuestoArticulo) {
				errores = errores + '<li>Se han detectado errores en la columna "Grupo impuesto artículo".</li>';
			}

			if (errorSustentoTributario) {
				errores = errores + '<li>Se han detectado errores en la columna "Sustento tributario".</li>';
			}

			if (errorImpuestoRentaGrupoImpuestoArticulo) {
				errores = errores + '<li>Se han detectado errores en la columna "Retención renta".</li>';
			}

			if (errorIvaGrupoImpuestoArticulo) {
				errores = errores + '<li>Se han detectado errores en la columna "Retención IVA".</li>';
            }

            if (ErrorDistribucionNoExiste) {
                errores = errores + '<li>Existen detalles sin una distribución asignada.</li>';
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

		$scope.GuardarParaContabilizar = function () {
			if ($scope.ValidarParaContabilizar()) {
				var detalles = [];

				angular.forEach($scope.Factura.Detalles, function (item) {
					detalles.push({
						GrupoImpuesto: item.GrupoImpuesto,
						GrupoImpuestoArticulo: item.GrupoImpuestoArticulo,
						SustentoTributario: item.SustentoTributario,
						ImpuestoRentaGrupoImpuestoArticulo: item.ImpuestoRentaGrupoImpuestoArticulo,
                        IvaGrupoImpuestoArticulo: item.IvaGrupoImpuestoArticulo,
                        PlantillaDistribucionDetalle: item.PlantillaDistribucionDetalle
					});
				});

                $scope.Ok({
                    NoLiquidacion: (($scope.Factura.TipoPago.EsReembolso) ? $scope.Factura.NoLiquidacion : null),
                    detalles: detalles,
                    NoAutorizacion: $scope.Factura.NoAutorizacion,
                    FechaEmision: $scope.Factura.FechaEmision,
                    FechaVencimiento: $scope.Factura.FechaVencimiento,

                    TipoPagoId: $scope.Factura.TipoPago ? ($scope.Factura.TipoPago.Id ? $scope.Factura.TipoPago.Id : null) : null,
                    TipoPagoObj: $scope.Factura.TipoPago ? $scope.Factura.TipoPago : null,
                });
			}
		}

        $scope.CopiarPrimeraLineaGrupoImpuesto = function () {
            if ($scope.Factura.Detalles.length > 1) {
                if ($scope.Factura.Detalles[0].GrupoImpuesto && $scope.Factura.Detalles[0].GrupoImpuesto.Codigo) {
                    for (var i = 1; i < $scope.Factura.Detalles.length; i++) {
                        $scope.Factura.Detalles[i].GrupoImpuesto = angular.copy($scope.Factura.Detalles[0].GrupoImpuesto);
                    }
                }
            }
        }

        $scope.CopiarPrimeraLineaGrupoImpuestoArticulo = function () {
            if ($scope.Factura.Detalles.length > 1) {
                if ($scope.Factura.Detalles[0].GrupoImpuestoArticulo && $scope.Factura.Detalles[0].GrupoImpuestoArticulo.Codigo) {
                    for (var i = 1; i < $scope.Factura.Detalles.length; i++) {
                        $scope.Factura.Detalles[i].GrupoImpuestoArticulo = angular.copy($scope.Factura.Detalles[0].GrupoImpuestoArticulo);

                        $scope.Factura.Detalles[i].MetadatosContabilizacionFacturaComplementarios = $scope.Factura.Detalles[0].MetadatosContabilizacionFacturaComplementarios;

                        $scope.Factura.Detalles[i].ImpuestoRentaGrupoImpuestoArticulo = $scope.Factura.Detalles[i].ImpuestoRentaGrupoImpuestoArticuloTem ? $scope.Factura.Detalles[i].ImpuestoRentaGrupoImpuestoArticuloTem : {};
                        $scope.Factura.Detalles[i].IvaGrupoImpuestoArticulo = $scope.Factura.Detalles[i].IvaGrupoImpuestoArticuloTem ? $scope.Factura.Detalles[i].IvaGrupoImpuestoArticuloTem : {};
                    }
                }
            }
        }

        $scope.CopiarPrimeraLineaRetencionRenta = function () {
            if ($scope.Factura.Detalles.length > 1) {
                if ($scope.Factura.Detalles[0].ImpuestoRentaGrupoImpuestoArticulo && $scope.Factura.Detalles[0].ImpuestoRentaGrupoImpuestoArticulo.Codigo) {
                    for (var i = 1; i < $scope.Factura.Detalles.length; i++) {
                        if ($scope.Factura.Detalles[i].MetadatosContabilizacionFacturaComplementarios && $scope.Factura.Detalles[i].MetadatosContabilizacionFacturaComplementarios.ImpuestoRentaGrupoImpuestosArticulosPago) {
                            var existe = false;
                            angular.forEach($scope.Factura.Detalles[i].MetadatosContabilizacionFacturaComplementarios.ImpuestoRentaGrupoImpuestosArticulosPago, function (item) {
                                if (item.Codigo == $scope.Factura.Detalles[0].ImpuestoRentaGrupoImpuestoArticulo.Codigo 
                                    && item.Descripcion == $scope.Factura.Detalles[0].ImpuestoRentaGrupoImpuestoArticulo.Descripcion 
                                    && item.CodigoDescripcion == $scope.Factura.Detalles[0].ImpuestoRentaGrupoImpuestoArticulo.CodigoDescripcion) {
                                    $scope.Factura.Detalles[i].ImpuestoRentaGrupoImpuestoArticulo = angular.copy($scope.Factura.Detalles[0].ImpuestoRentaGrupoImpuestoArticulo);
                                    existe = true;
                                }
                            });

                            if (!existe) {
                                $scope.Factura.Detalles[i].ImpuestoRentaGrupoImpuestoArticulo = {};
                            }
                        }
                    }
                }
            }
        }

        $scope.CopiarPrimeraLineaRetencionIVA = function () {
            if ($scope.Factura.Detalles.length > 1) {
                if ($scope.Factura.Detalles[0].IvaGrupoImpuestoArticulo && $scope.Factura.Detalles[0].IvaGrupoImpuestoArticulo.Codigo) {
                    for (var i = 1; i < $scope.Factura.Detalles.length; i++) {
                        if ($scope.Factura.Detalles[i].MetadatosContabilizacionFacturaComplementarios && $scope.Factura.Detalles[i].MetadatosContabilizacionFacturaComplementarios.IvaGrupoImpuestosArticulosPago) {
                            var existe = false;
                            angular.forEach($scope.Factura.Detalles[i].MetadatosContabilizacionFacturaComplementarios.IvaGrupoImpuestosArticulosPago, function (item) {
                                if (item.Codigo == $scope.Factura.Detalles[0].IvaGrupoImpuestoArticulo.Codigo
                                    && item.Descripcion == $scope.Factura.Detalles[0].IvaGrupoImpuestoArticulo.Descripcion
                                    && item.CodigoDescripcion == $scope.Factura.Detalles[0].IvaGrupoImpuestoArticulo.CodigoDescripcion) {
                                    $scope.Factura.Detalles[i].IvaGrupoImpuestoArticulo = angular.copy($scope.Factura.Detalles[0].IvaGrupoImpuestoArticulo);
                                    existe = true;
                                }
                            });

                            if (!existe) {
                                $scope.Factura.Detalles[i].IvaGrupoImpuestoArticulo = {};
                            }
                        }
                    }
                }
            }
        }

        $scope.CopiarPrimeraLineaSustentoTributario = function () {
            if ($scope.Factura.Detalles.length > 1) {
                if ($scope.Factura.Detalles[0].SustentoTributario && $scope.Factura.Detalles[0].SustentoTributario.Codigo) {
                    for (var i = 1; i < $scope.Factura.Detalles.length; i++) {
                        $scope.Factura.Detalles[i].SustentoTributario = angular.copy($scope.Factura.Detalles[0].SustentoTributario);
                    }
                }
            }
        }

    }]);