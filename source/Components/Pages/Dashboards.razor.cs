using source.Components.Elements;
using source.Models.Dashboards;
using source.Models.DataSources;
using source.Services.Dashboards;
using System.ComponentModel.DataAnnotations;

namespace source.Components.Pages
{
    public partial class Dashboards
    {
        private Modal? modalRef;
        private DashboardCreationModel dashboardCreationModel = new();
        private List<IDataSource> allSources = new();
        private string? filterTypeString;
        private DataSourceType? CurrentFilterType => string.IsNullOrEmpty(filterTypeString) ? null : Enum.Parse<DataSourceType>(filterTypeString);
        private string? filterName;
        private string? customErrorMessage;

        public class DashboardCreationModel
        {
            [Required(ErrorMessage = "Dashboard name is required.")]
            [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
            public string Name { get; set; } = string.Empty;

            [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
            public string? Description { get; set; }
            public List<DataSourceEntryModel> DataSourceEntries { get; set; } = new();
        }

        public class DataSourceEntryModel
        {
            public string UniqueKey { get; } = Guid.NewGuid().ToString();

            public string? SelectedDataSourceId { get; set; }
            public IDataSource? SelectedDataSource { get; set; }

            [Required(ErrorMessage = "A visualization type must be selected.")]
            public VisualizationType? SelectedVisualizationType { get; set; }
        }

        protected override async Task OnInitializedAsync() {
            allSources = await SourceService.GetAllAsync();
            ResetForm();
        }

        private IEnumerable<IDataSource> FilteredSources =>
            allSources.Where(ds =>
                (CurrentFilterType == null || ds.SourceType == CurrentFilterType) &&
                (string.IsNullOrEmpty(filterName) || ds.SourceName.Contains(filterName, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(ds => ds.SourceName);

        private void ShowCreateDashboardModal() {
            ResetForm();
            customErrorMessage = null;
            modalRef?.Show();
        }

        private void AddSpecificDataSourceEntry(IDataSource sourceToAdd) {
            customErrorMessage = null;

            if (dashboardCreationModel.DataSourceEntries.Count >= DashboardDefinition.MaxPairs) {
                customErrorMessage = "Maximum number of data sources reached.";
                return;
            }

            if (dashboardCreationModel.DataSourceEntries.Any(entry => entry.SelectedDataSourceId == sourceToAdd.Id.ToString())) {
                customErrorMessage = $"{sourceToAdd.SourceName} is already added to this dashboard.";
                return;
            }

            var newEntry = new DataSourceEntryModel {
                SelectedDataSourceId = sourceToAdd.Id.ToString(),
                SelectedDataSource = sourceToAdd,
                SelectedVisualizationType = null 
            };
            dashboardCreationModel.DataSourceEntries.Add(newEntry);
            StateHasChanged();
        }

        private void RemoveDataSourceEntry(DataSourceEntryModel entry) {
            dashboardCreationModel.DataSourceEntries.Remove(entry);
            customErrorMessage = null;
            StateHasChanged();
        }

        private async Task HandleValidSubmit() {
            customErrorMessage = null;

            bool allEntriesValid = dashboardCreationModel.DataSourceEntries.All(e =>
                e.SelectedDataSource != null &&
                e.SelectedVisualizationType.HasValue);

            if (dashboardCreationModel.DataSourceEntries.Any() && !allEntriesValid) {
                customErrorMessage = "All added data sources must have a visualization selected. Please complete or remove them.";
                return;
            }

            // ensure atleast one data source is defined
            if (!dashboardCreationModel.DataSourceEntries.Any())
            {
                customErrorMessage = "Please add at least one data source to the dashboard.";
                return;
            }

            var dashboardPairs = dashboardCreationModel.DataSourceEntries
                .Where(e => e.SelectedDataSource != null && e.SelectedVisualizationType != null)
                .Select(e => new DataSourceVisualizationPair {
                    DataSourceId = e.SelectedDataSource!.Id,
                    Visualization = e.SelectedVisualizationType!.Value
                })
                .ToList();

            var definition = new DashboardDefinition {
                Name = dashboardCreationModel.Name,
                Description = dashboardCreationModel.Description,
                DataPairs = dashboardPairs
            };

            await DashboardService.AddAsync(definition);
            modalRef?.Close();
            ResetForm();
        }

        private void CancelCreation() {
            modalRef?.Close();
            ResetForm();
        }

        private void ResetForm() {
            dashboardCreationModel = new DashboardCreationModel();
            filterTypeString = null;
            filterName = string.Empty;
            customErrorMessage = null;
            StateHasChanged();
        }
    }
}