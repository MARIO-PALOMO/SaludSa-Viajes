/**
 * Creación y registro del módulo app_global.
 */
var app_global = angular.module("app_global", []);

/**
 * Creación y registro del módulo app_solicitud_compra.
 */
var app_solicitud_compra = angular.module("app_solicitud_compra", [
    "datatables",
    "datatables.columnfilter",
    "datatables.bootstrap",
    "ngAnimate",
    "ui.bootstrap",
    "angular.chosen",
    "app_global",

    "app_platilla_distribucion"
]);

/**
 * Creación y registro del módulo app_platilla_distribucion.
 */
var app_platilla_distribucion = angular.module("app_platilla_distribucion", [
    "datatables",
    "datatables.columnfilter",
    "datatables.bootstrap",
    "ngAnimate",
    "ui.bootstrap",
    "angular.chosen",
    "app_global"
]);

/**
 * Creación y registro del módulo app_tarea.
 */
var app_tarea = angular.module("app_tarea", [
    "datatables",
    "datatables.columnfilter",
    "datatables.bootstrap",
    "ngAnimate",
    "ui.bootstrap",
    "angular.chosen",
    "app_global",

    "app_platilla_distribucion"
]);

/**
 * Creación y registro del módulo app_rol_gestor_compra.
 */
var app_rol_gestor_compra = angular.module("app_rol_gestor_compra", [
    "datatables",
    "datatables.columnfilter",
    "datatables.bootstrap",
    "ngAnimate",
    "ui.bootstrap",
    "angular.chosen",
    "app_global"
]);

/**
 * Creación y registro del módulo app_historial_tareas.
 */
var app_historial_tareas = angular.module("app_historial_tareas", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_seguimiento_procesos.
 */
var app_seguimiento_procesos = angular.module("app_seguimiento_procesos", [
    "datatables",
    "datatables.columnfilter",
    "datatables.bootstrap",
    "ngAnimate",
    "ui.bootstrap",
	"angular.chosen",
    "app_global"
]);

/**
 * Creación y registro del módulo app_anulacion_recepcion.
 */
var app_anulacion_recepcion = angular.module("app_anulacion_recepcion", [
    "datatables",
    "datatables.columnfilter",
    "datatables.bootstrap",
    "ngAnimate",
    "ui.bootstrap",
    "angular.chosen",
	"app_global",

	"app_platilla_distribucion"
]);

/**
 * Creación y registro del módulo app_reasignacion_tareas.
 */
var app_reasignacion_tareas = angular.module("app_reasignacion_tareas", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_fuera_oficina.
 */
var app_fuera_oficina = angular.module("app_fuera_oficina", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_detener_solicitudes.
 */
var app_detener_solicitudes = angular.module("app_detener_solicitudes", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_rol_administrativo.
 */
var app_rol_administrativo = angular.module("app_rol_administrativo", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_reasignacion_originador_compra.
 */
var app_reasignacion_originador_compra = angular.module("app_reasignacion_originador_compra", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_email_pendiente.
 */
var app_email_pendiente = angular.module("app_email_pendiente", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global",
	"ngSanitize"
]);

/*******************************/

/**
 * Creación y registro del módulo app_solicitud_pago.
 */
var app_solicitud_pago = angular.module("app_solicitud_pago", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global",

	"app_platilla_distribucion"
]);

/**
 * Creación y registro del módulo app_impuesto_pago.
 */
var app_impuesto_pago = angular.module("app_impuesto_pago", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_parametro_pago.
 */
var app_parametro_pago = angular.module("app_parametro_pago", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_tipo_pago_pago.
 */
var app_tipo_pago_pago = angular.module("app_tipo_pago_pago", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_tarea_pago.
 */
var app_tarea_pago = angular.module("app_tarea_pago", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global",

	"app_platilla_distribucion",
	"app_solicitud_pago"
]);

/**
 * Creación y registro del módulo app_historial_tareas_pago.
 */
var app_historial_tareas_pago = angular.module("app_historial_tareas_pago", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_seguimiento_procesos_pago.
 */
var app_seguimiento_procesos_pago = angular.module("app_seguimiento_procesos_pago", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_reasignacion_tareas_pago.
 */
var app_reasignacion_tareas_pago = angular.module("app_reasignacion_tareas_pago", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_data_warehouse_pago.
 */
var app_data_warehouse_pago = angular.module("app_data_warehouse_pago", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);

/**
 * Creación y registro del módulo app_config_fact_electronica_pago.
 */
var app_config_fact_electronica_pago = angular.module("app_config_fact_electronica_pago", [
	"datatables",
	"datatables.columnfilter",
	"datatables.bootstrap",
	"ngAnimate",
	"ui.bootstrap",
	"angular.chosen",
	"app_global"
]);