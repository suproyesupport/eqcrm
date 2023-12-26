$(document).ready(InicionEventos);

function InicionEventos() {

    $('#gridContainer').on('click', '.INV', BuscarFichaInventario);
    
    BuscarID();

   
}

function BuscarFichaInventario() {
    var id_codigo = $(this).attr('id');


    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Inventario/GetDataInv",
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

            $("#idcodigoeq2").val(response.CODIGO);
            

            
            $('#ModalFichaInv').modal('show');



        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


}




function GuardaInv() {
    
    $('#modal-guardainv').modal('show');


}

function BuscarCodigo() {

    var id_codigo = document.getElementById("codigoeq").value;

    
    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Inventario/BuscarCodigo",
        data: { id_codigo },
        success: function (response) {
            var arreglo = response;
            
            if (response.CODIGO == "ERROR") {


            }
            else {

            
                $('#Error').html("EL CODIGO DE PRODUCTO YA SE ENCUENTRA INGRESADO");
                $('#ModalError').modal('show');                
                return false;
                
            }

          
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


}



function BuscarID() {
    
    var id_codigo = document.getElementById("idcodigoeq").value;

    
    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Inventario/BuscarID",
        data: { id_codigo  },
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

            id_codigo = response.CODIGO + 1;

            $("#idcodigoeq").val(id_codigo);
               
            document.getElementById("costoeq1").value = "0.00";
            document.getElementById("costoeq2").value = "0.00";
            document.getElementById("costoeq3").value = "0.00";
            document.getElementById("costoeq4").value = "0.00";

            document.getElementById("precioeq1").value ="0.00";
            document.getElementById("precioeq2").value ="0.00";
            document.getElementById("precioeq3").value ="0.00";
            document.getElementById("precioeq4").value ="0.00";

            document.getElementById("stockmin").value = "0.00";
            document.getElementById("stockmax").value = "0.00";

            document.getElementById("codigoeq").value="";
            document.getElementById("alias").value="";
            document.getElementById("productoeq").value="";
            document.getElementById("linea").value="";
            document.getElementById("lineaeq").value="";


        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

  
}

function GuardarProducto() {

    $('#modal-guardainv').modal('hide');

    //alert("hola mundo");

    var idcodigoeq = document.getElementById("idcodigoeq").value;
    var codigoeq = document.getElementById("codigoeq").value;
    var alias = document.getElementById("alias").value;
    var producto = document.getElementById("productoeq").value;
    var id_linea = document.getElementById("id_linea").value;

    //var lineaeq = document.getElementById("lineaeq").value;

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

    if (chkservicio.checked == true)
    {
        servicio = "S";
    }
    else
    {
        servicio = "N";
    }

    //if (chkdolares.checked == true) {
    //    dolares = "S";
    //}
    //else {
    //    dolares = "N";
    //}
 
    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Inventario/InsertarProducto",
        data: { idcodigoeq, codigoeq, alias, producto, id_linea, lineaeq, costoeq1, costoeq2, costoeq3, costoeq4, precioeq1, precioeq2,precioeq3,precioeq4,stockmin,stockmax,servicio,obs },
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

            ActFiltroInventario();
            //$('#ModalInfoAdicional').modal('show');
            //BuscarID();
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


}







function ActFiltroInventario() {


    id_codigo = document.getElementById("idcodigoeq").value;;
    codigoe = "";
    linea = "";



    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/Inventario/GetInventario",
        data: { id_codigo, codigoe, linea },
        beforeSend: InicioInventario,
        success: ConsultaInventario
    });

    function InicioInventario() {
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ConsultaInventario(data) {
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
                    container.append($("<table class=\"table table-bordered m-0\"><tr><td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='INV' id='" + options.value + "'><img src='/images/Load.PNG'></a></td></tr></table >")).appendTo(container);
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


        $(function () {
            var dataGrid = $("#gridContainer").dxDataGrid({
                dataSource: cData,
                keyExpr: "CODIGO",
                selection: {
                    mode: "single"
                },
                "export": {
                    enabled: true,
                    fileName: "EstadisticasABC",
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
                onValueChanged: function (data) {
                    dataGrid.option("grouping.autoExpandAll", data.value);
                }
            });
        });

        $('#mostrar_consulta').html('');
    }
}