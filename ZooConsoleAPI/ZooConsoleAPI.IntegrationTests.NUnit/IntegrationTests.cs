using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using ZooConsoleAPI.Business;
using ZooConsoleAPI.Business.Contracts;
using ZooConsoleAPI.Data.Model;
using ZooConsoleAPI.DataAccess;

namespace ZooConsoleAPI.IntegrationTests.NUnit
{
	public class IntegrationTests
    {
        private TestAnimalDbContext dbContext;
        private IAnimalsManager animalsManager;

        [SetUp]
        public void SetUp()
        {
            this.dbContext = new TestAnimalDbContext();
            this.animalsManager = new AnimalsManager(new AnimalRepository(this.dbContext));
        }


        [TearDown]
        public void TearDown()
        {
            this.dbContext.Database.EnsureDeleted();
            this.dbContext.Dispose();
        }


		[TestCase(0)]
		[TestCase(1)]
		[TestCase(119)]
		[TestCase(120)]
		public async Task AddAnimalAsync_ShouldAddNewAnimal(int validAgeValue)
        {
            // Arrange
            var newAnimal = new Animal()
            {
	            CatalogNumber = "01HNTWXTQSH4",
                Name = "Lappy",
                Breed = "Baboon, savanna",
                Type = "Mammal",
                Age = validAgeValue,
                Gender = "Male",
                IsHealthy = true
            };

            // Act
            await animalsManager.AddAsync(newAnimal);

			// Assert
			var dbContact = await dbContext.Animals.FirstOrDefaultAsync(a => a.CatalogNumber == newAnimal.CatalogNumber);

            Assert.IsNotNull(dbContact);
			Assert.That(dbContact.CatalogNumber, Is.EqualTo(newAnimal.CatalogNumber));
			Assert.That(dbContact.Name, Is.EqualTo(newAnimal.Name));
			Assert.That(dbContact.Breed, Is.EqualTo(newAnimal.Breed));
			Assert.That(dbContact.Type, Is.EqualTo(newAnimal.Type));
			Assert.That(dbContact.Age, Is.EqualTo(newAnimal.Age));
			Assert.That(dbContact.Gender, Is.EqualTo(newAnimal.Gender));
		}

        [TestCase(-17)]
		[TestCase(-1)]
		[TestCase(121)]
		[TestCase(130)]
		public async Task AddAnimalAsync_TryToAddAnimalWithInvalidCredentials_ShouldThrowException(int invalidAgeValue)
        {
            // Arrange
			var newAnimal = new Animal()
			{
				CatalogNumber = "01HNTWXTQX03",
				Name = "Lora",
				Breed = "Colobus, magistrate black",
				Type = "Mammal",
				Age = invalidAgeValue,
				Gender = "Female",
				IsHealthy = false
			};

			// Act && Assert
			var expectation = Assert.ThrowsAsync<ValidationException>(() => animalsManager.AddAsync(newAnimal));
			var dbContact = await dbContext.Animals.FirstOrDefaultAsync(a => a.CatalogNumber == newAnimal.CatalogNumber);

            Assert.IsNull(dbContact);
		}

        [Test]
        public async Task DeleteAnimalAsync_WithValidCatalogNumber_ShouldRemoveAnimalFromDb()
        {
			// Arrange
			var newAnimal = new Animal()
			{
				CatalogNumber = "01HNTWXTQX03",
				Name = "Loris",
				Breed = "Colobus, magistrate black",
				Type = "Mammal",
				Age = 5,
				Gender = "Female",
				IsHealthy = false
			};

            await animalsManager.AddAsync(newAnimal);

            // Act
            await animalsManager.DeleteAsync(newAnimal.CatalogNumber);

			// Assert
			var dbContact = await dbContext.Animals.FirstOrDefaultAsync(a => a.CatalogNumber == newAnimal.CatalogNumber);

            Assert.IsNull(dbContact);
		}

