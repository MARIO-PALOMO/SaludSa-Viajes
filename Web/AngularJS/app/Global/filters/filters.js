/**
 * Filtro para dar formato a las fechas.
 */
app_global.filter("json_js_date", function () {
    return function (valor) {
        if (valor !== null && valor !== "") {
            var pattern = /Date\(([^)]+)\)/;
            if (pattern.test(valor)) {
                var results = pattern.exec(valor);
                var dt = new Date(parseFloat(results[1]));
                return ("0" + (dt.getDate())).slice(-2) + "/" + ("0" + (dt.getMonth() + 1)).slice(-2) + "/" + dt.getFullYear();
            } else {
                return valor;
            }
        }
    };
});

/**
 * Filtro para dar formato a las fechas (Incluye hora y minuto).
 */
app_global.filter("json_js_datetime", function () {
	return function (valor) {
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
})