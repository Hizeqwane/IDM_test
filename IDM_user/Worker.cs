using System.Globalization;
using IDM.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace IDM_user;

public class Worker : BackgroundService
{
    private readonly IConnector _connector;

    public Worker(IConnector connector)
    {
        _connector = connector;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                var positions = _connector.GetPositions();
                var units = _connector.GetUnitsByParentId(1);
                var employees = _connector.GetEmployeesByUnit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}