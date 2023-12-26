
var subtotal = 0.00;



//CARGAR FECHA ACTUAL CUANDO CARGA LA PÁGINA
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

}


function seleccionar(id) {

    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");

    document.getElementById("id_codigo").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("codigoe").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[1].innerHTML;
    document.getElementById("costo").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[3].innerHTML;
    document.getElementById("producto").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[4].innerHTML;

    $("#ayudainventario").modal('hide');
    //LlenarTablaInventario();
}



function BuscarProducto() {

    id_codigo = $("#id_codigo").val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/EntradaKardex/GetDataProducto",
        data: { id_codigo },
        success: function (response) {

            var arreglo = response;
            if (response.CODIGO == "ERROR") {

                if (response.PRODUCTO == "") {
                    $('#Error').html("EL CODIGO NO SE ENCUENTRA EN LA BASE DE DATOS");
                    $('#ModalError').modal('show');
                    return;
                }
                else {
                    $('#Error').html(response.PRODUCTO);
                    $('#ModalError').modal('show');
                    return;
                }
            }
            //nPrecio = arreglo.CLIENTE;
            $("#producto").val(arreglo.PRODUCTO);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



/*
 * FUNCION PARA VERIFICAR LOS CAMPOS ANTES DE AGREGAR LOS ITEMS A LA TABLA
 */
function ModalAgregar() {

    var id_codigo = document.getElementById("id_codigo").value;
    var cantidad = document.getElementById("cantidad").value;
    var costo = document.getElementById("costo").value;

    if (id_codigo == '') {

        $("#mensaje").html("Debe ingresar el ID del producto.");
        $('#modal-generico').modal('show');
        return;
    }

    if (cantidad == '') {

        $("#mensaje").html("Debe ingresar una cantidad.");
        $('#modal-generico').modal('show');
        return;
    }

    if (costo == '') {

        $("#mensaje").html("Debe ingresar el costo por unidad.");
        $('#modal-generico').modal('show');
        return;
    }

    AddDetalle();

}



/*
 * FUNCION PARA AGREGAR LOS ITEMS DE LA LISTA A LA TABLA
 */
function AddDetalle() {

    var table = document.getElementById("tabladetalle");
    var row = table.insertRow();

    var cell0 = row.insertCell(0);
    var cell1 = row.insertCell(1);
    var cell2 = row.insertCell(2);
    var cell3 = row.insertCell(3);
    var cell4 = row.insertCell(4);
    var cell5 = row.insertCell(5);
    var cell6 = row.insertCell(6);

    var costo = document.getElementById("costo").value;
    costo = parseFloat(costo).toFixed(2);

    cell0.innerHTML = $("#id_codigo").val();
    cell1.innerHTML = $("#codigoe").val();
    cell2.innerHTML = $("#producto").val();
    cell3.innerHTML = $("#cantidad").val();
    cell4.innerHTML = costo;
    cell5.innerHTML = CalcularSubtotal();
    cell6.innerHTML = '<i class="fas fa-trash" style="color:red;" onclick="DeleteDetalle(this)"> </i>'; //AGREGA EL ICONO DE ELIMINAR

    CalcularSubtotal();
    recorrerTabla();

    document.getElementById("id_codigo").value = "";
    document.getElementById("cantidad").value = "1";
    document.getElementById("costo").value = "0";
    document.getElementById("codigoe").value = "";
    document.getElementById("producto").value = "";

    document.getElementById("id_codigo").focus();
    document.querySelector('#btn-ingreso').disabled = false;
}



/*
 * FUNCION PARA CALCULAR SUBTOTAL
 */
function CalcularSubtotal() {

    var cantidad = document.getElementById("cantidad").value;
    var costo = document.getElementById("costo").value;
    var labelSubtotal = document.getElementById("subtotal");

    subtotal = (parseFloat(cantidad) * parseFloat(costo));

    labelSubtotal.innerHTML = "<strong>Subtotal:</strong> Q" + subtotal.toFixed(2);

    return subtotal.toFixed(2);

}



/*
 * FUNCION QUE SIRVE PARA ELIMINAR LOS ITEMS LA TABLA
 */
function DeleteDetalle(element) {

    let seElimina = confirm("¿Seguro que desea quitar el articulo?");

    if (seElimina == true) {
        $(element).closest("tr").remove();
    }

    recorrerTabla();
}



/*
 * FUNCION RECORRE TABLA Y REALIZA SUMA DE TOTAL
 */
function recorrerTabla() {

    var labelTotal = document.getElementById("total");
    var total = 0.00;

    $("#tabladetalle tr").each(function () {

        var tds = $(this).find("td");

        var cantidad = tds.filter(":eq(3)").text();
        var costo = tds.filter(":eq(4)").text();

        //Corrige el dato del total para que pueda realizar la suma
        if (cantidad == null || cantidad == "") {
            cantidad = 0.00;
        }

        if (costo == null || costo == "") {
            costo = 0.00;
        }

        total = (parseFloat(total) + (parseFloat(cantidad) * parseFloat(costo)));
        labelTotal.innerHTML = "<strong>Total:</strong> Q" + total.toFixed(2);

    });
}



/*
 * FUNCION RECORRE TABLA E INSERTA KARDEX
 */
function recorrerIngresoKardex() {

    var items = [];

    ////Obtener text del dropdownlist
    //var sel = document.getElementById("codabono");
    //var tipodocto = sel.options[sel.selectedIndex].text;

    var movimiento = document.getElementById("movimiento").value;//MOVIMIENTO
    var fecha = document.getElementById("fecha").value; //COD ABONO
    var nodocumento = document.getElementById("nodocumento").value; //NO DOC
    var serie = document.getElementById("serie").value;//SERIE
    var concepto = document.getElementById("concepto").value; //CONCEPTO

    if (movimiento == '') {

        document.getElementById("mensaje").innerHTML = "Debe seleccionar un movimiento.";
        $("#modal-generico").modal('show');
        return;
    }

    if (fecha == '') {

        document.getElementById("mensaje").innerHTML = "Debe seleccionar una fecha.";
        $("#modal-generico").modal('show');
        return;
    }

    if (nodocumento == '') {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el número de documento.";
        $("#modal-generico").modal('show');
        return;
    }


    $("#tabladetalle tr").each(function () {
        var itemKardex = {};

        var tds = $(this).find("td");

        itemKardex.codigo = tds.filter(":eq(0)").text();
        itemKardex.codigoe = tds.filter(":eq(1)").text();
        itemKardex.producto = tds.filter(":eq(2)").text();
        itemKardex.cantidad = tds.filter(":eq(3)").text();
        itemKardex.costo = tds.filter(":eq(4)").text();
        items.push(itemKardex);
    });

    var kardex = {};
    kardex = items;
    var informacion = JSON.stringify(kardex);

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        data: { informacion, movimiento, fecha, nodocumento, serie, concepto },
        url: "/EntradaKardex/IngresarKardex",

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

    window.location.href = "/EntradaKardex/EntradaKardex";

}



/*
 * FUNCION PARA MOSTRAR LA SERIE DEL MOVIMIENTO
 */
function CargarSerie() {

    var movimiento = document.getElementById("movimiento").value;

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/EntradaKardex/CargarSerie",
        data: { movimiento },
        success: function (response) {

            $('#serie').val(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

    BuscarCorrelativo();

}



/*
 * FUNCION PARA ENCONTRAR EL CORRELATIVO SEGUN SERIE
 */
function BuscarCorrelativo() {

    var nodocumento = document.getElementById("nodocumento").value;
    var serie = document.getElementById("serie").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/EntradaKardex/BuscarCorrelativo",
        data: { serie },
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

            nodocumento = response.ID + 1;

            $("#nodocumento").val(nodocumento);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



function BuscarCodigo() {

    id = $("#id_codigo").val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/GetDataInventario",
        data: { id },
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

            //alert(arreglo.COSTO1);

            //nPrecio = arreglo.PRECIO;
            document.getElementById("producto").value = arreglo.PRODUCTO;
            document.getElementById("codigoe").value = arreglo.CODIGOE;
            document.getElementById("costo").value = arreglo.COSTO1;

            //$("#producto").text(arreglo.PRODUCTO);
            // $("#precio").text(arreglo.PRECIO);
            //document.getElementById("cantidad").focus();
            //nExistencia = arreglo.EXISTENCIA;
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}