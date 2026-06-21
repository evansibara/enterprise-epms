# Enterprise EPMS (Sistem Manajemen Kinerja Karyawan)

![Backend CI](https://github.com/USERNAME/REPO/actions/workflows/backend.yml/badge.svg)
![Frontend CI](https://github.com/USERNAME/REPO/actions/workflows/frontend.yml/badge.svg)

*(Catatan: Sesuaikan `USERNAME/REPO` dengan link repository GitHub Anda)*

Enterprise EPMS adalah Sistem Manajemen Kinerja Karyawan (Employee Performance Management System) kelas *enterprise* secara *full-stack*. Aplikasi ini menyediakan seperangkat alat yang komprehensif untuk mengelola kinerja karyawan, dirancang dengan arsitektur modern dan standar industri terbaik.

## 🏗 Arsitektur & Teknologi

Proyek ini dibangun menggunakan arsitektur modern yang terpisah (*decoupled*), dibagi menjadi lapisan *frontend*, *backend*, dan *infrastructure*.

### 🎨 Frontend (`/frontend`)
*Frontend* pada proyek ini merupakan *Single Page Application* (SPA) yang tangguh yang dibangun menggunakan React dan teknologi web modern untuk memastikan pengalaman pengguna yang responsif dan sangat interaktif.
- **Inti Utama:** React 18, TypeScript, Vite
- **Desain & Gaya (Styling):** Tailwind CSS, clsx, tailwind-merge
- **Manajemen State:** Zustand (State Global), React Query (State Server & Pengambilan Data)
- **Routing:** React Router v6
- **Formulir & Validasi:** React Hook Form, Zod
- **Drag & Drop:** @dnd-kit/core
- **Ikon & Utilitas:** Lucide React, date-fns

### ⚙️ Backend (`/backend`)
*Backend* menggunakan RESTful API berperforma tinggi yang dikembangkan menggunakan bahasa C# dengan kerangka kerja .NET (ASP.NET Core).
- **Framework:** .NET / ASP.NET Core
- **Arsitektur:** Arsitektur Berlapis (*Layered Architecture*) / Arsitektur Bersih (*Clean Architecture*) (Berdasarkan pola standar *enterprise* .NET)
- **Autentikasi:** JWT (JSON Web Tokens)
- **Akses Database:** Entity Framework Core (terhubung ke PostgreSQL)

### 🐳 Infrastruktur (`/infrastructure`)
Deployment yang dikemas (*containerized*) dan konfigurasi infrastruktur menggunakan Docker.
- **Database:** PostgreSQL 16
- **Caching:** Redis 7
- **Kontainerisasi:** Docker & Docker Compose
- **Jaringan:** Jaringan *bridge* Docker yang terisolasi

### 🔄 CI/CD Pipeline (`/.github/workflows`)
Repositori ini telah dilengkapi dengan *Continuous Integration* menggunakan **GitHub Actions** untuk memastikan stabilitas kode dan mencegah kerentanan sebelum digabungkan ke cabang utama (*main branch*):
- **Backend CI:** Mengotomatiskan proses `Restore`, `Build`, dan pengujian `Unit/Integration Tests` untuk *Push* dan *Pull Request* di lingkungan .NET 8.
- **Frontend CI:** Mengotomatiskan proses pengecekan kualitas kode (*Linting*), instalasi dependensi, dan `Build` aplikasi untuk *Push* dan *Pull Request* di lingkungan Node.js.

## 📂 Struktur Proyek

```text
enterprise-epms/
├── backend/                # Source code .NET Core Web API
│   ├── src/                # Source code aplikasi utama
│   ├── tests/              # Pengujian unit dan integrasi (Unit & integration tests)
│   ├── Dockerfile          # Konfigurasi container untuk Backend
│   └── EPMS.sln            # File Solution .NET
├── frontend/               # Source code React / Vite frontend
│   ├── src/                # Komponen UI, halaman (pages), dan hooks
│   ├── Dockerfile          # Konfigurasi container untuk Frontend
│   └── package.json        # Dependensi Node.js
└── infrastructure/         # Deployment dan infrastruktur
    ├── docker-compose.yml  # Konfigurasi layanan (Postgres, Redis, Backend, Frontend)
    └── .env.example        # Template variabel lingkungan (Environment variables)
```

## 🚀 Memulai Proyek

### Persyaratan Awal (Prerequisites)
Pastikan Anda telah menginstal perangkat lunak berikut di mesin Anda:
- [Docker](https://www.docker.com/products/docker-desktop) dan Docker Compose
- [Node.js](https://nodejs.org/) (Direkomendasikan v20 ke atas)
- [.NET SDK](https://dotnet.microsoft.com/download) (untuk pengembangan *backend* secara lokal)

### 1. Pengaturan Lingkungan (Environment Setup)
Anda perlu mengatur variabel lingkungan sebelum menjalankan aplikasi.

```bash
# Mengatur environment infrastruktur
cp infrastructure/.env.example infrastructure/.env

# Mengatur environment backend
cp backend/.env.example backend/.env

# Mengatur environment frontend
cp frontend/.env.example frontend/.env
```
*Catatan: Pastikan untuk memeriksa file `.env` dan perbarui kunci rahasia (secret keys) atau kredensial database jika diperlukan.*

### 2. Menjalankan via Docker Compose (Direkomendasikan)
Cara termudah untuk menjalankan seluruh aplikasi adalah melalui Docker Compose.

```bash
cd infrastructure
docker-compose up -d --build
```

Perintah ini akan menjalankan:
- **PostgreSQL Database** di `localhost:5432`
- **Redis Cache** di `localhost:6379`
- **Backend API** di `localhost:8080`
- **Frontend App** di `localhost:5173`

Anda dapat mengakses aplikasi *frontend* dengan membuka `http://localhost:5173` di browser Anda.

### 3. Menjalankan Lingkungan Lokal (Tanpa Docker untuk Aplikasi)
Jika Anda ingin menjalankan *backend* dan *frontend* secara lokal untuk keperluan pengembangan (*development*):

**Jalankan Layanan Infrastruktur:**
```bash
cd infrastructure
# Hanya jalankan database dan redis
docker-compose up -d postgres redis
```

**Jalankan Backend:**
```bash
cd backend
dotnet restore
dotnet run --project src/EPMS.WebApi/EPMS.WebApi.csproj
```

**Jalankan Frontend:**
```bash
cd frontend
npm install
npm run dev
```

## 🧪 Menjalankan Pengujian (Testing)
- **Backend:** `cd backend && dotnet test`
- **Frontend:** Periksa `frontend/package.json` untuk perintah pengujian tertentu.

## 📄 Lisensi
Proyek ini bersifat hak milik (proprietary) dan ditujukan untuk penggunaan tingkat perusahaan (enterprise).
