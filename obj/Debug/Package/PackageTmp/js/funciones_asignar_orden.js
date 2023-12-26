
$(document).ready(InicionEventos);

function InicionEventos() {

    
    
    ActFiltroDoctos();


}


function openOrden(e) {
    let id = $(e).closest("tr")[0].cells[1].innerText + "|" +"OS";
    
    window.open("/IngresoOrden/AsignaOrden/"+id,"_blank")
   
}

function BuscarImagen(e) {

    let id_codigo = $(e).closest("tr")[0].cells[1].innerText;

    alert(id_codigo);

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoOrden/GetDataImages",
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
function ActFiltroDoctos() {


    var fecha1 = "";
    var fecha2 = "";
    

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/AsignarOrden/GetOrden",
        data: { fecha1, fecha2 },
        beforeSend: InicioConsulta,
        success: ConsultaOrdenes
    });

    function InicioConsulta() {
        $('#mostrar_consulta').html('Cargando Ordenes por favor espere...');
    }

    function ConsultaOrdenes(data) {
        var arreglo = eval(data);
        cData = arreglo;

        var columnas = [
            {
                dataField: "OTROS",
                alignment: "right",
                width: 60,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.append($("<i style='color:#651FFF;cursor:pointer;' class='COMPRAS' onclick='openOrden(this)'> <img src='/images/pdf_download.png' width=\"18\" height=\"auto\"></i><i style='color:#651FFF;cursor:pointer;' class='IMAGES' onclick='BuscarImagen(this)'> <img src='/images/images.png' width=\"18\" height=\"auto\"></i>")).appendTo(container);

                },

            },
            {
                dataField: "ID",
                alignment: "center",
                width: 100,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "FECHA",
                alignment: "center",
                width: 90,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "STATUS",
                alignment: "center",
                width: 150,
                allowFiltering: true,
                allowSorting: false,

            },

            {
                dataField: "REQUIRENTE",
                alignment: "center",
                width: 450,
                allowFiltering: true,
                allowSorting: false,

            },
            {
                dataField: "DEPARTAMENTO",
                alignment: "center",
                width: 100,
                allowFiltering: false,
                allowSorting: false,

            },
            {
                dataField: "EMAIL",
                alignment: "center",
                width: 200,
                allowFiltering: false,
                allowSorting: false,

            },
                        
            {
                dataField: "OBS",
                alignment: "center",
                width: 335,
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
                    fileName: "ConsultaOrdenes",
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

function AutorizarO() {

    var rcp = $("#prospecto_id_vendedor").val();

    if (rcp == "") {
        alert("Debe seleccionar un Receptor");
        return;
    }

    $('#modal-autorizaorden').modal('show');


}

function Autoriza() {


    var id  = $("#orden").val();
    var rcp = $("#prospecto_id_vendedor").val();

    

    if (rcp == "") {
        alert("Debe seleccionar un Receptor");
        return;
    }


    $('#modal-autorizaorden').modal('hide');

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoOrden/AsignarProductoOrden",
        data: { id,rcp },
        success: function (response) {
            var arreglo = response;


            if (response.CODIGO == "ERROR") {

                if (response.PRODUCTO == "") {
                    $('#Error').html("ERROR");
                    $('#ModalError').modal('show');
                    return;
                }
                else {
                    $('#Error').html(response.PRODUCTO);
                    $('#ModalError').modal('show');
                    return;
                }
            }

            document.getElementById("btnAsigna").disabled = true;
            $('#Error').html(response.PRODUCTO);
            $('#ModalError').modal('show');
            


        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


}
