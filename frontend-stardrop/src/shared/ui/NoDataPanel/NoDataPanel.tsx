import Image from 'next/image'
import cn from 'classnames'

import classes from './NoDataPanel.module.scss'

type NoDataPanelProps = {
  className?: string
  title: string
  text: string
  action: React.ReactNode
}

export const NoDataPanel = ({ className, title, text, action }: NoDataPanelProps) => {
  return (
    <div className={cn(classes.noDataPanel, className)}>
      <Image src='/icons/exclamation-mark.svg' width={56} height={56} alt='Восклицательный знак' />

      <div className={classes.info}>
        <p className={classes.title}>{title}</p>
        <p className={classes.text}>{text}</p>
      </div>

      <div className={classes.action}>{action}</div>
    </div>
  )
}
