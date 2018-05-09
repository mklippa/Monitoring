using System.Threading.Tasks;
using Emulator.Models;

namespace Emulator.Services
{
    public interface IReportService
    {
        Report Generate(Agent agent);

        Task<bool> SendReportAsync(Report report);
    }
}