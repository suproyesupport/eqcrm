$(document).ready(InicionEventos);

function InicionEventos() {
    ActFiltroProblemaTickets();
}


function GuardarClienteTicket() {

    var cliente = document.getElementById("cliente").value;
    var nit = document.getElementById("nit").value;
    var intercompany = document.getElementById("intercompany").value;


    if (cliente != "" && intercompany != "") {

        $.ajax({
            async: true,
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            url: "/IngresoClienteTicket/InsertarClienteTicket",
            data: { cliente, nit, intercompany },
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
                /*ActFiltroInventario();*/
            },
            error: function () {
                alert("Ocurrio un Error");
            }
        });

        window.location.href = "/IngresoClienteTicket/IngresoClienteTicket";

    } else {

        document.getElementById("mensaje").innerHTML = "Verifique los campos";
        $("#modalGenerico").modal('show');
    }

}


function ActFiltroProblemaTickets() {

    var fecha1 = "";
    var fecha2 = "";
    var linea = "";

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoClienteTicket/GetClienteTicket",
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
                    container.append($("<i style='color:red; cursor:pointer;' class='fas fa-trash' onclick='EliminarClienteTicket(this)'></i><i style='color:#651FFF;cursor:pointer;' class='abrir' onclick='AbrirModificarClienteTicket(this)'> <img src='/images/agencias.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);
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
                alignment: "LEFT",
                width: 400,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "NIT",
                alignment: "LEFT",
                width: 200,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "INTERCOMPANY",
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
                    fileName: "ConsultaClienteTickets",
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


function EliminarClienteTicket(e) {

    let id = $(e).closest("tr")[0].cells[1].innerText;

    $.ajax({
        async: true,
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoClienteTicket/EliminarClienteTicket",
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

            location.reload();

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}


function AbrirModificarClienteTicket(e) {

    let idm = $(e).closest("tr")[0].cells[1].innerText;
    $("#id").val(idm);

    let clientem = $(e).closest("tr")[0].cells[2].innerText;
    $("#clientee").val(clientem);

    let nitm = $(e).closest("tr")[0].cells[3].innerText;
    $("#nitt").val(nitm);

    $('#ModalClienteTicketReasignar').modal('show');

}


function ModificarClienteTicket() {

    var id = document.getElementById("id").value;
    var clientee = document.getElementById("clientee").value;
    var nitt = document.getElementById("nitt").value;
    var intercompany2 = "";
    intercompany2 = document.getElementById("intercompany2").value;
        
    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoClienteTicket/ModificarClienteTicket",
        data: { id, clientee, nitt, intercompany2 },
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





//function tiene_letras(texto) {

//    var letras = "abcdefghyjklmnñopqrstuvwxyz/*-+¡!/()=?{}[]";
//    var bool = false;

//    texto = texto.toLowerCase();

//    for (i = 0; i < texto.length; i++) {
//        if (letras.indexOf(texto.charAt(i), 0) != -1) {
//            //$("#modal-comprobarcaracter").modal('show');
//            bool = true;
//        }
//    }
//    return bool;
//}
