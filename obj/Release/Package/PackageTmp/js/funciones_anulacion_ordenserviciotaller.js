//$(document).ready(Orden);



window.onload = function () {

    let total = 0.00;
}

let anulaid = "";
let anulaserie = "";







function abrirBusqueda() {

    document.getElementById("panel_busqueda").className = "panel-container";

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

    document.getElementById("b_placa").value = "";
    document.getElementById("fecha1").value = "";
    document.getElementById("fecha2").value = "";

}

function limparaPlaca() {

    document.getElementById("bid_orden").value = "";
    document.getElementById("fecha1").value = "";
    document.getElementById("fecha2").value = "";

}


function limparaFecha() {

    document.getElementById("bid_orden").value = "";
    document.getElementById("b_placa").value = "";

}



function ModalAnular(id, serie) {

    $('#modal-anular').modal('show');
    anulaid = id;
    anulaserie = serie;
    
}


function AnularOrden() {

    alert(anulaid + "" + anulaserie);

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        data: { anulaid, anulaserie },
        url: "/AnulacionOrdenServicioTaller/AnularOrdenServicio",

        success: function (response) {

            var arreglo = response;

            if (response.CODIGO == "TRUE") {

                document.getElementById("Error").className = "alert alert-success";
                $('#mensaje').html("Orden Anulada Correctamente");
                $('#modal-generico').modal('show');
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



//FUNCION BUSCAR ORDEN TECNICA POR FILTROS, GENERA LISTADO
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
        url: "/AnulacionOrdenServicioTaller/GetListaOrden",
        data: { bid_orden, b_placa, fecha1, fecha2 },
        beforeSend: OrdenTecnica,
        success: ResultadoConsulta
    });

    function OrdenTecnica() {
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
                        MARCAVEHICULO: { type: "string" },
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
                        "<button class='btn btn-xs btn-dark waves-effect waves-themed' onclick='PDFOrdenServicio(#=(IDORDEN)# , \"#=(SERIE)#\")';'>PDF</button>" +
                        "<button class='btn btn-xs btn-danger waves-effect waves-themed' onclick='ModalAnular(#=(IDORDEN)# , \"#=(SERIE)#\")';'>Anular</button>",
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



//function BuscarFiltroOrifinal() {

//    let bid_orden = document.getElementById("bid_orden").value;
//    //let bidcliente = document.getElementById("bidcliente").value;
//    let fecha1 = document.getElementById("fecha1").value;
//    let fecha2 = document.getElementById("fecha2").value;

//    $.ajax({
//        async: false,
//        type: "POST",
//        dataType: "HTML",
//        contentType: "application/x-www-form-urlencoded",
//        url: "/AnulacionOrdenServicioTaller/GetListaOrden",
//        data: { bid_orden, fecha1, fecha2 },
//        beforeSend: OrdenTecnica,
//        success: Consulta
//    });

//    function OrdenTecnica() {
//        $('#mostrar_consulta').html('Cargando por favor espere...');
//    }

//    function Consulta(data) {
//        var arreglo = eval(data);
//        cData = arreglo;

//        var columnas = [
//            {
//                dataField: "OPC",
//                alignment: "CENTER",
//                width: 180,
//                allowFiltering: false,
//                allowSorting: false,
//                cellTemplate: function (container, options) {
//                    //container.append($("<i style='color:#651FFF;cursor:pointer;' class='.VER_ORD' onclick='BuscarOrden(this.value);obtieneserie(this)'> <img src='/images/Load.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);
//                    container.append($("<button value=\"" + options.key + "\" onclick=\"PDFOrdenServicio(this.value, '" + options.values[2] + "');\" class=\"btn btn-xs btn-dark waves-effect waves-themed\">PDF</button><button value=\"" + options.key + "\" onclick=\"ModalAnular(this.value, '" + options.values[2] + "');\" class=\"btn btn-xs btn-danger waves-effect waves-themed\">Anular</button>")).appendTo(container);
//                    //container.append($("<button value=\"" + options.key + "\" onclick=\"BuscarOrden(this.value);obtieneserie('" + options.values[2] + "');\" class=\"btn btn-xs btn-dark waves-effect waves-themed\">Mod.</button>")).appendTo(container);
//                    //container.append($("<button value=\"" + options.key + "\" onclick=\"BuscarOrden(this.value);obtieneserie(this);\" class=\"btn btn-xs btn-dark waves-effect waves-themed\">Mod.</button>")).appendTo(container);
//                }
//            },
//            {
//                dataField: "IDORDEN",
//                alignment: "CENTER",
//                width: 80,
//                allowFiltering: false,
//                allowSorting: false,
//            },
//            {
//                dataField: "SERIE",
//                alignment: "CENTER",
//                width: 80,
//                allowFiltering: false,
//                allowSorting: false,
//            },
//            {
//                dataField: "FECHA",
//                alignment: "CENTER",
//                width: 110,
//                allowFiltering: false,
//                allowSorting: false,
//            },
//            {
//                dataField: "STATUS",
//                alignment: "CENTER",
//                width: 80,
//                allowFiltering: false,
//                allowSorting: false,
//            },
//            {
//                dataField: "IDCLIENTE",
//                alignment: "CENTER",
//                width: 100,
//                allowFiltering: true,
//                allowSorting: false,
//            },
//            {
//                dataField: "CLIENTE",
//                alignment: "LEFT",
//                width: 300,
//                allowFiltering: true,
//                allowSorting: false,
//            },
//            {
//                dataField: "NIT",
//                alignment: "CENTER",
//                width: 90,
//                allowFiltering: false,
//                allowSorting: false,
//            },
//            {
//                dataField: "PLACA",
//                alignment: "CENTER",
//                width: 90,
//            },
//            {
//                dataField: "MARCAVEHICULO",
//                alignment: "CENTER",
//                width: 150,
//            },
//            {
//                dataField: "LINEA",
//                alignment: "CENTER",
//                width: 150,
//            },
//            {
//                dataField: "TOTAL",
//                alignment: "RIGHT",
//                width: 150,
//                dataType: 'number',
//                format: '#,##0.00',
//            }];


//        $(function () {
//            var dataGrid = $("#gridContainer").dxDataGrid({
//                dataSource: cData,
//                keyExpr: "IDORDEN",
//                selection: {
//                    mode: "single"
//                },
//                "export": {
//                    enabled: false,
//                    fileName: "OrdenTecnica",
//                    allowExportSelectedData: false
//                },
//                dataSource: cData,
//                rowAlternationEnabled: true,
//                allowColumnReordering: true,
//                allowColumnResizing: true,
//                columnHidingEnabled: false,
//                showBorders: true,
//                columnChooser: {
//                    enabled: true
//                },
//                columnFixing: {
//                    enabled: true
//                },
//                grouping: {
//                    autoExpandAll: false,
//                },
//                searchPanel: {
//                    visible: true,
//                    width: 240,
//                    placeholder: "Search..."
//                },
//                //headerFilter: {
//                //    visible: true
//                //},
//                //focusedRowEnabled: false,
//                paging: {
//                    pageSize: 18
//                },
//                groupPanel: {
//                    visible: true
//                },
//                pager: {
//                    showPageSizeSelector: true,
//                    showInfo: true
//                },
//                columns: columnas,

//            }).dxDataGrid("instance");

//            $("#autoExpand").dxCheckBox({
//                value: true,
//                text: "Expandir",
//                onValueChanged: function (data) {
//                    dataGrid.option("grouping.autoExpandAll", data.value);
//                }
//            });
//        });

//        $('#mostrar_consulta').html('');


//        document.getElementById("panel_tabla").className = "panel-container";
//        document.getElementById("panel_datos").className = "panel-container collapse";
//        eliminarAllDetalle();
//    }
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



// FUNCION CONVERTIR STRING A FLOAT
function ConvertStringToFloat(s) {

    let decimal = parseFloat(s).toFixed(2);
    return decimal;
}








function Reload() {

    location.reload();

}
