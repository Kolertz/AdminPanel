import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { env } from 'process';

// ������� ������ ��� HTTPS-������������
const baseConfig = {
    plugins: [react()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        host: '0.0.0.0',
        proxy: {
            '^/weatherforecast': {
                target: env.ASPNETCORE_HTTPS_PORT
                    ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
                    : env.ASPNETCORE_URLS
                        ? env.ASPNETCORE_URLS.split(';')[0]
                        : 'https://localhost:5001',
                secure: false
            }
        },
        port: 5173
    }
};

// ��������� HTTPS ������ � development-������
export default defineConfig(({ command }) => {
    if (command === 'serve') {
        // ��������� ���������� - ������� �������� HTTPS
        try {
            const fs = require('fs');
            const path = require('path');

            const baseFolder = env.APPDATA
                ? `${env.APPDATA}/ASP.NET/https`
                : `${env.HOME}/.aspnet/https`;

            const certName = "AdminPanelFront";
            const certPath = path.join(baseFolder, `${certName}.pem`);
            const keyPath = path.join(baseFolder, `${certName}.key`);

            if (fs.existsSync(certPath) && fs.existsSync(keyPath)) {
                return {
                    ...baseConfig,
                    server: {
                        ...baseConfig.server,
                        https: {
                            key: fs.readFileSync(keyPath),
                            cert: fs.readFileSync(certPath)
                        }
                    }
                };
            }
        } catch (e) {
            console.warn('HTTPS certificates not found, falling back to HTTP');
        }
    }

    // Production ��� fallback ��� ����������
    return baseConfig;
});