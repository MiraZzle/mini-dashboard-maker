using source.Components.Elements;
using source.Models.DataSources;

namespace source.Components.Pages
{
    public partial class DataSources
    {
        private Modal? modalRef;
        private Modal? detailsModalRef;

        private List<IDataSource> dataSources = [];
        private DataSourceType selectedType = DataSourceType.Api;
        private IDataSource? selectedDetails;


        protected override async Task OnInitializedAsync() {
            dataSources = await SourceService.GetAllAsync();
        }

        private async Task HandleApiSubmit(ApiDataSource source) {
            await AddAndRefresh(source);
        }

        private async Task HandleDownloadSubmit(DownloadDataSource source) {
            await AddAndRefresh(source);
        }

        private async Task HandleScrapeSubmit(ScrapeDataSource source) {
            await AddAndRefresh(source);
        }

        private async Task AddAndRefresh(IDataSource source) {
            await SourceService.AddAsync(source);
            dataSources = await SourceService.GetAllAsync();
            modalRef?.Close();
        }

        private void OpenDetails(IDataSource ds) {
            selectedDetails = ds;
            detailsModalRef?.Show();
        }
    }
}