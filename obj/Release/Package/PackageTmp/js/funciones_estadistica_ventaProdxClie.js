$(document).ready(InicionEventos);

function InicionEventos() {
    $('#gridContainer').on('click', '.UUID', BuscarFactura);
}


function ActivarCasilla(casilla) {
    var chckFacturas = document.getElementById("defaultUnchecked");
    var chckEnvios = document.getElementById("defaultUnchecked2");
    var chckConsolidado = document.getElementById("defaultUnchecked3");

    if (chckConsolidado.checked == true ) {
        chckFacturas.checked = false;
        chckEnvios.checked = false;
    } else if (chckFacturas.checked == true || chckEnvios.checked == true) {
        chckConsolidado.checked == false;
    }

    //if (chckConsolidado.checked === true) {
    //    chckFacturas.checked = false;
    //    chckEnvios.checked = false;
    //} else if (chckFacturas.checked === true) {
    //    chckEnvios.checked = false;
    //    chckConsolidado.checked == false;
    //} else if (chckEnvios.checked === true) {
    //    chckFacturas.checked = false;
    //    chckConsolidado.checked == false;
    //}
}


function BuscarEstadisticaVentaxProd() {

    let idcliente = document.getElementById("idcliente").value;
    let cliente = document.getElementById("cliente").value;
    let fecha1 = document.getElementById("fecha1").value;
    let fecha2 = document.getElementById("fecha2").value;
    let chckFacturas = document.getElementById("defaultUnchecked").checked;
    let chckEnvios = document.getElementById("defaultUnchecked2").checked;
    let chckConsolidado = document.getElementById("defaultUnchecked3").checked;

    let facturas;
    let envios;
    let consolidado;

    if (chckFacturas == true) {
        facturas = 'S';
    } else {
        facturas = 'N';
    }

    if (chckEnvios == true) {
        envios = 'S';
    } else {
        envios = 'N';
    }

    if (chckConsolidado == true) {
        consolidado = 'S';

    } else {
        consolidado = 'N';
    }

        $.ajax({
            async: false,
            type: "POST",
            dataType: "HTML",
            contentType: "application/x-www-form-urlencoded",
            url: "/EstadisticaVentaProdxClie/GenerarEstadisticaVentaProdxClie",
            data: { idcliente, cliente, fecha1, fecha2, facturas, envios, consolidado },
            beforeSend: InicioConsulta,
            success: ConsultaEstadisticaVentaProdxClie
        });

        function InicioConsulta() {
            //$('#mostrar_consulta').html('Cargando por favor espere...');
        }

        function ConsultaEstadisticaVentaProdxClie(data) {

            var arreglo = eval(data);
            cData = arreglo;

            $('#gridContainer').html('');

            let gridDataSource = new kendo.data.DataSource({
                data: cData,
                schema: {
                    model: {
                        fields: {
                            IDCLIENTE: { type: "number" },
                            CLIENTE: { type: "string" },
                            CODIGO: { type: "number" },
                            CODIGOE: { type: "string" },
                            ALIAS: { type: "string" },
                            PRODUCTO: { type: "string" },
                            ENE: { type: "string" },
                            FEB: { type: "string" },
                            MAR: { type: "string" },
                            ABR: { type: "string" },
                            MAY: { type: "string" },
                            JUN: { type: "string" },
                            JUL: { type: "string" },
                            AGO: { type: "string" },
                            SEP: { type: "string" },
                            OCT: { type: "string" },
                            NOV: { type: "string" },
                            DIC: { type: "string" },
                            IDVENDEDOR: { type: "number" },
                            FECHA: { type: "string" },
                            TIPODOCTO: { type: "string" },
                        }
                    }
                },
                //height: 550,
                pageSize: 20,
                //sort: {
                //    field: "OrderDate",
                //    dir: "desc"
                //}
            });

            $("#gridContainer").kendoGrid({
                toolbar: ["excel", "pdf", "search"], //Podemos colocar opciones en la toolbar del datagrid
                excel: {
                    fileName: "Est. Ventas producto por cliente.xlsx",
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
                        { name: "IDCLIENTE", operator: "eq" },
                        { name: "CLIENTE", operator: "contains" },
                        //{ name: "Freight", operator: "gte" },
                    ]
                },
                dataSource: gridDataSource,
                height: 800, //Tamaño en alto del DataGrid
                groupable: true,
                sortable: true,
                pageable: {
                    refresh: true,
                    pageSizes: true,
                    buttonCount: 5
                },
                filterable: true,
                columns: [{
                    field: "IDCLIENTE",
                    title: "ID Cliente",
                    width: 100
                }, {
                    field: "CLIENTE",
                    title: "Cliente",
                    width: 250,
                }, {
                    field: "CODIGO",
                    title: "Cod producto",
                    width: 130,
                }, {
                    field: "ALIAS",
                    title: "Alias",
                    width: 130,
                }, {
                    field: "PRODUCTO",
                    title: "Producto",
                    width: 250,
                }, {
                    field: "ENE",
                    title: "Enero",
                    width: 100,
                }, {
                    field: "FEB",
                    title: "Febrero",
                    width: 100,
                }, {
                    field: "MAR",
                    title: "Marzo",
                    width: 100,
                }, {
                    field: "ABR",
                    title: "Abril",
                    width: 100,
                }, {
                    field: "MAY",
                    title: "Mayo",
                    width: 100,
                }, {
                    field: "JUN",
                    title: "Junio",
                    width: 100,
                }, {
                    field: "JUL",
                    title: "Julio",
                    width: 100,
                }, {
                    field: "AGO",
                    title: "Agosto",
                    width: 100,
                }, {
                    field: "SEP",
                    title: "Septiembre",
                    width: 100,
                }, {
                    field: "OCT",
                    title: "Octubre",
                    width: 100,
                }, {
                    field: "NOV",
                    title: "Noviembre",
                    width: 100,
                }, {
                    field: "DIC",
                    title: "Diciembre",
                    width: 100,
                }, {
                    field: "IDVENDEDOR",
                    title: "ID Vendedor",
                    width: 100,
                }, {
                    field: "FECHA",
                    title: "Fecha",
                    width: 100,
                }, {
                    field: "TIPODOCTO",
                    title: "Tipo de documento",
                    width: 150,
                }]
            });

            //var columnas = [
            //    {
            //        dataField: "IDCLIENTE",
            //        alignment: "center",
            //        width: 100,
            //        allowFiltering: true,
            //        allowSorting: true,
            //    },
            //    {
            //        dataField: "CLIENTE",
            //        alignment: "left",
            //        width: 250,
            //        allowFiltering: false,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "CODIGO",
            //        alignment: "center",
            //        width: 98,
            //        allowFiltering: false,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "CODIGOE",
            //        alignment: "center",
            //        width: 98,
            //        allowFiltering: false,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "ALIAS",
            //        alignment: "center",
            //        width: 100,
            //        allowFiltering: false,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "PRODUCTO",
            //        alignment: "left",
            //        width: 250,
            //        allowFiltering: false,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "ENE",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: true,
            //    },
            //    {
            //        dataField: "FEB",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "MAR",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "ABR",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "MAY",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "JUN",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "JUL",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: false,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "AGO",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: false,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "SEP",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "OCT",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "NOV",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "DIC",
            //        alignment: "right",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "IDVENDEDOR",
            //        alignment: "left",
            //        width: 98,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    },
            //    {
            //        dataField: "TIPODOCTO",
            //        alignment: "left",
            //        width: 150,
            //        allowFiltering: true,
            //        allowSorting: false,
            //    }];




            //$(function () {
            //    var dataGrid = $("#gridContainer").dxDataGrid({
            //        "export": {
            //            enabled: true,
            //            fileName: "Estadisitca Venta Producto por Cliente",
            //            allowExportSelectedData: true
            //        },
            //        dataSource: cData,
            //        rowAlternationEnabled: true,
            //        allowColumnReordering: true,
            //        allowColumnResizing: true,
            //        columnHidingEnabled: true,
            //        showBorders: true,
            //        columnChooser: {
            //            enabled: true
            //        },
            //        columnFixing: {
            //            enabled: true
            //        },
            //        grouping: {
            //            autoExpandAll: false,
            //        },
            //        searchPanel: {
            //            visible: true,
            //            width: 240,
            //            placeholder: "Search..."
            //        },
            //        //headerFilter: {
            //        //    visible: true
            //        //},
            //        paging: {
            //            pageSize: 18
            //        },
            //        groupPanel: {
            //            visible: true
            //        },
            //        pager: {
            //            showPageSizeSelector: true,
            //            showInfo: true
            //        },
            //        columns: columnas,

            //    }).dxDataGrid("instance");

            //    $("#autoExpand").dxCheckBox({
            //        value: false,
            //        text: "Expandir",
            //        onValueChanged: function (data) {
            //            dataGrid.option("grouping.autoExpandAll", data.value);
            //        }
            //    });

            //});

            $('#mostrar_consulta').html('');
        }
}


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