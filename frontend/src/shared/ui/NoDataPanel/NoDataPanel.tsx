import Image from 'next/image'

import classes from './NoDataPanel.module.scss'

type NoDataPanelProps = {
  title: string
  text: string
  action: React.ReactNode
}

export const NoDataPanel = ({ title, text, action }: NoDataPanelProps) => {
  return (
    <div className={classes.noDataPanel}>
      <Image
        src='/icons/exclamation-mark.svg'
        width={56}
        height={55.23}
        alt='Восклицательный знак'
      />

      <div className={classes.info}>
        <p className={classes.title}>{title}</p>
        <p className={classes.text}>{text}</p>
      </div>

      <div className={classes.action}>{action}</div>
    </div>
  )
}
