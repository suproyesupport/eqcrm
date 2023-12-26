function GuadarProv() {
    $('#modal-guardaprov').modal('show');
}

function GuadarProveedor() {

    $('#modal-guardaprov').modal('hide');

    var nit = document.getElementById("nit").value;
    var cliente = document.getElementById("cliente").value;
    var direccion = document.getElementById("direccion").value;
    var nombre_contacto = document.getElementById("nombre_contacto").value;
    var telefono = document.getElementById("telefono").value;
    var dias = document.getElementById("dias").value;
    var limite = document.getElementById("limite").value;
    var email = document.getElementById("email").value;
    var observaciones = document.getElementById("telefono").value;
    
    alert(nit);
    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/IngresoProveedores/IngProveedor",
        data: { nit, cliente, direccion, nombre_contacto, telefono, dias, limite, email, observaciones },
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
            

            document.getElementById("nit").value="";
            document.getElementById("cliente").value = "";
            document.getElementById("direccion").value = "";
            document.getElementById("nombre_contacto").value = "";
            document.getElementById("telefono").value = "";
            document.getElementById("dias").value = "";
            document.getElementById("limite").value = "";
            document.getElementById("email").value = "";
            document.getElementById("telefono").value = "";
            

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


}

