using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.PetPatients;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.PetPatients;
using Xunit;

namespace Api.Tests.Integration.PetPatients;

public class PetPatientsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly PetPatient _firstTestPetPatient;
    private readonly PetPatient _secondTestPetPatient;
    private readonly PetPatient _thirdTestPetPatient;

    private readonly string _createPetPatientRoute;
    private readonly string _updatePetPatientRoute;

    public PetPatientsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _firstTestPetPatient = PetPatientsTestData.FirstTestPetPatient();
        _secondTestPetPatient = PetPatientsTestData.SecondTestPetPatient();
        _thirdTestPetPatient = PetPatientsTestData.ThirdTestPetPatient();

        _createPetPatientRoute = "pet-patients";
        _updatePetPatientRoute = $"pet-patients/{_secondTestPetPatient.Id}";
    }

    public async Task InitializeAsync()
    {
        await Context.PetPatients.AddAsync(_secondTestPetPatient);

        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        Context.PetPatients.RemoveRange(Context.PetPatients);

        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task ShouldCreatePetPatient()
    {
        // Arrange
        var request = new CreatePetPatientDto(
            _firstTestPetPatient.MicrochipId,
            _firstTestPetPatient.Name,
            _firstTestPetPatient.Species,
            _firstTestPetPatient.AgeYears,
            _firstTestPetPatient.IsVaccinated);

        // Act
        var response = await Client.PostAsJsonAsync(_createPetPatientRoute, request, JsonOptions);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseDto = await response.Content.ReadFromJsonAsync<PetPatientDto>(JsonOptions);
        responseDto.Should().NotBeNull();

        var dbPetPatient = await Context.PetPatients.FirstOrDefaultAsync(x =>
            x.Id == new PetPatientId(responseDto!.Id));

        dbPetPatient.Should().NotBeNull();
        dbPetPatient!.MicrochipId.Should().Be(request.MicrochipId);
        dbPetPatient.Name.Should().Be(request.Name);
        dbPetPatient.Species.Should().Be(request.Species);
        dbPetPatient.AgeYears.Should().Be(request.AgeYears);
        dbPetPatient.IsVaccinated.Should().Be(request.IsVaccinated);
    }

    [Fact]
    public async Task ShouldNotCreatePetPatientBecauseMicrochipIdAlreadyTaken()
    {
        var request = new CreatePetPatientDto(
            _secondTestPetPatient.MicrochipId,
            _secondTestPetPatient.Name,
            _secondTestPetPatient.Species,
            _secondTestPetPatient.AgeYears,
            _secondTestPetPatient.IsVaccinated);

        var response = await Client.PostAsJsonAsync(_createPetPatientRoute, request, JsonOptions);

        response.IsSuccessStatusCode.Should().BeFalse();
        
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        
        var dbPetPatient = await Context.PetPatients.SingleAsync(x => x.MicrochipId == _secondTestPetPatient.MicrochipId);
        dbPetPatient.Id.Should().Be(_secondTestPetPatient.Id);
    }

    [Fact]
    public async Task ShouldUpdatePetPatient()
    {
        var request = new UpdatePetPatientDto(
            _thirdTestPetPatient.MicrochipId,
            _thirdTestPetPatient.Name,
            _thirdTestPetPatient.Species,
            _thirdTestPetPatient.AgeYears,
            _thirdTestPetPatient.IsVaccinated);

        var response = await Client.PutAsJsonAsync(_updatePetPatientRoute, request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseDto = await response.Content.ReadFromJsonAsync<PetPatientDto>(JsonOptions);
        responseDto.Should().NotBeNull();
        responseDto!.Id.Should().Be(_secondTestPetPatient.Id.Value);
        responseDto.MicrochipId.Should().Be(request.MicrochipId);
        responseDto.Name.Should().Be(request.Name);
        responseDto.Species.Should().Be(request.Species);
        responseDto.AgeYears.Should().Be(request.AgeYears);
        responseDto.IsVaccinated.Should().Be(request.IsVaccinated);

        var dbPetPatient = await Context.PetPatients.FirstOrDefaultAsync(x => x.Id == _secondTestPetPatient.Id);
        dbPetPatient.Should().NotBeNull();
        dbPetPatient!.MicrochipId.Should().Be(request.MicrochipId);
        dbPetPatient.Name.Should().Be(request.Name);
        dbPetPatient.Species.Should().Be(request.Species);
        dbPetPatient.AgeYears.Should().Be(request.AgeYears);
        dbPetPatient.IsVaccinated.Should().Be(request.IsVaccinated);
    }
}
