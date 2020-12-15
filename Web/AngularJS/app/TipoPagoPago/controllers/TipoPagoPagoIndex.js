
/**
 * Gestiona la lógica de control de la vista index del módulo Tipo Pago.
 */
app_tipo_pago_pago.controller('TipoPagoPagoIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
    '$http',
    function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http) {

		$scope.TiposPago = [];
		$scope.MostrarInactivos = false;

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
			DTColumnDefBuilder.newColumnDef(4),
			DTColumnDefBuilder.newColumnDef(5),
			DTColumnDefBuilder.newColumnDef(6),
            DTColumnDefBuilder.newColumnDef(7).notSortable()
		];

		$scope.Buscar = function () {
			$http.get($rootScope.pathURL + '/TipoPagoPago/Buscar?MostrarInactivos=' + $scope.MostrarInactivos)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.TiposPago = response.tiposPago;
					}
				});
		}
		$scope.Buscar();

        $scope.Adicionar = function () {
            var modalInstance = $uibModal.open({
                animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/TipoPagoPago/views/form.html',
				controller: 'TipoPagoPagoCreate',
                backdrop: 'static',
                resolve: {
                    data: {
						TituloPag: 'Adicionar tipo de pago',
						Accion: 'create'
                    }
                }
            });

            modalInstance.result.then(function (result) {
				$scope.Buscar();
            }, function () {

            });
		}

		$scope.Editar = function (Id) {
			var modalInstance = $uibModal.open({
				animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/TipoPagoPago/views/form.html',
				controller: 'TipoPagoPagoEdit',
				backdrop: 'static',
				resolve: {
					data: {
						TituloPag: 'Editar tipo de pago',
						Accion: 'edit',
						Id: Id
					}
				}
			});

			modalInstance.result.then(function (result) {
				$scope.Buscar();
			}, function () {

			});
		}

		$scope.Detalle = function (Id) {
			var modalInstance = $uibModal.open({
				animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/TipoPagoPago/views/form.html',
				controller: 'TipoPagoPagoEdit',
				backdrop: 'static',
				resolve: {
					data: {
						TituloPag: 'Detalle de tipo de pago',
						Accion: 'detail',
						Id: Id
					}
				}
			});

			modalInstance.result.then(function (result) {

			}, function () {

			});
		}

    }]);