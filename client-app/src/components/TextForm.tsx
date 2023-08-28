import { ForwardedRef, forwardRef, useEffect, useState } from 'react';

interface TextFormProps {
  placeholderValue: string;
  onChange: (value: string) => void;
  onKeyDown?: (value: string, key: string) => void;
}

const InputForm = forwardRef((
    {placeholderValue, onChange, onKeyDown}: TextFormProps,
    ref: ForwardedRef<HTMLInputElement>
  ) => {
  const [value, setValue] = useState("");

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setValue(event.target.value);
  }

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (onKeyDown != null) {
      onKeyDown(value, event.key);
    }
  }

  useEffect(() => {
    onChange(value);
  }, [value]);

  return (
    <div className="form-group mt-3">
      {
        <input
          className="form-control"
          placeholder={placeholderValue}
          onChange={handleChange}
          onKeyDown={handleKeyDown}
          ref={ref}
        />
      }
    </div>
  );
});

export default InputForm;