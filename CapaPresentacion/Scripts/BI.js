BI = {

    checkRadioBox: function (nameRadioBox) {
        return $(nameRadioBox).is(":checked") ? true : false;
    },

    checkTextarea: function (idText) {
        return $(idText).val().length > 8 ? true : false;
    },

    checkSelect: function (idSelect) {
        return $(idSelect).val() ? true : false;
    },
    Mayuscula: function (e, elemento) {
        elemento.value = elemento.value.toUpperCasfe();
    },

    Completar: function (ctrl, len) {
        var numero = ctrl.value;
        if (numero.length == len || numero.length == 0) return true;
        for (var i = 1; numero.length < len; i++) {
            numero = '0' + numero;
        }
        ctrl.value = numero;
        return true;
    },

    PadLeft: function (value, len, character) {
        len = len - value.length;
        for (var i = 1; i <= len; i++) {
            value = character + value;
        }
        return value;
    },

    AjaxGetHtml: function (url) {
        var rsp = null;
        $.ajax({
            type: "get",
            url: url,
            contentType: "application/json;charset=utf-8",
            dataType: "html",
            success: function (response) {
                //alert(response);
                rsp = response;
                console.log(rsp);
            },
            error: function (request, status, error) {
                alert(request.statusText);
                console.error(request.responseText);
            }
        });
        return rsp;

    },

    AjaxHtml: function (url, exito, error) {
        //var rsp = null;
        $.ajax({
            type: "get",
            url: url,
            contentType: "application/json;charset=utf-8",
            dataType: "html",
            success: exito,
            error: error
        });
        //return rsp;

    },

    AjaxHtmlSeg: function (url, exito, error) {
        //var rsp = null;
        $.ajax({
            type: "get",
            url: url,
            contentType: "application/json;charset=utf-8",
            dataType: "html",
            beforeSend: function (xhr) { xhr.setRequestHeader('wts', 'wts*123'); },
            success: exito,
            error: error
        });
        //return rsp;

    },

    Ajax: function (url, parameters, async) {
        var rsp = null;
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: async,
            data: JSON.stringify(parameters),
            success: function (response) {
                rsp = response;
            },
            error: function (request, status, error) {
                BI.AbrirPopup(20, 15, "Comuniquese con Soporte Tecnico<br>" + request.statusText, 'Disculpe!!!', false, true, false)
                //alert(request.statusText);
                //console.error(request.responseText);
            }
        });
        return rsp;
    },

    AjaxSeg: function (url, parameters, async) {
        var rsp = null;
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) { xhr.setRequestHeader('wts', 'wts*123'); },
            async: async,
            data: JSON.stringify(parameters),
            success: function (response) {
                rsp = response;
            },
            error: function (request, status, error) {
                BI.AbrirPopup(20, 15, "Comuniquese con Soporte Tecnico<br>" + request.statusText, 'Disculpe!!!', false, true, false)
                //alert(request.statusText);
                //console.error(request.responseText);
            }
        });
        return rsp;
    },

    AjaxJson: function (type, url, parameters, async, exito, error) {
        //var rsp = null;
        $.ajax({
            type: type,
            url: url,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: async,
            data: JSON.stringify(parameters),
            success: exito,
            error: error
        });
        //return rsp;
    },

    AjaxJsonSeg: function (type, url, parameters, async, exito, error) {
        //var rsp = null;
        $.ajax({
            type: type,
            url: url,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) { xhr.setRequestHeader('wts', 'wts*123'); },
            async: async,
            data: JSON.stringify(parameters),
            success: exito,
            error: error
        });
        //return rsp;
    },

    AjaxFormData: function (url, formData, exito, error) {
        $.ajax({
            type: "POST",
            url: url,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: exito,
            error: error
        });


    },

    AjaxFormDataSeg: function (url, formData, exito, error) {
        $.ajax({
            type: "POST",
            url: url,
            data: formData,
            dataType: 'json',
            beforeSend: function (xhr) { xhr.setRequestHeader('wts', 'wts*123'); },
            contentType: false,
            processData: false,
            success: exito,
            error: error
        });


    },

    GetAsyncxhr: function (url, exito, error) {
        var xhr = new XMLHttpRequest();
        xhr.open("get", url, true);
        xhr.setRequestHeader("Content-Type", "text/plain");
        xhr.onload = function () {
            if (xhr.readyState == 4 && xhr.status == 200) { exito(xhr.responseText); }
            else error(xhr.statusText);
        }
        xhr.send();
    },

    CargarComboText: function (rspa, cbo, separadorCampo, separadorRegistro, ValorInicial, TextoInicial) {
        var n = rspa.split(separadorRegistro.trim()).length;
        var lista = rspa.split(separadorRegistro.trim());
        var opcion;
        var campos;

        if (ValorInicial != null && TextoInicial != null) {
            opcion = document.createElement("option");
            opcion.value = ValorInicial;
            opcion.innerText = TextoInicial;
            opcion.text = TextoInicial;
            combo.appendChild(opcion);
        }

        for (var i = 0; i < n; i++) {
            campos = lista[i].split(separadorCampo.trim());
            opcion = document.createElement("option");
            opcion.value = campos[0];
            opcion.innerHTML = campos[1];
            opcion.text = campos[1];
            cbo.appendChild(opcion);
        }
    },

    CargarComboJson: function (lista, combo, ValorInicial, TextoInicial, CampoValor, CampoDescripcion) {
        var n = lista.length;
        var opcion;
        var campos;
        if (ValorInicial != null && TextoInicial != null) {
            opcion = document.createElement("option");
            opcion.value = ValorInicial;
            opcion.innerText = TextoInicial;
            opcion.text = TextoInicial;
            combo.appendChild(opcion);

        }
        if (CampoValor != null || CampoDescripcion != null) {
            for (i = 0; i < n; i++) {
                opcion = document.createElement("option");
                opcion.value = lista[i][CampoValor];
                opcion.innerText = lista[i][CampoDescripcion];
                opcion.text = lista[i][CampoDescripcion];
                combo.appendChild(opcion);
            }

        }

    },

    ParseDate: function (date) {
        var parts = date.split("/");
        var fecha = new Date(parts[1] + "/" + parts[0] + "/" + parts[2]);
        return fecha.getTime();
    },

    ValidarCombo: function (args) {
        var isvalid = true;
        $.each(args, function (index, item) {
            var combo = jQuery('#' + item);
            var valor = combo.val();

            if (valor == 0 || valor == null) {
                var padre = combo.parent();
                var haySpan = padre.find("span").length;

                if (haySpan == 0) {
                    padre.append("<span class='error'>*</span>");
                }
                isvalid = false;
            }
        });
        return isvalid;
    },

    ObtenerFormulario: function (url, contenedorInformacion) {
        $.ajax({
            url: url,
            cache: false,
            dataType: 'html',
            success: function (result) {
                $('#' + contenedorInformacion).show();
                $('#' + contenedorInformacion).html(result);
            },
            error: function (request, status, error) {
                $('#' + contenedorInformacion).hide();
                alert(request.responseText);
            }
        });
    },

    MostrarInformacion: function (url, contenedorListado, contenedorInformacion) {
        $.ajax({
            url: url,
            cache: false,
            dataType: 'html',
            success: function (result) {
                $('#' + contenedorListado).hide();
                $('#' + contenedorInformacion).show();
                $('#' + contenedorInformacion).html(result);
            },
            error: function (request, status, error) {
                $('#' + contenedorListado).show();
                $('#' + contenedorInformacion).hide();
                BI.AbrirPopup(45, 35, 'Comuniquese con Soporte' + request.statusText, 'Disculpe!!!', false, true, false);
                //alert(request.responseText);
            }
        });
    },


    MostrarInformacionPopup: function (url, contenedorTabla, contenedorInformacion) {
        $.ajax({
            url: url,
            dataType: 'html',
            async: false,
            cache: false,
            success: function (result) {
                $('#' + contenedorInformacion).html(result);
                $('#' + contenedorInformacion).dialog("open");
            },
            error: function (request, status, error) {
                alert(request.responseText);
            }
        });
    },

    LoadDropDownList: function (name, url, parameters, selected) {
        var combo = document.getElementById(name);
        combo.options.length = 0;
        combo.options[0] = new Option("");
        combo.selectedIndex = 0;

        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(parameters),
            success: function (items) {
                var list = (typeof items.d) == 'string' ? eval('(' + items.d + ')') : items.d;

                $.each(list, function (index, item) {
                    combo.options[index] = new Option(item.Nombre, item.IdComun);
                });
                if (selected == undefined) selected = 0;
                $('#' + name).val(selected);
            }
        });
    },

    LoadDropDownListItems: function (name, url, parameters, selected) {
        var combo = document.getElementById(name);
        combo.options.length = 0;
        combo.options[0] = new Option("");
        combo.selectedIndex = 0;
        combo.disabled = true;

        var resultado = BI.Ajax(url, parameters, false);

        $.each(resultado, function (index, item) {
            combo.options[index] = new Option(item.Text, item.Value);
        });
        combo.disabled = false;
        if (selected == undefined) selected = 0;
        $('#' + name).val(selected);
    },

    Clear: function (divName) {
        $('#' + divName + ' select').children().remove();
        var elemt = $('#' + divName);
        $(':input', elemt).each(function () {
            var type = this.type;
            var tag = this.tagName.toLowerCase();
            if (type == 'text' || type == 'password' || tag == 'textarea')
                this.value = "";
            else if (type == 'checkbox' || type == 'radio')
                this.checked = false;
            else if (tag == 'select')
                this.selectedIndex = -1;
        });
    },

    Regresar: function Regresar(tabla, contenedorListado, contenedorInformacion) {
        try {
            $('#' + tabla)[0].clearToolbar();
        } catch (e) { }
        $('#' + tabla).trigger('reloadGrid');
        $('#' + contenedorListado).show();
        $('#' + contenedorInformacion).html('');
        $('#' + contenedorInformacion).hide();
    },

    RegresarPopup: function Regresar(contenedorInformacion) {
        $('#' + contenedorInformacion).dialog("close");
        Refrescar();
    },

    ValidarDecimal: function (numero) {
        var patron = /^([0-9])*[.]?[0-9]* $/;
        if (patron.test(numero))
            return true;
        return false;
    },

    ValidarFecha: function (fecha) {
        var patron = /^((([0][1-9]|[12][\d])|[3][01])[-\/]([0][13578]|[1][02])[-\/][1-9]\d\d\d)|((([0][1-9]|[12][\d])|[3][0])[-\/]([0][13456789]|[1][012])[-\/][1-9]\d\d\d)|(([0][1-9]|[12][\d])[-\/][0][2][-\/][1-9]\d([02468][048]|[13579][26]))|(([0][1-9]|[12][0-8])[-\/][0][2][-\/][1-9]\d\d\d)$/;
        if (patron.test(fecha))
            return true;
        return false;
    },

    ValidarEntero: function (numero) {
        var patron = /^\d+$/;
        if (patron.test(numero))
            return true;
        return false;
    },

    CrearPopup: function (nombre) {
        $('#' + nombre).dialog({
            width: 'auto',
            resizable: false,
            modal: true,
            autoOpen: false,
            closeOnEscape: false,
            open: function (event, ui) {
                $(this).parent().appendTo("form");
                $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
            },
            close: function () { }
        });
    },

    AbrirPopupView: function (porcentajeAncho, porcentajeAlto, rpta, titulo, btnGuardar, btnClose, btnEliminar) {

        $("#myModal #modal-content #IdContenido").empty();
        $("#IdContenido").empty();
        //$("#piemodal").empty();
        /*var pie = "<button type='button' class='btn btn-default' data-dismiss='modal' id='btnClose'>Cerrar</button>" +
                  "<button type='button' class='btn btn-primary' id='btnGuardar'>Guardar</button>"; +
                  "<button type='button' class='btn btn-danger' id='btnEliminar'>Eliminar</button>"*/
        //$("#piemodal").html(pie);
        //var pie = "<div class='modal-footer' id='piemodal'>" +
        //          "<button type='button' class='btn btn-default' data-dismiss='modal' id='btnClose'>Close</button>" +
        //          "<button type='button' class='btn btn-primary' id='btnGuardar'>Save</button>" +
        //          "<button type='button' class='btn btn-danger' id='btnEliminar'>Delete</button>"
        //$("#piemodal").html(pie);
        //$("#piemodal").html()
        //sb.AppendLine("<div class='modal-footer' id='piemodal'>");
        //sb.AppendLine("<button type='button' class='btn btn-default' data-dismiss='modal' id='btnClose'>Close</button>");
        //sb.AppendLine("<button type='button' class='btn btn-primary' id='btnGuardar'>Save</button>");
        //sb.AppendLine("<button type='button' class='btn btn-danger' id='btnEliminar'>Delete</button>");

        $('#piemodal').show();
        $("#myModal .modal-header").css('padding', '9px 15px');
        $("#myModal .modal-header").css('border-bottom', '1px solid #eee');
        //$("#myModal .modal-header").css('background-color','#255b96');
        $("#myModal .modal-header").css('background-color', '#3c8dbc');
        $("#myModal .modal-header").css('color', 'white');
        $("#myModal .modal-header").css('font-weight', 'bold');
        $("#myModal .modal-header").css('-webkit-border-top-left-radius', '5px');
        $("#myModal .modal-header").css('-webkit-border-top-right-radius', '5px');
        $("#myModal .modal-header").css('-moz-border-radius-topleft', '5px');
        $("#myModal .modal-header").css('-moz-border-radius-topright', '5px');
        $("#myModal .modal-header").css('border-top-left-radius', '5px');
        $("#myModal .modal-header").css('border-top-right-radius', '5px');
        $("#myModal .modal-footer").css('margin-top', '0px');

        if (porcentajeAncho < 0) {
            porcentajeAncho = porcentajeAncho * -1;
            $("#myModal .modalTamanio").css('width', porcentajeAncho + "%");
        } else {
            $("#myModal .modalTamanio").css('width', "");
        }

        $("#myModal .modal-content").css('height', "");
        //alert(porcentajeAncho);
        //modal-dialog modalTamanio
        //$("#myModal .modal-dialog").css('width', porcentajeAncho + "%");
        //$("#myModal .modal-dialog").css('height', porcentajeAlto + "%");
        //width:'auto', //probably not needed
        //height:'auto', //probably not needed 
        //'max-height':'100%'
        //$("#myModal .modal-body").css('position', 'relative');
        //$("#myModal .modal-body").css('width', porcentajeAncho + "%");
        //$("#myModal .modal-body").css('max-width', porcentajeAncho + "%");
        //$("#myModal .modal-body").css('height', 'auto');
        //.modal-body {
        //    position: relative;
        //    overflow-y: auto;
        //    max-height: 400px;
        //    padding: 15px;
        //}

        $("#myModal #modal-content #IdContenido").html(rpta);
        $("#btnEnviarIniciar").hide();
        $("#btnImprimir").hide();

        (btnGuardar == true ? $("#btnGuardar").show() : $("#btnGuardar").hide());
        (btnClose == true ? $("#btnClose").show() : $("#btnClose").hide());
        (btnEliminar == true ? $("#btnEliminar").show() : $("#btnEliminar").hide());
        $('#myModal').modal({ backdrop: 'static', keyboard: true });
        $('#myModalLabel').text('');
        $('#myModalLabel').text(titulo);
        $('#myModal').modal('show')
    },

    AbrirPopup: function (porcentajeAncho, porcentajeAlto, rpta, titulo, btnGuardar, btnClose, btnEliminar, btnImprimr) {

        $("#btnEnviarIniciar").hide();
        (btnImprimr == true ? $("#btnImprimir").show() : $("#btnImprimir").hide());
        (btnGuardar == true ? $("#btnGuardar").show() : $("#btnGuardar").hide());
        (btnClose == true ? $("#btnClose").show() : $("#btnClose").hide());
        (btnEliminar == true ? $("#btnEliminar").show() : $("#btnEliminar").hide());

        $("#IdContenido").empty();

        $("#myModal .modal-header").css('padding', '9px 15px');
        $("#myModal .modal-header").css('border-bottom', '1px solid #eee');
        $("#myModal .modal-header").css('background-color', '#3c8dbc');
        $("#myModal .modal-header").css('color', 'white');
        $("#myModal .modal-header").css('font-weight', 'bold');
        $("#myModal .modal-header").css('-webkit-border-top-left-radius', '5px');
        $("#myModal .modal-header").css('-webkit-border-top-right-radius', '5px');
        $("#myModal .modal-header").css('-moz-border-radius-topleft', '5px');
        $("#myModal .modal-header").css('-moz-border-radius-topright', '5px');
        $("#myModal .modal-header").css('border-top-left-radius', '5px');
        $("#myModal .modal-header").css('border-top-right-radius', '5px');
        $("#myModal .modal-footer").css('margin-top', '0px');
        $("#myModal .modalTamanio").css('width', "");
        $("#myModal #modal-content #IdContenido").html(rpta);
        $('#myModal').modal({ backdrop: 'static', keyboard: true });
        $('#myModalLabel').text('');
        $('#myModalLabel').text(titulo);
        $('#myModal').modal('show')
    },

    AbrirPopupPrint: function (porcentajeAncho, porcentajeAlto, rpta, titulo, btnGuardar, btnClose, btnEliminar) {

        $("#myModal #modal-content #IdContenido").empty();

        $("#IdContenido").empty();
        //$("#piemodal").empty();
        /*var pie = "<button type='button' class='btn btn-default' data-dismiss='modal' id='btnClose'>Cerrar</button>" +
                  "<button type='button' class='btn btn-primary' id='btnGuardar'>Guardar</button>"; +
                  "<button type='button' class='btn btn-danger' id='btnEliminar'>Eliminar</button>"*/
        //$("#piemodal").html(pie);
        //var pie = "<div class='modal-footer' id='piemodal'>" +
        //          "<button type='button' class='btn btn-default' data-dismiss='modal' id='btnClose'>Close</button>" +
        //          "<button type='button' class='btn btn-primary' id='btnGuardar'>Save</button>" +
        //          "<button type='button' class='btn btn-danger' id='btnEliminar'>Delete</button>"
        //$("#piemodal").html(pie);
        //$("#piemodal").html()
        //sb.AppendLine("<div class='modal-footer' id='piemodal'>");
        //sb.AppendLine("<button type='button' class='btn btn-default' data-dismiss='modal' id='btnClose'>Close</button>");
        //sb.AppendLine("<button type='button' class='btn btn-primary' id='btnGuardar'>Save</button>");
        //sb.AppendLine("<button type='button' class='btn btn-danger' id='btnEliminar'>Delete</button>");

        $("#myModal .modal-header").css('padding', '9px 15px');
        $("#myModal .modal-header").css('border-bottom', '1px solid #eee');
        //$("#myModal .modal-header").css('background-color','#255b96');
        $("#myModal .modal-header").css('background-color', '#3c8dbc');
        $("#myModal .modal-header").css('color', 'white');
        $("#myModal .modal-header").css('font-weight', 'bold');
        $("#myModal .modal-header").css('-webkit-border-top-left-radius', '5px');
        $("#myModal .modal-header").css('-webkit-border-top-right-radius', '5px');
        $("#myModal .modal-header").css('-moz-border-radius-topleft', '5px');
        $("#myModal .modal-header").css('-moz-border-radius-topright', '5px');
        $("#myModal .modal-header").css('border-top-left-radius', '5px');
        $("#myModal .modal-header").css('border-top-right-radius', '5px');
        $("#myModal .modal-footer").css('margin-top', '0px');
        $("#myModal .modal-footer").css('display', 'hide');
        $("#myModal .modalTamanio").css('width', porcentajeAncho + "%");
        $("#myModal .modalTamanio").css('height', porcentajeAlto + "%");
        //$("#myModal .modal-content").css('width', porcentajeAncho + "%");
        $("#myModal .modal-content").css('height', porcentajeAlto + "%");
        //alert(porcentajeAncho);
        //modal-dialog modalTamanio
        $("#myModal .modal-dialog").css('width', porcentajeAncho + "%");
        $("#myModal .modal-dialog").css('height', porcentajeAlto + "%");
        //width:'auto', //probably not needed
        //height:'auto', //probably not needed 
        //'max-height':'100%'
        $("#myModal .modal-body").css('position', 'relative');
        $("#myModal .modal-body").css('width', porcentajeAncho + "%");
        $("#myModal .modal-body").css('max-width', porcentajeAncho + "%");
        $("#myModal .modal-body").css('height', 'auto');
        //.modal-body {
        //    position: relative;
        //    overflow-y: auto;
        //    max-height: 400px;
        //    padding: 15px;
        //}

        $("#myModal #modal-content #IdContenido").html(rpta);
        (btnGuardar == true ? $("#btnGuardar").show() : $("#btnGuardar").hide());
        (btnClose == true ? $("#btnClose").show() : $("#btnClose").hide());
        (btnEliminar == true ? $("#btnEliminar").show() : $("#btnEliminar").hide());
        $('#myModal').modal({ backdrop: 'static', keyboard: true });
        $('#myModalLabel').text('');
        $('#myModalLabel').text(titulo);
        $('#myModal').modal('show')
    },

    CerrarPopup: function () {
        $('#myModal').modal('hide');
    },

    LimpiarPopup: function () {
        $("#IdContenido").empty();
    }
};

