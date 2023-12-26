
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
    document.getElementById('fechaentrega').value = ano + "-" + mes + "-" + dia;
    //document.getElementById("serie").value = 'WB';
    cargar_tipoplaca();
    let total = 0.00;

}

//let codhtml = '<select id="tipoFormaPago" class="browser-default custom-select">' +
//    '<option selected value="1">Efectivo</option>' +
//    '<option value="2">Tarjeta</option>' +
//    '</select>';


function CargarCatLineasi(id_linea) {

    let respuesta = "";

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/OrdenTecnicaMod/CargarCatLineasI",
        data: { id_linea },
        success: function (response) {
            respuesta = response.toString();
            //$('.celdaTipo').html(response);
            //lert(respuesta);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

    return respuesta;
}



function BorrarDetalleTabla() {

    var tables = document.getElementById("tabladetalle");

    var rowCount = tables.rows.length;
    for (var i = rowCount - 1; i > 0; i--) {
        tables.deleteRow(i);
    }

    recorrerTabla();
}


function modalModificarPrecio(element) {
    $('#modal-cambiaprecio').modal('show');
    console.warn(element);
}


//function modificarPrecio() {
//    recorrerTabla();
//}



// RECORREMOS LA TABLA 
function recorrerTablaModificada() {

    total = 0.00;
    let items = [];

    $("#tabladetalle tr").each(function () {

        let itemProducto = {};
        let tds = $(this).find("td");

        itemProducto.cantidad = tds.filter(":eq(1)").text().replace(",", "");
        itemProducto.precio = tds.filter(":eq(4)").text().replace(",", "");

        let cantidad = parseFloat(itemProducto.cantidad);
        let precio = parseFloat(itemProducto.precio);

        itemProducto.subtotal = (cantidad * precio).toFixed(2);
        tds.filter(":eq(6)").text(itemProducto.subtotal);

        items.push(itemProducto);
    });

    recorrerTabla();
}


function BuscarDetalleOrden() {

    let id_orden = document.getElementById("id_orden").value;
    let serie = document.getElementById("serie").value;

    BorrarDetalleTabla();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/OrdenTecnicaMod/BuscaDetalleOrden",
        data: { id_orden, serie },
        success: function (response) {

            for (let i = 0; i < response.length; i++) {

                let table = document.getElementById("tabladetalle");

                let row = table.insertRow();
                row.className = 'otrasFilas';
                let cellIDCod = row.insertCell(0);
                let cellCantidad = row.insertCell(1);
                cellCantidad.contentEditable = true;
                cellCantidad.addEventListener("input", function () {
                    recorrerTablaModificada();
                });
                let cellProducto = row.insertCell(2);
                cellProducto.contentEditable = true;
                let cellServicio = row.insertCell(3);
                let cellPrecio = row.insertCell(4);
                cellPrecio.contentEditable = true;
                cellPrecio.addEventListener("input", function () {
                    recorrerTablaModificada();
                });
                let cellDescto = row.insertCell(5);
                cellDescto.contentEditable = false;
                let cellSubtotal = row.insertCell(6);
                let cellTipo = row.insertCell(7);
                cellTipo.id = response[i].id_codigo;

                cellTipo.className = 'celdaTipo';
                let cellEliminar = row.insertCell(8);
                let fila = $("#tabladetalle tr").length;
                file = fila - 1;

                ConvertStringToFloat($("#tsubtotal").val());

                cellIDCod.innerHTML = response[i].id_codigo;
                cellCantidad.innerHTML = response[i].cantidad;
                cellProducto.innerHTML = response[i].obs;
                cellServicio.innerHTML = response[i].servicio;
                cellPrecio.innerHTML = ConvertStringToFloat(response[i].precio);
                cellDescto.innerHTML = ConvertStringToFloat(response[i].descto);
                cellSubtotal.innerHTML = ConvertStringToFloat(response[i].subtotal);
                let id_linea = response[i].id_linea;
                cellTipo.innerHTML = CargarCatLineasi(id_linea);
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





function abrirBusqueda() {

    document.getElementById("panel_busqueda").className = "panel-container";

}




function eliminarAllDetalle() {

    $(".otrasFilas").remove();
    total = 0.00;

}




// FUNCION PARA MOSTRAR MODAL DE INVENTARIO CON F2
function onKeyDownHandler(event) {

    let codigo = event.which || event.keyCode;
    let bid_orden = document.getElementById("bid_orden").value;

    if (codigo == 13) {
        BuscarOrden(bid_orden);
    }
}


function limparaCliente() {

    document.getElementById("bid_orden").value = "";
    document.getElementById("fecha1").value = "";
    document.getElementById("fecha2").value = "";

}

function limparaOrden() {

    document.getElementById("fecha1").value = "";
    document.getElementById("fecha2").value = "";
    document.getElementById("b_placa").value = "";

}

function limparaFecha() {

    document.getElementById("bid_orden").value = "";
    document.getElementById("b_placa").value = "";

}


function limparaPlaca() {

    document.getElementById("bid_orden").value = "";
    document.getElementById("fecha1").value = "";
    document.getElementById("fecha2").value = "";

}

function ModalFotos(ordenfoto, serie) {

    document.getElementById("ordenfoto").value = ordenfoto;
    document.getElementById("seriefoto").value = serie;

    $('#modal-subirfoto').modal('show');
}


//function ModalAnular(id, serie) {

//    //$('#modal-anular').modal('show');
//    $('#ModalError').modal('show');
//}








/* FUNCION BUSCAR ORDEN TECNICA POR FILTROS, GENERA LISTADO */

function BuscarFiltro() {

    let bid_orden = document.getElementById("bid_orden").value;
    let b_placa = document.getElementById("b_placa").value;
    let fecha1 = document.getElementById("fecha1").value;
    let fecha2 = document.getElementById("fecha2").value;

    $('#mostrar_consulta').html('');

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",   
        url: "/OrdenTecnicaMod/GetListaOrden",
        data: { bid_orden, b_placa, fecha1, fecha2 },
        beforeSend: InicioConsulta,
        success: ResultadoConsulta
    });

    function InicioConsulta() {
        //$('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ResultadoConsulta(data) {

        var arreglo = eval(data);
        cData = arreglo;

        var gridDataSource = new kendo.data.DataSource({
            data: cData,
            schema: {
                model: {
                    fields: {
                        IDORDEN: { type: "number" },
                        SERIE: { type: "string" },
                        FECHA: { type: "string" },
                        STATUS: { type: "string" },
                        IDLCIENTE: { type: "number" },
                        CLIENTE: { type: "string" },
                        NIT: { type: "string" },
                        PLACA: { type: "string" },
                        MARCAVEHICULO: { type: "string"},
                        LINEA: { type: "string" },
                        TOTAL: { type: "number" },
                    }
                }
            },
            pageSize: 20,
            //sort: {
            //    field: "IDORDEN",
            //    dir: "desc"
            //}
        });

        $("#mostrar_consulta").kendoGrid({

            toolbar: ["excel", "pdf", "search"], //Podemos colocar opciones en la toolbar del datagrid

            excel: {
                fileName: "Orden de Servicio.xlsx",
                filterable: true
            },
            pdf: {
                allPages: true,
                avoidLinks: true,
                paperSize: "letter",
                margin: { top: "2cm", left: "1cm", right: "1cm", bottom: "1cm" },
                landscape: true,
                repeatHeaders: true,
                template: $("#page-template").html(),
                scale: 0.8
            },
            search: {
                fields: [
                    { name: "IDORDEN", operator: "eq" },
                    //{ name: "Freight", operator: "gte" },
                    //{ name: "ShipName", operator: "contains" },
                    //{ name: "ShipCity", operator: "contains" },
                ]
            },

            dataSource: gridDataSource,
            pageable: true,
            height: 1000,
            columns: [
                {
                    field: "OPCIONES",
                    title: "Opciones",
                    width: 200,
                    template:
                        "<button class='btn btn-xs btn-dark waves-effect waves-themed' onclick='BuscarOrden(#=(IDORDEN)# , \"#=(SERIE)#\")';'>Mod.</button>" +
                        "<button class='btn btn-xs btn-danger waves-effect waves-themed' onclick='PDFOrdenServicio(#=(IDORDEN)# , \"#=(SERIE)#\")';'>PDF</button>" +
                        "<button class='btn btn-xs btn-secondary waves-effect waves-themed' onclick='ModalFotos(#=(IDORDEN)# , \"#=(SERIE)#\")';'>📷</button>",
                    stickable: true,
                    locked: true,
                    lockable: false,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },
                }, {
                    field: "IDORDEN",
                    title: "ID Orden",
                    width: 90,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },
                }, {
                    field: "SERIE",
                    title: "Serie",
                    width: 90,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },

                }, {
                    field: "FECHA",
                    title: "Fecha",
                    width: 130,
                    /*format: "0:{MM/dd/yyyy}",*/
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },

                }, {
                    field: "STATUS",
                    title: "Status",
                    width: 90,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },
                }, {
                    field: "IDCLIENTE",
                    title: "Id Cliente",
                    width: 130,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },
                }, {
                    field: "CLIENTE",
                    title: "Cliente",
                    width: 300,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                }, {
                    field: "NIT",
                    title: "Nit",
                    width: 130,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },
                }, {
                    field: "PLACA",
                    title: "Placa",
                    width: 130,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },
                }, {
                    field: "LINEA",
                    title: "Linea",
                    width: 130,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },
                }, {
                    field: "TOTAL",
                    title: "Total",
                    width: 130,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: right" },
                    format: "{0:n2}"
                    //format: "{0:c}",
                }],
            editable: "inline"
        });

        $('#mostrar_consulta2').html('');

        

        document.getElementById("panel_tabla").className = "panel-container";
        document.getElementById("panel_datos").className = "panel-container collapse";
        eliminarAllDetalle();
    }
}





