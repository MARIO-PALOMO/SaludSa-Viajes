/**
 * Servicio para dar formato a las fechas.
 */
app_global.service('json_js_date', function () {
	this.convertir = function (valor) {
		if (valor != null && valor != "") {
			var pattern = /Date\(([^)]+)\)/;
			if (pattern.test(valor)) {
				var results = pattern.exec(valor);
				var dt = new Date(parseFloat(results[1]));
				return ("0" + (dt.getDate())).slice(-2) + "/" + ("0" + (dt.getMonth() + 1)).slice(-2) + "/" + dt.getFullYear();
			} else {
				return valor;
			}
		}
	}
});

/**
 * Servicio para dar formato a las fechas (Incluye hora y minuto).
 */
app_global.service('json_js_datetime', function () {
	this.convertir = function (valor) {
		if (valor !== null && valor !== "") {
			var pattern = /Date\(([^)]+)\)/;
			if (pattern.test(valor)) {
				var results = pattern.exec(valor);
				var dt = new Date(parseFloat(results[1]));
				var hours = String(dt.getHours());
				if (hours.length == 1)
					hours = '0' + hours;
				var minutes = String(dt.getMinutes());
				if (minutes.length == 1)
					minutes = '0' + minutes;
				return ("0" + (dt.getDate())).slice(-2) + "/" + ("0" + (dt.getMonth() + 1)).slice(-2) + "/" + dt.getFullYear() + ' ' + hours + ':' + minutes;
			} else {
				return valor;
			}
		}
	}
});

/**
 * Servicio para interseptar las peticiones ajax.
 */
app_global
    .factory('ajaxInterceptor', ajaxInterceptor)
    .config(function ($httpProvider) {
        $httpProvider.interceptors.push('ajaxInterceptor');
    });

function ajaxInterceptor() {
    var loadingCount = 0;

    return {
        request: function (config) {
			if (++loadingCount === 1) {

				//var count_block1 = document.getElementsByClassName('blockUI');
				//var count_block2 = document.getElementsByClassName('blockOverlay');

				//if (count_block1.length == 0 && count_block2.length == 0) {
					App.blockUI({
						animate: true
					});
				//}				

                $('input[maxlength], textarea[maxlength]').maxlength({
                    alwaysShow: true,
                    appendToParent: true
                });
            }
            return config;
        },

        requestError: function (config) {
			bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ocurrió un error en la comunicación con el servidor. <br /><br /> Revise el log de la consola del navegador para mayor información.');
            console.log(config);

            if (--loadingCount === 0) {
				App.unblockUI();
            }

            return config;
        },

        response: function (response) {
            if (--loadingCount === 0) {
				App.unblockUI();

                $('input[maxlength], textarea[maxlength]').maxlength({
                    alwaysShow: true,
                    appendToParent: true
                });
            }

            console.log(response);

            return response;
        },

        responseError: function (response) {
			bootbox.alert('<i class="fa fa-times-circle fa-3x text-danger" style="position: relative; top:10px;"></i> Ocurrió un error en la comunicación con el servidor. <br /><br /> Revise el log de la consola del navegador para mayor información.');
            console.log(response);

            if (--loadingCount === 0) {
				App.unblockUI();

                $('input[maxlength], textarea[maxlength]').maxlength({
                    alwaysShow: true,
                    appendToParent: true
                });
            }

            return response;
        }
    };
};

/**
 *  Servicio para realizar la persistencia de solicitudes de compra
 */
