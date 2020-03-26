using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Web.Models.Report;

namespace Web.Reports
{
    public class ReportUser: ReportBase
    {
        public ReportUser(IWebHostEnvironment webHostEnvironment) : base(webHostEnvironment) { }

        List<ReportUserViewModel> _reportUsers = new List<ReportUserViewModel>();

        public virtual byte[] Report(List<ReportUserViewModel> reportUsers, string title, int maxColumn)
        {

            _title = title;
            _maxColumn = maxColumn;

            _pdfTable = new PdfPTable(maxColumn);
            _document = new Document();

            _reportUsers = reportUsers;

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

            _pdfPCell = new PdfPCell(new Phrase("Общая стоимость", _fontStyle));
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

            foreach (var report in _reportUsers)
            {
                _pdfPCell = new PdfPCell(new Phrase(report.Id.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(report.DateOrder.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(report.CountOrderDishes.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(report.FullPrice.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();
            }

            #endregion
        }
     
    }
}
