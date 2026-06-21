<div align="center">
  <h1>Enterprise EPMS</h1>
  <p><strong>Employee Performance Management System</strong></p>

  [![Backend CI](https://github.com/evansibara/enterprise-epms/actions/workflows/backend.yml/badge.svg)](https://github.com/evansibara/enterprise-epms/actions/workflows/backend.yml)
  [![Frontend CI](https://github.com/evansibara/enterprise-epms/actions/workflows/frontend.yml/badge.svg)](https://github.com/evansibara/enterprise-epms/actions/workflows/frontend.yml)
  <br />
</div>

<hr />

## 📖 Tentang Proyek

**Enterprise EPMS** adalah Sistem Manajemen Kinerja Karyawan berbasis *full-stack* yang dikembangkan untuk kebutuhan skala perusahaan (Enterprise). Aplikasi ini dirancang menggunakan *Clean Architecture* dan *Feature-Sliced Design* untuk memastikan stabilitas, skalabilitas, serta kemudahan dalam pemeliharaan jangka panjang.

## 🏗️ Arsitektur & Teknologi

Proyek ini dibangun dengan memisahkan *backend* (API), *frontend* (Web Client), dan *infrastructure* (Deployment) untuk memungkinkan pengembangan dan penskalaan secara independen.

### 🎨 Frontend (`/frontend`)
*Single Page Application* (SPA) dengan antarmuka yang modern, reaktif, dan sangat interaktif.
*   **Core:** React 18, TypeScript, Vite
*   **State Management:** Zustand (Global), React Query (Server State)
*   **Styling:** Tailwind CSS, clsx, tailwind-merge
*   **Utility & Forms:** React Hook Form, Zod, @dnd-kit/core, Lucide React

### ⚙️ Backend (`/backend`)
RESTful API tangguh berbasis .NET, diimplementasikan dengan standar pengembangan *enterprise*.
*   **Framework:** .NET 8 / ASP.NET Core Web API
*   **Arsitektur:** Clean Architecture / Layered Architecture
*   **Database ORM:** Entity Framework Core (PostgreSQL)
*   **Keamanan:** Autentikasi berbasis JWT (JSON Web Token)

### 🐳 Infrastruktur (`/infrastructure`)
Ekosistem pengembangan dan *deployment* yang sepenuhnya diisolasi dalam *container*.
*   **Datastore:** PostgreSQL 16, Redis 7 (Caching)
*   **Containerization:** Docker & Docker Compose
*   **CI/CD Pipeline:** GitHub Actions untuk otomatisasi pengujian dan pembangunan

---

## 📂 Struktur Repositori

```text
enterprise-epms/
├── backend/                # Source code .NET 8 API (Domain, Application, Infrastructure, WebApi)
├── frontend/               # Source code React + Vite dengan pola Feature-Sliced Design
├── infrastructure/         # Konfigurasi Docker Compose dan Environment Services
└── .github/workflows/      # Konfigurasi CI/CD Pipeline (GitHub Actions)
```

---

## 🚀 Panduan Memulai Cepat (*Quick Start*)

### Persyaratan Lingkungan
*   [Docker Desktop](https://www.docker.com/products/docker-desktop)
*   [Node.js](https://nodejs.org/) (Direkomendasikan v20+)
*   [.NET 8 SDK](https://dotnet.microsoft.com/download)

### 1. Konfigurasi Awal
Gandakan (*copy*) seluruh templat lingkungan (*environment*) menjadi file yang siap digunakan:

```bash
cp infrastructure/.env.example infrastructure/.env
cp backend/.env.example backend/.env
cp frontend/.env.example frontend/.env
```
*(Sesuaikan variabel di dalam file `.env` jika diperlukan).*

### 2. Menjalankan Aplikasi via Docker (Direkomendasikan)
Metode termudah untuk menjalankan seluruh ekosistem (Frontend, Backend, DB, Redis) hanya dengan satu perintah:

```bash
cd infrastructure
docker-compose up -d --build
```
*   **Frontend Web:** Akses melalui [http://localhost:5173](http://localhost:5173)
*   **Backend API:** Akses melalui [http://localhost:8080](http://localhost:8080)

### 3. Menjalankan Mode Pengembangan (*Development*) Secara Lokal
Jika Anda ingin melakukan proses *debugging* secara langsung:

**A. Jalankan Infrastruktur Dasar:**
```bash
cd infrastructure
docker-compose up -d postgres redis
```

**B. Jalankan Backend (.NET):**
```bash
cd backend
dotnet restore
dotnet run --project src/EPMS.WebApi/EPMS.WebApi.csproj
```

**C. Jalankan Frontend (React):**
```bash
cd frontend
npm install
npm run dev
```

---

## 🧪 Pengujian (*Testing*) & CI/CD
Proyek ini mengadopsi integrasi berkelanjutan (CI). Setiap *Push* atau *Pull Request* ke cabang utama akan diuji secara otomatis oleh GitHub Actions.

Untuk menjalankan pengujian secara manual:
*   **Backend (Unit & Integration Tests):** `cd backend && dotnet test`
*   **Frontend (Linting):** `cd frontend && npm run lint`

---

## 📄 Lisensi
Hak cipta © dilindungi. Proyek ini bersifat *proprietary* dan ditujukan untuk operasional tingkat perusahaan.
