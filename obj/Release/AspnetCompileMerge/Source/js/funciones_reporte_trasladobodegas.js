$(document).ready(InicionEventos);



function InicionEventos() {

    $('#gridContainer').on('click', '.UUID', BuscarFactura);

}



function GenerarReporte() {

    if (verificarCampos() != 'false') {
        var fecha1 = document.getElementById("fecha1").value;
        var fecha2 = document.getElementById("fecha2").value;

        $.ajax({
            async: false,
            type: "POST",
            dataType: "HTML",
            contentType: "application/x-www-form-urlencoded",
            url: "/ReporteTrasladoBodegas/GenerarReporte",
            data: { fecha1, fecha2 },
            beforeSend: InicioConsulta,
            success: ConsultaRPOTrasEntreBodegas
        });

        function InicioConsulta() {
            $('#mostrar_consulta').html('Cargando por favor espere...');
        }

        function ConsultaRPOTrasEntreBodegas(data) {
            var arreglo = eval(data);
            cData = arreglo;

            console.log(cData);

            var columnas = [
                {
                    dataField: "NO_TRASLADO",
                    alignment: "center",
                    width: 125,
                    allowFiltering: true,
                    allowSorting: true,
                },
                {
                    dataField: "SERIE",
                    alignment: "center",
                    width: 75,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "STATUS",
                    alignment: "CENTER",
                    width: 100,
                    allowFiltering: true,
                    allowSorting: true,
                    dataType: 'date',
                    format: 'yyyy-MM-dd',
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
                    dataField: "AGENCIA_TRASLADO",
                    alignment: "CENTER",
                    width: 125,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "AGENCIA_RECIBE",
                    alignment: "CENTER",
                    width: 125,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "NOMBRE",
                    alignment: "left",
                    width: 200,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "OBSERVACIONES",
                    alignment: "LEFT",
                    width: 350,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "REALIZADO_POR",
                    alignment: "LEFT",
                    width: 100,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "ANULADO_POR",
                    alignment: "left",
                    width: 125,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "RECEPCIONADO_POR",
                    alignment: "left",
                    width: 125,
                    allowFiltering: true,
                    allowSorting: false,
                }];

            $(function () {
                var dataGrid = $("#gridContainer").dxDataGrid({
                    "export": {
                        enabled: true,
                        fileName: "Reporte Traslado entre Bodegas",
                        allowExportSelectedData: true
                    },
                    dataSource: cData,
                    rowAlternationEnabled: true,
                    allowColumnReordering: true,
                    allowColumnResizing: true,
                    columnHidingEnabled: true,
                    showBorders: false,
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
                        width: 250,
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


