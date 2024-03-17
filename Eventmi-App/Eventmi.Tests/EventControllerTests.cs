using Eventmi.Core.Models.Event;
using RestSharp;

namespace Eventmi.Tests
{
	public class Tests
	{
		// The application is running on localhost and it is used as base URL for the tests
		private RestClient _client;
		private readonly string _baseUrl = "http://localhost:7236";

		[SetUp]
		public void Setup()
		{
			_client = new RestClient(_baseUrl);
		}

		[Test]
		public async Task GetAllEvents_ReturnsSuccessStatusCode()
		{
			// Arrange
			var request = new RestRequest("/Event/All", Method.Get);

			// Act
			var response = await _client.ExecuteAsync(request);

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}

		[Test]
		public async Task Add_GetRequest_ReturnsAddView()
		{
			// Arrange
			var request = new RestRequest("/Event/Add", Method.Get);

			// Act
			var response = await _client.ExecuteAsync(request);

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}

		[Test]
		public async Task Add_PostRequest_AddNewEventAndRedirects()
		{
			// Arrange
			var request = new RestRequest("/Event/Add", Method.Post);

			var newEvent = new EventFormModel()
			{
				Name = "Music Event",
				Place = "Stadium",
				Start = new DateTime(2024, 2, 5, 6, 7, 8),
				End = new DateTime(2024, 2, 6, 8, 0, 2)
			};

			// Adding Event's information to the header of the Post request
			request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			request.AddParameter("Name", newEvent.Name);
			request.AddParameter("Place", newEvent.Place);
			request.AddParameter("Start", newEvent.Start.ToString("MM/dd/yyyy/hh:mm tt"));
			request.AddParameter("End", newEvent.End.ToString("MM/dd/yyyy/hh:mm tt"));

			// Act
			var response = await _client.ExecuteAsync(request);

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}
	}
}