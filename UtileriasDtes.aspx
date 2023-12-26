<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UtileriasDtes.aspx.cs" Inherits="EqCrm.UtileriasDtes" %>

<%@ Register assembly="DevExpress.Web.v20.1, Version=20.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <table id="tablaroles" class="table table-bordered table-hover table-striped w-100">
                <tr>
                    <td>Xml</td>
                    <td>Resultado</td>
                </tr>
                <tr>
                    <td>
                        <dx:ASPxTextBox ID="txtXml1" runat="server" Height="696px" Theme="Office2003Blue" Width="595px">
                        </dx:ASPxTextBox>
                    </td>
                    <td>
                        <dx:ASPxTextBox ID="txtXml2" runat="server" Height="694px" Theme="Office2003Blue" Width="629px">
                        </dx:ASPxTextBox>
                    </td>
                </tr>
            </table>

        </div>
    </form>
</body>
</html>
