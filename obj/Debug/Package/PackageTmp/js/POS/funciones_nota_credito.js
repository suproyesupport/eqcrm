
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