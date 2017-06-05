﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IMerchantEntity : IEntity
    {
        string MerchantId { get; set; }
        string MerchantName { get; set; }
        string PublicKey { get; set; }
        string ApiKey { get; set; }
    }

    public interface IMerchantRepository
    {
        Task<IMerchantEntity> GetAsync(string merchantId);

        Task<IEnumerable<IMerchantEntity>> GetAllAsync();
    }
}