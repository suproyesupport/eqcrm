$(document).ready(InicionEventos);

function InicionEventos() {
    LlenarTablaLineaI();

}

function GuardaLinea() {

    $('#modal-guardalinea').modal('show');


}


function GuadarLineaI() {

    $('#modal-guardalinea').modal('hide');

    var Empresa = document.getElementById("Empresa").value;
    var BaseDatos = document.getElementById("BaseDatos").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/EmpresaLineaNegocio/InsertarLineaNegocio",
        data: { Empresa, BaseDatos },
        success: function (response) {
            var arreglo = response;


            if (response.CODIGO == "ERROR") {

                if (response.PRODUCTO == "") {
                    $('#Error').html("LA LINEA NO SE ENCUENTRA EN LA BASE DE DATOS");
                    $('#ModalError').modal('show');
                    return;
                }
                else {
                    $('#Error').html(response.PRODUCTO);
                    $('#ModalError').modal('show');
                    return;
                }
            }

            document.getElementById("Empresa").value = "";
            document.getElementById("BaseDatos").value = "";

            LlenarTablaLineaI();

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


    

}

function LlenarTablaLineaI() {
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/EmpresaLineaNegocio/ConsultaLineaNegocios",
        data: {},
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}

