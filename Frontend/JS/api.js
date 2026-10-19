import axios from "https://cdn.jsdelivr.net/npm/axios@1.19.0/+esm";
import { API_URL } from "./config.js";

const api = axios.create({
    baseURL: API_URL
});

export default api;
