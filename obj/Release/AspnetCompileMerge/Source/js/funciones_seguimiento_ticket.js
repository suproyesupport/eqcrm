$(document).ready(InicionEventos);



function InicionEventos() {

    ActFiltroTickets();
    
}



//MODAL PARA CONSULTAR LA FICHA DE LA ORDEN DE SERVICIO
function AbrirTicket(e) {

    let id = $(e).closest("tr")[0].cells[1].innerText;
    let fechaticket = $(e).closest("tr")[0].cells[5].innerText;
    let estado = $(e).closest("tr")[0].cells[6].innerText;
    let idcategoria = $(e).closest("tr")[0].cells[3].innerText;
    let problema = $(e).closest("tr")[0].cells[4].innerText;
    let cliente = $(e).closest("tr")[0].cells[2].innerText;
    let reporta = $(e).closest("tr")[0].cells[8].innerText;
    let descripcion = $(e).closest("tr")[0].cells[9].innerText;

    var button = document.getElementById("iniciabutton");
    var buttonsegui = document.getElementById("btn-seguimiento");
    var buttoncerrar = document.getElementById("cerrarbutton");
    var seguimiento = document.getElementById("seguimiento");
    var solucion = document.getElementById("solucion");

    $("#id").val(id);
    $("#fechaticket").val(fechaticket);
    $("#idcategoria").val(idcategoria);
    $("#problema").val(problema);
    $("#cliente").val(cliente);    
    $("#reporta").val(reporta);
    $("#estado").val(estado);
    $("#descripcion").val(descripcion);
    
    if (estado == 'GENERADO') {

        button.style.display = 'block';
        buttonsegui.style.display = 'none';
        buttoncerrar.style.display = 'none';
        seguimiento.disabled = true;
        solucion.disabled = true;

    } else if (estado == 'PROCESO') {

        button.style.display = 'none';
        buttonsegui.style.display = 'block';
        buttoncerrar.style.display = 'block';
        seguimiento.disabled = false;
        solucion.disabled = false;
    }

    $.ajax({
        async: true,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/SeguimientoTicket/GetSeguimientoTicket",
        data: { id },
        success: function (response) {

            $('#seguimientoticket').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

    $('#ModalTicketConsulta').modal('show');

}



function ActFiltroTickets() {

    var fecha1 = "";
    var fecha2 = "";
    var linea = "";

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/SeguimientoTicket/GetTicketsSeguimiento",
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
                    container.append($("<i style='color:#651FFF;cursor:pointer;' class='abrir' onclick='AbrirTicket(this)'> <img src='/images/Load.PNG' width=\"18\" height=\"auto\"></i><i style='color:#651FFF;cursor:pointer;' class='abrir' onclick='AbrirReasignarTicket(this)'> <img src='/images/agencias.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);
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
                dataField: "CLIENTE",
                alignment: "center",
                width: 200,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "CATEGORIA",
                alignment: "center",
                width: 160,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "PROBLEMA",
                alignment: "center",
                width: 160,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "FECHAA",
                alignment: "center",
                width: 90,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "ESTADO",
                alignment: "CENTER",
                width: 100,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "DIAS",
                alignment: "center",
                width: 50,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "REPORTA",
                alignment: "CENTER",
                width: 200,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "DESCRIPCION",
                alignment: "LEFT",
                width: 500,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "USUARIOASIGNADO",
                alignment: "LEFT",
                width: 200,
                allowFiltering: false,
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



function modalIniciarTicket() {
    $('#modal-iniciarTicket').modal('show');
}



function IniciarTicket() {

    $('#modal-iniciarTicket').modal('hide');

    var id = document.getElementById("id").value;

    //var button = document.getElementById("iniciabutton").setAttribute("class", "btn btn-secondary");
    //button.disabled = true;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/SeguimientoTicket/IniciarTicket",
        data: { id },
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



function modalSeguimientoTicket() {

    $('#modal-seguimientoTicket').modal('show');
}



/*  
 *FUNCION PARA ACTIVAR EL BOTON
 *BUG: CORRECCION QUE CIERRA EL MODAL
 *NOTA: COLOCAR UN oninput="" EN EL INPUT -- EJEMPLO: oninput="ActivarButton();"
 */
function ActivarButton() {

    document.querySelector('#btn-seguimiento').disabled = false;

}



/*
 * FUNCION MODAL CONFIRMACION MODIFICAR
 * BUG: CORRECCION QUE CIERRA EL MODAL // btn.disabled = true -- CORRIGE EL BUG ESPECIFICAMENTE QUE SE CIERRA 
 */
function ModalSeguimiento() {

    var seguimiento = document.getElementById("seguimiento").value;
    var btn = document.getElementById("btn-seguimiento");

    //BUG: btn.disabled = true -- CORRIGE EL BUG ESPECIFICAMENTE QUE SE CIERRA
    if (seguimiento == '') {

        btn.disabled = true;

        $("#mensaje").html("Debe ingresar el seguimiento del ticket.");
        $('#modal-generico').modal('show');
        return;
    }

    btn.disabled = true;
    SeguimientoTicket();
    document.getElementById("seguimiento").value = "";

}



function SeguimientoTicket() {

    var id = document.getElementById("id").value;
    var seguimiento = document.getElementById("seguimiento").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/SeguimientoTicket/IngSeguimientoTicket",
        data: { id, seguimiento },
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

    $.ajax({
        async: true,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/SeguimientoTicket/GetSeguimientoTicket",
        data: { id },
        success: function (response) {

            $('#seguimientoticket').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

    document.getElementById("seguimiento").value = "";
    
}



function modalFinalizarTicket() {
    $('#modal-cerrarTicket').modal('show');
}



function FinalizarTicket() {

    $('#modal-cerrarTicket').modal('hide');

    var id = document.getElementById("id").value;
    var solucion = document.getElementById("solucion").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/SeguimientoTicket/FinalizarTicket",
        data: { id, solucion },
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



function AbrirReasignarTicket(e) {

    let id = $(e).closest("tr")[0].cells[1].innerText;
    $("#idd").val(id);

    $('#ModalTicketReasignar').modal('show');
}



function modalReasignarTicket() {
    $('#modal-reasignarTicket').modal('show');
}



function ReasignarTicket() {

    var id = document.getElementById("idd").value;
    var usuarior = document.getElementById("usuarior").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/SeguimientoTicket/ReasignarTicket",
        data: { id, usuarior },
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

