
function BuscarNitProv() {
    nit = $("#nit").val();

    $.ajax({
        async: false,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/GetDataFel/GetDataNitProv",
        data: { nit },
        success: function (response) {
            var arreglo = response;
            if (arreglo.DIRECCION == "ERROR ESTE PROVEEDOR YA APARECE REGISTRADO") {

                $('#Error').html("EL PROVEEDOR " + arreglo.NOMBRE + " YA SE ENCUENTRA REGISTRADO POR FAVOR NO SEGUIR LLENANDO LA FICHA");
                $('#ModalError').modal('show');


                return;
            }
            else {
                document.getElementById("cliente").value = arreglo.NOMBRE;
                document.getElementById("direccion").value = arreglo.DIRECCION;
            }



        },
        error: function () {
            $('#Error').html("OCURRIÓ UN ERROR");
            $('#ModalError').modal('show');

        }
    });

}


function BuscarNit() {
    nit = $("#nit").val();

    $.ajax({
        async: false,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/GetDataFel/GetDataNit",
        data: { nit },
        success: function (response) {
            var arreglo = response;
            if (arreglo.DIRECCION == "ERROR ESTE CLIENTE YA APARECE REGISTRADO") {

                $('#Error').html("EL CLIENTE " + arreglo.NOMBRE + " YA SE ENCUENTRA REGISTRADO POR FAVOR NO SEGUIR LLENANDO LA FICHA");
                $('#ModalError').modal('show');


                return;
            }
            else {
                document.getElementById("cliente").value = arreglo.NOMBRE;
                document.getElementById("direccion").value = arreglo.DIRECCION;
            }



        },
        error: function () {
            $('#Error').html("OCURRIÓ UN ERROR");
            $('#ModalError').modal('show');

        }
    });

}

