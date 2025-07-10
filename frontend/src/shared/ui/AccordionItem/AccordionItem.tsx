'use client'

import { useState } from 'react'
import Image from 'next/image'
import cn from 'classnames'

import classes from './AccordionItem.module.scss'

type AccordionItemProps = {
  className?: string
  openClassName?: string
  heading: string | React.ReactNode
  children: React.ReactNode
}

export const AccordionItem = ({
  className,
  openClassName,
  heading,
  children
}: AccordionItemProps) => {
  const [isOpen, setOpen] = useState(false)

  const toggleOpen = () => {
    setOpen((isOpen) => !isOpen)
  }

  return (
    <div className={cn(classes.accordionItem, className)}>
      <div
        className={cn(classes.accordionHeading, {
          [classes.open]: isOpen
        })}
        onClick={toggleOpen}
      >
        {typeof heading === 'string' ? <p>{heading}</p> : heading}

        <button type='button' className={classes.accordionToggler}>
          <Image src='/icons/arrow-down-gray.svg' width={16} height={6} alt='Стрелка' />
        </button>
      </div>

      <div
        className={cn(
          classes.accordionContent,
          {
            [classes.open]: isOpen
          },
          openClassName
            ? {
                [openClassName]: isOpen
              }
            : null
        )}
      >
        <div className={classes.content}>{children}</div>
      </div>
    </div>
  )
}
