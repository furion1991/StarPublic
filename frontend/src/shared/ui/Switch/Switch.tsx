'use client'

import { FieldValues, RegisterOptions, useFormContext } from 'react-hook-form'
import cn from 'classnames'

import classes from './Switch.module.scss'

type SwitchProps = {
  name: string
  rules?: RegisterOptions<FieldValues, string>
  className?: string
}

export const Switch = ({ name, rules, className }: SwitchProps) => {
  const { register } = useFormContext()

  return (
    <label className={cn(classes.switch, className)}>
      <input type='checkbox' {...register(name, rules)} defaultChecked />

      <span />
    </label>
  )
}
