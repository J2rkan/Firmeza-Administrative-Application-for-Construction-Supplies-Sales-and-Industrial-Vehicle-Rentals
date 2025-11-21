using Firmeza.Application.Interfaces;
using Firmeza.Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmeza.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            // Configurar licencia Community (Gratuita)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateSaleReceipt(Sale sale)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(header => ComposeHeader(header, sale));
                    page.Content().Element(content => ComposeContent(content, sale));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            })
            .GeneratePdf();
        }

        public byte[] GenerateSalesReport(IEnumerable<Sale> sales)
        {
            // Implementación futura para reportes masivos
            throw new NotImplementedException();
        }

        void ComposeHeader(IContainer container, Sale sale)
        {
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Recibo de Venta #{sale.id}").Style(titleStyle);

                    column.Item().Text(text =>
                    {
                        text.Span("Fecha: ").SemiBold();
                        text.Span($"{sale.Date:dd/MM/yyyy HH:mm}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("Cliente: ").SemiBold();
                        text.Span(sale.Client?.Name ?? "Cliente General");
                    });
                    
                    if (!string.IsNullOrEmpty(sale.Client?.Document))
                    {
                        column.Item().Text(text =>
                        {
                            text.Span("Documento: ").SemiBold();
                            text.Span(sale.Client.Document);
                        });
                    }
                });

                row.ConstantItem(100).Height(50).Placeholder(); // Aquí iría el logo
            });
        }

        void ComposeContent(IContainer container, Sale sale)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(table =>
                {
                    table.Table(t =>
                    {
                        t.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Producto
                            columns.RelativeColumn();  // Cantidad
                            columns.RelativeColumn();  // Precio Unit.
                            columns.RelativeColumn();  // Total
                        });

                        t.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Producto");
                            header.Cell().Element(CellStyle).AlignRight().Text("Cant.");
                            header.Cell().Element(CellStyle).AlignRight().Text("Precio Unit.");
                            header.Cell().Element(CellStyle).AlignRight().Text("Total");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                            }
                        });

                        if (sale.SaleDetails != null)
                        {
                            foreach (var item in sale.SaleDetails)
                            {
                                t.Cell().Element(CellStyle).Text(item.Product?.Name ?? "Producto");
                                t.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                                t.Cell().Element(CellStyle).AlignRight().Text($"${item.UnitPrice:F2}");
                                t.Cell().Element(CellStyle).AlignRight().Text($"${item.Quantity * item.UnitPrice:F2}");
                            }
                        }

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    });
                });

                column.Item().PaddingRight(5).AlignRight().Text(text =>
                {
                    text.Span("Total: ").SemiBold().FontSize(14);
                    text.Span($"${sale.Total:F2}").FontSize(14);
                });
            });
        }
    }
}
