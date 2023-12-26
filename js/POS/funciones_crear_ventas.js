
window.onload = function () {
    permisoDescuento();
    var fecha = new Date(); //Fecha actual
    var mes = fecha.getMonth() + 1; //obteniendo mes
    var dia = fecha.getDate(); //obteniendo dia
    var ano = fecha.getFullYear(); //obteniendo año
    if (dia < 10)
        dia = '0' + dia; //agrega cero si el menor de 10
    if (mes < 10)
        mes = '0' + mes //agrega cero si el menor de 10
    document.getElementById('cFecha').value = ano + "-" + mes + "-" + dia;    
    $('#panel-exportaciones').hide();
    $('#chkextr').hide();
    $('#cTipoFrase').hide();
    $('#div-orden').hide();
    document.getElementById("simbtotal").textContent = "Q";

    getEmpresa();
    getCertificador();
}






// FUNCION PARA SELECCIONAR ORDEN DE SERVICIO DEL MODAL
function seleccionarOrden(id) {

    let Row = document.getElementById("fila")
    let Cells = Row.getElementsByTagName("td");

    //tablaorden
    let id_ordenss = document.getElementById("tablaorden").rows[id.rowIndex].cells[0].innerHTML;
    let seriess = document.getElementById("tablaorden").rows[id.rowIndex].cells[1].innerHTML;

    $('#div-orden').show();
    document.getElementById("id_orden").value = id_ordenss;
    document.getElementById("serie_orden").value = seriess;
    

    //BuscarDetalleOrden(id_ordenss, seriess);

    $("#ayudaordenes").modal('hide');
    $("#modalResumen").modal('show');

    document.querySelector('#ordenres').value = id_ordenss;
    document.querySelector('#serieres').value = seriess;

}


const OrdenResumen = () => {

    const id = document.querySelector('#ordenres').value;
    const serie = document.querySelector('#serieres').value;
    BuscarDetalleOrden(id, serie, "true");
}

const OrdenDetallada = () => {

    const id = document.querySelector('#ordenres').value;
    const serie = document.querySelector('#serieres').value;
    BuscarDetalleOrden(id, serie, "false");
}


