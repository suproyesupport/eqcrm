function ActFiltro(){   
    fecha1 = $("input:text[name=fech1]").val(); //d
    fecha2 = $("input:text[name=fech2]").val();
    var lEnvio = "";
    var lMonetario = "";

    var lCheckEnvio = document.getElementById('defaultInline1');
    var lCheckMon = document.getElementById('defaultInline2');

    
    if (lCheckEnvio.checked == true) {
        lEnvio = "Si";
    }
    else {
        lEnvio = "No";
    }

    if (lCheckMon.checked == true) {
        lMonetario = "Si";
    }
    else {
        lMonetario = "No";
    }

    if (fecha1 == "") {
        $('#Error').html("No ha ingresado la Fecha 1 ");
        $('#ModalError').modal('show');
        return;
    }


    if (fecha2 == "") {
        $('#Error').html("No ha ingresado la Fecha 2 ");
        $('#ModalError').modal('show');
        return;
    }

    fecha1 = formatDate(fecha1)
    fecha2 = formatDate(fecha2)  

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/AbcProductos/GetData",
        data: { fecha1, fecha2, lEnvio,lMonetario },
        beforeSend: InicioAbc,
        success: EnvioAbc
    });

    function InicioAbc(){
        $('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function EnvioAbc(data){
        var arreglo = eval(data);
        cData = arreglo;       
        
       
        var columnas = ["CODIGO", "CODIGOE",
            {
                dataField: "PRODUCTO",
                width: 400,
                allowFiltering: false,
                allowSorting: false,               
            }, "LINEA", "MARCA", "MEDIDA", "LINEAP", "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"];

        
        $(function(){
            var dataGrid = $("#gridContainer").dxDataGrid({
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
                onValueChanged: function(data) {
                    dataGrid.option("grouping.autoExpandAll", data.value);
                }
            });
        });

        $('#mostrar_consulta').html('');
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