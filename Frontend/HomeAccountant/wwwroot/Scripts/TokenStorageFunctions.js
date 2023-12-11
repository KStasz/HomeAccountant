const tokenKey = 'jwtToken';

function getToken() {
    return window.localStorage.getItem(tokenKey);
}

function setToken(value) {
    window.localStorage.setItem(tokenKey, value);
}

function clearToken() {
    window.localStorage.clear();
}

function removeToken() {
    window.localStorage.removeItem(tokenKey);
}