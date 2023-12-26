using DevExpress.XtraRichEdit.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EqCrm
{
    public static class CrearXml
    {

        public static string CreaDteFact(FelEstructura.DatosEmisor datosEmisor, FelEstructura.DatosReceptor datosReceptor, string cFrases, FelEstructura.Items[] items, FelEstructura.Totales totales, bool lCont, string id_interno, string nTotal, string cSerie, FelEstructura.Adenda adenda, FelEstructura.ComplementoFacturaCambiaria cfc, string certi,FelEstructura.ComplementoNotadeCredito cnc,FelEstructura.ComplementoFacturaEspecial cfe,FelEstructura.ComplementoFacturaExportacion cex )
        {
            string cDteFact = "";
            string cDetalle = "";
            cDteFact = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            cDteFact += "<dte:GTDocumento xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"";
            cDteFact += " xmlns:cfc=\"http://www.sat.gob.gt/dte/fel/CompCambiaria/0.1.0\"";
            cDteFact += " xmlns:cno=\"http://www.sat.gob.gt/face2/ComplementoReferenciaNota/0.1.0\"";
            cDteFact += " xmlns:cex=\"http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0\"";
            cDteFact += " xmlns:cfe=\"http://www.sat.gob.gt/face2/ComplementoFacturaEspecial/0.1.0\"";
            cDteFact += " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" Version=\"0.1\"";
            cDteFact += " xmlns:dte=\"http://www.sat.gob.gt/dte/fel/0.2.0\">";
            cDteFact += "<dte:SAT ClaseDocumento=\"dte\">";
            cDteFact += "<dte:DTE ID=\"DatosCertificados\">";
            cDteFact += "<dte:DatosEmision ID=\"DatosEmision\">";
            if (datosEmisor.cExp == "SI")
            {
                cDteFact += "<dte:DatosGenerales Exp=\"SI\" Tipo=\"" + datosEmisor.Tipo.Trim() + "\" FechaHoraEmision=\"" + datosEmisor.FechaHoraEmision.Trim() + "\"";
            }
            else
            {
                if (lCont == true)
                {
                    cDteFact += "<dte:DatosGenerales Tipo=\"" + datosEmisor.Tipo.Trim() + "\" FechaHoraEmision=\"" + datosEmisor.FechaHoraEmision.Trim() + "\"" + "{CONT}";
                }
                else
                {
                    cDteFact += "<dte:DatosGenerales Tipo=\"" + datosEmisor.Tipo.Trim() + "\" FechaHoraEmision=\"" + datosEmisor.FechaHoraEmision.Trim() + "\"";
                }
            }

            if (datosEmisor.Tipo == "RDON")
            {
                cDteFact += " CodigoMoneda=\"" + datosEmisor.CodigoMoneda.Trim() + "\" TipoPersoneria=\"" + datosEmisor.TipoPersoneria + "\"/>";
                
            }
            else
            {
                cDteFact += " CodigoMoneda=\"" + datosEmisor.CodigoMoneda.Trim() + "\"/>";
            }

            cDteFact += "<dte:Emisor NITEmisor=\"" + datosEmisor.NitEmisor.Trim() + "\"";
            cDteFact += " NombreEmisor=\"" + datosEmisor.NombreEmisor.Trim() + "\"";
            cDteFact += " CodigoEstablecimiento = \"" + datosEmisor.CodigoEstablecimiento.Trim() + "\" NombreComercial = \"" + datosEmisor.NombreComercial.Trim() + "\"";
            cDteFact += " CorreoEmisor=\"" + datosEmisor.CorreoEmisor.Trim() + "\" AfiliacionIVA=\"" + datosEmisor.AfiliacionIva.Trim() + "\">";
            cDteFact += "<dte:DireccionEmisor>";
            cDteFact += "<dte:Direccion>" + datosEmisor.Direccion.Trim() + "</dte:Direccion>";
            cDteFact += "<dte:CodigoPostal>" + datosEmisor.CodigoPostal.Trim() + "</dte:CodigoPostal>";
            cDteFact += "<dte:Municipio>" + datosEmisor.Municipio.Trim() + "</dte:Municipio>";
            cDteFact += "<dte:Departamento>" + datosEmisor.Departamento.Trim() + "</dte:Departamento>";
            cDteFact += "<dte:Pais>" + datosEmisor.Pais.Trim() + "</dte:Pais>";
            cDteFact += "</dte:DireccionEmisor>";
            cDteFact += "</dte:Emisor>";
            if (datosEmisor.cExp == "SI")
            {
                cDteFact += "<dte:Receptor  IDReceptor=\"" + datosReceptor.IdReceptor.Trim() + "\" TipoEspecial=\"EXT\" NombreReceptor=\"" + datosReceptor.NombreReceptor.Trim() + "\"";
            }
            else
            {
                if (datosReceptor.IdReceptor.Trim().Length >= 13)
                {
                    cDteFact += "<dte:Receptor TipoEspecial=\"CUI\" IDReceptor=\"" + datosReceptor.IdReceptor.Trim() + "\" NombreReceptor=\"" + datosReceptor.NombreReceptor.Trim() + "\"";
                }
                else
                {
                    if (datosReceptor.TipoEspecial=="EXT")
                    {
                        cDteFact += "<dte:Receptor TipoEspecial=\"EXT\" IDReceptor=\"" + datosReceptor.IdReceptor.Trim() + "\" NombreReceptor=\"" + datosReceptor.NombreReceptor.Trim() + "\"";
                    }
                    else
                    {
                        cDteFact += "<dte:Receptor IDReceptor=\"" + datosReceptor.IdReceptor.Trim() + "\" NombreReceptor=\"" + datosReceptor.NombreReceptor.Trim() + "\"";
                    }
                    
                }
            }


            cDteFact += " CorreoReceptor=\"" + datosReceptor.CorreoReceptor.Trim() + "\">";
            cDteFact += "<dte:DireccionReceptor>";
            cDteFact += "<dte:Direccion>" + datosReceptor.Direccion.Trim() + "</dte:Direccion>";
            cDteFact += "<dte:CodigoPostal>" + datosReceptor.CodigoPostal.Trim() + "</dte:CodigoPostal>";
            cDteFact += "<dte:Municipio>" + datosReceptor.Municipio.Trim() + "</dte:Municipio>";
            cDteFact += "<dte:Departamento>" + datosReceptor.Departamento.Trim() + "</dte:Departamento>";
            cDteFact += "<dte:Pais>" + datosReceptor.Pais.Trim() + "</dte:Pais>";
            cDteFact += "</dte:DireccionReceptor>";
            cDteFact += "</dte:Receptor>";
            // cDteFact += "<dte:Frases>";
            //  for (int i = 0; i < frases.Count(); i++)
            //  {
            if (datosEmisor.cExp == "SI")
            {

                cDteFact += cFrases.Replace("</dte:Frases>", "<dte:Frase TipoFrase=\"4\" CodigoEscenario=\"1\"/></dte:Frases>");
            }
            else
            {
                if (datosEmisor.Tipo == "RDON")
                {
                    cDteFact += "<dte:Frases><dte:Frase TipoFrase=\"4\" CodigoEscenario=\"4\"/></dte:Frases>";
                }
                else
                {
                    cDteFact += cFrases;
                }
            }
            //  }
            //cDteFact += "</dte:Frases>";
            cDteFact += "<dte:Items>";

            if (datosEmisor.Tipo == "RDON")
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    cDetalle += "<dte:Item NumeroLinea=\"" + items[i].NumeroLinea.Trim() + "\" BienOServicio=\"" + items[i].BienOServicio.Trim() + "\">";
                    cDetalle += "<dte:Cantidad>" + items[i].Cantidad.Trim() + "</dte:Cantidad>";
                    cDetalle += "<dte:UnidadMedida>" + items[i].UnidadMedida.Trim() + "</dte:UnidadMedida>";
                    cDetalle += "<dte:Descripcion>" + items[i].Descripcion.Trim() + "</dte:Descripcion>";
                    cDetalle += "<dte:PrecioUnitario>" + items[i].PrecioUnitario.Trim() + "</dte:PrecioUnitario>";
                    cDetalle += "<dte:Precio>" + items[i].Precio.Trim() + "</dte:Precio>";
                    cDetalle += "<dte:Descuento>" + items[i].Descuento.Trim() + "</dte:Descuento>";                    
                    cDetalle += "<dte:Total>" + items[i].Total.Trim() + "</dte:Total>";
                    cDetalle += "</dte:Item>";
                }
            }
            else if(datosEmisor.Tipo == "FPEQ")
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    cDetalle += "<dte:Item NumeroLinea=\"" + items[i].NumeroLinea.Trim() + "\" BienOServicio=\"" + items[i].BienOServicio.Trim() + "\">";
                    cDetalle += "<dte:Cantidad>" + items[i].Cantidad.Trim() + "</dte:Cantidad>";
                    cDetalle += "<dte:UnidadMedida>" + items[i].UnidadMedida.Trim() + "</dte:UnidadMedida>";
                    cDetalle += "<dte:Descripcion>" + items[i].Descripcion.Trim() + "</dte:Descripcion>";
                    cDetalle += "<dte:PrecioUnitario>" + items[i].PrecioUnitario.Trim() + "</dte:PrecioUnitario>";
                    cDetalle += "<dte:Precio>" + items[i].Precio.Trim() + "</dte:Precio>";
                    cDetalle += "<dte:Descuento>" + items[i].Descuento.Trim() + "</dte:Descuento>";
                    cDetalle += "<dte:Total>" + items[i].Total.Trim() + "</dte:Total>";
                    cDetalle += "</dte:Item>";
                }
            }
            else {
                for (int i = 0; i < items.Count(); i++)
                {
                    cDetalle += "<dte:Item NumeroLinea=\"" + items[i].NumeroLinea.Trim() + "\" BienOServicio=\"" + items[i].BienOServicio.Trim() + "\">";
                    cDetalle += "<dte:Cantidad>" + items[i].Cantidad.Trim() + "</dte:Cantidad>";
                    cDetalle += "<dte:UnidadMedida>" + items[i].UnidadMedida.Trim() + "</dte:UnidadMedida>";
                    cDetalle += "<dte:Descripcion>" + items[i].Descripcion.Trim() + "</dte:Descripcion>";
                    cDetalle += "<dte:PrecioUnitario>" + items[i].PrecioUnitario.Trim() + "</dte:PrecioUnitario>";
                    cDetalle += "<dte:Precio>" + items[i].Precio.Trim() + "</dte:Precio>";
                    cDetalle += "<dte:Descuento>" + items[i].Descuento.Trim() + "</dte:Descuento>";
                    cDetalle += "<dte:Impuestos>";
                    cDetalle += "<dte:Impuesto>";
                    cDetalle += "<dte:NombreCorto>" + items[i].NombreCorto.Trim() + "</dte:NombreCorto>";
                    cDetalle += "<dte:CodigoUnidadGravable>" + items[i].CodigoUnidadGravable.Trim() + "</dte:CodigoUnidadGravable>";
                    cDetalle += "<dte:MontoGravable>" + items[i].MontoGravable.Trim() + "</dte:MontoGravable>";
                    cDetalle += "<dte:MontoImpuesto>" + items[i].MontoImpuesto.Trim() + "</dte:MontoImpuesto>";
                    cDetalle += "</dte:Impuesto>";
                    cDetalle += "</dte:Impuestos>";
                    cDetalle += "<dte:Total>" + items[i].Total.Trim() + "</dte:Total>";
                    cDetalle += "</dte:Item>";
                }
            }
            cDteFact += cDetalle;

            if (datosEmisor.Tipo == "RDON")
            {
                cDteFact += "</dte:Items>";
                cDteFact += "<dte:Totales>";              
                cDteFact += "<dte:GranTotal>" + totales.GranTotal.Trim() + "</dte:GranTotal>";
                cDteFact += "</dte:Totales>";
            
            } else if (datosEmisor.Tipo == "FPEQ")
            {
                cDteFact += "</dte:Items>";
                cDteFact += "<dte:Totales>";
                cDteFact += "<dte:GranTotal>" + totales.GranTotal.Trim() + "</dte:GranTotal>";
                cDteFact += "</dte:Totales>";
            }
            else
            {
                cDteFact += "</dte:Items>";
                cDteFact += "<dte:Totales>";
                cDteFact += "<dte:TotalImpuestos>";
                cDteFact += "<dte:TotalImpuesto NombreCorto=\"" + totales.NombreCorto.Trim() + "\" TotalMontoImpuesto=\"" + totales.TotalMontoImpuesto.Trim() + "\"/>";
                cDteFact += "</dte:TotalImpuestos>";
                cDteFact += "<dte:GranTotal>" + totales.GranTotal.Trim() + "</dte:GranTotal>";
                cDteFact += "</dte:Totales>";
            }
            if(datosEmisor.cExp != "SI")
            { 
                if (datosEmisor.Tipo == "FCAM")
                {
                    cDteFact += "<dte:Complementos>";
                    cDteFact += "<dte:Complemento NombreComplemento = \"AbonosFacturaCambiaria\" URIComplemento = \"http://www.sat.gob.gt/dte/fel/CompCambiaria/0.1.0\">";
                    cDteFact += "<cfc:AbonosFacturaCambiaria Version = \"1\">";
                    cDteFact += "<cfc:Abono>";
                    cDteFact += "<cfc:NumeroAbono>" + cfc.NumeroAbono + "</cfc:NumeroAbono>";
                    cDteFact += "<cfc:FechaVencimiento>" + cfc.FechaVencimiento + "</cfc:FechaVencimiento>";
                    cDteFact += "<cfc:MontoAbono>" + cfc.MontoAbono + "</cfc:MontoAbono>";
                    cDteFact += "</cfc:Abono>";
                    cDteFact += "</cfc:AbonosFacturaCambiaria>";
                    cDteFact += "</dte:Complemento>";
                    cDteFact += "</dte:Complementos>";
                }
            }
            if (datosEmisor.cExp == "SI")
            {
                cDteFact += "<dte:Complementos>";

                if (datosEmisor.Tipo == "FCAM")
                {
                    
                    cDteFact += "<dte:Complemento NombreComplemento = \"AbonosFacturaCambiaria\" URIComplemento = \"http://www.sat.gob.gt/dte/fel/CompCambiaria/0.1.0\">";
                    cDteFact += "<cfc:AbonosFacturaCambiaria Version = \"1\">";
                    cDteFact += "<cfc:Abono>";
                    cDteFact += "<cfc:NumeroAbono>" + cfc.NumeroAbono + "</cfc:NumeroAbono>";
                    cDteFact += "<cfc:FechaVencimiento>" + cfc.FechaVencimiento + "</cfc:FechaVencimiento>";
                    cDteFact += "<cfc:MontoAbono>" + cfc.MontoAbono + "</cfc:MontoAbono>";
                    cDteFact += "</cfc:Abono>";
                    cDteFact += "</cfc:AbonosFacturaCambiaria>";
                    cDteFact += "</dte:Complemento>";
                    
                }


                cDteFact += "<dte:Complemento NombreComplemento=\"Exportacion\" URIComplemento=\"http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0\"><cex:Exportacion xmlns:cex=\"http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0 file:/C:/XSD/GT_Complemento_Exportaciones-0.1.0.xsd\"  Version=\"1\">";
                cDteFact += "<cex:NombreConsignatarioODestinatario>" +cex.NombreDistanatario.Trim() + "</cex:NombreConsignatarioODestinatario>";
                cDteFact += "<cex:DireccionConsignatarioODestinatario>" + cex.DireccionDestinatario.Trim() + "</cex:DireccionConsignatarioODestinatario>";
                cDteFact += "<cex:INCOTERM>"+cex.Incoterm.Trim() + "</cex:INCOTERM>";
                if(cex.CodigoConsignatario.Trim() != "")
                {
                    cDteFact += "<cex:CodigoConsignatarioODestinatario>" + cex.CodigoConsignatario.Trim() + "</cex:CodigoConsignatarioODestinatario>";
                }
                if (cex.NombreComprador.Trim() != "")
                {
                    cDteFact += "<cex:NombreComprador>" + cex.NombreComprador.Trim() + "</cex:NombreComprador>";
                }
                if (cex.NombreExportador.Trim() != "")
                {
                    cDteFact += "<cex:NombreExportador>" + cex.NombreExportador.Trim() + "</cex:NombreExportador>";
                }
                if (cex.DireccionComprador.Trim() != "")
                {
                    cDteFact += "<cex:DireccionComprador>" + cex.DireccionComprador.Trim() + "</cex:DireccionComprador>";
                }
                if (cex.CodigoComprador.Trim() != "")
                {
                    cDteFact += "<cex:CodigoComprador>" + cex.CodigoComprador.Trim() + "</cex:CodigoComprador>";
                }
                if (cex.OtraReferencia.Trim() != "")
                {
                    cDteFact += "<cex:OtraReferencia>" + cex.OtraReferencia.Trim() + "</cex:OtraReferencia>";
                }
                if (cex.CodigoExportador.Trim() != "")
                {
                    cDteFact += "<cex:CodigoExportador>" + cex.CodigoExportador.Trim() + "</cex:CodigoExportador>";
                }                
                cDteFact += "</cex:Exportacion>";
                cDteFact += "</dte:Complemento>";
                cDteFact += "</dte:Complementos>";
            }
            if (datosEmisor.Tipo == "NCRE")
            {
                cDteFact += "<dte:Complementos>";
                cDteFact += "<dte:Complemento IDComplemento = \"ReferenciasNota\"";
                cDteFact += " NombreComplemento = \"ReferenciasNota\"";
                cDteFact += " URIComplemento=\"http://www.sat.gob.gt/face2/ComplementoReferenciaNota/0.1.0\">";
                cDteFact += " <cno:ReferenciasNota xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"";
                cDteFact += " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"1\"";
                cDteFact += " NumeroAutorizacionDocumentoOrigen=\""+cnc.NumeroAutorizacionDocumentoOrigen+"\"";
                cDteFact += " FechaEmisionDocumentoOrigen=\""+cnc.FechaEmisionDocumentoOrigen+"\""+ " MotivoAjuste=\""+cnc.MotivoAjuste+"\"";
                cDteFact += " SerieDocumentoOrigen=\"" + cnc.SerieDocumentoOrigen + "\"" + " NumeroDocumentoOrigen=\""+cnc.NumeroDocumentoOrigen+"\"";
                cDteFact += " xmlns=\"http://www.sat.gob.gt/face2/ComplementoReferenciaNota/0.1.0\"/>";
                cDteFact += " </dte:Complemento>";
                cDteFact += "</dte:Complementos>";
            }
            if (datosEmisor.Tipo == "FESP")
            {
                cDteFact += "<dte:Complementos>";
                cDteFact += "<dte:Complemento URIComplemento=\"cfe\" NombreComplemento=\"FESP\"";
                cDteFact += " IDComplemento=\"ID\">";
                cDteFact += " <cfe:RetencionesFacturaEspecial";
                cDteFact += " xsi:schemaLocation=\"http://www.sat.gob.gt/face2/ComplementoFacturaEspecial/0.1.0 GT_Complemento_Fac_Especial-0.1.0.xsd\"";
                cDteFact += " Version=\"1\"";
                cDteFact += " xmlns:cfe=\"http://www.sat.gob.gt/face2/ComplementoFacturaEspecial/0.1.0\">";
                cDteFact += " <cfe:RetencionISR>"+cfe.RetencionISR+"</cfe:RetencionISR>";
                cDteFact += " <cfe:RetencionIVA>"+cfe.RetencionIVA+"</cfe:RetencionIVA>";
                cDteFact += " <cfe:TotalMenosRetenciones>"+cfe.TotalMenosRetenciones+"</cfe:TotalMenosRetenciones>";
                cDteFact += " </cfe:RetencionesFacturaEspecial>";
                cDteFact += " </dte:Complemento>";
                cDteFact += "</dte:Complementos>";
            }



            cDteFact += "</dte:DatosEmision>";
            cDteFact += "</dte:DTE>";

            if (certi.Trim() == "G4S")
            {
                cDteFact += "<dte:Adenda>";
                cDteFact += "<CamposAdionales>";
                cDteFact += "    <EncabezadoAdicional>";
                cDteFact += "            <TotalEnLetras>" + Funciones.NumLetras(nTotal) + "</TotalEnLetras>";
                cDteFact += "           <IdInterno>" + id_interno + "</IdInterno>";
                cDteFact += "            <Tipodecambio></Tipodecambio>";
                if (datosEmisor.Tipo == "FCAM")
                {
                    cDteFact += "            <LeyendaFacturaCambiaria>" + adenda.leyendafacc + "</LeyendaFacturaCambiaria>";
                }
                else
                {
                    cDteFact += "            <LeyendaFacturaCambiaria></LeyendaFacturaCambiaria>";
                }
                cDteFact += "            <Nota></Nota>";
                cDteFact += "            <Nota></Nota>";
                cDteFact += "            <Nota></Nota>";
                cDteFact += "            <Total>" + totales.GranTotal + "</Total>";

                cDteFact += "    </EncabezadoAdicional>";
                cDteFact += "</CamposAdionales>";
                cDteFact += "<CorreoElectronico xmlns=\"Schema-ediFactura\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"Schema-ediFactura esquemaAdendaEmail.xsd\">";
                cDteFact += "    <De>info@documenta.com.gt</De>";
                cDteFact += "    <Para>" + datosEmisor.cCorreos + "</Para>";
                cDteFact += "    <Asunto>DOCUMENTO TRIBUTARIO ELECTRONICO FEL</Asunto>";
                cDteFact += "    <Adjuntos>XML PDF</Adjuntos>";
                cDteFact += "</CorreoElectronico>";
                cDteFact += "</dte:Adenda>";
            }
            if (certi == "DIGIFACT")
            {
                cDteFact += "<dte:Adenda>";
                cDteFact += "<dtecomm:Informacion_COMERCIAL xmlns:dtecomm=\"https://www.digifact.com.gt/dtecomm\" xsi:schemaLocation=\"https://www.digifact.com.gt/dtecomm\">";
                cDteFact += "<dtecomm:InformacionAdicional Version=\"7.1234654163\">";
                cDteFact += "<dtecomm:REFERENCIA_INTERNA>" + id_interno.ToString() + "</dtecomm:REFERENCIA_INTERNA>";                
                cDteFact += "<dtecomm:FECHA_REFERENCIA>" + datosEmisor.FechaHoraEmision.Trim() + "</dtecomm:FECHA_REFERENCIA>";
                cDteFact += "<dtecomm:VALIDAR_REFERENCIA_INTERNA>VALIDAR</dtecomm:VALIDAR_REFERENCIA_INTERNA>"; ;
                cDteFact += "</dtecomm:InformacionAdicional>";
                cDteFact += "</dtecomm:Informacion_COMERCIAL>";
                cDteFact += "</dte:Adenda>";
            }

            cDteFact += "</dte:SAT>";
            cDteFact += "</dte:GTDocumento>";



            return cDteFact;

        }


        public static string CreaDteAnulaFact(string UUID, string FechaEmision, string cNitreceptor, string cNitEmisor, string cXmlAnula,string cMotivoAnula)
        {
            string cDteAnula = "";

            cDteAnula = cXmlAnula;
            cDteAnula = cDteAnula.Replace("{FECHAEMISION}", Convert.ToDateTime(FechaEmision).ToString("yyyy-MM-ddTHH\\:mm\\:ss"));
            cDteAnula = cDteAnula.Replace("{FECHAANULACION}", DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss"));
            cDteAnula = cDteAnula.Replace("{NITRECEPTOR}", cNitreceptor);
            cDteAnula = cDteAnula.Replace("{NITEMISOR}", cNitEmisor);
            cDteAnula = cDteAnula.Replace("{UUID}", UUID);
            cDteAnula = cDteAnula.Replace("{MOTIVOANULA}", cMotivoAnula);

            


            return cDteAnula;
        }
    }
}