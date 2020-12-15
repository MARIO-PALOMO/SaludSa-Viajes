/*
 * Copyright (c) 2015 Jorge Enrique Rodriguez Cañares
 *
 * Ej1.   $('#Element').IntOrDecimal();         Para un entero.   
 *
 * Ej2.   $('#Element').({ decimales: 2});;     Para un decimal de dos decimaes. 
 */
(function ($) {
    $.fn.IntOrDecimal = function (config, callback) {
        if (typeof config === 'boolean') {
            config = { decimal: config };
        }
        config = config || {};
        var decimales;
        if ((typeof config.decimales) == "number") {
            if (config.decimales == 0) {
                decimales = -1;
            }
            else
                decimales = config.decimales;
        }
        else
            decimales = -1;

        if (decimales > 0) {
            //recive un arreglo con todos los elementos seleccionados
            $(this).each(function (index) {
                $(this).val(($(this).val() * 1).toFixed(decimales));
                if ($(this).val() == "NaN") $(this).val((0 * 1).toFixed(decimales));
            });
        }
        //return this.keypress($.fn.IntOrDecimal.keypress).keyup($.fn.IntOrDecimal.keyup).blur($.fn.IntOrDecimal.blur);
        return this.data("parametro.decimales", decimales).keydown($.fn.IntOrDecimal.keydown).click($.fn.IntOrDecimal.click).blur($.fn.IntOrDecimal.blur);
    };

    $.fn.IntOrDecimal.keydown = function (e) {
        var decimales = $.data(this, "parametro.decimales");
        var selectedText = document.getSelection();
        var pos = this.value.slice(0, this.selectionStart).length;
        //solo numeros y el punto
        var charCode = (e.which) ? e.which : e.keyCode;
        if (((charCode < 48 || charCode > 57) && (charCode < 96 || charCode > 105) && charCode != 110 && charCode != 190 && charCode != 37 && charCode != 39 && charCode != 8 && charCode != 9 && charCode != 46 && charCode != 67 && charCode != 86/* && charCode != 88*/)) return false

        //el punto si es un decimal
        if (decimales == -1 && (charCode == 110 || charCode == 190)) return false;
        var numbers = $(this).val().split(".");
        //solo teclear un punto, pero si el texto esta seleccionado borro
        if (numbers.length > 1 && (charCode == 110 || charCode == 190) && selectedText.toString().length <= 0) return false;
        //si estoy escribiendo despues del punto valido la cantidad de digitos que se puede escrivir
        if (numbers.length > 1 && pos > $(this).val().indexOf('.') && numbers[1].length >= decimales && charCode != 37 && charCode != 39 && charCode != 8 && charCode != 9 && charCode != 46) return false;

        //si estoy escribiendo antes del punto valido la cantidad de digitos que se puede escribir
        //     if (numbers.length > 1 && pos < $(this).val().indexOf('.') && numbers[0].length >= 12 && charCode != 37 && charCode != 39 && charCode != 8 && charCode != 9 && charCode != 46 && selectedText.toString().length <= 0) return false;
        //     if (numbers.length <= 1 && $(this).val().length >= 12 && charCode != 37 && charCode != 39 && charCode != 8 && charCode != 9 && charCode != 46 && charCode != 110 && charCode != 190 && selectedText.toString().length <= 0) return false;
    };

    $.fn.IntOrDecimal.click = function (e) {
        $(this).select();
    }

    $.fn.IntOrDecimal.blur = function () {
        var decimales = $.data(this, "parametro.decimales");
        if (decimales > 0) {
            $(this).val(($(this).val() * 1).toFixed(decimales));
            if ($(this).val() == "NaN") $(this).val((0 * 1).toFixed(decimales));

            var tem = $(this).val().split(".");

            if ($(this).val().length == 0) {
                $(this).val("0.00");
            } else if (tem.length == 1) {
                $(this).val($(this).val() + ".00");
            } else if (tem.length == 2) {
                if(tem[1].length == 1) {
                    $(this).val($(this).val() + "0");
                }
            }
        }
    };
})(jQuery);
