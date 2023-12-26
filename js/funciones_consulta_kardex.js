$(document).ready(InicionEventos);

function InicionEventos() {

    $('#gridContainer').on('click', '.INV', BuscarFichaInventario);
    $('#gridContainer').on('click', '.EXIS', BuscarExistencias);
    //$('#gridContainer').on('click', '.IMGINV', BuscarImagen);
    //$('#gridContainer').on('click', '.FICHTEC', BuscarFichaTecnica);


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
                dataSource: ["/images/error.jpg"],
                height: 800,
                loop: true,
                animationDuration: 100
            });

            $('#ModalImages').modal('show');
        }
    });


}

function BuscarFichaInventario(id_codigo) {
    //var id_codigo = $(this).attr('id');


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

            $("#idcodigoeq").val(response.CODIGO);
            $("#codigoeq").val(arreglo.CODIGOE);
            $("#productoeq").val(arreglo.PRODUCTO);           
            $("#existenciaeq").val(arreglo.EXISTENCIA);
            $('#ModalFichaInv').modal('show');

            LlenarTablaKardex(response.CODIGO);


        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


}

function LlenarTablaKardex(id) {
    id_codigo = id;
    
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/ConsultaKardex/ConsultaKardexCodigo",
        data: {id_codigo},
        success: function (response) {
            
            $('#mostrar_kardex').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}

function BuscarExistencias(id_codigo) {
    //var id_codigo = $(this).attr('id');


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
        url: "/ConsultaKardex/GetListKardex",
        data: { id_codigo, codigoe, linea },
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
        document.querySelector("#mostrar_consulta").innerHTML = "";

        var arreglo = eval(data);
        cData = arreglo;

        var gridDataSource = new kendo.data.DataSource({
            data: cData,
            schema: {
                model: {
                    fields: {
                        CODIGO: { type: "string" },
                        CODIGOE: { type: "string" },
                        PRODUCTO: { type: "string" },
                        LINEA: { type: "string" },
                        EXISTENCIA: { type: "number" },
                        PRECIO1: { type: "number" },
                        PRECIO2: { type: "number" },
                        PRECIO3: { type: "number" },
                        PRECIO4: { type: "number" },
                        OBS: { type: "string" },
                    }
                }
            },
            pageSize: 20,
            sort: {
                field: "CODIGO",
                dir: "desc"
            }
        });

        $("#mostrar_consulta").kendoGrid({

            toolbar: ["excel", "pdf", "search"], //Podemos colocar opciones en la toolbar del datagrid

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
                    { name: "CODIGO", operator: "eq" },
                    //{ name: "Freight", operator: "gte" },
                    //{ name: "ShipName", operator: "contains" },
                    //{ name: "ShipCity", operator: "contains" },
                ]
            },

            dataSource: gridDataSource,
            pageable: true,
            height: 1000,
            columns: [
                {
                    field: "OTROS",
                    title: "Otros",
                    width: 200,
                    template: "<table class='table table-bordered m-0'><tr>" +
                        "<td> <a data-toggle='modal' style='cursor:pointer;' data-target='.VER_INVENTARIO' class='INV' id='#=(CODIGO)#'><img src='/images/Load.PNG' onclick='BuscarFichaInventario(#=(CODIGO)#)'></a></td > " +
                        "<td> <a data-toggle='modal' style='cursor:pointer;' data-target='.VER_INVENTARIO' class='EXIS' id='#=(CODIGO)#'><img src='/images/agencias.PNG' onclick='BuscarExistencias(#=(CODIGO)#)'></a></td > " +
                        "</tr></table>",
                    stickable: true,
                    locked: true,
                    lockable: false,
                }, {
                    field: "CODIGO",
                    title: "Codigo",
                    width: 130,
                }, {
                    field: "CODIGOE",
                    title: "Cod. Equivalente",
                    width: 130,
                }, {
                    field: "PRODUCTO",
                    title: "Producto",
                    width: 130,
                }, {
                    field: "LINEA",
                    title: "Linea",
                    width: 130,
                }, {
                    field: "EXISTENCIA",
                    title: "Existencia",
                    width: 130,
                }, {
                    field: "PRECIO1",
                    title: "Precio 1",
                    width: 130,
                }, {
                    field: "PRECIO2",
                    title: "Precio 2",
                    width: 130,
                }, {
                    field: "PRECIO3",
                    title: "Precio 3",
                    width: 130,
                }, {
                    field: "PRECIO4",
                    title: "Precio 4",
                    width: 130,
                }, {
                    field: "OBS",
                    title: "Observación",
                    width: 130,
                }],
            editable: "inline"
        });
    }
}



//function ActFiltroInventario() {


//    id_codigo = document.getElementById("id_codigo").value;
//    codigoe = document.getElementById("codigoe").value;
//    linea = document.getElementById("linea").value;

