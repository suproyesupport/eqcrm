$(document).ready(InicionEventos);

function InicionEventos() {
    LlenarTablaLineaI();

}

function GuardaLinea() {

    $('#modal-guardalinea').modal('show');


}


function GuadarLineaI() {

    $('#modal-guardalinea').modal('hide');

    var id_linea = document.getElementById("id_linea").value;
    var descripcion = document.getElementById("descripcion").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoDeptos/InsertarDepto",
        data: { id_linea, descripcion },
        success: function (response) {
            var arreglo = response;


            if (response.CODIGO == "ERROR") {

                if (response.PRODUCTO == "") {
                    $('#Error').html("NO SE ENCUENTRA EN LA BASE DE DATOS");
                    $('#ModalError').modal('show');
                    return;
                }
                else {
                    $('#Error').html(response.PRODUCTO);
                    $('#ModalError').modal('show');
                    return;
                }
            }

            document.getElementById("id_linea").value = "";
            document.getElementById("descripcion").value = "";

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
        url: "/IngresoDeptos/ConsultaDepto",
        data: {},
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}

