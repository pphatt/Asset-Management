import axios, { AxiosError, AxiosResponse } from 'axios';
import { toast } from 'react-toastify';
import { getAccessTokenFromCookie } from '@/utils/auth.ts';

// Configuration
const API_URL = '/api';

// Create axios instance
const http = axios.create({
    baseURL: API_URL,
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
    // Important: allow credentials to be sent with requests
    withCredentials: true,
    paramsSerializer: {
        indexes: null, // Prevents array indices in query params
    },
});

// Request interceptor - No need to manually add the token
// since it will be sent automatically in cookies
http.interceptors.request.use(
    (config) => {
        const accessToken = getAccessTokenFromCookie();
        if (accessToken) {
            config.headers['Authorization'] = `Bearer ${accessToken}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Response interceptor - handle common errors
http.interceptors.response.use(
    (response: AxiosResponse) => {
        return response;
    },
    (error: AxiosError) => {
        // Handle common errors
        if (error.response) {
            const { status } = error.response;

            if (status === 401) {
                // Unauthorized - Redirect to login
                console.log('Unauthorized access. Redirecting to login...');
            } else if (status === 403) {
                toast.error('Access denied. You do not have permission to perform this action.');
            } else if (status === 404) {
                toast.error('Resource not found. Please check the URL.');
            }
        } else if (error.request) {
            toast.error('No response from server. Please check your network connection.');
        } else {
            console.error('Error:', error.message);
        }

        return Promise.reject(error);
    }
);

export default http;
