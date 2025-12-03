// Support runtime environment variables from Docker
declare global {
  interface Window {
    _env_?: {
      VITE_DATA_API_URL?: string;
      VITE_AUTH_API_URL?: string;
      VITE_STORAGE_API_URL?: string;
    };
  }
}

export const env = {
  dataApiUrl: window._env_?.VITE_DATA_API_URL || import.meta.env.VITE_DATA_API_URL || "",
  authApiUrl: window._env_?.VITE_AUTH_API_URL || import.meta.env.VITE_AUTH_API_URL || "",
  storageApiUrl: window._env_?.VITE_STORAGE_API_URL || import.meta.env.VITE_STORAGE_API_URL || "",
}
