//$(document).ready(InicionEventos);

//function InicionEventos() {

//    $('#gridContainer').on('click', '.UUID', BuscarFactura);

//}

function BuscarGeneralFacturas() {

    if (verificarCampos() != 'false') {

        var fecha1 = document.getElementById("fecha1").value;
        var fecha2 = document.getElementById("fecha2").value;
        var comboagencia = document.getElementById("agencia");
        var agencia = comboagencia.options[comboagencia.selectedIndex].value;

        $.ajax({
            async: false,
            type: "POST",
            dataType: "HTML",
            contentType: "application/x-www-form-urlencoded",
            url: "/ReporteGeneralFacturas/GenerarReporteGenFacturas",
            data: { fecha1, fecha2, agencia },
            beforeSend: InicioConsulta,
            success: ResultadoConsulta
        });

        function InicioConsulta() {
            //$('#mostrar_consulta').html('Cargando por favor espere...');
        }

        function ResultadoConsulta(data) {

            var arreglo = eval(data);
            cData = arreglo;

            $('#mostrar_consulta').html('');

            var gridDataSource = new kendo.data.DataSource({
                data: cData,
                schema: {
                    model: {
                        fields: {
                            ID_INTERNO: { type: "number" },
                            SERIE_INTERNA: { type: "string" },
                            FECHA: { type: "string" },
                            NIT: { type: "string" },
                            CLIENTE: { type: "string" },
                            STATUS: { type: "string" },
                            VENDEDOR: { type: "string" },
                            TOTAL: { type: "number" },
                            AGENCIA: { type: "number" },
                            SALDO: { type: "number" },
                            CONDICION: { type: "string" },
                            NOAUTORIZACION: { type: "string" },
                            NO_DOCTO: { type: "string" },
                            SERIE_FEL: { type: "string" },
                            NOACCESO: { type: "string" },
                            MAIL: { type: "string" },
                            RECIBOPAGO: { type: "string" },
                            FECHAAPLICACION: { type: "string" },
                            BOLETA: { type: "string" },
                            BANCO: { type: "string" },
                            CONTRASENA: { type: "string" }
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
                    fileName: "Reporte General de Facturas.xlsx",
                    filterable: true,
                    allPages: true
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
                        { name: "ID_INTERNO", operator: "eq" },
                        { name: "NIT", operator: "contains" },
                        { name: "CLIENTE", operator: "contains" },
                        { name: "NOAUTORIZACION", operator: "eq" },
                        { name: "NO_DOCTO", operator: "eq" },
                        { name: "SERIE_FEL", operator: "eq" },
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
                        field: "ID_INTERNO",
                        title: "ID Interno",
                        width: 90,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "SERIE_INTERNA",
                        title: "Serie Interna",
                        width: 90,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },

                    }, {
                        field: "FECHA",
                        title: "Fecha",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },

                    }, {
                        field: "NIT",
                        title: "Nit",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "CLIENTE",
                        title: "Cliente",
                        width: 300,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                    }, {
                        field: "STATUS",
                        title: "Status",
                        width: 90,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "VENDEDOR",
                        title: "Vendedor",
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
                    }, {
                        field: "AGENCIA",
                        title: "Agencia",
                        width: 90,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "SALDO",
                        title: "Saldo",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: right" },
                        format: "{0:n2}"
                    }, {
                        field: "CONDICION",
                        title: "Condicion",
                        width: 100,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "NOAUTORIZACION",
                        title: "Firma Electronica",
                        width: 200,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "NO_DOCTO",
                        title: "No. Documento",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "SERIE_FEL",
                        title: "Serie Fel",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "NOACCESO",
                        title: "No. Acceso",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "MAIL",
                        title: "Email",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "RECIBOPAGO",
                        title: "Recibo Pago",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "FECHAAPLICACION",
                        title: "Fecha de Aplicacion",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "BOLETA",
                        title: "Boleta",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "BANCO",
                        title: "Banco",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }, {
                        field: "CONTRASENA",
                        title: "Contrasena",
                        width: 130,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center" },
                    }],
                editable: "inline"
            });

        }
    }
}




//function BuscarGeneralFacturasOriginal() {

//    if (verificarCampos() != 'false') {

//        var fecha1 = document.getElementById("fecha1").value;
//        var fecha2 = document.getElementById("fecha2").value;
//        var comboagencia = document.getElementById("agencia");
//        var agencia = comboagencia.options[comboagencia.selectedIndex].value;

//        $.ajax({
//            async: false,
//            type: "POST",
//            dataType: "HTML",
//            contentType: "application/x-www-form-urlencoded",
//            url: "/ReporteGeneralFacturas/GenerarReporteGenFacturas",
//            data: { fecha1, fecha2, agencia },
//            beforeSend: InicioConsulta,
//            success: ConsultaRPOGenFacturas
//        });

