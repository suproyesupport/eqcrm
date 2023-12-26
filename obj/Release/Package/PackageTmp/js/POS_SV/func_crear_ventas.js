
// FACTURADOR SALVADOR

$(document).ready(InicioEventos);

let cNit = "";
let cNrc = "";
let cEmisor = "";
let cCodActividad = "";
let descActividad = "";
let cComercial = "";
let cTipoEstablecimiento = "";
let cDireccion = "";
let cDepartamento = "";
let cMunicipio = "";
let cComplemento = "";
let cTelefono = "";
let cEmail = "";
let codEstableMH = "";
let codEstable = "";
let codPuntoVentaMH = "";
let codPuntoVenta = "";

function InicioEventos() {

    FechaActual();
    GetListDropDeptoMuni();
    GetListDropTipoActividad();
    GetListDropTipoDocumento();
    GetDataEmisor();
}

// Funciones iniciales

function FechaActual() {
    var fecha = new Date(); //Fecha actual
    var mes = fecha.getMonth() + 1; //obteniendo mes
    var dia = fecha.getDate(); //obteniendo dia
    var ano = fecha.getFullYear(); //obteniendo año
    if (dia < 10)
        dia = '0' + dia; //agrega cero si el menor de 10
    if (mes < 10)
        mes = '0' + mes //agrega cero si el menor de 10
    document.getElementById('inp_fecha').value = ano + "-" + mes + "-" + dia;
}

function GetListDropDeptoMuni() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/Operaciones_SV/GetListDropDepartamento",
        data: {},
        beforeSend: InicioConsulta,
        success: ResultadoConsulta
    });

    function InicioConsulta() {
        //$('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ResultadoConsulta(data) {

        const json = eval(data);
        cjsonData = json;

        var departamentos = $("#inp_departamentos").kendoDropDownList({
            optionLabel: "Seleccionar departamento...",
            dataTextField: "descripcion",
            dataValueField: "id_depto",
            filter: "contains",
            dataSource: cjsonData
        }).data("kendoDropDownList");

        //Llenar dataSource con JSON API
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: "/Operaciones_SV/GetListDropMunicipio",
                    dataType: "json"
                }
            }
        });

        const jsonMuni = eval(dataSource);
        cJsonMunicipio = jsonMuni;

        var municipios = $("#inp_municipios").kendoDropDownList({
            autoBind: false,
            cascadeFrom: "inp_departamentos",
            optionLabel: "Seleccionar municipio...",
            dataTextField: "descripcion",
            dataValueField: "id_municipio",
            dataSource: cJsonMunicipio
        }).data("kendoDropDownList");

    }
}

function GetListDropTipoActividad() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/Operaciones_SV/GetListDropTipoActividad",
        data: {},
        beforeSend: InicioConsulta,
        success: ResultadoConsulta
    });

    function InicioConsulta() {
        //$('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ResultadoConsulta(data) {

        const json = eval(data);
        cjsonData = json;

        $("#inp_tipoactividad").kendoDropDownList({
            optionLabel: "Seleccionar tipo de actividad...",
            //template: '<span class="order-id">#= OrderID #</span> #= OrderID #, #= ShipCountry #',
            dataTextField: "descripcion",
            dataValueField: "id_codigo",
            filter: "contains",
            height: 520,
            dataSource: cjsonData
        });

    }
}

function GetListDropTipoDocumento() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/Operaciones_SV/GetListDropTipoDocumento",
        data: {},
        beforeSend: InicioConsulta,
        success: ResultadoConsulta
    });

    function InicioConsulta() {
        //$('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ResultadoConsulta(data) {

        const json = eval(data);
        cjsonData = json;

        $("#inp_tipodocumento").kendoDropDownList({
            optionLabel: "Seleccionar tipo de documento...",
            //template: '<span class="order-id">#= OrderID #</span> #= OrderID #, #= ShipCountry #',
            dataTextField: "descripcion",
            dataValueField: "id_tipo",
            filter: "contains",
            height: 520,
            dataSource: cjsonData
        });

    }
}

