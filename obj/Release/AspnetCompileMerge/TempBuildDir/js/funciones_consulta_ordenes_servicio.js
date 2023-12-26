//$(document).ready(InicionEventos);

//function InicionEventos() {
   
//    //$('#gridContainer').on('click', '.INV', BuscarFichaInventario);
//    //$('#gridContainer').on('click', '.EXIS', BuscarExistencias);
//    //$('#gridContainer').on('click', '.IMGINV', BuscarImagen);

   
//}

$(document).ready(InicionEventos);

function InicionEventos() {
    //$('#gridContainer').on('click', '.ORS', BuscarFichaOrdenServicio);
    LlenarTablaOrdenServicio();
    
}

function LlenarTablaOrdenServicio() {
    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/ConsultaOrdenServicio/ConsultarOrdenServicio",
        data: {},
        beforeSend: InicioConsulta,
        success: ConsultaDocumentos
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    //<table class=\"table table-bordered m-0\"><tr><td><a data-toggle='modal' style='color:#651FFF;cursor:pointer;' data-target='.VER_ORD' class='ORS' id='" + options.value + "'><img src='/images/Load.PNG'></a></td>" + "<td></tr></table >"
    //<i style='color:#651FFF;cursor:pointer;' class='.VER_ORD' onclick='BuscarFichaOrdenServicio(this)'> <img src='/images/Load.PNG' width=\"18\" height=\"auto\"></i>
    function ConsultaDocumentos(data) {
        var arreglo = eval(data);
        cData = arreglo;

        var columnas = [
            {
                dataField: "OTROS",
                alignment: "right",
                width: 70,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                container.append($("<i style='color:#651FFF;cursor:pointer;' class='.VER_ORD' onclick='BuscarFichaOrdenServicio(this)'> <img src='/images/Load.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);
                }
            },
            {
                dataField: "ID",
                alignment: "center",
                width: 60,
                allowFiltering: true,
                allowSorting: true,
            },
            {
                dataField: "STATUS",
                alignment: "center",
                width: 98,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "CLIENTE",
                alignment: "left",
                width: 200,
                allowFiltering: true,
                allowSorting: true,
            },
            {
                dataField: "DIRECCION",
                alignment: "left",
                width: 300,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "RUTA",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "FECHA",
                alignment: "left",
                width: 140,
                allowFiltering: true,
                allowSorting: false,
            },
            //{
            //    dataField: "INICIO",
            //    alignment: "left",
            //    width: 140,
            //    allowFiltering: true,
            //    allowSorting: false,
            //},
            {
                dataField: "OBSERVACION",
                alignment: "right",
                width: 300,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "TECNICO",
                alignment: "left",
                width: 150,
                allowFiltering: true,
                allowSorting: false,
            }];

        $(function () {
            var dataGrid = $("#gridContainer").dxDataGrid({
                dataSource: cData,
                keyExpr: "ID",
                selection: {
                    mode: "single"
                },
                "export": {
                    enabled: true,
                    fileName: "OrdenesPendientes",
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
                headerFilter: {
                    visible: true
                },
                paging: {
                    pageSize: 25
                },
                groupPanel: {
                    visible: true
                },
                filterRow: {
                    visible: true,
                },
                pager: {
                    showPageSizeSelector: true,
                    showInfo: true
                },
                columns: columnas,
                //onSelectionChanged: function (selectedItems) {
                //    var data = selectedItems.selectedRowsData[0];
                //    if (data) {
                //        //$(".Obs").text(data.SERIEINTERNA);
                //        alert(data.ID);

                   //}
                //}
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


//MODAL PARA CONSULTAR LA FICHA DE LA ORDEN DE SERVICIO
function BuscarFichaOrdenServicio(e) {
    
    let id = $(e).closest("tr")[0].cells[1].innerText;
    var id_orden = id;

    let st = $(e).closest("tr")[0].cells[2].innerText;
    var status = st;

    //alert(status);
 
    $('#ModalFichaOrdenS').modal('show');
    
    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/ConsultaOrdenServicio/GetDataOrdenServicio",
        data: { id_orden },
        success: function (response) {
            var arreglo = response;

            if (response.CODIGO == "ERROR") {

                if (response.PRODUCTO == "") {
                    $('#Error').html("EL CODIGO DE EN LA BASE DE DATOS");
                    $('#ModalError').modal('show');
                    return;
                }
                else {
                    $('#Error').html(response.PRODUCTO);
                    $('#ModalError').modal('show');
                    return;
                }
            }

            $("#id_orden").val(response.ID);
            $("#fechaa").val(arreglo.FECHAA);
            $("#status").val(arreglo.STATUS);
            $("#id").val(arreglo.IDCLIENTE);
            $("#cliente").val(arreglo.CLIENTE);
            $("#tecnico").val(arreglo.TECNICO);
            $("#direccion").val(arreglo.DIRECCION);
            $("#telefono").val(arreglo.TELEFONO);
            $("#ruta").val(arreglo.RUTA);
            $("#tipoorden").val(arreglo.TIPOORDEN);
            $("#obs").val(arreglo.OBSERVACION);

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

    var button = document.getElementById("iniciabutton");
    var buttoncerrar = document.getElementById("cerrarbutton");
    var descripcion = document.getElementById("descripcion");

    if (status == 'INGRESADA') {
        button.style.display = 'block';
        buttoncerrar.style.display = 'none';
        descripcion.disabled = true;
    } else if (status == 'PROCESO') {
        button.style.display = 'none';
        buttoncerrar.style.display = 'block';
        descripcion.disabled = false;
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


function modalIniciarOrdenServicio() {
    $('#modal-iniciarOrden').modal('show');
}


function IniciarOrdenServicio() {

    $('#modal-iniciarOrden').modal('hide');

    var id_orden = document.getElementById("id_orden").value;

    //var button = document.getElementById("iniciabutton").setAttribute("class", "btn btn-secondary");
    //button.disabled = true;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/ConsultaOrdenServicio/IniciarOrden",
        data: { id_orden },
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

    location.reload();
}


function modalFinalizarOrdenServicio() {
    $('#modal-cerrarOrden').modal('show');
}


function FinalizarOrdenServicio() {

    $('#modal-cerrarOrden').modal('hide');

    var id_orden = document.getElementById("id_orden").value;
    var descripcion = document.getElementById("descripcion").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/ConsultaOrdenServicio/FinalizarOrden",
        data: { id_orden, descripcion },
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
            //alert("Ocurrio un Error");
        }
    });

    location.reload();
}