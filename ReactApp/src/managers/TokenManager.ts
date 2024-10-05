export class TokenManager {
  private static tokenKey = "token";

  static setToken(token: string): void {
    sessionStorage.setItem(this.tokenKey, token);
  }

  static getToken(): string | null {
    return sessionStorage.getItem(this.tokenKey);
  }

  static clearToken(): void {
    sessionStorage.removeItem(this.tokenKey);
  }
}