// Función: obtiene los valores del emisor
function GetDataEmisor() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/Operaciones_SV/GetDataEmisor",
        data: {},
        beforeSend: InicioConsulta,
        success: ResultadoConsulta
    });

    function InicioConsulta() {
        //$('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ResultadoConsulta(data) {

        const objeto = JSON.parse(data);

        cNit = objeto.cNit;
        cNrc = objeto.cNrc;
        cEmisor = objeto.cEmisor;
        cCodActividad = objeto.CodActividad;
        descActividad = objeto.descActividad;
        cComercial = objeto.cComercial;
        cTipoEstablecimiento = objeto.cTipoEstablecimiento;
        cDireccion = objeto.cDireccion;
        cDepartamento = objeto.cDepartamento;
        cMunicipio = objeto.cMunicipio;
        cComplemento = objeto.cComplemento;
        cTelefono = objeto.cTelefono;
        cEmail = objeto.cEmail;
        codEstableMH = objeto.codEstableMH;
        codEstable = objeto.codEstable;
        codPuntoVentaMH = objeto.codPuntoVentaMH;
        codPuntoVenta = objeto.codPuntoVenta;




        //alert(objeto.cEmisor);
        //console.warn(objeto.cEmisor);

        //$("#inp_tipodocumento").kendoDropDownList({
        //    optionLabel: "Seleccionar tipo de documento...",
        //    //template: '<span class="order-id">#= OrderID #</span> #= OrderID #, #= ShipCountry #',
        //    dataTextField: "descripcion",
        //    dataValueField: "id_tipo",
        //    filter: "contains",
        //    height: 520,
        //    dataSource: cjsonData
        //});

    }

}


function ValidarCampos() {

    let validacion = false;

    const inp_nit = document.querySelector("#inp_nit");
    const inp_cliente = document.querySelector("#inp_cliente");
    const inp_fecha = document.querySelector("#inp_fecha");
    const inp_departamentos = document.querySelector("#inp_departamentos");
    const inp_municipios = document.querySelector("#inp_municipios");
    const inp_tipoactividad = document.querySelector("#inp_tipoactividad");
    const inp_tipodocumento = document.querySelector("#inp_tipodocumento");

    if (inp_nit.value == "") {
        inp_nit.setAttribute("class", "form-control is-invalid");
        validacion = false;
    } else {
        inp_nit.setAttribute("class", "form-control");
        validacion = true;
    }

    if (inp_cliente.value == "") {
        inp_cliente.setAttribute("class", "form-control is-invalid");
        validacion = false;
    } else {
        inp_cliente.setAttribute("class", "form-control");
        validacion = true;
    }

    if (inp_departamentos.value === "" || inp_departamentos.value === null) {
        $("#inp_departamentos").closest('.k-dropdownlist').addClass('form-control is-invalid');
        validacion = false;
    } else {
        $("#inp_departamentos").closest('.k-dropdownlist').addClass('form-control');
        $("#inp_departamentos").closest('.k-dropdownlist').removeClass('form-control is-invalid').addClass('form-control');
        validacion = true;
    }

    if (inp_municipios.value === "" || inp_departamentos.value === null) {
        $("#inp_municipios").closest('.k-dropdownlist').addClass('form-control is-invalid');
        validacion = false;
    } else {
        $("#inp_municipios").closest('.k-dropdownlist').addClass('form-control');
        $("#inp_municipios").closest('.k-dropdownlist').removeClass('form-control is-invalid').addClass('form-control');
        validacion = true;
    }

    if (inp_tipoactividad.value === "" || inp_departamentos.value === null) {
        $("#tipoactividad").closest('.k-dropdownlist').addClass('form-control is-invalid');
        validacion = false;
    } else {
        $("#inp_tipoactividad").closest('.k-dropdownlist').addClass('form-control');
        $("#inp_tipoactividad").closest('.k-dropdownlist').removeClass('form-control is-invalid').addClass('form-control');
        validacion = true;
    }

    if (inp_tipodocumento.value === "" || inp_departamentos.value === null) {
        $("#tipodocumento").closest('.k-dropdownlist').addClass('form-control is-invalid');
        validacion = false;
    } else {
        $("#inp_tipodocumento").closest('.k-dropdownlist').addClass('form-control');
        $("#inp_tipodocumento").closest('.k-dropdownlist').removeClass('form-control is-invalid').addClass('form-control');
        validacion = true;
    }

    return validacion;
}

