$(document).ready(InicionEventos);

function InicionEventos() {
    LlenarTabla();
}

//function GuardarTipo() {
//    $('#modal-guardatipo').modal('show');
//}

function GuardarMarca() {

    //$('#modal-guardatipo').modal('hide');

    let descripcion = document.querySelector("#descripcion").value;


    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/CatalogoMarcas/InsertarMarca",
        data: { descripcion },
        success: function (response) {
            var arreglo = response;

            if (response.CODIGO == "ERROR") {
                if (response.PRODUCTO == "") {
                    $('#Error').html("OCURRIO UN ERROR");
                    $('#ModalError').modal('show');
                    return;
                }
                else {
                    $('#Error').html(response.PRODUCTO);
                    $('#ModalError').modal('show');
                    return;
                }
            }

            location.reload();

            //ActFiltroInventario();

            //$('#ModalInfoAdicional').modal('show');
            //BuscarID();
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



function LlenarTabla() {
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/CatalogoMarcas/ConsultaMarcas",
        data: {},
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}