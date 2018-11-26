// =======================================================================//
// ! Declaración de Variables Globales                                    //
// =======================================================================//
var dtFechaDesde, dtFechaHasta, txtCliente, selEstado, btnConsultar, btnLimpiar, ifrReporte, frmConsulta;

$(document).ready(function () {
    // =======================================================================//
    // ! Asignar Variables Globales                                           //
    // =======================================================================//
    txtCliente = $("#txtCliente");
    dtFechaDesde = $("#dtFechaDesde");
    dtFechaHasta = $('#dtFechaHasta');
    selEstado = $('#selEstado');
    btnConsultar = $('#btnConsultar');
    btnLimpiar = $('#btnLimpiar');
    ifrReporte = $('#ifrReporte');
    frmConsulta = $('#frmConsulta');
    Iniciar();
});

// Configuracion
function Iniciar() {
  
    CargarFecha();
    CargarEstado();
    SetupParsley();
    Bata.FncCerrarMensaje();
}

function SetupParsley() {
    $.listen('parsley:field:validate', function () {
        Bata.FncValidateParsley(frmConsulta);
    });
}

function CargarFecha() {
    // Cargar DateRangeTime
    dtFechaDesde.daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        calender_style: "picker_1",
        format: 'DD/MM/YYYY',
        minDate: moment("1900-01-01T00:00:00"),
        maxDate: moment(),
        ignoreReadonly: true
    });

    dtFechaHasta.daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        calender_style: "picker_1",
        format: 'DD/MM/YYYY',
        minDate: moment("1900-01-01T00:00:00"),
        maxDate: moment(),
        ignoreReadonly: true
    });
}

function CargarEstado() {
    // Cargar Selects
    var json = [{ id: GLB_DEF_SEL_VALUE, text: GLB_DEF_SEL_VALUE }, { id: 'PF', text: 'Pedientes' }, { id: 'PDE', text: 'Facturados' }]
    Bata.FncEstableceSelectJson(selEstado, json);
}

function MostrarReporte2(e) {
    e.preventDefault();

    // Validar FrontEnd
    if (frmConsulta.parsley().validate() === false)
        return;

    // Validar Params
    var pFecDes = $.trim(dtFechaDesde.val());
    var pFecHas = $.trim(dtFechaHasta.val());
    var pEst = $.trim(selEstado.val());
    var pCli = $.trim(txtCliente.val());

    pFecDes = (pFecDes == '') ? pFecDes : Bata.FncConvertirFechaDB(pFecDes);
    pFecHas = (pFecHas == '') ? pFecHas : Bata.FncConvertirFechaDB(pFecHas);
    pEst = (pEst == GLB_DEF_SEL_VALUE) ? '' : pEst;

    Bata.FncDeshabilitarControl(btnConsultar);
    Bata.FncMostrarMensaje(GLB_INF_APP_NOMBRE, null, Bata.TipoMensaje.Loader, null, null);
    var GLB_RUT_APP_REPVENR = Url.Action("ReporteVentas", "Reporte", null, Request.Url.Scheme);

    $.ajax({
        url: GLB_RUT_APP_REPVENR + "?pFecDes=" + pFecDes + "&pFecHas=" + pFecHas + "&pEst=" + pEst + "&pCli=" + pCli,
        cache: false,
        async: false,
        dataType: "html",
        success: function (data) {
            ifrReporte.html(data);
            return false;
        },
        error: function (request, status, error) {
            Bata.FncMostrarMensaje(GLB_INF_APP_NOMBRE, GLB_MSJ_DES_ERROR, Bata.TipoMensaje.Informacion, null, null);
        }
    }).done(function () {
        Bata.FncCerrarMensaje();
    });

    Bata.FncHabilitarControl(btnConsultar);
}

function Limpiar(e) {
    e.preventDefault();
    Bata.FncMostrarMensaje(GLB_INF_APP_NOMBRE, null, Bata.TipoMensaje.Loader, null, null);
    Bata.FncLlenarControl(txtCliente, '');
    Bata.FncLlenarControl(dtFechaDesde, '');
    Bata.FncLlenarControl(dtFechaHasta, '');
    Bata.FncLlenarControl(selEstado, GLB_DEF_SEL_VALUE);
    Bata.FncCerrarMensaje();
}



