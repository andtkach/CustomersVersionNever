# Build stage
FROM node:20-alpine AS build

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy source files
COPY . .

# Build the application
RUN npm run build

# Production stage
FROM nginx:alpine

# Copy built files to nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy environment variable script
COPY env.sh /docker-entrypoint.d/40-generate-env.sh

# Make script executable
RUN chmod +x /docker-entrypoint.d/40-generate-env.sh

# Expose port 80
EXPOSE 80

# The env.sh script will generate env-config.js and start nginx
CMD ["/docker-entrypoint.d/40-generate-env.sh"]
