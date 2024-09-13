using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Countries;
public partial class CountryEdit
{
    private Country? country;
    private CountryForm? countryForm;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await Repository.GetAsync<Country>($"api/countries/{Id}");
        if (response.Error)
        {
            if (response.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/countries");
            }
            else
            {
                var message = await response.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(Localizer["Error"], message, SweetAlertIcon.Error);
            }
        }
        else
        {
            country = response.Response;
        }
    }

    private async Task EditAsync()
    {
        var response = await Repository.PutAsync("api/countries", country);
        if (response.Error)
        {
            var message = await response.GetErrorMessageAsync();
            await SweetAlertService.FireAsync(Localizer["Error"], message, SweetAlertIcon.Error);
            return;
        }

        Return();
        var toast = SweetAlertService.Mixin(new SweetAlertOptions
        {
            Toast = true,
            Position = SweetAlertPosition.BottomEnd,
            ShowConfirmButton = true,
            Timer = 3000,
        });

        await toast.FireAsync(icon: SweetAlertIcon.Success, message: Localizer["RecordSavedOk"]);
    }

    private void Return()
    {
        countryForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("/countries");
    }
}