//AQUI
function BuscarDetalleOrden(id_orden, serie, resumen) {

    BorrarDetalleTabla();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Operaciones/BuscaDetalleOrden",
        data: { id_orden, serie, resumen },
        success: function (response) {

            for (let i = 0; i < response.length; i++) {

                let table = document.getElementById("tabladetalle");
                let row = table.insertRow();
                let cellIDCod = row.insertCell(0);
                let cellCantidad = row.insertCell(1);
                let cellProducto = row.insertCell(2);
                cellProducto.contentEditable = true;
                let cellPrecio = row.insertCell(3);
                let cellDescto = row.insertCell(4);
                let cellSubtotal = row.insertCell(5);
                let cellImpuesto = row.insertCell(6);
                let cellEliminar = row.insertCell(7);

                cellIDCod.innerHTML = response[i].id_codigo;
                cellCantidad.innerHTML = response[i].cantidad;
                cellProducto.innerHTML = response[i].obs;
                cellPrecio.textContent = formatNumber.new(response[i].precio);
                cellDescto.innerHTML = formatNumber.new(response[i].descto);
                cellSubtotal.innerHTML = formatNumber.new(response[i].subtotal);

                let impuestoProducto = '<select class="form-control" onchange="mostrarFrases(value);"> <option value="12">12</option><option value="0">0</option></select>';
                cellImpuesto.innerHTML = impuestoProducto;

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


function BorrarDetalleTabla() {

    var tables = document.getElementById("tabladetalle");

    var rowCount = tables.rows.length;
    for (var i = rowCount - 1; i > 0; i--) {
        tables.deleteRow(i);
    }

    recorrerTabla();
}





















function mostrarFrases(nval) {

    if (nval == "0") {
        $('#cTipoFrase').show();
    }
}




var bss;
var total2;
var descuento_;
var no_parte;
var marca;

let empresa;
let certificador;
let nEstablecimiento;

let codigoEscenario = 0;

let chkextranjero = "N";
let cMoneda = "GTQ";
    
let facturaexportacion = "";

let nTasa;



function BuscarFacturas() {


    $.post('/Operaciones/ObtieneVentas', {}).done(function (respuesta) {        
        $('#facturas').html(respuesta);
        
        
        $('#buscarfacturas').modal('show');
        
    });



}



function seleccionarfactura(id) {

    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");

}

function SetearTipoCliente(nClie) {

    if (nClie == 1) {

        facturaexportacion = "2";
        chkextranjero = "N";
        $('#labelnit').html("NIT/CUI")
        $('#panel-exportaciones').hide();

        document.getElementById("id").value = "";
        document.getElementById("nit").value = "";
        document.getElementById("nombre").value = "";
        document.getElementById("direccion").value = "";
        document.getElementById("id").focus();
    }

    if (nClie == 2) {

        facturaexportacion = "2";
        chkextranjero = "N";
        $('#labelnit').html("NIT")
        $('#panel-exportaciones').hide();

        document.getElementById("id").value = "CF";
        document.getElementById("nit").value = "CF";
        document.getElementById("nombre").value = "CONSUMIDOR FINAL";
        document.getElementById("direccion").value = "CIUDAD";


        document.getElementById("nombre").focus();
    }

    if (nClie == 3) {

        facturaexportacion = "2";
        chkextranjero = "S";        
        $('#labelnit').html("EXTRANJERO")
        document.getElementById('labelnit').value = "EXTRANJERO";        
        $('#panel-exportaciones').hide();

        document.getElementById("id").value = "";
        document.getElementById("nit").value = "";
        document.getElementById("nombre").value = "";
        document.getElementById("direccion").value = "";
        document.getElementById("id").focus();
    }


    if (nClie == 4) {

        facturaexportacion = "1";
        chkextranjero = "N";
        $('#labelnit').html("EXPORTACION")
        $('#panel-exportaciones').show();        
        //$('#panel-exportaciones').removeAttr("hidden");
         $('#mostrarIncoterm').show();

        document.getElementById("id").value = "";
        document.getElementById("nit").value = "";
        document.getElementById("nombre").value = "";
        document.getElementById("direccion").value = "";
        document.getElementById("id").focus();
        
    }
}

function SetearTipoMoneda(cMon) {

    cMoneda = cMon;

    if (cMoneda == "USD") {
        gettasa();
        document.getElementById("ntasa").value = nTasa;

        //document.querySelector("simbolo").textContent = "oooo";
        document.getElementById("simbprecio").textContent = "$";
        document.getElementById("simbsubtotal").textContent = "$";
        document.getElementById("simbtotal").textContent = "$";

    } else {

        nTasa = "0";
        document.getElementById("ntasa").value = "";
        document.getElementById("simbprecio").textContent = "Q";
        document.getElementById("simbsubtotal").textContent = "Q";
        document.getElementById("simbtotal").textContent = "Q";

    }
    
    let boton = document.getElementById("idcmoneda");

    boton.innerText = "Moneda " + cMoneda;
}

function SetearEstablecimiento(nEst) {
    nEstablecimiento = nEst;

    let boton = document.getElementById("idbtnestablecimiento");

    boton.innerText ="Establecimiento "+ nEstablecimiento;
    
}

function SetearFrase(nEst) {

    let cDesc = nEst.split("|");

    codigoEscenario = cDesc[0];

    let desc = cDesc[1];

    let boton = document.getElementById("idbtnfrases");

    boton.innerText = "Frase " + desc.slice(0, 150)

    $("#id_codigo").focus();

}



function getCertificador() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/getCertificador",
        data: {},
        success: function (response) {
            certificador = response;
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

}


function gettasa() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/gettasa",
        data: {},
        success: function (response) {
            nTasa = response;
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

}







/*FUNCION PARA VERIFICAR SERIE */
function verificarSerie(val) {

    var exportacion = "";
    cTipoDocto = $("#cTipoDocto").val();

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/Cargar",
        data: { cTipoDocto },
        success: function (response) {

            exportacion = response;

            if (exportacion == "1") {

                document.getElementById("mostrarIncoterm").style.display = "block";

            } else if (exportacion == "2") {

                document.getElementById("mostrarIncoterm").style.display = "none";
                document.getElementById("mostrarIncoterm").value = "";
            }
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

}






//FUNCION PARA VALIDAR EXISTENCIA DE PRODUCTO
function validarCantExist() {

    var cantidad = document.getElementById("cantidad").value;
    var existencia = document.getElementById("existencia").value;

    if (validarBienServ(bss) == "N") {
        //alert("ES UN BIEN");
        if (parseFloat(cantidad) > parseFloat(existencia)) {
            $('#Error').html("LA CANTIDAD NO PUEDE SER MAYOR A LA EXISTENCIA");
            $("#cantidad").focus();
            $('#ModalError').modal('show');
            document.getElementById("cantidad").value = 0;
            document.getElementById("cantidad").focus();
            return;
        }
        else {
            document.getElementById("btnagregar").focus();
        }
    } else if (validarBienServ(bss) == "S") {
        //alert("ES UN SERVICIO");
        document.getElementById("btnagregar").focus();
    }

    if (validarBienServ(bss) == "BIEN") {
        //alert("ES UN BIEN");
        if (parseFloat(cantidad) > parseFloat(existencia)) {
            $('#Error').html("LA CANTIDAD NO PUEDE SER MAYOR A LA EXISTENCIA");
            $("#cantidad").focus();
            $('#ModalError').modal('show');
            document.getElementById("cantidad").value = 0;
            document.getElementById("cantidad").focus();
            return;
        }
        else {
            document.getElementById("btnagregar").focus();
        }
    } else if (validarBienServ(bss) == "SERVICIO") {
        //alert("ES UN SERVICIO");
        document.getElementById("btnagregar").focus();
    }
}



// FUNCION PARA VALIDAR SI LA DIRECCION ESTA VACIA
function validarDireccion() {

    var dire = document.getElementById("direccion").value;

    if (dire === "") {
        document.getElementById("direccion").value = "CIUDAD";
    }
}



// FUNCION VALIDAR BIEN O SERVICIO DESACTIVA INPUT
function validarBienServ(bs) {

    if (bs == "S") {
        document.getElementById("labelexistencia").style.display = 'none';
        document.getElementById("existencia").style.display = 'none';

    } else if (bs == "N"){
        document.getElementById("labelexistencia").style.display = 'block';
        document.getElementById("existencia").style.display = 'block';

    } else if (bs == "SERVICIO") {
        document.getElementById("labelexistencia").style.display = 'none';
        document.getElementById("existencia").style.display = 'none';

    } else if (bs == "BIEN") {
        document.getElementById("labelexistencia").style.display = 'block';
        document.getElementById("existencia").style.display = 'block';
    }

    return bs;

}



// FUNCION PARA BUSCAR EL CODIGO DE INVENTARIO EN EL INPUT
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

            nPrecio = arreglo.PRECIO;

            document.getElementById("id_codigo").value = arreglo.CODIGO;
            document.getElementById("codigoe").value = arreglo.CODIGOE;
            document.getElementById("producto").value = arreglo.PRODUCTO;
            document.getElementById("existencia").value = arreglo.EXISTENCIA;
            //document.getElementById("noparte").value = arreglo.NOPARTE;
            //document.getElementById("marca").value = arreglo.MARCA;
            no_parte = arreglo.NOPARTE;
            marca = arreglo.MARCA;
            $("#producto").text(arreglo.PRODUCTO);
            document.getElementById("precio").value = nPrecio;
            document.getElementById("cantidad").focus();
            

            var bienserv = arreglo.SERVICIO;

            
            bss = validarBienServ(bienserv);
            
            BuscarPrecioCliente();
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



// FUNCION PARA MOSTRAR MODAL DE INVENTARIO CON F2
function onKeyDownHandler(event) {

    var codigo = event.which || event.keyCode;

    //console.log("Presionada: " + codigo);

    //if (codigo === 13) {
    //    console.log("Tecla ENTER");
    //}

    //if (codigo >= 65 && codigo <= 90) {
    //    console.log(String.fromCharCode(codigo));
    //}

    if (codigo == 113) {
        $('#ayudaproductos').modal('show');
        
    }
}






// FUNCION PARA MOSTRAR MODAL DE CLIENTES CON F2
function onKeyDownHandlerCliente(event) {

    var codigo = event.which || event.keyCode;

    //console.log("Presionada: " + codigo);

    //if (codigo === 13) {
    //    console.log("Tecla ENTER");
    //}

    //if (codigo >= 65 && codigo <= 90) {
    //    console.log(String.fromCharCode(codigo));
    //}

    if (codigo == 113) {
        $('#ayudaclientes').modal('show');

    }
}



// FUNCION PARA SELECCIONAR SI ES CREDITO O CONTADO
function seleccionarCreditoContado(val) {

    if (val == 1) {
        document.getElementById("mostrarDiasCredito").style.display = "none";
    }
    if (val == 2) {
        document.getElementById("mostrarDiasCredito").style.display = "block";
        document.getElementById("seleccionarFormaPago").style.display = "none";
    }
}




function seleccionarinventario(id) {

    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");

    document.getElementById("id_codigo").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("codigoe").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[1].innerHTML;

    //$("#producto").text(document.getElementById("tablainventario").rows[id.rowIndex].cells[1].innerHTML);
    //$("#precio").text(parseFloat(document.getElementById("tablainventario").rows[id.rowIndex].cells[2].innerHTML).toFixed(2));
    //document.getElementById("producto").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[1].innerHTML;
    //document.getElementById("precio").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[2].innerHTML;
    document.getElementById("producto").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[4].innerHTML;
    document.getElementById("precio").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[5].innerHTML;
    document.getElementById("existencia").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[6].innerHTML;
    tipo = document.getElementById("tablainventario").rows[id.rowIndex].cells[7].innerHTML;
    no_parte = document.getElementById("tablainventario").rows[id.rowIndex].cells[2].innerHTML;
    marca = document.getElementById("tablainventario").rows[id.rowIndex].cells[3].innerHTML;
    document.getElementById("cantidad").focus();

    bss = validarBienServ(tipo);



    //validarCantExist();

    $("#ayudaproductos").modal('hide');


    document.getElementById("cantidad").focus();

}



// FUNCION PARA SELECCIONAR CLIENTE DEL MODAL
function seleccionar(id) {

    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");

     var cliente = document.getElementById("tabla").rows[id.rowIndex].cells[6].innerHTML;

    if (cliente == "") {
        document.getElementById("nombre").value = document.getElementById("tabla").rows[id.rowIndex].cells[1].innerHTML;
    } else {
        document.getElementById("nombre").value = document.getElementById("tabla").rows[id.rowIndex].cells[6].innerHTML;
    }

    document.getElementById("id").value = document.getElementById("tabla").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("direccion").value = document.getElementById("tabla").rows[id.rowIndex].cells[2].innerHTML;
    document.getElementById("nit").value = document.getElementById("tabla").rows[id.rowIndex].cells[3].innerHTML;
    document.getElementById("diascredito").value = document.getElementById("tabla").rows[id.rowIndex].cells[4].innerHTML;
    document.getElementById("correo").value = document.getElementById("tabla").rows[id.rowIndex].cells[5].innerHTML;

    validarDireccion();

    $("#ayudaclientes").modal('hide');
}



//FUNCION BUSCAR CLIENTE POR ID
function BuscarCliente() {

    id = $("#id").val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/GetDataCliente",
        data: { id },
        success: function (response) {

            var arreglo = response;
            if (response.NIT == "ERROR") {
                $('#Error').html("EL CLIENTE NO SE ENCUENTRA EN LA BASE DE DATOS");
                $('#ModalError').modal('show');
                return;
            }

            var cliente = arreglo.FACTURAR;

            if (cliente == "") {
                $("#nombre").val(arreglo.CLIENTE);
            } else {
                $("#nombre").val(arreglo.FACTURAR);

            }

            $("#direccion").val(arreglo.DIRECCION);
            $("#nit").val(arreglo.NIT);
            $("#diascredito").val(arreglo.DIASCRED);
            $("#correo").val(arreglo.CORREO);

            validarDireccion();
            document.getElementById("id_codigo").focus();

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



function BuscarPrecioCliente() {
    id = $("#id_codigo").val();
    nit = $("#id").val();
    nPrecio = document.getElementById("precio").value;
    // alert(nPrecio);
    if (nit == "") {
        return;
    }

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/GetDataPrecioClie",
        data: { id, nit },
        success: function (response) {

            var arreglo = response;
            if (response.PRECIO == "ERROR") {

                document.getElementById("precio").value = nPrecio;
            }
            else {
                document.getElementById("precio").value = arreglo.PRECIO;
            }

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



// -- RECORREMOS LA TABLA 
function recorrerTabla() {

    var total = 0;

    var items = [];

    $("#tabladetalle tr").each(function () {

        var itemProducto = {};

        var tds = $(this).find("td");

        itemProducto.subtotal = tds.filter(":eq(5)").text().replace(",","");
        items.push(itemProducto);

        //Corrige el dato del total para que pueda realizar la suma
        if (itemProducto.subtotal == null || itemProducto.subtotal == "") {
            itemProducto.subtotal = 0.00;
            total = itemProducto.subtotal + (parseFloat(total) + parseFloat(itemProducto.subtotal)).toFixed(2);
        }

            parseFloat(itemProducto.subtotal);
            total = (parseFloat(total) + parseFloat(itemProducto.subtotal)).toFixed(2);
            $("#total").text(formatNumber.new(parseFloat(total).toFixed(2)));
    });
}



function AnularVenta() {
   // window.location = "@Url.Action("", "POS")";
    location.reload();
}



function focusCantidad(){
    CalcularValoresGenerico();
    validarDesctoVacio();
    document.getElementById("precio").focus();
}



function focusPrecio() {

    var element = document.getElementById("descto");

    if (element.disabled) {
        CalcularValoresGenerico();
        validarDesctoVacio();
        document.getElementById("descto").focus();

    } else {
        CalcularValoresGenerico();
        validarDesctoVacio();
        document.getElementById("btnagregar").focus();
    }    
}



function focusDescuento() {
    CalcularValoresGenerico();
    validarDesctoVacio();
    document.getElementById("btnagregar").focus();
}



function CalcularValoresGenerico() {

    var nTotal;
    var nDescto;
    var nSubTotal = 0;

    var cantidad = $("#cantidad").val();
    var precio = $("#precio").val();
    var descto = $("#descto").val();

    //DESCUENTO
    var d = parseFloat(descto);
    d = (precio * (d / 100)) * cantidad;
    nDescto = d;
    descuento_ = nDescto

    //TOTAL SIN DESCUENTO
    nTotal = cantidad * precio;

    //SUBTOTAL = TOTAL - DESCUENTO
    nSubTotal = nTotal - nDescto;
    nSubTotal.toFixed(2);

    $("#tsubtotal").text(formatNumber.new(parseFloat(nSubTotal).toFixed(2)));
    document.getElementById("tsubtotal").value = nSubTotal;
    
    document.getElementById("btnagregar").focus();

   
}



function getEmpresa() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/getEmpresa",
        data: {},
        success: function (response) {
            empresa = response;
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

}

function getListaOrdenes() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/CargarOrdenes",
        data: {},
        success: function (response) {
            $("#tablita").html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


    //$.ajax({
    //    url: '@Url.Action("TuMetodo", "TuControlador")',
    //    type: 'GET',
    //    success: function (data) {
    //        // Actualiza el contenido de la tabla
            
    //    },
    //    error: function () {
    //        console.log("Error al obtener la tabla.");
    //    }
    //});


}














// FUNCION PARA AGREGAR PRODUCTOS A LA TABLA DETALLE
function AddDetalle() {

    var table = document.getElementById("tabladetalle");
    var existencia = document.getElementById("existencia").value;
    var subtotal = document.getElementById("tsubtotal").value;
    var nCantidad = $("#cantidad").val();
    var descuento = document.getElementById("descto").value;

    validarDesctoVacio();

    if (subtotal <= 0) {
        return;
    }

    if (descuento < 0) {
        $('#Error').html("EL DESCUENTO NO PUEDE SER MENOR IGUAL A 0");
        $('#ModalError').modal('show');
        return;
    }

    if (nCantidad <= 0) {
        $('#Error').html("LA CANTIDAD NO PUEDE SER MENOR IGUAL A 0");
        $('#ModalError').modal('show');
        return;
    }

    if (nCantidad = "") {
        $('#Error').html("LA CANTIDAD NO PUEDE ESTAR EN BLANCO");
        $('#ModalError').modal('show');
        return;
    }

    if (nCantidad > existencia)
    {
        $('#Error').html("LA CANTIDAD NO PUEDE SER MAYOR A LA EXISTENCIA");        
        $('#ModalError').modal('show');
        $("#cantidad").focus();
        return;
    }

    if (document.getElementById("id_codigo").value == "") {
        $('#Error').html("Codigo de producto vacio");
        $('#ModalError').modal('show');
        $("#id_codigo").focus();
        return;
    }

    var row = table.insertRow();
    var cell1 = row.insertCell(0);
    var cell2 = row.insertCell(1);
    var cell3 = row.insertCell(2);
    //var cell3 = "<pre>" + row.insertCell(2) + "</pre>" ;
    //cell3.className = 'expand footable-visible';
    //cell3.setAttribute('contentEditable', 'true');
    //cell3.innerHTML = "<div contenteditable></div>";
    var cell4 = row.insertCell(3);
    var cell5 = row.insertCell(4);
    var cell6 = row.insertCell(5);
    var cell7 = row.insertCell(6);
    var cell8 = row.insertCell(7);

    cell1.innerHTML = $("#id_codigo").val();
    cell2.innerHTML = $("#cantidad").val();

    if (empresa != "kenichi") {        
        cell3.innerHTML = "<pre>" + $("#producto").val() + "</pre>";
    } else {
        cell3.innerHTML = document.getElementById("codigoe").value + "|" + no_parte + "|" + document.getElementById("producto").value + "|" + marca;
    }

    cell4.innerHTML = formatNumber.new(document.getElementById("precio").value);
    cell5.innerHTML = formatNumber.new(descuento_);
    cell6.innerHTML = formatNumber.new($("#tsubtotal").text());
    
    let eliminarProducto = '<i class="fas fa-trash" style="color:red;" onclick="DeleteDetalle(this)"> </i>'; 
    let impuestoProducto = '<select class="form-control" onchange="mostrarFrases(value);"> <option value="12">12</option><option value="0">0</option></select>';
    cell7.innerHTML = impuestoProducto;
    //var eliminarProducto = '<a href="javascript:void(0);" class="btn btn-danger btn-icon waves-effect waves-themed"><i class="fal fa-times" onclick="DeleteDetalle(this)></i>';
    cell8.innerHTML = eliminarProducto;

    document.getElementById("id_codigo").value = "";
    document.getElementById("cantidad").value = "0";
    document.getElementById("producto").value = "";
    document.getElementById("precio").value = "";
    document.getElementById("descto").value = "0";
    document.getElementById("tsubtotal").value = "";
    document.getElementById("existencia").value = "";
    document.getElementById("codigoe").value = "";
    document.getElementById("id_codigo").focus();

    recorrerTabla();
}


function DeleteDetalle(element) {
    let seElimina = confirm("¿Seguro que desea eliminar el articulo?");
    if (seElimina == true) {
        $(element).closest("tr").remove();
        recorrerTabla(); 
    }
}



// FUNCION ORIGINAL
//function ObtieneInfoTabla() {


//    //var chkextranjero = $('input:checkbox[name=defaultUnchecked]:checked').val()


//    var extranjero = "";

    

//    if (chkextranjero == "S") {
//        extranjero = "S";
//    }
//    else {
//        extranjero = "N";
//    }

//    var tabla = $('#tabladetalle tr').length;   //cuenta la cantidad de filas en la tabla
//    var xtabla = '~DETALLES}';
//    var xTablaE = "~EQDOCUMENTO}~ENCABEZADO}"


//    var cXml = "";
//    xTablaE = xTablaE + '~TIPODOCTO}' + "FACTU" + '~|TIPODOCTO}';
//    xTablaE = xTablaE + '~SERIE}' + $("#cTipoDocto").val() + '~|SERIE}';
//    xTablaE = xTablaE + '~FECHADOCTO}' + $("#cFecha").val() + '~|FECHADOCTO}';
//    xTablaE = xTablaE + '~CODIGOCLIENTE}' + $("#id").val() + '~|CODIGOCLIENTE}';
//    xTablaE = xTablaE + '~NOMBRECLIENTE}' + $("#nombre").val() + '~|NOMBRECLIENTE}';
//    xTablaE = xTablaE + '~DIRECCIONCLIENTE}' + $("#direccion").val() + '~|DIRECCIONCLIENTE}';
//    xTablaE = xTablaE + '~NITCLIENTE}' + $("#nit").val() + '~|NITCLIENTE}';
//    xTablaE = xTablaE + '~CORREO}' + $("#correo").val() + '~|CORREO}';
//    xTablaE = xTablaE + '~VENDEDOR}' + "1" + '~|VENDEDOR}'; //CORREGIR Y CONSULTAR EL VENDEDOR SI ES COMO EL USUARIO LOGUEADO
//    xTablaE = xTablaE + '~ENCOBS}' + "OBSERVACIONES" + '~|ENCOBS}';
//    xTablaE = xTablaE + '~TIPOVENTA}' + $('#tipoVenta option:selected').val() + '~|TIPOVENTA}';
//    xTablaE = xTablaE + '~EXTRANJERO}' + extranjero + '~|EXTRANJERO}';
//    xTablaE = xTablaE + '~ESTABLECIMIENTO}' + nEstablecimiento + '~|ESTABLECIMIENTO}';
//    xTablaE = xTablaE + '~EXPORTACION}' + facturaexportacion + '~|EXPORTACION}';
//    xTablaE = xTablaE + '~MONEDA}' + cMoneda + '~|MONEDA}';

//    // segmento para factura de exportacion
//    xTablaE = xTablaE + '~NOMBREDESTINATARIO}' + $("#expdestinatario").val() + '~|NOMBREDESTINATARIO}';
//    xTablaE = xTablaE + '~DIRECCIONDESTINATARIO}' + $("#expdireccion").val() + '~|DIRECCIONDESTINATARIO}';
//    xTablaE = xTablaE + '~INCOTERM}' + $("#incoterm").val() + '~|INCOTERM}';
//    xTablaE = xTablaE + '~CODIGOCONSIGNATARIO}' + $("#expdconsignatario").val() + '~|CODIGOCONSIGNATARIO}';
//    xTablaE = xTablaE + '~NOMBRECOMPRADOR}' + $("#expnombrecomprador").val() + '~|NOMBRECOMPRADOR}';
//    xTablaE = xTablaE + '~DIRECCIONCOMPRADOR}' + $("#expdireccioncomprador").val() + '~|DIRECCIONCOMPRADOR}';
//    xTablaE = xTablaE + '~CODIGOCOMPRADOR}' + $("#expcodcomprador").val() + '~|CODIGOCOMPRADOR}';
//    xTablaE = xTablaE + '~NOMBREEXPORTADOR}' + $("#expnombreexportador").val() + '~|NOMBREEXPORTADOR}';
//    xTablaE = xTablaE + '~CODIGOEXPORTADOR}' + $("#expcodigoexportador").val() + '~|CODIGOEXPORTADOR}';
//    xTablaE = xTablaE + '~OTRAREFERENCIA}' + $("#expotrareferencia").val() + '~|OTRAREFERENCIA}';
//    // fin segmento factura exportacion expdconsignatario

//    xTablaE = xTablaE + '~DIAS}' + $("#diascredito").val() + '~|DIAS}~|ENCABEZADO}';

    
    

//    for (var n = 1; n < tabla; n++) {

//        var valor = parseInt(n) + 1;
//        var fila = $("#tabladetalle tr:nth-child(" + valor + ")").html();  //recupera desde la segunda fila de la tabla

//        // fila = fila.replace('class="expand footable-visible"', '');
//        fila = fila.replace('contenteditable="true"', '');
//        fila = fila.replace('<td >', '<td>');


//        var datos = fila.replace(/<td>/g, '~');
//        datos = datos.replace(/[/]/g, '');
//        datos = datos.replace(/<td>/g, '');
//        var campos = datos.split('~');


//        var nombre = campos[8];

//        var campoparaproducto = campos[3].replace(/<br>/g, '\n');

//        var base64Producto = btoa(unescape(encodeURIComponent(campoparaproducto)));

//        xtabla = xtabla + '~DETALLE}~CODIGO}' + campos[1] + '~|CODIGO}';
//        xtabla = xtabla + '~CODIGOE}' + campos[1] + '~|CODIGOE}';
//        xtabla = xtabla + '~CANTIDAD}' + campos[2] + '~|CANTIDAD}';
//        xtabla = xtabla + '~PRODUCTO}' + btoa(base64Producto) + '~|PRODUCTO}';

//        xtabla = xtabla + '~PRECIO}' + campos[4] + '~|PRECIO}';
//        xtabla = xtabla + '~DESCTO}' + campos[5] + '~|DESCTO}';
//        xtabla = xtabla + '~SUBTOTAL}' + campos[6] + '~|SUBTOTAL}~|DETALLE}';

//    }

//    xtabla = xtabla + '~|DETALLES}~|EQDOCUMENTO}'
//    cXml = xTablaE + xtabla;
//    return (cXml);

//}












//COCA
function ObtieneInfoTabla() {


    var extranjero = "";

    if (chkextranjero == "S") {
        extranjero = "S";
    }
    else {
        extranjero = "N";
    }

    var tabla = $('#tabladetalle tr').length;   //cuenta la cantidad de filas en la tabla
    var xtabla = '~DETALLES}';
    var xTablaE = "~EQDOCUMENTO}~ENCABEZADO}"


    var cXml = "";
    xTablaE = xTablaE + '~TIPODOCTO}' + "FACTU" + '~|TIPODOCTO}';
    xTablaE = xTablaE + '~SERIE}' + $("#cTipoDocto").val() + '~|SERIE}';
    xTablaE = xTablaE + '~FECHADOCTO}' + $("#cFecha").val() + '~|FECHADOCTO}';
    xTablaE = xTablaE + '~CODIGOCLIENTE}' + $("#id").val() + '~|CODIGOCLIENTE}';
    xTablaE = xTablaE + '~NOMBRECLIENTE}' + $("#nombre").val() + '~|NOMBRECLIENTE}';
    xTablaE = xTablaE + '~DIRECCIONCLIENTE}' + $("#direccion").val() + '~|DIRECCIONCLIENTE}';
    xTablaE = xTablaE + '~NITCLIENTE}' + $("#nit").val() + '~|NITCLIENTE}';
    xTablaE = xTablaE + '~CORREO}' + $("#correo").val() + '~|CORREO}';
    xTablaE = xTablaE + '~VENDEDOR}' + "1" + '~|VENDEDOR}'; //CORREGIR Y CONSULTAR EL VENDEDOR SI ES COMO EL USUARIO LOGUEADO
    xTablaE = xTablaE + '~ENCOBS}' + "OBSERVACIONES" + '~|ENCOBS}';
    xTablaE = xTablaE + '~TIPOVENTA}' + $('#tipoVenta option:selected').val() + '~|TIPOVENTA}';
    xTablaE = xTablaE + '~EXTRANJERO}' + extranjero + '~|EXTRANJERO}';
    xTablaE = xTablaE + '~ESTABLECIMIENTO}' + nEstablecimiento + '~|ESTABLECIMIENTO}';
    xTablaE = xTablaE + '~EXPORTACION}' + facturaexportacion + '~|EXPORTACION}';
    xTablaE = xTablaE + '~MONEDA}' + cMoneda + '~|MONEDA}';
    xTablaE = xTablaE + '~ESCENARIO}' + codigoEscenario + '~|ESCENARIO}';

    // segmento para factura de exportacion
    xTablaE = xTablaE + '~NOMBREDESTINATARIO}' + $("#expdestinatario").val() + '~|NOMBREDESTINATARIO}';
    xTablaE = xTablaE + '~DIRECCIONDESTINATARIO}' + $("#expdireccion").val() + '~|DIRECCIONDESTINATARIO}';
    xTablaE = xTablaE + '~INCOTERM}' + $("#incoterm").val() + '~|INCOTERM}';
    xTablaE = xTablaE + '~CODIGOCONSIGNATARIO}' + $("#expdconsignatario").val() + '~|CODIGOCONSIGNATARIO}';
    xTablaE = xTablaE + '~NOMBRECOMPRADOR}' + $("#expnombrecomprador").val() + '~|NOMBRECOMPRADOR}';
    xTablaE = xTablaE + '~DIRECCIONCOMPRADOR}' + $("#expdireccioncomprador").val() + '~|DIRECCIONCOMPRADOR}';
    xTablaE = xTablaE + '~CODIGOCOMPRADOR}' + $("#expcodcomprador").val() + '~|CODIGOCOMPRADOR}';
    xTablaE = xTablaE + '~NOMBREEXPORTADOR}' + $("#expnombreexportador").val() + '~|NOMBREEXPORTADOR}';
    xTablaE = xTablaE + '~CODIGOEXPORTADOR}' + $("#expcodigoexportador").val() + '~|CODIGOEXPORTADOR}';
    xTablaE = xTablaE + '~OTRAREFERENCIA}' + $("#expotrareferencia").val() + '~|OTRAREFERENCIA}';
    // fin segmento factura exportacion expdconsignatario

    xTablaE = xTablaE + '~DIAS}' + $("#diascredito").val() + '~|DIAS}~|ENCABEZADO}';

    let items = [];

    $("#tabladetalle tr").each(function () {

        let itemFact = {};
        let tds = $(this).find("td");
        if (tds.filter(":eq(0)").text() != "") {
            itemFact.codigo = tds.filter(":eq(0)").text();
            itemFact.cantidad = tds.filter(":eq(1)").text();
            itemFact.producto = btoa(unescape(encodeURIComponent(tds.filter(":eq(2)").text())));
            itemFact.precio = tds.filter(":eq(3)").text();
            itemFact.descto = tds.filter(":eq(4)").text();
            itemFact.subtotal = tds.filter(":eq(5)").text();
            itemFact.impuesto = tds.filter(":eq(6)").find("select").val();

            xtabla = xtabla + '~DETALLE}~CODIGO}' + itemFact.codigo + '~|CODIGO}';
            xtabla = xtabla + '~CODIGOE}' + itemFact.codigo + '~|CODIGOE}';
            xtabla = xtabla + '~CANTIDAD}' + itemFact.cantidad + '~|CANTIDAD}';
            xtabla = xtabla + '~PRODUCTO}' + itemFact.producto + '~|PRODUCTO}';

            xtabla = xtabla + '~PRECIO}' + itemFact.precio + '~|PRECIO}';
            xtabla = xtabla + '~DESCTO}' + itemFact.descto + '~|DESCTO}';
            xtabla = xtabla + '~IMPUESTO}' + itemFact.impuesto + '~|IMPUESTO}';
            xtabla = xtabla + '~SUBTOTAL}' + itemFact.subtotal + '~|SUBTOTAL}~|DETALLE}';


            //alert(btoa(unescape(encodeURIComponent(tds.filter(":eq(2)").text()))))

            //itemOrdenSrv.servicio = tds.filter(":eq(3)").text();

            //itemOrdenSrv.tipo = tds.filter(":eq(7)").find("select").val();
            //alert("ID CODIGO " + tds.filter(":eq(0)").text() + "LINEA " + tds.filter(":eq(7)").find("select").val());
            items.push(itemFact);
        }

    });


    xtabla = xtabla + '~|DETALLES}~|EQDOCUMENTO}'
    cXml = xTablaE + xtabla;
    return (cXml);

}




function ObtieneInfoTablaPrev() {


    var extranjero = "";

    if (chkextranjero == "S") {
        extranjero = "S";
    }
    else {
        extranjero = "N";
    }

    var tabla = $('#tabladetalle tr').length;   //cuenta la cantidad de filas en la tabla
    var xtabla = '~DETALLES}';
    var xTablaE = "~EQDOCUMENTO}~ENCABEZADO}"


    var cXml = "";
    xTablaE = xTablaE + '~TIPODOCTO}' + "FACTU" + '~|TIPODOCTO}';
    xTablaE = xTablaE + '~SERIE}' + $("#cTipoDocto").val() + '~|SERIE}';
    xTablaE = xTablaE + '~FECHADOCTO}' + $("#cFecha").val() + '~|FECHADOCTO}';
    xTablaE = xTablaE + '~CODIGOCLIENTE}' + $("#id").val() + '~|CODIGOCLIENTE}';
    xTablaE = xTablaE + '~NOMBRECLIENTE}' + $("#nombre").val() + '~|NOMBRECLIENTE}';
    xTablaE = xTablaE + '~DIRECCIONCLIENTE}' + $("#direccion").val() + '~|DIRECCIONCLIENTE}';
    xTablaE = xTablaE + '~NITCLIENTE}' + $("#nit").val() + '~|NITCLIENTE}';
    xTablaE = xTablaE + '~CORREO}' + $("#correo").val() + '~|CORREO}';
    xTablaE = xTablaE + '~VENDEDOR}' + "1" + '~|VENDEDOR}'; //CORREGIR Y CONSULTAR EL VENDEDOR SI ES COMO EL USUARIO LOGUEADO
    xTablaE = xTablaE + '~ENCOBS}' + "OBSERVACIONES" + '~|ENCOBS}';
    xTablaE = xTablaE + '~TIPOVENTA}' + $('#tipoVenta option:selected').val() + '~|TIPOVENTA}';
    xTablaE = xTablaE + '~EXTRANJERO}' + extranjero + '~|EXTRANJERO}';
    xTablaE = xTablaE + '~ESTABLECIMIENTO}' + nEstablecimiento + '~|ESTABLECIMIENTO}';
    xTablaE = xTablaE + '~EXPORTACION}' + facturaexportacion + '~|EXPORTACION}';
    xTablaE = xTablaE + '~MONEDA}' + cMoneda + '~|MONEDA}';
    xTablaE = xTablaE + '~ESCENARIO}' + codigoEscenario + '~|ESCENARIO}';

    // segmento para factura de exportacion
    xTablaE = xTablaE + '~NOMBREDESTINATARIO}' + $("#expdestinatario").val() + '~|NOMBREDESTINATARIO}';
    xTablaE = xTablaE + '~DIRECCIONDESTINATARIO}' + $("#expdireccion").val() + '~|DIRECCIONDESTINATARIO}';
    xTablaE = xTablaE + '~INCOTERM}' + $("#incoterm").val() + '~|INCOTERM}';
    xTablaE = xTablaE + '~CODIGOCONSIGNATARIO}' + $("#expdconsignatario").val() + '~|CODIGOCONSIGNATARIO}';
    xTablaE = xTablaE + '~NOMBRECOMPRADOR}' + $("#expnombrecomprador").val() + '~|NOMBRECOMPRADOR}';
    xTablaE = xTablaE + '~DIRECCIONCOMPRADOR}' + $("#expdireccioncomprador").val() + '~|DIRECCIONCOMPRADOR}';
    xTablaE = xTablaE + '~CODIGOCOMPRADOR}' + $("#expcodcomprador").val() + '~|CODIGOCOMPRADOR}';
    xTablaE = xTablaE + '~NOMBREEXPORTADOR}' + $("#expnombreexportador").val() + '~|NOMBREEXPORTADOR}';
    xTablaE = xTablaE + '~CODIGOEXPORTADOR}' + $("#expcodigoexportador").val() + '~|CODIGOEXPORTADOR}';
    xTablaE = xTablaE + '~OTRAREFERENCIA}' + $("#expotrareferencia").val() + '~|OTRAREFERENCIA}';
    // fin segmento factura exportacion expdconsignatario

    xTablaE = xTablaE + '~DIAS}' + $("#diascredito").val() + '~|DIAS}~|ENCABEZADO}';

    let items = [];

    $("#tabladetalle tr").each(function () {

        let itemFact = {};
        let tds = $(this).find("td");


        if (tds.filter(":eq(0)").text() != "") {

            itemFact.codigo = tds.filter(":eq(0)").text();
            itemFact.cantidad = tds.filter(":eq(1)").text();
            itemFact.producto = btoa(unescape(encodeURIComponent(tds.filter(":eq(2)").text())));
            itemFact.precio = tds.filter(":eq(3)").text();
            itemFact.descto = tds.filter(":eq(4)").text();
            itemFact.subtotal = tds.filter(":eq(5)").text();
            itemFact.impuesto = tds.filter(":eq(6)").find("select").val();

            xtabla = xtabla + '~DETALLE}~CODIGO}' + itemFact.codigo + '~|CODIGO}';
            xtabla = xtabla + '~CODIGOE}' + itemFact.codigo + '~|CODIGOE}';
            xtabla = xtabla + '~CANTIDAD}' + itemFact.cantidad + '~|CANTIDAD}';
            xtabla = xtabla + '~PRODUCTO}' + itemFact.producto + '~|PRODUCTO}';

            xtabla = xtabla + '~PRECIO}' + itemFact.precio + '~|PRECIO}';
            xtabla = xtabla + '~DESCTO}' + itemFact.descto + '~|DESCTO}';
            xtabla = xtabla + '~IMPUESTO}' + itemFact.impuesto + '~|IMPUESTO}';
            xtabla = xtabla + '~SUBTOTAL}' + itemFact.subtotal + '~|SUBTOTAL}~|DETALLE}';


            //alert(btoa(unescape(encodeURIComponent(tds.filter(":eq(2)").text()))))

            //itemOrdenSrv.servicio = tds.filter(":eq(3)").text();

            //itemOrdenSrv.tipo = tds.filter(":eq(7)").find("select").val();
            //alert("ID CODIGO " + tds.filter(":eq(0)").text() + "LINEA " + tds.filter(":eq(7)").find("select").val());
            items.push(itemFact);

        }
        

    });


    xtabla = xtabla + '~|DETALLES}~|EQDOCUMENTO}'
    cXml = xTablaE + xtabla;
    return (cXml);

}




////// esto esta mal
//function ObtieneInfoTabla() {

//    var tabla = $('#tabladetalle tr').length;   //cuenta la cantidad de filas en la tabla
//    var xtabla = '~DETALLES}';
//    var xTablaE = "~EQDOCUMENTO}~ENCABEZADO}"
//    var cXml = "";
//    xTablaE = xTablaE + '~TIPODOCTO}' + "FACTU" + '~|TIPODOCTO}';
//    xTablaE = xTablaE + '~SERIE}' + $("#cTipoDocto").val() + '~|SERIE}';
//    xTablaE = xTablaE + '~FECHADOCTO}' + $("#cFecha").val() + '~|FECHADOCTO}';
//    xTablaE = xTablaE + '~CODIGOCLIENTE}' + $("#id").val() + '~|CODIGOCLIENTE}';
//    xTablaE = xTablaE + '~NOMBRECLIENTE}' + $("#nombre").val() + '~|NOMBRECLIENTE}';
//    xTablaE = xTablaE + '~DIRECCIONCLIENTE}' + $("#direccion").val() + '~|DIRECCIONCLIENTE}';
//    xTablaE = xTablaE + '~NITCLIENTE}' + $("#nit").val() + '~|NITCLIENTE}';
//    xTablaE = xTablaE + '~CORREO}' + $("#correo").val() + '~|CORREO}';
//    xTablaE = xTablaE + '~VENDEDOR}' + "1" + '~|VENDEDOR}'; //CORREGIR Y CONSULTAR EL VENDEDOR SI ES COMO EL USUARIO LOGUEADO
//    xTablaE = xTablaE + '~ENCOBS}' + "OBSERVACIONES" + '~|ENCOBS}';
//    xTablaE = xTablaE + '~TIPOVENTA}' + $('#tipoVenta option:selected').val() + '~|TIPOVENTA}';
//    xTablaE = xTablaE + '~DIAS}' + $("#diascredito").val() + '~|DIAS}~|ENCABEZADO}';

//    for (var n = 1; n < tabla; n++) {

//        var valor = parseInt(n) + 1;
//        var fila = $("#tabladetalle tr:nth-child(" + valor + ")").html();  //recupera desde la segunda fila de la tabla


//        var datos = fila.replace(/<td>/g, '~');
//        datos = datos.replace(/[/]/g, '');
//        datos = datos.replace(/<td>/g, '');

//            var campos = datos.split('~');
//        var nombre = campos[8];


//        xtabla = xtabla + '~DETALLE}~CODIGO}' + campos[1] + '~|CODIGO}';
//       // xtabla = xtabla + '~CODIGOE}' + campos[1] + '~|CODIGOE}';
//        xtabla = xtabla + '~BIENOSERVICIO}' + campos[3] + '~|BIENOSERVICIO}';
//        xtabla = xtabla + '~CANTIDAD}' + campos[2] + '~|CANTIDAD}';
//        xtabla = xtabla + '~PRODUCTO}' + campos[4] + '~|PRODUCTO}';
//        xtabla = xtabla + '~PRECIO}' + campos[5] + '~|PRECIO}';
//        xtabla = xtabla + '~DESCTO}' + campos[6] + '~|DESCTO}';
//        xtabla = xtabla + '~SUBTOTAL}' + campos[7] + '~|SUBTOTAL}~|DETALLE}';


//    }
//    xtabla = xtabla + '~|DETALLES}~|EQDOCUMENTO}'
//    cXml = xTablaE + xtabla;

    
//    return (cXml);

//}


//function ObtieneInfoTablaPrev() {

//    var tabla = $('#tabladetalle tr').length;   //cuenta la cantidad de filas en la tabla
//    var xtabla = '~DETALLES}';
//    var xTablaE = "~EQDOCUMENTO}~ENCABEZADO}"
//    var cXml = "";
//    xTablaE = xTablaE + '~TIPODOCTO}' + "FACTU" + '~|TIPODOCTO}';
//    xTablaE = xTablaE + '~SERIE}' + $("#cTipoDocto").val() + '~|SERIE}';
//    xTablaE = xTablaE + '~FECHADOCTO}' + $("#cFecha").val() + '~|FECHADOCTO}';
//    xTablaE = xTablaE + '~CODIGOCLIENTE}' + $("#id").val() + '~|CODIGOCLIENTE}';
//    xTablaE = xTablaE + '~NOMBRECLIENTE}' + $("#nombre").val() + '~|NOMBRECLIENTE}';
//    xTablaE = xTablaE + '~DIRECCIONCLIENTE}' + $("#direccion").val() + '~|DIRECCIONCLIENTE}';
//    xTablaE = xTablaE + '~NITCLIENTE}' + $("#nit").val() + '~|NITCLIENTE}';
//    xTablaE = xTablaE + '~CORREO}' + $("#correo").val() + '~|CORREO}';
//    xTablaE = xTablaE + '~VENDEDOR}' + "1" + '~|VENDEDOR}'; //CORREGIR Y CONSULTAR EL VENDEDOR SI ES COMO EL USUARIO LOGUEADO
//    xTablaE = xTablaE + '~ENCOBS}' + "OBSERVACIONES" + '~|ENCOBS}';
//    xTablaE = xTablaE + '~TIPOVENTA}' + $('#tipoVenta option:selected').val() + '~|TIPOVENTA}';
//    xTablaE = xTablaE + '~DIAS}' + $("#diascredito").val() + '~|DIAS}~|ENCABEZADO}';

//    for (var n = 1; n < tabla; n++) {

//        var valor = parseInt(n) + 1;
//        var fila = $("#tabladetalle tr:nth-child(" + valor + ")").html();  //recupera desde la segunda fila de la tabla

//       // fila = fila.replace('class="expand footable-visible"', '');
//        fila = fila.replace('contenteditable="true"', '');
//        fila = fila.replace('<td >', '<td>');

      
//        var datos = fila.replace(/<td>/g, '~');
//        datos = datos.replace(/[/]/g, '');
//            datos = datos.replace(/<td>/g, '');
//            var campos = datos.split('~');
        
        
//        var nombre = campos[8];

//        var campoparaproducto = campos[3].replace(/<br>/g, '\n');
        

//        xtabla = xtabla + '~DETALLE}~CODIGO}' + campos[1] + '~|CODIGO}';
//        xtabla = xtabla + '~CODIGOE}' + campos[1] + '~|CODIGOE}';
//        xtabla = xtabla + '~CANTIDAD}' + campos[2] + '~|CANTIDAD}';
//        xtabla = xtabla + '~PRODUCTO}' + btoa(campoparaproducto) + '~|PRODUCTO}';

//        xtabla = xtabla + '~PRECIO}' + campos[4] + '~|PRECIO}';
//        xtabla = xtabla + '~DESCTO}' + campos[5] + '~|DESCTO}';
//        xtabla = xtabla + '~SUBTOTAL}' + campos[6] + '~|SUBTOTAL}~|DETALLE}';


//    }
//    xtabla = xtabla + '~|DETALLES}~|EQDOCUMENTO}'
//    cXml = xTablaE + xtabla;
//    return (cXml);

//}



function ValidarFraseExcenta() {

    let respuesta = "";

    $("#tabladetalle tr").each(function () {

        let tds = $(this).find("td");

        if (tds.filter(":eq(0)").text() != "") {

            let celdaImpuesto = tds.filter(":eq(6)").find("select").val();

            if (celdaImpuesto == "0") {

                let botonExcento = document.getElementById("idbtnfrases").innerText;
                
                if (botonExcento.toString() == "Frases Exentas") {
                    
                    respuesta = "false";
                }
            }

        }
    });

    return respuesta;
}





function SaveDataPost() {

    let tipodoct = document.getElementById("cTipoDocto").value;

    if (tipodoct == "" || tipodoct == "Seleccionar") {
        $('#Error').html("SELECCIONAR TIPO DE DOCUMENTO");
        $('#ModalError').modal('show');
        return;
    }

    if (certificador == "G4S") {
    if (nEstablecimiento === undefined) {
        $('#Error').html("DEBE SELECCIONAR EL ESTABLECIMIENTO DONDE DESEA EFECTUAR LA FACTURA");
        $('#ModalError').modal('show');
        return;
        }
    }

    //alert(ValidarFraseExcenta());

    if (ValidarFraseExcenta() == "false") {
        $('#Error').html("SELECCIONAR UNA FRASE EXCENTA DE IVA");
        $('#ModalError').modal('show');
        return;
    }


    cdetalle = ObtieneInfoTabla();

    var formaPago = document.getElementById("tipoVenta").value;

    if (formaPago == 1) {

        $('#formaPago').modal("show");

    } else {
        $.ajax({
            async: false,
            type: "POST",
            dataType: "text",
            contentType: "application/x-www-form-urlencoded",
            url: "/POS/Operaciones/InsertarDocumento",
            data: { cdetalle },
            success: function (response) {

                var mensaje = response.toString();
                let arr = mensaje.split('|');

                var msj = arr[1];

                if (msj == 'DOCUMENTO CREADO CON EXITO') {
                    
                    document.getElementById("id").value = "";
                    document.getElementById("nombre").value = "";
                    document.getElementById("direccion").value = "";
                    document.getElementById("nit").value = "";
                    document.getElementById("codigoe").value = "";
                    document.getElementById("correo").value = "";
                    document.getElementById("correo").value = "";
                    document.getElementById("diascredito").value = "";
                    document.getElementById("mostrarDiasCredito").style.display = "none";

                    var tableHeaderRowCount = 1;
                    var table = document.getElementById('tabladetalle');
                    var rowCount = table.rows.length;
                    for (var i = tableHeaderRowCount; i < rowCount; i++) {
                        table.deleteRow(tableHeaderRowCount);
                    }

                    $('#Error1').html(response);
                    $('#ModalFinalizarPOS').modal('show');
                    //window.location.reload();
                    
                    
                } else {
                    $('#Error1').html(response);
                    $('#formaPago').modal("hide");
                    $('#ModalFinalizarPOS').modal('show');
                }
                

            },
            error: function () {
                alert("Ocurrio un Error");
            }
        });
    }

    //enviamos un arreglo de la cantidad de filas que existan
    console.log(cdetalle);
}



function encode(base64) {
    var binaryString = btoa(base64);
    var bytes = new Uint8Array(binaryString.length);
    for (var i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes.buffer;
}


function PreDataPost() {

    cdetalle = ObtieneInfoTablaPrev();

   // var cDetalleb64 = btoa(cdetalle);

    //cdetalle = cDetalleb64;

    var formaPago = document.getElementById("tipoVenta").value;

    //alert(cdetalle);
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/PrevisualizarDocumento",
        data: { cdetalle }  ,
        success: function (response) {

            var mensaje = response.toString();
            window.open(mensaje);
            
                       

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

    //$.ajax({
    //    async: false,
    //    type: "POST",
    //    dataType: "text",
    //    contentType: "application/x-www-form-urlencoded",
    //    url: "/POS/Operaciones/PrevisualizarDocumento",
    //    data: { cdetalle },
    //    success: function (response) {

    //        var mensaje = response.toString();
    //        window.open(mensaje);



    //    },
    //    error: function () {
    //        alert("Ocurrio un Error");
    //    }
    //});

    

    //enviamos un arreglo de la cantidad de filas que existan
    console.log(cdetalle);
}


function finalizarDataPost() {

    cdetalle = ObtieneInfoTabla();
    formaPago = $("#tipoFormaPago").val();

    let id_orden = document.getElementById("id_orden").value;
    let serie_orden = document.getElementById("serie_orden").value;

    id_orden = (id_orden == '' || id_orden == null) ? '' : id_orden;
    serie_orden = (serie_orden == '' || serie_orden == null) ? '' : serie_orden;



    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/InsertarDocumento",
        data: { cdetalle, formaPago, id_orden, serie_orden },
        success: function (response) {

            var mensaje = response.toString();
            let arr = mensaje.split('|');

            var msj = arr[1];
            

            if (msj == 'DOCUMENTO CREADO CON EXITO') {


                document.getElementById("id").value = "";
                document.getElementById("nombre").value = "";
                document.getElementById("direccion").value = "";
                document.getElementById("nit").value = "";
                document.getElementById("correo").value = "";
                document.getElementById("diascredito").value = "";
                document.getElementById("mostrarDiasCredito").style.display = "none";
                document.getElementById("codigoe").value = "";
                document.getElementById("id_orden").value = "";
                document.getElementById("serie_orden").value = "";
                $('#div-orden').hide();

                var tableHeaderRowCount = 1;
                var table = document.getElementById('tabladetalle');
                var rowCount = table.rows.length;
                for (var i = tableHeaderRowCount; i < rowCount; i++) {
                    table.deleteRow(tableHeaderRowCount);
                }

                $('#Error1').html(response);
                $('#formaPago').modal("hide");
                $('#ModalFinalizarPOS').modal('show');

                location.reload();

            } else {
                $('#Error1').html(response);
                $('#formaPago').modal("hide");
                $('#ModalFinalizarPOS').modal('show');
            }
            
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

}



//FUNCION VERIFICAR CARACTERES OSCAR
var formatNumber = {
    separador: ",",
    sepDecimal: '.',
    formatear: function (num) {
        num += '';
        var splitStr = num.split('.');
        var splitLeft = splitStr[0];
        var splitRight = splitStr.length > 1 ? this.sepDecimal + splitStr[1] : '';
        var regx = /(\d+)(\d{3})/;
        while (regx.test(splitLeft)) {
            splitLeft = splitLeft.replace(regx, '$1' + this.separador + '$2');
        }
        return this.simbol + splitLeft + splitRight;
    },
    new: function (num, simbol) {
        this.simbol = simbol || '';
        return this.formatear(num);
        console.log("SI PASO POR LA FUNCION DE NUMEROS");
    }
}



function permisoDescuento() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Operaciones/permisoDescuento",
        data: {},
        success: function (response) {
            if (response == "True") {

                //$('#descto').prop('true');
                $("#descto").prop('disabled', false);

            } else {
                //$('#descto').prop('false');
                $("#descto").prop('disabled', true);

            }

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

}



function validarDesctoVacio() {

    var descuento = document.getElementById("descto");

    if (descuento.value == "") {
        descuento.value = "0";

    }

}





// FUNCION SIN UTILIZAR
function seleccionarCaja(val) {
    var Caja = document.getElementById("Nombre").value;
    alert(Caja);
    console.log(Caja);
}



// FUNCION SIN UTILIZAR
function redireccionar() {
    window.location.reload();
}



function CalcularValores() {
    var nTotal;

    var cantidad = $("#cantidad").val();
    var precio = document.getElementById("precio").value; //$("#precio").text();
    var subtotal = $("#tsubtotal").text();
    var descto = $("#descto").val();

    nTotal = cantidad * precio;
    subtotal = nTotal - descto;


    $("#tsubtotal").text(formatNumber.new(parseFloat(subtotal).toFixed(2)));
    //$("#tsubtotal").value(formatNumber.new(parseFloat(subtotal).toFixed(2)));

    //document.getElementById("tsubtotal").innerHTML = formatNumber.new(parseFloat(subtotal).toFixed(2));
    //document.getElementById("tsubtotal").innerHTML = subtotal;
    document.getElementById("tsubtotal").value = subtotal;


    //document.getElementById("subtotal").text = formatNumber.new(parseFloat(subtotal).toFixed(2));

    document.getElementById("precio").focus();


    //var n = 0;
    //var total2 = n + subtotal;
    //$("#total").text(formatNumber.new(parseFloat(total2).toFixed(2)));

    //var p = document.getElementById("factsinexist").value;
    //var nombre = <%=Sesión["Permiso"] %>;

    //jQuery(document).ready(function ($) {
    //    var value = '@Request.RequestContext.HttpContext.Session["Permiso"]';
    //    var jeje = "";
    //});

    var facsinexist = document.getElementById("chckfactsinexist").value;

    if (facsinexist == "false") {
        validarCantExist();
    }



    //if (total2 == null) {
    //    total2 = 0;
    //    total2 = total2 + subtotal;
    //} else {
    //    total2 = total2 + subtotal;
    //}
    //$("#total").text(formatNumber.new(parseFloat(total2).toFixed(2)));

}