$(document).ready(InicionEventos);

function InicionEventos() {
    BuscarID();
    
}


function GuardaPromo() {
    
    $('#modal-guardapromotor').modal('show');


}

function BuscarID() {
    
    var id_codigo = document.getElementById("id_vendedor").value;


    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Promotores/BuscarID",
        data: { },
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

            id_codigo = response.CODIGO + 1;

            $("#id_vendedor").val(id_codigo);
               
            
           
            document.getElementById("nombre").value="";
            document.getElementById("telefono").value="";
            document.getElementById("celular").value="";
            document.getElementById("email").value="";
            
            LlenarTablaPromotores();

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });

  
}

function GuadarPromo() {

    $('#modal-guardapromotor').modal('hide');

    var id_vendedor = document.getElementById("id_vendedor").value;
    var nombre = document.getElementById("nombre").value;
    var telefono = document.getElementById("telefono").value;
    var celular = document.getElementById("celular").value;
    var email = document.getElementById("email").value;

    

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/Promotores/InsertarPromotor",
        data: { id_vendedor, nombre, telefono, celular, email },
        success: function (response) {
            var arreglo = response;
            
            if (response.CODIGO == "ERROR") {

                if (response.PRODUCTO == "") {
                    $('#Error').html("EL CODIGO  NO SE ENCUENTRA EN LA BASE DE DATOS");
                    $('#ModalError').modal('show');
                    return;
                }
                else {
                    $('#Error').html(response.PRODUCTO);
                    $('#ModalError').modal('show');
                    return;
                }
            }

            BuscarID();
            

        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });


}

function LlenarTablaPromotores() {
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/Promotores/ConsultaPromotores",
        data: {},
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}