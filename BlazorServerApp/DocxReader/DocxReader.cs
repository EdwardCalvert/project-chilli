using DocumentFormat.OpenXml.Packaging;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace BlazorServerApp.DocxReader
{
    public interface IDocxReader
    {
        public Task<string> GetTextAsync(string path);
    }

    class docxReader:IDocxReader
    {


        public string doccumentText = "";
        public int stepNumber = 1;

        public async Task<string> GetTextAsync(string path)
        {
            doccumentText = ""; // Clear text.
            stepNumber = 1;
            using WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, true);
            var text = wordDoc.MainDocumentPart.Document.InnerXml;
            XmlDocument xml = new();
            xml.LoadXml(text);
            await Task.Run(() => Traverse(xml.DocumentElement));

            return doccumentText;

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
            
            if(node.Name == "w:numPr") //<w:numId w:val=\"1\" />{
            {
                doccumentText += stepNumber+".\t";
                stepNumber++;
            }
            if (node.Name == "w:drawing")//<w:drawing> // do nothing. as this would cause co-ordinates to appear!
            {
                //do nothing.
            }
            else if (node is XmlElement) 
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
