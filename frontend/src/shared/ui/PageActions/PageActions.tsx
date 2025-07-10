import Image from 'next/image'
import cn from 'classnames'

import { LinkBack } from '../LinkBack/LinkBack'

import classes from './PageActions.module.scss'

type PageActionsProps = {
  className?: string
}

export const PageActions = ({ className }: PageActionsProps) => {
  return (
    <div className={cn(classes.pageActions, className)}>
      <LinkBack className={classes.linkBack} />

      <button type='button' className={classes.soundBtn}>
        <Image src='/icons/sound.svg' width={26} height={25} alt='Звук' />
      </button>
    </div>
  )
}
