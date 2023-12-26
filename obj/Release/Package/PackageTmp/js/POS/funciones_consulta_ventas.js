//$(document).ready(InicionEventos);

//function InicionEventos() {

//    $('#gridContainer').on('click', '.UUID', BuscarFactura);
    
//}

function BuscarVentasCoca() {

    let fecha1 = document.getElementById("fecha1").value;
    let fecha2 = document.getElementById("fecha2").value;

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Consultas/GetVentas",
        data: { fecha1, fecha2 },
        beforeSend: InicioConsulta,
        success: ResultadoConsulta
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ResultadoConsulta(data) {

        //Para vaciar lo que tiene la tabla
        document.querySelector("#gridContainer").innerHTML = "";

        const arreglo = eval(data);
        cData = arreglo;

        var gridDataSource = new kendo.data.DataSource({
            data: cData,
            schema: {
                model: {
                    fields: {
                        IDINTERNO: { type: "number" },
                        SERIEINTERNA: { type: "string" },
                        UUID: { type: "string" },
                        NO_DOCTO: { type: "string" },
                        SERIE: { type: "string" },
                        FECHA: { type: "string" },
                        STATUS: { type: "string" },
                        NIT: { type: "string" },
                        IDCLIENTE: { type: "string" },
                        CLIENTE: { type: "string" },
                        ASESOR: { type: "string" },
                        AGENCIA: { type: "string" },
                        OBS: { type: "string" },
                    }
                }
            },
            height: 550,

            pageSize: 12,
            //sort: {
            //    field: "IDINTERNO",
            //    dir: "desc"
            //}
        });

        $("#gridContainer").kendoGrid({
            toolbar: ["excel", "pdf", "search"],
            excel: {
                fileName: "EqWebReportList Facturas.xlsx",
                filterable: true
            },
            pdf: {
                allPages: true,
                avoidLinks: true,
                paperSize: "letter",
                margin: { top: "2cm", left: "1cm", right: "1cm", bottom: "1cm" },
                landscape: true,
                repeatHeaders: true,
                template: $("#page-template").html(),
                scale: 0.8
            },
            search: {
                fields: [
                    { name: "IDINTERNO", operator: "eq" },
                    { name: "SERIEINTERNA", operator: "eq" },
                    { name: "UUDI", operator: "eq" },
                    { name: "NO_DOCTO", operator: "eq" },
                    { name: "SERIE", operator: "eq" },
                    { name: "NIT", operator: "eq" },
                    { name: "CLIENTE", contains: "eq" },
                    //{ name: "Freight", operator: "gte" },
                    //{ name: "ShipName", operator: "contains" },
                    //{ name: "ShipCity", operator: "contains" },
                ]
            },
            dataSource: gridDataSource,
            height: 1000,
            groupable: true,
            sortable: true,
            pageable: {
                refresh: true,
                pageSizes: true,
                buttonCount: 5
            },
            filterable: true,
            columns: [
                {
                    field: "OTROS",
                    title: "Otros",
                    width: 75,
                    template: "<table class='table table-bordered m-0'> <tr><td> " +
                        "<a data-toggle='modal' style='cursor:pointer;' class='UUID' value='#=(UUID)#' onclick='BuscarFactura(\"#=(UUID)#\");' ><img src='/images/folder.png' width='35' height='35'></a> " +
                        "</td></tr></table>",
                    stickable: true,
                    locked: true,
                    lockable: false,
                }, {

                    field: "IDINTERNO",
                    title: "No. Factura",
                    width: 130,
                    stickable: true,

                }, {
                    field: "SERIEINTERNA",
                    title: "Serie Interna",
                    width: 130,
                    stickable: true,
                }, {
                    field: "UUID",
                    title: "Firma Electronica",
                    width: 200,
                    stickable: true,
                    locked: true,

                }, {
                    field: "NO_DOCTO",
                    title: "No. Docto.",
                    width: 160, stickable: true, lockable: false,
                }, {
                    field: "SERIE",
                    title: "Serie",
                    width: 160, stickable: true,
                }, {
                    field: "FECHA",
                    title: "Fecha",
                    width: 130,
                    /*format: "0:{MM/dd/yyyy}",*/
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },
                    stickable: true,

                }, {
                    field: "STATUS",
                    title: "Status",
                    width: 160, stickable: true,
                }, {
                    field: "NIT",
                    title: "Nit",
                    width: 160, stickable: true,
                }, {
                    field: "IDCLIENTE",
                    title: "ID Cliente",
                    width: 160, stickable: true,
                }, {
                    field: "CLIENTE",
                    title: "Cliente",
                    width: 160, stickable: true,
                }, {
                    field: "ASESOR",
                    title: "Asesor",
                    width: 160, stickable: true,
                }, {
                    field: "TOTAL",
                    title: "Total",
                    width: 160, stickable: true,
                }, {
                    field: "AGENCIA",
                    title: "Agencia",
                    width: 160, stickable: true,
                }, {
                    field: "OBS",
                    title: "Observacion",
                    width: 160, stickable: true,
                }
            ]
        });

        $('#mostrar_consulta').html('');
    }
}


