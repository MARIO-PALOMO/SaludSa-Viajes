
/**
 * Gestiona la lógica de control de la vista CreacionSolicitudEdit del módulo Tarea Pago.
 */
app_tarea_pago.controller('CreacionSolicitudPagoEdit', [
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
						$scope.Data.Accion = $('#Accion').val();

						$scope.Solicitud = response.cabecera;
						$scope.TiposPago = response.TiposPago;

						$scope.Solicitud.FechaSolicitud = json_js_datetime.convertir($scope.Solicitud.FechaSolicitud);

						angular.forEach($scope.Solicitud.Facturas, function (item) {
							item.Pk = $scope.secFacturas++;
							item.FechaEmision = json_js_date.convertir(item.FechaEmision);
							item.FechaVencimiento = json_js_date.convertir(item.FechaVencimiento);
						});

						if ($scope.Solicitud.JsonOriginal) {
							$scope.JsonOriginal = JSON.parse($scope.Solicitud.JsonOriginal);
							
							$scope.ObtenerMetadatosFactura($scope.JsonOriginal.EmpresaCodigo);
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
						SolicitudId: $scope.Solicitud.Id,
						ShowEstadoCreacion: true
					}
				}
			});

			modalInstance.result.then(function (result) {

			}, function () {

			});
		}

		$scope.CargarJsonOriginal = function () {
			$scope.Solicitud.NombreCorto = $scope.JsonOriginal.NombreCorto;
			$scope.Solicitud.Observacion = $scope.JsonOriginal.Observacion;
			$scope.Solicitud.BeneficiarioIdentificacion = $scope.JsonOriginal.BeneficiarioIdentificacion;
			$scope.Solicitud.BeneficiarioTipoIdentificacion = $scope.JsonOriginal.BeneficiarioTipoIdentificacion;
			$scope.Solicitud.BeneficiarioNombre = $scope.JsonOriginal.BeneficiarioNombre;
			$scope.Solicitud.MontoTotal = $scope.JsonOriginal.MontoTotal;

			$scope.Solicitud.Facturas = [];
			
			angular.forEach($scope.JsonOriginal.Facturas, function (item) {

				var tem = $scope.MetadatosFactura.TiposPago.filter(function (item2) {
					return item.TipoPagoId == item2.Id;
				});

				item.TipoPagoObj = tem[0];

				angular.forEach(item.FacturaDetallesPago, function (item3) {

					var tem = $scope.MetadatosFactura.ImpuestosPago.filter(function (item4) {
						return item3.ImpuestoPagoId == item4.Id;
					});

					item3.ImpuestoPagoObj = tem[0];
				});

				$scope.Solicitud.Facturas.push(item);
			});

			if ($scope.JsonOriginal.AprobacionJefeArea) {
				var tem = $scope.Metadatos.JefesAreas.filter(function (item) {
					return item.Usuario == $scope.JsonOriginal.AprobacionJefeArea;
				});

				$scope.Solicitud.AprobacionJefeArea = tem[0];
			}

			if ($scope.JsonOriginal.AprobacionSubgerenteArea) {
				var tem = $scope.Metadatos.SubgerentesAreas.filter(function (item) {
					return item.Usuario == $scope.JsonOriginal.AprobacionSubgerenteArea;
				});

				$scope.Solicitud.AprobacionSubgerenteArea = tem[0];
			}

			if ($scope.JsonOriginal.AprobacionGerenteArea) {
				var tem = $scope.Metadatos.GerentesAreas.filter(function (item) {
					return item.Usuario == $scope.JsonOriginal.AprobacionGerenteArea;
				});

				$scope.Solicitud.AprobacionGerenteArea = tem[0];
			}

			if ($scope.JsonOriginal.AprobacionVicePresidenteFinanciero) {
				var tem = $scope.Metadatos.VicepresidentesFinancieros.filter(function (item) {
					return item.Usuario == $scope.JsonOriginal.AprobacionVicePresidenteFinanciero;
				});

				$scope.Solicitud.AprobacionVicePresidenteFinanciero = tem[0];
			}

			if ($scope.JsonOriginal.AprobacionGerenteGeneral) {
				var tem = $scope.Metadatos.GerentesGenerales.filter(function (item) {
					return item.Usuario == $scope.JsonOriginal.AprobacionGerenteGeneral;
				});

				$scope.Solicitud.AprobacionGerenteGeneral = tem[0];
			}
		}

		$scope.ObtenerMetadatosFactura = function (EmpresaCodigo, ) {
			$http.get($rootScope.pathURL + '/SolicitudPago/ObtenerMetadatosFactura?EmpresaCodigo=' + EmpresaCodigo)
				.success(function (response) {

					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.MetadatosFactura = response;
						$scope.CargarJsonOriginal();
					}
				});
		}

    }]);