using URLEntryMVC.Data;
using URLEntryMVC.Entities;
using URLEntryMVC.ViewModel.BusinessReviewVM;

namespace URLEntryMVC.Services;

public class BusinessPointDelaySettingService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BusinessPointDelaySettingService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Service Start At Time:" + DateTime.UtcNow);
            // Your database update logic here...
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<DataContext>();
                // get List of all distinct UrlIdFk
                var urlPointIds = _db.BusinessReviewPoints.Select(x => x.UrlIdFk).Distinct().ToList();
                foreach (var pointId in urlPointIds)
                {
                    List<BusinessReviewPoint> listOfBrPoints = _db.BusinessReviewPoints.
                                                                Where(x => x.UrlIdFk == pointId && x.PointUrl != null).ToList();
                    for (int i = 0; i < listOfBrPoints.Count; i++)
                    {
                        if (listOfBrPoints[i].IsCurrentlyActive==true && listOfBrPoints[i].DatePointer<=DateTime.UtcNow && listOfBrPoints[i].PointUrl != null)
                        {
                            if (i + 1 < listOfBrPoints.Count)
                            {
                                //There is item in next iteration
                                listOfBrPoints[i].IsCurrentlyActive =false;
                                listOfBrPoints[i].DatePointer = null;
                                listOfBrPoints[i+1].IsCurrentlyActive = true;
                                listOfBrPoints[i+1].DatePointer = DateTime.UtcNow.AddMinutes(Convert.ToDouble(listOfBrPoints[i+1].DelayTimeInMinuts));
                                Console.WriteLine("Activating the Link{0} with Id {1} and with delayTime {2} and DatePointer:", listOfBrPoints[i].PointUrl, listOfBrPoints[i].BusinessPointId, listOfBrPoints[i].DelayTimeInMinuts, listOfBrPoints[i].DatePointer);
                            }
                            else
                            {
                                //There is no more item in next iteration so move to the first item
                                listOfBrPoints[i].IsCurrentlyActive = false;
                                listOfBrPoints[i].DatePointer = null;
                                listOfBrPoints[0].IsCurrentlyActive = true;
                                listOfBrPoints[0].DatePointer = DateTime.UtcNow.AddMinutes(Convert.ToDouble(listOfBrPoints[0].DelayTimeInMinuts));
                                Console.WriteLine("Activating the Link{0} with Id {1} and with delayTime {2} and DatePointer:", listOfBrPoints[i].PointUrl, listOfBrPoints[i].BusinessPointId, listOfBrPoints[i].DelayTimeInMinuts, listOfBrPoints[i].DatePointer);
                            }
                        }
                    }
                    _db.BusinessReviewPoints.UpdateRange(listOfBrPoints);
                    _db.SaveChanges();
                }
            }
            // Example: _dbContext.SaveChanges();
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Delay between updates
        }
    }
}
