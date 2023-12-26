var ndocto;
var cserie;

function openCotizacion(e) {
    let id = $(e).closest("tr")[0].cells[1].innerText + "|" + $(e).closest("tr")[0].cells[2].innerText;
    window.open("/ConsultarCotizaciones/VerCotizacion/" + id, "_blank")
}



function ventaganada(e) {
    let id = $(e).closest("tr")[0].cells[1].innerText + "|" + $(e).closest("tr")[0].cells[2].innerText;

    ndocto = $(e).closest("tr")[0].cells[1].innerText;
    cserie = $(e).closest("tr")[0].cells[2].innerText;
    $('#modal-ganada').modal('show');


}


function ventaperdida(e) {
    let id = $(e).closest("tr")[0].cells[1].innerText + "|" + $(e).closest("tr")[0].cells[2].innerText;
    
    ndocto = $(e).closest("tr")[0].cells[1].innerText;
    cserie = $(e).closest("tr")[0].cells[2].innerText;
    $('#modal-perdida').modal('show');
    
}


function Actganada() {
    $('#modal-ganada').modal('hide');
    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/SeguimientoCotizaciones/Ganada",
        data: { ndocto,cserie },
        beforeSend: InicioActualizacion,
        success: Satisfactorio
    });

    function InicioActualizacion() {
        $('#mostrar_consulta').html('Cargando Cotizaciones por favor espere...');
    }

    function Satisfactorio() {
        ActFiltroDoctos();
    }

}

function ActPerdida() {
    $('#modal-perdida').modal('hide');
    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/SeguimientoCotizaciones/Perdida",
        data: { ndocto, cserie },
        beforeSend: InicioActualizacion,
        success: Satisfactorio
    });

    function InicioActualizacion() {
        $('#mostrar_consulta').html('Cargando Cotizaciones por favor espere...');
    }

    function Satisfactorio() {
        ActFiltroDoctos();
    }

}


function ActFiltroDoctos() {


    var fecha1 = document.getElementById("fecha1").value;
    var fecha2 = document.getElementById("fecha2").value;

    var linea = document.getElementById("linea").value;
   

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/SeguimientoCotizaciones/GetCotizaciones",
        data: { fecha1, fecha2, linea },
        beforeSend: InicioConsulta,
        success: ConsultaCotizaciones
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando Cotizaciones por favor espere...');
    }

    function ConsultaCotizaciones(data) {
        var arreglo = eval(data);
        cData = arreglo;

        var columnas = [
            {
                dataField: "OTROS",
                alignment: "right",
                width: 80,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.append($("<i style='color:#651FFF;cursor:pointer;' class='vganada' onclick='ventaganada(this)'> <img src='/images/ventaganada.png' width=\"18\" height=\"auto\"></i><i style='color:#651FFF;cursor:pointer;' class='vperdida' onclick='ventaperdida(this)'> <img src='/images/ventaperdida.png' width=\"18\" height=\"auto\"></i><i style='color:#651FFF;cursor:pointer;' class='INV' onclick='openCotizacion(this)'> <img src='/images/pdf_download.png' width=\"18\" height=\"auto\"></i>"   )).appendTo(container);

                },

            },
            {
                dataField: "NO_FACTURA",
                alignment: "center",
                width: 100,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "SERIE",
                alignment: "center",
                width: 90,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "STATUS",
                alignment: "centar",
                width: 50,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "FECHA",
                alignment: "center",
                width: 150,
                allowFiltering: true,
                allowSorting: false,

            },

            {
                dataField: "CLIENTE",
                alignment: "center",
                width: 450,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "NIT",
                alignment: "center",
                width: 100,
                allowFiltering: false,
                allowSorting: false,

            },
            {
                dataField: "NOMBRE",
                alignment: "center",
                width: 100,
                allowFiltering: false,
                allowSorting: false,

            },

            {
                dataField: "DIRECCION",
                alignment: "center",
                width: 200,
                allowFiltering: false,
                allowSorting: false,

            },
            {
                dataField: "TOTAL",
                alignment: "center",
                width: 100,
                allowFiltering: false,
                allowSorting: false,

            },
            {
                dataField: "TDESCTO",
                alignment: "center",
                width: 100,
                allowFiltering: false,
                allowSorting: false,

            },
            {
                dataField: "OBS",
                alignment: "center",
                width: 335,
                allowFiltering: false,
                allowSorting: false,

            }];




        $(function () {
            var dataGrid = $("#gridContainer").dxDataGrid({

                dataSource: cData,
                keyExpr: "NO_FACTURA",
                selection: {
                    mode: "single"
                },
                "export": {
                    enabled: true,
                    fileName: "ConsultaCotizaciones",
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