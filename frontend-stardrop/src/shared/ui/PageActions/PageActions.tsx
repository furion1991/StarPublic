import Image from 'next/image'
import cn from 'classnames'
import type { RefObject } from 'react'

import { LinkBack } from '../LinkBack/LinkBack'

import classes from './PageActions.module.scss'

type PageActionsProps = {
  className?: string
  soundBtnRef?: RefObject<HTMLButtonElement | null>
  linkBackRef?: RefObject<HTMLButtonElement | null>
}

export const PageActions = ({ className, soundBtnRef, linkBackRef }: PageActionsProps) => {
  return (
    <div className={cn(classes.pageActions, className)}>
      <LinkBack ref={linkBackRef} className={cn(classes.linkBack, 'shaking')} />

      <button ref={soundBtnRef} type='button' className={cn(classes.soundBtn, 'shaking')}>
        <Image src='/icons/sound.svg' width={26} height={25} alt='Звук' />
      </button>
    </div>
  )
}
