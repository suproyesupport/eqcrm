


function GuardarPrecioGAS() {

    $('#modal-guardar').modal('hide');
    
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
        url: "/IngresoPreciosGAS/InsertarPrecioGAS",
        data: { idcodigoeq, producto, costoeq1, precioeq1, costoeq2, precioeq2, idp },
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

            LimpiarCampos();

        },
        error: function () {
            alert("Ocurrio un Error");
        }

    });

}



function ModalGuardar() {

    var idcodigoeq = document.getElementById("idcodigoeq").value;
    var producto = document.getElementById("productoeq").value;
    var costoeq1 = document.getElementById("costoeq1").value;
    var precioeq1 = document.getElementById("precioeq1").value;
    var costoeq2 = document.getElementById("costoeq2").value;
    var precioeq2 = document.getElementById("precioeq2").value;
    var idp = document.getElementById("idp").value;

    if (idcodigoeq == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el código de producto.";
        $("#ModalGenerico").modal('show');
        return;

    }

    if (producto == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar nombre del producto.";
        $("#ModalGenerico").modal('show');
        return;

    }

    if (costoeq1 == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el costo 1.";
        $("#ModalGenerico").modal('show');
        return;

    }

    if (precioeq1 == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el precio 1.";
        $("#ModalGenerico").modal('show');
        return;

    }

    if (costoeq2 == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el costo 2.";
        $("#ModalGenerico").modal('show');
        return;

    }

    if (precioeq2 == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el precio 2.";
        $("#ModalGenerico").modal('show');
        return;

    }

    if (idp == "") {

        document.getElementById("mensaje").innerHTML = "Debe ingresar el IDP.";
        $("#ModalGenerico").modal('show');
        return;

    }


    $('#modal-guardar').modal('show');

}



function LimpiarCampos() {

    document.getElementById("idcodigoeq").value = "";
    document.getElementById("productoeq").value = "";
    document.getElementById("costoeq1").value = "";
    document.getElementById("precioeq1").value = "";
    document.getElementById("costoeq2").value = "";
    document.getElementById("precioeq2").value = "";
    document.getElementById("idp").value = "";

    document.getElementById("mensaje").innerHTML = "Registro ingresado correctamente.";
    $("#ModalGenerico").modal('show');

}
