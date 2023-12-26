
window.onload = function () {
    //permisoDescuento();
    var fecha = new Date(); //Fecha actual
    var mes = fecha.getMonth() + 1; //obteniendo mes
    var dia = fecha.getDate(); //obteniendo dia
    var ano = fecha.getFullYear(); //obteniendo año
    if (dia < 10)
        dia = '0' + dia; //agrega cero si el menor de 10
    if (mes < 10)
        mes = '0' + mes //agrega cero si el menor de 10
    document.getElementById('cFecha').value = ano + "-" + mes + "-" + dia;
    //$('#panel-exportaciones').hide();
    //$('#chkextr').hide();
    //$('#cTipoFrase').hide();
    //$('#div-orden').hide();
    //document.getElementById("simbtotal").textContent = "Q";

    //getEmpresa();
    //getCertificador();
}

function mostrarFiltro() {
    $('#modal-filtro').modal('show');
}

function BuscarDetalleOrden(no_factura, serie) {

    //BorrarDetalleTabla();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Operaciones/BuscaDetalleOrden",
        data: { no_factura, serie },
        success: function (response) {

            for (let i = 0; i < response.length; i++) {

                let table = document.getElementById("tabladetalle");
                let row = table.insertRow();
                //row.className = 'otrasFilas';
                let cellIDCod = row.insertCell(0);
                let cellCantidad = row.insertCell(1);
                let cellProducto = row.insertCell(2);
                //cellProducto.contentEditable = true;
                //let cellServicio = row.insertCell(3);
                let cellPrecio = row.insertCell(3);
                let cellDescto = row.insertCell(4);
                let cellSubtotal = row.insertCell(5);
                let cellImpuesto = row.insertCell(6);
                let cellEliminar = row.insertCell(7);


                //let cellTipo = row.insertCell(7);
                //cellTipo.id = response[i].id_codigo;

                //cellTipo.className = 'celdaTipo';
                //let fila = $("#tabladetalle tr").length;
                //file = fila - 1;

                //ConvertStringToFloat($("#tsubtotal").val());


                //let precio = Convert.toString(response[i].id_codigo);
                //let preciodouble = ConvertStringToFloat(precio);

                cellIDCod.innerHTML = response[i].id_codigo;
                cellCantidad.innerHTML = response[i].cantidad;
                cellProducto.innerHTML = response[i].obs;
                cellPrecio.textContent = formatNumber.new(response[i].precio);

                //cellPrecio.innerHTML = formatNumber.new(response[i].precio);
                //cellPrecio.innerHTML = formatNumber.new(response[i].precio);
                cellDescto.innerHTML = formatNumber.new(response[i].descto);
                cellSubtotal.innerHTML = formatNumber.new(response[i].subtotal);

                let impuestoProducto = '<select class="form-control" onchange="mostrarFrases(value);"> <option value="12">12</option><option value="0">0</option></select>';
                cellImpuesto.innerHTML = impuestoProducto;


                //cellPrecio.innerHTML = ConvertStringToFloat(response[i].precio);

                //cellDescto.innerHTML = ConvertStringToFloat(response[i].descto);
                //cellServicio.innerHTML = response[i].servicio;



                //let id_linea = response[i].id_linea;
                //cellTipo.innerHTML = CargarCatLineasi(id_linea);

                //cellTipo.innerHTML = '<select id = "catlineasi" name="catlineasi" class="form-control"></select>';
                let eliminarProducto = '<i class="fas fa-trash" style="color:red;" onclick="DeleteDetalle(this)"> </i>'; //AGREGA EL ICONO DE ELIMINAR
                cellEliminar.innerHTML = eliminarProducto;
            }

            //recorrerTabla();
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



function seleccionarFactura(id) {

    let Row = document.getElementById("fila")
    let Cells = Row.getElementsByTagName("td");

    let no_factura = document.getElementById("tablafactnc").rows[id.rowIndex].cells[0].innerHTML;
    let serie = document.getElementById("tablafactnc").rows[id.rowIndex].cells[1].innerHTML;
    let id_cliente = document.getElementById("tablafactnc").rows[id.rowIndex].cells[4].innerHTML;
    let cliente = document.getElementById("tablafactnc").rows[id.rowIndex].cells[5].innerHTML;
    let nit = document.getElementById("tablafactnc").rows[id.rowIndex].cells[6].innerHTML;
    let direccion = document.getElementById("tablafactnc").rows[id.rowIndex].cells[8].innerHTML;

    //$('#div-orden').show();
    document.getElementById("no_factura").value = no_factura;
    document.getElementById("serie").value = serie;
    document.getElementById("id_cliente").value = id_cliente;
    document.getElementById("cliente").value = cliente;
    document.getElementById("nit").value = nit;
    document.getElementById("direccion").value = direccion;


    BuscarDetalleFactura(no_factura, serie);

    $("#modal-filtro").modal('hide');
}



function filtroFirma() {

    //let filtro_firma = document.getElementById("filtro_firma").value;
    let fecha1 = document.getElementById("fecha1");
    let fecha2 = document.getElementById("fecha2");

    fecha1.value = "";
    fecha2.value = "";
}


function filtroFecha() {

    let firma = document.getElementById("firma");
    firma.value = "";
}




function getListaFacturas() {

    let firma = document.getElementById("firma").value;
    let fecha1 = document.getElementById("fecha1").value;
    let fecha2 = document.getElementById("fecha2").value;

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/NotaCredito/CargarFacturas",
        data: { firma, fecha1, fecha2 },
        success: function (response) {
            $("#tablita").html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}


function BorrarDetalleTabla() {

    var tables = document.getElementById("tabladetalle");

    var rowCount = tables.rows.length;
    for (var i = rowCount - 1; i > 0; i--) {
        tables.deleteRow(i);
    }

    recorrerTabla();
}


function BuscarDetalleFactura(no_factura, serie) {

    BorrarDetalleTabla();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/NotaCredito/BuscaDetalleFactura",
        data: { no_factura, serie },
        success: function (response) {

            for (let i = 0; i < response.length; i++) {

                //row.className = 'otrasFilas';
                //cellProducto.contentEditable = true;
                //let cellServicio = row.insertCell(3);

                let table = document.getElementById("tabladetalle");
                let row = table.insertRow();

                let cellIDCod = row.insertCell(0);
                let cellCantidad = row.insertCell(1);
                let cellProducto = row.insertCell(2);

                let cellPrecio = row.insertCell(3);
                let cellDescto = row.insertCell(4);
                let cellSubtotal = row.insertCell(5);
                let cellImpuesto = row.insertCell(6);
                let cellEliminar = row.insertCell(7);

                cellIDCod.innerHTML = response[i].id_codigo;
                cellCantidad.innerHTML = response[i].cantidad;
                cellProducto.innerHTML = response[i].obs;
                cellPrecio.textContent = response[i].precio;
                cellDescto.textContent = response[i].descto;
                cellSubtotal.textContent = response[i].subtotal;
                //cellPrecio.textContent = formatNumber.new(response[i].precio);

                //cellPrecio.innerHTML = formatNumber.new(response[i].precio);
                //cellPrecio.innerHTML = formatNumber.new(response[i].precio);
                //cellDescto.innerHTML = formatNumber.new(response[i].descto);
                //cellSubtotal.innerHTML = formatNumber.new(response[i].subtotal);

                let impuestoProducto = '<select class="form-control" onchange="mostrarFrases(value);" disabled> <option value="12">12</option><option value="0">0</option></select>';
                cellImpuesto.innerHTML = impuestoProducto;


                //cellPrecio.innerHTML = ConvertStringToFloat(response[i].precio);

                //cellDescto.innerHTML = ConvertStringToFloat(response[i].descto);
                //cellServicio.innerHTML = response[i].servicio;



                //let id_linea = response[i].id_linea;
                //cellTipo.innerHTML = CargarCatLineasi(id_linea);

                //cellTipo.innerHTML = '<select id = "catlineasi" name="catlineasi" class="form-control"></select>';
                let eliminarProducto = '<i class="fas fa-trash" style="color:red;" onclick="DeleteDetalle(this)"> </i>'; //AGREGA EL ICONO DE ELIMINAR
                cellEliminar.innerHTML = eliminarProducto;
            }

            recorrerTabla();
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



function recorrerTabla() {

    var total = 0;

    var items = [];

    $("#tabladetalle tr").each(function () {

        var itemProducto = {};

        var tds = $(this).find("td");

        itemProducto.subtotal = tds.filter(":eq(5)").text().replace(",", "");
        items.push(itemProducto);

        //Corrige el dato del total para que pueda realizar la suma
        if (itemProducto.subtotal == null || itemProducto.subtotal == "") {
            itemProducto.subtotal = 0.00;
            total = itemProducto.subtotal + (parseFloat(total) + parseFloat(itemProducto.subtotal)).toFixed(2);
        }

        parseFloat(itemProducto.subtotal);
        total = (parseFloat(total) + parseFloat(itemProducto.subtotal)).toFixed(2);
        $("#total").text(total);
        //$("#total").text(formatNumber.new(parseFloat(total).toFixed(2)));
    });
}



