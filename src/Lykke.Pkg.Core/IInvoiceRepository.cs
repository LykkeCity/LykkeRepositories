using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Core
{

    public interface IInvoiceEntity
    {
        string InvoiceId { get; set; }
        string InvoiceNumber { get; set; }
        double Amount { get; set; }
        string Currency { get; set; }
    }

    public interface IInvoiceRepository
    {
        Task<bool> SaveInvoice(IInvoiceEntity invoice);
        Task<List<IInvoiceEntity>> GetInvoices();
        Task<IInvoiceEntity> GetInvoice(string invoiceId);
    }
}
