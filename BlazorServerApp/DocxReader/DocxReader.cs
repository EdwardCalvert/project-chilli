using DocumentFormat.OpenXml.Packaging;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace BlazorServerApp.DocxReader
{
    class docxReader
    {


        public string doccumentText = "";

        public string getTextAsync(string path)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, true))
            {
                var text = wordDoc.MainDocumentPart.Document.InnerXml;

                XmlDocument xml = new();
                xml.LoadXml(text);
                Traverse(xml.DocumentElement);

                return doccumentText;

            }

        }

        private void Traverse(XmlNode node)
        {
            if (node.Name == "w:p")
            {
                doccumentText += "\n";
            }
            if (node.Name is "w:tab")
            {
                doccumentText += "\t";
            }
            if (node is XmlElement)
            {

                if (node.HasChildNodes)
                {
                    Traverse(node.FirstChild);
                }

                if (node.NextSibling != null)
                {
                    Traverse(node.NextSibling);
                }
            }
            else if (node is XmlText)
            {
                var text = ((XmlText)node).Value;
                doccumentText += text;
            }
        }
    }
}