// FUNCION PARA CARGAR EL TIPO DE PLACA
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

    //document.getElementById("bidcliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("bcliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[1].innerHTML;

    document.getElementById("idcliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("cliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[1].innerHTML;
    document.getElementById("direccion").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[2].innerHTML;
    document.getElementById("nit").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[3].innerHTML;
    document.getElementById("diascredito").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[4].innerHTML;
    document.getElementById("telefono").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[5].innerHTML;

    $("#ayudaclientes").modal('hide');
}

////FUNCION PARA SELECCIONAR CLIENTE DEL MODAL FILTRO
//function bseleccionar(id) {

//    var Row = document.getElementById("fila")
//    var Cells = Row.getElementsByTagName("td");

//    document.getElementById("bidcliente").value = document.getElementById("btablapdf").rows[id.rowIndex].cells[0].innerHTML;
//    document.getElementById("bcliente").value = document.getElementById("btablapdf").rows[id.rowIndex].cells[1].innerHTML;

//    $("#bayudaclientes").modal('hide');
//}






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




/*FUNCION PARA BUSCAR LINEA DE VEHICULO SEGUN VEHICULO*/
function cargar_lineav(val) {

    marca = $("#marca").val();

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/OrdenTecnicaMod/Cargar",
        data: { marca },
        success: function (response) {

            $('#lineavehiculo').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
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


//FUNCION BUSCAR ORDEN x INPUT ID (CARGAR CAMPOS DE LA ORDEN)
function BuscarOrden(bid_orden, serie) {

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/OrdenTecnicaMod/GetDataOrden",
        data: { bid_orden, serie },
        success: function (response) {

            var arreglo = response;

            if (response.NIT == "ERROR") {
                $('#Error').html("NO SE ENCUENTRA EN LA BASE DE DATOS");
                $('#ModalError').modal('show');
                return;
            }

            $("#id_orden").val(arreglo.IDORDEN);
            $("#serie").val(arreglo.SERIE);
            $("#fechaingreso").val(arreglo.FECHAI);
            $("#fechaentrega").val(arreglo.FECHAE);
            $("#asesores").val(arreglo.ASESORES);
            $("#idcliente").val(arreglo.IDCLIENTE);
            $("#cliente").val(arreglo.CLIENTE);
            $("#direccion").val(arreglo.DIRECCION);
            $("#nit").val(arreglo.NIT);
            $("#tipoVenta").val(arreglo.TIPOVENTA);
            //$("#contacto").val(arreglo.CONTACTO);
            $("#telefono").val(arreglo.TELEFONO);
            $("#correo").val(arreglo.CORREO);
            $("#obs").val(arreglo.OBS);

            //ASEGURADORA
            $("#aseguradoras").val(arreglo.ASEGURADORAS);
            $("#poliza").val(arreglo.POLIZA);
            $("#asesoremergencia").val(arreglo.ASESOREMERGENCIA);
            $("#reclamo").val(arreglo.RECLAMO);
            $("#corredora").val(arreglo.CORREDORA);
            $("#deducible").val(arreglo.DEDUCIBLE);
            $("#ajustador").val(arreglo.AJUSTADOR);
            $("#opciones").val(arreglo.OPCIONES);

            //VEHICULO
            let tipo = arreglo.TIPOVEHICULO;
            let marca = arreglo.MARCAVEHICULO;
            let linea = arreglo.LINEA

            $("#tiposvehiculos").val(tipo.trim());
            $("#marca").val(marca.trim());
            $("#lineavehiculo").val(linea.trim());
            $("#modelo").val(arreglo.MODELO);
            $("#tipoplaca").val(arreglo.TIPOPLACA);
            $("#placa").val(arreglo.PLACA);
            $("#color").val(arreglo.COLOR);
            $("#chassis").val(arreglo.CHASSIS);
            $("#kilometraje").val(arreglo.KILOMETRAJE);
            $("#medida").val(arreglo.MEDIDA);

            //CHECKLIST
            let radio = arreglo.RADIO;
            let encendedor = arreglo.ENCENDEDOR;
            let documentos = arreglo.DOCUMENTOS;
            let alfombras = arreglo.ALFOMBRAS;
            let llanta = arreglo.LLANTA;
            let tricket = arreglo.TRICKET;
            let llave = arreglo.LLAVE;
            let herramienta = arreglo.HERRAMIENTA;
            let platos = arreglo.PLATOS;

            radio = (radio == '1') ? true : false;
            encendedor = (encendedor == '1') ? true : false;
            documentos = (documentos == '1') ? true : false;
            alfombras = (alfombras == '1') ? true : false;
            llanta = (llanta == '1') ? true : false;
            tricket = (tricket == '1') ? true : false;
            llave = (llave == '1') ? true : false;
            herramienta = (herramienta == '1') ? true : false;
            platos = (platos == '1') ? true : false;

            $("#radio").prop('checked', radio);
            $("#encendedor").prop('checked', encendedor);
            $("#documentos").prop('checked', documentos);
            $("#alfombras").prop('checked', alfombras);
            $("#llanta").prop('checked', llanta);
            $("#tricket").prop('checked', tricket);
            $("#llave").prop('checked', llave);
            $("#herramienta").prop('checked', herramienta);
            $("#platos").prop('checked', platos);


            BuscarDetalleOrden();

            document.getElementById("panel_busqueda").className = "panel-container collapse";
            document.getElementById("panel_tabla").className = "panel-container collapse";
            document.getElementById("panel_datos").className = "panel-container";

        },

        error: function () {
            alert("Ocurrio un Error");
        }
    });
}






