/**
 * Directiva para ejecutar una función cuando se da enter.
 */
app_global.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });
                event.preventDefault();
            }
        });
    };
});

/**
 * Directiva para inicializar los campos de tipo fecha.
 */
app_global.directive('ngDatepicker', function () {
	return {
		require: 'ngModel',
		link: function (scope, element, attrs, ngModel) {

			//var date = new Date();

			element.datepicker({
				autoclose: true,
				orientation: "right",
				language: 'es'
			});

			/*element.css('cursor', 'pointer');

			element.on('keydown', function (e) {
				var keyCode = e.keyCode || e.which;

				if (keyCode != 9) {
					return false;
				}
			});

			var date = new Date();
			var anio = date.getFullYear();
			var mes = date.getMonth();

			scope.fecha_desde = '01/' + ('00' + (parseInt(mes) + 1)).slice(1) + '/' + anio;

			element.datepicker("setDate", scope.fecha_desde);*/
		}
	};
});

/**
 * Directiva para inicializar los campos de tipo fecha.
 */
app_global.directive('ngDatepickerDesde', function () {
    return {
        require: 'ngModel',
		link: function (scope, element, attrs, ngModel) {

			var date = new Date();

            element.datepicker({
                defaultDate: "+1w",
                changeMonth: true,
                changeYear: true,
				numberOfMonths: 1,
				format: 'dd/mm/yyyy',
				startDate: '01-01-' + date.getFullYear(),
				endDate: '31-12-' + date.getFullYear(),
				language: 'es'
			});

            element.css('cursor', 'pointer');

            element.on('keydown', function (e) {
                var keyCode = e.keyCode || e.which;

                if (keyCode != 9) {
                    return false;
                }
			});

			var date = new Date();
			var anio = date.getFullYear();
			var mes = date.getMonth();

			scope.fecha_desde = '01/' + ('00' + (parseInt(mes) + 1)).slice(1) + '/' + anio;

			element.datepicker("setDate", scope.fecha_desde);
        }
    };
});

/**
 * Directiva para inicializar los campos de tipo fecha.
 */
app_global.directive('ngDatepickerHasta', function () {
	return {
		require: 'ngModel',
		link: function (scope, element, attrs, ngModel) {

			var date = new Date();

			element.datepicker({
				defaultDate: "+1w",
				changeMonth: true,
				changeYear: true,
				numberOfMonths: 1,
				format: 'dd/mm/yyyy',
				startDate: '01-01-' + date.getFullYear(),
				endDate: '31-12-' + date.getFullYear(),
				language: 'es'
			});

			element.css('cursor', 'pointer');

			element.on('keydown', function (e) {
				var keyCode = e.keyCode || e.which;

				if (keyCode != 9) {
					return false;
				}
			});

			var date = new Date();
			var anio = date.getFullYear();
			var mes = date.getMonth();

			var ultimo_dia = (new Date(anio, mes + 1, 0)).getDate();
			scope.fecha_hasta = ultimo_dia + '/' + ('00' + (parseInt(mes) + 1)).slice(1) + '/' + anio;

			element.datepicker("setDate", scope.fecha_hasta);
		}
	};
});

/**
 * Directiva para inicializar los campos numéricos con dos decimales.
 */
app_global.directive('ngDosDecimales', function () {
    return function (scope, element, attrs) {
        element.IntOrDecimal({ decimales: 2 });
    };
});

/**
 * Directiva para inicializar los campos numéricos enteros.
 */
app_global.directive('ngEntero', function () {
    return function (scope, element, attrs) {
        element.IntOrDecimal();
    };
});

/**
 * Directiva para completar los números decimales cuando se abandona el campo.
 */
app_global.directive('ngIntOrDecimal', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {

            elm.blur(function () {
                var tem = ctrl.$viewValue.split(".");

                if (ctrl.$viewValue.length === 0) {
                    ctrl.$setViewValue("0.00");
                } else if (tem.length === 1) {
                    ctrl.$setViewValue(ctrl.$viewValue + ".00");
                } else if (tem.length === 2) {
                    if (tem[1].length === 1) {
                        ctrl.$setViewValue(ctrl.$viewValue + "0");
                    }
                }
                ctrl.$render();
            });
        }

    }
});

