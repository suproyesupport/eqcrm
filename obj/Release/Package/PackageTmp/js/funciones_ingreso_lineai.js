

function GuardarLineaI()
{
    let inp_idlinea = document.getElementById("inp_idlinea").value;
    let inp_descripcion = document.getElementById("inp_descripcion").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/LineaInventario/Guardar",
        data: { inp_idlinea, inp_descripcion },
        success: function (response) {

            var arreglo = response;

            if (response.CODIGO == "ERROR") {

                if (response.PRODUCTO == "") {
                    $('#Error').html("LA LINEA NO SE ENCUENTRA EN LA BASE DE DATOS");
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



//LlenarTablaLineaI();