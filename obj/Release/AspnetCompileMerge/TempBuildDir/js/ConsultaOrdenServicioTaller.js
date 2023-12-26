//$(document).ready(Orden);


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

    document.getElementById("bidcliente").value = "";
    document.getElementById("fecha1").value = "";
    document.getElementById("fecha2").value = "";

}

function limparaFecha() {

    document.getElementById("bid_orden").value = "";
    document.getElementById("bidcliente").value = "";

}





//FUNCION BUSCAR ORDEN TECNICA POR FILTROS, GENERA LISTADO
function BuscarFiltro() {

    let bid_orden = document.getElementById("bid_orden").value;
    let bidcliente = document.getElementById("bidcliente").value;
    let fecha1 = document.getElementById("fecha1").value;
    let fecha2 = document.getElementById("fecha2").value;

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/ConsultaOrdenServicioTaller/GetListaOrden",
        data: { bid_orden, bidcliente, fecha1, fecha2 },
        beforeSend: OrdenTecnica,
        success: Consulta
    });

    function OrdenTecnica() {
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function Consulta(data) {
        var arreglo = eval(data);
        cData = arreglo;

        var columnas = [
            {
                dataField: "OPC",
                alignment: "CENTER",
                width: 150,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    //container.append($("<i style='color:#651FFF;cursor:pointer;' class='.VER_ORD' onclick='BuscarOrden(this.value);obtieneserie(this)'> <img src='/images/Load.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);
                    container.append($("<button value=\"" + options.key + "\" onclick=\"PDFOrdenServicio(this.value, '" + options.values[2] + "');\" class=\"btn btn-xs btn-dark waves-effect waves-themed\">PDF</button>")).appendTo(container);
                    //container.append($("<button value=\"" + options.key + "\" onclick=\"BuscarOrden(this.value);obtieneserie('" + options.values[2] + "');\" class=\"btn btn-xs btn-dark waves-effect waves-themed\">Mod.</button>")).appendTo(container);
                    //container.append($("<button value=\"" + options.key + "\" onclick=\"BuscarOrden(this.value);obtieneserie(this);\" class=\"btn btn-xs btn-dark waves-effect waves-themed\">Mod.</button>")).appendTo(container);
                }
            },
            {
                dataField: "IDORDEN",
                alignment: "CENTER",
                width: 80,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "SERIE",
                alignment: "CENTER",
                width: 80,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "FECHA",
                alignment: "CENTER",
                width: 110,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "STATUS",
                alignment: "CENTER",
                width: 80,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "IDCLIENTE",
                alignment: "CENTER",
                width: 100,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "CLIENTE",
                alignment: "LEFT",
                width: 300,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "NIT",
                alignment: "CENTER",
                width: 90,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "PLACA",
                alignment: "CENTER",
                width: 90,
            },
            {
                dataField: "MARCAVEHICULO",
                alignment: "CENTER",
                width: 150,
            },
            {
                dataField: "LINEA",
                alignment: "CENTER",
                width: 150,
            },
            {
                dataField: "TOTAL",
                alignment: "RIGHT",
                width: 150,
                dataType: 'number',
                format: '#,##0.00',
            }];


        $(function () {
            var dataGrid = $("#gridContainer").dxDataGrid({
                dataSource: cData,
                keyExpr: "IDORDEN",
                selection: {
                    mode: "single"
                },
                "export": {
                    enabled: false,
                    fileName: "OrdenTecnica",
                    allowExportSelectedData: false
                },
                dataSource: cData,
                rowAlternationEnabled: true,
                allowColumnReordering: true,
                allowColumnResizing: true,
                columnHidingEnabled: false,
                showBorders: true,
                columnChooser: {
                    enabled: true
                },
                columnFixing: {
                    enabled: true
                },
                grouping: {
                    autoExpandAll: false,
                },
                searchPanel: {
                    visible: true,
                    width: 240,
                    placeholder: "Search..."
                },
                //headerFilter: {
                //    visible: true
                //},
                //focusedRowEnabled: false,
                paging: {
                    pageSize: 18
                },
                groupPanel: {
                    visible: true
                },
                pager: {
                    showPageSizeSelector: true,
                    showInfo: true
                },
                columns: columnas,

            }).dxDataGrid("instance");

            $("#autoExpand").dxCheckBox({
                value: true,
                text: "Expandir",
                onValueChanged: function (data) {
                    dataGrid.option("grouping.autoExpandAll", data.value);
                }
            });
        });

        $('#mostrar_consulta').html('');


        document.getElementById("panel_tabla").className = "panel-container";
        document.getElementById("panel_datos").className = "panel-container collapse";

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

    document.getElementById("bidcliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[0].innerHTML;
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
        url: "/ConsultaOrdenServicioTaller/GetDataCliente",
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




//function obtieneserie(serie) {

//    return serie;
//}

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
            $("#contacto").val(arreglo.CONTACTO);
            //$("#telefono").val(arreglo.TELEFONO);
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

            BuscarDetalleOrden();
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










//FUNCION PATA CARGAR EL DETALLE DE LA ORDEN
function BuscarDetalleOrden() {

    let id_orden = document.getElementById("id_orden").value;
    let serie = document.getElementById("serie").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/OrdenTecnicaMod/BuscaDetalleOrden",
        data: { id_orden, serie },
        success: function (response) {

            for (var i = 0; i < response.length; i++) {

                var table = document.getElementById("tabladetalle");

                var d = new Date();
                //var idTemporal = create_UUID();// d.getDate() + d.getMonth() + 1 + d.getFullYear() + d.getMinutes() + d.getSeconds() + d.getMilliseconds();

                var row = table.insertRow();

                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);
                var cell3 = row.insertCell(2);
                var cell4 = row.insertCell(3);
                var cell5 = row.insertCell(4);
                var cell6 = row.insertCell(5);
                var cell7 = row.insertCell(6);
                let fila = $("#tabladetalle tr").length;
                file = fila - 1;


                cell1.innerHTML = response[i].id_codigo;
                cell2.innerHTML = response[i].cantidad;
                cell3.innerHTML = response[i].obs;
                cell4.innerHTML = response[i].precio;
                cell5.innerHTML = response[i].descto;
                cell6.innerHTML = response[i].subtotal;
                let agregarArchivo = '<i class="fas fa-trash" style="color:red;" onclick="DeleteDetalle(this)"> </i>';
                //let agregarArchivo = '<i class="fas fa-upload" onclick="openUploadPicture(this)"> </i> <i class="fas fa-trash" style="color:red;" onclick="DeleteDetalle(this)"> </i><i class="fas fa-sync agrega-info" style="color:green;" data-toggle="modal" data-target="#editar-detalle" correlativo="' + response[i].id_codigo + '" subtotal="' + response[i].subtotal + '" producto="' + response[i].producto + '" cantidad="' + response[i].cantidad + '" precio="' + response[i].precio + '" fila="' + fila + '" > </i>'; //AGREGA EL ICONO DE SUBIR
                cell7.innerHTML = agregarArchivo;
                //cell9.id = idTemporal; //ASIGNA AL ID DEL <i> EL IDTEMPORAL PARA SUBIR

                //alert("hola" + i);

                //document.getElementById("idTemporal").value = "";
                //document.getElementById("codigo").focus();
                //AddDetalle();
                recorrerTabla();
            }
            //var arreglo = response;
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
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

    document.getElementById("id_codigo").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("codigoe").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[1].innerHTML;
    document.getElementById("producto").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[2].innerHTML;
    document.getElementById("precio").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[3].innerHTML;
    document.getElementById("existencia").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[4].innerHTML;

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
    let idcliente = document.getElementById("idcliente").value;
    let cliente = document.getElementById("cliente").value;
    let direccion = document.getElementById("direccion").value;
    let nit = document.getElementById("nit").value;
    let tipoVenta = document.getElementById("tipoVenta").value;
    let diascredito = document.getElementById("diascredito").value;
    let contacto = document.getElementById("contacto").value;
    let telefono = document.getElementById("telefono").value;
    let correo = document.getElementById("correo").value;
    let obs = document.getElementById("obs").value;

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
    let chassis = document.getElementById("chassis").value;
    let kilometraje = document.getElementById("kilometraje").value;
    let medida = document.getElementById("medida").value;


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

    if (direccion == '' || direccion == null) {
        $("#mensaje").html("La dirección no puede estar vacía.");
        $('#modal-generico').modal('show');
        return;
    }

    if (nit == '' || nit == null) {
        document.getElementById("nit").value = 'CF';
    }

    if (diascredito == '' || diascredito == null) {
        document.getElementById("diascredito").value = '0';
    }

    if (contacto == '' || contacto == null) {
        $("#mensaje").html("Debe ingresar un contacto.");
        $('#modal-generico').modal('show');
        return;
    }

    if (telefono == '' || telefono == null) {
        $("#mensaje").html("Debe ingresar un número de teléfono.");
        $('#modal-generico').modal('show');
        return;
    }

    //VERIFICACION DATOS ASEGURADORA

    if (aseguradoras == '' || aseguradoras == null) {
        document.getElementById("aseguradoras").value = '0';
    }

    if (asesoremergencia == '' || asesoremergencia == null) {
        document.getElementById("asesoremergencia").value = '';
    }

    if (poliza == '' || poliza == null) {
        document.getElementById("poliza").value = '';
    }

    if (reclamo == '' || reclamo == null) {
        document.getElementById("reclamo").value = '';
    }

    if (corredora == '' || corredora == null) {
        document.getElementById("reclamo").value = '';
    }

    if (deducible == '' || deducible == null) {
        document.getElementById("reclamo").value = '0.00';
    }

    if (ajustador == '' || ajustador == null) {
        document.getElementById("ajustador").value = '';
    }

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

    //if (lineavehiculo == '' || lineavehiculo == null) {
    //    $("#mensaje").html("Debe seleccionar una línea de vehículo.");
    //    $('#modal-generico').modal('show');
    //    return;
    //}

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

    if (chassis == '' || chassis == null) {
        document.getElementById("chassis").value = '';
        return;
    }

    if (kilometraje == '' || kilometraje == null) {
        $("#mensaje").html("Debe ingresar el kilometraje del vehículo.");
        $('#modal-generico').modal('show');
        return;
    }

    let arr = { id_orden, serie, fechaingreso, fechaentrega, asesores, idcliente, cliente, direccion, nit, tipoVenta, diascredito, obs, contacto, telefono, aseguradoras, poliza, asesoremergencia, reclamo, corredora, deducible, ajustador, opciones, tiposvehiculos, marca, lineavehiculo, modelo, tipoplaca, placa, color, chassis, kilometraje, medida };
    let informacion = JSON.stringify(arr);

    let items = [];

    $("#tabladetalle tr").each(function () {

        let itemOrdenSrv = {};
        let tds = $(this).find("td");

        itemOrdenSrv.codigo = tds.filter(":eq(0)").text();
        itemOrdenSrv.cantidad = tds.filter(":eq(1)").text();
        itemOrdenSrv.producto = tds.filter(":eq(2)").text();
        itemOrdenSrv.precio = tds.filter(":eq(3)").text();
        itemOrdenSrv.subtotal = tds.filter(":eq(5)").text();
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





}


// FUNCION PARA AGREGAR PRODUCTOS A LA TABLA DETALLE
function AddDetalle() {

    let table = document.getElementById("tabladetalle");
    let subtotal = document.getElementById("tsubtotal").value;
    let nCantidad = $("#cantidad").val();
    let nPrecio = ConvertStringToFloat($("#precio").val());
    let tSubtotal = ConvertStringToFloat($("#tsubtotal").val());

    //validarDesctoVacio();

    if (subtotal <= 0) {
        return;
    }

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

    var row = table.insertRow();
    var cell1 = row.insertCell(0);
    var cell2 = row.insertCell(1);
    var cell3 = row.insertCell(2);
    var cell4 = row.insertCell(3);
    var cell5 = row.insertCell(4);
    var cell6 = row.insertCell(5);
    var cell7 = row.insertCell(6);

    cell1.innerHTML = $("#id_codigo").val();
    cell2.innerHTML = $("#cantidad").val();
    cell3.innerHTML = $("#producto").val();
    cell4.innerHTML = nPrecio;
    cell5.innerHTML = "0.00";
    cell6.innerHTML = tSubtotal;

    var eliminarProducto = '<i class="fas fa-trash" style="color:red;" onclick="DeleteDetalle(this)"> </i>'; //AGREGA EL ICONO DE ELIMINAR
    cell7.innerHTML = eliminarProducto;

    document.getElementById("id_codigo").value = "";
    document.getElementById("cantidad").value = "0";
    document.getElementById("existencia").value = "";
    document.getElementById("precio").value = "";
    document.getElementById("tsubtotal").value = "";
    document.getElementById("codigoe").value = "";
    document.getElementById("producto").value = "";

    document.getElementById("id_codigo").focus();

    recorrerTabla();
}



// RECORREMOS LA TABLA 
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
        document.getElementById("total").innerHTML = total;
    });
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
}