function PDFOrdenServicio(id, serie) {

    console.log(serie);
    //let id = $(e).closest("tr")[0].cells[1].innerText + "|" + $(e).closest("tr")[0].cells[2].innerText;
    window.open("/OrdenTecnicaMod/PDFOrdenServicio/" + id + "|" + serie, "_blank");

}




//FUNCION BUSCAR CLIENTE POR ID (INPUT)
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



//FUNCION PARA SELECCIONAR INVENTARIO DE MODAL
function seleccionarinventario(id) {

    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");
    let servicio = "";

    document.getElementById("id_codigo").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("codigoe").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[1].innerHTML;
    document.getElementById("producto").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[2].innerHTML;
    document.getElementById("precio").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[3].innerHTML;
    document.getElementById("existencia").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[4].innerHTML;

    if (document.getElementById("tablainventario").rows[id.rowIndex].cells[4].innerHTML == "BIEN") {
        servicio = "N";
    } else {
        servicio = "S";
    }

    document.getElementById("servicio").value = servicio;

    document.getElementById("cantidad").focus();

    //bss = validarBienServ(tipo);
    //validarCantExist();

    $("#ayudaproductos").modal('hide');

    //$("#cantidad").focus();
    //document.getElementById("cantidad").focus();

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


            //var bienserv = arreglo.SERVICIO;


            //bss = validarBienServ(bienserv);

            //BuscarPrecioCliente();
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}


