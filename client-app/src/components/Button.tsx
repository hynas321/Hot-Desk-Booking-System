import { ReactNode } from "react";

interface ButtonProps {
  text: string;
  active: boolean;
  spacing: number;
  type?: string;
  icon?: ReactNode;
  onClick?: () => void;
}

function Button({text, active, spacing, type, icon, onClick}: ButtonProps) {
  return (
    <>
      <button 
        className={
          active ?
          `btn btn-${type === undefined ? "primary" : type} mx-${spacing}` :
          `btn btn-${type === undefined ? "primary" : type} mx-${spacing} disabled`
        } 
        onClick={onClick}
      > 
        <span>{icon} {text}</span> 
      </button>
    </>
  );
}

export default Button;