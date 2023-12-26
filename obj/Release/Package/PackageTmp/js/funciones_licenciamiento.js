$(document).ready(InicionEventos);

function InicionEventos() {

    //$('#gridContainer').on('click', '.INV', BuscarFichaLicencias);
    //$('#gridContainer').on('click', '.EXIS', BuscarExistencias);
    //$('#gridContainer').on('click', '.IMGINV', BuscarImagen);
    $('#gridContainer').on('click', '.FICHLIC', BuscarFichaLicencias);
/*    $('#gridContainer').on('click', '.ELI', EliminaCodigo);*/

}

function BuscarCliente() {

    nit = $("#nit").val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Licenciamiento/GetDataCliente",
        data: { nit },
        success: function (response) {

            var arreglo = response;
            if (response.NIT == "ERROR") {
                $('#Error').html("EL CLIENTE NO SE ENCUENTRA EN LA BASE DE DATOS");
                $('#ModalError').modal('show');
                return;
            }

            $("#nombre").val(arreglo.CLIENTE);
            /*$("#direccion").val(arreglo.DIRECCION);*/
            $("#nit").val(arreglo.NIT);
            /*$("#diascredito").val(arreglo.DIASCRED);*/
            $("#email").val(arreglo.CORREO);

            document.getElementById("id_codigo").focus();

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}


function GuardarLicencia() {

    //$('#modal-guardainv').modal('hide');

    var nit = document.getElementById("nit").value;
    var nombre = document.getElementById("nombre").value;
    var nombrec = document.getElementById("nombrec").value;
    var email = document.getElementById("email").value;
    var fechavencimiento = document.getElementById("fechavencimiento").value;

    var idintercompany = document.getElementById("intercompany");
    var intercompany = idintercompany.options[idintercompany.selectedIndex].text;

    var fel = document.getElementById("fel").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Licenciamiento/InsertarLicencia",
        data: { nit, nombre, nombrec, email, fechavencimiento, intercompany, fel },
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

    window.location.href = "/Licenciamiento/Licenciamiento";
}




function ActFiltroLincencias() {

    nitb = document.getElementById("nitb").value;
    nombrecb = document.getElementById("nombrecb").value;

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/Licenciamiento/GetLicencias",
        data: { nitb, nombrecb },
        beforeSend: InicioLicencias,
        success: ConsultaLicencias
    });

    function InicioLicencias() {
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ConsultaLicencias(data) {
        var arreglo = eval(data);
        cData = arreglo;

        var columnas = [
            {
                dataField: "DATOS",
                alignment: "right",
                width: 60,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.append($("<table class=\"table table-bordered m-0\"><tr><td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='FICHLIC' id_empresa='" + options.value + "'><img src='/images/Load.PNG'></a></td></table >")).appendTo(container);
                    //container.append($("<td><a data-toggle='modal'  style='color:#651FFF;cursor:pointer;' data-target='.VER_INVENTARIO' class='EXIS' id='" + options.value + "'><img src='/images/agencias.PNG'></a></td></tr></table>")).appendTo(container);
                }
            },
            {
                dataField: "CODIGO",
                width: 70,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "EMPRESA",
                width: 150,
                allowFiltering: false,
                allowSorting: false,
            },
            {
                dataField: "COMERCIAL",
                width: 250,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "NIT",
                alignment: "right",
                width: 100,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "AUTORIZACION",
                alignment: "right",
                width: 350,
                allowFiltering: true,
                allowSorting: false,
            },
            {
                dataField: "VENCIMIENTO",
                alignment: "right",
                width: 110,
                allowFiltering: false,
                allowSorting: false,
            },
            ,
            {
                dataField: "INTERCOMPANY",
                alignment: "right",
                width: 110,
                allowFiltering: false,
                allowSorting: false,

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
                    fileName: "Empresa",
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






function BuscarFichaLicencias() {

    var id_empresa = $(this).attr('id_empresa');

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Licenciamiento/GetDataLicencias",
        data: { id_empresa },
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

            $("#nitm").val(response.NIT);
            $("#nombrem").val(arreglo.EMPRESA);
            $("#nombrecm").val(arreglo.COMERCIAL);
            $("#fechavencimientom").val(arreglo.VENCIMIENTO);
            $('#ModalFichaLicencia').modal('show');
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}









function ActualizaLicencia() {

    $('#ModalFichaLicencia').modal('hide');

    var nit = document.getElementById("nitm").value;
    var nombre = document.getElementById("nombrem").value;
    var nombrec = document.getElementById("nombrecm").value;
    var fechavencimiento = document.getElementById("fechavencimientom").value;
    var fel = document.getElementById("felm").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Licenciamiento/ModificarLicencia",
        data: { nit, nombre, nombrec, fechavencimiento, fel },
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