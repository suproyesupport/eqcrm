//$(document).ready(InicionEventos);

//function InicionEventos() {

//    $('#gridContainer').on('click', '.UUID', BuscarFactura);
//    $('#gridContainer').on('click', '.NUMAUT', AbrirAnulacion);
//    $('#gridContainer').on('click', '.NCDESC', AbrirNCDesc);
    
//}

function BuscarFactura(cUUid) {    

    let  clink = "";
    //var cUUid = $(this).attr('id');
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",        
        url: "/GetDataFel/GetDataFEL",
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


function AnularDocumento() {

    const label_motivoanula = document.getElementById("label_motivoanula");

    label_motivoanula.style.display = 'none';

    if (document.getElementById("motivoanula").value === "") {
        label_motivoanula.style.display = 'block';
        return;
    }

    var clink = "";
    var cUUid = document.getElementById("uuid").value + "|" + document.getElementById("motivoanula").value;
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",   
        contentType: "application/x-www-form-urlencoded",
        url: "/GetDataFel/GetAnulaFel",
        data: { cUUid },
        success: function (response) {
            //alert(response);
            //downloadPDF(response,cUUid);
            //clink = clink + response + ".pdf";
            clink = clink + response;
            window.open(clink, "_blank");
            // PrintJS(clink);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

}

function DocumentoDesc() {

    const label_fechaanula = document.getElementById("label_fechaanula");
    label_motivoanula.style.display = 'none';

    const label_abono = document.getElementById("label_abono");
    label_abono.style.display = 'none';

    if (document.getElementById("fecha").value === "") {
        label_fechaanula.style.display = 'block';
        return;
    }

    if (document.getElementById("abono").value === "") {
        label_abono.style.display = 'block';
        return;
    }

    var clink = "";
    var cUUid = document.getElementById("uuid2").value + "|" + document.getElementById("motivoanula2").value + "|" + document.getElementById("nofactura2").value + "|" + document.getElementById("serie2").value + "|" + document.getElementById("fecha").value + "|" + document.getElementById("abono").value;

    let total = parseFloat(document.getElementById("total").value);
    let abono = parseFloat(document.getElementById("abono").value);

    if (abono > total) {
        $("#mensaje").html("El abono no puede ser mayor al total");
        $('#modal-generico').modal('show');
        return;
    }
    
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/GetDataFel/GetNcDescFel",
        data: { cUUid },
        success: function (response) {
            //alert(response);
            //downloadPDF(response,cUUid);
            //clink = clink + response + ".pdf";
            //clink = clink + response;
            //window.open(clink, "_blank");
			alert("Documento Creado");
            // PrintJS(clink);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

}

//PENDIENTE
function AbrirAnulacion(cUUid) {
        
    //var cUUid = $(this).attr('id');
    var aUUid = cUUid.split('|');

    var uuid = aUUid[0];
    var nofactura = aUUid[1];
    var serie = aUUid[2];
    var cliente = aUUid[3];
    var total = aUUid[4];
    
    const label_motivoanula = document.getElementById("label_motivoanula");


    $("#uuid").val(uuid);
    $("#nofactura").val(nofactura);
    $("#serie").val(serie);
    $("#cliente").val(cliente);
    $("#total").val(total);
    
    label_motivoanula.style.display = 'none';
    $('#ModalAnulacion').modal('show');
}


function AbrirNCDesc(cUUid) {

    //var cUUid = $(this).attr('id');

    var aUUid = cUUid.split('|');

    var uuid = aUUid[0];
    var nofactura = aUUid[1];
    var serie = aUUid[2];
    var cliente = aUUid[3];
    var total = aUUid[4];
    var fecha = aUUid[5];

    const label_fechaanula = document.getElementById("label_fechaanula");
    const label_abono = document.getElementById("label_abono");
    

    $("#uuid2").val(uuid);
    $("#nofactura2").val(nofactura);
    $("#serie2").val(serie);
    $("#cliente2").val(cliente);
    $("#total2").val(total);
    $("#fecha").val(fecha);


    label_fechaanula.style.display = 'none';
    label_abono.style.display = 'none';
    $('#ModalNcDescuento').modal('show');
}


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



function BuscarDtes(){
    var clink = "";
    var cFechas = formatDate(document.getElementById("fecha1").value) + "|" + formatDate(document.getElementById("fecha2").value);
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/GetDataFel/BusquedaDtes",
        data: { cFechas },
        success: function (response) {
            //alert(response);
            //downloadPDF(response,cUUid);
            //clink = clink + response + ".pdf";
            clink = clink + response;
            window.open(clink, "_blank");
            // PrintJS(clink);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

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



function BuscarVentas() {

    var fecha1 = document.getElementById("fecha1").value;
    var fecha2 = document.getElementById("fecha2").value;


    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/POS/Consultas/GetVentasDevoluciones",
        data: { fecha1, fecha2 },
        beforeSend: InicioConsulta,
        success: ResultadoConsulta
    });

    function InicioConsulta() {
        //$('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ResultadoConsulta(data) {

        var arreglo = eval(data);
        cData = arreglo;

        //Para vaciar lo que tiene la tabla
        $('#mostrar_consulta').html('');

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
            pageSize: 10,
            sort: {
                field: "IDINTERNO",
                dir: "desc"
            }
        });

        $("#gridContainer").kendoGrid({
            toolbar: ["excel", "pdf", "search"],
            excel: {
                fileName: "EqWebReportList Orders.xlsx",
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
                    field: "OPCIONES",
                    title: "Opciones",
                    width: 180,
                    template: "<table border=\"0\"> <tr> " +
                        "<td><a data-toggle='modal' style='cursor:pointer;' value='#=(UUID)#' onclick='BuscarFactura(\"#=(UUID)#\");' ><img src='/images/folder.png' width='35' height='35'></a></td>" +
                        "<td><a data-toggle='modal' style='cursor:pointer;' value='#=(UUID)#' onclick='AbrirAnulacion(\"#=(UUID)#|#=(NO_DOCTO)#|#=(SERIE)#|#=(CLIENTE)#|#=(TOTAL)#|#=(FECHA)#\");' ><img src='/images/x.png' width='25' height='25'></a></td>" +
                        "<td><a data-toggle='modal' style='cursor:pointer;' value='#=(UUID)#' onclick='AbrirNCDesc(\"#=(UUID)#|#=(NO_DOCTO)#|#=(SERIE)#|#=(CLIENTE)#|#=(TOTAL)#\");' ><img src='/images/descto.png' width='25' height='25'></a></td>" +
                        "</tr></table>",

                    //"<table class=\"table table-bordered m-0\"><tr>" +
                    //    "<td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_FACTURAS' class='UUID' id='" + options.data.UUID + "'><img src='/images/folder.png' width='35' height='35'></a></td>" +
                    //    "<td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_FACTURAS' class='NUMAUT' id='" + options.data.UUID + "|" + options.data.NO_DOCTO + "|" + options.data.SERIE + "|" + options.data.CLIENTE + "|" + options.data.TOTAL + "|" + options.data.FECHA + "'><img src='/images/x.png' width='30' height='30'></a></td>" +
                    //    "<td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_FACTURAS' class='NCDESC' id='" + options.data.UUID + "|" + options.data.NO_DOCTO + "|" + options.data.SERIE + "|" + options.data.CLIENTE + "|" + options.data.TOTAL + "'><img src='/images/descto.png' width='30' height='30'></a></td>" + "</tr></table >")).appendTo(container);





                    stickable: true,
                    locked: true,
                    lockable: false,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "align: center" },
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
                    width: 160, stickable: true,
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

        var columnas = [
            {
                dataField: "OPCIONES",
                alignment: "CENTER",
                width: 250,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.append($("<table class=\"table table-bordered m-0\"><tr><td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_FACTURAS' class='UUID' id='" + options.data.UUID + "'><img src='/images/folder.png' width='35' height='35'></a></td>" + "<td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_FACTURAS' class='NUMAUT' id='" + options.data.UUID + "|" + options.data.NO_DOCTO + "|" + options.data.SERIE + "|" + options.data.CLIENTE + "|" + options.data.TOTAL + "|" + options.data.FECHA + "'><img src='/images/x.png' width='30' height='30'></a></td>" + "<td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_FACTURAS' class='NCDESC' id='" + options.data.UUID + "|" + options.data.NO_DOCTO + "|" + options.data.SERIE + "|" + options.data.CLIENTE + "|" + options.data.TOTAL + "'><img src='/images/descto.png' width='30' height='30'></a></td>"+"</tr></table >")).appendTo(container);

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



      
    }
}


function onShowing(e) {

    alert("hola");
    var toolbarItems = e.component.option("toolbarItems");
    var grid = $("#gridContainer").dxDataGrid("instance");
    toolbarItems[0].options.text = "jejejee";
    toolbarItems[1].options.text = "Close";

    e.component.option("toolbarItems", toolbarItems);
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