/**
 * Directiva para completar con ceros a la izquierda el valor en un input cuando se abandona (on-blur) el campo.
 */
app_global.directive('ngCompletarCeros', function () {
	return {
		require: 'ngModel',
		link: function (scope, elm, attrs, ctrl) {

			elm.blur(function () {
				var val = ctrl.$viewValue;

				if (val) {
					ctrl.$setViewValue(('0000000000' + val).slice(-1 * attrs.ngCompletarCeros));
					ctrl.$render();
				}
			});
		}

	}
});

/**
 * Directiva para hacer un modal draggable.
 */
app_global.directive('ngModalDraggable', function ($document) {
    "use strict";
    return function (scope, element, attr) {
        var startX = 0,
          startY = 0,
          x = 0,
          y = 0;

        element = angular.element(document.getElementById(attr.ngModalDraggable).parentNode.parentNode);
        var header = angular.element(document.querySelector('#' + attr.ngModalDraggable + ' .modal-header'));
        
        header.css({
            cursor: 'move'
        });

        header.on('mousedown', function (event) {
            event.preventDefault();
            startX = event.screenX - x;
            startY = event.screenY - y;
            $document.on('mousemove', mousemove);
            $document.on('mouseup', mouseup);
        });

        function mousemove(event) {
            y = event.screenY - startY;
            x = event.screenX - startX;
            element.css({
                top: y + 'px',
                left: x + 'px'
            });
        }

        function mouseup() {
            $document.unbind('mousemove', mousemove);
            $document.unbind('mouseup', mouseup);
        }
    };
});

/**
 * Directiva para mostrar el indicador de cantidad máxima de caracteres.
 */
app_global.directive('ngMaxlengthIndicator', function () {
    return function (scope, element, attrs) {
        element.maxlength({
            alwaysShow: true,
            appendToParent: true
        });
    };
});

/**
 * Directiva para cargar varios adjuntos.
 */
app_global.directive('ngAdjuntarArchivosRequerimientosAdjuntos', function ($parse) {
	return {
		restrict: 'A',
		link: function (scope, element, attributes) {
			//var model = $parse(attributes.ngAdjuntarArchivosRequerimientosAdjuntos);
			//var assign = model.assign;
			
			element.bind('change', function () {
				var files = [];
				
				angular.forEach(element[0].files, function (file) {
					
					var ext = file.name.match(/\.([^\.]+)$/);
					ext = ext ? ext[1].toLowerCase() : ext;

					if (file.size <= 5242880 &&
						(ext == 'pdf'
						|| ext == 'doc'
						|| ext == 'docx'
						|| ext == 'xls'
						|| ext == 'xlsx'
						|| ext == 'csv'
						|| ext == 'bmp'
						|| ext == 'dib'
						|| ext == 'jpe'
						|| ext == 'jpeg'
						|| ext == 'jpg'
						|| ext == 'png'
						|| ext == 'tif'
						|| ext == 'tiff')) {
						files.push(file);
					}
				});
				scope.$apply(function () {
					//assign(scope, files);
					scope.Solicitud.RequerimientosAdjuntos = scope.Solicitud.RequerimientosAdjuntos.concat(files);
					scope.SubirRequerimientosAdjuntos();
				});
			});
		}
	};
});

/**
 * Directiva para cargar varios adjuntos.
 */
app_global.directive('ngAdjuntarArchivosCotizacionesAdjuntos', function ($parse) {
	return {
		restrict: 'A',
		link: function (scope, element, attributes) {
			//var model = $parse(attributes.ngAdjuntarArchivosRequerimientosAdjuntos);
			//var assign = model.assign;

			element.bind('change', function () {
				var files = [];

				angular.forEach(element[0].files, function (file) {

					var ext = file.name.match(/\.([^\.]+)$/);
					ext = ext ? ext[1].toLowerCase() : ext;

					if (file.size <= 5242880 &&
						(ext == 'pdf'
							|| ext == 'doc'
							|| ext == 'docx'
							|| ext == 'xls'
							|| ext == 'xlsx'
							|| ext == 'csv'
							|| ext == 'bmp'
							|| ext == 'dib'
							|| ext == 'jpe'
							|| ext == 'jpeg'
							|| ext == 'jpg'
							|| ext == 'png'
							|| ext == 'tif'
							|| ext == 'tiff')) {
						files.push(file);
					}
				});
				scope.$apply(function () {
					//assign(scope, files);
					scope.Solicitud.CotizacionesAdjuntos = scope.Solicitud.CotizacionesAdjuntos.concat(files);
					scope.SubirCotizacionesAdjuntos();
				});
			});
		}
	};
});

