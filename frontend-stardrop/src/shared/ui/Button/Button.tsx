import cn from 'classnames'
import type { ButtonHTMLAttributes } from 'react'

import { LogoLoadingIndicator } from '../LogoLoadingIndicator/LogoLoadingIndicator'

import classes from './Button.module.scss'

type Color = 'pink' | 'purple' | 'cyan'
type BorderRadius = 'normal' | 'medium'

type ButtonProps = {
  color?: Color
  className?: string
  boxShadow?: boolean
  borderRadius?: BorderRadius
  fullWidth?: boolean
  loading?: boolean
} & ButtonHTMLAttributes<HTMLButtonElement>

export const Button = ({
  color = 'pink',
  className,
  boxShadow = false,
  borderRadius = 'normal',
  fullWidth,
  loading = false,
  children,
  ...props
}: ButtonProps) => {
  return (
    <button
      className={cn(classes.button, classes[color], className, {
        [classes.boxShadow]: Boolean(boxShadow),
        [classes.brMedium]: borderRadius === 'medium',
        [classes.fullWidth]: fullWidth
      })}
      disabled={loading}
      {...props}
    >
      {loading ? <LogoLoadingIndicator /> : children}
    </button>
  )
}
