var n_fila;

$(document).ready(InicionEventos);

function InicionEventos() {
  //  $("#TasaCambio").hide();


    var hoy = new Date();

    var nMes = hoy.getMonth() + 1;
    if (nMes <= 9) {
        cMes = "0" + nMes;
    }
    else {
        cMes = nMes;
    }

    var nDia = hoy.getDate();
    if (nDia <= 9) {
        cDia = "0" + nDia;
    }
    else {
        cDia = nDia;
    }


    document.getElementById("cFecha").value = hoy.getFullYear() + '-' + cMes + '-' + cDia;


    $("#tabladetalle").on("click", ".agrega-info", function () {
        $("#productotable").val($(this).attr("producto"));
        $("#cantidadtable").val($(this).attr("cantidad"));
        $("#preciotable").val($(this).attr("precio"));
        $("#subtotaltable").val($(this).attr("subtotal"));
        n_fila = $(this).attr("fila");
    })
    $("#defaultInline1Radio").click(function () {
        if ($("#defaultInline1Radio").is(':checked')) {
           
            $("#moneda").text("Q");
            $("#moneda2").text("Q");
            
        }
    });

    $("#defaultInline2Radio").click(function () {
        if ($("#defaultInline2Radio").is(':checked')) {
           
            $("#moneda").text("$");
            $("#moneda2").text("$");
        }
    });

    $("#descto").change(function () {
        var cantidad = $("#cantidad").val();
        if (cantidad == "") {
            $('#Error').html("SIN CANTIDAD NO PUEDE APLICARSE NINGUN DESCUENTO");
            $('#ModalError').modal('show');
        }
        else {
            CalcularValores();
        }

    });

    $("#desctotable").change(function () {
        var cantidad = $("#cantidadtable").val();
        if (cantidad == "") {
            $('#Error').html("SIN CANTIDAD NO PUEDE APLICARSE NINGUN DESCUENTO");
            $('#ModalError').modal('show');
        }
        else {
            CalcularValoresTable();
        }

    });

    $("#prospecto_id_vendedor").change(function () {
        var nVende = $("#prospecto_id_vendedor").val();
        if (nVende == "") {
            $('#Error').html("TIENE QUE SELECCIONAR UN VENDEDOR");
            $('#ModalError').modal('show');
        }
        else {
            CalcularValores();
        }

    });

    $("#nTasa").prop("disabled", true);
    $("#subtotal").prop("disabled", true);
    $("#precio").prop("disabled", false);
    $("#tabladatosdocto").hide();
    $("#nDias1").hide();
    $("#diascred").hide();
    $("#subtotaltable").prop("disabled", true);
    $("#preciotable").prop("disabled", true);
    
    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngDoctos/GetTasa",
        success: function (response) {

            var arreglo = response;
            if (response.NTASA == "ERROR") {
                $('#Error').html("ERROR EN LA CONSULTA");
                $('#ModalError').modal('show');
                return;
            }

            $("#nTasa").val(arreglo.NTASA);

            if (response.HABPRECIO == "N") {
                $("#precio").prop("disabled", true);
            }


        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


}

function modifica() {
    var producto = $("#productotable").val();
    var cantidad = $("#cantidadtable").val();
    var precio = $("#preciotable").val();
    var subtotal = $("#subtotaltable").val();
    var descto = $("#desctotable").val();


    $("#tabladetalle td:nth-child(3)").eq(n_fila - 2).html(cantidad);
    $("#tabladetalle td:nth-child(4)").eq(n_fila - 2).html(producto);
    $("#tabladetalle td:nth-child(5)").eq(n_fila - 2).html(precio);
    $("#tabladetalle td:nth-child(6)").eq(n_fila - 2).html(descto);
    $("#tabladetalle td:nth-child(7)").eq(n_fila - 2).html(subtotal);



}

function seleccionar(id) {
    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");

    document.getElementById("id").value = document.getElementById("tabla").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("nombre").value = document.getElementById("tabla").rows[id.rowIndex].cells[1].innerHTML;
    document.getElementById("direccion").value = document.getElementById("tabla").rows[id.rowIndex].cells[2].innerHTML;
    document.getElementById("nit").value = document.getElementById("tabla").rows[id.rowIndex].cells[3].innerHTML;
    document.getElementById("diascred").value = document.getElementById("tabla").rows[id.rowIndex].cells[4].innerHTML;
    document.getElementById("contacto").value = document.getElementById("tabla").rows[id.rowIndex].cells[5].innerHTML;

    $("#ayudaclientes").modal('hide');
}

function seleccionarinventario(id) {
    var Row = document.getElementById("fila");
    var Cells = Row.getElementsByTagName("td");

    document.getElementById("id_codigo").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("codigoe").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[1].innerHTML;
    document.getElementById("producto").value = document.getElementById("tablainventario").rows[id.rowIndex].cells[2].innerHTML;
    document.getElementById("precio").value = parseFloat(document.getElementById("tablainventario").rows[id.rowIndex].cells[3].innerHTML).toFixed(2);

    $("#ayudaproductos").modal('hide');
    BuscarCodigo();
    
    document.getElementById("cantidad").focus();
}
         
function seleccionarcotiz(id) {
    var Row = document.getElementById("fila");
    var Cells = Row.getElementsByTagName("td");
    
    
    $("#tabladatosdocto").show();

    document.getElementById("id").value = document.getElementById("tablacotiz").rows[id.rowIndex].cells[4].innerHTML;
    document.getElementById("nodocto").value = document.getElementById("tablacotiz").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("serie").value = document.getElementById("tablacotiz").rows[id.rowIndex].cells[1].innerHTML;
    
    $("#ayudacotiz").modal('hide');
    BuscarCliente();
    BuscarDetalle();
    
    
}

function BuscarCliente() {
    id = $("#id").val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngDoctos/GetDataCliente",
        data: { id },
        success: function (response) {

            var arreglo = response;
            if (response.NIT == "ERROR") {
                $('#Error').html("EL CLIENTE NO SE ENCUENTRA EN LA BASE DE DATOS");
                $('#ModalError').modal('show');
                return;
            }

            $("#nombre").val(arreglo.CLIENTE);
            $("#direccion").val(arreglo.DIRECCION);
            $("#nit").val(arreglo.NIT);
            $("#diascred").val(arreglo.DIASCRED);
            $("#contacto").val(arreglo.CONTACTO);

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}


function BuscarDetalle() {
    nodocto = $("#nodocto").val();
    serie = $("#serie").val();
    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngDoctos/BuscaDetalleCotizacion",
        data: { nodocto,serie },
        success: function (response) {
            
            for (var i = 0; i < response.length; i++) {

                
                var table = document.getElementById("tabladetalle");

                var d = new Date();
                var idTemporal = create_UUID();// d.getDate() + d.getMonth() + 1 + d.getFullYear() + d.getMinutes() + d.getSeconds() + d.getMilliseconds();

                var row = table.insertRow();

                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);
                var cell3 = row.insertCell(2);
                var cell4 = row.insertCell(3);
                var cell5 = row.insertCell(4);
                var cell6 = row.insertCell(5);
                var cell7 = row.insertCell(6);
                var cell8 = row.insertCell(7);
                var cell9 = row.insertCell(8);
                var fila = $("#tabladetalle tr").length;
                file = fila - 1;


                cell1.innerHTML = response[i].id_codigo;
                cell2.innerHTML = response[i].codigoe;
                cell3.innerHTML = response[i].cantidad;
                cell4.innerHTML = response[i].producto;
               // cell4.setAttribute('contentEditable', 'true');

                cell5.innerHTML = response[i].precio;
                cell6.innerHTML = response[i].descto;
                cell7.innerHTML = response[i].subtotal;
                var agregarArchivo = '<i class="fas fa-upload" onclick="openUploadPicture(this)"> </i> <i class="fas fa-trash" style="color:red;" onclick="DeleteDetalle(this)"> </i><i class="fas fa-sync agrega-info" style="color:green;" data-toggle="modal" data-target="#editar-detalle" correlativo="' + response[i].id_codigo + '" subtotal="' + response[i].subtotal + '" producto="' + response[i].producto + '" cantidad="' + response[i].cantidad + '" precio="' + response[i].precio + '" fila="' + fila + '" > </i>'; //AGREGA EL ICONO DE SUBIR
                cell8.innerHTML = agregarArchivo;
                cell9.id = idTemporal; //ASIGNA AL ID DEL <i> EL IDTEMPORAL PARA SUBIR

                //alert("hola" + i);

                

                //document.getElementById("idTemporal").value = "";
                //document.getElementById("codigo").focus();
                //AddDetalle();
            }
            var arreglo = response;
            
            
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}


function BuscarCoti() {
    id = $("#id").val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngDoctos/GetDataCliente",
        data: { id },
        success: function (response) {

            var arreglo = response;
            if (response.NIT == "ERROR") {
                $('#Error').html("EL CLIENTE NO SE ENCUENTRA EN LA BASE DE DATOS");
                $('#ModalError').modal('show');
                return;
            }

            $("#nombre").val(arreglo.CLIENTE);
            $("#direccion").val(arreglo.DIRECCION);
            $("#nit").val(arreglo.NIT);
            $("#diascred").val(arreglo.DIASCRED);
            $("#contacto").val(arreglo.CONTACTO);

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}


function BuscarCodigo() {
    id = $("#id_codigo").val();
    nTasa = $("#nTasa").val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngDoctos/GetDataInventario",
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

            var nPrecio = 0;
            nPrecio = arreglo.PRECIO;

            $("#producto").val(arreglo.PRODUCTO);
            //alert(arreglo.DOLAR);

            
            if ($("#defaultInline2Radio").is(':checked'))
            {
                
                if (arreglo.DOLAR == "S") {                    
                    $("#precio").val(formatNumber.new(parseFloat(nPrecio).toFixed(2)));
                }
                else {
                    nPrecio = nPrecio / nTasa;
                    $("#precio").val(formatNumber.new(parseFloat(nPrecio).toFixed(2)));
                }
            }
            
            if ($("#defaultInline1Radio").is(':checked')) {
                if (arreglo.DOLAR == "S") {
                    nPrecio = nPrecio * nTasa;
                    $("#precio").val(formatNumber.new(parseFloat(nPrecio).toFixed(2)));
                }
                else {
                    
                    $("#precio").val(formatNumber.new(parseFloat(arreglo.PRECIO).toFixed(2)));
                }
            }

           
            $("#codigoe").val(arreglo.CODIGOE);
            document.getElementById("cantidad").focus();
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}

function CalcularValores() {
    var nTotal = 0;
    var Subtotal = 0;
    var nPorcentajePrecio = 0;
    var nPrecioCP = 0;
    var cantidad = $("#cantidad").val();
    var precio = $("#precio").val();
    var subtotal = $("#subtotal").val();
    var descto = $("#descto").val();

    precio = precio.replace(",", "");


    if (descto >= 1) {
        nPorcentajePrecio = precio * (descto / 100);
        nPrecioCP = (precio - nPorcentajePrecio);
        // document.getElementById("precio").value = formatNumber.new(parseFloat(nPrecioCP).toFixed(2));
        nTotal = cantidad * nPrecioCP;
        subtotal = nTotal;

    }
    else {
        nTotal = cantidad * precio;
        subtotal = nTotal;

    }


    document.getElementById("subtotal").value = formatNumber.new(parseFloat(subtotal).toFixed(2));
    document.getElementById("descto").focus();
}


function CalcularValoresTable() {
    var nTotal = 0;
    var Subtotal = 0;
    var nPorcentajePrecio = 0;
    var nPrecioCP = 0;
    var cantidad = $("#cantidadtable").val();
    var precio = $("#preciotable").val();
    var subtotal = $("#subtotaltable").val();
    var descto = $("#desctotable").val();

    precio = precio.replace(",", "");

    

    if (descto >= 1) {
        nPorcentajePrecio = precio * (descto / 100);
        nPrecioCP = (precio - nPorcentajePrecio);
        // document.getElementById("precio").value = formatNumber.new(parseFloat(nPrecioCP).toFixed(2));
        nTotal = cantidad * nPrecioCP;
        subtotal = nTotal;

    }
    else {
        nTotal = cantidad * precio;
        subtotal = nTotal;

    }


    document.getElementById("subtotaltable").value = formatNumber.new(parseFloat(subtotal).toFixed(2));
    document.getElementById("desctotable").focus();
}


function CalcularValoresConDecto() {
    var nTotal = 0;
    var Subtotal = 0;

    var cantidad = $("#cantidad").val();
    var precio = $("#precio").val();
    var subtotal = $("#subtotal").val();
    var descto = $("#descto").val();

    nTotal = cantidad * precio;
    subtotal = nTotal - descto;

    document.getElementById("subtotal").value = formatNumber.new(parseFloat(subtotal).toFixed(2));
    document.getElementById("subtotal").focus();
}

//CREAR UN ID UNICO PARA NOMBRE DEL ARCHIVO.
function create_UUID() {
    var dt = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (dt + Math.random() * 16) % 16 | 0;
        dt = Math.floor(dt / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
}

function AddDetalle() {
    var table = document.getElementById("tabladetalle");

    var d = new Date();
    var idTemporal = create_UUID();// d.getDate() + d.getMonth() + 1 + d.getFullYear() + d.getMinutes() + d.getSeconds() + d.getMilliseconds();

    var row = table.insertRow();
    
    var cell1 = row.insertCell(0);
    var cell2 = row.insertCell(1);
    var cell3 = row.insertCell(2);
    var cell4 = row.insertCell(3);
    var cell5 = row.insertCell(4);
    var cell6 = row.insertCell(5);
    var cell7 = row.insertCell(6);
    var cell8 = row.insertCell(7);
    var cell9 = row.insertCell(8);
    var fila = $("#tabladetalle tr").length;
    file = fila - 1;

    cell1.innerHTML = $("#id_codigo").val();
    cell2.innerHTML = $("#codigoe").val();
    cell3.innerHTML = $("#cantidad").val();    
    cell4.innerHTML = $("#producto").val();
    //cell4.setAttribute('contentEditable', 'true');


    cell5.innerHTML = formatNumber.new($("#precio").val());
    cell6.innerHTML = formatNumber.new($("#descto").val());
    cell7.innerHTML = formatNumber.new($("#subtotal").val());
    var agregarArchivo = '<i class="fas fa-upload" onclick="openUploadPicture(this)"> </i> <i class="fas fa-trash" style="color:red;" onclick="DeleteDetalle(this)"> </i><i class="fas fa-sync agrega-info" style="color:green;" data-toggle="modal" data-target="#editar-detalle" correlativo="' + $("#id_codigo").val() + '" subtotal="' + $("#subtotal").val() + '" producto="' + $("#producto").val() + '" cantidad="' + $("#cantidad").val() + '" precio="' + $("#precio").val() + '" fila="' + fila  + '" > </i>'; //AGREGA EL ICONO DE SUBIR
    cell8.innerHTML = agregarArchivo;
    cell9.id = idTemporal; //ASIGNA AL ID DEL <i> EL IDTEMPORAL PARA SUBIR
    
    //alert("Hola");

    document.getElementById("id_codigo").value = "";
    document.getElementById("codigoe").value = "";
    document.getElementById("producto").value = "";
    document.getElementById("cantidad").value = "";
    document.getElementById("precio").value = "";
    document.getElementById("descto").value = "";
    document.getElementById("subtotal").value = "";
    document.getElementById("idTemporal").value = "";
    document.getElementById("codigo").focus();
}

//function DeleteDetalle() {
//    document.getElementById("tabladetalle").deleteRow(1);
//}


function DeleteDetalle(element) {
    let seElimina = confirm("¿Seguro que desea eliminar el articulo?");
    if (seElimina == true) {
        $(element).closest("tr").remove();
    }
}

function ObtieneInfoTabla() {

    var tabla = $('#tabladetalle tr').length;   //cuenta la cantidad de filas en la tabla
    var xtabla = '~DETALLES}';
    var xTablaE = "~EQDOCUMENTO}~ENCABEZADO}"
    var cXml = "";
    var cDolar = "N";

    if ($("#defaultInline2Radio").is(':checked'))
    {
        cDolar = "S";
    }

    xTablaE = xTablaE + '~TIPODOCTO}' + $("#prospecto_cTipoDocto").val() + '~|TIPODOCTO}';
    xTablaE = xTablaE + '~FECHADOCTO}' + $("#cFecha").val() + '~|FECHADOCTO}';
    xTablaE = xTablaE + '~CODIGOCLIENTE}' + $("#id").val() + '~|CODIGOCLIENTE}';
    xTablaE = xTablaE + '~NOMBRECLIENTE}' + $("#nombre").val() + '~|NOMBRECLIENTE}';
    xTablaE = xTablaE + '~DIRECCIONCLIENTE}' + $("#direccion").val() + '~|DIRECCIONCLIENTE}';
    xTablaE = xTablaE + '~NITCLIENTE}' + $("#nit").val() + '~|NITCLIENTE}';
    xTablaE = xTablaE + '~VENDEDOR}' + $("#prospecto_id_vendedor").val() + '~|VENDEDOR}';
    xTablaE = xTablaE + '~ENCOBS}' + $("#EncObs").val() + '~|ENCOBS}';
    xTablaE = xTablaE + '~TASA}' + $("#nTasa").val() + '~|TASA}';
    xTablaE = xTablaE + '~CONTACTO}' + $("#contacto").val() + '~|CONTACTO}';
    xTablaE = xTablaE + '~ENDOLAR}' + cDolar + '~|ENDOLAR}';
    xTablaE = xTablaE + '~DIAS}' + $("#diascred").val() + '~|DIAS}~|ENCABEZADO}';



    for (var n = 1; n < tabla; n++) {

        var valor = parseInt(n) + 1;
        var fila = $("#tabladetalle tr:nth-child(" + valor + ")").html();  //recupera desde la segunda fila de la tabla


        var datos = fila.replace(/<td>/g, '~');
        datos = datos.replace(/[/]/g, '');
        datos = datos.replace(/<td>/g, '');

        var campos = datos.split('~');
        var nombre = "";//campos[8];

        //OBTIENE LOS CARACTERES DEL 71 al 127 Y SE ARMA LA URL DE LA FOTO PARA ALMACENARLO A LA BASE DE DATOS
        var res1 = "";//nombre.substring(71, 127);
        var pathImages = "";//res1.substring(0, 6);
        var pathCarpeta = "";// res1.substring(6, 15);
        var pathArchivo = "";//res1.substring(15, 55);

        var pathFinal = "";//("/" + pathImages + "/" + pathCarpeta + "/" + pathArchivo);

        console.log(pathFinal);

        xtabla = xtabla + '~DETALLE}~CODIGO}' + campos[1] + '~|CODIGO}';
        xtabla = xtabla + '~CODIGOE}' + campos[2] + '~|CODIGOE}';
        xtabla = xtabla + '~CANTIDAD}' + campos[3] + '~|CANTIDAD}';
        xtabla = xtabla + '~PRODUCTO}' + campos[4] + '~|PRODUCTO}';
        xtabla = xtabla + '~PRECIO}' + campos[5] + '~|PRECIO}';
        xtabla = xtabla + '~DESCTO}' + campos[6] + '~|DESCTO}';
        xtabla = xtabla + '~SUBTOTAL}' + campos[7] + '~|SUBTOTAL}';
        xtabla = xtabla + '~FOTO}' + pathFinal + '~|FOTO}~|DETALLE}';

    }
    xtabla = xtabla + '~|DETALLES}~|EQDOCUMENTO}'
    cXml = xTablaE + xtabla;

    
    return (cXml);

}

function openUploadPicture(element) {
    var tempTr = $(element).closest("tr")[0].cells;
    var idUploadPicture = tempTr[8].id;
    $("#nameFileUpload").val(idUploadPicture);
    $("#subirArchivos").modal("show");
}

//SE CREA LA FUNCION PARA ALMACENAMIENTO DE LA FOTO EN EL CONTROLADOR
function SavePhoto() {
    if (window.FormData == undefined)
        alert("Error: FormData is undefined");

    else {
        id = $("#nameFileUpload").val();
        var fileUpload = $("#fileToUpload").get(0);
        var files = fileUpload.files;

        var fileData = new FormData();

        fileData.append(files[0].name, files[0]);

        $.ajax({
            url: '/IngDoctos/uploadFile/' + id,
            type: 'post',
            datatype: 'json',
            contentType: false,
            processData: false,
            async: false,
            data: fileData,
            success: function (response) {
                alert("Archivo cargado con exito");
                var campo = document.getElementById(id);
                campo.innerHTML = "<img src=\"" + response.toString() + "\" alt=\"Archivo no visible\" width=\"50\" height=\"auto\">";
                campo.id = response.toString();
            }
        });
    }

}

function SaveDataPost() {
    cdetalle = ObtieneInfoTabla();
    //enviamos un arreglo de la cantidad de filas que existan

    if ($("#contacto").val() == "") {
        alert("Los datos de contacto estan vacios");
        return false;

    }

   
   

    console.log(cdetalle);
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngDoctos/InsertarDocumento",
        data: { cdetalle },
        success: function (response) {
            $('#Error').html(response);
            $('#ModalError').modal('show');

            document.getElementById("id").value = "";
            document.getElementById("nombre").value = "";
            document.getElementById("direccion").value = "";
            document.getElementById("nit").value = "";
            document.getElementById("diascred").value = "";

            var tableHeaderRowCount = 1;
            var table = document.getElementById('tabladetalle');
            var rowCount = table.rows.length;
            for (var i = tableHeaderRowCount; i < rowCount; i++) {
                table.deleteRow(tableHeaderRowCount);
            }

            //OBTIENE EL VALOR DE LA COTIZACION PARA ENVIAR A IMPRIMIR
            var idCotizacion = response.toString().substring(40);
            var urlVerCotizacion = "/ConsultarCotizaciones/Vercotizacion/" + idCotizacion;

            let seImprime = confirm("¿Desea imprimir la cotizacion?");

            if (seImprime == true) {
                window.open(urlVerCotizacion);
            }



        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}

function SaveDataEditPost() {
    cdetalle = ObtieneInfoTablaMOD();
    //enviamos un arreglo de la cantidad de filas que existan
    //alert(cdetalle);

    if ($("#contacto").val() == "") {
        alert("Los datos de contacto estan vacios");
        return false;

    }

   

    console.log(cdetalle);
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngDoctos/ModificarDocumento",
        data: { cdetalle },
        success: function (response) {
            $('#Error').html(response);
            $('#ModalError').modal('show');

            document.getElementById("id").value = "";
            document.getElementById("nombre").value = "";
            document.getElementById("direccion").value = "";
            document.getElementById("nit").value = "";
            document.getElementById("diascred").value = "";

            var tableHeaderRowCount = 1;
            var table = document.getElementById('tabladetalle');
            var rowCount = table.rows.length;
            for (var i = tableHeaderRowCount; i < rowCount; i++) {
                table.deleteRow(tableHeaderRowCount);
            }

            //OBTIENE EL VALOR DE LA COTIZACION PARA ENVIAR A IMPRIMIR
            var idCotizacion = response.toString().substring(40);
            var urlVerCotizacion = "/ConsultarCotizaciones/Vercotizacion/" + idCotizacion;

            let seImprime = confirm("¿Desea imprimir la cotizacion?");

            if (seImprime == true) {
                window.open(urlVerCotizacion);
            }



        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}
function ObtieneInfoTablaMOD() {

    var tabla = $('#tabladetalle tr').length;   //cuenta la cantidad de filas en la tabla
    var xtabla = '~DETALLES}';
    var xTablaE = "~EQDOCUMENTO}~ENCABEZADO}"
    var cXml = "";
    var cDolar = "N";

    if ($("#defaultInline2Radio").is(':checked')) {
        cDolar = "S";
    }

    xTablaE = xTablaE + '~TIPODOCTO}' + $("#prospecto_cTipoDocto").val() + '~|TIPODOCTO}';
    xTablaE = xTablaE + '~DOCTO}' + $("#nodocto").val() + '~|DOCTO}';
    xTablaE = xTablaE + '~SERIE}' + $("#serie").val() + '~|SERIE}';
    xTablaE = xTablaE + '~FECHADOCTO}' + $("#cFecha").val() + '~|FECHADOCTO}';
    xTablaE = xTablaE + '~CODIGOCLIENTE}' + $("#id").val() + '~|CODIGOCLIENTE}';
    xTablaE = xTablaE + '~NOMBRECLIENTE}' + $("#nombre").val() + '~|NOMBRECLIENTE}';
    xTablaE = xTablaE + '~DIRECCIONCLIENTE}' + $("#direccion").val() + '~|DIRECCIONCLIENTE}';
    xTablaE = xTablaE + '~NITCLIENTE}' + $("#nit").val() + '~|NITCLIENTE}';
    xTablaE = xTablaE + '~VENDEDOR}' + $("#prospecto_id_vendedor").val() + '~|VENDEDOR}';
    xTablaE = xTablaE + '~ENCOBS}' + $("#EncObs").val() + '~|ENCOBS}';
    xTablaE = xTablaE + '~TASA}' + $("#nTasa").val() + '~|TASA}';
    xTablaE = xTablaE + '~CONTACTO}' + $("#contacto").val() + '~|CONTACTO}';
    xTablaE = xTablaE + '~ENDOLAR}' + cDolar + '~|ENDOLAR}';
    xTablaE = xTablaE + '~DIAS}' + $("#diascred").val() + '~|DIAS}~|ENCABEZADO}';



    for (var n = 1; n < tabla; n++) {

        var valor = parseInt(n) + 1;
        var fila = $("#tabladetalle tr:nth-child(" + valor + ")").html();  //recupera desde la segunda fila de la tabla


        var datos = fila.replace(/<td>/g, '~');
        datos = datos.replace(/[/]/g, '');
        datos = datos.replace(/<td>/g, '');

        var campos = datos.split('~');
        var nombre = "";//campos[8];

        //OBTIENE LOS CARACTERES DEL 71 al 127 Y SE ARMA LA URL DE LA FOTO PARA ALMACENARLO A LA BASE DE DATOS
        var res1 = "";//nombre.substring(71, 127);
        var pathImages = "";//res1.substring(0, 6);
        var pathCarpeta = "";// res1.substring(6, 15);
        var pathArchivo = "";//res1.substring(15, 55);

        var pathFinal = "";//("/" + pathImages + "/" + pathCarpeta + "/" + pathArchivo);


        console.log(pathFinal);

        xtabla = xtabla + '~DETALLE}~CODIGO}' + campos[1] + '~|CODIGO}';
        xtabla = xtabla + '~CODIGOE}' + campos[2] + '~|CODIGOE}';
        xtabla = xtabla + '~CANTIDAD}' + campos[3] + '~|CANTIDAD}';
        xtabla = xtabla + '~PRODUCTO}' + campos[4] + '~|PRODUCTO}';
        xtabla = xtabla + '~PRECIO}' + campos[5] + '~|PRECIO}';
        xtabla = xtabla + '~DESCTO}' + campos[6] + '~|DESCTO}';
        xtabla = xtabla + '~SUBTOTAL}' + campos[7] + '~|SUBTOTAL}';
        xtabla = xtabla + '~FOTO}' + pathFinal + '~|FOTO}~|DETALLE}';

    }
    xtabla = xtabla + '~|DETALLES}~|EQDOCUMENTO}'
    cXml = xTablaE + xtabla;

    //alert(cXml);
    return (cXml);

}



var formatNumber = {
    separador: ",",
    sepDecimal: '.',
    formatear: function (num) {
        num += '';
        var splitStr = num.split('.');
        var splitLeft = splitStr[0];
        var splitRight = splitStr.length > 1 ? this.sepDecimal + splitStr[1] : '';
        var regx = /(\d+)(\d{3})/;
        while (regx.test(splitLeft)) {
            splitLeft = splitLeft.replace(regx, '$1' + this.separador + '$2');
        }
        return this.simbol + splitLeft + splitRight;
    },
    new: function (num, simbol) {
        this.simbol = simbol || '';
        return this.formatear(num);
    }
}