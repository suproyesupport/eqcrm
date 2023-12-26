$(document).ready(InicionEventos);

function InicionEventos() {
    LlenarVentasDiarias();
}


function FiltrarVentasDiarias() {

    let cFecha1 = document.getElementById("cfecha1").value;
    let cFecha2 = document.getElementById("cfecha2").value;
  

    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/PanelVentasDiarias/ConsultaVentasDiariasCF",
        data: { cFecha1, cFecha2 },
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}



function LlenarVentasDiarias() {
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/PanelVentasDiarias/ConsultaVentasDiarias",
        data: {},
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}

