using System.Net.Http.Json;
using Api.Dtos;
using Domain.PetPatients;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.PetPatients;

public class PetPatientsControllerTests : BaseIntegrationTest
{
    public PetPatientsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ShouldCreatePetPatient()
    {
        // Arrange
        var request = new CreatePetPatientDto(
            "111111111111111",
            "Fluffy",
            Species.Dog,
            3,
            true);

        // Act
        var response = await Client.PostAsJsonAsync("pet-patients", request, JsonOptions);

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
}
