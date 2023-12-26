$(document).ready(InicionEventos);

function InicionEventos() {

    $('#gridContainer').on('click', '.UUID', BuscarFactura);

}


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
            success: ConsultaRPOGenFacturas
        });

        function InicioConsulta() {
            $('#mostrar_consulta').html('Cargando por favor espere...');
        }

        function ConsultaRPOGenFacturas(data) {
            var arreglo = eval(data);
            cData = arreglo;

            console.log(cData);

            var columnas = [
                {
                    dataField: "ID_INTERNO",
                    alignment: "center",
                    width: 75,
                    allowFiltering: true,
                    allowSorting: true,
                },
                {
                    dataField: "SERIE_INTERNA",
                    alignment: "center",
                    width: 98,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "FECHA",
                    alignment: "CENTER",
                    width: 100,
                    allowFiltering: true,
                    allowSorting: true,
                    dataType: 'date',
                    format: 'yyyy-MM-dd',
                },
                {
                    dataField: "NIT",
                    alignment: "LEFT",
                    width: 125,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "CLIENTE",
                    alignment: "LEFT",
                    width: 200,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "STATUS",
                    alignment: "CENTER",
                    width: 50,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "VENDEDOR",
                    alignment: "left",
                    width: 75,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "TOTAL",
                    alignment: "LEFT",
                    width: 125,
                    allowFiltering: true,
                    allowSorting: false,
                    dataType: 'number',
                    format: '#,##0.00',
                },
                {
                    dataField: "AGENCIA",
                    alignment: "left",
                    width: 50,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "SALDO",
                    alignment: "right",
                    width: 125,
                    allowFiltering: false,
                    allowSorting: false,
                    dataType: 'number',
                    format: '#,##0.00',
                },
                {
                    dataField: "CONDICION",
                    alignment: "right",
                    width: 75,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "NOAUTORIZACION",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "NO_DOCTO",
                    alignment: "left",
                    width: 125,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "SERIE_FEL",
                    alignment: "left",
                    width: 125,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "NOACCESO",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "MAIL",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "RECIBOPAGO",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "FECHAAPLICACION",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "BOLETA",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "BANCO",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "CONTRASENA",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,
                }];




            $(function () {
                var dataGrid = $("#gridContainer").dxDataGrid({
                    "export": {
                        enabled: true,
                        fileName: "Consulta de Facturas",
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
                        width: 25,
                        placeholder: "Search..."
                    },
                    //headerFilter: {
                    //    visible: true
                    //},
                    paging: {
                        pageSize: 25
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