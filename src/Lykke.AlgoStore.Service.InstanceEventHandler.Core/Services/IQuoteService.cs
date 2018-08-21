﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services
{
    public interface IQuoteService
    {
        Task WriteAsync(
            AlgoClientInstanceData clientInstnaceData,
            IEnumerable<QuoteChartingUpdate> quotes);
    }
}
