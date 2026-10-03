\# License Management System



A full-stack License Management application built with \*\*ASP.NET Core Web API, React, SQL Server, and Docker\*\*.



The application provides APIs and a web interface to generate, validate, and revoke software licenses, with license data stored in SQL Server.



\## Tech Stack



\### Backend



\* .NET 10

\* ASP.NET Core Web API

\* C#

\* Entity Framework Core

\* SQL Server

\* Swagger / OpenAPI



\### Frontend



\* React

\* Vite

\* JavaScript

\* HTML / CSS

\* Nginx



\### DevOps / Containerization



\* Docker

\* Docker Compose

\* Docker volumes

\* Multi-container application architecture



\## Features



\* Generate a license

\* Validate a license

\* Revoke a license

\* Store license information in SQL Server

\* RESTful Web API

\* React-based user interface

\* Swagger API documentation

\* Dockerized frontend, backend, and database

\* Persistent SQL Server storage using a Docker volume

\* Environment-based configuration using `.env`



\## Application Architecture



```text

&#x20;                  Browser

&#x20;                     |

&#x20;                     v

&#x20;             React Application

&#x20;               localhost:3000

&#x20;                     |

&#x20;                     v

&#x20;              Nginx Container

&#x20;                     |

&#x20;                     v

&#x20;             ASP.NET Core API

&#x20;               localhost:8080

&#x20;                     |

&#x20;                     v

&#x20;             SQL Server Container

&#x20;               localhost:1433

&#x20;                     |

&#x20;                     v

&#x20;          LicenseManagementDb

```



\## Project Structure



```text

LicenseManagement/

│

├── LicenseManagement.API/

│   ├── Controllers/

│   ├── Data/

│   ├── Models/

│   ├── Services/

│   ├── Program.cs

│   ├── appsettings.json

│   ├── appsettings.Docker.json

│   └── Dockerfile

│

├── LicenseManagement.UI/

│   ├── src/

│   │   ├── components/

│   │   ├── services/

│   │   ├── App.jsx

│   │   └── main.jsx

│   ├── public/

│   ├── package.json

│   ├── nginx.conf

│   └── Dockerfile

│

├── compose.yml

├── .gitignore

├── LicenseManagement.slnx

└── README.md

```



\## API Endpoints



Base URL:



```text

http://localhost:8080/api/Licenses

```



\### Generate License



```http

POST /api/Licenses/generate

```



Creates a new license and stores it in the database.



\### Validate License



```http

POST /api/Licenses/validate

```



Validates the supplied license information.



\### Revoke License



```http

POST /api/Licenses/revoke

```



Revokes an existing license.



\## Swagger



When the Docker containers are running, Swagger is available at:



```text

http://localhost:8080/swagger

```



Swagger can be used to test and explore the API endpoints.



\## Running with Docker Compose



\### Prerequisites



Install:



\* Docker Desktop

\* Git



\### 1. Clone the repository



```bash

git clone https://github.com/rutujamokashi/LicenseManagement.git

cd LicenseManagement

```



\### 2. Create `.env`



Create a `.env` file in the project root.



Example:



```env

SQL\_PASSWORD=your\_sql\_password

SQL\_DATABASE=LicenseManagementDb

SQL\_USER=sa

```



> Do not commit `.env` to GitHub. It is excluded through `.gitignore`.



\### 3. Start the application



```bash

docker compose up -d --build

```



\### 4. Check running containers



```bash

docker compose ps

```



Expected services:



```text

sqlserver

api

ui

```



\### 5. Open the application



Frontend:



```text

http://localhost:3000

```



Swagger:



```text

http://localhost:8080/swagger

```



\## Docker Services



| Service          | Container                | Port |

| ---------------- | ------------------------ | ---: |

| React UI         | `license-management-ui`  | 3000 |

| ASP.NET Core API | `license-management-api` | 8080 |

| SQL Server       | `crn-sqlserver`          | 1433 |



\## Database



Database:



```text

LicenseManagementDb

```



The SQL Server data is persisted using a Docker volume so that database data is retained when containers are recreated.



The application uses Entity Framework Core for database access.



\## Useful Docker Commands



Start services:



```bash

docker compose up -d

```



Build images:



```bash

docker compose build

```



Rebuild and start:



```bash

docker compose up -d --build

```



View containers:



```bash

docker compose ps

```



View API logs:



```bash

docker compose logs api

```



View UI logs:



```bash

docker compose logs ui

```



Stop services:



```bash

docker compose down

```



\## Environment Configuration



The Docker Compose configuration uses environment variables from `.env`.



The API receives the database connection configuration through Docker Compose environment variables.



The SQL Server password and other credentials should not be committed to source control.



\## GitHub



Repository:



https://github.com/rutujamokashi/LicenseManagement



\## Future Enhancements



Potential improvements include:



\* JWT authentication and authorization

\* Role-based access control

\* License expiration notifications

\* License search and filtering

\* Pagination

\* Automated unit and integration tests

\* CI/CD pipeline using GitHub Actions

\* Azure deployment

\* Application monitoring and logging

\* Production-ready secret management

\* Health checks for Docker services

\* Container orchestration with Kubernetes



\## Author



\*\*Rutuja Mokashi\*\*



Full Stack .NET Developer



Technologies: C# | ASP.NET Core | Web API | React | Angular | SQL Server | Entity Framework Core | Docker | Azure



