'use client'

import Image from 'next/image'

import { useSignalrData } from '@/shared/hooks'

import classes from './CurrentOnlineNumber.module.scss'

export const CurrentOnlineNumber = () => {
  const {
    data: { usersOnlineNumber }
  } = useSignalrData()

  return (
    <div className={classes.currentOnlineNumber}>
      <Image src='/icons/network.svg' width={23.92} height={18.23} alt='сеть' priority />

      <div className={classes.right}>
        <p>{usersOnlineNumber}</p>
        <p>Online</p>
      </div>
    </div>
  )
}
