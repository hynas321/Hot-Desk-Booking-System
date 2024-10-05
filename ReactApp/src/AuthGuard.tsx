import { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { TokenManager } from "./managers/TokenManager";

interface AuthGuardProps {
  children: ReactNode;
}

const AuthGuard = ({ children }: AuthGuardProps) => {
  const token = TokenManager.getToken();

  if (!token) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};

export default AuthGuard;
