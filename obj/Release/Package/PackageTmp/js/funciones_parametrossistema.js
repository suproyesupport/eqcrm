

function act() {

    var chckfactsinexist = "";

    if (document.getElementById('chckfactsinexist').checked) {
        chckfactsinexist = "S";

    } else {
        chckfactsinexist = "N";
    }

    $.ajax({
        async: true,
        type: "POST",
        dataType: "TEXT",
        contentType: "application/x-www-form-urlencoded",
        data: { chckfactsinexist } ,
        url: "/ParametrosSistema/ActivarPermiso",

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
}