function openCompras(e) {
    let id = $(e).closest("tr")[0].cells[1].innerText + "|" + $(e).closest("tr")[0].cells[2].innerText;
//    window.open("/ConsultarCotizaciones/VerCotizacion/"+id,"_blank")
    $('.modal-body').load("/ConsultaCompras/VerCompra/" + id, function () {
        //$('#consultar').modal({ show: true });
        $('#consultar').modal("show");
    });
}


function ActFiltroDoctos() {


    var fecha1 = document.getElementById("fecha1").value;
    var fecha2 = document.getElementById("fecha2").value;
    

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/ConsultaCompras/GetCompras",
        data: { fecha1, fecha2 },
        beforeSend: InicioConsulta,
        success: ConsultaCompras
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando Compras por favor espere...');
    }

    function ConsultaCompras(data) {
        var arreglo = eval(data);
        cData = arreglo;

        var columnas = [
            {
                dataField: "OTROS",
                alignment: "right",
                width: 60,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.append($("<i style='color:#651FFF;cursor:pointer;' class='COMPRAS' onclick='openCompras(this)'> <img src='/images/pdf_download.png' width=\"18\" height=\"auto\"></i>")).appendTo(container);

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
                dataField: "FECHA",
                alignment: "center",
                width: 150,
                allowFiltering: true,
                allowSorting: false,

            },

            {
                dataField: "PROVEEDOR",
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
                    fileName: "ConsultaCompras",
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