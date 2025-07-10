import Image from 'next/image'

import { IdCopier } from '@/shared/ui'

import classes from './UserMainInfo.module.scss'

type UserMainInfoProps = {
  id: string
  userName: string
}

export const UserMainInfo = ({ id, userName }: UserMainInfoProps) => {
  return (
    <div className={classes.userMainInfo}>
      <div className={classes.avatar}>
        <Image src='/placeholders/avatar-big.png' width={180} height={180} alt='Аватарка' />
      </div>

      <p className={classes.userName}>{userName}</p>

      <div className={classes.userId}>
        <IdCopier id={id} />
      </div>
    </div>
  )
}
