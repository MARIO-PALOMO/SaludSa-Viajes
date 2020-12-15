
/**
 * Gestiona la lógica de control de la vista index del módulo Parámetro Pago.
 */
app_parametro_pago.controller('ParametroPagoIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
    '$http',
    function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http) {

		$scope.Parametros = [
			{
				Id: 1,
				Nombre: 'Número de autorización electrónica',
				Tamanno: '37',
				Descripcion: 'Ejemplo de descripción.'
			},
			{
				Id: 2,
				Nombre: 'Número de autorización física',
				Tamanno: '10',
				Descripcion: 'Ejemplo de descripción.'
			},
			{
				Id: 3,
				Nombre: 'Número de factura',
				Tamanno: '15',
				Descripcion: 'Ejemplo de descripción.'
			},
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

        $scope.Editar = function () {
            var modalInstance = $uibModal.open({
                animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/ParametroPago/views/form.html',
				controller: 'ParametroPagoEdit',
                backdrop: 'static',
                resolve: {
                    data: {
						TituloPag: 'Editar parámetro',
						Accion: 'edit'
                    }
                }
            });

            modalInstance.result.then(function (result) {
				
            }, function () {

            });
		}

    }]);