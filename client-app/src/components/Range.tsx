import { useEffect, useState } from "react";

interface RangeProps {
  title: string;
  suffix: string;
  minValue: number;
  maxValue: number;
  step: number;
  defaultValue: number;
  onChange: (value: number) => void;
}

function Range({title, suffix, minValue, maxValue, step, defaultValue, onChange}: RangeProps) {
  const [value, setValue] = useState(defaultValue);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setValue(parseInt(event.target.value));
  }

  useEffect(() => {
    onChange(value);
  }, [value]);
  
  return (
    <div className="d-flex align-items-center">
      <label className="form-label">{`${title}: ${value} ${suffix}`}</label>
      <div className="flex-grow-1">
        <input 
            className="form-range mx-2"
            type="range"
            value={value}
            min={minValue}
            max={maxValue}
            step={step}
            onChange={handleChange}
        />
      </div>
  </div>
  )
}

export default Range;