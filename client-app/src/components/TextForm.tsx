import { ForwardedRef, forwardRef, useEffect, useState } from 'react';

interface InputFormProps {
  placeholderValue: string;
  inputType: string;
  onChange: (value: string) => void;
}

const InputForm = forwardRef((
    {placeholderValue, inputType, onChange}: InputFormProps,
    ref: ForwardedRef<HTMLInputElement>
  ) => {
  const [value, setValue] = useState("");

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setValue(event.target.value);
  }

  useEffect(() => {
    onChange(value);
  }, [value]);

  return (
    <div className="form-group mt-3">
      {
        <input
          className="form-control"
          type={inputType}
          placeholder={placeholderValue}
          onChange={handleChange}
          ref={ref}
        />
      }
    </div>
  );
});

export default InputForm;