$(document).ready(InicionEventos);

function InicionEventos() {
   
    $('#gridContainer').on('click', '.INV', BuscarFichaInventario);
    $('#gridContainer').on('click', '.EXIS', BuscarExistencias);
    $('#gridContainer').on('click', '.IMGINV', BuscarImagen);
    $('#gridContainer').on('click', '.FICHTEC', BuscarFichaTecnica);
    $('#gridContainer').on('click', '.ELI', EliminaCodigo);
   
}

function EliminaCodigo() {
    var id = $(this).attr('id');
    if (id >= 1) {
        $.ajax({
            async: false,
            type: "POST",
            dataType: "text",
            contentType: "application/x-www-form-urlencoded",
            url: "/Inventario/EliminaCodigo/" + id,
            data: { id },
            success: function (response) {
                var url = response.toString();
                var letras = url.length;
                var urlFormateada = (url.slice(1, (letras - 1)));
                window.open(urlFormateada);
                urlFormateada = null;
            },
            error: function (response) {
                alert(response.toString());
            }
        });
    }
}


function BuscarFichaTecnica() {
    var id = $(this).attr('id');
    if (id >= 1) {
        $.ajax({
            async: false,
            type: "POST",
            dataType: "text",
            contentType: "application/x-www-form-urlencoded",
            url: "/Inventario/GetDataFichaTecnica/" + id,
            data: { id },
            success: function (response) {
                var url = response.toString();
                var letras = url.length;
                var urlFormateada = (url.slice(1, (letras - 1)));
                window.open(urlFormateada);
                urlFormateada = null;
            },
            error: function (response) {
                alert(response.toString());
            }
        });
    }

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

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModInventario/GetDataInv",
        data: { id_codigo },
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

            $("#idcodigoeq").val(response.CODIGO);
            $("#codigoeq").val(arreglo.CODIGOE);
            $("#productoeq").val(arreglo.PRODUCTO);
            $("#lineaeq").val(arreglo.LINEA);
            $("#existenciaeq").val(arreglo.EXISTENCIA);
            $("#lineapeq").val(arreglo.LINEAP);

            $("#costoeq1").val(arreglo.COSTO1);
            $("#costoeq2").val(arreglo.COSTO2);
            $("#costoeq3").val(arreglo.COSTO3);
            $("#costoeq4").val(arreglo.COSTO4);

            $("#precioeq1").val(arreglo.PRECIO1);
            $("#precioeq2").val(arreglo.PRECIO2);
            $("#precioeq3").val(arreglo.PRECIO3);
            $("#precioeq4").val(arreglo.PRECIO4);

            $("#adicionales").val(arreglo.OBS);

            $("#stockmin").val(arreglo.STOCKMIN);
            $("#stockmax").val(arreglo.STOCKMAX);

            $("#alias").val(arreglo.ALIAS);

            $("#defaultUnchecked").val(arreglo.SERVICIO);
            
                        
            $('#ModalFichaInv').modal('show');
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


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
    
    id_codigo = document.getElementById("id_codigo").value;
    codigoe = document.getElementById("codigoe").value;
    linea = document.getElementById("linea").value;

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModInventario/GetInventario",
        data: { id_codigo,codigoe,linea },
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
                dataField: "DATOS",
                alignment: "right",
                width: 200,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {                    
                    container.append($("<table class=\"table table-bordered m-0\"><tr><td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='INV' id='" + options.value + "'><img src='/images/Load.PNG'></a></td>" + "<td><a data-toggle='modal' style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='EXIS' id='" + options.value + "'><img src='/images/agencias.PNG'></a></td>" + "<td><a data-toggle='modal' style='color:#651FFF;cursor:pointer;' data-target='.VER_IMAGEN' class='IMGINV' id='" + options.value + "'><img src='/images/images.PNG'></a></td> <td><a style='color:#651FFF;cursor:pointer;' class='FICHTEC' id='" + options.value + "'><img src='/images/ficha_tecnica.png' onclick='BuscarFichaTecnica()' width=\"18\" height=\"auto\"></a></td>" + "<td><a data-toggle='modal' style='color:#651FFF;cursor:pointer;' data-target='.ELIMIMAR_INVENTARIO' class='ELI' id='" + options.value + "'><img src='/images/agencias.PNG'></a></td>"+" </tr></table >")).appendTo(container);
                    //container.append($("<td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='EXIS' id='" + options.value + "'><img src='/images/agencias.PNG'></a></td></tr></table>")).appendTo(container);
                }
            },
            {
                dataField: "CODIGO",
                width: 80,
                allowFiltering: false,
                allowSorting: false,

            }, 
            {
                dataField: "CODIGOE",
                width: 80,
                allowFiltering: false,
                allowSorting: false,

            },             
              
            {
                dataField: "PRODUCTO",
                width: 400,
                allowFiltering: true,
                allowSorting: false,    
                
            },
            {
                dataField: "LINEA",
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
                format: {
                    type: "fixedPoint",
                    precision: 0
                },
            },
            {
                dataField: "PRECIO2",
                alignment: "right",
                format: "c2"
            },
            {
                dataField: "PRECIO3",
                alignment: "right",
                format: "c2"
            },
            {
                dataField: "PRECIO4",
                alignment: "right",
                format: "c2"
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
                    fileName: "ModInventario",
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



function ActualizaInv() {

    $('#modal-guardainv').modal('show');


}

function ModificarProducto() {

    $('#modal-guardainv').modal('hide');

    var idcodigoeq = document.getElementById("idcodigoeq").value;
    var codigoeq = document.getElementById("codigoeq").value;
    var alias = document.getElementById("alias").value;
    var producto = document.getElementById("productoeq").value;
    var id_linea = document.getElementById("id_linea").value;
    
    var lineaeq = "1";

    var costoeq1 = document.getElementById("costoeq1").value;
    var costoeq2 = document.getElementById("costoeq2").value;
    var costoeq3 = document.getElementById("costoeq3").value;
    var costoeq4 = document.getElementById("costoeq4").value;

    var precioeq1 = document.getElementById("precioeq1").value;
    var precioeq2 = document.getElementById("precioeq2").value;
    var precioeq3 = document.getElementById("precioeq3").value;
    var precioeq4 = document.getElementById("precioeq4").value;

    var stockmin = document.getElementById("stockmin").value;
    var stockmax = document.getElementById("stockmax").value;
    var chkservicio = document.getElementById("defaultUnchecked").value;
    

    var obs = document.getElementById("obs").value;

    if (chkservicio.checked == true) {
        servicio = "S";
    }
    else {
        servicio = "N";
    }
   
    

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModInventario/ModificarProducto",
        data: { idcodigoeq, codigoeq, alias, producto, id_linea, lineaeq, costoeq1, costoeq2, costoeq3, costoeq4, precioeq1, precioeq2, precioeq3, precioeq4, stockmin, stockmax, servicio, obs },
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


}