function CrearJSON(p) {

    const miJSON = {
        "identificacion": {
            "version": 3,
            "ambiente": "01",
            "tipoDte": "03",
            "numeroControl": "DTE-03-00000000-000000000001769",
            "codigoGeneracion": "441BBA84-9B41-4557-A7E5-24290AB6EFA8",
            "tipoModelo": 1,
            "tipoOperacion": 1,
            "tipoContingencia": null,
            "motivoContin": null,
            "fecEmi": "2023-10-17",
            "horEmi": "17:27:59",
            "tipoMoneda": "USD"
        },
        "documentoRelacionado": null,
        "emisor": {
            "nit": cNit,
            "nrc": cNrc,
            "nombre": cEmisor,
            "codActividad": "46493",
            "descActividad": "Venta al por mayor de electrodomésticos y artículos del hogar excepto bazar; artículos de iluminacion",
            "nombreComercial": cComercial,
            "tipoEstablecimiento": "01",
            "direccion": {
                "departamento": cDepartamento,
                "municipio": cMunicipio,
                "complemento": cDireccion
            },
            "telefono": cTelefono,
            "correo": cEmail,
            "codEstableMH": null,
            "codEstable": null,
            "codPuntoVentaMH": null,
            "codPuntoVenta": null
        },
        "receptor": {
            "nit": p.nitRec,
            "nrc": p.nrcRec,
            "nombre": p.nombreRec,
            "codActividad": p.codActividadRec,
            "descActividad": p.descActividadRec,
            "nombreComercial": p.nombreComercialRec,
            "direccion": {
                "departamento": p.deptoRec,
                "municipio": p.muniRec,
                "complemento": p.complementoRec
            },
            "telefono": p.telefonoRec,
            "correo": p.correoRec
        },
        "otrosDocumentos": null,
        "ventaTercero": null,
        "cuerpoDocumento": [
            {
                "numItem": 1,
                "tipoItem": 1,
                "numeroDocumento": null,
                "cantidad": 5.0,
                "codigo": "00000009981",
                "codTributo": null,
                "uniMedida": 59,
                "descripcion": "REFRIGERADORA HISENSE 1P/FH/04 RR43D6ACX1",
                "precioUni": 247.78761062,
                "montoDescu": 402.65486726,
                "ventaNoSuj": 0.0,
                "ventaExenta": 0.0,
                "ventaGravada": 836.28318584,
                "tributos": [
                    "20"
                ],
                "psv": 0.0,
                "noGravado": 0.0
            },
            // ... (otros items)
        ],
        "resumen": {
            // ... (detalles del resumen)
        },
        "extension": null,
        "apendice": null
    };

    alert(miJSON);
    console.warn(miJSON);
}

function GuardarFactura() {

    if (ValidarCampos() === false) {
        return
    }

    const inp_nit = document.querySelector("#inp_nit");
    const inp_direccion = document.querySelector("#inp_direccion");
    const inp_departamentos = $("#inp_departamentos").data("kendoDropDownList");
    const inp_municipios = $("#inp_municipios").data("kendoDropDownList");
    const inp_tipoactividad = $("#inp_tipoactividad").data("kendoDropDownList");
    const inp_tipodocumento = $("#inp_tipodocumento").data("kendoDropDownList");
    const inp_inp_email = document.querySelector("#inp_email");

    let datosFactura = {

        //Emisor

        //Receptor
        nitRec : inp_nit.value,
        nrcRec : "pendiente",
        nombreRec : inp_cliente.value,
        codActividadRec : inp_tipoactividad.value(),
        descActividadRec : inp_tipoactividad.text(),
        nombreComercialRec: inp_cliente.value,
        deptoRec: inp_departamentos.value(),
        muniRec: inp_municipios.value(),
        complementoRec : inp_direccion.value,
        telefonoRec : "",
        correoRec : inp_email.value,

    }



    CrearJSON(datosFactura);
}

function Previsualizar() {


    //GetDataEmisor();

    alert(cComercial);
    alert(cNit);
    alert(cEmisor);

    //const tipoactividad = document.querySelector("#inp_tipoactividad");

    //const ddl = $("#inp_tipoactividad").data("kendoDropDownList");
    //alert(ddl.value());
    //alert(ddl.text());
    
}


















function ActivaNit() {

    const nit = document.getElementById("defaultUnchecked");
    const cf = document.getElementById("defaultUnchecked2");
    const inp_nit = document.querySelector("#inp_nit");
    const inp_codclie = document.querySelector("#inp_codclie");
    const inp_razon = document.querySelector("#inp_razon");

    nit.checked = true;
    cf.checked = false;
    inp_nit.disabled = false;
    inp_codclie.disabled = false;
    inp_razon.disabled = false;

    inp_nit.value = "";
    inp_razon.value = "";

}

function ActivaCf() {

    const nit = document.getElementById("defaultUnchecked");
    const cf = document.getElementById("defaultUnchecked2");
    const inp_nit = document.querySelector("#inp_nit");
    const inp_codclie = document.querySelector("#inp_codclie");
    const inp_razon = document.querySelector("#inp_razon");

    cf.checked = true;
    nit.checked = false;
    inp_nit.disabled = true;
    inp_codclie.disabled = true;
    inp_razon.disabled = true;

    inp_nit.value = "CF";
    inp_razon.value = "CONSUMIDOR FINAL";



}



