# syntax=docker/dockerfile:1

# ===== Stage 1: Build =====
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files dulu untuk caching layer install dependency
COPY package.json package-lock.json ./
RUN npm ci

# Copy seluruh source code lalu build
COPY . .

# VITE_API_BASE_URL di-bake ke dalam bundle JS saat build (Vite env var
# sifatnya build-time, bukan runtime). Bisa di-override lewat --build-arg.
ARG VITE_API_BASE_URL=http://localhost:8080/api/v1
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

RUN npm run build

# ===== Stage 2: Serve dengan Nginx =====
FROM nginx:1.27-alpine AS runtime

# Hapus default config Nginx, pakai config custom (lihat nginx.conf)
RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy hasil build Vite (folder dist) ke folder yang di-serve Nginx
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 5173

CMD ["nginx", "-g", "daemon off;"]
