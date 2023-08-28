import { useEffect, useState } from "react";

interface SelectTimeProps {
    title: string;
    onChange: (days: number) => void;
}

function SelectTime({title, onChange}: SelectTimeProps) {
  const [selectedDaysValue, setSelectedDaysValue] = useState(1);

  const handleChange = (event: any) => {
    setSelectedDaysValue(event.target.value);
  }

  useEffect(() => {
    onChange(selectedDaysValue)
  }, [selectedDaysValue]);
  
  return (
      <>
        <label className="form-label">{title}</label>
        <select className="form-select" value={selectedDaysValue} onChange={handleChange}>
          <option value="1">One day</option>
          <option value="2">Two days</option>
          <option value="3">Three days</option>
          <option value="4">Four days</option>
          <option value="5">Five days</option>
          <option value="6">Six days</option>
          <option value="7">Whole week</option>
        </select>
      </>
  )
}

export default SelectTime;