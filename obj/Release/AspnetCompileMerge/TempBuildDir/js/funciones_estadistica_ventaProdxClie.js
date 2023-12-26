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

    var fecha1 = document.getElementById("fecha1").value;
    var fecha2 = document.getElementById("fecha2").value;
    var chckFacturas = document.getElementById("defaultUnchecked").checked;
    var chckEnvios = document.getElementById("defaultUnchecked2").checked;
    var chckConsolidado = document.getElementById("defaultUnchecked3").checked;

    var facturas;
    var envios;
    var consolidado;

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

    if (fecha1 != "" && fecha2 != "") {
        $.ajax({
            async: false,
            type: "POST",
            dataType: "HTML",
            contentType: "application/x-www-form-urlencoded",
            url: "/EstadisticaVentaProdxClie/GenerarEstadisticaVentaProdxClie",
            data: { fecha1, fecha2, facturas, envios, consolidado },
            beforeSend: InicioConsulta,
            success: ConsultaEstadisticaVentaProdxClie
        });

        function InicioConsulta() {
            $('#mostrar_consulta').html('Cargando por favor espere...');
        }

        function ConsultaEstadisticaVentaProdxClie(data) {
            var arreglo = eval(data);
            cData = arreglo;

            console.log(cData);

            var columnas = [
                {
                    dataField: "IDCLIENTE",
                    alignment: "center",
                    width: 60,
                    allowFiltering: true,
                    allowSorting: true,
                },
                {
                    dataField: "CLIENTE",
                    alignment: "center",
                    width: 98,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "CODIGO",
                    alignment: "center",
                    width: 98,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "CODIGOE",
                    alignment: "center",
                    width: 98,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "PRODUCTO",
                    alignment: "center",
                    width: 150,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "ENE",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: true,
                },
                {
                    dataField: "FEB",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "MAR",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "ABR",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "MAY",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "JUN",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "JUL",
                    alignment: "right",
                    width: 98,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "AGO",
                    alignment: "right",
                    width: 98,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "SEP",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "OCT",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "NOV",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "DIC",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "IDVENDEDOR",
                    alignment: "left",
                    width: 98,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "TIPODOCTO",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,
                }];




            $(function () {
                var dataGrid = $("#gridContainer").dxDataGrid({
                    "export": {
                        enabled: true,
                        fileName: "Estadisitca Venta Producto x Cliente",
                        allowExportSelectedData: true
                    },
                    dataSource: cData,
                    rowAlternationEnabled: true,
                    allowColumnReordering: true,
                    allowColumnResizing: true,
                    columnHidingEnabled: true,
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
                    value: false,
                    text: "Expandir",
                    onValueChanged: function (data) {
                        dataGrid.option("grouping.autoExpandAll", data.value);
                    }
                });

            });

            $('#mostrar_consulta').html('');
        }
    } else {
        alert("Debe seleccionar un rango de fechas");
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