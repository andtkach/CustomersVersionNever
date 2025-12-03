#!/bin/sh

# Generate env-config.js from environment variables at container startup
cat <<EOF > /usr/share/nginx/html/env-config.js
window._env_ = {
  VITE_DATA_API_URL: "${VITE_DATA_API_URL:-}",
  VITE_AUTH_API_URL: "${VITE_AUTH_API_URL:-}",
  VITE_STORAGE_API_URL: "${VITE_STORAGE_API_URL:-}"
};
EOF

echo "Environment configuration generated:"
cat /usr/share/nginx/html/env-config.js

# Start nginx
exec nginx -g 'daemon off;'
