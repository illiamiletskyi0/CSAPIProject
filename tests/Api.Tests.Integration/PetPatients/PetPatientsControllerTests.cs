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

        var dbPetPatient =
            await Context.PetPatients.SingleAsync(x => x.MicrochipId == _secondTestPetPatient.MicrochipId);
        dbPetPatient.Id.Should().Be(_secondTestPetPatient.Id);
    }

    [Fact]
    public async Task ShouldNotCreatePetPatientBecauseRequestIsInvalid()
    {
        var request = new CreatePetPatientDto(
            _firstTestPetPatient.MicrochipId,
            _firstTestPetPatient.Name,
            _firstTestPetPatient.Species,
            0,
            _firstTestPetPatient.IsVaccinated);

        var response = await Client.PostAsJsonAsync(_createPetPatientRoute, request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await Context.PetPatients.AsNoTracking().CountAsync()).Should().Be(1);
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

    [Fact]
    public async Task ShouldNotUpdatePetPatientBecauseMicrochipIdAlreadyTaken()
    {
        await Context.PetPatients.AddAsync(_firstTestPetPatient);
        await Context.SaveChangesAsync();

        var request = new UpdatePetPatientDto(
            _firstTestPetPatient.MicrochipId,
            _firstTestPetPatient.Name,
            _firstTestPetPatient.Species,
            _firstTestPetPatient.AgeYears,
            _firstTestPetPatient.IsVaccinated);

        var response = await Client.PutAsJsonAsync(_updatePetPatientRoute, request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var dbPetPatients = await Context.PetPatients.AsNoTracking().ToListAsync();
        dbPetPatients.Should().HaveCount(2);

        var unchangedPetPatient = dbPetPatients.Single(x => x.Id == _firstTestPetPatient.Id);
        unchangedPetPatient.MicrochipId.Should().Be(_firstTestPetPatient.MicrochipId);
        unchangedPetPatient.Name.Should().Be(_firstTestPetPatient.Name);
        unchangedPetPatient.Species.Should().Be(_firstTestPetPatient.Species);
        unchangedPetPatient.AgeYears.Should().Be(_firstTestPetPatient.AgeYears);
        unchangedPetPatient.IsVaccinated.Should().Be(_firstTestPetPatient.IsVaccinated);

        var unchangedUpdatedPetPatient = dbPetPatients.Single(x => x.Id == _secondTestPetPatient.Id);
        unchangedUpdatedPetPatient.MicrochipId.Should().Be(_secondTestPetPatient.MicrochipId);
        unchangedUpdatedPetPatient.Name.Should().Be(_secondTestPetPatient.Name);
        unchangedUpdatedPetPatient.Species.Should().Be(_secondTestPetPatient.Species);
        unchangedUpdatedPetPatient.AgeYears.Should().Be(_secondTestPetPatient.AgeYears);
        unchangedUpdatedPetPatient.IsVaccinated.Should().Be(_secondTestPetPatient.IsVaccinated);
    }

    [Fact]
    public async Task ShouldNotUpdatePetPatientBecausePetPatientNotFound()
    {
        var request = new UpdatePetPatientDto(
            _thirdTestPetPatient.MicrochipId,
            _thirdTestPetPatient.Name,
            _thirdTestPetPatient.Species,
            _thirdTestPetPatient.AgeYears,
            _thirdTestPetPatient.IsVaccinated);

        var response = await Client.PutAsJsonAsync($"pet-patients/{Guid.NewGuid()}", request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var dbPetPatients = await Context.PetPatients.AsNoTracking().ToListAsync();
        dbPetPatients.Should().ContainSingle();
        dbPetPatients[0].Id.Should().Be(_secondTestPetPatient.Id);
        dbPetPatients[0].MicrochipId.Should().Be(_secondTestPetPatient.MicrochipId);
        dbPetPatients[0].Name.Should().Be(_secondTestPetPatient.Name);
        dbPetPatients[0].Species.Should().Be(_secondTestPetPatient.Species);
        dbPetPatients[0].AgeYears.Should().Be(_secondTestPetPatient.AgeYears);
        dbPetPatients[0].IsVaccinated.Should().Be(_secondTestPetPatient.IsVaccinated);
    }

    [Fact]
    public async Task ShouldDeletePetPatient()
    {
        var response = await Client.DeleteAsync(_updatePetPatientRoute);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await Context.PetPatients.AsNoTracking().AnyAsync(x => x.Id == _secondTestPetPatient.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task ShouldNotDeletePetPatientBecausePetPatientNotFound()
    {
        var response = await Client.DeleteAsync($"pet-patients/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var dbPetPatient = await Context.PetPatients.AsNoTracking().SingleAsync();
        dbPetPatient.Id.Should().Be(_secondTestPetPatient.Id);
        dbPetPatient.MicrochipId.Should().Be(_secondTestPetPatient.MicrochipId);
        dbPetPatient.Name.Should().Be(_secondTestPetPatient.Name);
        dbPetPatient.Species.Should().Be(_secondTestPetPatient.Species);
        dbPetPatient.AgeYears.Should().Be(_secondTestPetPatient.AgeYears);
        dbPetPatient.IsVaccinated.Should().Be(_secondTestPetPatient.IsVaccinated);
    }
}