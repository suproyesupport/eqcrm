<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfSubirExcel.aspx.cs" Inherits="EqCrm.wfSubirExcel" %>

<%@ Register Assembly="DevExpress.Web.v20.1, Version=20.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <!-- Favicon-->
    <link rel="icon" type="image/x-icon" href="assets/favicon.ico" />
    <!-- Font Awesome icons (free version)-->
    <script src="https://use.fontawesome.com/releases/v6.1.0/js/all.js" crossorigin="anonymous"></script>
    <!-- Simple line icons-->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/simple-line-icons/2.5.5/css/simple-line-icons.min.css" rel="stylesheet" />
    <!-- Google fonts-->
    <link href="https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,700,300italic,400italic,700italic" rel="stylesheet" type="text/css" />
    <!-- Core theme CSS (includes Bootstrap)-->
    <link href="css/styles.css" rel="stylesheet" />

    <!-- CSS only -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.0/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-gH2yIJqKdNHPEq0n4Mqa/HGKIhSkIHeL5AyhkYV8i59U5AR6csBvApHHNl/vI1Bx" crossorigin="anonymous">

</head>


<body>
    <br />
            <br />
    <div class="container px-4 px-lg-5 text-center">
        <form id="form1" runat="server">
            <h3 class="mb-5"><em>
                <asp:Label ID="Label2" runat="server" ForeColor="Black" Text="Subir archivo"></asp:Label>
            </em></h3>
            <div>
                <p>
                    <asp:FileUpload ID="FileUpload1" runat="server" BackColor="White" ForeColor="Black" />
                </p>
            </div>
            <br />
            <p>
                <asp:Button ID="btnCargar" runat="server" Text="Cargar excel" OnClick="btnCargar_Click" class="btn btn-success" />
                <asp:Button ID="btnFacturar" runat="server" Text="Facturar" OnClick="btnFacturar_Click" class="btn btn-success" />
            
            </p>
            <asp:Label ID="label1" runat="server" Text=""></asp:Label>
            <br />
            <br />

            <asp:GridView ID="grViewExcel" runat="server" Height="169px" Width="1012px" CssClass="table">
            </asp:GridView>
        </form>
    </div>
</body>
</html>
