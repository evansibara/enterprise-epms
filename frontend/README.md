# EPMS — Frontend (React + Vite + TypeScript)

Scaffold awal frontend untuk **Enterprise Project Management System**, dibangun
mengikuti `enterprise-epms-spec.md` (section 3.2: Feature-Sliced Design, React
Query untuk server state, Zustand untuk client state).

## 1. Instalasi

```bash
cd epms-frontend
npm install
cp .env.example .env   # sesuaikan VITE_API_BASE_URL dengan backend kamu
npm run dev
```

Backend belum dipilih di spesifikasi (Golang Fiber atau ASP.NET Core) — pastikan
`VITE_API_BASE_URL` mengarah ke base URL backend yang nanti kamu pilih.

## 2. Struktur Folder (Feature-Sliced Design)

```
src/
├── app/            # Setup aplikasi: providers, router, global CSS
│   ├── App.tsx
│   └── providers/  # QueryClientProvider, BrowserRouter, dst.
│
├── pages/          # Satu file = satu route, hanya komposisi widget/feature
│   ├── auth/
│   ├── dashboard/
│   ├── projects/
│   └── tasks/
│
├── widgets/        # Blok UI komposit lintas-feature (Navbar, Sidebar, KanbanBoard)
│
├── features/       # Logika bisnis per fitur (auth, project-management,
│   │                  task-management, rbac) — masing-masing punya api/, model/, ui/
│   ├── auth/
│   ├── project-management/
│   ├── task-management/
│   └── rbac/
│
├── entities/        # Tipe & model domain murni (User, Project, Task)
│
└── shared/           # Kode reusable lintas seluruh app
    ├── api/          # axios instance + interceptor JWT
    ├── ui/           # Button, Input, Card, Badge, AppLayout
    └── lib/          # util (cn, dll)
```

**Aturan dependency FSD:** layer di atas hanya boleh import dari layer di
bawahnya (pages → widgets → features → entities → shared), tidak sebaliknya.

## 3. Apa yang sudah ada

- [x] Routing dasar + route guard berbasis role (`ProtectedRoute`, sesuai RBAC
      Admin/Manager/Employee di section 4.2 spesifikasi)
- [x] Auth flow: store sesi (Zustand), API client (axios + interceptor JWT),
      form login (react-hook-form + zod)
- [x] Layout aplikasi (Sidebar + Navbar) untuk halaman yang sudah login
- [x] Dashboard, daftar proyek, detail proyek, dan papan Kanban (struktur +
      query React Query, belum drag-and-drop)
- [x] Tailwind + design token dasar, komponen UI primitif (Button, Input,
      Card, Badge)
- [x] TypeScript strict mode aktif (`tsconfig.json`)

## 4. Langkah selanjutnya (sesuai urutan fitur di spesifikasi)

1. **Tentukan bahasa backend** (Go Fiber atau ASP.NET Core) — spesifikasi
   section 2 masih menandainya "tunggu instruksi". Ini menentukan bentuk
   endpoint & DTO yang dikonsumsi frontend.
2. **Lengkapi `RegisterForm`** dengan pola yang sama seperti `LoginForm.tsx`.
3. **Implementasikan refresh-token rotation** di `axios-instance.ts` (saat ini
   baru placeholder yang logout otomatis saat 401).
4. **Tambahkan drag-and-drop** ke `KanbanBoard.tsx` — disarankan `@dnd-kit/core`
   karena ringan dan accessible; saat drop, panggil
   `taskApi.updateStatus(taskId, newStatus)`.
5. **Search, filter, pagination** di `ProjectListPage` — sambungkan input
   pencarian ke query params `projectApi.list({ search, status, page })`.
6. **File attachment upload** pada Task Detail (belum dibuat) — validasi MIME
   type & ukuran di sisi frontend sebagai lapisan pertama, backend tetap wajib
   validasi ulang (section 4.5).
7. **Activity log widget** di `ProjectDetailPage` (section 4.6).
8. **Setup Docker** — tambahkan `Dockerfile` multi-stage (build → nginx) dan
   masukkan service `frontend` ke `docker-compose.yml` bersama backend +
   PostgreSQL + Redis.
9. **CI pipeline** (GitHub Actions) — job `lint` → `build` → `docker build`
   pada setiap PR, sesuai section 5.

## 5. Konvensi commit

Gunakan **Conventional Commits**:

```
feat: tambah halaman kanban board
fix: perbaiki validasi form login
chore: setup eslint & prettier
```