function ExportExcel(strTitulo, strAlignTitulo, IdImport, nroColum) {

    var date = new Date();
    var d = date.getDate();
    var day = (d < 10) ? '0' + d : d;
    var m = date.getMonth() + 1;
    var month = (m < 10) ? '0' + m : m;
    var yy = date.getYear();
    var year = (yy < 1000) ? yy + 1900 : yy;

    var f = new Date();
    var sigHora = 'am'

    if (f.getHours() > 12)
        sigHora = 'pm'

    var h = f.getHours();
    var nroHora = (h < 10) ? '0' + h : h;

    var M = f.getMinutes();
    var nroM = (M < 10) ? '0' + M : M;

    var Fecha = 'Fecha%20:%20' + day + "/" + month + "/" + year;
    var hora = 'Hora%20:%20' + nroHora + ":" + nroM + '%20' + sigHora;

    strTitulo = strTitulo.replace(/\s/g, "%20");

    var ArstrTitulo = strTitulo.split(",")

    var StrEncabezado = '%3Ctable%20border%3D%221px%22%20bordercolor%3D%22%23DFDFDF%22%20class%3D%22table%20%22%3E%3Ctbody%3E%3C'
    var Cabecera1 = 'tr%3E%3Ctd%20colspan%3D%22' + nroColum + '%22%20style%3D%22color%3Bfont-weight%3A%20normal%3Btext-align%3A' + strAlignTitulo + '%22%3E%3Cb%3E' + ArstrTitulo[0] + '%20%20%3C%2Fb%3E%3C%2Ftd%3E%3C%2Ftr%3E%3C'

    if (ArstrTitulo.length > 1)
        Cabecera1 = Cabecera1 + 'tr%3E%3Ctd%20colspan%3D%22' + nroColum + '%22%20style%3D%22color%3Bfont-weight%3A%20normal%3Btext-align%3A' + strAlignTitulo + '%22%3E%3Cb%3E' + ArstrTitulo[1] + '%20%20%3C%2Fb%3E%3C%2Ftd%3E%3C%2Ftr%3E%3C'

    if (ArstrTitulo.length > 2)
        Cabecera1 = Cabecera1 + 'tr%3E%3Ctd%20colspan%3D%22' + nroColum + '%22%20style%3D%22color%3Bfont-weight%3A%20normal%3Btext-align%3A' + strAlignTitulo + '%22%3E%3Cb%3E' + ArstrTitulo[2] + '%20%20%3C%2Fb%3E%3C%2Ftd%3E%3C%2Ftr%3E%3C'

    Cabecera1 = Cabecera1 + 'tr%3E%3Ctd%20colspan%3D%22' + nroColum + '%22%20style%3D%22color%3Bfont-weight%3A%20normal%3Btext-align%3A' + strAlignTitulo + '%22%3E%3Cb%3E' + Fecha + '%20%20%3C%2Fb%3E%3C%2Ftd%3E%3C%2Ftr%3E%3C'
    Cabecera1 = Cabecera1 + 'tr%3E%3Ctd%20colspan%3D%22' + nroColum + '%22%20style%3D%22color%3Bfont-weight%3A%20normal%3Btext-align%3A' + strAlignTitulo + '%22%3E%3Cb%3E' + hora + '%20%20%3C%2Fb%3E%3C%2Ftd%3E%3C%2Ftr%3E%3C'
    var strRowEmpty = 'tr%3E%3Ctd%20colspan%3D%22' + nroColum + '%22%20%2Ftd%3E%3C%2Ftr%3E%3C'
    var TableFin = 'tbody%3E%3C%2Ftable%3E'


    StrEncabezado = StrEncabezado + Cabecera1
    StrEncabezado = StrEncabezado + strRowEmpty
    StrEncabezado = StrEncabezado + strRowEmpty
    StrEncabezado = StrEncabezado + strRowEmpty
    StrEncabezado = StrEncabezado + TableFin

    StrEncabezado = StrEncabezado + encodeURIComponent($('#' + IdImport).html())
    window.open('data:application/vnd.ms-excel,' + StrEncabezado);


}

