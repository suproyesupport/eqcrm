function crearCaja() {


    var fecha1 = document.getElementById("fecha1").value;
    var fecha2 = document.getElementById("fecha2").value;
    var id_caja = document.getElementById("Nombre").value;


    if (id_caja < 1) {
        alert('Debe seleccionar una caja...');
    }
    else {
        $.ajax({
            async: false,
            type: "POST",
            dataType: "HTML",
            contentType: "application/x-www-form-urlencoded",
            url: "/POS/Operaciones/FiltrarDatos",
            data: { fecha1, fecha2, id_caja },
            beforeSend: InicioConsulta,
            success: ConsultaCortes
        });

        function InicioConsulta() {
            $('#mostrar_consulta').html('Cargando por favor espere...');
        }

        function ConsultaCortes(data) {
            var arreglo = eval(data);
            cData = arreglo;

            console.log(cData);

            var columnas = [
                {
                    dataField: "TIPODOCTO",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,

                },
                {
                    dataField: "NO_DOCTO",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,

                },
                {
                    dataField: "SERIE",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,

                },

                {
                    dataField: "NIT",
                    alignment: "left",
                    width: 150,
                    allowFiltering: true,
                    allowSorting: false,

                },
                {
                    dataField: "OBS",
                    alignment: "left",
                    width: 400,
                    allowFiltering: false,
                    allowSorting: false,

                },
                {
                    dataField: "FECHA",
                    alignment: "left",
                    width: 140,
                    allowFiltering: false,
                    allowSorting: false,

                },

                {
                    dataField: "ENTRADA",
                    alignment: "left",
                    width: 110,
                    allowFiltering: false,
                    allowSorting: false,

                },
                {
                    dataField: "SALIDA",
                    alignment: "left",
                    width: 110,
                    allowFiltering: false,
                    allowSorting: false,

                },
                {
                    dataField: "CAJA",
                    alignment: "right",
                    width: 110,
                    allowFiltering: false,
                    allowSorting: false,

                },
                {
                    dataField: "SALDO",
                    alignment: "left",
                    width: 110,
                    allowFiltering: true,
                    allowSorting: false,
                    selectedFilterOperation: "contains",

                }
            ];




            $(function () {
                var dataGrid = $("#gridContainer").dxDataGrid({

                    dataSource: cData,
                    keyExpr: "TIPODOCTO",
                    selection: {
                        mode: "single"
                    },
                    "export": {
                        enabled: true,
                        fileName: "ConsultaCorteCaja",
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


