using ContactsConsoleAPI.Business;
using ContactsConsoleAPI.Business.Contracts;
using ContactsConsoleAPI.Data.Models;
using ContactsConsoleAPI.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ContactsConsoleAPI.IntegrationTests.NUnit
{
	public class IntegrationTests
    {
        private TestContactDbContext dbContext;
        private IContactManager contactManager;

        [SetUp]
        public void SetUp()
        {
            this.dbContext = new TestContactDbContext();
            this.contactManager = new ContactManager(new ContactRepository(this.dbContext));
        }


        [TearDown]
        public void TearDown()
        {
            this.dbContext.Database.EnsureDeleted();
            this.dbContext.Dispose();
        }


        // positive test
        [Test]
        public async Task AddContactAsync_ShouldAddNewContact()
        {
            // Arrange
            var newContact = new Contact()
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                Address = "Ruse",
                Contact_ULID = "1ABC23456HH", // Contact_ULID must be minimum 10 symbols - numbers or Upper case letters
				Email = "test@gmail.com",
                Gender = "Male",
                Phone = "0889933779"
            };

            // Act
            await contactManager.AddAsync(newContact);

            // Assert
            var dbContact = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Contact_ULID == newContact.Contact_ULID);

            Assert.NotNull(dbContact);
            Assert.That(dbContact.FirstName, Is.EqualTo(newContact.FirstName));
            Assert.That(dbContact.LastName, Is.EqualTo(newContact.LastName));
            Assert.That(dbContact.Phone, Is.EqualTo(newContact.Phone));
            Assert.That(dbContact.Email, Is.EqualTo(newContact.Email));
            Assert.That(dbContact.Address, Is.EqualTo(newContact.Address));
            Assert.That(dbContact.Contact_ULID, Is.EqualTo(newContact.Contact_ULID));
        }

        // negative test
        [Test]
        public async Task AddContactAsync_TryToAddContactWithInvalidCredentials_ShouldThrowException()
        {
            // Arrange
            var newContact = new Contact()
            {
                FirstName = "Asen",
                LastName = "Asenov",
                Address = "Asenovgrad",
                Contact_ULID = "1ABC23456HF",
                Email = "invalid_Mail", // invalid email
                Gender = "Male",
                Phone = "0889933770"
            };

            var errorMessage = "Invalid contact!";

			// Act & Assert
			var expectation = Assert.ThrowsAsync<ValidationException>(() => contactManager.AddAsync(newContact));
            var actual = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Contact_ULID == newContact.Contact_ULID);

            Assert.IsNull(actual);
            Assert.That(expectation?.Message, Is.EqualTo(errorMessage));

        }

        [Test]
        public async Task DeleteContactAsync_WithValidULID_ShouldRemoveContactFromDb()
        {
			// Arrange
			var newContact = new Contact()
			{
				FirstName = "Maya",
				LastName = "Ivanova",
				Address = "Burgas",
				Contact_ULID = "3AAA23456HH",
                Email = "test@gmail.com",
				Gender = "Female",
				Phone = "0888933779"
			};

            await contactManager.AddAsync(newContact);
            var ulid = newContact.Contact_ULID;

            // Act
            await contactManager.DeleteAsync(ulid);

            // Assert
            var actual = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Contact_ULID == ulid);

            Assert.IsNull(actual);
        }

        [Test]
        public async Task DeleteContactAsync_TryToDeleteWithNullOrWhiteSpaceULID_ShouldThrowException()
        {
            // Arrange
            var errorMessage = "ULID cannot be empty.";

			// Act && Assert
			var expectation = Assert.ThrowsAsync<ArgumentException>(() => contactManager.DeleteAsync(""));

            Assert.That(expectation?.Message, Is.EqualTo(errorMessage));
        }

        [Test]
        public async Task GetAllAsync_WhenContactsExist_ShouldReturnAllContacts()
        {
			// Arrange
			var newContact1 = new Contact()
			{
				FirstName = "Silvia",
				LastName = "Medeva",
				Address = "Sofia",
				Contact_ULID = "1AAA23456HH",
				Email = "test1@gmail.com",
				Gender = "Female",
				Phone = "0887933779"
			};

			var newContact2 = new Contact()
			{
				FirstName = "Stefan",
				LastName = "Ivanov",
				Address = "Plovdiv",
				Contact_ULID = "2AAA23456HH",
				Email = "test2@gmail.com",
				Gender = "Male",
				Phone = "0886933779"
			};

			var newContact3 = new Contact()
			{
				FirstName = "Simeon",
				LastName = "Ivanov",
				Address = "Pleven",
				Contact_ULID = "3AAA23456HH",
				Email = "test3@gmail.com",
				Gender = "Male",
				Phone = "0885933779"
			};

            await contactManager.AddAsync(newContact1);
			await contactManager.AddAsync(newContact2);
			await contactManager.AddAsync(newContact3);

			// Act
            var contacts = await contactManager.GetAllAsync();

			// Assert
			var firstContact = contacts.First();

			Assert.That(contacts.Count, Is.EqualTo(3));

            Assert.That(firstContact.FirstName, Is.EqualTo(newContact1.FirstName));
			Assert.That(firstContact.LastName, Is.EqualTo(newContact1.LastName));
			Assert.That(firstContact.Address, Is.EqualTo(newContact1.Address));
			Assert.That(firstContact.Contact_ULID, Is.EqualTo(newContact1.Contact_ULID));
			Assert.That(firstContact.Email, Is.EqualTo(newContact1.Email));
			Assert.That(firstContact.Gender, Is.EqualTo(newContact1.Gender));
			Assert.That(firstContact.Phone, Is.EqualTo(newContact1.Phone));

		}

        [Test]
        public async Task GetAllAsync_WhenNoContactsExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var errorMessage = "No contact found.";

			// Act && Assert
			var expectation = Assert.ThrowsAsync<KeyNotFoundException>(() => contactManager.GetAllAsync());

            Assert.That(expectation?.Message, Is.EqualTo(errorMessage));
        }

        [Test]
        public async Task SearchByFirstNameAsync_WithExistingFirstName_ShouldReturnMatchingContacts()
        {
			// Arrange
			var newContact = new Contact()
			{
				FirstName = "Zara",
				LastName = "Popova",
				Address = "Lom",
				Contact_ULID = "1BAA23456HH",
				Email = "test@gmail.com",
				Gender = "Female",
				Phone = "0884933779"
			};

            await contactManager.AddAsync(newContact);

            // Act
            var contacts = await contactManager.SearchByFirstNameAsync(newContact.FirstName);

			// Assert
			var firstContact = contacts.First();

			Assert.That(contacts.Count(), Is.EqualTo(1));

			Assert.That(firstContact.FirstName, Is.EqualTo(newContact.FirstName));
			Assert.That(firstContact.LastName, Is.EqualTo(newContact.LastName));
			Assert.That(firstContact.Address, Is.EqualTo(newContact.Address));
			Assert.That(firstContact.Contact_ULID, Is.EqualTo(newContact.Contact_ULID));
			Assert.That(firstContact.Email, Is.EqualTo(newContact.Email));
			Assert.That(firstContact.Gender, Is.EqualTo(newContact.Gender));
			Assert.That(firstContact.Phone, Is.EqualTo(newContact.Phone));
		}

        [Test]
        public async Task SearchByFirstNameAsync_WithNonExistingFirstName_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var firstNameNonExistingInContacts = "Plamen";

			var newContact = new Contact()
			{
				FirstName = "Zara",
				LastName = "Popova",
				Address = "Lom",
				Contact_ULID = "1BAA23456HH",
				Email = "test@gmail.com",
				Gender = "Female",
				Phone = "0884933779"
			};

			await contactManager.AddAsync(newContact);

            var errorMessage = "No contact found with the given first name.";

			// Act && Assert
			var expectation = Assert.ThrowsAsync<KeyNotFoundException>(() => contactManager.SearchByFirstNameAsync(firstNameNonExistingInContacts));
            Assert.That(expectation?.Message, Is.EqualTo(errorMessage));
		}

        [Test]
        public async Task SearchByLastNameAsync_WithExistingLastName_ShouldReturnMatchingContacts()
        {
            // Arrange
            var lastNameExistingInContacts = "Popova";

			var newContact1 = new Contact()
			{
				FirstName = "Zara",
				LastName = "Popova",
				Address = "Lom",
				Contact_ULID = "1BAA23456HH",
				Email = "test1@gmail.com",
				Gender = "Female",
				Phone = "0884933709"
			};

			var newContact2 = new Contact()
			{
				FirstName = "Desi",
				LastName = "Popova",
				Address = "Vidin",
				Contact_ULID = "7BAC23456HC",
				Email = "test2@gmail.com",
				Gender = "Female",
				Phone = "0885555779"
			};

			var newContact3 = new Contact()
			{
				FirstName = "Valentin",
				LastName = "Enev",
				Address = "Stara Zagora",
				Contact_ULID = "5BC23456AH",
				Email = "test3@gmail.com",
				Gender = "Male",
				Phone = "0885345679"
			};

			await contactManager.AddAsync(newContact1);
			await contactManager.AddAsync(newContact2);
			await contactManager.AddAsync(newContact3);

			// Act
			var contacts = await contactManager.SearchByLastNameAsync(lastNameExistingInContacts);

			// Assert
			Assert.That(contacts.Count(), Is.EqualTo(2));

		}

        [Test]
        public async Task SearchByLastNameAsync_WithNonExistingLastName_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var lastNameNonExistingInContacts = "Botev";
            var errorMessage = "No contact found with the given last name.";

			// Act && Assert
			var expectation = Assert.ThrowsAsync<KeyNotFoundException>(() => contactManager.SearchByLastNameAsync(lastNameNonExistingInContacts));
            Assert.That(expectation?.Message, Is.EqualTo(errorMessage));
        }

        [Test]
        public async Task GetSpecificAsync_WithValidULID_ShouldReturnContact()
        {
			// Arrange
			var ulidExistingInContacts = "5BC23456AH";

			var newContact1 = new Contact()
			{
				FirstName = "Zara",
				LastName = "Popova",
				Address = "Lom",
				Contact_ULID = "1BAA23456HH",
				Email = "test1@gmail.com",
				Gender = "Female",
				Phone = "0884933709"
			};

			var newContact2 = new Contact()
			{
				FirstName = "Desi",
				LastName = "Popova",
				Address = "Vidin",
				Contact_ULID = "7BAC23456HC",
				Email = "test2@gmail.com",
				Gender = "Female",
				Phone = "0885555779"
			};

			var newContact3 = new Contact()
			{
				FirstName = "Valentin",
				LastName = "Enev",
				Address = "Stara Zagora",
				Contact_ULID = "5BC23456AH",
				Email = "test3@gmail.com",
				Gender = "Male",
				Phone = "0885345679"
			};

			await contactManager.AddAsync(newContact1);
			await contactManager.AddAsync(newContact2);
			await contactManager.AddAsync(newContact3);

			// Act
			var contact = await contactManager.GetSpecificAsync(ulidExistingInContacts);

			// Assert
			Assert.IsNotNull(contact);
			Assert.That(contact.FirstName, Is.EqualTo(newContact3.FirstName));
			Assert.That(contact.LastName, Is.EqualTo(newContact3.LastName));
			Assert.That(contact.Address, Is.EqualTo(newContact3.Address));
			Assert.That(contact.Contact_ULID, Is.EqualTo(newContact3.Contact_ULID));
			Assert.That(contact.Email, Is.EqualTo(newContact3.Email));
			Assert.That(contact.Gender, Is.EqualTo(newContact3.Gender));
			Assert.That(contact.Phone, Is.EqualTo(newContact3.Phone));

		}

        [Test]
        public async Task GetSpecificAsync_WithInvalidULID_ShouldThrowKeyNotFoundException()
        {
			// Arrange
			var ulidNonExistingInContacts = "7BAC23456HC";
			var errorMessage = $"No contact found with ULID: {ulidNonExistingInContacts}";

			// Act && Assert
			var expectation = Assert.ThrowsAsync<KeyNotFoundException>(() => contactManager.GetSpecificAsync(ulidNonExistingInContacts));
			Assert.That(expectation?.Message, Is.EqualTo(errorMessage));
		}

        [Test]
        public async Task UpdateAsync_WithValidContact_ShouldUpdateContact()
        {
			// Arrange
			var newContact = new Contact()
			{
				FirstName = "Stella",
				LastName = "Smith",
				Address = "Svoge",
				Contact_ULID = "7BAC77456HC",
				Email = "test0@gmail.com",
				Gender = "Female",
				Phone = "0883355779"
			};

			var updatedInformation = newContact;
			updatedInformation.Address = "Silistra";
			updatedInformation.Email = "test777@gmail.com";
			// Act
			await contactManager.UpdateAsync(updatedInformation);

			// Assert
			var updatedContact = await contactManager.GetSpecificAsync(newContact.Contact_ULID);

			Assert.That(updatedContact.FirstName, Is.EqualTo(newContact.FirstName));
			Assert.That(updatedContact.LastName, Is.EqualTo(newContact.LastName));
			Assert.That(updatedContact.Address, Is.EqualTo(updatedInformation.Address));
			Assert.That(updatedContact.Contact_ULID, Is.EqualTo(newContact.Contact_ULID));
			Assert.That(updatedContact.Email, Is.EqualTo(updatedInformation.Email));
			Assert.That(updatedContact.Gender, Is.EqualTo(newContact.Gender));
			Assert.That(updatedContact.Phone, Is.EqualTo(newContact.Phone));

		}

        [Test]
        public async Task UpdateAsync_WithInvalidContact_ShouldThrowValidationException()
        {
			// Act && Assert
			var expectation = Assert.ThrowsAsync<ValidationException>(() => contactManager.UpdateAsync(new Contact()));
		}
    }
}
