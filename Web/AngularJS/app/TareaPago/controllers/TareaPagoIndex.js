
/**
 * Gestiona la lógica de control de la vista index del módulo Tarea Pago.
 */
app_tarea_pago.controller('TareaPagoIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
    '$http',
    '$window',
    function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http, $window) {

		$scope.tareas = [];

        $scope.dtOptions = DTOptionsBuilder.newOptions()
            .withBootstrap()                               
            .withOption('order', [1, 'asc'])
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
                    { type: 'text' }
                ]
            });

        $scope.dtColumnDefs = [
            DTColumnDefBuilder.newColumnDef(0),
            DTColumnDefBuilder.newColumnDef(1),
			DTColumnDefBuilder.newColumnDef(2),
			DTColumnDefBuilder.newColumnDef(3)
		];

		$scope.Buscar = function () {
			$http.get($rootScope.pathURL + '/TareaPago/Buscar')
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.tareas = response.tareas;
					}
				});
		}
		$scope.Buscar();

    }]);