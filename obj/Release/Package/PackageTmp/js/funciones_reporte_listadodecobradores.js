$(document).ready(InicionEventos);

function InicionEventos() {

    GenerarReporteListaDeCobradores();
    //$('#gridContainer').on('click', '.UUID', BuscarFactura);
    
}

function GenerarReporteListaDeCobradores() {

        $.ajax({
            async: false,
            type: "POST",
            dataType: "HTML", 
            contentType: "application/x-www-form-urlencoded",
            url: "/ReporteListadoDeCobradoresC/GenerarReporteListaDeCobradores",
            data: { },
            beforeSend: InicioConsulta,
            success: ConsultaListadoCobradores
        });

        function InicioConsulta() {
            $('#mostrar_consulta').html('Cargando por favor espere...');
        }

        function ConsultaListadoCobradores(data) {
            var arreglo = eval(data);
            cData = arreglo;

            console.log(cData);

            var columnas = [
                {
                    dataField: "CODIGO",
                    alignment: "CENTER",
                    width: 75,
                    allowFiltering: true,
                    allowSorting: true,
                },
                {
                    dataField: "CLIENTE",
                    alignment: "LEFT",
                    width: 300,
                    allowFiltering: true,
                    allowSorting: true,
                },
                {
                    dataField: "FECHAI",
                    alignment: "CENTER",
                    width: 90,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "RUTA",
                    alignment: "CENTER",
                    width: 200,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "DIRECCION",
                    alignment: "LEFT",
                    width: 300,
                    allowFiltering: true,
                    allowSorting: false,
                },
                {
                    dataField: "TELEFONO",
                    alignment: "CENTER",
                    width: 90,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "FECHA",
                    alignment: "CENTER",
                    width: 90,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "MES",
                    alignment: "CENTER",
                    width: 50,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "MESACOBRAR",
                    alignment: "LEFT",
                    width: 200,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "IMPORTE",
                    alignment: "CENTER",
                    width: 80,
                    allowFiltering: false,
                    allowSorting: false,
                },
                {
                    dataField: "SALDO",
                    alignment: "CENTER",
                    width: 80,
                    allowFiltering: false,
                    allowSorting: false,
                }];

            $(function () {
                var dataGrid = $("#gridContainer").dxDataGrid({
                    "export": {
                        enabled: true,
                        fileName: "Listado Para Cobradores",
                        allowExportSelectedData: true
                    },
                    dataSource: cData,
                    rowAlternationEnabled: true,
                    allowColumnReordering: true,
                    allowColumnResizing: true,
                    /*columnHidingEnabled: true,*/
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