        [TestCase(null)]
		[TestCase("")]
		[TestCase("  ")]
		public async Task DeleteAnimalAsync_TryToDeleteWithNullOrWhiteSpaceCatalogNumber_ShouldThrowException(string invalidCatalogNumber)
        {
            // Arrange
            var errorMessage = "Catalog number cannot be empty.";

			// Act && Assert
            var expectation = Assert.ThrowsAsync<ArgumentException>(() => animalsManager.DeleteAsync(invalidCatalogNumber));

            Assert.That(expectation.Message, Is.EqualTo(errorMessage));
        }

        [Test]
        public async Task GetAllAsync_WhenAnimalsExist_ShouldReturnAllAnimals()
        {
			// Arrange
			var newAnimal1 = new Animal()
			{
				CatalogNumber = "01HNTWXTQSH4",
				Name = "Lappy",
				Breed = "Baboon, savanna",
				Type = "Mammal",
				Age = 1,
				Gender = "Male",
				IsHealthy = true
			};

			var newAnimal2 = new Animal()
			{
				CatalogNumber = "01HNTWXTQYTY",
				Name = "Charly",
				Breed = "Chilean flamingo",
				Type = "Birth",
				Age = 4,
				Gender = "Male",
				IsHealthy = true
			};

            await animalsManager.AddAsync(newAnimal1);
            await animalsManager.AddAsync(newAnimal2);

			// Act
            var animals = await animalsManager.GetAllAsync();

			// Assert
            var animal1 = animals.FirstOrDefault(a => a.CatalogNumber == newAnimal1.CatalogNumber);
			var animal2 = animals.FirstOrDefault(a => a.CatalogNumber == newAnimal2.CatalogNumber);

			Assert.That(animals.Count, Is.EqualTo(2));

			Assert.That(animal1.CatalogNumber, Is.EqualTo(newAnimal1.CatalogNumber));
			Assert.That(animal1.Name, Is.EqualTo(newAnimal1.Name));
			Assert.That(animal1.Breed, Is.EqualTo(newAnimal1.Breed));
			Assert.That(animal1.Type, Is.EqualTo(newAnimal1.Type));
			Assert.That(animal1.Age, Is.EqualTo(newAnimal1.Age));
			Assert.That(animal1.Gender, Is.EqualTo(newAnimal1.Gender));

			Assert.That(animal2.CatalogNumber, Is.EqualTo(newAnimal2.CatalogNumber));
			Assert.That(animal2.Name, Is.EqualTo(newAnimal2.Name));
			Assert.That(animal2.Breed, Is.EqualTo(newAnimal2.Breed));
			Assert.That(animal2.Type, Is.EqualTo(newAnimal2.Type));
			Assert.That(animal2.Age, Is.EqualTo(newAnimal2.Age));
			Assert.That(animal2.Gender, Is.EqualTo(newAnimal2.Gender));
		}

        [Test]
        public async Task GetAllAsync_WhenNoAnimalsExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var errorMessage = "No animal found.";

			// Act && Assert
			var expectation = Assert.ThrowsAsync<KeyNotFoundException>(() => animalsManager.GetAllAsync());

			Assert.That(expectation.Message, Is.EqualTo(errorMessage));
		}

