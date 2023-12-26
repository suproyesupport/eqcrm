$(document).ready(InicionEventos);



function InicionEventos() {
    ActFiltroTickets();
}



function AbrirTicket(e) {
    let id = $(e).closest("tr")[0].cells[1].innerText;
    let idcategoria = $(e).closest("tr")[0].cells[5].innerText;
    let problema = $(e).closest("tr")[0].cells[6].innerText;
    let reporta = $(e).closest("tr")[0].cells[8].innerText;
    let descripcion = $(e).closest("tr")[0].cells[11].innerText;

    $("#idcategoria").val(idcategoria);
    $("#problema").val(problema);
    $("#reporta").val(reporta);
    $("#descripcion").val(descripcion);

    $('#ModalTicketConsulta').modal('show');
}



function ActFiltroTickets() {


    //var fecha1 = document.getElementById("fecha1").value;
    //var fecha2 = document.getElementById("fecha2").value;

    //var linea = document.getElementById("linea").value;

    var fecha1 = "";
    var fecha2 = "";
    var linea = "";

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/TicketsFinalizados/GetTicketsFinalizados",
        data: { fecha1, fecha2, linea },
        beforeSend: InicioConsulta,
        success: ConsultaCotizaciones
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando Tickets por favor espere...');
    }

    function ConsultaCotizaciones(data) {
        var arreglo = eval(data);
        cData = arreglo;

        var columnas = [
            {
                dataField: "OTROS",
                alignment: "CENTER",
                width: 70,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.append($("<i style='color:#651FFF;cursor:pointer;' class='abrir' onclick='AbrirTicket(this)'> <img src='/images/Load.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);

                },

            },
            {
                dataField: "ID",
                alignment: "center",
                width: 60,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "CLIENTE",
                alignment: "center",
                width: 200,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "CATEGORIA",
                alignment: "center",
                width: 160,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "PROBLEMA",
                alignment: "center",
                width: 160,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "FECHAA",
                alignment: "center",
                width: 90,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "FECHAI",
                alignment: "center",
                width: 90,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "FECHAF",
                alignment: "center",
                width: 90,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "ESTADO",
                alignment: "CENTER",
                width: 100,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "DIAS",
                alignment: "center",
                width: 50,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "REPORTA",
                alignment: "CENTER",
                width: 200,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "DESCRIPCION",
                alignment: "LEFT",
                width: 500,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "SOLUCION",
                alignment: "LEFT",
                width: 500,
                allowFiltering: false,
                allowSorting: false,
            }];




        $(function () {
            var dataGrid = $("#gridContainer").dxDataGrid({

                dataSource: cData,
                keyExpr: "ID",
                selection: {
                    mode: "single"
                },
                "export": {
                    enabled: true,
                    fileName: "ConsultaTickets",
                    allowExportSelectedData: true
                },
                rowAlternationEnabled: true,
                allowColumnReordering: true,
                allowColumnResizing: true,
                //columnHidingEnabled: true,
                focusedRowEnabled: true,
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



function FiltrarxFecha() {

    var fecha1 = "";
    var fecha2 = "";

    var fecha1 = document.getElementById("fecha1").value;
    var fecha2 = document.getElementById("fecha2").value;

    
    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/TicketsFinalizados/GetFiltrarxFecha",
        data: { fecha1, fecha2 },
        beforeSend: InicioConsulta,
        success: ConsultaCotizaciones
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando Tickets por favor espere...');
    }

    function ConsultaCotizaciones(data) {
        var arreglo = eval(data);
        cData = arreglo;

        var columnas = [
            {
                dataField: "OTROS",
                alignment: "CENTER",
                width: 70,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                container.append($("<i style='color:#651FFF;cursor:pointer;' class='abrir' onclick='AbrirTicket(this)'> <img src='/images/Load.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);

             },
            },
            {
                dataField: "ID",
                alignment: "center",
                width: 60,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "CLIENTE",
                alignment: "center",
                width: 200,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "CATEGORIA",
                alignment: "center",
                width: 160,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "PROBLEMA",
                alignment: "center",
                width: 160,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "FECHAA",
                alignment: "center",
                width: 90,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "FECHAI",
                alignment: "center",
                width: 90,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "FECHAF",
                alignment: "center",
                width: 90,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "ESTADO",
                alignment: "CENTER",
                width: 100,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "DIAS",
                alignment: "center",
                width: 50,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "REPORTA",
                alignment: "CENTER",
                width: 200,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "DESCRIPCION",
                alignment: "LEFT",
                width: 500,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "SOLUCION",
                alignment: "LEFT",
                width: 500,
                allowFiltering: false,
                allowSorting: false,
            }];




        $(function () {
            var dataGrid = $("#gridContainer").dxDataGrid({

                dataSource: cData,
                keyExpr: "ID",
                selection: {
                    mode: "single"
                },
                "export": {
                    enabled: true,
                    fileName: "ConsultaTickets",
                    allowExportSelectedData: true
                },
                rowAlternationEnabled: true,
                allowColumnReordering: true,
                allowColumnResizing: true,
                //columnHidingEnabled: true,
                focusedRowEnabled: true,
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


