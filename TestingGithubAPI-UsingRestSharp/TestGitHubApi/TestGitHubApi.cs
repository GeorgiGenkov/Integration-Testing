using RestSharpServices;
using System.Net;
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using NUnit.Framework.Internal;
using RestSharpServices.Models;
using System;

namespace TestGitHubApi
{
    public class TestGitHubApi
    {
        private GitHubApiClient client;
        private static string repo;
        private static string usernameGitHub = "your_username";
        private static string tokenGitHub = "your_token";
        private static int lastCreatedIssueNumber;
		private static int lastCreatedCommentId;


		[SetUp]
        public void Setup()
        {            
            client = new GitHubApiClient("https://api.github.com/repos/testnakov/", usernameGitHub, tokenGitHub);
            repo = "test-nakov-repo";
        }


        [Test, Order (1)]
        public void Test_GetAllIssuesFromARepo()
        {
            // Arrange

            // Act
			var issues = client.GetAllIssues(repo);

            // Assert
            Assert.That(issues, Has.Count.GreaterThan(0), "There should be at least one issue");

			foreach(Issue issue in issues)
			{
				Assert.That(issue.Id, Is.GreaterThan(0), "Issue ID should be greater than 0");
				Assert.That(issue.Number, Is.GreaterThan(0), "Issue Number should be greater than 0");
				Assert.That(issue.Title, Is.Not.Empty, "Issue Title should not be empty");
			}
        }

        [Test, Order (2)]
        public void Test_GetIssueByValidNumber()
        {
			// Arrange
			int issueNumber = 3;

			// Act
			var issue = client.GetIssueByNumber(repo, issueNumber);

			// Assert
			Assert.That(issue.Id, Is.GreaterThan(0), "Issue ID should be greater than 0");
			Assert.That(issue.Number, Is.EqualTo(issueNumber), "Issue Number should match with issueNumber");
			Assert.That(issue, Is.Not.Null, "The response should contains issue data");
		}

		[Test, Order (3)]
        public void Test_GetAllLabelsForIssue()
        {
			// Arrange
			int issueNumber = 8;

			// Act
			var labels = client.GetAllLabelsForIssue(repo, issueNumber);

			// Assert
			Assert.That(labels, Has.Count.GreaterThan(0), "There should be at least one label");

			foreach (Label label in labels)
			{
				Assert.That(label.Id, Is.GreaterThan(0), "Label ID should be greater than 0");
				Assert.That(label.Name, Is.Not.Empty, "Label Name should not be empty");

				// Print the body of each label
				Console.WriteLine($"LabelID: {label.Id} - Name: {label.Name}");
			}
		}

		[Test, Order (4)]
        public void Test_GetAllCommentsForIssue()
        {
			// Arrange
			int issueNumber = 12;

			// Act
			var comments = client.GetAllCommentsForIssue(repo, issueNumber);

			// Assert
			Assert.That(comments, Has.Count.GreaterThan(0), "There should be at least one comment");

			foreach (Comment commnet in comments)
			{
				Assert.That(commnet.Id, Is.GreaterThan(0), "Comment ID should be greater than 0");
				Assert.That(commnet.Body, Is.Not.Empty, "Comment body should not be empty");

				// Print the body of each label
				Console.WriteLine($"CommentID: {commnet.Id} - Body: {commnet.Body}");
			}
		}

		[Test, Order(5)]
        public void Test_CreateGitHubIssue()
        {
			// Arrange
			string title = "Title for testing purposes";
			string body = "Issue body description";

			// Act
			var issue = client.CreateIssue(repo, title, body);

			// Assert
			Assert.Multiple(() => 
			{
				Assert.That(issue.Id, Is.GreaterThan(0), "Issue ID should be greater than 0");
				Assert.That(issue.Number, Is.GreaterThan(0), "Issue Number should be greater than 0");
				Assert.That(issue.Title, Is.Not.Empty, "Issue Title should not be empty");
				Assert.That(issue.Title, Is.EqualTo(title), "Issue Title should match with the title value");
			});

			// Print issue number and store it for testing purposes
			Console.WriteLine(issue.Number);
			lastCreatedIssueNumber = issue.Number;
		}

		[Test, Order (6)]
        public void Test_CreateCommentOnGitHubIssue()
        {
			// Arrange
			string body = "Comment body description";
			int issueNumber = lastCreatedIssueNumber;

			// Act
			var comment = client.CreateCommentOnGitHubIssue(repo, issueNumber, body);

			// Assert
			Assert.That(comment.Body, Is.EqualTo(body));

			// Print comment ID and store it for testing purposes
			Console.WriteLine(comment.Id);
			lastCreatedCommentId = comment.Id;

		}

		[Test, Order (7)]
        public void Test_GetCommentById()
        {
			// Arrange


			// Act


			// Assert

		}


		[Test, Order (8)]
        public void Test_EditCommentOnGitHubIssue()
        {
			// Arrange


			// Act


			// Assert

		}

		[Test, Order (9)]
        public void Test_DeleteCommentOnGitHubIssue()
        {
			// Arrange


			// Act


			// Assert

		}


	}
}

