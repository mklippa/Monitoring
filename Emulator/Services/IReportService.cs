using Emulator.Models;

namespace Emulator.Services
{
    public interface IReportService
    {
        Report Generate(Agent agent);
    }
}