$(document).ready(InicioEventos);



function InicioEventos() {

    ActFiltro();

}



//FUNCION ACTIVAR FILTRO
function ActFiltro() {

    var fecha1 = "";
    var fecha2 = "";
    var linea = "";

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModPreciosGAS/GetPreciosGAS",
        data: { fecha1, fecha2, linea },
        beforeSend: InicioConsulta,
        success: ConsultaCotizaciones
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando Tickets por favor espere...');
    }

    function ConsultaCotizaciones(data) {
        var arreglo = eval(data);
        cData = arreglo;

        var columnas = [
            {
                dataField: "OTROS",
                alignment: "center",
                width: 80,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.append($("<i style='color:#651FFF;cursor:pointer;' class='abrir' onclick='AbrirModal(this)'> <img src='/images/Load.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);
                },
            },
            {
                dataField: "ID",
                alignment: "center",
                width: 60,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "PRODUCTO",
                alignment: "center",
                width: 300,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "IDP",
                alignment: "center",
                width: 100,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "COSTO1",
                alignment: "center",
                width: 100,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "PRECIO1",
                alignment: "center",
                width: 100,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "COSTO2",
                alignment: "CENTER",
                width: 100,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "PRECIO2",
                alignment: "center",
                width: 100,
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
                    fileName: "ConsultaTickets",
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



//FUNCION ABRIR MODAL
function AbrirModal(e) {

    let idcodigoeq = $(e).closest("tr")[0].cells[1].innerText;
    let productoeq = $(e).closest("tr")[0].cells[2].innerText;
    let idp = $(e).closest("tr")[0].cells[3].innerText;
    let costoeq1 = $(e).closest("tr")[0].cells[4].innerText;
    let precioeq1 = $(e).closest("tr")[0].cells[5].innerText;
    let costoeq2 = $(e).closest("tr")[0].cells[6].innerText;
    let precioeq2 = $(e).closest("tr")[0].cells[7].innerText;
 
    $("#idcodigoeq").val(idcodigoeq);
    $("#productoeq").val(productoeq);
    $("#idp").val(idp);
    $("#costoeq1").val(costoeq1);
    $("#precioeq1").val(precioeq1);
    $("#costoeq2").val(costoeq2);
    $("#precioeq2").val(precioeq2);

    $('#modal-consulta').modal('show');

}



//FUNCION MODIFICAR PRECIOS GAS
function ModificarPreciosGAS() {

    var idcodigoeq = document.getElementById("idcodigoeq").value;
    var producto = document.getElementById("productoeq").value;
    var costoeq1 = document.getElementById("costoeq1").value;
    var precioeq1 = document.getElementById("precioeq1").value;
    var costoeq2 = document.getElementById("costoeq2").value;
    var precioeq2 = document.getElementById("precioeq2").value;
    var idp = document.getElementById("idp").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModPreciosGAS/ModificarPreciosGAS",
        data: { idcodigoeq, producto, costoeq1, precioeq1, costoeq1, costoeq2, precioeq2, idp },
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

            $('#modal-iniciarTicket').modal('hide');
            location.reload();

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

    
}



/*
 * FUNCION MODAL CONFIRMACION MODIFICAR
 * BUG: CORRECCION QUE CIERRA EL MODAL // btn.disabled = true -- CORRIGE EL BUG ESPECIFICAMENTE QUE SE CIERRA 
 */
function ModalModificar() {

    var idcodigoeq = document.getElementById("idcodigoeq").value;
    var producto = document.getElementById("productoeq").value;
    var costoeq1 = document.getElementById("costoeq1").value;
    var precioeq1 = document.getElementById("precioeq1").value;
    var costoeq2 = document.getElementById("costoeq2").value;
    var precioeq2 = document.getElementById("precioeq2").value;
    var idp = document.getElementById("idp").value;

    var btn = document.getElementById("btn-modificar");


    //BUG: btn.disabled = true -- CORRIGE EL BUG ESPECIFICAMENTE QUE SE CIERRA
    if (producto == '') {

        btn.disabled = true;

        $("#mensaje").html("Debe ingresar el nombre del producto.");
        $('#modal-generico').modal('show');
        return;
    }

    if (costoeq1 == '') {

        btn.disabled = true;

        $("#mensaje").html("Debe ingresar el Costo 1.");
        $('#modal-generico').modal('show');
        return;
    }

    if (precioeq1 == '') {

        btn.disabled = true;

        $("#mensaje").html("Debe ingresar el Precio 1.");
        $('#modal-generico').modal('show');
        return;
    }

    if (costoeq2 == '') {

        btn.disabled = true;

        $("#mensaje").html("Debe ingresar el Costo 2.");
        $('#modal-generico').modal('show');
        return;
    }

    if (precioeq2 == '') {

        btn.disabled = true;

        $("#mensaje").html("Debe ingresar el Precio 2.");
        $('#modal-generico').modal('show');
        return;
    }

    if (idp == '') {

        btn.disabled = true;

        $("#mensaje").html("Debe ingresar el IDP.");
        $('#modal-generico').modal('show');
        return;
    }

    ModificarPreciosGAS();
}



/*  
 *FUNCION PARA ACTIVAR EL BOTON
 *BUG: CORRECCION QUE CIERRA EL MODAL
 *NOTA: COLOCAR UN oninput="" EN EL INPUT -- EJEMPLO: oninput="ActivarButton();"
 */
function ActivarButton() {

    document.querySelector('#btn-modificar').disabled = false;

}