function RepetirCadena(cadena, nro) {
    var cadenaTotal = ''

    while (nro > 0) {
        cadenaTotal = cadenaTotal + cadena
        nro = nro - 1;

    }

    return cadenaTotal
}


function CombinarJson(json1, json2) {

    /*var json1 = JSON.parse(data1);
    var json2 = JSON.parse(data2);*/

    var merged = {};
    for (var i in json1) {
        if (json1.hasOwnProperty(i))
            merged[i] = json1[i];
    }
    for (var i in json2) {
        if (json2.hasOwnProperty(i))
            merged[i] = json2[i];
    }
    return merged;
}

(function ($) { var d = $(document), h = $("head"), drag = null, tables = [], count = 0, ID = "id", PX = "px", SIGNATURE = "JColResizer", FLEX = "JCLRFlex", I = parseInt, M = Math, ie = navigator.userAgent.indexOf('Trident/4.0') > 0, S; try { S = sessionStorage } catch (e) { }; h.append("<style type='text/css'>  .JColResizer{table-layout:fixed;} .JColResizer td, .JColResizer th{overflow:hidden;padding-left:0!important; padding-right:0!important;}  .JCLRgrips{ height:0px; position:relative;} .JCLRgrip{margin-left:-5px; position:absolute; z-index:5; } .JCLRgrip .JColResizer{position:absolute;background-color:red;filter:alpha(opacity=1);opacity:0;width:10px;height:100%;cursor: e-resize;top:0px} .JCLRLastGrip{position:absolute; width:1px; } .JCLRgripDrag{ border-left:1px dotted black;	} .JCLRFlex{width:auto!important;}</style>"); var init = function (tb, options) { var t = $(tb); t.opt = options; if (t.opt.disable) return destroy(t); var id = t.id = t.attr(ID) || SIGNATURE + count++; t.p = t.opt.postbackSafe; if (!t.is("table") || tables[id] && !t.opt.partialRefresh) return; t.addClass(SIGNATURE).attr(ID, id).before('<div class="JCLRgrips"/>'); t.g = []; t.c = []; t.w = t.width(); t.gc = t.prev(); t.f = t.opt.fixed; if (options.marginLeft) t.gc.css("marginLeft", options.marginLeft); if (options.marginRight) t.gc.css("marginRight", options.marginRight); t.cs = I(ie ? tb.cellSpacing || tb.currentStyle.borderSpacing : t.css('border-spacing')) || 2; t.b = I(ie ? tb.border || tb.currentStyle.borderLeftWidth : t.css('border-left-width')) || 1; tables[id] = t; createGrips(t) }, destroy = function (t) { var id = t.attr(ID), t = tables[id]; if (!t || !t.is("table")) return; t.removeClass(SIGNATURE + " " + FLEX).gc.remove(); delete tables[id] }, createGrips = function (t) { var th = t.find(">thead>tr>th,>thead>tr>td"); if (!th.length) th = t.find(">tbody>tr:first>th,>tr:first>th,>tbody>tr:first>td, >tr:first>td"); t.cg = t.find("col"); t.ln = th.length; if (t.p && S && S[t.id]) memento(t, th); th.each(function (i) { var c = $(this), g = $(t.gc.append('<div class="JCLRgrip"></div>')[0].lastChild); g.append(t.opt.gripInnerHtml).append('<div class="' + SIGNATURE + '"></div>'); if (i == t.ln - 1) { g.addClass("JCLRLastGrip"); if (t.f) g.html("") }; g.bind('touchstart mousedown', onGripMouseDown); g.t = t; g.i = i; g.c = c; c.w = c.width(); t.g.push(g); t.c.push(c); c.width(c.w).removeAttr("width"); g.data(SIGNATURE, { i: i, t: t.attr(ID), last: i == t.ln - 1 }) }); t.cg.removeAttr("width"); syncGrips(t); t.find('td, th').not(th).not('table th, table td').each(function () { $(this).removeAttr('width') }); if (!t.f) t.removeAttr('width').addClass(FLEX) }, memento = function (t, th) { var w, m = 0, i = 0, aux = [], tw; if (th) { t.cg.removeAttr("width"); if (t.opt.flush) { S[t.id] = ""; return }; w = S[t.id].split(";"); tw = w[t.ln + 1]; if (!t.f && tw) t.width(tw); for (; i < t.ln; i++) { aux.push(100 * w[i] / w[t.ln] + "%"); th.eq(i).css("width", aux[i]) }; for (i = 0; i < t.ln; i++) t.cg.eq(i).css("width", aux[i]) } else { S[t.id] = ""; for (; i < t.c.length; i++) { w = t.c[i].width(); S[t.id] += w + ";"; m += w }; S[t.id] += m; if (!t.f) S[t.id] += ";" + t.width() } }, syncGrips = function (t) { t.gc.width(t.w); for (var i = 0; i < t.ln; i++) { var c = t.c[i]; t.g[i].css({ left: c.offset().left - t.offset().left + c.outerWidth(false) + t.cs / 2 + PX, height: t.opt.headerOnly ? t.c[0].outerHeight(false) : t.outerHeight(false) }) } }, syncCols = function (t, i, isOver) { var inc = drag.x - drag.l, c = t.c[i], c2 = t.c[i + 1], w = c.w + inc, w2 = c2.w - inc; c.width(w + PX); t.cg.eq(i).width(w + PX); if (t.f) { c2.width(w2 + PX); t.cg.eq(i + 1).width(w2 + PX) }; if (isOver) { c.w = w; c2.w = t.f ? w2 : c2.w } }, applyBounds = function (t) { var w = $.map(t.c, function (c) { return c.width() }); t.width(t.width()).removeClass(FLEX); $.each(t.c, function (i, c) { c.width(w[i]).w = w[i] }); t.addClass(FLEX) }, onGripDrag = function (e) { if (!drag) return; var t = drag.t, oe = e.originalEvent.touches, ox = oe ? oe[0].pageX : e.pageX, x = ox - drag.ox + drag.l, mw = t.opt.minWidth, i = drag.i, l = t.cs * 1.5 + mw + t.b, last = i == t.ln - 1, min = i ? t.g[i - 1].position().left + t.cs + mw : l, max = t.f ? i == t.ln - 1 ? t.w - l : t.g[i + 1].position().left - t.cs - mw : Infinity; x = M.max(min, M.min(max, x)); drag.x = x; drag.css("left", x + PX); if (last) { var c = t.c[drag.i]; drag.w = c.w + x - drag.l }; if (t.opt.liveDrag) { if (last) { c.width(drag.w); t.w = t.width() } else syncCols(t, i); syncGrips(t); var cb = t.opt.onDrag; if (cb) { e.currentTarget = t[0]; cb(e) } }; return false }, onGripDragOver = function (e) { d.unbind('touchend.' + SIGNATURE + ' mouseup.' + SIGNATURE).unbind('touchmove.' + SIGNATURE + ' mousemove.' + SIGNATURE); $("head :last-child").remove(); if (!drag) return; drag.removeClass(drag.t.opt.draggingClass); var t = drag.t, cb = t.opt.onResize, i = drag.i, last = i == t.ln - 1, c = t.g[i].c; if (last) { c.width(drag.w); c.w = drag.w } else syncCols(t, i, true); if (!t.f) applyBounds(t); syncGrips(t); if (cb) { e.currentTarget = t[0]; cb(e) }; if (t.p && S) memento(t); drag = null }, onGripMouseDown = function (e) { var o = $(this).data(SIGNATURE), t = tables[o.t], g = t.g[o.i], oe = e.originalEvent.touches; g.ox = oe ? oe[0].pageX : e.pageX; g.l = g.position().left; d.bind('touchmove.' + SIGNATURE + ' mousemove.' + SIGNATURE, onGripDrag).bind('touchend.' + SIGNATURE + ' mouseup.' + SIGNATURE, onGripDragOver); h.append("<style type='text/css'>*{cursor:" + t.opt.dragCursor + "!important}</style>"); g.addClass(t.opt.draggingClass); drag = g; if (t.c[o.i].l) for (var i = 0, c; i < t.ln; i++) { c = t.c[i]; c.l = false; c.w = c.width() }; return false }, onResize = function () { for (t in tables) { var t = tables[t], i, mw = 0; t.removeClass(SIGNATURE); if (t.f && t.w != t.width()) { t.w = t.width(); for (i = 0; i < t.ln; i++) mw += t.c[i].w; for (i = 0; i < t.ln; i++) t.c[i].css("width", M.round(1e3 * t.c[i].w / mw) / 10 + "%").l = true }; syncGrips(t.addClass(SIGNATURE)) } }; $(window).bind('resize.' + SIGNATURE, onResize); $.fn.extend({ colResizable: function (options) { var defaults = { draggingClass: 'JCLRgripDrag', gripInnerHtml: '', liveDrag: false, fixed: true, minWidth: 15, headerOnly: false, hoverCursor: "e-resize", dragCursor: "e-resize", postbackSafe: false, flush: false, marginLeft: null, marginRight: null, disable: false, partialRefresh: false, onDrag: null, onResize: null }, options = $.extend(defaults, options); return this.each(function () { init(this, options) }) } }) })(jQuery)

