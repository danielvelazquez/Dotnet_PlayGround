using Playwrite.LectorDePagina.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playwrite.LectorDePagina.Servicios
{
    public interface IPageReaderService
    {
        Task LoadTransviewerReport(string pageUrl);
        Task ReadTransviewerTable(TvTable tvTable);
        Task CalculateSpikes(TvTable tvTable);
        Task<Dictionary<string, string>> ReadBanxicoIndicators();
    }
}
