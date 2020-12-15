
/**
 * Gestiona la lógica de control de la vista index del módulo Rol Administrativo.
 */
app_rol_administrativo.controller('RolAdministrativoIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
    '$http',
    function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http) {

		$scope.Roles = [];

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

		$scope.Buscar = function () {
			$http.get($rootScope.pathURL + '/RolAdministrativo/ObtenerRolesAdministrativos')
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Roles = response.roles;
					}
				});
		}
		$scope.Buscar();

        $scope.Adicionar = function () {
            var modalInstance = $uibModal.open({
                animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/RolAdministrativo/views/form.html',
				controller: 'RolAdministrativoCreate',
                backdrop: 'static',
                resolve: {
                    data: {
						TituloPag: 'Adicionar rol administrativo',
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
				templateUrl: $rootScope.pathURL + '/AngularJS/app/RolAdministrativo/views/form.html',
				controller: 'RolAdministrativoEdit',
				backdrop: 'static',
				resolve: {
					data: {
						TituloPag: 'Editar rol administrativo',
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
				templateUrl: $rootScope.pathURL + '/AngularJS/app/RolAdministrativo/views/form.html',
				controller: 'RolAdministrativoEdit',
				backdrop: 'static',
				resolve: {
					data: {
						TituloPag: 'Detalle de rol administrativo',
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
					$http.post($rootScope.pathURL + '/RolAdministrativo/Eliminar', { Id: Id })
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