//FUNCION PARA GUARDAR LA ORDEN DE SERVICIO
function GuardarOrdenServicio() {

    //ORDEN
    let id_orden = document.getElementById("id_orden").value;
    let serie = document.getElementById("serie").value;
    let fechaingreso = document.getElementById("fechaingreso").value;
    let fechaentrega = document.getElementById("fechaentrega").value;
    let asesores = document.getElementById("asesores").value;
    let tipoVenta = document.getElementById("tipoVenta").value;
    let diascredito = document.getElementById("diascredito").value;
    let obs = document.getElementById("obs").value;

    //CLIENTE
    let idcliente = document.getElementById("idcliente").value;
    let cliente = document.getElementById("cliente").value;
    let direccion = document.getElementById("direccion").value;
    let nit = document.getElementById("nit").value;
    let telefono = document.getElementById("telefono").value;
    let correo = document.getElementById("correo").value;

    //ASEGURADORAS
    let aseguradoras = document.getElementById("aseguradoras").value;
    let poliza = document.getElementById("poliza").value;
    let asesoremergencia = document.getElementById("asesoremergencia").value;
    let reclamo = document.getElementById("reclamo").value;
    let corredora = document.getElementById("corredora").value;
    let deducible = document.getElementById("deducible").value;
    let ajustador = document.getElementById("ajustador").value;
    let opciones = document.getElementById("opciones").value;

    //VEHICULO
    let tiposvehiculos = document.getElementById("tiposvehiculos").value;
    let marca = document.getElementById("marca").value;
    let lineavehiculo = document.getElementById("lineavehiculo").value;
    let modelo = document.getElementById("modelo").value;
    let tipoplaca = document.getElementById("tipoplaca").value;
    let placa = document.getElementById("placa").value;
    let color = document.getElementById("color").value;
    let kilometraje = document.getElementById("kilometraje").value;
    let medida = document.getElementById("medida").value;
    let chassis = document.getElementById("chassis").value;

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


    //VERIFICACION DATOS DE ORDEN

    //VERFICACION ORDEN-----------------------------------------

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

    diascredito = (diascredito == '' || diascredito == null) ? '0' : diascredito;
    obs = (obs == '' || obs == null) ? '' : obs;

    //VERIFICACION CLIENTE-----------------------------------------

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

    direccion = (direccion == '' || direccion == null) ? '' : direccion;

    if (nit == '' || nit == null) {
        $("#mensaje").html("El NIT no puede estar vacío.");
        $('#modal-generico').modal('show');
        return;
    }

    if (telefono == '' || telefono == null) {
        $("#mensaje").html("Debe ingresar un número de teléfono.");
        $('#modal-generico').modal('show');
        return;
    }

    correo = (correo == '' || correo == null) ? '' : correo;

    //VERIFICACION DATOS ASEGURADORA-----------------------------------------

    aseguradoras = (aseguradoras == '' || aseguradoras == null) ? '' : aseguradoras;
    poliza = (poliza == '' || poliza == null) ? '' : poliza;
    asesoremergencia = (asesoremergencia == '' || asesoremergencia == null) ? '' : asesoremergencia;
    reclamo = (reclamo == '' || reclamo == null) ? '' : reclamo;
    corredora = (corredora == '' || corredora == null) ? '' : corredora;
    deducible = (deducible == '' || deducible == null) ? '0' : deducible;
    ajustador = (ajustador == '' || ajustador == null) ? '' : ajustador;

    //VERIFICACION DATOS VEHICULO-------------------------------------------

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
        $("#mensaje").html("Debe seleccionar una línea de vehículo.");
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

    chassis = (chassis == '' || chassis == null) ? '' : chassis;

    //VERIFICACION CHECKLIST
    radio = (radio == true) ? '1' : '0';
    encendedor = (encendedor == true) ? '1' : '0';
    documentos = (documentos == true) ? '1' : '0';
    alfombras = (alfombras == true) ? '1' : '0';
    llanta = (llanta == true) ? '1' : '0';
    tricket = (tricket == true) ? '1' : '0';
    llave = (llave == true) ? '1' : '0';
    herramienta = (herramienta == true) ? '1' : '0';
    platos = (platos == true) ? '1' : '0';

    let arr = { id_orden, serie, fechaingreso, fechaentrega, asesores, tipoVenta, diascredito, obs, idcliente, cliente, direccion, nit, telefono, correo, aseguradoras, poliza, asesoremergencia, reclamo, corredora, deducible, ajustador, opciones, tiposvehiculos, marca, lineavehiculo, modelo, tipoplaca, placa, color, kilometraje, medida, chassis, radio, encendedor, documentos, alfombras, llanta, tricket, llave, herramienta, platos, total };
    let informacion = JSON.stringify(arr);

    let items = [];

    $("#tabladetalle tr").each(function () {

        let itemOrdenSrv = {};
        let tds = $(this).find("td");

        itemOrdenSrv.codigo = tds.filter(":eq(0)").text();
        itemOrdenSrv.cantidad = tds.filter(":eq(1)").text();
        itemOrdenSrv.producto = tds.filter(":eq(2)").text();
        itemOrdenSrv.servicio = tds.filter(":eq(3)").text();
        itemOrdenSrv.precio = tds.filter(":eq(4)").text();
        itemOrdenSrv.descto = tds.filter(":eq(5)").text();
        itemOrdenSrv.subtotal = tds.filter(":eq(6)").text();
        itemOrdenSrv.tipo  = tds.filter(":eq(7)").find("select").val();
        //alert("ID CODIGO " + tds.filter(":eq(0)").text() + "LINEA " + tds.filter(":eq(7)").find("select").val());
        items.push(itemOrdenSrv);

    });

    let ordenServ = {};
    ordenServ = items;
    let jsonTablaDetalle = JSON.stringify(ordenServ);

    let sizeTablaDetalle = ordenServ.length;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        data: { informacion, jsonTablaDetalle, sizeTablaDetalle },
        url: "/OrdenTecnicaMod/GuardarOrdenServicio",

        success: function (response) {
            var arreglo = response;

            if (response.CODIGO == "TRUE") {
                document.getElementById("Error").className = "alert alert-success";
                $('#Confirmacion').html("Orden Modificada Correctamente");
                $('#ModalConfirmacion').modal('show');
                PDFOrdenServicio(id_orden, serie);
            }

            if (response.CODIGO == "ERROR") {

                if (response.PRODUCTO == "") {
                    document.getElementById("Error").className = "alert alert-danger";
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
}


// FUNCION PARA AGREGAR PRODUCTOS A LA TABLA DETALLE
function AddDetalle() {

    let table = document.getElementById("tabladetalle");
    let subtotal = document.getElementById("tsubtotal").value;
    let nCantidad = $("#cantidad").val();
    let nPrecio = ConvertStringToFloat($("#precio").val()); 
    let tSubtotal = ConvertStringToFloat($("#tsubtotal").val());
    let tipo = document.getElementById("tipo").value;

    //validarDesctoVacio();


    //COMENTADO
    //if (subtotal <= 0) {
    //    return;
    //}

    //if (descuento < 0) {
    //    $('#Error').html("EL DESCUENTO NO PUEDE SER MENOR IGUAL A 0");
    //    $('#ModalError').modal('show');
    //    return;
    //}

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

    //if (nCantidad > existencia) {
    //    $('#Error').html("LA CANTIDAD NO PUEDE SER MAYOR A LA EXISTENCIA");
    //    $('#ModalError').modal('show');
    //    $("#cantidad").focus();
    //    return;
    //}

    if (document.getElementById("id_codigo").value == "") {
        $('#Error').html("Codigo de producto vacio");
        $('#ModalError').modal('show');
        $("#id_codigo").focus();
        return;
    }

    let row = table.insertRow();
    row.className = 'otrasFilas';
    let cellIDCod = row.insertCell(0);

    let cellCantidad = row.insertCell(1);
    cellCantidad.contentEditable = true;
    cellCantidad.addEventListener("input", function () {
        recorrerTablaModificada();
    });

    let cellProducto = row.insertCell(2);
    cellProducto.contentEditable = true;

    let cellServicio = row.insertCell(3);

    let cellPrecio = row.insertCell(4);
    cellPrecio.contentEditable = true;
    cellPrecio.addEventListener("input", function () {
        recorrerTablaModificada();
    });

    let cellDescto = row.insertCell(5);
    let cellSubtotal = row.insertCell(6);
    let cellTipo = row.insertCell(7);
    cellTipo.className = 'celdaTipo';
    let cellEliminar = row.insertCell(8);

    cellIDCod.innerHTML = $("#id_codigo").val();
    cellCantidad.innerHTML = $("#cantidad").val();
    cellProducto.innerHTML = $("#producto").val();
    cellServicio.innerHTML = $("#servicio").val();
    cellPrecio.innerHTML = nPrecio;
    cellDescto.innerHTML = "0.00";
    cellSubtotal.innerHTML = tSubtotal;
    cellTipo.innerHTML = CargarCatLineasi();

    let eliminarProducto = '<i class="fas fa-trash" style="color:red;" onclick="DeleteDetalle(this)"> </i>'; //AGREGA EL ICONO DE ELIMINAR
    cellEliminar.innerHTML = eliminarProducto;

    document.getElementById("id_codigo").value = "";
    document.getElementById("cantidad").value = "0";
    document.getElementById("existencia").value = "";
    document.getElementById("precio").value = "0";
    document.getElementById("tsubtotal").value = "0";
    document.getElementById("codigoe").value = "";
    document.getElementById("producto").value = "";

    document.getElementById("id_codigo").focus();

    recorrerTabla();
}






// FUNCION PARA CALCULAR VALORES
function CalcularValoresGenerico() {

    let nTotal;
    //let nDescto;
    let nSubTotal = 0;

    let cantidad = $("#cantidad").val();
    let precio = $("#precio").val();
    //var descto = $("#descto").val();

    ////DESCUENTO
    //var d = parseFloat(descto);
    //d = (precio * (d / 100)) * cantidad;
    //nDescto = d;
    //descuento_ = nDescto

    //precio.toFixed(2);

    precio = parseFloat(precio);


    //TOTAL SIN DESCUENTO
    nTotal = cantidad * precio;

    //SUBTOTAL = TOTAL - DESCUENTO
    //nSubTotal = nTotal - nDescto;
    //nSubTotal.toFixed(2);
    nSubTotal = nTotal;
    nSubTotal.toFixed(2);

    //$("#tsubtotal").text(formatNumber.new(parseFloat(nSubTotal).toFixed(2)));

    document.getElementById("precio").value = precio;
    document.getElementById("tsubtotal").value = nSubTotal;

    //document.getElementById("btnagregar").focus();

}



// FUNCION CONVERTIR STRING A FLOAT
function ConvertStringToFloat(s) {

    let decimal = parseFloat(s).toFixed(2);
    return decimal;
}



//ELIMINAR DETALLE
function DeleteDetalle(element) {
    let seElimina = confirm("¿Seguro que desea eliminar el articulo?");
    if (seElimina == true) {
        $(element).closest("tr").remove();
        recorrerTabla();
    }
    nDetalle = nDetalle - 1;

    total = 0.00;
}













/*FUNCION PARA BUSCAR LINEA DE VEHICULO SEGUN VEHICULO*/
function cargaDDLSecundario(val) {

    marca = $("#marca").val();

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoOrdenServicio/Cargar",
        data: { marca },
        success: function (response) {

            $('#lineavehiculoo').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}













// RECORREMOS LA TABLA 
function recorrerTabla() {

    total = 0.00;
    var items = [];

    $("#tabladetalle tr").each(function () {

        var itemProducto = {};

        var tds = $(this).find("td");

        itemProducto.subtotal = tds.filter(":eq(6)").text().replace(",", "");
        items.push(itemProducto);

        //Corrige el dato del total para que pueda realizar la suma
        if (itemProducto.subtotal == null || itemProducto.subtotal == "") {
            itemProducto.subtotal = 0.00;
            total = itemProducto.subtotal + (parseFloat(total) + parseFloat(itemProducto.subtotal)).toFixed(2);
        }

        parseFloat(itemProducto.subtotal);
        total = (parseFloat(total) + parseFloat(itemProducto.subtotal)).toFixed(2);
        document.getElementById("total").innerHTML = total;
    });
}


function Reload() {

    location.reload();

}
