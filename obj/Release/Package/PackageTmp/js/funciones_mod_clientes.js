    $(document).ready(InicioEventos);

function InicioEventos() {
    GetList();
    //GetListComboStatus();
    GetListDropStatus();
}

function GetList() {

    $.ajax({
        async: false,
        type: "POST",
        dataType: "HTML",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModClientes/GetList",
        data: {},
        beforeSend: InicioConsulta,
        success: ResultadoConsulta
    });

    function InicioConsulta() {
        //$('#mostrar_consulta').html('Cargando por favor espere...');
    }

    function ResultadoConsulta(data) {

        //Para vaciar lo que tiene la tabla
        document.querySelector("#gridContainer").innerHTML = "";

        const json = eval(data);
        cjsonData = json;

        var gridDataSource = new kendo.data.DataSource({
            data: cjsonData,
            schema: {
                model: {
                    fields: {
                        id_codigo: { type: "number" },
                        cliente: { type: "string" },
                        nit: { type: "string" },
                        status: { type: "string" },
                        facturar: { type: "string" },
                        direccion: { type: "string" },
                        telefono: { type: "string" },
                        email: { type: "string" },
                    }
                }
            },
            height: 550,
            pageSize: 12,
        });

        $("#gridContainer").kendoGrid({
            toolbar: ["excel", "pdf", "search"],
            excel: {
                fileName: "Clientes.xlsx",
                filterable: true
            },
            pdf: {
                allPages: true,
                avoidLinks: true,
                paperSize: "letter",
                margin: { top: "2cm", left: "1cm", right: "1cm", bottom: "1cm" },
                landscape: true,
                repeatHeaders: true,
                template: $("#page-template").html(),
                scale: 0.8
            },
            search: {
                fields: [
                    { name: "id_codigo", operator: "eq" },
                    { name: "cliente", operator: "contains" },
                    { name: "nit", operator: "contains" },
                    { name: "email", operator: "contains" },
                ]
            },

            dataSource: gridDataSource,
            height: 1000,
            resizable: true,
            groupable: true,
            sortable: true,
            pageable: {
                refresh: true,
                pageSizes: true,
                buttonCount: 5
            },
            filterable: true,
            columns: [
                {
                    field: "OTROS",
                    title: "Opciones",
                    width: 150,
                    template: "<table border='0'> <tr> " +
                        "<td style=\"border:none;\"><a data-toggle='modal' style='cursor:pointer;' onclick='GetData(#=(id_codigo)#)'; ><img src='/images/edit.png' width='35' height='35'></a></td>" +
                        "<td style=\"border:none;\"><a data-toggle='modal' style='cursor:pointer;' value=''  ><img src='/images/delete.png' width='30' height='30'></a></td>" +
                        "</tr></table>",
                    stickable: true,
                    locked: true,
                    lockable: false,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                }, 
                {
                    field: "id_codigo",
                    title: "Código",
                    width: 130,
                    stickable: true,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" }
                },
                {
                    field: "cliente",
                    title: "Cliente",
                    width: 400,
                    stickable: true,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                },
                {
                    field: "nit",
                    title: "Nit",
                    width: 150,
                    stickable: true,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },
                },
                {
                    field: "status",
                    title: "Status",
                    width: 100,
                    stickable: true,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                    attributes: { style: "text-align: center" },

                },
                {
                    field: "direccion",
                    title: "Direccion",
                    width: 400,
                    stickable: true,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                },
                {
                    field: "email",
                    title: "Email",
                    width: 300,
                    stickable: true,
                    headerAttributes: { style: "text-align: center; justify-content: center" },
                }
            ]
        });
    }
}


