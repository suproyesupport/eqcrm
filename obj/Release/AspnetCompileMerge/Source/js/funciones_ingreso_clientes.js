//GuadarProv 
function ModalGuardarCliente() {
    $('#modal-guardaprov').modal('show');
}

//GuadarProveedor
function GuadarCliente() {

    $('#modal-guardaprov').modal('hide');

    var nit = document.getElementById("nit").value;
    var cliente = document.getElementById("cliente").value;
    var direccion = document.getElementById("direccion").value;

    var id_vendedor = document.getElementById("prospecto_id_vendedor").value;
    //var selectedtecnico = comboid_vendedor.options[comboid_vendedor.selectedIndex].value;

    var tipo = document.getElementById("prospecto_tipo").value;
    //var selectedcombotipo = combotipo.options[combotipo.selectedIndex].value;

    var contacto = document.getElementById("contacto").value;
    var telefono = document.getElementById("telefono").value;
    var dias = document.getElementById("dias").value;
    var limite = document.getElementById("limite").value;
    var comision = document.getElementById("comision").value;
    var coordinador = document.getElementById("coordinador").value;
    var email = document.getElementById("email").value;
    var observaciones = document.getElementById("telefono").value;

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoClientes/IngCliente",
        data: { nit, cliente, direccion, id_vendedor, tipo, contacto, telefono, dias, limite, comision, coordinador, email, observaciones },
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

            //document.getElementById("nit").value = "";
            //document.getElementById("cliente").value = "";
            //document.getElementById("direccion").value = "";
            //document.getElementById("nombre_contacto").value = "";
            //document.getElementById("telefono").value = "";
            //document.getElementById("dias").value = "";
            //document.getElementById("limite").value = "";
            //document.getElementById("email").value = "";
            //document.getElementById("telefono").value = "";
            window.location.href = "/IngresoClientes/IngresoClientes";

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


}

