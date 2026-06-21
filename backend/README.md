# EPMS Backend — ASP.NET Core Web API

Backend untuk Enterprise Project Management System (EPMS), dibangun dengan
**ASP.NET Core 8 Web API**, **Clean Architecture**, **PostgreSQL** (EF Core,
Code-First), dan **Redis** untuk caching/refresh-token storage.

> ⚠️ **Catatan penting:** kode di repository ini ditulis lengkap sesuai
> rencana teknis (`enterprise-epms-spec` → tahapan implementasi), namun
> **belum pernah dijalankan `dotnet restore`/`build` di environment pembuatan
> kode ini** karena tidak ada akses ke NuGet feed di sandbox tersebut.
> **Langkah pertama yang WAJIB Anda lakukan** di komputer Anda sendiri adalah
> `dotnet restore` lalu `dotnet build`, dan perbaiki bila ada error compile
> kecil (typo, versi paket) sebelum lanjut ke langkah berikutnya.

---

## 1. Struktur Proyek

```
EPMS.sln
├── src/
│   ├── EPMS.Domain/           # Entities, enums, domain exceptions
│   ├── EPMS.Application/      # Use cases, DTOs, interfaces, validators
│   ├── EPMS.Infrastructure/   # EF Core, Repository, Redis, JWT, file storage
│   └── EPMS.WebApi/           # Controllers, Middleware, Program.cs
├── tests/
│   ├── EPMS.UnitTests/
│   └── EPMS.IntegrationTests/
├── Dockerfile
├── docker-compose.yml
└── .github/workflows/backend-ci.yml
```

Aturan dependency: `WebApi → Infrastructure → Application → Domain`.

---

## 2. Prasyarat

| Tool | Versi | Cek |
|---|---|---|
| .NET SDK | 8.0 (LTS) | `dotnet --version` |
| PostgreSQL | 14+ (lokal atau Docker) | `psql --version` |
| Redis | 6+ (lokal atau Docker) | `redis-cli --version` |
| EF Core CLI | terbaru | `dotnet tool install --global dotnet-ef` |
| Docker + Docker Compose | (opsional, untuk section 6 & 7) | `docker --version` |

---

## 3. Setup & Menjalankan Secara Lokal (tanpa Docker)

### 3.1. Restore & Build

```bash
cd EPMS
dotnet restore
dotnet build
```

Jika ada error compile, perbaiki dulu sebelum lanjut (lihat catatan di
bagian atas README ini).

### 3.2. Jalankan PostgreSQL & Redis

Opsi A — pakai Docker hanya untuk database (paling cepat untuk dev):

```bash
docker run -d --name epms-postgres \
  -e POSTGRES_DB=epms_db_dev \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 postgres:16-alpine

docker run -d --name epms-redis -p 6379:6379 redis:7-alpine
```

Opsi B — install PostgreSQL & Redis langsung di OS Anda, sesuaikan kredensial
dengan `appsettings.Development.json`.

### 3.3. Sesuaikan konfigurasi

Cek dan sesuaikan `src/EPMS.WebApi/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=epms_db_dev;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Secret": "GANTI_DENGAN_STRING_RANDOM_MINIMAL_32_KARAKTER"
  }
}
```

> 🔐 **Jangan pernah commit secret asli ke git.** File `appsettings.Development.json`
> di repo ini berisi secret placeholder untuk dev — ganti sebelum dipakai
> serius, dan untuk production gunakan environment variable / secret manager,
> bukan file `appsettings.Production.json` yang ikut di-commit.

### 3.4. Buat migration pertama & apply ke database

```bash
dotnet ef migrations add InitialCreate \
  -p src/EPMS.Infrastructure \
  -s src/EPMS.WebApi

dotnet ef database update \
  -p src/EPMS.Infrastructure \
  -s src/EPMS.WebApi
```

Verifikasi tabel sudah terbuat:

```bash
psql -h localhost -U postgres -d epms_db_dev -c "\dt"
```

Anda harus melihat tabel: `Users`, `Projects`, `Tasks`, `TaskAttachments`,
`ActivityLogs`, `RefreshTokens`, plus tabel migration history EF Core.

### 3.5. Jalankan API

```bash
dotnet run --project src/EPMS.WebApi
```

Default: `http://localhost:8080` (lihat `Properties/launchSettings.json`).
Swagger UI otomatis terbuka di `http://localhost:8080/swagger`.

---

## 4. Menjalankan dengan Docker Compose (full stack)

```bash
cp .env.example .env
# edit .env, isi JWT_SECRET dengan string random panjang

docker compose up -d --build
```

Ini akan menjalankan 4 service: `postgres`, `redis`, `backend` (port 8080),
dan `frontend` (port 5173, **mengasumsikan folder `./frontend` sudah ada**
berisi project React + Vite Anda dengan `Dockerfile` sendiri — sesuaikan path
di `docker-compose.yml` jika struktur folder Anda berbeda).

Setelah container `backend` jalan, jalankan migration:

```bash
# Cara paling praktis untuk dev/staging: jalankan dari mesin lokal Anda
# (yang sudah ada SDK) dengan connection string mengarah ke port yang
# di-expose docker-compose.yml (localhost:5432):
dotnet ef database update -p src/EPMS.Infrastructure -s src/EPMS.WebApi \
  --connection "Host=localhost;Port=5432;Database=epms_db;Username=postgres;Password=postgres"
```

