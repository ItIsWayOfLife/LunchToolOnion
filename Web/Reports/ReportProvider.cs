using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Web.Models.Report;

namespace Web.Reports
{
    public class ReportProvider : ReportBase
    {
        public ReportProvider(IWebHostEnvironment webHostEnvironment) : base(webHostEnvironment) { }
                
        protected string _path;
        protected string _nameProvider;
     
        protected List<ReportProviderViewModel> _reportProviders = new List<ReportProviderViewModel>();

        public virtual byte[] Report(List<ReportProviderViewModel> reportProviders, string path, string nameProvider, string title, int maxColumn)
        {
            _path = path;
            _nameProvider = nameProvider;
            _title = title;
            _maxColumn = maxColumn;

            _pdfTable = new PdfPTable(maxColumn);
            _document = new Document();

            _reportProviders = reportProviders;

            _document.SetPageSize(PageSize.A4);

            _document.SetMargins(5f, 5f, 20f, 5f);

            _pdfTable.WidthPercentage = 100;

            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

            string ttf = _webHostEnvironment.WebRootPath + @"\fonts\ArialRegular\ArialRegular.ttf";
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            _fontStyle = new Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);

            PdfWriter pdfWriter = PdfWriter.GetInstance(_document, _memoryStream);

            _document.Open();

            float[] sizes = new float[_maxColumn];

            for (var i = 0; i < _maxColumn; i++)
            {
                if (i == 0)
                {
                    sizes[i] = 20;
                }
                else
                {
                    sizes[i] = 100;
                }
            }

            _pdfTable.SetWidths(sizes);

            this.ReportHeader();
            this.EmptyRow(2);
            this.ReportBody();


            _pdfTable.HeaderRows = 2;
            _document.Add(_pdfTable);

            _document.Close();

            return _memoryStream.ToArray();
        }

        protected  PdfPTable AddLogo()
        {
            int maxColumn = 1;
            PdfPTable pdfPTable = new PdfPTable(maxColumn);

            string path = _webHostEnvironment.WebRootPath + _path;

            Image img = Image.GetInstance(path);

            _pdfPCell = new PdfPCell(img);

            _pdfPCell.Colspan = maxColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.ExtraParagraphSpace = 0;

            pdfPTable.AddCell(_pdfPCell);

            pdfPTable.CompleteRow();

            return pdfPTable;
        }
  
        protected override void ReportBody()
        {
            var ttf = _webHostEnvironment.WebRootPath + @"\fonts\ArialBold\ArialBold.ttf";
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            _fontStyle = new Font(baseFont, 14, iTextSharp.text.Font.ITALIC);

            #region Datail Table Header

            _pdfPCell = new PdfPCell(new Phrase("Номер заказа", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.Gray;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Дата и время заказа", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.Gray;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Количество блюд", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.Gray;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Цена", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.Gray;
            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();

            #endregion

            #region Detail table body

            ttf = _webHostEnvironment.WebRootPath + @"\fonts\ArialBold\ArialBold.ttf";
            baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            _fontStyle = new Font(baseFont, 14, iTextSharp.text.Font.NORMAL);

            foreach (var reportProvider in _reportProviders)
            {
                _pdfPCell = new PdfPCell(new Phrase(reportProvider.Id.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(reportProvider.DateOrder.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(reportProvider.CountOrderDishes.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(reportProvider.FullPrice.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();
            }

            #endregion
        }

        protected override void ReportHeader()
        {
            if (_path != null)
            {
                _pdfPCell = new PdfPCell(this.AddLogo());
            }
            else
            {
                _pdfPCell = new PdfPCell();
            }

            _pdfPCell.Colspan = 1;
            _pdfPCell.Border = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(this.SetPageTitle());
            _pdfPCell.Colspan = _maxColumn - 1;
            _pdfPCell.Border = 0;

            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();
        }
    }
}
