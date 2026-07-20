using Google.Apis.Fitness.v1.Data;
using ScheduleViewer.Domain.Modules.Helpers;
using ScheduleViewer.Domain.ValueObjects;
using ScheduleViewer.Infrastructure;
using static ScheduleViewer.Infrastructure.GoogleFacade;

namespace ScheduleViewerTest
{
    public class GoogleService
    {
        [Fact]
        public async void Book()
        {
            DateOnly date = DateOnly.FromDateTime(DateTime.Today);
            var value = new DateValue(date);

            await Task.WhenAll(
            GoogleFacade.Calendar.InitializeAsync(),
            GoogleFacade.Tasks.InitializeAsync(),
            GoogleFacade.Photo.InitializeAsync(),
            GoogleFacade.Drive.InitializeAsync(),
            GoogleFacade.Fitbit.InitializeAsync(),
            GoogleFacade.Fitness.ReadActivity(value.FirstDateOfMonth, value.LastDateOfMonth),
            GoogleFacade.Fitness.ReadSteps(value.FirstDateOfMonth, value.LastDateOfMonth),
            GoogleFacade.Fitness.ReadSleepTime(value.FirstDateOfMonth, value.LastDateOfMonth));

            

            var events = GoogleFacade.Calendar.FindByDate(date);

            if (events.IsEmpty())
            {
                return;
            }

            Books.FindByTitle("Ç®åZÇøÇ·ÇÒÇÕÇ®ÇµÇ‹Ç¢ÅI(1)");
        }
    }
}