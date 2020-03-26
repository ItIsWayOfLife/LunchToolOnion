using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Web.Reports
{
    public abstract class ReportBase
    {
        protected readonly IWebHostEnvironment _webHostEnvironment;

        public ReportBase(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        protected int _maxColumn;
        protected string _title;

        protected PdfPTable _pdfTable;
        protected Document _document;
        protected Font _fontStyle;
        protected PdfPCell _pdfPCell;

        protected MemoryStream _memoryStream = new MemoryStream();

        protected virtual void EmptyRow(int nCount)
        {
            for (int i = 0; i < nCount; i++)
            {
                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = _maxColumn;
                _pdfPCell.Border = 0;
                _pdfPCell.ExtraParagraphSpace = 10;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();
            }
        }

        protected virtual void ReportHeader()
        {
            _pdfPCell = new PdfPCell();
            _pdfPCell.Colspan = 1;
            _pdfPCell.Border = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(this.SetPageTitle());
            _pdfPCell.Colspan = _maxColumn - 1;
            _pdfPCell.Border = 0;

            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();
        }

        protected virtual PdfPTable SetPageTitle()
        {
            int maxColumn = 3;
            PdfPTable pdfPTable = new PdfPTable(maxColumn);

            string ttf = _webHostEnvironment.WebRootPath + @"\fonts\ArialBold\ArialBold.ttf";
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            _fontStyle = new Font(baseFont, 16, iTextSharp.text.Font.BOLD);

            _pdfPCell = new PdfPCell(new Phrase(_title, _fontStyle));
            _pdfPCell.Colspan = maxColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfPCell);
            pdfPTable.CompleteRow();

            ttf = _webHostEnvironment.WebRootPath + @"\fonts\ArialBold\ArialBold.ttf";
            baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            _fontStyle = new Font(baseFont, 14, iTextSharp.text.Font.BOLD);

            _pdfPCell = new PdfPCell(new Phrase($"Дата и время отчёта {DateTime.Now.ToString()}", _fontStyle));
            _pdfPCell.Colspan = maxColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_pdfPCell);
            pdfPTable.CompleteRow();

            return pdfPTable;
        }

        protected abstract void ReportBody();
    }
}
