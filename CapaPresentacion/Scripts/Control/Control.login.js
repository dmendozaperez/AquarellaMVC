// =======================================================================//
// ! Declaración de Variables Globales                                    //
// =======================================================================//
var txtUsuario, txtContrasena, chkRecordar, btnEnviar, frmLogin;

$(document).ready(function () {
    // =======================================================================//
    // ! Asignar Variables Globales                                           //
    // =======================================================================//
    txtUsuario = $("#txtUsuario");
    txtContrasena = $("#txtContrasena");
    //chkRecordar = $("#chkRecordar");
    btnEnviar = $('#btnEnviar');
    frmLogin = $('#frmLogin');
    Iniciar();
});

// Configuracion
function Iniciar() {
    SetupParsley();
}

function SetupParsley() {
    $.listen('parsley:field:validate', function () {
        Bata.FncValidateParsley(frmLogin);
    });
}


function Login(e) {
    // Validar FrontEnd
    e.preventDefault();
    if (frmLogin.parsley().validate() === false)
        return;

    // Enviar Form
    Bata.FncDeshabilitarControl(btnEnviar);

    var request = new FormData();
    alert(txtUsuario.val())
    request.append('Usuario', txtUsuario.val());
    request.append('Password', txtContrasena.val());

    $.ajax({
        url: GLB_RUT_APP_LOGIN,
        processData: false,
        contentType: false,
        type: 'POST',
        async: true,
        data: request,
        success: function (data) {
            if (data.Tipo == Bata.Notifica.Error) {
                //Bata.FncMostrarAlerta(GLB_ALT_TITLE, data.Mensaje, Bata.Notifica.Error, Bata.NotificaPos.Bottom);
            }
            if (data.Tipo == Bata.Notifica.Exito) {
                //Bata.FncMostrarAlerta(GLB_ALT_TITLE, data.Mensaje, Bata.Notifica.Exito, Bata.NotificaPos.Bottom);
                //var param = { sid: data.Token };
                //Bata.FncRedirigePost(GLB_RUT_APP_PANEL, param);
            }
            //Bata.FncEstableceFoco(txtUsuario);
            return;
        },
        error: GLB_MSJ_DES_ERROR
    });

    Bata.FncHabilitarControl(btnEnviar);
}