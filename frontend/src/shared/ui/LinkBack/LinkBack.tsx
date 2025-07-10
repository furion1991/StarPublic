'use client'

import { useRouter } from 'next/navigation'
import cn from 'classnames'

import classes from './LinkBack.module.scss'

type LinkBackProps = {
  className?: string
}

export const LinkBack = ({ className }: LinkBackProps) => {
  const router = useRouter()

  return (
    <button type='button' className={cn(classes.linkBack, className)} onClick={() => router.back()}>
      ‹ Назад
    </button>
  )
}
