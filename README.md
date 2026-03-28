# Learn Well University – Course Management System (PoC)

A Course Management System for Learn Well University, comprising a .NET 9 Web API backend.

## Tech Stack

**Backend**

- **.NET 9**
- **Entity Framework Core 9**
- **Swagger (Open API)**
- **PostgreSQL (latest)**

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — must be **running** before executing any `docker` commands

### Environment Setup

- In the root folder, create **.env** file by following the exact format from [env.example](/env.example) file.
- Fill in the required environment variables.

## Running the Application

```bash
docker compose up --build
```

Once running, the containers start on an internal bridge network:

| Container | Exposed Port | Description                |
| --------- | ------------ | -------------------------- |
| `api`     | `8080`       | .NET 9 Web API             |
| `db`      | *(none)*     | PostgreSQL — internal only |

The API will automatically apply EF migrations and seed a default admin account on startup.

## API

### Swagger UI

<http://localhost:8080/swagger> — interactive API docs.

### Import Postman Collection

- Open **Postman** and click **Import** button.
- Paste <http://localhost:8080/swagger/v1/swagger.json> into the **Import** dialog.
- Click **Import** button to import the collection.

## ER Diagram

!\[ER Diagram]\(/diagram/erd.png null)

### Edit Steps

1. Go to [Mermaid.ai](https://mermaid.ai/live/edit)
2. Click **Continue to mermaid.ai/live** button.
3. On the left panel's **Code** section, paste the codes from [erd.md](/diagram/erd.md) file

## Stopping the Application

```bash
docker compose down          # stop and remove containers
docker compose down -v       # also remove volumes (wipes the database)
```

