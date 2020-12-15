/**
 * Acciones a realizar cuando se ejecuta el módulo.
 */
app_global.run(function ($rootScope, $templateCache, $http) {

    /**
     * Alias virtual de URL configurado en IIS.
     */
    $rootScope.pathURL = '';

    /**
     * Evito que las plantillas se almacenen en cache.
     */
    $rootScope.$on('$viewContentLoaded', function () {
        $templateCache.removeAll();
    });

    /**
     * Almaceno en el ámbito del módulo principal la traducción a español de los textos de datatable.
     */
    $rootScope.datatable_traduccion_es = {
        "sEmptyTable": '<i class="fa fa-warning text-warning"></i> No hay datos disponibles en la tabla',
        "sInfo": "Mostrando del _START_ al _END_ de _TOTAL_ registros",
        "sInfoEmpty": "Mostrando del 0 al 0 de 0 registros",
        "sInfoFiltered": "(filtrado de _MAX_ registros en total)",
        "sInfoPostFix": "",
        "sInfoThousands": ",",
        "sLengthMenu": "_MENU_",
        "sLoadingRecords": "Cargando...",
        "sProcessing": "Procesando...",
        "sSearch": '',
        "sZeroRecords": '<i class="fa fa-warning text-warning"></i> No existen coincidencias',
        "oPaginate": {
            "sFirst": "Primero",
            "sLast": "Último",
            "sNext": "Siguiente",
            "sPrevious": "Anterior"
        },
        "oAria": {
            "sSortAscending": ": activate to sort column ascending",
            "sSortDescending": ": activate to sort column descending"
        }
    };

    /**
     * Traducción a español de los componentes para fecha.
     */
    //$.datepicker.regional['es'] = {
    //    closeText: 'Cerrar',
    //    prevText: '<',
    //    nextText: '>',
    //    currentText: 'Hoy',
    //    monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
    //    monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
    //    dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
    //    dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
    //    dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
    //    weekHeader: 'Sm',
    //    dateFormat: 'dd/mm/yy',
    //    firstDay: 1,
    //    isRTL: false,
    //    showMonthAfterYear: false,
    //    yearSuffix: '',
    //    autoSize: true,
    //    changeYear: false
    //};
    //$.datepicker.setDefaults($.datepicker.regional['es']);

    jQuery('#panel-listado-general-fade').fadeIn(2000);
});