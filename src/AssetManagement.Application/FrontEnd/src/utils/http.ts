import axios, { AxiosError, type AxiosInstance } from "axios";
import { toast } from "react-toastify";
import config from "src/constant/config";

class Http {
  instance: AxiosInstance;
  constructor() {
    this.instance = axios.create({
      baseURL: `${config.baseURL}`,
      timeout: 10000,
      headers: {
        "Content-Type": "application/json",
      },
    });
    this.instance.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        const message = error.message;
        toast.error(message);
      }
    );
  }
}

const http = new Http().instance;

export default http;