app_global.service('Persistencia', ["$http", "$q", "$rootScope", function ($http, $q, $rootScope) {

	this.Create = function (files, Cabecera) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('Cabecera', JSON.stringify(Cabecera));

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file);
			});
		}

		return $http.post($rootScope.pathURL + '/SolicitudCompra/Create', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

	this.Clonar = function (files, Cabecera, SolicitudId, AdjuntosAnteriores) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('Cabecera', JSON.stringify(Cabecera));
		formData.append('SolicitudId', SolicitudId);
		formData.append('AdjuntosAnteriores', AdjuntosAnteriores);

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file);
			});
		}

		return $http.post($rootScope.pathURL + '/SolicitudCompra/Clonar', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

	this.Edit = function (files, Cabecera) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('Cabecera', JSON.stringify(Cabecera));

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file);
			});
		}

		return $http.post($rootScope.pathURL + '/SolicitudCompra/Edit', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

	this.EditDevueltaSolicitante = function (files, Cabecera, TareaId, Accion) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('Cabecera', JSON.stringify(Cabecera));
		formData.append('TareaId', TareaId);
		formData.append('Accion', Accion);

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file);
			});
		}

		return $http.post($rootScope.pathURL + '/Tarea/EditSolicitudDevueltaASolicitante', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

	this.SubirRequerimientosAdjuntos = function (files, SolicitudId) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('SolicitudId', SolicitudId);

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file);
			});
		}

		return $http.post($rootScope.pathURL + '/SolicitudCompra/SubirRequerimientosAdjuntos', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

	this.SubirCotizacionesAdjuntos = function (files, SolicitudId) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('SolicitudId', SolicitudId);

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file);
			});
		}

		return $http.post($rootScope.pathURL + '/SolicitudCompra/SubirCotizacionesAdjuntos', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

	this.SubirEvaluacionesAdjuntos = function (files, SolicitudId) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('SolicitudId', SolicitudId);

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file);
			});
		}

		return $http.post($rootScope.pathURL + '/SolicitudCompra/SubirEvaluacionesAdjuntos', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

	this.SubirFacturasAdjuntos = function (files, SolicitudId, RecepcionId) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('SolicitudId', SolicitudId);
		formData.append('RecepcionId', RecepcionId);

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file);
			});
		}

		return $http.post($rootScope.pathURL + '/SolicitudCompra/SubirFacturasAdjuntos', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

}]);

/**
 *  Servicio para realizar la persistencia de solicitudes de pago
 */
app_global.service('PersistenciaPago', ["$http", "$q", "$rootScope", function ($http, $q, $rootScope) {

	this.Create = function (files, Cabecera) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('Cabecera', JSON.stringify(Cabecera));

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file.file, file.name);
			});
		}

		return $http.post($rootScope.pathURL + '/SolicitudPago/Create', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

	this.Edit = function (files, Cabecera, FacturasEliminar, FacturasEliminarAdjunto) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('Cabecera', JSON.stringify(Cabecera));
		formData.append('FacturasEliminar', JSON.stringify(FacturasEliminar));
		formData.append('FacturasEliminarAdjunto', JSON.stringify(FacturasEliminarAdjunto));

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file.file, file.name);
			});
		}

		return $http.post($rootScope.pathURL + '/SolicitudPago/Edit', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

	this.EditTarea = function (files, Cabecera, FacturasEliminar, FacturasEliminarAdjunto, TareaId) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('Cabecera', JSON.stringify(Cabecera));
		formData.append('FacturasEliminar', JSON.stringify(FacturasEliminar));
		formData.append('FacturasEliminarAdjunto', JSON.stringify(FacturasEliminarAdjunto));
		formData.append('TareaId', TareaId);

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file.file, file.name);
			});
		}

		return $http.post($rootScope.pathURL + '/TareaPago/TareaDevueltaSolicitanteJefeEdit', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

	this.EditDevolucionContabilidad = function (
			files,
			Factura,
			FacturasEliminarAdjunto,
			TareaId,
			AprobacionJefeArea,
			AprobacionSubgerenteArea,
			AprobacionGerenteArea,
			AprobacionVicePresidenteFinanciero,
			AprobacionGerenteGeneral) {
		var deferred = $q.defer();
		var formData = new FormData();

		formData.append('Factura', JSON.stringify(Factura));
		formData.append('FacturasEliminarAdjunto', JSON.stringify(FacturasEliminarAdjunto));
		formData.append('TareaId', TareaId);
		formData.append('AprobacionJefeArea', AprobacionJefeArea);
		formData.append('AprobacionSubgerenteArea', AprobacionSubgerenteArea);
		formData.append('AprobacionGerenteArea', AprobacionGerenteArea);
		formData.append('AprobacionVicePresidenteFinanciero', AprobacionVicePresidenteFinanciero);
		formData.append('AprobacionGerenteGeneral', AprobacionGerenteGeneral);

		if (files) {
			angular.forEach(files, function (file, index) {
				var NombreArchivo = 'ArchivoAdjunto' + index;
				formData.append(NombreArchivo, file.file, file.name);
			});
		}

		return $http.post($rootScope.pathURL + '/TareaPago/TareaDevueltaSolicitanteContabilidadEdit', formData, {
			headers: { 'Content-Type': undefined },
			transformRequest: angular.identity
		})
			.success(function (response) {
				deferred.resolve(response);
			})
			.error(function (msg, code) {
				deferred.reject(msg)
			})
		return deferred.promise;
	};

}]);