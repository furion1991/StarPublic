'use client'

import ReactCodeInput from 'react-verification-input'
import { Controller, useFormContext } from 'react-hook-form'

import classes from './CodeField.module.scss'

type CodeFieldProps = {
  name: string
  onComplete?: (code: string) => void
}

export const CodeField = ({ name, onComplete }: CodeFieldProps) => {
  const { control } = useFormContext()

  return (
    <Controller
      name={name}
      control={control}
      render={({ field: { value, onChange } }) => {
        return (
          <ReactCodeInput
            classNames={{
              container: classes.container,
              character: classes.character,
              characterInactive: classes.characterInactive,
              characterSelected: classes.characterSelected,
              characterFilled: classes.characterFilled
            }}
            autoFocus
            placeholder=''
            validChars='0-9'
            value={value}
            onChange={onChange}
            onComplete={onComplete}
          />
        )
      }}
    />
  )
}
