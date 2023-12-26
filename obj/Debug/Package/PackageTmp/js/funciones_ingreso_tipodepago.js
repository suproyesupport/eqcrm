$(document).ready(InicionEventos);

function InicionEventos() {
    LlenarTablaTipodePago();
}

function GuardarTipo() {
    $('#modal-guardatipo').modal('show');
}

function GuardarTipodePago() {

    $('#modal-guardatipo').modal('hide');

    var descripcion = document.getElementById("descripcion").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/CatTipodePago/InsertarTipodePago",
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

            //ActFiltroInventario();

            //$('#ModalInfoAdicional').modal('show');
            //BuscarID();
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

    window.location.href = "/CatTipodePago/CatTipodePago";
}



function LlenarTablaTipodePago() {
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/CatTipodePago/ConsultaTipodePago",
        data: {},
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}