
/**
 * Gestiona la lógica de control de la vista index del módulo Configuración de Factura Electrónica.
 */
app_config_fact_electronica_pago.controller('ConfigFactElectronicaPagoIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
    '$http',
    function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http) {

		$scope.Configuraciones = [
			{
				Id: 1,
				RucProvedor: '1212121212001',
				Campo: 'Campo 1',
				Operacion: '(+) SUMA',
			},
			{
				Id: 2,
				RucProvedor: '1212121212001',
				Campo: 'Campo 2',
				Operacion: '(-) RESTA',
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
				templateUrl: $rootScope.pathURL + '/AngularJS/app/ConfigFactElectronicaPago/views/form.html',
				controller: 'ConfigFactElectronicaPagoCreate',
                backdrop: 'static',
                resolve: {
                    data: {
						TituloPag: 'Adicionar configuración',
						Accion: 'create'
                    }
                }
            });

            modalInstance.result.then(function (result) {
				
            }, function () {

            });
		}

    }]);