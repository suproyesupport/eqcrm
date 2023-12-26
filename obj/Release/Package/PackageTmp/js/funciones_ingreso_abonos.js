


function seleccionar(id) {
    var Row = document.getElementById("fila")
    var Cells = Row.getElementsByTagName("td");

    document.getElementById("idcliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[0].innerHTML;
    document.getElementById("cliente").value = document.getElementById("tablapdf").rows[id.rowIndex].cells[1].innerHTML;
    //alert(document.getElementById("tablapdf").rows[id.rowIndex].cells[0].innerHTML);
    $("#ayudaclientes").modal('hide');
    LlenarTablaCtacc();
}





function seleccionarEdicion(id) {

    recorrerAbonoMenorSaldo();
    /*recorrer();*/
    
}


function LlenarTablaCtacc() {

    var idcliente = document.getElementById("idcliente").value;

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoAbonos/ConsultaCtcca",
        data: { idcliente },
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



function verificarCampos() {

    var idcliente = document.getElementById("idcliente").value;
    var codabono = document.getElementById("codabono").value; //COD ABONO
    var fechaa = document.getElementById("fecha").value;
    var fechai = document.getElementById("fecha").value;
    var nodocumento = document.getElementById("nodocumento").value;
    var serie = document.getElementById("serie").value;
    var cobradores = document.getElementById("cobradores").value;
    var formapago = document.getElementById("formapago").value;
    var concepto = document.getElementById("concepto").value;

    if (idcliente == "" || codabono == "" || fechaa == "" || nodocumento == "" || serie == "" || cobradores == "" || formapago == "") {
        $("#modal-faltacampos").modal('show');
    } else {
        recorrerIngresoAbono();
    }
}



function recorrerIngresoAbono() {

    var items = [];

    $("#tablacxc tr").each(function () {
        var itemAbonos = {};

        var tds = $(this).find("td");

        itemAbonos.docto = tds.filter(":eq(0)").text();
        itemAbonos.serie = tds.filter(":eq(1)").text();
        itemAbonos.tipodocto = tds.filter(":eq(2)").text();
        itemAbonos.fechaa = tds.filter(":eq(3)").text();
        itemAbonos.importeoriginal = tds.filter(":eq(4)").text();
        itemAbonos.saldoactual = tds.filter(":eq(5)").text();
        itemAbonos.abono = tds.filter(":eq(6)").text();
        items.push(itemAbonos);

    });

    var abono = {};
    abono = items;
    var informacion = JSON.stringify(abono);

    //Obtener text del dropdownlist
    var sel = document.getElementById("codabono");
    var tipodocto = sel.options[sel.selectedIndex].text;

    var idcliente = document.getElementById("idcliente").value;
    var codabono = document.getElementById("codabono").value; //COD ABONO
    var fechaa = document.getElementById("fecha").value;
    var fechai = document.getElementById("fecha").value;
    var nodocumento = document.getElementById("nodocumento").value;
    var serie = document.getElementById("serie").value;
    var cobradores = document.getElementById("cobradores").value;
    var formapago = document.getElementById("formapago").value;
    var concepto = document.getElementById("concepto").value;

        $.ajax({
            async: true,
            type: "POST",
            dataType: "JSON",
            contentType: "application/x-www-form-urlencoded",
            data: { informacion, idcliente, codabono, fechaa, fechai, nodocumento, serie, cobradores, concepto, tipodocto, formapago },
            url: "/IngresoAbonos/AbonarCuentasxCobrar",

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

        window.location.href = "/IngresoAbonos/IngresoAbonos";

}

var letras = "abcdefghyjklmnñopqrstuvwxyz/*-+¡!/()=?{}[]";

function tiene_letras(texto) {
    var bool = false;
    texto = texto.toLowerCase();
    for (i = 0; i < texto.length; i++) {
        if (letras.indexOf(texto.charAt(i), 0) != -1) {
            $("#modal-comprobarcaracter").modal('show');
            bool = true;
        }
    }
    return bool;
}



// -- RECORREMOS LA TABLA, COMPROBACION ABONO <= SALDO
function recorrerAbonoMenorSaldo() {

    var labelTotal = document.getElementById("total");
    var buttonguardar = document.getElementById("guardarbutton");
    var total = 0;

    var items = [];

    $("#tablacxc tr").each(function () {

        var itemAbonos = {};
  
        var tds = $(this).find("td");

        var nImp = parseFloat(tds.filter(":eq(5)").text());
        var nAbo = parseFloat(tds.filter(":eq(6)").text());

        itemAbonos.docto = tds.filter(":eq(0)").text();
        itemAbonos.serie = tds.filter(":eq(1)").text();
        itemAbonos.tipodocto = tds.filter(":eq(2)").text();
        itemAbonos.fechaa = tds.filter(":eq(3)").text();
        itemAbonos.importeoriginal = tds.filter(":eq(4)").text();
        itemAbonos.saldoactual = tds.filter(":eq(5)").text();
        itemAbonos.abono = tds.filter(":eq(6)").text();
        items.push(itemAbonos);

        //Corrige el dato del total para que pueda realizar la suma
        if (itemAbonos.abono == null || itemAbonos.abono == "") {
            itemAbonos.abono = 0.00;      
        }

        /*tiene_letras(tds.filter(":eq(6)").text());*/

        if (tiene_letras(tds.filter(":eq(6)").text()) == true) {
            tds.filter(":eq(6)").html('0.00');
            itemAbonos.abono = 0.00;
        }

        if (nAbo > nImp) {
            $("#modal-saldomenorabono").modal('show');
            //Si el abono en la celda es mayor al saldo colocamos el valor de 0.00
            tds.filter(":eq(6)").html('0.00');

        } else {
            //Calculo del total abonado
            parseFloat(itemAbonos.abono);
            total = (parseFloat(total) + parseFloat(itemAbonos.abono)).toFixed(2);
            labelTotal.innerHTML = "<strong>Total:</strong> Q" + total;
            buttonguardar.disabled = false;
        }

    });
}



function BuscarCliente() {
    idcliente = $("#idcliente").val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoAbonos/GetDataCliente",
        data: { idcliente },
        success: function (response) {

            var arreglo = response;
            if (response.CODIGO == "ERROR") {

                if (response.PRODUCTO == "") {
                    $('#Error').html("EL CODIGO DE CLIENTE NO SE ENCUENTRA EN LA BASE DE DATOS");
                    $('#ModalError').modal('show');
                    return;
                }
                else {
                    $('#Error').html(response.PRODUCTO);
                    $('#ModalError').modal('show');
                    return;
                }
            }

            nPrecio = arreglo.CLIENTE;

            $("#cliente").val(arreglo.CLIENTE);
            // $("#precio").text(arreglo.PRECIO);
            //document.getElementById("cantidad").focus();
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}




























