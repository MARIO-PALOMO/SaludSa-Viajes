
/**
 * Gestiona la lógica de control de la vista show del módulo Email Pendiente.
 */
app_email_pendiente.controller('EmailPendienteShow', [
    '$scope',
    '$rootScope',
    '$uibModal',
    '$uibModalInstance',
    '$log',
    '$http',
	'data',
	function ($scope, $rootScope, $uibModal, $uibModalInstance, $log, $http, data) {

        $scope.Data = data;
        $scope.Email = {};

        $scope.Ok = function (result) {
            $uibModalInstance.close(result);
        };

        $scope.Cerrar = function () {
            $uibModalInstance.dismiss();
		};

		$scope.Buscar = function () {
			$http.get($rootScope.pathURL + '/EmailPendiente/ObtenerItem?Id=' + $scope.Data.Id)
				.success(function (response) {
					var mensajes = '';

					angular.forEach(response.validacion, function (value) {
						mensajes = mensajes + '<li>' + value + '</li>';
					});

					if (mensajes.length > 1) {
						bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Se han detectado errores.<br /><br /><ul>' + mensajes + '</ul>');
					} else {
						$scope.Email = response.email;
					}
				});
		}
		$scope.Buscar();

		$scope.Enviar = function () {
			var EmailsEnviar = [];

			EmailsEnviar.push($scope.Data.Id);

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
						$scope.Ok();

						toastr['success']("Correo enviado.", "Confirmación");
					}
				});
		}

    }]);