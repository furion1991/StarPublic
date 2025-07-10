'use client'

import { useRouter } from 'next/navigation'
import cn from 'classnames'
import type { RefObject } from 'react'

import classes from './LinkBack.module.scss'

type LinkBackProps = {
  className?: string
  style?: React.CSSProperties
  ref?: RefObject<HTMLButtonElement | null>
}

export const LinkBack = ({ className, style, ref }: LinkBackProps) => {
  const router = useRouter()

  return (
    <button
      ref={ref}
      type='button'
      className={cn(classes.linkBack, className)}
      style={style}
      onClick={() => router.back()}
    >
      ‹ Назад
    </button>
  )
}
