//$(document).ready(InicionEventos);

//function InicionEventos() {
   
//    //$('#gridContainer').on('click', '.INV', BuscarFichaInventario);
//    //$('#gridContainer').on('click', '.EXIS', BuscarExistencias);
//    //$('#gridContainer').on('click', '.IMGINV', BuscarImagen);

   
//}



function ActFiltroDoctos() {   


    var cfecha1 = document.getElementById("fecha1").value;
    var cfecha2 = document.getElementById("fecha2").value;
    var id_vendedor = document.getElementById("id_vendedor").value;
    var id_agencia = document.getElementById("id_agencia").value;
    var nit = document.getElementById("nit").value;
    var uuid = document.getElementById("uuid").value;
  
    fecha1 = formatDate(cfecha1);
    fecha2 = formatDate(cfecha2);

   
    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/ConsultaDocumentos/GetDocumentos",
        data: { fecha1,fecha2,id_vendedor,id_agencia,nit,uuid },
        beforeSend: InicioConsulta,
        success: ConsultaDocumentos
    });

    function InicioConsulta(){
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ConsultaDocumentos(data){
        var arreglo = eval(data);
        cData = arreglo;       
        
          

        var columnas = [
            {
                dataField: "OTROS",
                alignment: "right",
                width: 50,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {                    
                    container.append($("<table class=\"table table-bordered m-0\"><tr><td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='INV' id='" + options.value + "'><img src='/images/Load.PNG'></a></td>" + "<td></tr></table >")).appendTo(container);
                    
                }
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
           
            

        
        $(function(){
            var dataGrid = $("#gridContainer").dxDataGrid({
                
                dataSource: cData, 
                keyExpr: "IDINTERNO",
                selection: {
                    mode: "single"
                },
                "export": {
                    enabled: true,
                    fileName: "ConsultaDocumentos",
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

                //onSelectionChanged: function (selectedItems) {
                //    var data = selectedItems.selectedRowsData[0];
                //    if (data) {
                //        $(".Obs").text(data.SERIEINTERNA);
                //        alert(data.SERIEINTERNA);

                //    }
                //}
            }).dxDataGrid("instance");
            
            $("#autoExpand").dxCheckBox({
                value: false,
                text: "Expandir",
                onValueChanged: function(data) {
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