> Catatan: image runtime `backend` di Dockerfile ini adalah hasil `publish`
> (tanpa SDK/source code lengkap), sehingga `dotnet ef` tidak bisa dijalankan
> langsung di dalam container tersebut. Alternatif lain: tambahkan
> `dbContext.Database.Migrate();` di awal `Program.cs` agar migration
> otomatis berjalan setiap aplikasi start (cocok untuk dev/staging, kurang
> disarankan untuk production tanpa kontrol rollout migration).

---

## 5. Menyambungkan ke Frontend

Frontend (Vite, React + TypeScript) berjalan di `http://localhost:5173`.
CORS sudah dikonfigurasi di `Program.cs` dan `appsettings.json`
(`Cors:AllowedOrigins`) untuk mengizinkan origin tersebut dengan
`AllowCredentials()` (diperlukan karena refresh token dikirim lewat
HttpOnly Cookie).

Set `.env` di project frontend:

```
VITE_API_BASE_URL=http://localhost:8080/api/v1
```

> ⚠️ Endpoint `/api/v1/auth/login`, `/refresh`, `/logout` mengeset cookie
> dengan `Secure = true`, yang **mengharuskan koneksi HTTPS**. Untuk testing
> lokal dengan HTTP biasa, ubah sementara `Secure = false` di
> `AuthController.SetRefreshTokenCookie`, atau jalankan backend dengan
> profile `https` (`dotnet run --launch-profile https`) — browser modern
> menganggap `localhost` sebagai secure context meski diakses lewat HTTP.

---

## 6. Testing

```bash
# Unit test (mock semua repository/service, tidak butuh database)
dotnet test tests/EPMS.UnitTests

# Integration test (butuh Docker daemon aktif — pakai Testcontainers.PostgreSql
# untuk spin up PostgreSQL asli secara otomatis per test run)
dotnet test tests/EPMS.IntegrationTests
```

---

## 7. RBAC — Ringkasan Role & Policy

| Role | Bisa apa |
|---|---|
| **Admin** | Semua endpoint, termasuk `UsersController` (kelola user) |
| **Manager** | Create/Update/Delete Project, kelola Task |
| **Employee** | Lihat Project & Task, update status/assign task |

Policy didefinisikan di `src/EPMS.WebApi/Authorization/PolicyNames.cs` dan
didaftarkan di `Program.cs`. Endpoint publik tanpa autentikasi hanya:
`POST /api/v1/auth/login`, `register`, `refresh`, `logout`.

---

## 8. Endpoint Utama

| Method | Endpoint | Auth |
|---|---|---|
| POST | `/api/v1/auth/register` | Publik |
| POST | `/api/v1/auth/login` | Publik |
| POST | `/api/v1/auth/refresh` | Publik (via cookie) |
| POST | `/api/v1/auth/logout` | Publik (via cookie) |
| GET/POST/PUT/DELETE | `/api/v1/users` | Admin only |
| GET/POST/PUT/DELETE | `/api/v1/projects` | Authenticated (write: Admin/Manager) |
| GET/POST/PUT/PATCH/DELETE | `/api/v1/projects/{id}/tasks`, `/api/v1/tasks/{id}` | Authenticated |
| GET/POST/DELETE | `/api/v1/tasks/{id}/attachments`, `/api/v1/attachments/{id}` | Authenticated |
| GET | `/api/v1/projects/{id}/activity-logs`, `/api/v1/tasks/{id}/activity-logs` | Authenticated |

Dokumentasi interaktif lengkap tersedia di Swagger UI (`/swagger`) setelah
API berjalan.

---

## 9. Catatan Keamanan yang Sudah Diterapkan

- Password di-hash dengan **BCrypt**, work factor 12 (di atas minimum 10 sesuai spec).
- Access token JWT umur pendek (15 menit default), refresh token random
  string di-hash (SHA-256) sebelum disimpan, dengan **rotation** setiap kali
  refresh (token lama otomatis di-revoke).
- Refresh token dikirim ke client lewat **HttpOnly Cookie**, tidak pernah
  lewat response body atau localStorage.
- Endpoint `register` publik **selalu** membuat user dengan role `Employee`,
  terlepas dari payload yang dikirim — mencegah self-elevation ke Admin.
  Pembuatan user dengan role lain hanya lewat `UsersController` (Admin only).
- Rate limiting: 100 req/menit/IP global, 10 req/menit/IP khusus endpoint Auth
  (mitigasi brute-force login).
- Soft delete konsisten via global query filter EF Core — data terhapus
  tidak pernah hard-delete kecuali via cascade FK yang disengaja.

## 10. Yang Masih Perlu Anda Lengkapi/Putuskan

- Generate `Jwt:Secret` & `JWT_SECRET` production yang sungguh-sungguh random
  (`openssl rand -base64 48`), simpan di secret manager, bukan di git.
- Sesuaikan `Cors:AllowedOrigins` dengan domain frontend production Anda.
- `RequestSizeLimit` di `AttachmentsController` (10 MB) sengaja disamakan
  dengan `FileStorageSettings:MaxFileSizeBytes` — ubah keduanya bersamaan jika
  ingin limit berbeda.
- Implementasi `S3FileStorageService` (ganti `LocalFileStorageService`) saat
  siap pindah ke AWS S3 — interface `IFileStorageService` sudah didesain
  untuk itu, tidak perlu mengubah Application/Domain layer.

