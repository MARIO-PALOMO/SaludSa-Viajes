
/**
 * Gestiona la lógica de control de la vista index del módulo Plantilla Distribucion.
 */
app_platilla_distribucion.controller('PlantillaDistribucionIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
    '$http',
    function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http) {

		$scope.Plantillas = [];
		$scope.Sesion = {};

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

		$http.get($rootScope.pathURL + '/Sesion/ObtenerSesion')
			.success(function (response) {
				$scope.Sesion = response;

				$scope.Buscar();
			});

		$scope.Buscar = function () {
			$http.get($rootScope.pathURL + '/PlantillaDistribucion/ObtenerPlantillasDistribucion')
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Plantillas = response.plantillas;
					}
				});
		}

        $scope.Adicionar = function () {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: $rootScope.pathURL + '/AngularJS/app/PlantillaDistribucion/views/form.html',
                controller: 'PlantillaDistribucionCreate',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    data: {
						TituloPag: 'Adicionar plantilla de distribución',
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
				templateUrl: $rootScope.pathURL + '/AngularJS/app/PlantillaDistribucion/views/form.html',
				controller: 'PlantillaDistribucionEdit',
				backdrop: 'static',
				size: 'lg',
				resolve: {
					data: {
						TituloPag: 'Editar plantilla de distribución',
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
				templateUrl: $rootScope.pathURL + '/AngularJS/app/PlantillaDistribucion/views/form.html',
				controller: 'PlantillaDistribucionEdit',
				backdrop: 'static',
				size: 'lg',
				resolve: {
					data: {
						TituloPag: 'Detalle de plantilla de distribución',
						Accion: 'detail',
						Id: Id
					}
				}
			});

			modalInstance.result.then(function (result) {
				
			}, function () {

			});
		}

        $scope.Eliminar = function (Id) {
			bootbox.confirm('<i class="fa fa-question-circle fa-3x text-danger" style="position: relative; top:10px;"></i> ¿Seguro que desea eliminar el elemento?', function (result) {
                if (result) {
					$http.post($rootScope.pathURL + '/PlantillaDistribucion/Eliminar', { Id: Id })
                        .success(function (response) {
                            var mensajes = '';

                            angular.forEach(response.validacion, function (value, index) {
                                mensajes = mensajes + '<li>' + value + '</li>';
                            });

                            if (mensajes.length > 1) {
								bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
                            } else {
                                $scope.Buscar();
								toastr['success']("Elemento eliminado.", "Confirmación");
                            }
                        });
                }
            });
        };

    }]);