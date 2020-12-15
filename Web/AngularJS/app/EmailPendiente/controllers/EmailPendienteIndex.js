
/**
 * Gestiona la lógica de control de la vista index del módulo Email Pendiente.
 */
app_email_pendiente.controller('EmailPendienteIndex', [
    '$scope',
    '$rootScope',
    'DTOptionsBuilder',
    'DTColumnDefBuilder',
    '$uibModal',
    '$log',
	'$http',
    function ($scope, $rootScope, DTOptionsBuilder, DTColumnDefBuilder, $uibModal, $log, $http) {

		$scope.Emails = [];

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
					null,
					{ type: 'text' },
					{ type: 'text' },
					{ type: 'text' }
                ]
            });

        $scope.dtColumnDefs = [
			DTColumnDefBuilder.newColumnDef(0).notSortable(),
			DTColumnDefBuilder.newColumnDef(1),
			DTColumnDefBuilder.newColumnDef(2),
			DTColumnDefBuilder.newColumnDef(3)
		];

		$scope.Buscar = function () {
			$scope.SeleccionarTodo = false;

			$http.get($rootScope.pathURL + '/EmailPendiente/ObtenerItems')
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Emails = response.emails;
					}
				});
		}
		$scope.Buscar();

		$scope.Show = function (Email) {
            var modalInstance = $uibModal.open({
                animation: true,
				templateUrl: $rootScope.pathURL + '/AngularJS/app/EmailPendiente/views/form.html',
				controller: 'EmailPendienteShow',
				backdrop: 'static',
				size: 'lg',
                resolve: {
                    data: {
						TituloPag: 'Correo pendiente',
						Id: Email.Id
                    }
                }
            });

            modalInstance.result.then(function (result) {
				$scope.Buscar();
            }, function () {

            });
		}

		$scope.SeleccionarTodoChange = function () {
			$scope.SeleccionarTodo = !$scope.SeleccionarTodo;

			angular.forEach($scope.Emails, function (item) {
				item.Seleccionada = angular.copy($scope.SeleccionarTodo);
			});
		}

		$scope.Enviar = function () {
			var errores = '';

			var EmailsEnviar = [];

			angular.forEach($scope.Emails, function (item) {
				if (item.Seleccionada) {
					EmailsEnviar.push(item.Id);
				}
			});

			if (EmailsEnviar.length == 0) {
				errores = errores + '<li>Debe seleccionar al menos un correo para enviar.</li>';
			}

			if (errores.length > 1) {
				bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ha cometido errores en la entrada de datos.<br /><br /><ul>' + errores + '</ul>');
			}
			else {
				$http.post($rootScope.pathURL + '/EmailPendiente/Enviar', { EmailsEnviar: EmailsEnviar })
					.success(function (response) {
						var mensajes = '';

						angular.forEach(response.error, function (value, index) {
							mensajes = mensajes + '<li>' + value.error + '</li>';
						});

						angular.forEach(response.validacion, function (value, index) {
							mensajes = mensajes + '<li>' + value + '</li>';
						});

						if (mensajes.length > 1) {
							bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
						}
						else {
							$scope.Buscar();
							$scope.SeleccionarTodo = false;

							toastr['success']("Correos enviados.", "Confirmación");
						}
					});
			}
		}

    }]);