$(document).ready(InicionEventos);

function InicionEventos() {
    ActFiltroProblemaTickets();
}


function GuardarProblemaTicket() {

    var problema = document.getElementById("problema").value.toUpperCase();
    
    $.ajax({
        async: true,
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoProblemaTicket/InsertarProblemaTicket",
        data: { problema },
        success: function (response) {
            var arreglo = response;

            if (response.CODIGO == "ERROR") {
                if (response.PRODUCTO == "") {
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

    window.location.href = "/IngresoProblemaTicket/IngresoProblemaTicket";
}


function ActFiltroProblemaTickets() {

    var fecha1 = "";
    var fecha2 = "";
    var linea = "";

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoProblemaTicket/GetProblemasTicket",
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
                alignment: "center",
                width: 80,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.append($("<i style='color:red; cursor:pointer;' class='fas fa-trash' onclick='EliminarTicket(this)'></i>")).appendTo(container);
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
                dataField: "PROBLEMA",
                alignment: "LEFT",
                width: 1200,
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
                    fileName: "ConsultaProblemaTickets",
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


function EliminarTicket(e) {

    let id = $(e).closest("tr")[0].cells[1].innerText;
  
    $.ajax({
        async: true,
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoProblemaTicket/EliminarProblemaTicket",
        data: { id },
        success: function (response) {
            var arreglo = response;

            if (response.CODIGO == "ERROR") {
                if (response.PRODUCTO == "") {
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

    window.location.href = "/IngresoProblemaTicket/IngresoProblemaTicket";
}


function FiltroxProblema() {

    
}