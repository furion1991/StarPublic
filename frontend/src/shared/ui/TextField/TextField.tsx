'use client'

import { type FieldValues, type RegisterOptions, useFormContext } from 'react-hook-form'
import { useState, type InputHTMLAttributes } from 'react'
import cn from 'classnames'
import Image from 'next/image'

import classes from './TextField.module.scss'

type TextFieldProps = {
  name: string
  rules?: RegisterOptions<FieldValues, string>
  endAdornment?: React.ReactNode
} & InputHTMLAttributes<HTMLInputElement>

export const TextField = ({
  name,
  rules = {},
  className,
  type,
  endAdornment,
  ...props
}: TextFieldProps) => {
  const [isFieldValueVisible, setFieldValueVisible] = useState(false)
  const { register, getFieldState } = useFormContext()

  const { error } = getFieldState(name)

  return (
    <div
      className={cn(classes.container, className, {
        [classes.hasError]: Boolean(error)
      })}
    >
      <input
        type={isFieldValueVisible ? 'text' : type}
        className={cn(classes.textField)}
        {...props}
        {...register(name, rules)}
      />

      {type === 'password' || endAdornment ? (
        <>
          {type === 'password' ? (
            <button
              type='button'
              className={cn(classes.toggleVisibilityBtn, {
                [classes.eyeCross]: !isFieldValueVisible
              })}
              onClick={() => {
                setFieldValueVisible(!isFieldValueVisible)
              }}
            >
              <Image src='/icons/eye.svg' width={29} height={19} alt='глаз' />
            </button>
          ) : null}

          {endAdornment}
        </>
      ) : null}
    </div>
  )
}
