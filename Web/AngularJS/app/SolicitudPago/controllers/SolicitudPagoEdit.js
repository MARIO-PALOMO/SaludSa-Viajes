
/**
 * Gestiona la lógica de control de la vista edit del módulo Solicitud Pago.
 */
app_solicitud_pago.controller('SolicitudPagoEdit', [
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
		$scope.Data = { Accion: 'edit' };

		$scope.secFacturas = 1;

		$scope.FacturasEliminar = [];
		$scope.FacturasEliminarAdjunto = [];

		$scope.ObtenerMetadatos = function () {

			$scope.SolicitudId = $('#SolicitudId').val();
			$scope.Data.Accion = $('#Accion').val();

			$http.get($rootScope.pathURL + '/SolicitudPago/ObtenerMetadatos?SolicitudId=' + $scope.SolicitudId)
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
			$http.get($rootScope.pathURL + '/SolicitudPago/Detalle?Id=' + $scope.SolicitudId)
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

						angular.forEach($scope.Solicitud.Facturas, function (item) {
							item.sec = $scope.secFacturas++;
							item.FechaEmision = json_js_date.convertir(item.FechaEmision);
							item.FechaVencimiento = json_js_date.convertir(item.FechaVencimiento);
						});
					}
				});
		}

		$scope.BuscarBeneficiario = function () {
			var modalInstance = $uibModal.open({
				animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/SolicitudPago/views/buscar_beneficiario.html',
				controller: 'SolicitudPagoBuscarBeneficiario',
				backdrop: 'static',
				resolve: {
					data: {

						TituloPag: 'Buscar beneficiario',
						EmpresaCodigo: $scope.Solicitud.EmpresaParaLaQueSeCompra.Codigo
					}
				}
			});

			modalInstance.result.then(function (result) {
				$scope.Solicitud.BeneficiarioIdentificacion = result.Identificacion;
				$scope.Solicitud.BeneficiarioNombre = result.Nombre;
				$scope.Solicitud.BeneficiarioTipoIdentificacion = result.TipoIdentificacion;
			}, function () {

			});
		}

		$scope.AdicionarFacturaFisica = function () {
			var modalInstance = $uibModal.open({
				animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/SolicitudPago/views/factura_fisica.html',
				controller: 'SolicitudPagoFacturaFisica',
				backdrop: 'static',
				size: 'lg',
				resolve: {
					data: {
						TituloPag: 'Factura física',
						EmpresaParaLaQueSeCompra: $scope.Solicitud.EmpresaParaLaQueSeCompra,
						TipoFactura: 'Física',
						EsReembolso: $scope.Solicitud.Facturas[0] ? ($scope.Solicitud.Facturas[0].TipoPagoObj.EsReembolso) : null,
                        SolicitudId: $scope.Solicitud.Id,
                        BeneficiarioIdentificacion: $scope.Solicitud.BeneficiarioIdentificacion
					}
				}
			});

			modalInstance.result.then(function (result) {
				result.sec = $scope.secFacturas++;
				$scope.Solicitud.Facturas.push(result);
				$scope.CalcularMontoTotal();
			}, function () {

			});
		}

		$scope.AdicionarFacturaElectronica = function () {
			var modalInstance = $uibModal.open({
				animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/SolicitudPago/views/BuscarFacturasElectronicas.html',
				controller: 'BuscarFacturasElectronicas',
				backdrop: 'static',
				size: 'small',
				resolve: {
					data: {
						TituloPag: 'Buscar facturas electrónicas',
						EmpresaParaLaQueSeCompra: $scope.Solicitud.EmpresaParaLaQueSeCompra,
						TipoFactura: 'Electrónica'
					}
				}
			});

			modalInstance.result.then(function (result) {
				/******/
				var modalInstance = $uibModal.open({
					animation: true,
					templateUrl: $rootScope.pathURL + '/AngularJS/app/SolicitudPago/views/factura_fisica.html',
					controller: 'SolicitudPagoFacturaFisica',
					backdrop: 'static',
					size: 'lg',
					resolve: {
						data: {
							TituloPag: 'Factura electrónica',
							EmpresaParaLaQueSeCompra: $scope.Solicitud.EmpresaParaLaQueSeCompra,
							TipoFactura: 'Electrónica',
							FacturaElectronica: result,
							EsReembolso: $scope.Solicitud.Facturas[0] ? ($scope.Solicitud.Facturas[0].TipoPagoObj.EsReembolso) : null,
                            SolicitudId: $scope.Solicitud.Id,
                            BeneficiarioIdentificacion: $scope.Solicitud.BeneficiarioIdentificacion
						}
					}
				});

				modalInstance.result.then(function (result2) {
					result2.sec = $scope.secFacturas++;
					$scope.Solicitud.Facturas.push(result2);
					$scope.CalcularMontoTotal();
				}, function () {

				});
				/******/
			}, function () {

			});
		}

		$scope.CalcularMontoTotal = function () {
			$scope.Solicitud.MontoTotal = 0;

			angular.forEach($scope.Solicitud.Facturas, function (item) {
				$scope.Solicitud.MontoTotal = parseFloat($scope.Solicitud.MontoTotal) + parseFloat(item.Total);
			});

			$scope.Solicitud.MontoTotal = parseFloat($scope.Solicitud.MontoTotal).toFixed(2);
		}

		$scope.EliminarFactura = function (Factura) {

			if (Factura.Id) {
				$scope.FacturasEliminar.push(Factura.Id);
			}

			$scope.Solicitud.Facturas = $scope.Solicitud.Facturas.filter(function (item) {
				return item.sec != Factura.sec;
			});

			$scope.CalcularMontoTotal();
		}

		$scope.CambiarEmpresaParaLaQueSeCompra = function () {

			angular.forEach($scope.Solicitud.Facturas, function (item) {
				if (item.Id) {
					$scope.FacturasEliminar.push(item.Id);
				}
			});

			$scope.Solicitud.Facturas = [];
			$scope.Solicitud.MontoTotal = '0.00';

			$scope.secFacturas = 1;

			$scope.Solicitud.BeneficiarioIdentificacion = '';
			$scope.Solicitud.BeneficiarioNombre = '';
			$scope.Solicitud.BeneficiarioTipoIdentificacion = '';
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
                        BeneficiarioIdentificacion: $scope.Solicitud.BeneficiarioIdentificacion
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
			if (Factura.Tipo == 'Física' && Factura.Id) {
				$window.open($rootScope.pathURL + '/SolicitudPago/DescargarAdjunto?SolicitudId=' + $scope.Solicitud.Id + '&FacturaId=' + Factura.Id + '&NoFactura=' + Factura.NoFactura, '_blank');
			}
			else {
				$window.open($scope.Metadatos.UrlVisorRidePdf + Factura.FacturaElectronica.claveAcceso, '_blank');
			}
		}

		$scope.ValidarGuardar = function () {
			var errores = '';

			if (!$scope.Solicitud.NombreCorto) {
				errores = errores + '<li>El campo "Nombre corto" es obligatorio.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
				return false;
			}

			return true;
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

		$scope.Crear = function (accion) {

			var Validacion = false;

			if (accion == 1) { // GUARDAR
				Validacion = $scope.ValidarGuardar();
			}
			else if (accion == 2) { // ENVIAR
				Validacion = $scope.ValidarEnviar();
			}

			if (Validacion) {
				var secTem = 1;

				var Facturas = [];
				var Files = [];

				angular.forEach($scope.Solicitud.Facturas, function (item) {

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
				});

				var Cabecera = {
					Id: $scope.Solicitud.Id,
					NumeroSolicitud: accion == 1 ? null : -1,
					FechaSolicitud: null,
					SolicitanteUsuario: $scope.Solicitud.SolicitanteObj.Usuario,
					SolicitanteNombreCompleto: $scope.Solicitud.SolicitanteObj.NombreCompleto,
					SolicitanteCiudadCodigo: $scope.Solicitud.SolicitanteObj.CiudadCodigo,
					EmpresaCodigo: $scope.Solicitud.EmpresaParaLaQueSeCompra.Codigo,
					EmpresaNombre: $scope.Solicitud.EmpresaParaLaQueSeCompra.Nombre,
					NombreCorto: $scope.Solicitud.NombreCorto ? $scope.Solicitud.NombreCorto.toUpperCase() : null,
					Observacion: $scope.Solicitud.Observacion ? $scope.Solicitud.Observacion.toUpperCase() : null,
					BeneficiarioIdentificacion: $scope.Solicitud.BeneficiarioIdentificacion,
					BeneficiarioTipoIdentificacion: $scope.Solicitud.BeneficiarioTipoIdentificacion,
					BeneficiarioNombre: $scope.Solicitud.BeneficiarioNombre,
					MontoTotal: $scope.Solicitud.MontoTotal,
					AprobacionJefeArea: $scope.Solicitud.AprobacionJefeArea ? ($scope.Solicitud.AprobacionJefeArea.Usuario ? $scope.Solicitud.AprobacionJefeArea.Usuario : null) : null,
					AprobacionSubgerenteArea: $scope.Solicitud.AprobacionSubgerenteArea ? ($scope.Solicitud.AprobacionSubgerenteArea.Usuario ? $scope.Solicitud.AprobacionSubgerenteArea.Usuario : null) : null,
					AprobacionGerenteArea: $scope.Solicitud.AprobacionGerenteArea ? ($scope.Solicitud.AprobacionGerenteArea.Usuario ? $scope.Solicitud.AprobacionGerenteArea.Usuario : null) : null,
					AprobacionVicePresidenteFinanciero: $scope.Solicitud.AprobacionVicePresidenteFinanciero ? ($scope.Solicitud.AprobacionVicePresidenteFinanciero.Usuario ? $scope.Solicitud.AprobacionVicePresidenteFinanciero.Usuario : null) : null,
					AprobacionGerenteGeneral: $scope.Solicitud.AprobacionGerenteGeneral ? ($scope.Solicitud.AprobacionGerenteGeneral.Usuario ? $scope.Solicitud.AprobacionGerenteGeneral.Usuario : null) : null,
					//JsonOriginal: ,
					EstadoId: 1,
					Facturas: Facturas
				};

				PersistenciaPago.Edit(Files, Cabecera, $scope.FacturasEliminar, $scope.FacturasEliminarAdjunto)
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
							$scope.Data.Accion = 'detail';
							$scope.ObtenerSolicitud($scope.Solicitud.Id);

							if (response.data.NumeroSolicitud) {
								toastr['success']("Elemento procesado correctamente. <br> <b>Número de solicitud: " + response.data.NumeroSolicitud + "</b>", "Confirmación");
							}
							else {
								toastr['success']("Elemento procesado correctamente.", "Confirmación");
							}
						}
					});
			}
		}

    }]);