        [Test]
        public async Task SearchByTypeAsync_WithExistingType_ShouldReturnMatchingAnimals()
        {
			// Arrange
			var searchingValue = "Mammal";

			var newAnimal1 = new Animal()
			{
				CatalogNumber = "01HNTWXTR07A",
				Name = "Steve",
				Breed = "Pine siskin",
				Type = "Birth",
				Age = 3,
				Gender = "Male",
				IsHealthy = true
			};

			var newAnimal2 = new Animal()
			{
				CatalogNumber = "01HNTWXTR0E5",
				Name = "Laura",
				Breed = "Oriental short-clawed otter",
				Type = "Mammal",
				Age = 7,
				Gender = "Female",
				IsHealthy = false
			};

			var newAnimal3 = new Animal()
			{
				CatalogNumber = "01HNTWXTR3J8",
				Name = "Stew",
				Breed = "Macaque, pig-tailed",
				Type = "Mammal",
				Age = 4,
				Gender = "Male",
				IsHealthy = true
			};

			await animalsManager.AddAsync(newAnimal1);
			await animalsManager.AddAsync(newAnimal2);
			await animalsManager.AddAsync(newAnimal3);

			// Act
			var animals = await animalsManager.SearchByTypeAsync(searchingValue);

			// Assert
			var animal1 = animals.FirstOrDefault(a => a.CatalogNumber == newAnimal2.CatalogNumber);
			var animal2 = animals.FirstOrDefault(a => a.CatalogNumber == newAnimal3.CatalogNumber);

			Assert.That(animals.Count, Is.EqualTo(2));

			Assert.That(animal1.CatalogNumber, Is.EqualTo(newAnimal2.CatalogNumber));
			Assert.That(animal1.Name, Is.EqualTo(newAnimal2.Name));
			Assert.That(animal1.Breed, Is.EqualTo(newAnimal2.Breed));
			Assert.That(animal1.Type, Is.EqualTo(newAnimal2.Type));
			Assert.That(animal1.Age, Is.EqualTo(newAnimal2.Age));
			Assert.That(animal1.Gender, Is.EqualTo(newAnimal2.Gender));

			Assert.That(animal2.CatalogNumber, Is.EqualTo(newAnimal3.CatalogNumber));
			Assert.That(animal2.Name, Is.EqualTo(newAnimal3.Name));
			Assert.That(animal2.Breed, Is.EqualTo(newAnimal3.Breed));
			Assert.That(animal2.Type, Is.EqualTo(newAnimal3.Type));
			Assert.That(animal2.Age, Is.EqualTo(newAnimal3.Age));
			Assert.That(animal2.Gender, Is.EqualTo(newAnimal3.Gender));
		}

        [TestCase("Fish")]
		[TestCase("Reptile")]
		[TestCase("Amphibian")]
		[TestCase("Insect")]
		public async Task SearchByTypeAsync_WithNonExistingType_ShouldThrowKeyNotFoundException(string invalidTypeValue)
        {
			// Arrange
			var errorMessage = "No animal found with the given type.";

			var newAnimal1 = new Animal()
			{
				CatalogNumber = "01HNTWXTR07A",
				Name = "Mo",
				Breed = "Pine siskin",
				Type = "Birth",
				Age = 2,
				Gender = "Male",
				IsHealthy = true
			};

			var newAnimal2 = new Animal()
			{
				CatalogNumber = "01HNTWXTR0E5",
				Name = "Laura",
				Breed = "Oriental short-clawed otter",
				Type = "Mammal",
				Age = 5,
				Gender = "Female",
				IsHealthy = true
			};

			await animalsManager.AddAsync(newAnimal1);
			await animalsManager.AddAsync(newAnimal2);

			// Act && Assert
			var expectation = Assert.ThrowsAsync<KeyNotFoundException>(() => animalsManager.SearchByTypeAsync(invalidTypeValue));

			Assert.That(expectation.Message, Is.EqualTo(errorMessage));
		}

        [Test]
        public async Task GetSpecificAsync_WithValidCatalogNumber_ShouldReturnAnimal()
        {
			// Arrange
			var newAnimal1 = new Animal()
			{
				CatalogNumber = "01HNTWXTR07A",
				Name = "Steve",
				Breed = "Pine siskin",
				Type = "Birth",
				Age = 3,
				Gender = "Male",
				IsHealthy = true
			};

			var newAnimal2 = new Animal()
			{
				CatalogNumber = "01HNTWXTQYTY",
				Name = "Charly",
				Breed = "Chilean flamingo",
				Type = "Birth",
				Age = 4,
				Gender = "Male",
				IsHealthy = true
			};

			await animalsManager.AddAsync(newAnimal1);
			await animalsManager.AddAsync(newAnimal2);

			// Act
			var animal = await animalsManager.GetSpecificAsync(newAnimal1.CatalogNumber);

			// Assert
			Assert.IsNotNull(animal);
			Assert.That(animal.CatalogNumber, Is.EqualTo(newAnimal1.CatalogNumber));
			Assert.That(animal.Name, Is.EqualTo(newAnimal1.Name));
			Assert.That(animal.Breed, Is.EqualTo(newAnimal1.Breed));
			Assert.That(animal.Type, Is.EqualTo(newAnimal1.Type));
			Assert.That(animal.Age, Is.EqualTo(newAnimal1.Age));
			Assert.That(animal.Gender, Is.EqualTo(newAnimal1.Gender));
		}