function BuscarFactura(cUUid) {    

    var clink = "";
    //var cUUid = $(this).attr('id');

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",        
        url: "/GetDataFel/GetDataFel",
        data: { cUUid },
        success: function (response) {
            //alert(response);
            //downloadPDF(response,cUUid);
            //clink = clink + response + ".pdf";
            clink = clink + response ;
            window.open(clink, "_blank");
           // PrintJS(clink);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
  
}


//function BuscarFactura() {
//    var cUUid = $(this).attr('id');    

//    var ua = window.navigator.userAgent;
//    var msie = ua.indexOf('MSIE ');
//    var trident = ua.indexOf('Trident/');
//    var Edge = ua.indexOf('Edge/');
//    var url = "/Rgp/";
//    var pdf = '';
//    var style = 'position:fixed; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%; border:none; margin:0; padding:0; overflow:hidden;';

//    $.ajax({
//        async: false,
//        type: "POST",
//        dataType: "text",
//        contentType: "application/x-www-form-urlencoded",
//        url: "/GetDataFel/GetDataFEL",
//        data: { cUUid },
//        success: function (response) {
//            //alert(response);
//            //downloadPDF(response,cUUid);
//            url = url + response + ".pdf";
//            //window.open(clink, "_blank");
//            //PrintJS(clink);
//            //alert(url);
//        },
//        error: function () {
//            alert("Ocurrio un Error");
//        }
//    });



//    if (msie > 0 || trident > 0 || Edge > 0) {
//        pdf = '<object data="' + url + '" name="print_frame" id="print_frame" style="' + style + '" type="application/pdf">';
//    }
//    else {
//        pdf = '<iframe src="' + url + '" name="print_frame" id="print_frame" style="' + style + '"></iframe>';
//    }

//    //alert(pdf);

//    $(document.body).append(pdf);

//    $("#print_frame").hide();
//    setTimeout(function () {
//        window.frames["print_frame"].focus();
//        window.frames["print_frame"].print();
//    }, 2000);
//}

function downloadPDF(pdf,cUUid) {
    const linkSource = `data:application/pdf;base64,${pdf}`;
    const downloadLink = document.createElement("a");
    const fileName = cUUid+".pdf";
    downloadLink.href = linkSource;
    downloadLink.download = fileName;
    downloadLink.click();
    //var pdfUrl = URL.createObjectURL(fileName);
    //alert(linkSource);
    //window.open(linkSource, "_blank");
    //printJS(pdfUrl);
}

function BuscarVentas() {


    var fecha1 = document.getElementById("fecha1").value;
    var fecha2 = document.getElementById("fecha2").value;


    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Consultas/GetVentas",
        data: { fecha1, fecha2 },
        beforeSend: InicioConsulta,
        success: ConsultaCotizaciones
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ConsultaCotizaciones(data) {
        var arreglo = eval(data);
        cData = arreglo;

        console.log(cData);

        var columnas = [
            {
                dataField: "OTROS",
                alignment: "CENTER",
                width: 75,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.append($("<table class=\"table table-bordered m-0\"><tr><td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_FACTURAS' class='UUID' id='" + options.data.UUID + "'><img src='/images/folder.png' width='35' height='35'></a></td>" + "</tr></table >")).appendTo(container);

                }
            },
            {
                dataField: "UUID",
                alignment: "left",
                width: 300,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "IDINTERNO",
                alignment: "left",
                width: 90,
                allowFiltering: true,
                allowSorting: false,

            },
                   
            {
                dataField: "SERIEINTERNA",
                alignment: "left",
                width: 100,
                allowFiltering: true,
                allowSorting: false,

            },           

            {
                dataField: "NO_DOCTO",
                alignment: "left",
                width: 100,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "SERIE",
                alignment: "left",
                width: 100,
                allowFiltering: false,
                allowSorting: false,

            },
            {
                dataField: "FECHA",
                alignment: "left",
                width: 100,
                allowFiltering: false,
                allowSorting: false,

            },

            {
                dataField: "STATUS",
                alignment: "left",
                width: 80,
                allowFiltering: false,
                allowSorting: false,

            },
            {
                dataField: "NIT",
                alignment: "left",
                width: 100,
                allowFiltering: false,
                allowSorting: false,

            },
            {
                dataField: "IDCLIENTE",
                alignment: "right",
                width: 100,
                allowFiltering: false,
                allowSorting: false,

            },
            {
                dataField: "CLIENTE",
                alignment: "left",
                //width: 400,
                allowFiltering: true,
                allowSorting: false,
                selectedFilterOperation: "contains",

            },
            {
                dataField: "ASESOR",
                alignment: "right",
                // width: 80,
                allowFiltering: false,
                allowSorting: false,

            },

            {
                dataField: "TOTAL",
                alignment: "right",
                //width: 150,
                allowFiltering: false,
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