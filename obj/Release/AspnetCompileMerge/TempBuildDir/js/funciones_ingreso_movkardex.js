$(document).ready(InicionEventos);



function InicionEventos() {
    ActFiltroMovKardex();
}



function ActFiltroMovKardex() {

    var fecha1 = "";
    var fecha2 = "";
    var linea = "";

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoMovKardex/GetMovKardex",
        data: { fecha1, fecha2, linea },
        beforeSend: InicioConsulta,
        success: ConsultaCotizaciones
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando por favor espere...');
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
                    //container.append($("<i style='color:red; cursor:pointer;' class='fas fa-trash' onclick='EliminarMovKardex(this)'></i><i style='color:#651FFF;cursor:pointer;' class='abrir' onclick='ModificarMovKardex(this)'> <img src='/images/agencias.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);
                    container.append($("<i style='color:#651FFF;cursor:pointer;' class='abrir' onclick='ModificarMovKardex(this)'> <img src='/images/agencias.PNG' width=\"18\" height=\"auto\"></i>")).appendTo(container);
                },
            },
            {
                dataField: "ID",
                alignment: "center",
                width: 80,
                allowFiltering: false,
                allowSorting: false,

            },
            {
                dataField: "MOVIMIENTO",
                alignment: "CENTER",
                width: 400,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "SERIE",
                alignment: "CENTER",
                width: 400,
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
                    fileName: "Consulta",
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



function GuardarMovKardex() {

    var id = document.getElementById("id").value;
    var movimiento = document.getElementById("movimiento").value;
    var serie = document.getElementById("serie").value;

    if (id == "") {
        document.getElementById("mensaje").innerHTML = "Debe ingresar el ID";
        $("#modalGenerico").modal('show');
        return;
    }

    if (movimiento == "") {
        document.getElementById("mensaje").innerHTML = "Debe ingresar el nombre del movimiento";
        $("#modalGenerico").modal('show');
        return;
    }

    if (serie == "") {
        document.getElementById("mensaje").innerHTML = "Debe ingresar la serie";
        $("#modalGenerico").modal('show');
        return;
    }

    $.ajax({
        async: true,
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoMovKardex/InsertarMovKardex",
        data: { id, movimiento, serie },
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



function EliminarMovKardex(e) {

    $('#modal-eliminar').modal('show');

    let confirmar = document.getElementById("eliminabutton");

    //CAPTURA CLICK EN EL BOTON Y PROCEDEMOS A ELIMINAR
    confirmar.onclick = function () {

        let id = $(e).closest("tr")[0].cells[1].innerText;

        $.ajax({
            async: true,
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            url: "/IngresoMovKardex/EliminarMovKardex",
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
}



function ModificarMovKardex(e) {

    //let col2 = $(e).closest("tr")[0].cells[2].innerText;
    //$("#movimientoo").val(col2);

    //DAMOS VALOR AL INPUT 
    $("#idd").val($(e).closest("tr")[0].cells[1].innerText);
    $("#movimientoo").val($(e).closest("tr")[0].cells[2].innerText);
    $("#seriee").val($(e).closest("tr")[0].cells[3].innerText);

    $('#modal-editar').modal('show');

    //CONFIRMAR LA MODIFICACION
    let confirmar = document.getElementById("modificabutton");

    //CAPTURA CLICK EN EL BOTON Y PROCEDEMOS A MODIFICAR
    confirmar.onclick = function () {

        var idd = document.getElementById("idd").value;
        var movimientoo = document.getElementById("movimientoo").value;
        var seriee = document.getElementById("seriee").value;

        if (idd == "") {
            document.getElementById("mensaje").innerHTML = "Debe ingresar el ID";
            $("#modalGenerico").modal('show');
            return;
        }

        if (movimientoo == "") {
            document.getElementById("mensaje").innerHTML = "Debe ingresar el nombre del movimiento";
            $("#modalGenerico").modal('show');
            return;
        }

        if (seriee == "") {
            document.getElementById("mensaje").innerHTML = "Debe ingresar la serie";
            $("#modalGenerico").modal('show');
            return;
        }

        $.ajax({
            async: true,
            type: "POST",
            dataType: "JSON",
            contentType: "application/x-www-form-urlencoded",
            url: "/IngresoMovKardex/ModificarMovKardex",
            data: { idd, movimientoo, seriee },
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

}