/**
 * Directiva para cargar varios adjuntos.
 */
app_global.directive('ngAdjuntarArchivosEvaluacionesAdjuntos', function ($parse) {
	return {
		restrict: 'A',
		link: function (scope, element, attributes) {
			//var model = $parse(attributes.ngAdjuntarArchivosRequerimientosAdjuntos);
			//var assign = model.assign;

			element.bind('change', function () {
				var files = [];

				angular.forEach(element[0].files, function (file) {

					var ext = file.name.match(/\.([^\.]+)$/);
					ext = ext ? ext[1].toLowerCase() : ext;

					if (file.size <= 5242880 &&
						(ext == 'pdf'
							|| ext == 'doc'
							|| ext == 'docx'
							|| ext == 'xls'
							|| ext == 'xlsx'
							|| ext == 'csv'
							|| ext == 'bmp'
							|| ext == 'dib'
							|| ext == 'jpe'
							|| ext == 'jpeg'
							|| ext == 'jpg'
							|| ext == 'png'
							|| ext == 'tif'
							|| ext == 'tiff')) {
						files.push(file);
					}
				});
				scope.$apply(function () {
					//assign(scope, files);
					scope.Solicitud.EvaluacionesAdjuntos = scope.Solicitud.EvaluacionesAdjuntos.concat(files);
					scope.SubirEvaluacionesAdjuntos();
				});
			});
		}
	};
});

/**
 * Directiva para cargar varios adjuntos.
 */
app_global.directive('ngAdjuntarArchivosFacturasAdjuntos', function ($parse) {
	return {
		restrict: 'A',
		link: function (scope, element, attributes) {
			//var model = $parse(attributes.ngAdjuntarArchivosRequerimientosAdjuntos);
			//var assign = model.assign;

			element.bind('change', function () {
				var files = [];

				angular.forEach(element[0].files, function (file) {

					var ext = file.name.match(/\.([^\.]+)$/);
					ext = ext ? ext[1].toLowerCase() : ext;

					if (file.size <= 5242880 &&
						(ext == 'pdf'
							|| ext == 'doc'
							|| ext == 'docx'
							|| ext == 'xls'
							|| ext == 'xlsx'
							|| ext == 'csv'
							|| ext == 'bmp'
							|| ext == 'dib'
							|| ext == 'jpe'
							|| ext == 'jpeg'
							|| ext == 'jpg'
							|| ext == 'png'
							|| ext == 'tif'
							|| ext == 'tiff')) {
						files.push(file);
					}
				});
				scope.$apply(function () {
					//assign(scope, files);
					scope.Solicitud.FacturasAdjuntos = scope.Solicitud.FacturasAdjuntos.concat(files);
					scope.SubirFacturasAdjuntos();
				});
			});
		}
	};
});

/**
 * Directiva para cargar un adjunto.
 */
app_global.directive('ngAdjuntarArchivoFacturaPago', function ($parse) {
	return {
		restrict: 'A',
		link: function (scope, element, attributes) {
			//var model = $parse(attributes.ngAdjuntarArchivosRequerimientosAdjuntos);
			//var assign = model.assign;

			element.bind('change', function () {
				var files = [];

				angular.forEach(element[0].files, function (file) {

					var ext = file.name.match(/\.([^\.]+)$/);
					ext = ext ? ext[1].toLowerCase() : ext;

					if (file.size <= 5242880 && ext == 'pdf') {
						files.push(file);
					}
				});

				if (files && files[0]) {
					scope.$apply(function () {
						scope.Factura.FacturaAdjunta = files[0];
					});
				}
				else {
					scope.$apply(function () {
						scope.Factura.FacturaAdjunta = {};
					});
				}
			});
		}
	};
});