function GetListComboStatus() {

    //$.ajax({
    //    async: false,
    //    type: "POST",
    //    dataType: "HTML",
    //    contentType: "application/x-www-form-urlencoded",
    //    url: "/ModClientes/GetListComboStatus",
    //    data: {},
    //    beforeSend: InicioConsulta,
    //    success: ResultadoConsulta
    //});

    //function InicioConsulta() {
    //    //$('#mostrar_consulta').html('Cargando por favor espere...');
    //}

    /*function ResultadoConsulta(data) {*/

        //var arreglo = eval(data);
        //cData = arreglo;

    const orderData = [
        { OrderID: 1, ShipCountry: "Belgium" },
        { OrderID: 2, ShipCountry: "Singapore" },
    ]; 


        //var dataSource = new kendo.data.DataSource({
        //    data: cData,
        //    schema: {
        //        model: {
        //            fields: {
        //                id_status: { type: "string" },
        //                nombre: { type: "string" },
        //            }
        //        }
        //    }
        //});

        $("#orders").kendoComboBox({
            //template: '<span class="order-id">#= OrderID #</span> #= ShipName #, #= ShipCountry #',
            dataTextField: "ShipCountry",
            dataValueField: "OrderID",
            height: 520,
            dataSource: orderData
        });
        
    /*}*/
}


function GetListDropStatus() {

    //Ejemplo Json
    const orderData = [
        { OrderID: 1, ShipCountry: "Belgium" },
        { OrderID: 2, ShipCountry: "Singapore" },
        { OrderID: 4, ShipCountry: "jeje" },
        { OrderID: 5, ShipCountry: "jejeje" },
        { OrderID: 6, ShipCountry: "hola" },
        { OrderID: 7, ShipCountry: "holamundo" },
        { OrderID: 8, ShipCountry: "honda" },
        { OrderID: 9, ShipCountry: "turbo" },
        { OrderID: 10, ShipCountry: "apple" },
        { OrderID: 11, ShipCountry: "xbox" },
        { OrderID: 12, ShipCountry: "xbox elite" },
        { OrderID: 12, ShipCountry: "juju" },
    ];

    $("#orders").kendoDropDownList({
        //template: '<span class="order-id">#= OrderID #</span> #= OrderID #, #= ShipCountry #',
        dataTextField: "ShipCountry",
        dataValueField: "OrderID",
        filter: "contains",
        height: 520,
        dataSource: orderData
    });
}



function prueba(){

    const jeje = document.getElementById("orders");

    alert(jeje.value);
}



function GetData(id_codigo){

    OpenDatos();

    $.ajax({
        async: true,
        type: "POST",
        dataType: "JSON",
        contentType: "application/x-www-form-urlencoded",
        url: "/ModClientes/GetData",
        data: { id_codigo },
        success: function (response) {

            alert(response[0].id_codigo);

            document.querySelector("#inp_codclie").value = response[0].id_codigo;
            document.querySelector("#inp_nit").value = response[0].nit;
            document.querySelector("#inp_nombre").value = response[0].cliente;
            document.querySelector("#inp_direccion").value = response[0].direccion;
            document.querySelector("#inp_telefono").value = response[0].telefono;
            document.querySelector("#inp_email").value = response[0].email;
            document.querySelector("#inp_atencion").value = response[0].atencion;

            if (response.NIT == "ERROR") {
                $('#Error').html("NO SE ENCUENTRA EN LA BASE DE DATOS");
                $('#ModalError').modal('show');
                return;
            }

            



        },

        error: function () {
            alert("Ocurrio un Error");
        }
    });

}



//Abrir y cerrar modal
function OpenDatos() {

    const mbusqueda = document.getElementById("panel_busqueda");
    mbusqueda.setAttribute("data-action", "panel-collapse");
    mbusqueda.setAttribute("data-toggle", "tooltip");
    mbusqueda.setAttribute("data-offset", "0,10");

    const mdatos = document.getElementById("panel_datos");
    mdatos.setAttribute("class", "panel-container");
    mdatos.setAttribute("data-toggle", "tooltip");
    mdatos.setAttribute("data-offset", "0,10");

}