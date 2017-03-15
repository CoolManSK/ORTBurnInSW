using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace SigmaSureManualReportGenerator
{
    public class ProductBarcode
    {
        private String ProductsConfigFileName = "ProductsConfiguration.xml";
        private XmlDocument ProductsConfig = new XmlDocument();

        public ProductBarcode()
        {
            this.LoadProductsConfig();
        }

        private void LoadProductsConfig()
        {
            try
            {
                this.ProductsConfig.Load(this.ProductsConfigFileName);
            }
            catch
            {
                this.ErrorMessageBoxShow("Nenasiel sa konfiguracny subor. Zavolajte technika.");
            }
        }

        private void ErrorMessageBoxShow(String Message)
        {
            MessageBox.Show(Message, "CHYBA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Int32 i = 0;
            while (i < 9999999) i++;
        }

        private XmlNode GetProductNode(String ProductNo)
        {
            XmlNode AssembliesNode = this.ProductsConfig.SelectSingleNode("./Configuration/Assemblies");
            foreach (XmlNode actProdNode in AssembliesNode.ChildNodes)
            {
                if (actProdNode.SelectSingleNode("./Name").InnerText == ProductNo) return actProdNode;
            }
            return null;
        }        
        private String FormatSerialNumber(String InputSN)
        {
            while (InputSN.Length < 5)
            {
                InputSN = String.Concat("0", InputSN);
            }
            return InputSN;
        }

        public static String GetProductNoFromBarcode(String BarcodeText)
        {
            ProductBarcode PB = new ProductBarcode();
            String retValue = BarcodeText;
            if (BarcodeText.IndexOf(';') > -1)
            {
                return BarcodeText.Substring(0, BarcodeText.IndexOf(';'));
            }
            foreach (XmlNode actFamilyNode in PB.ProductsConfig.SelectSingleNode("./Configuration/Families").ChildNodes)
            {
                String str_actFamilyName = actFamilyNode.SelectSingleNode("./Name").InnerText;
                if (str_actFamilyName.Substring(0, 1) == "%") str_actFamilyName = str_actFamilyName.Substring(1);
                if (BarcodeText.IndexOf(str_actFamilyName) > -1)
                {
                    XmlNode node_PNStartIndex = actFamilyNode.SelectSingleNode("./BarcodeSettings/ProductNo/StartIndex");
                    if (node_PNStartIndex == null)
                    {
                        PB.ErrorMessageBoxShow("V konfiguracnom subore chyba informacia o zaciatocnej pozicii ProductNo v barcode pre rodinu \"" + str_actFamilyName + "\". Zavolajte prosim testovacieho inziniera.");
                        return "Error";
                    }
                    Int16 n_PNStartIndex = Convert.ToInt16(node_PNStartIndex.InnerText);

                    if (n_PNStartIndex == -1) break;

                    XmlNode node_PNLength = actFamilyNode.SelectSingleNode("./BarcodeSettings/ProductNo/Length");
                    if (node_PNLength == null)
                    {
                        PB.ErrorMessageBoxShow("V konfiguracnom subore chyba informacia o dlzke ProductNo v barcode pre rodinu \"" + str_actFamilyName + "\". Zavolajte prosim testovacieho inziniera.");
                        return "Error";
                    }
                    Int16 n_PNLength = Convert.ToInt16(node_PNLength.InnerText);

                    if (n_PNLength == 0)
                    {
                        retValue = BarcodeText.Substring(n_PNStartIndex);
                    }
                    else
                    {
                        retValue = BarcodeText.Substring(n_PNStartIndex, n_PNLength);
                    }
                    break;
                }
            }
            return retValue;
        }
        public static String GetJobIdFromBarcode(String ProductNo, String BarcodeText)
        {
            ProductBarcode PB = new ProductBarcode();
            PB.LoadProductsConfig();
            XmlNode node_actProdNoNode = PB.GetProductNode(ProductNo);
            if (node_actProdNoNode == null)
            {
                return null;
            }
            if (node_actProdNoNode.SelectSingleNode("./BarcodeLength") != null)
            {
                if (BarcodeText.Length < Convert.ToInt32(node_actProdNoNode.SelectSingleNode("./BarcodeLength").InnerText))
                {
                    if ((BarcodeText.Length == 7) || (BarcodeText.Length == 8))
                    {
                        return BarcodeText;
                    }
                    return "";
                }
            }
            XmlNode actBarcodeIdentifierNode = node_actProdNoNode.SelectSingleNode("./BarcodeSettings");
            if (actBarcodeIdentifierNode == null)
            {
                
            }
            else
            {
                XmlNode actJobIDNode = actBarcodeIdentifierNode.SelectSingleNode("./JobID");
                if (actJobIDNode == null)
                {
                    PB.ErrorMessageBoxShow(String.Concat("V konfiguracnom subore chyba info \"BarcodeSettings/JobID\" pre vyrobok ", ProductNo, "\n Zavolajte testovacieho inziniera!"));
                    return "";
                }
                else
                {
                    try
                    {
                        Int16 n_StartIndex = Convert.ToInt16(actJobIDNode.SelectSingleNode("./StartIndex").InnerText);
                        Int16 n_Length = Convert.ToInt16(actJobIDNode.SelectSingleNode("./Length").InnerText);
                        if (n_StartIndex < 0)
                        {
                            return "";
                        }
                        else
                        {
                            return BarcodeText.Substring(n_StartIndex, n_Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        PB.ErrorMessageBoxShow(ex.Message);
                        return "";
                    }
                }
            }
            return BarcodeText;
        }
        public static String GetSerialNumberFromBarcode(String ProductNo, String BarcodeText)
        {            
            ProductBarcode PB = new ProductBarcode();
            if (BarcodeText.Length == 13)
            {
                BarcodeText = PB.FormatSerialNumber(BarcodeText);
                return BarcodeText;
            }

            if (BarcodeText.Length == 22)
            {
                BarcodeText = String.Concat(BarcodeText.Substring(2, 8), BarcodeText.Substring(17));
            }
            if (BarcodeText.Length == 18)
            {
                BarcodeText = String.Concat(BarcodeText.Substring(2, 8), BarcodeText.Substring(13));
            }
            if (BarcodeText.IndexOf(';') > -1)
            {
                switch (BarcodeText.Length - BarcodeText.Replace(";", "").Length)
                {
                    case 1:
                    case 2:
                    case 3:
                        PB.ErrorMessageBoxShow("Nespravny format barcodu. Prilis malo znakov ';'");
                        return BarcodeText;
                    case 4:
                        return BarcodeText.Substring(BarcodeText.LastIndexOf(';') + 1);
                    default:
                        if (BarcodeText.Substring(0, 1) == "#")
                        {
                            return BarcodeText.Substring(1, 13);
                        }
                        else
                        {
                            Array ProductBarcode = BarcodeText.Split(';');
                            return String.Concat(ProductBarcode.GetValue(2).ToString().Trim(), ProductBarcode.GetValue(4).ToString().Trim());
                        }
                }
            }
            else return BarcodeText;
        }
    }
}
