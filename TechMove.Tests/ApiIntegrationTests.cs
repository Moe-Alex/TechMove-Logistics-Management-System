using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TechMove.Api.DTOs;
using TechMove.Models.Enums;
using Xunit;

namespace TechMove.Tests
{
    public class ApiIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [Fact]
        public async Task GetClients_ReturnsSuccessStatusCode()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/clients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_ReturnsSuccessStatusCode()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequests_ReturnsSuccessStatusCode()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/service-requests");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMissingContract_ReturnsNotFound()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/contracts/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateClient_ThenReadClient_ReturnsCreatedClient()
        {
            var client = _factory.CreateClient();

            var createDto = new CreateClientDto
            {
                Name = "Integration Test Client",
                ContactDetails = "0811111111",
                Region = "Pretoria"
            };

            var postResponse = await client.PostAsJsonAsync("/api/clients", createDto);

            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var createdClient = await postResponse.Content.ReadFromJsonAsync<ClientDto>();

            Assert.NotNull(createdClient);
            Assert.True(createdClient!.Id > 0);
            Assert.Equal("Integration Test Client", createdClient.Name);

            var getResponse = await client.GetAsync($"/api/clients/{createdClient.Id}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var readClient = await getResponse.Content.ReadFromJsonAsync<ClientDto>();

            Assert.NotNull(readClient);
            Assert.Equal(createdClient.Id, readClient!.Id);
            Assert.Equal("Integration Test Client", readClient.Name);
        }

        [Fact]
        public async Task CreateContract_WithInvalidClient_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            var createDto = new CreateContractDto
            {
                ClientId = 999999,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7),
                Status = ContractStatus.Active,
                ServiceLevel = "Test Level"
            };

            var response = await client.PostAsJsonAsync("/api/contracts", createDto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateContract_WithEndDateBeforeStartDate_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            var createDto = new CreateContractDto
            {
                ClientId = 1,
                StartDate = DateTime.Today.AddDays(7),
                EndDate = DateTime.Today,
                Status = ContractStatus.Active,
                ServiceLevel = "Invalid Date Test"
            };

            var response = await client.PostAsJsonAsync("/api/contracts", createDto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}