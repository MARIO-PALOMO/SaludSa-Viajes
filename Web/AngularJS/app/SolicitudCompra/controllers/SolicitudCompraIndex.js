
/**
 * Gestiona la lógica de control de la vista index del módulo Solicitud Compra.
 */
app_solicitud_compra.controller('SolicitudCompraIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
	'$http',
	'$window',
	function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http, $window) {

		$scope.solicitudes = [];
		$scope.MostrarSaldoCero = false;

        $scope.dtOptions = DTOptionsBuilder.newOptions()
            .withBootstrap()                               
            .withOption('order', [0, 'asc'])
            .withBootstrapOptions({
                pagination: {
                    classes: {
                        ul: 'pagination pagination-sm'     
                    }
                }
            })
            .withLanguage($rootScope.datatable_traduccion_es)
            .withColumnFilter({
				aoColumns: [
					{ type: 'text' },
                    { type: 'text' },
                    { type: 'text' },
                    { type: 'text' },
                    null
                ]
            });

        $scope.dtColumnDefs = [
            DTColumnDefBuilder.newColumnDef(0),
            DTColumnDefBuilder.newColumnDef(1),
			DTColumnDefBuilder.newColumnDef(2),
			DTColumnDefBuilder.newColumnDef(3),
            DTColumnDefBuilder.newColumnDef(4).notSortable()
		];

		$scope.ObtenerSolicitudes = function () {
			$http.get($rootScope.pathURL + '/SolicitudCompra/ObtenerSolicitudes?SaldoCero=' + ($scope.MostrarSaldoCero ? true : false))
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.solicitudes = response.solicitudes;
					}
				});
		}
		$scope.ObtenerSolicitudes();

		$scope.Editar = function (solicitud) {
			$window.open($rootScope.pathURL + '/SolicitudCompra/Edit?Id=' + solicitud.Id + '&accion=edit', '_self');
		}

		$scope.Clonar = function (solicitud) {
			$window.open($rootScope.pathURL + '/SolicitudCompra/Edit?Id=' + solicitud.Id + '&accion=clonar', '_self');
		}

		$scope.Alcance = function (solicitud) {
			$window.open($rootScope.pathURL + '/SolicitudCompra/Edit?Id=' + solicitud.Id + '&accion=alcance', '_self');
		}

		$scope.Eliminar = function (Id) {
			bootbox.confirm('<i class="fa fa-question-circle fa-3x text-danger" style="position: relative; top:10px;"></i> ¿Seguro que desea eliminar el elemento?', function (result) {
				if (result) {
					$http.post($rootScope.pathURL + '/SolicitudCompra/Eliminar', { Id: Id })
						.success(function (response) {
							var mensajes = '';

							angular.forEach(response.validacion, function (value, index) {
								mensajes = mensajes + '<li>' + value + '</li>';
							});

							if (mensajes.length > 1) {
								bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
							} else {
								$scope.ObtenerSolicitudes();
								toastr['success']("Elemento eliminado.", "Confirmación");
							}
						});
				}
			});
		};

    }]);