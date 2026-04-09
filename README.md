# Learn Well University – Course Management System (PoC)

A Course Management System for Learn Well University, comprising a .NET 9 Web API backend.

## Tech Stack

**Backend**

- **.NET 9**
- **Entity Framework Core 9**
- **Swagger (Open API)**
- **PostgreSQL (DB)**
- **JWT (Bearer Authentication)**
- **Serilog + Seq (Logging)**
- **xUnit (Testing)**

**Frontend**

- **Angular 21**
- **PrimeNG 21**

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — must be **running** before executing any `docker` commands

### Environment Setup

- In the root folder, create **.env** file by following the exact format from [env.example](/env.example) file.
- Fill in the required environment variables.

## Running the Application

```bash
docker compose up --build -d          # start containers in detached mode
```

Once running, the containers start on an internal bridge network:

| Container | Exposed Port | Description                |
| --------- | ------------ | -------------------------- |
| `api`     | `8080`       | .NET 9 Web API             |
| `db`      | *(none)*     | PostgreSQL — internal only |
| `seq`     | `8081` (UI)  | Structured log viewer      |
| `frontend`| `8080` (UI) | Angular 21 Frontend        |

The API will automatically apply EF migrations and seed a default admin account on startup.

## API

### Swagger UI

<http://localhost:8080/swagger> — interactive API docs.
          
**Auth Rule**
- All endpoints require `Authorization: Bearer <token>` except `POST /api/auth/login`.

**Endpoints**
| Method | Path | Operation | Access |
|---|---|---|---|
| POST | `/api/auth/login` | Log in | Public |
| POST | `/api/staff` | Add staff user | Staff |
| POST | `/api/student` | Create student | Staff |
| GET | `/api/student` | Get students (filtered, paged) | Staff |
| GET | `/api/student/{id}` | Get student by id | Staff |
| PUT | `/api/student/{id}` | Update student | Staff |
| DELETE | `/api/student/{id}` | Delete student | Staff |
| GET | `/api/student/getOtherStudents/{classId}` | Get other students in class | Student |
| GET | `/api/student/getClasses/{studentId}` | Get classes for a student id | Staff |
| GET | `/api/student/getClasses` | Get current student classes | Student |
| GET | `/api/student/getCourses/{studentId}` | Get courses for a student id | Staff |
| GET | `/api/student/getCourses` | Get current student courses | Student |
| GET | `/api/student/getOwnInfo` | Get current student profile | Student |
| POST | `/api/course` | Create course | Staff |
| GET | `/api/course` | Get courses (filtered, paged) | Staff |
| GET | `/api/course/{id}` | Get course by id | Staff |
| PUT | `/api/course/{id}` | Update course | Staff |
| DELETE | `/api/course/{id}` | Delete course | Staff |
| POST | `/api/course/addRemoveStudents/{courseId}` | Add/remove students in course | Staff |
| GET | `/api/course/getStudents/{courseId}` | Get students in course | Staff |
| GET | `/api/course/getClasses/{courseId}` | Get classes in course | Staff |
| POST | `/api/course/addRemoveClasses/{courseId}` | Add/remove classes in course | Staff |
| POST | `/api/class` | Create class | Staff |
| GET | `/api/class` | Get classes (filtered, paged) | Staff |
| GET | `/api/class/{id}` | Get class by id | Staff |
| PUT | `/api/class/{id}` | Update class | Staff |
| DELETE | `/api/class/{id}` | Delete class | Staff |
| POST | `/api/class/addRemoveStudents/{classId}` | Add/remove students in class | Staff |
| GET | `/api/class/getStudents/{classId}` | Get students in class | Staff |
| GET | `/api/class/getCourses/{classId}` | Get courses associated with class | Staff |

### Import Postman Collection

- Open **Postman** and click **Import** button from **...** menu.
- Paste <http://localhost:8080/swagger/v1/swagger.json> into the **Import** dialog.
- Click **Import** button to import the collection.
- In the collection's variables, set the `baseUrl` to `http://localhost:8080`.
- After successful logging in copy the `Token` property from response and set it in the collection's `Authorization` section's `Token` variable.

## Default Credentials

| Service | Login | Password  |
|---------|-------|-----------|
| API/Frontend (Staff) | admin@learnwell.edu | Admin@123 |
| API/Frontend (Staff) | admin2@learnwell.edu | Admin@123 |
| SEQ ([http://localhost:8081](http://localhost:8081)) | admin | Admin@123 |

## Running Tests

```bash
dotnet test
```

## ER Diagram

![ER Diagram](/diagram/erd.png)

### Edit Steps

1. Go to [Mermaid.ai](https://mermaid.ai/live/edit)
2. Click **Continue to mermaid.ai/live** button.
3. On the left panel's **Code** section, paste the codes from [erd.md](/diagram/erd.md) file

## Stopping the Application

```bash
docker compose down          # stop and remove containers
docker compose down -v       # also remove volumes (wipes the database and seq data)
```

