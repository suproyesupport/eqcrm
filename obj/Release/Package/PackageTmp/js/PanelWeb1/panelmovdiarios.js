$(document).ready(InicionEventos);

function InicionEventos() {
    LlenarMoviDiario();
}


function FiltrarMoviDiario() {

    let cFecha1 = document.getElementById("cfecha1").value;
    let cFecha2 = document.getElementById("cfecha2").value;
  

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/movidiarios/filtromovdiarios",
        data: { cFecha1, cFecha2 },
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



function LlenarMoviDiario() {
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/movidiarios/movdiarios",
        data: {},
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}

