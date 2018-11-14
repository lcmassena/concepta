# Concepta
This solution is part of the test made to prove concepts for Concepta

In this solution, I am using Event Sourcing with Mediart as execution engine for commands, queries and events.

The solution contains 5 projects as described:
- Hexagonal.API
	An API using .net core, where commands and queries are triggered
- Application
	The core of business, following the DDD concept, where rules are described
- Repositories
	I've created a repository just for explanation purpouses.
	Called AzureTables, just contains a list of users/authentication.
- Services
	Where logics related to third part systems are located and executed.
	In this case just for TravelLogix API.
- Core
	Main components for resilience, dependency injection, extensions and more.
	This is a personal collection of best practices organized like a small framework.  
 
## Testing the solution

To facilitate the tests, I've created the settings for Postman.
First, import the settings on Postman using the file below:
https://github.com/lcmassena/concepta/blob/master/docs/Concepta%20-%20Test.postman_collection.json

#### Starting the application
- Open the .sln file on Visual Studio
- Start debugging - Check if the API started on door 1404

#### Requesting data to the API
##### To request a token
- With postman opened, open the **Concepta - Test** collection
- Open and run the **Token** request
  
##### To request an availability
- With postman opened, open the **Concepta - Test** collection
- Open and run the **Ticket Availability** request

#### Testing TravelLogix response time
*This requests do not share tokens with API requests*
##### To request a TravelLogix token
- With postman opened, open the **Concepta - Test** collection
- Open and run the **TravelLogix Token** request
  
##### To request an availability
- With postman opened, open the **Concepta - Test** collection
- - Open and run the **TravelLogix Search** request