//    $.ajax({
//        async: false,
//        type: "POST",
//        dataType: "HTML",
//        contentType: "application/x-www-form-urlencoded",
//        url: "/ConsultaKardex/GetKardex",
//        data: { id_codigo, codigoe, linea },
//        beforeSend: InicioInventario,
//        success: ConsultaInventario
//    });

//    function InicioInventario() {
//        $('#mostrar_consulta').html('Cargando por favor espere...');
//    }

//    function ConsultaInventario(data) {
//        var arreglo = eval(data);
//        cData = arreglo;



//        var columnas = [
//            {
//                dataField: "DATOS",
//                alignment: "right",
//                width: 200,
//                allowFiltering: false,
//                allowSorting: false,
//                cellTemplate: function (container, options) {
//                    container.append($("<table class=\"table table-bordered m-0\"><tr><td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='INV' id='" + options.value + "'><img src='/images/Load.PNG'></a></td>" + "<td><a data-toggle='modal' style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='EXIS' id='" + options.value + "'><img src='/images/agencias.PNG'></a></td>" + " </tr></table >")).appendTo(container);
//                    //container.append($("<table class=\"table table-bordered m-0\"><tr><td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='INV' id='" + options.value + "'><img src='/images/Load.PNG'></a></td>" + "<td><a data-toggle='modal' style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='EXIS' id='" + options.value + "'><img src='/images/agencias.PNG'></a></td>" + "<td><a data-toggle='modal' style='color:#651FFF;cursor:pointer;' data-target='.VER_IMAGEN' class='IMGINV' id='" + options.value + "'><img src='/images/images.PNG'></a></td> <td><a style='color:#651FFF;cursor:pointer;' class='FICHTEC' id='" + options.value + "'><img src='/images/ficha_tecnica.png' onclick='BuscarFichaTecnica()' width=\"18\" height=\"auto\"></a></td> </tr></table >")).appendTo(container);
//                    //container.append($("<td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='EXIS' id='" + options.value + "'><img src='/images/agencias.PNG'></a></td></tr></table>")).appendTo(container);
//                }
//            },
//            {
//                dataField: "CODIGO",
//                width: 80,
//                allowFiltering: false,
//                allowSorting: false,

//            },
//            {
//                dataField: "CODIGOE",
//                width: 80,
//                allowFiltering: false,
//                allowSorting: false,

//            },

//            {
//                dataField: "PRODUCTO",
//                width: 400,
//                allowFiltering: true,
//                allowSorting: false,

//            },
//            {
//                dataField: "LINEA",
//                alignment: "right",
//                width: 80,
//                allowFiltering: true,
//                allowSorting: false,

//            },
//            {
//                dataField: "EXISTENCIA",
//                alignment: "right",
//                width: 90,
//                allowFiltering: false,
//                allowSorting: false,

//            },

//            {
//                dataField: "PRECIO1",
//                alignment: "right",
//                format: {
//                    type: "fixedPoint",
//                    precision: 0
//                },
//            },
//            {
//                dataField: "PRECIO2",
//                alignment: "right",
//                format: "c2"
//            },
//            {
//                dataField: "PRECIO3",
//                alignment: "right",
//                format: "c2"
//            },
//            {
//                dataField: "PRECIO4",
//                alignment: "right",
//                format: "c2"
//            }];


//        $(function () {
//            var dataGrid = $("#gridContainer").dxDataGrid({
//                dataSource: cData,
//                keyExpr: "CODIGO",
//                selection: {
//                    mode: "single"
//                },
//                "export": {
//                    enabled: true,
//                    fileName: "EstadisticasABC",
//                    allowExportSelectedData: true
//                },
//                dataSource: cData,
//                rowAlternationEnabled: true,
//                allowColumnReordering: true,
//                allowColumnResizing: true,
//                columnHidingEnabled: true,
//                showBorders: true,
//                columnChooser: {
//                    enabled: true
//                },
//                columnFixing: {
//                    enabled: true
//                },
//                grouping: {
//                    autoExpandAll: false,
//                },
//                searchPanel: {
//                    visible: true,
//                    width: 240,
//                    placeholder: "Search..."
//                },
//                //headerFilter: {
//                //    visible: true
//                //},
//                focusedRowEnabled: false,
//                paging: {
//                    pageSize: 18
//                },
//                groupPanel: {
//                    visible: true
//                },
//                pager: {
//                    showPageSizeSelector: true,
//                    showInfo: true
//                },
//                columns: columnas,

//            }).dxDataGrid("instance");

//            $("#autoExpand").dxCheckBox({
//                value: false,
//                text: "Expandir",
//                onValueChanged: function (data) {
//                    dataGrid.option("grouping.autoExpandAll", data.value);
//                }
//            });
//        });

//        $('#mostrar_consulta').html('');
//    }
//}

