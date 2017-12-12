using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class InvoiceEntity : TableEntity, IInvoiceEntity
    {
        public InvoiceEntity()
        {
            PartitionKey = GeneratePartitionKey();
        }

        public string InvoiceId
        {
            get => RowKey;
            set => RowKey = value;
        }

        public string InvoiceNumber { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }

        public static string GeneratePartitionKey()
        {
            return "I";
        }
    }

    public class InvoiceRepository: IInvoiceRepository
    {
        private readonly INoSQLTableStorage<InvoiceEntity> _tableStorage;

        public InvoiceRepository(INoSQLTableStorage<InvoiceEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<bool> SaveInvoice(IInvoiceEntity invoice)
        {
            try
            {
                var store = new InvoiceEntity
                {
                    InvoiceNumber = invoice.InvoiceNumber,
                    Amount = invoice.Amount,
                    Currency = invoice.Currency,
                    Status = invoice.Status
                };
                await _tableStorage.InsertOrMergeAsync(store);
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public async Task<List<IInvoiceEntity>> GetInvoices()
        {
            var pk = InvoiceEntity.GeneratePartitionKey();
            return (await _tableStorage.GetDataAsync(pk)).Cast<IInvoiceEntity>().ToList();
        }

        public async Task<IInvoiceEntity> GetInvoice(string invoiceId)
        {
            var pk = InvoiceEntity.GeneratePartitionKey();
            return await _tableStorage.GetDataAsync(pk, invoiceId);
        }
    }
}
