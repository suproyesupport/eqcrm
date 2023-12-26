//CARGAR FECHA
window.onload = function () {
    var fecha = new Date(); //Fecha actual
    var mes = fecha.getMonth() + 1; //obteniendo mes
    var dia = fecha.getDate(); //obteniendo dia
    var ano = fecha.getFullYear(); //obteniendo año
    if (dia < 10)
        dia = '0' + dia; //agrega cero si el menor de 10
    if (mes < 10)
        mes = '0' + mes //agrega cero si el menor de 10
    document.getElementById('fecha').value = ano + "-" + mes + "-" + dia;
    cHtml = "<select class=\"form-control\" id=\"problema\" name=\"problema\" \"><option value=\"\">Seleccionar...</option></select>";
    $('#tipoproblemaa').html(cHtml);
}



/*FUNCION PARA BUSCAR EL TIPO DEL PROBLEMA SEGUN EL COMBOBOX DE PROBLEMA*/
function cargaDDLSecundario(val) {

    problema = $("#problema").val();
    
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoTicket/Cargar",
        data: { problema },
        success: function (response) {
            
            $('#tipoproblemaa').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



//FUNCION PARA ABRIR EL MODAL CON UN BOTON
//CLIENTES F2
function onKeyDownHandlerCliente(event) {

    var codigo = event.which || event.keyCode;

    if (codigo == 113) {
        $('#ayudaclientes').modal('show');

    }
}



//FUNCION SELECCIONAR EL CLIENTE EN EL MODAL
function seleccionar(id) {

    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");

    document.getElementById("id_cliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("cliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[1].innerHTML;
    $("#ayudaclientes").modal('hide');

}



function BuscarCliente() {

    id_cliente = $("#id_cliente").val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoTicket/GetDataCliente",
        data: { id_cliente },
        success: function (response) {

            var arreglo = response;
            if (response.NIT == "ERROR") {
                $('#Error').html("EL CLIENTE NO SE ENCUENTRA EN LA BASE DE DATOS");
                $('#ModalError').modal('show');
                return;
            }
            $("#cliente").val(arreglo.CLIENTE);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

}



function GuardarTicket() {

    var id_cliente = document.getElementById("id_cliente").value;
    var reporta = document.getElementById("reporta").value.toUpperCase();
    var problema = document.getElementById("problema").value;
    var tipoproblema = document.getElementById("tipoproblema").value;
    var medio = document.getElementById("medio").value;
    var usuarior = document.getElementById("usuarior").value;
    var fecha = document.getElementById("fecha").value;
    var error = document.getElementById("error").value.toUpperCase();
    var descripcion = document.getElementById("descripcion").value.toUpperCase();
    var intercompany = document.getElementById("intercompany").value;

    if (id_cliente == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el ID del cliente.";
        $("#ModalGenerico").modal('show');
        return;
    }

    if (reporta == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar quién reporta.";
        $("#ModalGenerico").modal('show');
        return;
    }

    if (problema == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el problema.";
        $("#ModalGenerico").modal('show');
        return;
    }

    if (typeof tipoproblema === 'undefined' || tipoproblema == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el tipo de problema. (:";
        $("#ModalGenerico").modal('show');
        return;
    }

    if (intercompany == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar la intercompany.";
        $("#ModalGenerico").modal('show');
        return;
    }

    if (medio == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el medio.";
        $("#ModalGenerico").modal('show');
        return;
    }

    if (usuarior == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el usuario responsable.";
        $("#ModalGenerico").modal('show');
        return;
    }

    if (descripcion == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar una descripción.";
        $("#ModalGenerico").modal('show');
        return;
    }

    $.ajax({
        async: true,
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoTicket/InsertarTicket",
        data: { id_cliente, reporta, problema, tipoproblema, medio, usuarior, descripcion, fecha, error, intercompany },
        success: function (response) {
            var arreglo = response;

            if (response.CODIGO == "ERROR") {
                if (response.PRODUCTO == "") {
                    $('#Error').html("EL CODIGO DE PRODUCTO NO SE ENCUENTRA EN LA BASE DE DATOS");
                    $('#ModalError').modal('show');
                    return;
                }
                else {
                    $('#Error').html(response.PRODUCTO);
                    $('#ModalError').modal('show');
                    return;
                }
            }
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

    window.location.href = "/IngresoTicket/IngresoTicket";
    
}
