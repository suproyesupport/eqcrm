$(document).ready(BuscarOrden);

function seleccionar(id) {
    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");

    document.getElementById("id").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("cliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[1].innerHTML;
    document.getElementById("direccion").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[2].innerHTML;
    document.getElementById("telefono").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[5].innerHTML;

    $("#ayudaclientes").modal('hide');
}

function BuscarOrden() {
    var id_orden = document.getElementById("id_orden").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/CableVision/BuscarOrden",
        data: { id_orden },
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
            id_orden = response.ID + 1;

            $("#id_orden").val(id_orden);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}


function GuardarOS() {

    //Obtener value del dropdownlist
    var comboruta = document.getElementById("ruta").value;
    var combotecnico = document.getElementById("tecnico").value;
    var combotipo = document.getElementById("tipo").value;
    var id = document.getElementById("id").value; //id Cliente
    var cliente = document.getElementById("cliente").value;
    
    if (id == '') {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el ID de cliente.";
        $("#modal-generico").modal('show');
        return;
    }

    if (cliente == '') {

        document.getElementById("mensaje").innerHTML = "Debe ingresar un cliente.";
        $("#modal-generico").modal('show');
        return;
    }

    if (comboruta == '') {

        document.getElementById("mensaje").innerHTML = "Debe seleccionar una ruta.";
        $("#modal-generico").modal('show');
        return;
    }

    if (combotecnico == '') {

        document.getElementById("mensaje").innerHTML = "Debe seleccionar un técnico.";
        $("#modal-generico").modal('show');
        return;
    }

    if (combotipo == '') {

        document.getElementById("mensaje").innerHTML = "Debe seleccionar un técnico.";
        $("#modal-generico").modal('show');
        return;
    }

    $('#modal-guardaos').modal('show');

}

function GuardarOrden() {

    //Obtener value del dropdownlist
    var comboruta = document.getElementById("ruta");
    var selectedruta = comboruta.options[comboruta.selectedIndex].value;

    var combotecnico = document.getElementById("tecnico");
    var selectedtecnico = combotecnico.options[combotecnico.selectedIndex].value;

    var combotipo = document.getElementById("tipo");
    var selectedtipo = combotipo.options[combotipo.selectedIndex].value;

    var id_orden = document.getElementById("id_orden").value;
    var id = document.getElementById("id").value; //id Cliente
    var cliente = document.getElementById("cliente").value;
    var direccion = document.getElementById("direccion").value;
    var telefono = document.getElementById("telefono").value;
    var id_ruta = selectedruta;
    var id_tecnico = selectedtecnico;
    var id_tipoorden = selectedtipo;
    var obs = document.getElementById("obs").value;

    //if (id == '') {

    //    document.getElementById("mensaje").innerHTML = "Debe ingresar el ID de cliente.";
    //    $("#modal-generico").modal('show');
    //    return;
    //}

    //if (cliente == '') {

    //    document.getElementById("mensaje").innerHTML = "Debe ingresar un cliente.";
    //    $("#modal-generico").modal('show');
    //    return;
    //}

    //if (comboruta == '') {

    //    document.getElementById("mensaje").innerHTML = "Debe seleccionar una ruta.";
    //    $("#modal-generico").modal('show');
    //    return;
    //}

    //if (combotecnico == '') {

    //    document.getElementById("mensaje").innerHTML = "Debe seleccionar un técnico.";
    //    $("#modal-generico").modal('show');
    //    return;
    //}

    //if (combotipo == '') {

    //    document.getElementById("mensaje").innerHTML = "Debe seleccionar un técnico.";
    //    $("#modal-generico").modal('show');
    //    return;
    //}

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/CableVision/InsertarOrden",
        data: { id_orden, id, cliente, direccion, telefono, id_ruta, id_tecnico, id_tipoorden, obs },
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

            $('#modal-guardaos').modal('hide');
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

    window.location.href = "/CableVision/CableVision";
}