        [TestCase("999")]
		[TestCase("1234567890A")]
		[TestCase("1234567890ABC")]
		[TestCase("999AJFJ78JBSAJ7995FJKAFBJ64FJVG45757")]
		public async Task GetSpecificAsync_WithInvalidCatalogNumber_ShouldThrowKeyNotFoundException(string invalidCatalogNumber)
        {
			// Arrange
			var errorMessage = $"No animal found with catalog number: {invalidCatalogNumber}";

			// Act && Assert
			var expectation = Assert.ThrowsAsync<KeyNotFoundException>(() => animalsManager.GetSpecificAsync(invalidCatalogNumber));

			Assert.That(expectation.Message, Is.EqualTo(errorMessage));
		}

        [Test]
        public async Task UpdateAsync_WithValidAnimal_ShouldUpdateAnimal()
        {
			// Arrange
			var newAnimal = new Animal()
			{
				CatalogNumber = "10HNTYYYR07A",
				Name = "Jhon",
				Breed = "Pine siskin",
				Type = "Birth",
				Age = 3,
				Gender = "Male",
				IsHealthy = true
			};

			var newAnimal2 = new Animal()
			{
				CatalogNumber = "03FDTHGHR0E7",
				Name = "Laura",
				Breed = "Oriental short-clawed otter",
				Type = "Mammal",
				Age = 7,
				Gender = "Female",
				IsHealthy = false
			};

			await animalsManager.AddAsync(newAnimal);
			await animalsManager.AddAsync(newAnimal2);

			var animalNewValues = newAnimal;
			animalNewValues.Name = "Jo";
			animalNewValues.Age = 4;
			animalNewValues.IsHealthy = false;

			// Act
			await animalsManager.UpdateAsync(animalNewValues);

			// Assert
			var animalUpdatedInformation = await animalsManager.GetSpecificAsync(newAnimal.CatalogNumber);

			Assert.That(animalUpdatedInformation.CatalogNumber, Is.EqualTo(newAnimal.CatalogNumber));
			Assert.That(animalUpdatedInformation.Name, Is.EqualTo(newAnimal.Name));
			Assert.That(animalUpdatedInformation.Breed, Is.EqualTo(newAnimal.Breed));
			Assert.That(animalUpdatedInformation.Type, Is.EqualTo(newAnimal.Type));
			Assert.That(animalUpdatedInformation.Age, Is.EqualTo(newAnimal.Age));
			Assert.That(animalUpdatedInformation.Gender, Is.EqualTo(newAnimal.Gender));

		}

        [Test]
        public async Task UpdateAsync_WithInvalidAnimal_ShouldThrowValidationException()
        {
			// Arrange
			var errorMessage = "Invalid animal!";

			var newAnimal = new Animal()
			{
				CatalogNumber = "12FDTHXXR0E7",
				Name = "Steve",
				Breed = "Pine siskin",
				Type = "Birth",
				Age = 3,
				Gender = "Male",
				IsHealthy = true
			};

			await animalsManager.AddAsync(newAnimal);

			var animalNewInvalidValues = newAnimal;
			animalNewInvalidValues.Name = "Hubert Blaine Wolfeschlegelsteinhausenbergerdorff";
			animalNewInvalidValues.Age = -3;

			// Act && Assert
			var expectation = Assert.ThrowsAsync<ValidationException>(() => animalsManager.UpdateAsync(animalNewInvalidValues));

			Assert.That(expectation.Message, Is.EqualTo(errorMessage));

		}
    }
}

