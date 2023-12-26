$(document).ready(InicionEventos);



let nEstablecimiento;
let cConsola;


function Genera_Pass() {
    document.getElementById("Password").value = generateP();
}


function SetearCertificador(nEst) {
    nEstablecimiento = nEst;

    let boton = document.getElementById("idcertificador");

    boton.innerText = "Certificador " + nEstablecimiento;

}

function SetearConsola(cCons) {
    cConsola = cCons;

    let boton = document.getElementById("idconsola");

    boton.innerText = "Tipo de consola " + cConsola;

}




function InicionEventos() {
    LlenarTablaLineaI();

}

function GuardaLinea() {

    $('#modal-guardalinea').modal('show');


}


function GuadarLineaI() {

    $('#modal-guardalinea').modal('hide');

    //let DatosEmpresa = new Array();

    var json_arr = {};

    let nit = document.getElementById("nit").value;
    let Empresa = document.getElementById("Empresa").value;
    let nComercial = document.getElementById("EmpresaC").value;
    let cDireccion = document.getElementById("direccion").value;
    let depto = document.getElementById("depto").value;
    let muni = document.getElementById("muni").value;
    let token = document.getElementById("token").value;
    let cUser = document.getElementById("cUser").value;
    let cUrl = document.getElementById("cUrl").value;
    let cFrase = document.getElementById("cFrase").value;
    let cAfilia = document.getElementById("cAfilia").value;
    let cCerti = nEstablecimiento;
    let BaseDatos = document.getElementById("BaseDatos").value;
    let cTipoP = document.getElementById("cTipoP").value;
    let cAutorizacion = document.getElementById("cAutorizacion").value;
    let UsuarioNit = document.getElementById("UsuarioNit").value;
    let Password = document.getElementById("Password").value;
    let cTipoConsola = cConsola;




    json_arr["nit"] = nit;
    json_arr["empresa"] = Empresa;
    json_arr["ncomercial"] = nComercial;
    json_arr["direccion"] = cDireccion;
    json_arr["depto"] = depto;
    json_arr["municipio"] = muni;
    json_arr["token"] = token;
    json_arr["user"] = cUser;
    json_arr["curl"] = cUrl;
    json_arr["cfrase"] = btoa(cFrase);
    json_arr["afilia"] = cAfilia;
    json_arr["certificador"] = cCerti;
    json_arr["basedatos"] = BaseDatos;
    json_arr["personeria"] = cTipoP;
    json_arr["cAutorizacion"] = cAutorizacion;
    json_arr["UsuarioNit"] = UsuarioNit;
    json_arr["Password"] = Password;
    json_arr["cTipoConsola"] = cTipoConsola;

    
    var JsonEmpresa = JSON.stringify(json_arr);


    $.post('/EmpresaLineaNegocio/InsertarEmpresa', { JsonEmpresa }).done(function (respuesta) {
        alert("Registro Actualizado");
        $("#resultado").html(respuesta.cantidad);
    });


    

    

}

function LlenarTablaLineaI() {
    $.ajax({
        async: false,
        type: "POST",
        dataType: "text",
        contentType: "application/x-www-form-urlencoded",
        url: "/EmpresaLineaNegocio/ConsultaLineaNegocios",
        data: {},
        success: function (response) {
            $('#mostrar_consulta').html(response);
        },
        error: function () {
            alert("Ocurrio un Error");
        }
    });
}

