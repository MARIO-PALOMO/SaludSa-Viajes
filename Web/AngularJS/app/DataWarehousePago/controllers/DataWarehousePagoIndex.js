
/**
 * Gestiona la lógica de control de la vista index del módulo Data Warehouse.
 */
app_data_warehouse_pago.controller('DataWarehousePagoIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
    '$http',
    function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http) {

		$scope.Reportes = [
			{
				Id: 1,
				Nombre: 'Reporte 1',
				Fecha: '02/02/2019',
				Usuario: 'Nombre de Usuario'
			},
			{
				Id: 2,
				Nombre: 'Reporte 2',
				Fecha: '02/02/2019',
				Usuario: 'Nombre de Usuario'
			},
			{
				Id: 3,
				Nombre: 'Reporte 3',
				Fecha: '02/02/2019',
				Usuario: 'Nombre de Usuario'
			}
		];

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
                    null
                ]
            });

        $scope.dtColumnDefs = [
			DTColumnDefBuilder.newColumnDef(0),
			DTColumnDefBuilder.newColumnDef(1),
			DTColumnDefBuilder.newColumnDef(2),
            DTColumnDefBuilder.newColumnDef(3).notSortable()
		];

        $scope.Adicionar = function () {
            var modalInstance = $uibModal.open({
                animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/DataWarehousePago/views/form.html',
				controller: 'DataWarehousePagoCreate',
				backdrop: 'static',
				size: 'lg',
                resolve: {
                    data: {
						TituloPag: 'Adicionar reporte',
						Accion: 'create'
                    }
                }
            });

            modalInstance.result.then(function (result) {
				
            }, function () {

            });
		}

    }]);