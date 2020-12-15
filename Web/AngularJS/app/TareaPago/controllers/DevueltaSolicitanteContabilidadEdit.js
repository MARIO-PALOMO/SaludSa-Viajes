
/**
 * Gestiona la lógica de control de la vista DevueltaSolicitanteContabilidadEdit del módulo Tarea Pago.
 */
app_tarea_pago.controller('DevueltaSolicitanteContabilidadEdit', [
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
		
		$scope.Metadatos = {};
		$scope.Data = { };

		$scope.secFacturas = 1;

		$scope.FacturasEliminar = [];
		$scope.FacturasEliminarAdjunto = [];

		$scope.ObtenerMetadatos = function () {

			$scope.SolicitudId = $('#SolicitudId').val();
            $scope.TareaId = $('#TareaId').val();
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
							item.sec = $scope.secFacturas++;
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

		$scope.EditarFactura = function (Factura) {
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
						SolicitudId: $scope.Solicitud.Id,
                        DesdeDevolucionContabilidad: true,
                        BeneficiarioIdentificacion: $scope.Solicitud.BeneficiarioIdentificacion,
                        Accion: ($scope.DataAccion == 'show' ? 'detail' : undefined),
					}
				}
			});

			modalInstance.result.then(function (result) {

				angular.forEach($scope.Solicitud.Facturas, function (item) {
					if (result.sec == item.sec) {
						item.sec = result.sec;
						item.Id = result.Id;
						item.NoFactura = result.NoFactura;
						item.NoAutorizacion = result.NoAutorizacion;
						item.Concepto = result.Concepto;
						item.FechaEmision = result.FechaEmision;
						item.FechaVencimiento = result.FechaVencimiento;
						item.Total = result.Total;
						item.TipoPagoId = result.TipoPagoId;
						item.TipoPagoObj = result.TipoPagoObj;
						item.FacturaDetallesPago = result.FacturaDetallesPago;
						item.Tipo = result.Tipo;
						item.FacturaAdjunta = result.FacturaAdjunta;
						item.FacturaElectronica = result.FacturaElectronica;

						item.Modificado = true;
						item.FacturaElectronicaCambiada = result.FacturaElectronicaCambiada;
					}
				});

				$scope.CalcularMontoTotal();
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

		$scope.ValidarEnviar = function () {
			var errores = '';

			if (!$scope.Solicitud.NombreCorto) {
				errores = errores + '<li>El campo "Nombre corto" es obligatorio.</li>';
			}

			if (!$scope.Solicitud.EmpresaParaLaQueSeCompra || !$scope.Solicitud.EmpresaParaLaQueSeCompra.Codigo) {
				errores = errores + '<li>El campo "Empresa" es obligatorio.</li>';
			}

			if (!$scope.Solicitud.BeneficiarioIdentificacion || !$scope.Solicitud.BeneficiarioNombre) {
				errores = errores + '<li>Debe seleccionar un beneficiario.</li>';
			}

			if (!$scope.Solicitud.MontoTotal) {
				$scope.Solicitud.MontoTotal = parseFloat(0).toFixed(2);
			}

			if (!($scope.Solicitud.MontoTotal > 0)) {
				errores = errores + '<li>No ha entrado facturas.</li>';
			}

			if (!$scope.Solicitud.AprobacionJefeArea || !$scope.Solicitud.AprobacionJefeArea.Usuario) {
				errores = errores + '<li>El campo "Jefe de Área" es obligatorio.</li>';
			}
            
			if (parseFloat($scope.Solicitud.MontoTotal) >= 2500 && (!$scope.Solicitud.AprobacionSubgerenteArea || !$scope.Solicitud.AprobacionSubgerenteArea.Usuario)) {
				errores = errores + '<li>El campo "Subgerente de Área" es obligatorio.</li>';
			}
            
			if ((parseFloat($scope.Solicitud.MontoTotal) >= 5000 || ($scope.Solicitud.AprobacionSubgerenteArea && $scope.Solicitud.AprobacionSubgerenteArea.Usuario == 'noaplica_0123')) && (!$scope.Solicitud.AprobacionGerenteArea || !$scope.Solicitud.AprobacionGerenteArea.Usuario)) {
				errores = errores + '<li>El campo "Gerente de Área" es obligatorio.</li>';
			}
            
			if (parseFloat($scope.Solicitud.MontoTotal) >= 10000 && (!$scope.Solicitud.AprobacionVicePresidenteFinanciero || !$scope.Solicitud.AprobacionVicePresidenteFinanciero.Usuario)) {
				errores = errores + '<li>El campo "Vicepresidente Financiero" es obligatorio.</li>';
			}

			if (parseFloat($scope.Solicitud.MontoTotal) >= 120000 && (!$scope.Solicitud.AprobacionGerenteGeneral || !$scope.Solicitud.AprobacionGerenteGeneral.Usuario)) {
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
				var secTem = 1;

				var Facturas = [];
				var Files = [];

				var item = $scope.Solicitud.Facturas[0];

				if (item.Tipo == 'Física') {
					if (item.FacturaAdjunta && item.FacturaAdjunta.name) {
						var ext = item.FacturaAdjunta.name.match(/\.([^\.]+)$/);
						ext = ext ? ext[1].toLowerCase() : ext;

						Files.push({
							file: item.FacturaAdjunta,
							name: item.NoFactura + '-' + secTem + '.' + ext
						});

						if (item.Id) {
							$scope.FacturasEliminarAdjunto.push(item.Id);
						}
					}
						
				}

				var FacturaDetallesPago = [];

				angular.forEach(item.FacturaDetallesPago, function (item2) {
					var Distribuciones = [];

					if (item2.PlantillaDistribucionDetalle) {
						angular.forEach(item2.PlantillaDistribucionDetalle, function (item3) {
							Distribuciones.push({
								Id: 0,
								Porcentaje: item3.Porcentaje ? parseFloat(item3.Porcentaje) : parseFloat(0),
								DepartamentoCodigo: item3.DepartamentoCodigo,
								DepartamentoDescripcion: item3.DepartamentoDescripcion,
								DepartamentoCodigoDescripcion: item3.DepartamentoCodigoDescripcion,
								CentroCostoCodigo: item3.CentroCostoCodigo,
								CentroCostoDescripcion: item3.CentroCostoDescripcion,
								CentroCostoCodigoDescripcion: item3.CentroCostoCodigoDescripcion,
								PropositoCodigo: item3.PropositoCodigo,
								PropositoDescripcion: item3.PropositoDescripcion,
								PropositoCodigoDescripcion: item3.PropositoCodigoDescripcion,
								EstadoId: item3.EstadoId
							});
						});
					}

					var NuevoDetalle = {
						Id: item2.Id,
						Descripcion: item2.Descripcion,
						Valor: item2.Valor,
						Subtotal: item2.Subtotal,
						ImpuestoPagoId: item2.ImpuestoPagoId,
						Distribuciones: Distribuciones
					};

					FacturaDetallesPago.push(NuevoDetalle);
				});

				var NuevaFactura = {
					AdjuntoName: item.NoFactura + '-' + secTem + '.' + ext,
					Id: item.Id,
					NoFactura: item.NoFactura,
					NoAutorizacion: item.NoAutorizacion,
					Concepto: item.Concepto,
					FechaEmision: item.FechaEmision,
					FechaVencimiento: item.FechaVencimiento,
					Total: item.Total,
					Tipo: item.Tipo,
					TipoPagoId: item.TipoPagoId,
					FacturaDetallesPago: FacturaDetallesPago
				};

				if (NuevaFactura.Tipo == 'Electrónica') {
					NuevaFactura.ComprobanteElectronico = item.FacturaElectronica;

					NuevaFactura.ComprobanteElectronico.EstadoId = 1;
					NuevaFactura.ComprobanteElectronico.fechaEmision = item.FechaEmision;

					if (item.Id && item.FacturaElectronicaCambiada) {
						$scope.FacturasEliminarAdjunto.push(item.Id);
					}
				}

				Facturas.push(NuevaFactura);
				secTem += 1;

				var AprobacionJefeArea = ($scope.Solicitud.AprobacionJefeArea ? ($scope.Solicitud.AprobacionJefeArea.Usuario ? $scope.Solicitud.AprobacionJefeArea.Usuario : null) : null);
				var AprobacionSubgerenteArea = null;
				var AprobacionGerenteArea = null;
				var AprobacionVicePresidenteFinanciero = null;
				var AprobacionGerenteGeneral = null;
                
				if ($scope.Solicitud.MontoTotal >= 2500) {
					AprobacionSubgerenteArea = ($scope.Solicitud.AprobacionSubgerenteArea ? ($scope.Solicitud.AprobacionSubgerenteArea.Usuario ? $scope.Solicitud.AprobacionSubgerenteArea.Usuario : null) : null);
				}
                
                if ($scope.Solicitud.MontoTotal >= 5000 || ($scope.Solicitud.AprobacionSubgerenteArea && $scope.Solicitud.AprobacionSubgerenteArea.Usuario == 'noaplica_0123')) {
					AprobacionGerenteArea = ($scope.Solicitud.AprobacionGerenteArea ? ($scope.Solicitud.AprobacionGerenteArea.Usuario ? $scope.Solicitud.AprobacionGerenteArea.Usuario : null) : null);
				}
                
				if ($scope.Solicitud.MontoTotal >= 10000) {
					AprobacionVicePresidenteFinanciero = ($scope.Solicitud.AprobacionVicePresidenteFinanciero ? ($scope.Solicitud.AprobacionVicePresidenteFinanciero.Usuario ? $scope.Solicitud.AprobacionVicePresidenteFinanciero.Usuario : null) : null);
				}

				if ($scope.Solicitud.MontoTotal >= 120000) {
					AprobacionGerenteGeneral = ($scope.Solicitud.AprobacionGerenteGeneral ? ($scope.Solicitud.AprobacionGerenteGeneral.Usuario ? $scope.Solicitud.AprobacionGerenteGeneral.Usuario : null) : null);
				}

				PersistenciaPago.EditDevolucionContabilidad(
					Files,
					Facturas[0],
					$scope.FacturasEliminarAdjunto,
					$scope.TareaId,
					AprobacionJefeArea,
					AprobacionSubgerenteArea,
					AprobacionGerenteArea,
					AprobacionVicePresidenteFinanciero,
					AprobacionGerenteGeneral)
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
							$window.open($rootScope.pathURL + '/TareaPago/Index?mensaje=Elemento procesado correctamente.', '_self');
						}
					});
			}
		}

    }]);