'use client'

import Image from 'next/image'

import { useUserStatistics } from '@/shared/hooks'

import classes from './TotalSiteStatistics.module.scss'

export const TotalSiteStatistics = () => {
  const {
    data: { casesOpenedNumber }
  } = useUserStatistics()

  const stats = [
    {
      label: 'Всего пользоватей',
      number: 9780,
      icon: <Image src='/icons/user.svg' width={100} height={100} alt='Пользователь' />
    },
    {
      label: 'Открыто кейсов',
      number: casesOpenedNumber,
      icon: <Image src='/icons/chest.svg' width={100} height={100} alt='Сундук' />
    },
    {
      label: 'Сделано апгрейдов',
      number: 9780,
      icon: <Image src='/icons/upgrade-double.svg' width={100} height={100} alt='Апгрейд' />
    },
    {
      label: 'Создано контрактов',
      number: 9780,
      icon: <Image src='/icons/medal-big.svg' width={100} height={100} alt='Медаль' />
    }
  ]

  return (
    <ul className={classes.totalSiteStatistics}>
      {stats.map(({ label, number, icon }) => {
        const numberWithDot = number ? new Intl.NumberFormat('de-DE').format(number) : null

        return (
          <li key={label}>
            {icon}

            <div className={classes.right}>
              <p>{numberWithDot}</p>
              <p>{label}</p>
            </div>
          </li>
        )
      })}
    </ul>
  )
}
