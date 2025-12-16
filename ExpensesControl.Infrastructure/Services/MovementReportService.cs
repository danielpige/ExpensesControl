using ClosedXML.Excel;
using ExpensesControl.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Services
{
    public class MovementReportService : IMovementReportService
    {
        private readonly IMovementService _movementService;

        public MovementReportService(IMovementService service)
        {
            _movementService = service;
        }

        public async Task<byte[]> ExportMovementsToExcelAsync(int userId, DateTime from, DateTime to, int? moneyFundId)
        {
            var movements = await this._movementService.GetMovementsAsync(userId, from, to, moneyFundId);

            // Creamos el Excel
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Movimientos");

            var currentRow = 1;

            // Título
            var title = $"Movimientos de {from:dd/MM/yyyy} a {to:dd/MM/yyyy}";
            worksheet.Cell(currentRow, 1).Value = title;
            worksheet.Range(currentRow, 1, currentRow, 6).Merge();
            var titleCell = worksheet.Cell(currentRow, 1);
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 14;
            titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            currentRow += 2;

            // Encabezados
            worksheet.Cell(currentRow, 1).Value = "Fecha";
            worksheet.Cell(currentRow, 2).Value = "Tipo de movimiento";
            worksheet.Cell(currentRow, 3).Value = "Referencia";
            worksheet.Cell(currentRow, 4).Value = "Fondo monetario";
            worksheet.Cell(currentRow, 5).Value = "Monto";
            worksheet.Cell(currentRow, 6).Value = "Descripción";

            for (int col = 1; col <= 6; col++)
            {
                var cell = worksheet.Cell(currentRow, col);
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            currentRow++;

            // Data
            foreach (var m in movements)
            {
                worksheet.Cell(currentRow, 1).Value = m.Date;
                worksheet.Cell(currentRow, 2).Value = m.MovementType;
                worksheet.Cell(currentRow, 3).Value = m.ReferenceId;
                worksheet.Cell(currentRow, 4).Value = m.MoneyFundName;
                worksheet.Cell(currentRow, 5).Value = m.Amount;
                worksheet.Cell(currentRow, 6).Value = m.Description;

                currentRow++;
            }

            // Formatos
            // Fecha
            worksheet.Column(1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            // Monto
            worksheet.Column(5).Style.NumberFormat.Format = "#,##0.00";

            // Autoajustar columnas
            worksheet.Columns().AdjustToContents();

            // Auto-filtro
            worksheet.Range(3, 1, 3, 6).SetAutoFilter();
            worksheet.SheetView.FreezeRows(3);

            // Guardar en memoria
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
