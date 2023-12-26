$(document).ready(InicionEventos);

function InicionEventos() {
    LlenarTablaOrdenServicio();

}

function LlenarTablaOrdenServicio() {
    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModOrdenServicio/ConsultarOrdenServicio",
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
            {
                dataField: "INICIO",
                alignment: "left",
                width: 140,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "FINALIZACION",
                alignment: "left",
                width: 140,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "OBSERVACION",
                alignment: "right",
                width: 300,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "DESCRIPCION",
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


//INICIO MODAL PARA CONSULTAR LA FICHA DE LA ORDEN DE SERVICIO
function BuscarFichaOrdenServicio(e) {

    let id = $(e).closest("tr")[0].cells[1].innerText;
    var id_orden = id;

    let st = $(e).closest("tr")[0].cells[2].innerText;
    var status = st;

    $('#ModalFichaOrdenS').modal('show');

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModOrdenServicio/GetDataOrdenServicio",
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
            $("#fechai").val(arreglo.FECHAI);
            $("#fechaf").val(arreglo.FECHAF);
            $("#direccion").val(arreglo.DIRECCION);
            $("#telefono").val(arreglo.TELEFONO);
            $("#obs").val(arreglo.OBSERVACION);
            $("#descripcion").val(arreglo.DESCRIPCION);

            //COLOCAMOS EL VALOR QUE YA TENIA EL COMBOBOX // SE REALIZO EN EL QUERY LA CONSULTA DEL ID
            $("#ruta").val(arreglo.IDRUTA);
            $("#tipo").val(arreglo.IDTIPOORDEN);
            $("#tecnico").val(arreglo.IDTECNICO);
        },

        error: function () {
            alert("Ocurrio un Error");
        }
    });
}
//FINAL MODAL PARA CONSULTAR LA FICHA DE LA ORDEN DE SERVICIO


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


function ModalConModificarOrdenS() {
    $('#modal-modificarOrden').modal('show');
}


function ModificarOrdenServicio() {

    //Obtener value del dropdownlist
    var comboruta = document.getElementById("ruta");
    var selectedruta = comboruta.options[comboruta.selectedIndex].value;

    var combotecnico = document.getElementById("tecnico");
    var selectedtecnico = combotecnico.options[combotecnico.selectedIndex].value;

    var combotipo = document.getElementById("tipo");
    var selectedtipo = combotipo.options[combotipo.selectedIndex].value;

    var id_orden = document.getElementById("id_orden").value;
    //var id = document.getElementById("id").value; //id Cliente
    //var cliente = document.getElementById("cliente").value;
    var direccion = document.getElementById("direccion").value;
    var telefono = document.getElementById("telefono").value;
    var id_ruta = selectedruta;
    var id_tecnico = selectedtecnico;
    var id_tipoorden = selectedtipo;
    var obs = document.getElementById("obs").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModOrdenServicio/ModificarOrdenServicio",
        data: { id_orden, direccion, telefono, id_ruta, id_tecnico, id_tipoorden, obs },
        success: function (response) {
            var arreglo = response;

            if (response.CODIGO == "ERROR") {
                if (response.PRODUCTO == "") {
                    $('#Error').html("OCURRIO UN ERROR");
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

    window.location.href = "/ModOrdenServicio/ModOrdenServicio";
}


function seleccionar(id) {
    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");

    document.getElementById("id").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("cliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[1].innerHTML;
    document.getElementById("direccion").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[2].innerHTML;
    document.getElementById("telefono").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[5].innerHTML;

    $("#ayudaclientes").modal('hide');
    $('#ModalFichaOrdenS').modal('show');
}


function ModalConAnularOrdenS() {
    $('#modal-anularOrden').modal('show');
}


function AnularOrdenServicio() {

    var id_orden = document.getElementById("id_orden").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModOrdenServicio/AnularOrdenServicio",
        data: { id_orden },
        success: function (response) {
            var arreglo = response;
            if (response.CODIGO == "ERROR") {
                if (response.PRODUCTO == "") {
                    $('#Error').html("OCURRIO UN ERROR");
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

    window.location.href = "/ModOrdenServicio/ModOrdenServicio";
}



