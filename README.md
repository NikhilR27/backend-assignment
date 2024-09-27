# Online Betting Data Capture and Analytics System
## Note
I changed the database credentials because I'm using a Mac
## Scenario
In the dynamic world of online betting, capturing and analyzing data is paramount for business success. Every time a player initiates a spin in a casino game, it's essential to capture and store relevant data. This project aims to develop an API and service capable of receiving, storing, and retrieving player casino data.

## Pre-Requisites
- **RabbitMQ:** Setup a local RabbitMQ instance running on `localhost` with credentials `guest/guest`.
    > [Run RabbitMQ Windows Service](https://www.rabbitmq.com/docs/install-windows#installer) or
 
    > [Docker Compose Rabbit Image](docker/RabbitMQ/docker-up.bat)
- **SQL Server:** Setup a local SQL Server 2022 Developer Edition with the connection string: `"SERVER=localhost; DATABASE=OT_Assessment_DB; Integrated Security=SSPI;"`.

## Requirements Tasks
### 1. .NET 8 API
The API must include the following endpoints:
- **POST** `api/player/casinowager`: Receives player casino wager events to publish to the local RabbitMQ queue.
    ```json
    {
      "wagerId": "aa6700eb-1a06-483e-9739-d293dc7a9383",
      "theme": "adventure",
      "provider": "Ergonomic Soft Fish",
      "gameName": "Ergonomic Granite Cheese",
      "transactionId": "410b7161-3473-4d74-85c3-a533d050a9d3",
      "brandId": "8a2016f8-c4c4-471f-9a9c-337a54664650",
      "accountId": "5ac75fec-23e9-27d1-b660-179eee70003d",
      "Username": "Jay.Bernhard67",
      "externalReferenceId": "0267dbca-2760-4a9e-ab42-5ce766fa8ca0",
      "transactionTypeId": "8aaece0c-5d53-4225-a937-adb454c4da31",
      "amount": 38273.974454660885,
      "createdDateTime": "2024-05-04T02:25:05.9906387+02:00",
      "numberOfBets": 3,
      "countryCode": "BS",
      "sessionData": "Central Chile Awesome Cotton Gloves cross-platform Handmade Rubber Shoes portals leading-edge Coordinator Data Producer end-to-end encoding Gorgeous Clothing View Health, Outdoors & Music embrace Metrics Facilitator morph",
      "duration": 1827254
    }
    ```
- **GET** `api/player/{playerId}/casino`: Returns a paginated list of the latest casino wagers for a specific player.
    ```
    {
      "data": [
        {
          "wagerId": "string",
          "game": "string",
          "provider": "string",
          "amount": 0,
          "createdDate": "2024-07-10T07:25:13.577Z"
        }
      ],
      "page": 0,
      "pageSize": 0,
      "total": 0,
      "totalPages": 0
    }
    ```
- **GET** `api/player/topSpenders?count=10`: Returns the top {count} players based on their total spending.
    ```json
    {
        "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "username": "string",
        "totalAmountSpend": 0
    }
    ```
### 2. .NET Service
This service will consume messages published to the aforementioned queue and store consumed messages in a database:
- Save Player Account to database if it does not already exist.
- Save entries to the Player Casino Wager table.

### 3. SQL Database
Design tables, indexes, keys, and stored procedures necessary for populating and retrieving data from:
- **Player Account Table**: Includes fields for Account ID and Username.
- **Player CasinoWager Table**: Contains fields for Wager ID, Game, Provider, Account ID, Amount, and Created DateTime.
- **DatabaseGenerate.sql**: Populate the file in the root of the directory with the necessary statements to recreate the database.

## BONUS
Surprise us! Add somethings to your project you think are valuable additions.


## Scoring Criteria
- Completion of all requirement tasks.
- Architecture and project structure.
- Naming conventions for variables, projects, tables, and stored procedures.
- Adherence to SOLID principles.
- Readability and maintainability of the code.
- Efficient database design.
- Efficiently consuming the data from the tester application.
- Proficiency and adherence to best practices for .NET 8 Projects, RabbitMQ, REST, Git, and SQL Server.

## FAQ
- Can I modify the suggested solutions, projects, and classes?  ==Yes, but ensure all prerequisites and tasks are met.==
- What technologies should I use? ==Feel free to use any packages you're comfortable with, though we recommend using built-in .NET 8 libraries, RabbitMQ Client, and Dapper for ORM.==
- How do I test my application? ==Run the tester application to simulate sending casino game data to your API. Ensure both GET routes return the correct data.==
- How is my application tested and assessed? ==We'll generate the database from your schema script, start the API and consumer, run the tester application, and evaluate against the scoring criteria.==
- Need clarification? ==Compile a list of questions and send them to your OT representative for prompt assistance.==
- What's next after completing the project? ==Ensure your database schema script is ready, commit your code to a public GitHub repository, share the link with your OT representative, and consider enhancements for production readiness.==

Try to remember and briefly document the decisions you made and why you made them. We may ask you in a follow-up interview.
