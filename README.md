# Integration Testing
Integration testing is a software testing technique that focuses on verifying the interactions and data exchange between different components or modules of a software application. The goal of integration testing is to identify any problems or bugs that arise when different components are combined and interact with each other.

## Details:
Performing integration testing on several different applications using variety of QA testing frameworks. 

### 1. ContactsConsoleAPI:
Application with console UI for adding, updating, searching and deleting contacts. Each contact has id, first name, last name, address, phone number, email, gender, contact ulid. <br/>

Build with:
- C#

Integration testing with:
- NUnit

### 2. ZooConsoleAPI:
Application with console UI for adding, updating, searching and deleting animals. Each animal has id, catalog number, name, breed, type, age, gender, health status. <br/>

Build with:
- C#

Integration testing with:
- NUnit

### 3. TestingGithubAPI-UsingRestSharp:
The application is interacting with Github's API endpoints to:
- create issues, labels and comments
- get information about issues and comments/labels related to given issue
- edit and delete comments

Build with:
- C#

Integration testing with:
- RestSharp and NUnit

### 4. Eventmi-App:
Application for event managing. The following methods are available: create/edit/delete events, get all events, get an event by ID.

Build with:
- C#

Integration testing with:
- RestSharp and NUnit