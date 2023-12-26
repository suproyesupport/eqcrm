$(document).ready(InicionEventos);

function InicionEventos() {

    $('#gridContainer').on('click', '.UUID', BuscarFactura);
    $('#gridContainer').on('click', '.NUMAUT', AbrirAnulacion);
    $('#gridContainer').on('click', '.NCDESC', AbrirNCDesc);
    
}

function BuscarFactura() {    
    var clink = "";
    var cUUid = $(this).attr('id');
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


function AbrirAnulacion() {
        
    var cUUid = $(this).attr('id');
    
    var aUUid = cUUid.split('|');

    var uuid = aUUid[0];
    var nofactura = aUUid[1];
    var serie = aUUid[2];
    var cliente = aUUid[3];
    var total = aUUid[4];
    

    $("#uuid").val(uuid);
    $("#nofactura").val(nofactura);
    $("#serie").val(serie);
    $("#cliente").val(cliente);
    $("#total").val(total);
    

    $('#ModalAnulacion').modal('show');
}


function AbrirNCDesc() {

    var cUUid = $(this).attr('id');

    var aUUid = cUUid.split('|');

    var uuid = aUUid[0];
    var nofactura = aUUid[1];
    var serie = aUUid[2];
    var cliente = aUUid[3];
    var total = aUUid[4];

    var fecha = aUUid[5];



    $("#uuid2").val(uuid);
    $("#nofactura2").val(nofactura);
    $("#serie2").val(serie);
    $("#cliente2").val(cliente);
    $("#total2").val(total);
    $("#fecha").val(fecha);


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
                alignment: "right",
                width: 250,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.append($("<table class=\"table table-bordered m-0\"><tr><td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_FACTURAS' class='UUID' id='" + options.data.UUID + "'><img src='/images/Open.PNG'></a></td>" + "<td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_FACTURAS' class='NUMAUT' id='" + options.data.UUID + "|" + options.data.NO_DOCTO + "|" + options.data.SERIE + "|" + options.data.CLIENTE + "|" + options.data.TOTAL + "|" + options.data.FECHA + "'><img src='/images/Anula.PNG'></a></td>" + "<td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_FACTURAS' class='NCDESC' id='" + options.data.UUID + "|" + options.data.NO_DOCTO + "|" + options.data.SERIE + "|" + options.data.CLIENTE + "|" + options.data.TOTAL + "'><img src='/images/Devscuento.png'></a></td>"+"</tr></table >")).appendTo(container);

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
                //editing: {
                //    mode: 'popup',
                //    useIcons:true,
                //    allowUpdating: true,
                //    allowAdding: false,
                //    allowDeleting: false,                    
                //popup: {
                //    title: 'Informacion de Documento ',
                //    showTitle: true,
                //    width: 800,
                //    height: 525,                    
                //},
                //form: {
                //    items: [{
                       

                //        itemType: 'group',
                //        colCount: 2,
                //        colSpan: 2,
                //        OnShowing: "onShowing",
                //        items: ['NIT','CLIENTE','UUID', 'SERIE', 'NO_DOCTO',  'TOTAL', {
                //            dataField: 'MOTIVO ANULACION',
                //            editorType: 'dxTextArea',
                //            colSpan: 2,
                //            editorOptions: {
                //                height: 200,
                //            },
                            
                //        }],
                       
                //    }],

                //    },

                  
                //},               
                //columns: [
                //    {
                //        dataField: 'NIT',
                //        width: 270,
                //    },
                //    {
                //        dataField: 'CLIENTE',
                //        width: 270,
                //    },
                    
                //    {
                //        dataField: 'UUID',
                //        width: 270,
                //    },
                //    {
                //        dataField: 'SERIE',
                //        width: 170,
                //    },
                //    {
                //        dataField: 'TOTAL',
                //        caption: 'State',
                //        width: 125,
                       
                //    },
                   
                //],
                //onSaving: function (e) {
                //    e.cancel = true;
                //    alert("hola");
                //    if (e.changes.length) {
                //        e.promise = sendBatchRequest(URL, e.changes).done(function () {
                //            e.component.refresh(true).done(function () {
                //                e.component.cancelEditData();
                //            });
                //        });
                //    }
                //},

                headerFilter: {
                   visible: true
                },
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