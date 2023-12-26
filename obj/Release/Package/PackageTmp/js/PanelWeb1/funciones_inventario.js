$(document).ready(InicionEventos);

function InicionEventos() {
   
    $('#gridContainer').on('click', '.INV', BuscarFichaInventario);
    $('#gridContainer').on('click', '.EXIS', BuscarExistencias);
    $('#gridContainer').on('click', '.IMGINV', BuscarImagen);
    
    ActFiltroInventario();
   
}



function BuscarImagen() {

    var id_codigo = $(this).attr('id');


    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/Inventario/GetDataImages",
        data: { id_codigo },
        success: function (data) {           
             
            var gallery = eval(data);
            images = gallery;
            $("#galleryContainer").dxGallery({
                dataSource: images,
                height: 800,
                loop: true,
                animationDuration: 100
            });

            $('#ModalImages').modal('show');
        },
        error: function () {
            $("#galleryContainer").dxGallery({
                dataSource: [ "/images/error.jpg" ],
                height: 800,
                loop: true,
                animationDuration: 100
            });

            $('#ModalImages').modal('show');
        }
    });

    
}

function BuscarFichaInventario() {
    var id_codigo = $(this).attr('id');

    location.href = "/movidiarios/filtromovdiarioscod/" + id_codigo;
    //window.open("/movidiarios/filtromovdiarioscod/" + id, "_blank")
    

}



function BuscarExistencias() {
    var id_codigo = $(this).attr('id');


    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/Inventario/GetDataExistenciaAgencias",
        data: { id_codigo },
        success: function (response) {
            $('#mostrar_consulta_agencias').html(response);
            $('#ModalExistencias').modal('show');
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


}

function ActFiltroInventario() {   
    


    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/PanelInventario/GetInventario",
        data: { },
        beforeSend: InicioInventario,
        success: ConsultaInventario
    });

    function InicioInventario(){
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ConsultaInventario(data){
        var arreglo = eval(data);
        cData = arreglo;       
        
          

        var columnas = [            
            {
                dataField: "CODIGO",
                width: 80,
                allowFiltering: false,
                allowSorting: false,

            }, 
                 
              
            {
                dataField: "PRODUCTO",
                width: 100,
                allowFiltering: true,
                allowSorting: false,    
                
            },
            {
                dataField: "UMEDIDA",
                alignment: "right",
                width: 80,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "EXISTENCIA",
                alignment: "right",
                width: 90,
                allowFiltering: false,
                allowSorting: false,

            }, 
            
            {
                dataField: "PRECIO1",
                alignment: "right",
                width: 90,
                dataType: 'number',
                format: '#,##0.00',
            },

            {
                dataField: "COSTO",
                alignment: "right",
                width: 90,
                dataType: 'number',
                format: '#,##0.00',
            },

            {
                dataField: "COSTOT",
                width: 150,
                dataType: 'number',
                alignment: "right",
                format: '#,##0.00',
            }];

        
        $(function(){
            var dataGrid = $("#gridContainer").dxDataGrid({
                dataSource: cData,
                keyExpr: "CODIGO",
                selection: {
                    mode: "single"
                },
                "export": {
                    enabled: true,
                    fileName: "MovimientoProducto",
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
                    placeholder: "Buscar..."
                },
                //headerFilter: {
                //    visible: true
                //},
                focusedRowEnabled: false,
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
                summary: {
                    totalItems: [{
                        column: 'PRODUCTO',
                        summaryType: 'count',
                    }, 
                    {
                        column: 'COSTOT',
                        summaryType: 'sum',
                        valueFormat: '#,##0.00',
                    }],
                },
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

