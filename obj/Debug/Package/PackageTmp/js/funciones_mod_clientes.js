$(document).ready(InicionEventos);

function InicionEventos() {
    GenerarLista();
}

function GenerarLista() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModClientes/GenerarConsulta",
        data: {},
        beforeSend: InicioConsulta,
        success: Consulta
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function Consulta(data) {
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
                dataField: "NIT",
                alignment: "CENTER",
                width: 90,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "TELEFONO",
                alignment: "CENTER",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "CORREO",
                alignment: "LEFT",
                width: 200,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "DIRECCION",
                alignment: "LEFT",
                width: 300,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "OBSERVACION",
                alignment: "LEFT",
                width: 300,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "TIPO",
                alignment: "CENTER",
                width: 50,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "VENDEDOR",
                alignment: "CENTER",
                width: 150,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "ATENCION",
                alignment: "CENTER",
                width: 150,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "FECHA_INICIO",
                alignment: "CENTER",
                width: 150,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "DIAS_CREDITO",
                alignment: "CENTER",
                width: 80,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "LIM_CRED",
                alignment: "CENTER",
                width: 80,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "COMISION",
                alignment: "CENTER",
                width: 80,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "STATUS",
                alignment: "CENTER",
                width: 80,
                allowFiltering: false,
                allowSorting: false,
            }];

        $(function () {
            var dataGrid = $("#gridContainer").dxDataGrid({
                "export": {
                    enabled: true,
                    fileName: "Listado Clientes",
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