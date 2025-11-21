using Firmeza.Core.Entities;

namespace Firmeza.Application.Interfaces
{
    public interface IPdfService
    {
        /// <summary>
        /// Genera un recibo de venta en formato PDF.
        /// </summary>
        /// <param name="sale">La entidad de venta con sus detalles.</param>
        /// <returns>Un array de bytes con el contenido del PDF.</returns>
        byte[] GenerateSaleReceipt(Sale sale);

        /// <summary>
        /// Genera un reporte general de ventas.
        /// </summary>
        /// <param name="sales">Lista de ventas.</param>
        /// <returns>Array de bytes del PDF.</returns>
        byte[] GenerateSalesReport(IEnumerable<Sale> sales);
    }
}
