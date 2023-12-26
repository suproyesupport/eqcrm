$(document).ready(InicionEventos);

function InicionEventos() {

    $('#gridContainer').on('click', '.UUID', BuscarFactura);

}


function BuscarSabanaFacturas() {

    var fecha1 = document.getElementById("fecha1").value;
    var fecha2 = document.getElementById("fecha2").value;
    //var comboagencia = document.getElementById("agencia");
    //var agencia = comboagencia.options[comboagencia.selectedIndex].value;

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/ReporteSabanaFact/GenerarReporteSabanaFact",
        data: { fecha1, fecha2 },
        beforeSend: InicioConsulta,
        success: ConsultaRPOSabanaFacturas
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ConsultaRPOSabanaFacturas(data) {
        var arreglo = eval(data);
        cData = arreglo;

        console.log(cData);

        var columnas = [
            {
                dataField: "FECHA",
                alignment: "center",
                width: 60,
                allowFiltering: true,
                allowSorting: true,
            },
            {
                dataField: "NO_FACTURA",
                alignment: "center",
                width: 98,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "SERIE",
                alignment: "left",
                width: 200,
                allowFiltering: true,
                allowSorting: true,
            },
            {
                dataField: "VENDEDOR",
                alignment: "left",
                width: 300,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "CODIGO",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "CODIGOE",
                alignment: "left",
                width: 140,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "PRODUCTO",
                alignment: "left",
                width: 140,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "LINEA",
                alignment: "left",
                width: 140,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "PRECIO",
                alignment: "right",
                width: 300,
                allowFiltering: false,
                allowSorting: false,
                format: {
                    type: "fixedPoint",
                    precision: 2
                }
            },
            {
                dataField: "SUBTOTAL",
                alignment: "right",
                width: 300,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "UCOSTO",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "COSTOP",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "CANTIDAD",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "CODCLIE",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "CLIENTE",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "TELEFONO",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "FAX",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "EMAIL",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "DEPTO",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "MUNI",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            }];




        $(function () {
            var dataGrid = $("#gridContainer").dxDataGrid({
                "export": {
                    enabled: true,
                    fileName: "Sabana",
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




























































////$(document).ready(InicionEventos);

////function InicionEventos() {

////    LlenarTablaReporteGeneralFacturas();
////}

////function LlenarTablaReporteGeneralFacturas() {
////    $.ajax({
////        async: false,
////        type: "POST",
////        dataType: "HTML",
////        contentType: "application/x-www-form-urlencoded",
////        url: "/ReporteGeneralFacturas/GenerarReporteGenFacturas",
////        data: {},
////        beforeSend: InicioConsulta,
////        success: ConsultaDocumentos
////    });

////    function InicioConsulta() {
////        $('#mostrar_consulta').html('Cargando por favor espere...');
////    }

////    function ConsultaDocumentos(data) {
////        var arreglo = eval(data);
////        cData = arreglo;

////        var columnas = [
////            //{
////            //    dataField: "OTROS",
////            //    alignment: "right",
////            //    width: 70,
////            //    allowFiltering: false,
////            //    allowSorting: false,
////            //    cellTemplate: function (container, options) {
////            //        /*container.append($("<i style='color:#651FFF;cursor:pointer;' class='.VER_ORD' onclick='BuscarFichaOrdenServicio(this)'> <img src='/images/Load.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);*/
////            //        container.append($("<i style='color:#651FFF;cursor:pointer;' class='.VER_ORD'> <img src='/images/Load.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);
////            //    }
////            //},

////            {
////                dataField: "SERIE",
////                alignment: "center",
////                width: 60,
////                allowFiltering: true,
////                allowSorting: true,
////            },
////            {
////                dataField: "FACTURA",
////                alignment: "center",
////                width: 98,
////                allowFiltering: false,
////                allowSorting: false,
////            },
////            {
////                dataField: "FECHA",
////                alignment: "left",
////                width: 200,
////                allowFiltering: true,
////                allowSorting: true,
////            },
////            {
////                dataField: "CLIENTE",
////                alignment: "left",
////                width: 300,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "STATUS",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "CONTADO",
////                alignment: "left",
////                width: 140,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "CREDITO",
////                alignment: "left",
////                width: 140,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "TDESCTO",
////                alignment: "left",
////                width: 140,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "TOTAL",
////                alignment: "right",
////                width: 300,
////                allowFiltering: false,
////                allowSorting: false,
////            },
////            {
////                dataField: "ID_VENDEDOR",
////                alignment: "right",
////                width: 300,
////                allowFiltering: false,
////                allowSorting: false,
////            },
////            {
////                dataField: "ID_CLIENTE",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "NIT",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "INGUAT",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "ID_RESERVA",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "NO_NOTA",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "NO_DOCTO_FEL",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "SERIE_DOCTO_FEL",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "FIRMAELECTRONICA",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            }{
////                dataField: "SERIE",
////                alignment: "center",
////                width: 60,
////                allowFiltering: true,
////                allowSorting: true,
////            },
////            {
////                dataField: "FACTURA",
////                alignment: "center",
////                width: 98,
////                allowFiltering: false,
////                allowSorting: false,
////            },
////            {
////                dataField: "FECHA",
////                alignment: "left",
////                width: 200,
////                allowFiltering: true,
////                allowSorting: true,
////            },
////            {
////                dataField: "CLIENTE",
////                alignment: "left",
////                width: 300,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "STATUS",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "CONTADO",
////                alignment: "left",
////                width: 140,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "CREDITO",
////                alignment: "left",
////                width: 140,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "TDESCTO",
////                alignment: "left",
////                width: 140,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "TOTAL",
////                alignment: "right",
////                width: 300,
////                allowFiltering: false,
////                allowSorting: false,
////            },
////            {
////                dataField: "ID_VENDEDOR",
////                alignment: "right",
////                width: 300,
////                allowFiltering: false,
////                allowSorting: false,
////            },
////            {
////                dataField: "ID_CLIENTE",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "NIT",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "INGUAT",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "ID_RESERVA",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "NO_NOTA",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "NO_DOCTO_FEL",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "SERIE_DOCTO_FEL",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            },
////            {
////                dataField: "FIRMAELECTRONICA",
////                alignment: "left",
////                width: 150,
////                allowFiltering: true,
////                allowSorting: false,
////            }];

////        $(function () {
////            var dataGrid = $("#gridContainer").dxDataGrid({
////                dataSource: cData,
////                keyExpr: "FACTURA",
////                selection: {
////                    mode: "single"
////                },
////                "export": {
////                    enabled: true,
////                    fileName: "Reporte",
////                    allowExportSelectedData: true
////                },
////                rowAlternationEnabled: true,
////                allowColumnReordering: true,
////                allowColumnResizing: true,
////                //columnHidingEnabled: true,
////                focusedRowEnabled: true,
////                showBorders: true,
////                columnChooser: {
////                    enabled: true
////                },
////                columnFixing: {
////                    enabled: true
////                },
////                grouping: {
////                    autoExpandAll: false,
////                },
////                searchPanel: {
////                    visible: true,
////                    width: 240,
////                    placeholder: "Search..."
////                },
////                headerFilter: {
////                    visible: true
////                },
////                paging: {
////                    pageSize: 25
////                },
////                groupPanel: {
////                    visible: true
////                },
////                filterRow: {
////                    visible: true,
////                },
////                pager: {
////                    showPageSizeSelector: true,
////                    showInfo: true
////                },
////                columns: columnas,
////                //onSelectionChanged: function (selectedItems) {
////                //    var data = selectedItems.selectedRowsData[0];
////                //    if (data) {
////                //        //$(".Obs").text(data.SERIEINTERNA);
////                //        alert(data.ID);

////                //}
////                //}
////            }).dxDataGrid("instance");

////            $("#autoExpand").dxCheckBox({
////                value: false,
////                text: "Expandir",
////                onValueChanged: function (data) {
////                    dataGrid.option("grouping.autoExpandAll", data.value);
////                }
////            });
////        });
////        $('#mostrar_consulta').html('');
////    }
////}