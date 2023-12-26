//$(document).ready(BuscarOrden);



window.onload = function () {

    var fecha = new Date(); //Fecha actual
    var mes = fecha.getMonth() + 1; //obteniendo mes
    var dia = fecha.getDate(); //obteniendo dia
    var ano = fecha.getFullYear(); //obteniendo año
    if (dia < 10)
        dia = '0' + dia; //agrega cero si el menor de 10
    if (mes < 10)
        mes = '0' + mes //agrega cero si el menor de 10

    document.getElementById('fechaingreso').value = ano + "-" + mes + "-" + dia;
    //document.getElementById('fechaentrega').value = ano + "-" + mes + "-" + dia;

    BuscarOrden();
    cargar_tipoplaca();
}






// FUNCION PARA CARGAR LOS TIPOS DE PLACAS
function cargar_tipoplaca() {

    let array = ["P", "C", "M", "A", "U", "CD", "MI", "DIS", "O", "CC", "E", "EXT", "TC", "TRC", "INV"];
    for (var i in array) {
        document.getElementById("tipoplaca").innerHTML += "<option value='" + array[i] + "'>" + array[i] + "</option>";
    }
}



//FUNCION PARA SELECCIONAR CLIENTE DEL MODAL
function seleccionar(id) {
    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");

    document.getElementById("idcliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("cliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[1].innerHTML;
    //document.getElementById("direccion").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[2].innerHTML;
    document.getElementById("nit").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[3].innerHTML;
    //document.getElementById("diascredito").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[4].innerHTML;
    document.getElementById("telefono").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[5].innerHTML;

    $("#ayudaclientes").modal('hide');
}



// FUNCION PARA SELECCIONAR SI ES CREDITO O CONTADO
function seleccionarCreditoContado(val) {
    if (val == 1) {
        document.getElementById("mostrarDiasCredito").style.display = "none";
    }
    if (val == 2) {
        document.getElementById("mostrarDiasCredito").style.display = "block";
        //document.getElementById("seleccionarFormaPago").style.display = "none";
    }
}



//FUNCION BUSCAR CORRELATIVO DE LA ORDEN
function BuscarOrden() {

    var id_orden = document.getElementById("id_orden").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoOrdenServicio/BuscarCorrelativo",
        data: {},
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

    return id_orden;
}


function BuscarCliente() {

    id_cliente = $("#id_cliente").val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoOrdenServicio/GetDataCliente",
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



function PDFOrdenServicio(id, serie) {

    //console.log(serie);
    //let id = $(e).closest("tr")[0].cells[1].innerText + "|" + $(e).closest("tr")[0].cells[2].innerText;
    window.open("/OrdenTecnicaMod/PDFOrdenServicio/" + id + "|" + serie, "_blank");

}




//FUNCION PARA GUARDAR LA ORDEN DE SERVICIO
function GuardarOrdenServicio() {

    //VARIABLES
    // DATOS DE LA ORDEN
    let id_orden = BuscarOrden();
    let serie = document.getElementById("serie").value;
    let fechaingreso = document.getElementById("fechaingreso").value;
    let asesores = document.getElementById("asesores").value;

    // DATOS DEL CLIENTE
    let idcliente = document.getElementById("idcliente").value;
    let cliente = document.getElementById("cliente").value;
    let telefono = document.getElementById("telefono").value;
    let correo = document.getElementById("correo").value;
    let nit = document.getElementById("nit").value;

    //DATOS DEL VEHICULO
    let tiposvehiculos = document.getElementById("tiposvehiculos").value;
    let marca = document.getElementById("marca").value;
    let lineavehiculo = document.getElementById("lineavehiculo").value;
    let modelo = document.getElementById("modelo").value;
    let tipoplaca = document.getElementById("tipoplaca").value;
    let placa = document.getElementById("placa").value;
    let color = document.getElementById("color").value;
    let kilometraje = document.getElementById("kilometraje").value;
    let medida = document.getElementById("medida").value;

    //CHECKLIST
    let radio = document.getElementById("radio").checked;
    let encendedor = document.getElementById("encendedor").checked;
    let documentos = document.getElementById("documentos").checked;
    let alfombras = document.getElementById("alfombras").checked;
    let llanta = document.getElementById("llanta").checked;
    let tricket = document.getElementById("tricket").checked;
    let llave = document.getElementById("llave").checked;
    let herramienta = document.getElementById("herramienta").checked;
    let platos = document.getElementById("platos").checked;

    //OBSERVACIONES
    let obs = document.getElementById("obs").value;

    //VERIFICACION DATOS DE ORDEN

    if (id_orden == '' || id_orden == null) {
        $("#mensaje").html("El número de orden no puede estar vacío.");
        $('#modal-generico').modal('show');
        return;
    }

    if (serie == '' || serie == null) {
        $("#mensaje").html("El número de serie no puede estar vacío.");
        $('#modal-generico').modal('show');
        return;
    }

    if (asesores == '' || asesores == null) {
        $("#mensaje").html("Debe ingresar un asesor.");
        $('#modal-generico').modal('show');
        return;
    }

    if (idcliente == '' || idcliente == null) {
        $("#mensaje").html("El ID de cliente no puede estar vacío.");
        $('#modal-generico').modal('show');
        return;
    }

    if (cliente == '' || cliente == null) {
        $("#mensaje").html("El cliente no puede estar vacío.");
        $('#modal-generico').modal('show');
        return;
    }

    if (telefono == '' || telefono == null) {
        $("#mensaje").html("Debe ingresar un número de teléfono.");
        $('#modal-generico').modal('show');
        return;
    }

    if (nit == '' || nit == null) {
        document.getElementById("nit").value = 'CF';
    }

    correo = (correo == '' || correo == null) ? '' : correo;


    //VERIFICACION DATOS VEHICULO

    if (tiposvehiculos == '' || tiposvehiculos == null) {
        $("#mensaje").html("Debe seleccionar el tipo de vehículo.");
        $('#modal-generico').modal('show');
        return;
    }

    if (marca == '' || marca == null) {
        $("#mensaje").html("Debe seleccionar una marca de vehículo.");
        $('#modal-generico').modal('show');
        return;
    }

    if (lineavehiculo == '' || lineavehiculo == null) {
        $("#mensaje").html("Debe ingresar la línea de vehículo.");
        $('#modal-generico').modal('show');
        return;
    }

    if (modelo == '' || modelo == null) {
        $("#mensaje").html("Debe ingresar un modelo de vehículo.");
        $('#modal-generico').modal('show');
        return;
    }

    if (placa == '' || placa == null) {
        $("#mensaje").html("Debe ingresar la placa del vehículo.");
        $('#modal-generico').modal('show');
        return;
    }

    if (color == '' || color == null) {
        $("#mensaje").html("Debe ingresar un color de vehículo.");
        $('#modal-generico').modal('show');
        return;
    }

    if (kilometraje == '' || kilometraje == null) {
        $("#mensaje").html("Debe ingresar el kilometraje del vehículo.");
        $('#modal-generico').modal('show');
        return;
    }

    obs = (obs == '' || obs == null) ? '' : obs;

    //CHECKLIST
    radio = (radio == true) ? '1' : '0';
    encendedor = (encendedor == true) ? '1' : '0';
    documentos = (documentos == true) ? '1' : '0';
    alfombras = (alfombras == true) ? '1' : '0';
    llanta = (llanta == true) ? '1' : '0';
    tricket = (tricket == true) ? '1' : '0';
    llave = (llave == true) ? '1' : '0';
    herramienta = (herramienta == true) ? '1' : '0';
    platos = (platos == true) ? '1' : '0';

    let arr = { id_orden, serie, fechaingreso, asesores, idcliente, cliente, telefono, correo, nit, tiposvehiculos, marca, lineavehiculo, modelo, tipoplaca, placa, color, kilometraje, medida, radio, encendedor, documentos, alfombras, llanta, tricket, llave, herramienta, platos, obs };
    let informacion = JSON.stringify(arr);

    $.post('/IngresoOrdenServicio/GuardarOrdenServicio', { informacion }).done(function (respuesta) {
        //alert("Registro Actualizado");
        PDFOrdenServicio(id_orden, serie);
        location.reload();
    });





}