//        function InicioConsulta() {
//            $('#mostrar_consulta').html('Cargando por favor espere...');
//        }

//        function ConsultaRPOGenFacturas(data) {
//            var arreglo = eval(data);
//            cData = arreglo;

//            console.log(cData);

//            var columnas = [
//                {
//                    dataField: "ID_INTERNO",
//                    alignment: "center",
//                    width: 75,
//                    allowFiltering: true,
//                    allowSorting: true,
//                },
//                {
//                    dataField: "SERIE_INTERNA",
//                    alignment: "center",
//                    width: 98,
//                    allowFiltering: false,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "FECHA",
//                    alignment: "CENTER",
//                    width: 100,
//                    allowFiltering: true,
//                    allowSorting: true,
//                    dataType: 'date',
//                    format: 'yyyy-MM-dd',
//                },
//                {
//                    dataField: "NIT",
//                    alignment: "LEFT",
//                    width: 125,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "CLIENTE",
//                    alignment: "LEFT",
//                    width: 200,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "STATUS",
//                    alignment: "CENTER",
//                    width: 50,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "VENDEDOR",
//                    alignment: "left",
//                    width: 75,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "TOTAL",
//                    alignment: "LEFT",
//                    width: 125,
//                    allowFiltering: true,
//                    allowSorting: false,
//                    dataType: 'number',
//                    format: '#,##0.00',
//                },
//                {
//                    dataField: "AGENCIA",
//                    alignment: "left",
//                    width: 50,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "SALDO",
//                    alignment: "right",
//                    width: 125,
//                    allowFiltering: false,
//                    allowSorting: false,
//                    dataType: 'number',
//                    format: '#,##0.00',
//                },
//                {
//                    dataField: "CONDICION",
//                    alignment: "right",
//                    width: 75,
//                    allowFiltering: false,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "NOAUTORIZACION",
//                    alignment: "left",
//                    width: 150,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "NO_DOCTO",
//                    alignment: "left",
//                    width: 125,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "SERIE_FEL",
//                    alignment: "left",
//                    width: 125,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "NOACCESO",
//                    alignment: "left",
//                    width: 150,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "MAIL",
//                    alignment: "left",
//                    width: 150,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "RECIBOPAGO",
//                    alignment: "left",
//                    width: 150,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "FECHAAPLICACION",
//                    alignment: "left",
//                    width: 150,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "BOLETA",
//                    alignment: "left",
//                    width: 150,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "BANCO",
//                    alignment: "left",
//                    width: 150,
//                    allowFiltering: true,
//                    allowSorting: false,
//                },
//                {
//                    dataField: "CONTRASENA",
//                    alignment: "left",
//                    width: 150,
//                    allowFiltering: true,
//                    allowSorting: false,
//                }];




//            $(function () {
//                var dataGrid = $("#gridContainer").dxDataGrid({
//                    "export": {
//                        enabled: true,
//                        fileName: "Consulta de Facturas",
//                        allowExportSelectedData: true
//                    },
//                    dataSource: cData,
//                    rowAlternationEnabled: true,
//                    allowColumnReordering: true,
//                    allowColumnResizing: true,
//                    columnHidingEnabled: true,
//                    showBorders: true,
//                    columnChooser: {
//                        enabled: true
//                    },
//                    columnFixing: {
//                        enabled: true
//                    },
//                    grouping: {
//                        autoExpandAll: false,
//                    },
//                    searchPanel: {
//                        visible: true,
//                        width: 25,
//                        placeholder: "Search..."
//                    },
//                    //headerFilter: {
//                    //    visible: true
//                    //},
//                    paging: {
//                        pageSize: 25
//                    },
//                    groupPanel: {
//                        visible: true
//                    },
//                    pager: {
//                        showPageSizeSelector: true,
//                        showInfo: true
//                    },
//                    columns: columnas,

//                }).dxDataGrid("instance");

//                $("#autoExpand").dxCheckBox({
//                    value: false,
//                    text: "Expandir",
//                    onValueChanged: function (data) {
//                        dataGrid.option("grouping.autoExpandAll", data.value);
//                    }
//                });

//            });

//            $('#mostrar_consulta').html('');
//        }
//    }
//}



function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}



function verificarCampos() {

    var fecha1 = document.getElementById("fecha1").value;
    var fecha2 = document.getElementById("fecha2").value;

    if (fecha1 == '' && fecha2 != '') {

        document.getElementById("mensaje").innerHTML = "Debe seleccionar ambos rangos de fecha.";
        $('#modal-generico').modal('show');
        return 'false';

    } else if (fecha2 == '' && fecha1 != '') {

        document.getElementById("mensaje").innerHTML = "Debe seleccionar ambos rangos de fecha.";
        $("#modal-generico").modal('show');
        return 'false';
    }
}