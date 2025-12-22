# Build stage
FROM node:24-alpine AS build

WORKDIR /app

# Copy package files
COPY Web/package*.json ./

# Install dependencies
RUN npm ci

# Copy source files
COPY Web/ .

# Build the application
RUN npm run build

# Production stage
FROM nginx:alpine

# Install dos2unix for line ending conversion
RUN apk add --no-cache dos2unix

# Copy built files to nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx configuration
COPY Web/nginx.conf /etc/nginx/conf.d/default.conf

# Copy environment variable script
COPY Web/env.sh /docker-entrypoint.d/40-generate-env.sh

# Convert line endings and make script executable
RUN dos2unix /docker-entrypoint.d/40-generate-env.sh && chmod +x /docker-entrypoint.d/40-generate-env.sh

# Expose port 8080
EXPOSE 8080

# The env.sh script will generate env-config.js and start nginx
#CMD ["/docker-entrypoint.d/40-generate-env.sh"]
